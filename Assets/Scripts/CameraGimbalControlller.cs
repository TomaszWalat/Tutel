using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CameraGimbalControlller : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("Amount to rotate by each late update")]
    private float rotationAngle; // should maybe be called rotation speed - should X and Y have separate speeds?

    [SerializeField] 
    [Tooltip(" Rotation smoothing")]
    private float smoothing;

    [SerializeField] 
    [Range(-180, 180)]
    private float rotationCapY_left;

    [SerializeField] 
    [Range(-180, 180)]
    private float rotationCapY_right;

    [SerializeField]
    private float realRotCapY_left;

    [SerializeField]
    private float realRotCapY_right;

    private Transform m_transform;

    private Vector3 rotationUp;

    void Start()
    {

        rotationUp = Vector3.up; // code stub for if we ever want to mess around with dynamically changing the RotateAround() up direction

        m_transform = gameObject.transform;
    }

    void FixedUpdate()
    {

        if (rotationCapY_left < rotationCapY_right)
        {
            rotationCapY_left = rotationCapY_right;
        }

        if (rotationCapY_left > 0.0f)
        {
            realRotCapY_left = 360.0f - rotationCapY_left;
        }
        else if (rotationCapY_left < 0.0f)
        {
            realRotCapY_left = 0.0f - rotationCapY_left;
        }
        else
        {
            realRotCapY_left = 0.0f;
        }

        if (rotationCapY_right < 0.0f)
        {
            realRotCapY_right = 0.0f - rotationCapY_right;
        }
        else if (rotationCapY_right > 0.0f)
        {
            realRotCapY_right = 360.0f - rotationCapY_right;
        }
        else
        {
            realRotCapY_right = 0.0f;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKey("right"))
        {
            Rotate(-rotationAngle);
        }
        else if (Input.GetKey("left"))
        {
            Rotate(rotationAngle);
        }
    }

    public void Rotate(float angleAxisY)
    {

        float currentRotationY = m_transform.eulerAngles.y;

        float formatedRotY = 0.0f;

        if (currentRotationY > 180.0f)
        {
            formatedRotY = 360.0f - currentRotationY;
        }
        else if (currentRotationY < 180.0f)
        {
            formatedRotY = 0.0f - currentRotationY;
        }

        float rotationAmountY = angleAxisY * smoothing * Time.deltaTime;

        // ---------------------------------> WINNER WINNER CHICKEN DINNER! <--------------------------------------------------- //
        if (rotationAmountY > 0.0f && formatedRotY > rotationCapY_right)
        {

            // Rotate around the world Y axis ( Vector3(0, 1, 0) ) 
            m_transform.RotateAround(Vector3.zero, rotationUp, angleAxisY * smoothing * Time.deltaTime);
        }
        else if (rotationAmountY < 0.0f && formatedRotY < rotationCapY_left)
        {

            // Rotate around the world Y axis ( Vector3(0, 1, 0) ) 
            m_transform.RotateAround(Vector3.zero, rotationUp, angleAxisY * smoothing * Time.deltaTime);
        }
    }
}
