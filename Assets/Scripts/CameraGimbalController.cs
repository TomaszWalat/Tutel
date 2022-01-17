using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CameraGimbalController : MonoBehaviour
{

    [SerializeField] [Tooltip("Centre of rotation for the gimbal")]
    private GameObject anchorPoint;

    [SerializeField] [Tooltip("(DEBUG OBJECT) Object visualising the inverse behaviour of the gimbal relative to the anchor - useful if gimbal rotation radius is negative")]
    private GameObject counterWeight;

    [SerializeField] [Tooltip("Horizontal rotation speed")]
    private float rotationSpeedY;

    [SerializeField] [Tooltip("Verical rotation speed")]
    private float rotationSpeedX;

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

    [SerializeField] [Tooltip("(VIEW ONLY) The radius of the gimbal's rotation sphere (i.e. distance from the anchor). Negative distance inverts the X and Y caps")]
    private float rotationRadius;

    [SerializeField]
    private float startingRadius = -10.0f;

    [SerializeField] [Tooltip("Furthest the gimbal can move forward - acts as maximum distance from anchor if positive, minimum if negative")]
    private float rotRadiusCapForward;

    [SerializeField] [Tooltip("Furthest the gimbal can move backward - acts as minimum distance from anchor if positive, maximum if negative")]
    private float rotRadiusCapBackward;


    private Transform m_transform;

    private Vector3 rotationUp;

    void Start()
    {
        // --- lock the mouse for camera controls --- //
        Cursor.lockState = CursorLockMode.Locked;

        // --- anchorPoint validation --- //
        Debug.Assert( (anchorPoint != null), "anchorPoint object is not set");

        if (anchorPoint != null)
        {
            Debug.Assert( anchorPoint.CompareTag("cameraAnchor"), "object entered into \"anchorPoint\" is not tagged as \"cameraAnchor\"");
        }

        // --- counterWeight validation --- //
        Debug.Assert((counterWeight != null), "counterWeight object is not set");

        // --- Set default up vector --- //
        if (anchorPoint != null && anchorPoint.CompareTag("cameraAnchor"))
        {
            rotationUp = anchorPoint.transform.up; // Rotate around the anchor's Y axis
        }
        else
        {
            rotationUp = Vector3.up; // Rotate around the world Y axis (0, 1, 0)
        }

        m_transform = gameObject.transform;

        rotationRadius = 0.0f; // Reset any value input through editor
        UpdateRadius(startingRadius); // Starting value
    }

    void FixedUpdate()
    {
        // --- Prevent caps from overlapping --- //
        // horizontal roation cap
        if (rotationCapY_left < rotationCapY_right)
        {
            rotationCapY_left = rotationCapY_right;
        }
        // vertical rotation cap
        if (rotationCapX_upper < rotationCapX_lower)
        {
            rotationCapX_upper = rotationCapX_lower;
        }
        // rotation radius ("zoom") cap
        if (rotRadiusCapForward < rotRadiusCapBackward)
        {
            rotRadiusCapBackward = rotRadiusCapForward;
        }

        // --- Calculate the real cap values --- //
        // (Unity's engine rotation vs Editor's displayed rotation are different)
        // Debug info only
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
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        /*
        float x = Input.GetAxis("CamHorizontal");
        float y = Input.GetAxis("CamVertical");
        */
        RotateBy(y * rotationSpeedY, x * rotationSpeedX);

        // --- The Updateradius is a pc only feature --- //

        float z = Input.GetAxis("Mouse ScrollWheel");

        UpdateRadius(0.1f * z);
        /*
        // --- Input detection --- //
        // Temporary - final build should handle input better
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
        */
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xAxisAngleDelta"></param>
    /// <param name="yAxisAngleDelta"></param>
    public void RotateBy(float xAxisAngleDelta, float yAxisAngleDelta)
    {
        // --- Horizontal rotation --- //
        float currentRotationY = m_transform.eulerAngles.y;

        float formatedRotY = 0.0f;

        // Format from real (0 to 360) to cap (-180 to 180) rotation format
        if (currentRotationY > 180.0f)
        {
            formatedRotY = 360.0f - currentRotationY;
        }
        else if (currentRotationY < 180.0f)
        {
            formatedRotY = 0.0f - currentRotationY;
        }

        float rotationAmountY = yAxisAngleDelta * smoothing * Time.deltaTime; // used for estimation only

        // Rotate around anchor's up or world's Y axis if in bounds
        if (rotationAmountY > 0.0f && formatedRotY > rotationCapY_right)
        {
            m_transform.RotateAround(anchorPoint.transform.position, rotationUp, yAxisAngleDelta * smoothing * Time.deltaTime);
        }
        else if (rotationAmountY < 0.0f && formatedRotY < rotationCapY_left)
        {
            m_transform.RotateAround(anchorPoint.transform.position, rotationUp, yAxisAngleDelta * smoothing * Time.deltaTime);
        }

        // --- Vertical rotation --- //
        float currentXRotation = m_transform.eulerAngles.x;

        float formatedRotX = 0.0f;

        // Format from real (0 to 360) to cap (-90 to 90) rotation format
        if (currentXRotation > 270.0f)
        {
            formatedRotX = 360.0f - (currentXRotation);
        }
        else if (currentXRotation < 90.0f)
        {
            formatedRotX = 0.0f - (currentXRotation);
        }

        float rotationAmountX = xAxisAngleDelta * smoothing * Time.deltaTime; // used for estimation only

        // Rotate around the gimbal X axis at anchor's position (not gimbal's position) if in bounds
        if (rotationAmountX > 0.0f && formatedRotX > (rotationCapX_lower + 1)) // +1 to clamp (to prevent flickering behaviour)
        {
            
            m_transform.RotateAround(anchorPoint.transform.position, m_transform.right, xAxisAngleDelta * smoothing * Time.deltaTime);
        }
        else if (rotationAmountX < 0.0f && formatedRotX < (rotationCapX_upper - 1)) // -1 to clamp (to prevent flickering behaviour)
        {
            m_transform.RotateAround(anchorPoint.transform.position, m_transform.right, xAxisAngleDelta * smoothing * Time.deltaTime);
        }

        //// reset gimbal's z rotation - sometimes erroneously altered by RotateAround()
        // m_transform.rotation = Quaternion.Euler(m_transform.eulerAngles.x, m_transform.eulerAngles.y, 0.0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="radiusDelta"></param>
    private void UpdateRadius(float radiusDelta)
    {
        // Change radius if in bounds
        if (rotRadiusCapBackward <= (rotationRadius + radiusDelta) && (rotationRadius + radiusDelta) <= rotRadiusCapForward)
        {
            // Move gimbal
            m_transform.Translate(Vector3.forward * radiusDelta, Space.Self);

            // Move counterWeight (debug object) inversely
            counterWeight.transform.Translate(Vector3.forward * (radiusDelta * -2), Space.Self);

            // Update radius
            rotationRadius += radiusDelta;
        }
    }

}
