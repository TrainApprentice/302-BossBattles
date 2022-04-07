using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionStickyFeet : MonoBehaviour
{
    //public AnimationCurve curveStepVertical;
    public Transform raycastSource;
    public bool wantsToMove = false;
    [HideInInspector]
    public float minStepDistance = 2f;

    private float stepHeight = .4f;
    private float raycastLength = 2f;
    

    /// <summary>
    /// The local-space rotation of where the IK spawned
    /// </summary>
    private Quaternion startRot;

    private Vector3 currControlPos;

    private Vector3 prevGroundPos;
    private Vector3 currGroundPos;

    private Quaternion prevGroundRot;
    private Quaternion currGroundRot;

    private float animLength = .1f;
    private float animCurrTime = 0;

    private bool isAnimating
    {
        get
        {
            return (animCurrTime < animLength);
        }
    }
    void Start()
    {
        startRot = transform.localRotation;
        FindGround();
    }

    
    void Update()
    {
        if(isAnimating)
        {
            animCurrTime += Time.deltaTime;
            float p = animCurrTime / animLength;
            p = Mathf.Clamp(p, 0, 1);

            //float y = curveStepVertical.Evaluate(p);

            //transform.position = AnimMath.Lerp(prevGroundPos, currGroundPos, p) + new Vector3(0, y, 0);  // FindPointOnCurve(p);
            transform.position = FindPositionOnCurve(p);
            

            transform.rotation = AnimMath.Lerp(prevGroundRot, currGroundRot, p);
        } else
        {
            transform.position = currGroundPos;
            transform.rotation = currGroundRot;

            Vector3 vToCurrPos = transform.position - raycastSource.position;

            
            if (vToCurrPos.sqrMagnitude > minStepDistance * minStepDistance)
            {
                FindGround();
            }
            

            /*
            if(vToCurrPos.sqrMagnitude > minStepDistance * minStepDistance)
            {
                wantsToMove = true;
            }
            else
            {
                wantsToMove = false;
            }
            */
            
        }
    }

    public void DoMove()
    {
        FindGround();
    }

    void FindGround()
    {
        animCurrTime = 0;
        Vector3 origin = raycastSource.position + Vector3.up * (raycastLength / 2);
        Vector3 direction = Vector3.down;


        // Check for collision with a ray
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, raycastLength))
        {
            prevGroundPos = currGroundPos;
            prevGroundRot = currGroundRot;
            // Find ground position
            currGroundPos = hitInfo.point;

            // Convert starting rotation into worldspace (order is important for quaternion multiplication)
            Quaternion worldNeutral = transform.parent.rotation * startRot;

            // Find ground rotation
            currGroundRot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * worldNeutral;

            // Set the new control point for the vertical movement
            Vector3 dirFromPrevGround = currGroundPos - prevGroundPos;

            currControlPos = prevGroundPos + new Vector3(dirFromPrevGround.x / 2, stepHeight, dirFromPrevGround.z / 2);
        }
    }

    Vector3 FindPositionOnCurve(float p)
    {
        Vector3 a = AnimMath.Lerp(prevGroundPos, currControlPos, p);
        Vector3 b = AnimMath.Lerp(currControlPos, currGroundPos, p);

        return AnimMath.Lerp(a, b, p);
    }
    
}
