using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootRaycast : MonoBehaviour
{
    
    private float raycastLength = 2f;
    private Vector3 targetPos;

    /// <summary>
    /// The local-space position of where the IK spawned
    /// </summary>
    private Vector3 startPos;

    /// <summary>
    /// The local-space rotation of where the IK spawned
    /// </summary>
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
        startRot = transform.localRotation;
        startPos = transform.localPosition;
    }

    
    void Update()
    {
        //FindGround();
        transform.localPosition = AnimMath.Ease(transform.localPosition, targetPos, .01f);
    }

    void FindGround()
    {
        Vector3 origin = transform.position + Vector3.up * (raycastLength / 2);
        Vector3 direction = Vector3.down;


        // Check for collision with a ray
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, raycastLength))
        {
            // Find ground position
            groundPos = hitInfo.point + Vector3.up * startPos.y;

            // Convert starting rotation into worldspace (order is important for quaternion multiplication)
            Quaternion worldNetural = transform.parent.rotation * startRot;

            // Find ground rotation
            groundRot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * worldNetural;
        }
    }

    public void SetLocalPosition(Vector3 p)
    {
        targetPos = p;
    }

    public void BackToHome()
    {
        targetPos = startPos;
    }

    public void SetOffsetPosition(Vector3 p)
    {
        targetPos = startPos + p;
    }
}
