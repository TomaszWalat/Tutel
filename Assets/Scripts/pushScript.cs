using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushScript : MonoBehaviour
{
    //Grab range is how far the ray cast detects anything || The offset ofsets the ray form the player's centre || pushForce is obvious
    public float grabRange = 2f;
    public Vector3 offset = new Vector3(0f, -0.5f, 0f);
    public float pushForce = 1000f;

    Vector3 collision = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        //Checks if the action button form the input manager is hit (Default should be E or A on a controller)
        if (Input.GetButtonDown("action button"))
        {
            var ray = new Ray(transform.position + offset, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grabRange))
            {
                if (hit.transform.gameObject.tag == "pushBlock")
                {
                    //Moves the green gizmo sphere to where the raycast hit
                    //Determines the direction of the player from where the raycast hit
                    //Determines the direction the block should be pushed using the player's direction and the raycast point
                    collision = hit.point;
                    Vector3 playerDir = hit.point - transform.position;
                    Vector3 reflection = Vector3.Reflect(playerDir, hit.normal);

                    //Adds the push force
                    hit.rigidbody.AddForce(-reflection * pushForce * Time.deltaTime, ForceMode.Impulse);
                }
            }
        }
        
    }

    //Draws a green sphere gizmo for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(collision, 0.2f);
    }
}
