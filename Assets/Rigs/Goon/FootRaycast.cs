using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootRaycast : MonoBehaviour
{
    float distanceBetweenGroundAndIK = 0;
    private float raycastLength = 1f;

    private Quaternion startRot;

    /// <summary>
    /// The world-space position of the ground above or below the foot IK.
    /// </summary>
    private Vector3 groundPos;

    /// <summary>
    /// The world-space rotation for the foot IK to align with the ground.
    /// </summary>
    private Quaternion groundRot;
    void Start()
    {
        distanceBetweenGroundAndIK = transform.localPosition.y;
        startRot = transform.localRotation;
    }

    
    void Update()
    {
        FindGround();
    }

    void FindGround()
    {
        Vector3 origin = transform.position + Vector3.up * (raycastLength / 2);
        Vector3 direction = Vector3.down;


        // Check for collision with a ray
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, raycastLength))
        {
            // Find ground position
            groundPos = hitInfo.point + Vector3.up * distanceBetweenGroundAndIK;

            // Convert starting rotation into worldspace (order is important for quaternion multiplication)
            Quaternion worldNetural = transform.parent.rotation * startRot;

            // Find ground rotation
            groundRot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * worldNetural;
        }
    }
}
