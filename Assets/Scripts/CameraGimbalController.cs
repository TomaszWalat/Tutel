using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CameraGimbalController : MonoBehaviour
{

    [SerializeField] [Tooltip("Centre of rotation for the gimbal")]
    private Transform anchorPoint;

    [SerializeField] [Tooltip("(DEBUG OBJECT) Object visualising the inverse behaviour of the gimbal relative to the anchor - useful is gimbal rotation radius is negative")]
    private GameObject counterWeight;

    [SerializeField] [Tooltip("Horizontal rotation speed")]
    private float rotationSpeedY; // should maybe be called rotation speed - should X and Y have separate speeds?

    [SerializeField] [Tooltip("Verical rotation speed")]
    private float rotationSpeedX; // should maybe be called rotation speed - should X and Y have separate speeds?

    [SerializeField] [Tooltip("Rotation speed smoothing value")]
    private float smoothing;

    [SerializeField] [Range(-180, 180)] [Tooltip("Horizontal rotation cap anti-clock wise from anchor's forward direction")]
    private float rotationCapY_left;

    [SerializeField] [Range(-180, 180)] [Tooltip("Horizontal rotation cap clock wise from anchor's forward direction")]
    private float rotationCapY_right;

    [SerializeField] [Tooltip("(DEBUG INFO) Internal value of anti-clock wise Y cap")]
    private float realRotCapY_left;

    [SerializeField] [Tooltip("(DEBUG INFO) Internal value of clock wise Y cap")]
    private float realRotCapY_right;

    [SerializeField] [Range(-90, 90)] [Tooltip("Vertical rotation cap anti-clock wise from anchor's forward direction")]
    private float rotationCapX_upper;

    [SerializeField] [Range(-90, 90)] [Tooltip("Vertical rotation cap clock wise from anchor's forward direction")]
    private float rotationCapX_lower;

    [SerializeField] [Tooltip("(DEBUG INFO) Internal value of anti-clock wise X cap. Clamped (-1) to prevent gimbal from aligning its Z axis with anchor Y axis - causes strange behaviour")]
    private float realRotCapX_upper;

    [SerializeField] [Tooltip("(DEBUG INFO) Internal value of clock wise X cap, Clamped (+1) to prevent gimbal from aligning its Z axis with anchor Y axis - causes strange behaviour")]
    private float realRotCapX_lower;

    [SerializeField] [Tooltip("(DEBUG INFO) The radius of the gimbal's rotation sphere (i.e. distance from the anchor). Negative distance inverts the X and Y caps")]
    private float rotationRadius;

    [SerializeField] [Tooltip("Furthest the gimbal can move forward - acts as maximum distance from anchor if positive, minimum if negative")]
    private float rotRadiusCapForward;

    [SerializeField] [Tooltip("Furthest the gimbal can move backward - acts as minimum distance from anchor if positive, maximum if negative")]
    private float rotRadiusCapBackward;



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

        rotationRadius = 0.0f;
        UpdateRadius(-10.0f);
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

        if (rotRadiusCapForward < rotRadiusCapBackward)
        {
            rotRadiusCapBackward = rotRadiusCapForward;
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
            RotateBy(0.0f, -rotationSpeedY);
        }
        else if (Input.GetKey("left"))
        {
            RotateBy(0.0f, rotationSpeedY);
        }

        if (Input.GetKey("up"))
        {
            RotateBy(rotationSpeedX, 0.0f);
        }
        else if (Input.GetKey("down"))
        {
            RotateBy(-rotationSpeedX, 0.0f);
        }

        if (Input.GetKey("."))
        {
            UpdateRadius(0.1f);
        }
        else if (Input.GetKey(","))
        {
            UpdateRadius(-0.1f);
        }
    }

    public void RotateBy(float xAxisAngleDelta, float yAxisAngleDelta)
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

        float rotationAmountY = yAxisAngleDelta * smoothing * Time.deltaTime;

        // ---------------------------------> WINNER WINNER CHICKEN DINNER! <--------------------------------------------------- //
        if (rotationAmountY > 0.0f && formatedRotY > rotationCapY_right)
        {
            // Rotate around the world Y axis ( Vector3(0, 1, 0) ) -> (this can later be made to rotate around the anchor Y axis by setting the rotationUp to anchorPoint.up)
            m_transform.RotateAround(anchorPoint.position, rotationUp, yAxisAngleDelta * smoothing * Time.deltaTime);
        }
        else if (rotationAmountY < 0.0f && formatedRotY < rotationCapY_left)
        {
            // Rotate around the world Y axis ( Vector3(0, 1, 0) ) -> (this can later be made to rotate around the anchor Y axis by setting the rotationUp to anchorPoint.up)
            m_transform.RotateAround(anchorPoint.position, rotationUp, yAxisAngleDelta * smoothing * Time.deltaTime);
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

        float rotationAmountX = xAxisAngleDelta * smoothing * Time.deltaTime;

        // ---------------------------------> WINNER WINNER CHICKEN DINNER! <--------------------------------------------------- //
        if (rotationAmountX > 0.0f && formatedRotX > (rotationCapX_lower + 1))//< (rotCapUpperX))
        {
            // Rotate around the gimbal X axis at anchor's position (not gimbal's position)
            m_transform.RotateAround(anchorPoint.position, m_transform.right, xAxisAngleDelta * smoothing * Time.deltaTime);
        }
        else if (rotationAmountX < 0.0f && formatedRotX < (rotationCapX_upper - 1))//> (rotCapLowerX))
        {
            // Rotate around the gimbal X axis at anchor's position (not gimbal's position)
            m_transform.RotateAround(anchorPoint.position, m_transform.right, xAxisAngleDelta * smoothing * Time.deltaTime);
        }

        // m_transform.rotation = Quaternion.Euler(m_transform.eulerAngles.x, m_transform.eulerAngles.y, 0.0f);
    }

    private void UpdateRadius(float radiusDelta)
    {
        if (rotRadiusCapBackward < (rotationRadius + radiusDelta) && (rotationRadius + radiusDelta) < rotRadiusCapForward)
        {
            m_transform.Translate(Vector3.forward * radiusDelta, Space.Self);

            counterWeight.transform.Translate(Vector3.forward * (radiusDelta * -2), Space.Self);

            rotationRadius += radiusDelta;
        }
    }
}
