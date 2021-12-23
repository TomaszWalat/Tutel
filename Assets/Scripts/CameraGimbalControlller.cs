using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CameraGimbalControlller : MonoBehaviour
{

    [SerializeField]
    private Transform anchorPoint;

    [SerializeField] [Tooltip("Amount to rotate by each late update")]
    private float rotationSpeedY; // should maybe be called rotation speed - should X and Y have separate speeds?

    [SerializeField] [Tooltip("Amount to rotate by each late update")]
    private float rotationSpeedX; // should maybe be called rotation speed - should X and Y have separate speeds?

    [SerializeField] [Tooltip(" Rotation smoothing")]
    private float smoothing;

    [SerializeField] [Range(-180, 180)]
    private float rotationCapY_left;

    [SerializeField] [Range(-180, 180)]
    private float rotationCapY_right;

    [SerializeField]
    private float realRotCapY_left;

    [SerializeField]
    private float realRotCapY_right;

    [SerializeField] [Range(-90, 90)]
    private float rotationCapX_upper;

    [SerializeField] [Range(-90, 90)]
    private float rotationCapX_lower;

    [SerializeField]
    private float realRotCapX_upper;

    [SerializeField]
    private float realRotCapX_lower;


    private Transform m_transform;

    private Vector3 rotationUp;

    void Start()
    {
        Debug.Assert( (anchorPoint != null), "anchorPoint object is not set");

        if (anchorPoint != null)
        {
            Debug.Assert( anchorPoint.CompareTag("cameraAnchor"), "object entered into \"anchorPoint\" is not tagged as \"cameraAnchor\"");
        }

        if(anchorPoint != null && anchorPoint.CompareTag("cameraAnchor"))
        {
            rotationUp = anchorPoint.up; // Rotate around the anchor's Y axis
        }
        else
        {
            rotationUp = Vector3.up; // Rotate around the world Y axis (0, 1, 0)
        }

        m_transform = gameObject.transform;
    }

    void FixedUpdate()
    {

        if (rotationCapY_left < rotationCapY_right)
        {
            rotationCapY_left = rotationCapY_right;
        }

        if (rotationCapX_upper < rotationCapX_lower)
        {
            rotationCapX_upper = rotationCapX_lower;
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

        if (rotationCapX_upper > 0.0f)
        {
            realRotCapX_upper = (360.0f - rotationCapX_upper) + 1.0f;
        }
        else if (rotationCapX_upper < 0.0f)
        {
            realRotCapX_upper = (0.0f - rotationCapX_upper) - 1.0f;
        }
        else
        {
            realRotCapX_upper = 0.0f;
        }

        if (rotationCapX_lower < 0.0f)
        {
            realRotCapX_lower = (0.0f - rotationCapX_lower) - 1.0f;
        }
        else if (rotationCapX_lower > 0.0f)
        {
            realRotCapX_lower = (360.0f - rotationCapX_lower) + 1.0f;
        }
        else
        {
            realRotCapX_lower = 0.0f;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKey("right"))
        {
            Rotate(0.0f, -rotationSpeedY);
        }
        else if (Input.GetKey("left"))
        {
            Rotate(0.0f, rotationSpeedY);
        }

        if (Input.GetKey("up"))
        {
            Rotate(rotationSpeedX, 0.0f);
        }
        else if (Input.GetKey("down"))
        {
            Rotate(-rotationSpeedX, 0.0f);
        }
    }

    public void Rotate(float angleAxisX, float angleAxisY)
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
            // Rotate around the world Y axis ( Vector3(0, 1, 0) ) -> (this can later be made to rotate around the anchor Y axis by setting the rotationUp to anchorPoint.up)
            m_transform.RotateAround(anchorPoint.position, rotationUp, angleAxisY * smoothing * Time.deltaTime);
        }
        else if (rotationAmountY < 0.0f && formatedRotY < rotationCapY_left)
        {
            // Rotate around the world Y axis ( Vector3(0, 1, 0) ) -> (this can later be made to rotate around the anchor Y axis by setting the rotationUp to anchorPoint.up)
            m_transform.RotateAround(anchorPoint.position, rotationUp, angleAxisY * smoothing * Time.deltaTime);
        }


        float currentXRotation = m_transform.eulerAngles.x;

        float formatedRotX = 0.0f;

        if (currentXRotation > 270.0f)
        {
            formatedRotX = 360.0f - (currentXRotation);
        }
        else if (currentXRotation < 90.0f)
        {
            formatedRotX = 0.0f - (currentXRotation);
        }

        float rotationAmountX = angleAxisX * smoothing * Time.deltaTime;

        // ---------------------------------> WINNER WINNER CHICKEN DINNER! <--------------------------------------------------- //
        if (rotationAmountX > 0.0f && formatedRotX > (rotationCapX_lower + 1))//< (rotCapUpperX))
        {
            // Rotate around the gimbal X axis at anchor's position (not gimbal's position)
            m_transform.RotateAround(anchorPoint.position, m_transform.right, angleAxisX * smoothing * Time.deltaTime);
        }
        else if (rotationAmountX < 0.0f && formatedRotX < (rotationCapX_upper - 1))//> (rotCapLowerX))
        {
            // Rotate around the gimbal X axis at anchor's position (not gimbal's position)
            m_transform.RotateAround(anchorPoint.position, m_transform.right, angleAxisX * smoothing * Time.deltaTime);
        }

        // m_transform.rotation = Quaternion.Euler(m_transform.eulerAngles.x, m_transform.eulerAngles.y, 0.0f);
    }
}
