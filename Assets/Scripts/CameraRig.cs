using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public Transform target;
    float pitch = 0;
    float yaw = 0;

    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {
        if (target == null) return;

        transform.position = AnimMath.Ease(transform.position, target.position, .001f);

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * mouseSensitivityX;
        pitch += my * mouseSensitivityY;

        pitch = Mathf.Clamp(pitch, -20, 89);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

    }
}
