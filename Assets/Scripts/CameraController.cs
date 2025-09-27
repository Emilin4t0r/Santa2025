using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 startRotation;

    public static bool freeLooking;
    public float min = -60f; //lower limit value for camera x-axis
    public float max = 60f; //upper limit value for camera x-axis
    public float sensitivity;

    float yRot = 0f;
    float xRot = 0f;

    private void Start()
    {
        startRotation = transform.localEulerAngles;
        Cursor.lockState = CursorLockMode.Confined;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ZoomIn(true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            ZoomIn(false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ActivateFreeLook(true);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            ActivateFreeLook(false);
        }
    }

    private void FixedUpdate()
    {
        if (freeLooking)
        {
            LookRotations();
        } else
        {
            transform.localEulerAngles = startRotation;
            transform.localEulerAngles += new Vector3(MouseAim.Ycoord * 0.01f, -Input.GetAxis("Horizontal") * 0.2f, -MouseAim.Xcoord * 0.01f);
        }
    }

    void ZoomIn(bool state)
    {
        float fov = state ? 50 : 70;
        Camera.main.fieldOfView = fov;
    }

    void ActivateFreeLook(bool state)
    {
        if (state)
        {
            freeLooking = true;
            Cursor.lockState = CursorLockMode.Locked;
            xRot += 5;
        }
        else
        {
            freeLooking = false;
            Cursor.lockState = CursorLockMode.Confined;
            
            yRot = 0;
            xRot = 0;
        }
    }

    void LookRotations()
    {
        yRot += Input.GetAxis("Mouse X") * sensitivity;
        xRot += Input.GetAxis("Mouse Y") * sensitivity;

        //stop from turning over
        xRot = Mathf.Clamp(xRot, min, max); 
        yRot = Mathf.Clamp(yRot, min, max);

        transform.localEulerAngles = new Vector3(-xRot, yRot, 0);
    }
}
