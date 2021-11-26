using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushScript : MonoBehaviour
{
    public float grabRange = 2f;
    public Vector3 offset = new Vector3(0f, -0.5f, 0f);
    public float pushForce = 10f;

    Vector3 collision = Vector3.zero;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("action button"))
        {
            var ray = new Ray(transform.position + offset, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grabRange))
            {
                if (hit.transform.gameObject.tag == "pushBlock")
                {
                    collision = hit.point;
                    Vector3 playerDir = hit.point - transform.position;
                    Vector3 reflection = Vector3.Reflect(playerDir, hit.normal);

                    hit.rigidbody.AddForce(-reflection * pushForce * Time.deltaTime, ForceMode.Impulse);
                }
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(collision, 0.2f);
    }
}
