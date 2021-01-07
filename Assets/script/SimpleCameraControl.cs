using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleCameraControl : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }

    [Header("Input")]
    public KeyCode cameraLockKey = KeyCode.Alpha1;
    public bool isLocked = false;
    public KeyCode enableMouseInputKey = KeyCode.Alpha2;
    public bool isMouseEnabled = true;
    public KeyCode enableKeyboardKey = KeyCode.Alpha3;
    public bool isKeyboardEnabled = true;
    //public KeyCode resetCameraKey = KeyCode.Alpha4;
    
    [Header("Rotate")]
    public float sensitivityX = 5F;
    public float sensitivityY = 5F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -360F;
    public float maximumY = 360F;

    float rotationX = 0F;
    float rotationY = 0F;

    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 4;

    private Quaternion baseRotation;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [Header("Zoom")]
    public float minFov = 15f;
    public float maxFov = 90f;
    public float zoomStep = 60.0f;
    public float zoomSpeed = 4.0f;
    private float targetFov;
    private float originalFov;

    [Header("Pan")]
    public float panSpeed = 5.0f;

    private Camera myCamera;

    void Start()
    {
        myCamera = transform.GetComponent<Camera>();

        originalRotation = transform.localRotation;
        originalPosition = transform.position;
        originalFov = myCamera.fieldOfView;

        baseRotation = originalRotation;
        targetFov = originalFov;
    }

    private void Update()
    {
        if (!isLocked)
        {
            if (isMouseEnabled)
            {
                MouseRotate();
                Zoom();
            }

            if (isKeyboardEnabled)
            {
                Pan();
            }
        }

        if (Input.GetKeyDown(cameraLockKey))
        {
            isLocked = !isLocked;
        }

        if (Input.GetKeyDown(enableMouseInputKey))
        {
            isMouseEnabled = !isMouseEnabled;
        }

        if (Input.GetKeyDown(enableKeyboardKey))
        {
            isKeyboardEnabled = !isKeyboardEnabled;
        }

        //if (Input.GetKeyDown(resetCameraKey))
        //{
        //    //baseRotation = transform.localRotation;
        //    targetFov = originalFov;

        //    transform.rotation = originalRotation;
        //    transform.position = originalPosition;
        //    myCamera.fieldOfView = originalFov;
		//}
    }

    private void Pan()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction.x = -1.0f;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction.x = 1.0f;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction.y = 1.0f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction.y = -1.0f;
        }

        direction.Normalize();
        transform.position += transform.TransformDirection(direction) * Time.deltaTime * panSpeed;
    }

    private void Zoom()
    {
        targetFov -= Input.GetAxis("Mouse ScrollWheel") * zoomStep;
        targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFov, Time.deltaTime * zoomSpeed);
    }
     
    private void MouseRotate()
    {
        float lastX = rotAverageX;
        float lastY = rotAverageY;

        rotAverageY = 0f;
        rotAverageX = 0f;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;

        rotArrayY.Add(rotationY);
        rotArrayX.Add(rotationX);

        if (rotArrayY.Count >= frameCounter)
        {
            rotArrayY.RemoveAt(0);
        }
        if (rotArrayX.Count >= frameCounter)
        {
            rotArrayX.RemoveAt(0);
        }

        for (int j = 0; j < rotArrayY.Count; j++)
        {
            rotAverageY += rotArrayY[j];
        }
        for (int i = 0; i < rotArrayX.Count; i++)
        {
            rotAverageX += rotArrayX[i];
        }

        rotAverageY /= rotArrayY.Count;
        rotAverageX /= rotArrayX.Count;

        rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
        rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);
        
        Debug.Log("X ROT: " + rotAverageX + ", Y ROT: " + rotAverageY);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.right);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

        transform.localRotation = baseRotation * xQuaternion * yQuaternion;
    }
    
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}