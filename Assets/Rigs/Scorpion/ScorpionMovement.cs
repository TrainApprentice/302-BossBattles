using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ScorpionMovement : MonoBehaviour
{
    enum Mode
    {
        Idle,
        Walk
    }

    public ScorpionStickyFeet[] leftFeet = new ScorpionStickyFeet[4];
    public ScorpionStickyFeet[] rightFeet = new ScorpionStickyFeet[4];
    public Transform groundRing;

    private int prevLeft = 0;
    private int prevRight = 0;
    private float animWalkDelay = 0;

    private CharacterController pawn;
    private CameraRig rig;
    private float walkSpeed = 4f;
    private Vector3 input;
    private Vector3 groundRingTargetPos;

    private Mode currState = Mode.Idle;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
        rig = FindObjectOfType<CameraRig>();
    }


    void Update()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 lookForward = rig.transform.forward;
        lookForward.y = 0;
        lookForward.Normalize();

        Vector3 lookRight = Vector3.Cross(Vector3.up, lookForward);

        input = lookForward * v + lookRight * h;
        if (input.sqrMagnitude > 1) input.Normalize();

        pawn.SimpleMove(input * walkSpeed);

        // Set the current state
        if (input.sqrMagnitude > .05f) currState = Mode.Walk;
        else currState = Mode.Idle;

        Animate();
    }


    void Animate()
    {
        groundRingTargetPos = transform.InverseTransformDirection(input) + new Vector3(0, -.3f, 0);

        groundRing.localPosition = AnimMath.Ease(groundRing.localPosition, groundRingTargetPos, .001f);
        switch (currState)
        {
            case Mode.Idle:
                AnimIdle();
                break;
            case Mode.Walk:
                AnimWalk();
                break;
        }
    }
    void AnimIdle()
    {
        SetAllFeetStepDist(.5f);
        for (int i = 0; i < leftFeet.Length; i++)
        {
            if(leftFeet[i].wantsToMove) leftFeet[i].DoMove();
            if(rightFeet[i].wantsToMove) rightFeet[i].DoMove();
        }
    }
    void AnimWalk()
    {
        SetAllFeetStepDist(2);
        Quaternion faceDirection = Quaternion.LookRotation(input, Vector3.up);
        transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .001f);
        
        /*
        float footCounter = 0;
        for (int i = 0; i < leftFeet.Length; i++)
        {
            if (leftFeet[i].wantsToMove) footCounter++;
        }
        for (int i = 0; i < rightFeet.Length; i++)
        {
            if (rightFeet[i].wantsToMove) footCounter++;
        }

        if (footCounter >= 2)
        {
            int randLeft = Random.Range(0, 4);
            int randRight = Random.Range(0, 4);

            while (randLeft == prevLeft)
            {
                randLeft = Random.Range(0, 4);
            }

            while (randRight == randLeft || randRight == prevRight)
            {
                randRight = Random.Range(0, 4);
            }

            leftFeet[randLeft].DoMove();
            rightFeet[randRight].DoMove();
            prevRight = randRight;
            prevLeft = randLeft;
        }
        */
            
        
        
        
    }

    void SetAllFeetStepDist(float newNum)
    {
        for(int i = 0; i < leftFeet.Length; i++)
        {
            leftFeet[i].minStepDistance = newNum;
            rightFeet[i].minStepDistance = newNum;
        }
    }
}
