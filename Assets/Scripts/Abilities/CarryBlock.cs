using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarryBlock : MonoBehaviour,Ability
{
    //This is my push script but modified to pick up instead.
    //Grab range is how far the ray cast detects anything || The rayOffset ofsets the ray from the player's centre || The height offset controls how high the block is held above the character || Collision stores where the hit was for the debug gizmo
    [Header("Grab detection")]
    public float grabRange = 2f;
    public Vector3 rayOffset = new Vector3(0f, -0.5f, 0f);
    public Vector3 grabPoint = Vector3.zero;

    [Header("Drop detection")]
    public Vector3 checkBoxSize = new Vector3(1f, 0.5f, 1f);
    Vector3 dropPoint = Vector3.zero;

    [Header("Block control")]
    public float holdHeight = 1f;
    public Vector3 dropHeightOffset = new Vector3(0f, -0.25f, 0);
    bool currentlyCarrying = false;
    string childName = "";
    
    //MAY NEED CHANGES FOR A DIFFERENT MOVEMENT SCRIPT
    [Header("Player control")]
    public bool slowEnabled = true;
    public float percentageSlowdown = 0.5f;
    //Movement movementScript;
    float startingSpeed;
    float startingRotSpeed;

    void Start()
    {
        ////Initialises the variables needed for the slow down
        //movementScript = gameObject.GetComponent<Movement>();
        //startingSpeed = movementScript.speed;
        //startingRotSpeed = movementScript.rotSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if the action button from the input manager is hit (Default should be E or A on a controller) then checks if they're already holding something
        if (Input.GetButtonDown("action button"))
        {
            //Casts a ray to see if something is hit,
            var ray = new Ray(transform.position + rayOffset, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grabRange))
            {
                //Moves the green gizmo sphere to where the raycast hit
                grabPoint = hit.point;

                //Checks if player is carrying anythign then if the block hit has the correct tag
                if (currentlyCarrying == false)
                {
                    if (hit.transform.gameObject.tag == "carryBlock")
                    {
                        Debug.Log("Hit the carry block");
                        //Determines the direction of the player from where the raycast hit
                        //Determines the direction the block should be pushed using the player's direction and the raycast point 
                        Vector3 playerDir = hit.point - transform.position;
                        Vector3 reflection = Vector3.Reflect(playerDir, hit.normal);

                        //PUT ANIMATION HERE

                        //Parents the block to the player then puts it above their head, it also makes sure they're rotated the right way as well as storing their hirearchy name for later
                        currentlyCarrying = true;
                        hit.transform.position = transform.position + new Vector3(0, holdHeight, 0);
                        hit.transform.parent = transform.transform;
                        hit.transform.rotation = transform.rotation;

                        childName = hit.transform.gameObject.name;

                        changePlayerSpeed();
                    }
                }
                else
                {
                    //PUT NOPE SFX HERE
                    Debug.Log("Drop site is obscured || Raycast");
                }
            }
            //The raycast hit nothing so now it runs a box cast to check if the area in front is clear to drop the block. The area is the same size as the carry block
            else if(currentlyCarrying == true)
            {
                RaycastHit radiusHit;
                if (Physics.BoxCast(transform.position, checkBoxSize, transform.forward, out radiusHit, transform.rotation, grabRange))
                {
                    //PUT NOPE SFX HERE
                    //Draws the gizmo where the box ray was cast
                    Debug.Log("Drop site is obscured || Boxcast");
                    dropPoint = radiusHit.point;
                }
                else
                {
                    //Draws the gizmo in front of the player
                    Debug.Log("Clear to drop");
                    dropPoint = transform.position + (transform.forward + dropHeightOffset);

                    //PUT ANIMATION HERE

                    //Retrieves the transform of the carry block, sets its parent to none and then places it in front of the player with a height offset & resets the currentlyCarrying bool
                    Transform carryBlock = transform.Find(childName);
                    carryBlock.parent = null;
                    carryBlock.transform.position = transform.position + (transform.forward + dropHeightOffset);

                    currentlyCarrying = false;

                    changePlayerSpeed();
                }
            }
        }

    }

    private void changePlayerSpeed()
    {
        //Checks if this setting is even enabled and then checks if the player is carrying something then will modify their speed accordingly or reset it when not carrying anything.
        if(slowEnabled == true)
        {
            if(currentlyCarrying == true)
            {
                //movementScript.speed = startingSpeed * percentageSlowdown;
                //movementScript.rotSpeed = startingRotSpeed * percentageSlowdown;
            }
            else
            {
                //movementScript.speed = startingSpeed;
                //movementScript.rotSpeed = startingRotSpeed;
            }
        }
    }

    //Draws a green sphere gizmo for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(grabPoint, 0.2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(dropPoint, checkBoxSize);

    }



}
