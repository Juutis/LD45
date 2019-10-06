using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    float ScrollSpeed = 1.0f;

    [SerializeField]
    GameObject cam;

    private float shakeDuration = 0.0f;

    [SerializeField]
    private float shakeMagnitude = 0.7f;

    [SerializeField]
    private float dampingSpeed = 1.0f;

    [SerializeField]
    private bool mouseScroll = true;

    [SerializeField]
    private float ZoomFactor = 1.0f;

    [SerializeField]
    private float MaxZoom = 15.0f;

    [SerializeField]
    private float MinZoom = 1.0f;

    [SerializeField]
    private float MinX = -10.0f;

    [SerializeField]
    private float MaxX = 10.0f;

    [SerializeField]
    private float MinY = -10.0f;

    [SerializeField]
    private float MaxY = 10.0f;


    Vector3 initialPosition;

    public static CameraHandler INSTANCE;

    bool shaking;
    
    void Awake()
    {
        INSTANCE = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        initialPosition = cam.transform.localPosition;
    }
    
    void Update()
    {
        if (Input.GetAxis("Vertical") > 0.01 || mouseScroll && Input.mousePosition.y >= Screen.height * 0.98)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed, Space.World);
        }
        if (Input.GetAxis("Vertical") < -0.01 || mouseScroll && Input.mousePosition.y <= Screen.height * 0.02)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
        }
        if (Input.GetAxis("Horizontal") > 0.01 || mouseScroll && Input.mousePosition.x >= Screen.width * 0.98)
        {
            transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed, Space.World);
        }
        if (Input.GetAxis("Horizontal") < -0.01 || mouseScroll && Input.mousePosition.x <= Screen.width * 0.02)
        {
            transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
        }

        if (transform.position.x < MinX)
        {
            transform.position = new Vector3(MinX, transform.position.y, transform.position.z);
        }
        if (transform.position.x > MaxX)
        {
            transform.position = new Vector3(MaxX, transform.position.y, transform.position.z);
        }
        if (transform.position.z < MinY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, MinY);
        }
        if (transform.position.z > MaxY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, MaxY);
        }

        if (shakeDuration > 0)
        {
            cam.transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude * shakeDuration;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            cam.transform.localPosition = initialPosition;
        }

        float zoom = Input.GetAxis("Zoom");
        if (Mathf.Abs(zoom) > 0.01)
        {
            Camera.main.orthographicSize -= zoom * ZoomFactor;
        }

        float zoomB = Input.GetAxis("ZoomB");
        if (Mathf.Abs(zoomB) > 0.01)
        {
            Camera.main.orthographicSize -= zoomB * ZoomFactor * Time.deltaTime;
        }

        if (Camera.main.orthographicSize > MaxZoom)
        {
            Camera.main.orthographicSize = MaxZoom;
        }
        if (Camera.main.orthographicSize < MinZoom)
        {
            Camera.main.orthographicSize = MinZoom;
        }
    }

    public void TriggerShake(Vector3 position)
    {
        float shakeAmount = Vector3.Distance(cam.transform.position, position);
        shakeAmount = (25 - shakeAmount) / 10;
        if (shakeAmount > 1.0f) shakeAmount = 1.0f;
        if (shakeAmount < 0.0f) return;
        shakeDuration = 1.5f * shakeAmount;
    }

    public void TriggerMinorShake(Vector3 position)
    {
        float shakeAmount = Vector3.Distance(cam.transform.position, position);
        shakeAmount = (25 - shakeAmount) / 10;
        if (shakeAmount > 1.0f) shakeAmount = 1.0f;
        if (shakeAmount < 0.0f) return;
        shakeDuration = 0.5f * shakeAmount;
    }
}
