using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Joint : MonoBehaviour
{
    [HideInInspector]
    public Quaternion startRot;
    [HideInInspector]
    public Vector3 startPos;

    public Transform target;

    private void Start()
    {
        SetNewStart();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) Destroy(rb);
    }

    public void SetCurrentRotation(Quaternion newRot)
    {
        transform.localRotation = newRot;
    }
    public void SetCurrentPosition(Vector3 newPos)
    {
        transform.localPosition = newPos;
    }
    public void EaseToStartPosition(float percent)
    {
        transform.localPosition = AnimMath.Ease(transform.localPosition, startPos, percent);
    }
    public void EaseToStartRotation(float percent)
    {
        transform.localRotation = AnimMath.Ease(transform.localRotation, startRot, percent);
    }
    public void EaseToNewPosition(Vector3 newPos, float percent)
    {
        transform.localPosition = AnimMath.Ease(transform.localPosition, newPos, percent);
        
    }
    public void EaseToNewRotation(Quaternion newRot, float percent)
    {
        transform.localRotation = AnimMath.Ease(transform.localRotation, newRot, percent);
        
    }


    public void ResetToStart()
    {
        transform.localRotation = startRot;
        transform.localPosition = startPos;
    }

    public void SetNewStart()
    {
        startRot = transform.localRotation;
        startPos = transform.localPosition;
    }

    public void LockToTarget()
    {
        if (target == null) return;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
