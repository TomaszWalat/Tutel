using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleController : MonoBehaviour
{
    [Tooltip("Do not change, this is set by the turtle manager\n" +
        "it shows the order to swap between the turtles")]
    public int characterIndex; 
    [Tooltip("The main camera in the scene to take offset from")]
    public Camera mainCam;
    [Tooltip("The character controller on the same object")]
    public CharacterController controller;
    [Tooltip("Make sure this is a child with the same name \n" +
        "This will search automatically if not assigned")]
    public Transform groundCheck;
    [Tooltip("Enable the input from the player")]
    public bool currentlyActive = true;
    [Tooltip("turn gravity on or off, best used by scripts")]
    public bool useGravity;

    [Space(20)]

    [Tooltip("Movement speed of the character")]
    public float speed = 10f;
    [SerializeField][Tooltip("Mass of the character")]
    public float gravityMul = 1;
    [SerializeField][Tooltip("Distance to check for ground, from \"GroundCheck\" component")]
    public float groundDistance = 0.4f;
    [SerializeField][Tooltip("Place all objects with colliders in this layer too")]
    public LayerMask groundMask;
    [SerializeField][Tooltip("Not exact, due to gravity")]
    public float jumpHeight = 3f;
    [SerializeField][Tooltip("When close to the ground, player has a different gravity \n" +
        "This is a gravity multiplier")]
    public float groundAttraction = 2f;
    [SerializeField][Tooltip("movement is normalized, with no transition")]
    public bool snapMovement;
    [SerializeField][Tooltip("Set this to spawn the player with a given rotation" +
        "Facing direction \n" +
        "0 - -x \n" +
        "1 - +z \n" +
        "2 - +x \n" +
        "3 - -z \n")]
    public int lastFaced; //values 0-3, directions NESW
    [SerializeField][Tooltip("speed multiplier for the facing rotation")]
    public float rotSpeed = 5;

    private Vector3 velocity;
    private bool isGrounded = true;
    private Vector3 facing; //currently faced cardinal
    private Quaternion newDirection; // the new direction to face
    private Quaternion offset = Quaternion.Euler(0, 0, 0); //camera offset for rotation
    private int lastCamera; //last direction the camera faced


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        groundCheck = gameObject.transform.Find("groundCheck").transform;     
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -groundAttraction;
        }

        if (currentlyActive) // we want the player to only move while its the selected turtle
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Move(x, z);
            CheckLastDirection(x, z);

            //jumping logic
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityMul * -9.81f);
            }
        }
        //the rotation of the character should go on even after a switch
        RotateToDirection();

        //disable gravity if the boolean is false, used for scripting
        if (useGravity)
        {
            velocity.y += gravityMul * -9.81f * Time.deltaTime;
        }

        //after all calculations, we move the player object.
        controller.Move(velocity * Time.deltaTime);
        
    }



    void CheckLastDirection(float x, float z)
    {
        if (Mathf.Abs(x) > Mathf.Abs(z)) //we are facing north or south, along the X axis
        {
            if (Mathf.Abs(x) == x) //we are looking towards the positive of X
            {
                lastFaced = 2; //code for south
            }
            else //we are looking towards the negative of x
            {
                lastFaced = 0; // code for north
            }
        }
        else if(Mathf.Abs(x) < Mathf.Abs(z)) //we are facing west or east, along te z axis
        {
            if (Mathf.Abs(z) == z) //we are looking towards the positive of Z
            {
                lastFaced = 1; //code for east
            }
            else //we are looking towards the negative of Z
            {
                lastFaced = 3; // code for west
            }
        }
        //if they are both 0, nothing should happen
    }


    void RotateToDirection()
    {
        lastCamera = CameraFacing();
        //we determine the movement offset based on the camera's facing cardinal
        switch(lastCamera)
        {
            case 0:
                offset.y = -90;
                break;
            case 1:
                offset.y = 0;
                break;
            case 2:
                offset.y = 90;
                break;
            case 3:
                offset.y = 180;
                break;
        }

        switch(lastFaced)
        {
            case 0:
                newDirection = Quaternion.Euler(0, -90 + offset.y, 0);
                foreach(Transform child in transform)
                    child.rotation = Quaternion.Lerp(child.rotation, newDirection, Time.deltaTime * rotSpeed);
                break;
            case 1:
                newDirection = Quaternion.Euler(0, 0 + offset.y, 0);
                foreach (Transform child in transform)
                    child.rotation = Quaternion.Lerp(child.rotation, newDirection, Time.deltaTime * rotSpeed);
                break;
            case 2:
                newDirection = Quaternion.Euler(0, 90 + offset.y, 0);
                foreach (Transform child in transform)
                    child.rotation = Quaternion.Lerp(child.rotation, newDirection, Time.deltaTime * rotSpeed);
                break;
            case 3:
                newDirection = Quaternion.Euler(0, 180 + offset.y, 0);
                foreach (Transform child in transform)
                    child.rotation = Quaternion.Lerp(child.rotation, newDirection, Time.deltaTime * rotSpeed);
                break;
            default:
                break;
        }
    }

    public int CameraFacing()
    {
        //determine the cardinal direction of the camera
        //0 offset fo camera facing N, 1 offset for camera facing East, etc.
        //hypothetically, we will set negative x as the North to more easily understand these comments
        facing = mainCam.transform.forward;

        if (Mathf.Abs(facing.x) > Mathf.Abs(facing.z)) //we are facing north or south, along the X axis
        {
            if(Mathf.Abs(facing.x) == facing.x) //we are looking towards the positive of X
            {
                return 2; //code for south
            }
            else //we are looking towards the negative of x
            {
                return 0; // code for north
            }
        }
        else //we are facing west or east, along te z axis
        {
            if (Mathf.Abs(facing.z) == facing.z) //we are looking towards the positive of Z
            {
                return 1; //code for east
            }
            else //we are looking towards the negative of Z
            {
                return 3; // code for west
            }
        }
    }


    //Player input based movement
    void Move(float x, float z)
    {
        //calculate the resulting vector of the axis
        Vector3 move = transform.right * x + transform.forward * z;
        
        if(snapMovement)
        { move.Normalize(); }

        //depending on result of CameraFacing, rotate them.
        switch(CameraFacing())
        {
            case 0: //camera is facing north (-x)
                Debug.Log("facing North -x");
                move = Quaternion.AngleAxis(-90, Vector3.up) * move;
                break;
            case 1: //camera is facing East (+z)
                Debug.Log("facing East +z");
                //the movement script is made to work on this axis.
                break;
            case 2: //camera is facing South (+x)
                Debug.Log("facing South +x");
                move = Quaternion.AngleAxis(90, Vector3.up) * move;
                break;
            case 3: // camera is facing West (-z)
                Debug.Log("facing West -z");
                move = Quaternion.AngleAxis(180, Vector3.up) * move;
                break;
        }


        controller.Move(move * speed * Time.deltaTime);
    }
}
