using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls camera look direction and fieldOfView by mouse, and manages cursor lock.
/// </summary>
[DisallowMultipleComponent]
public class MouseControllableCamera : MonoBehaviour
{
    //inspector
    [Header("Sensitivity")]
    [SerializeField]
    float sensitivityMouse = 2f;
    [SerializeField]
    float sensitivetyMouseWheel = 10f;

    [Header("Option")]
    [SerializeField]
    bool lockCursorOnAwake = true;

    [Header("Reference")]
    [SerializeField]
    new Camera camera;
    [SerializeField]
    Transform xAxis, yAxis;

    //data
    private void Awake()
    {
        if (lockCursorOnAwake)
            LockCursor(true);
    }
    void Update()
    {
        //transform
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            camera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
        }
        xAxis.eulerAngles += Vector3.right * -Input.GetAxis("Mouse Y") * sensitivityMouse;
        yAxis.eulerAngles += Vector3.up * Input.GetAxis("Mouse X") * sensitivityMouse;
        //cursor
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LockCursor(false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            LockCursor(true);
        }
    }
    public void LockCursor(bool cond)
    {
        if (cond)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
