using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float rotSpeed = 200f;
    private CharacterController cCont;

    // Start is called before the first frame update
    void Start()
    {
        cCont = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float hIn = Input.GetAxis("Horizontal");
        float vIn = Input.GetAxis("Vertical");

        Vector3 movDir = new Vector3(hIn, 0, vIn);
        float magnitude = Mathf.Clamp01(movDir.magnitude) * speed;
        movDir.Normalize();

        cCont.SimpleMove(movDir * magnitude);

        if (movDir != Vector3.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(movDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, rotSpeed * Time.deltaTime);
        }
    }
}
