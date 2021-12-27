using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Tooltip("Object the camera will follow (move towards)")]
    private GameObject cameraGimbal;

    [SerializeField] [Tooltip("Object the camera will focus on (look at)")]
    private Transform focusPoint;

    [SerializeField] [Tooltip("(CODE STUB) Offset from the gimbal's position - always relative to the gimbal")]
    private float offsetX, offsetY, offsetZ;

    [SerializeField] [Range(-180,180)] [Tooltip("(CODE STUB) Tilt from the gimbal's rotation - works only without a focus point")]
    private float tiltX, tiltY, tiltZ;

    [SerializeField] [Tooltip("Speed at which the camera will move towards the gimbal")]
    private float movementSpeed;

    [SerializeField] [Tooltip("Smoothing factor for movement")]
    private float movementSmoothing;

    [SerializeField] [Tooltip("(CODE STUB) Speed at which the camera will rotate (tilt) around its axes")]
    private float tiltSpeed;

    [SerializeField] [Tooltip("(CODE STUB) Smoothing factor for rotation")]
    private float tiltSmoothing;


    private Transform m_transform;

    //private Transform gimbalTransform;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = gameObject.transform;
        //gimbalTransform = cameraGimbal.transform;
        m_transform.position = cameraGimbal.transform.position;// gimbalTransform.position;
        //m_transform.Rotate(tiltX, tiltY, tiltZ);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float step = movementSpeed * movementSmoothing * Time.deltaTime;
        m_transform.position = Vector3.MoveTowards(m_transform.position, cameraGimbal.transform.position, step);
        //m_transform.position = Vector3.MoveTowards(m_transform.position, gimbalTransform.position, step);

        //Vector3 m_up = m_transform.up;
        //m_transform.LookAt(focusPoint, m_up);

        //m_transform.LookAt(focusPoint, gimbalTransform.up);

        m_transform.LookAt(focusPoint);

    }



}