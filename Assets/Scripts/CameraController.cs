using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Target transform to rotate around")]
    private Transform target;

    [SerializeField]
    [Tooltip("Offset from target")]
    private float offsetX, offsetY, offsetZ;

    [SerializeField]
    [Range(-180,180)]
    [Tooltip("Set tilt around own axis, -180 to 180 (tiltY doesn't do anything)")]
    private float tiltX, tiltY, tiltZ; // tilt Y is a dummy, here just for consistency

    [SerializeField]
    [Tooltip("Amount to rotate by each late update")]
    private float rotationAngle;

    [SerializeField]
    [Tooltip(" Rotation smoothing")]
    private float smoothing;

    private Transform m_transform;

    private Vector3 rotationUp;

    // Start is called before the first frame update
    void Start()
    {
        rotationUp = Vector3.up; // code stub for if we ever want to mess around with dynamically changing the RotateAround() up direction
        m_transform = gameObject.transform;
        m_transform.position = target.position;
        m_transform.Rotate(tiltX, tiltY, tiltZ);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Rotate(rotationAngle);
    }

    public void Rotate(float angle)
    {
        m_transform.RotateAround(target.position, rotationUp, angle * smoothing * Time.deltaTime);
        m_transform.position = target.position + (m_transform.right * offsetX) + (m_transform.up * offsetY) + (m_transform.forward * offsetZ);
    }
}