using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{

    //Grab range is how far the ray cast detects anything || The offset ofsets the ray form the player's centre || Collision is for the gizmo
    [Header("Wall detection")]
    public float grabRange = 1f;
    public Vector3 offset = new Vector3(0f, 0f, 0f);
    Vector3 collision = Vector3.zero;

    [Header("Climb control values")]
    [Tooltip("Not entirely sure why, but climb speed has to be set super low")]
    public float climbSpeed = 0.005f;
    public float dropRange = 0.5f;
    private bool climbing;

    //All the variables that I nab from the player objects
    private TurtleController turtCont;
    private CharacterController charCont;
    private int defaultLayer;
    private int groundLayer;
    private int climbPlayerLayer;

    void Start()
    {
        //Nabs the script that controls the turtle this is on and the character controller.
        turtCont = gameObject.GetComponent<TurtleController>();
        charCont = gameObject.GetComponent<CharacterController>();

        //Nabs the layer numbers for the default, ground, and climberPlayer, just in case it gets messed up by something or someone
        defaultLayer = LayerMask.NameToLayer("Default");
        groundLayer = LayerMask.NameToLayer("Ground");
        climbPlayerLayer = LayerMask.NameToLayer("ClimberPlayer");
    }

    void Update()
    {
        //Checks if the action button from the input manager is hit (Default should be E or A on a controller)
        if (Input.GetButtonDown("action button"))
        {
            if (!climbing)
            {
                var ray = new Ray(transform.position + offset, transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, grabRange))
                {
                    if (hit.transform.gameObject.tag == "climbable")
                    {
                        //Moves the green gizmo sphere to where the raycast hit
                        collision = hit.point;

                        //Disables the character controller, sets its colission to the ClimbingPLayer
                        gameObject.layer = climbPlayerLayer;
                        turtCont.enabled = false;
                        climbing = true;
                    }
                }
            }
            else
            {
                //Casts a ray downwards to check if there's ground before dropping the player off the wall
                var ray = new Ray(transform.position + offset, -transform.up);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, dropRange))
                {
                    if (hit.transform.gameObject.layer == groundLayer)
                    {
                        collision = hit.point;

                        //Disables the character controller, sets its colission to the Default
                        gameObject.layer = defaultLayer;
                        turtCont.enabled = true;
                        climbing = false;
                    }
                }
            }

        }
    }

    //Made this a late update so if the player hits the action button it takes priority over moving
    private void LateUpdate()
    {
        //Uses the character controller to move the player vertically
        if (climbing)
        {
            float hIn = Input.GetAxis("Horizontal");
            float vIn = Input.GetAxis("Vertical");

            Vector3 movDir = new Vector3(hIn, vIn, 0);
            float magnitude = Mathf.Clamp01(movDir.magnitude) * climbSpeed;
            movDir.Normalize();

            charCont.Move(movDir * magnitude);

            //You'd do animations here, or above the controller
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(collision, 0.2f);
    }
}
