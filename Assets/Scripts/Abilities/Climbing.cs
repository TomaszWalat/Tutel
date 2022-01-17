using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : Ability
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
    private Transform wall;

    //All the variables that I nab from the player objects
    private TurtleController turtCont;
    private CharacterController charCont;
    private int defaultLayer;
    private int groundLayer;
    private int climbPlayerLayer;


    void Start()
    {
        //Nabs the script that controls the turtle this is on and the character controller.
        turtCont = transform.parent.gameObject.GetComponent<TurtleController>();
        charCont = transform.parent.gameObject.GetComponent<CharacterController>();

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
                        transform.parent.gameObject.layer = climbPlayerLayer;
                        turtCont.enabled = false;
                        climbing = true;

                        //Makes the player perpendicular to the wall in the Z axis
                        Vector3 difference = hit.transform.position - transform.position;
                        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                        rotZ = Mathf.Round(rotZ / 90) * 90;
                        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                        //Saves the wall transform
                        wall = hit.transform;

                    }
                }
            }
            else
            {
                //Casts a ray downwards to check if there's ground before dropping the player off the wall
                var ray = new Ray(transform.parent.position, -transform.parent.up);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, dropRange))
                {
                    if (hit.transform.gameObject.layer == groundLayer)
                    {
                        collision = hit.point;

                        //Disables the character controller, sets its collision to the Default
                        transform.parent.gameObject.layer = defaultLayer;
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
            float vert = Input.GetAxis("Vertical");
            Vector3 movDir = new Vector3(0, vert, 0);

            if (Input.GetAxis("Horizontal") > 0)
            {
                movDir += wall.forward;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                movDir -= wall.forward;
            }

            
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
