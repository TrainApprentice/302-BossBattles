using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    public bool lockAxisX, lockAxisY, lockAxisZ;
    public Transform target;
    public bool wantToTarget = true;

    public PlayerMovement playerController;
    
    private Quaternion startRot;
    private Quaternion goalRotation;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerMovement>();
        
        startRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        TurnTowardTarget();
    }

    private void TurnTowardTarget()
    {
        if (playerController)
        {
            if (playerController.isInvincible || playerController.isDead || !playerController.isGrounded) return;
        }
        
        if(target != null && wantToTarget)
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.Normalize();
            Quaternion worldRot = Quaternion.LookRotation(toTarget, Vector3.up);
            Quaternion localRot = worldRot;
            if (transform.parent)
            {
                localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            }

            Vector3 euler = localRot.eulerAngles;

            if (lockAxisX) euler.x = startRot.x;
            if (lockAxisY) euler.y = startRot.y;
            if (lockAxisZ) euler.z = startRot.z;

            localRot.eulerAngles = euler;

            goalRotation = localRot;
        }
        
        else goalRotation = startRot;

        Quaternion tempRot = transform.localRotation;

        transform.localRotation = AnimMath.Ease(tempRot, goalRotation, .001f);
    }
}
