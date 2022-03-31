using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GoonMovement : MonoBehaviour
{
    enum Mode { 
        Idle,
        Walk
    }

    public FootRaycast footLeft, footRight;

    private CharacterController pawn;
    private CameraRig rig;
    private float walkSpeed = 4f;
    private Vector3 input;

    public float footSeparateAmt = .2f;
    public float walkSpreadZ = .7f;
    public float walkSpreadY = .7f;

    float walkTimer = 0;
    float idleTimer = 0;

    private Mode currState = Mode.Idle;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
        rig = FindObjectOfType<CameraRig>();
    }


    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

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
    delegate void MoveFoot(FootRaycast foot, float time);

    void Animate()
    {
        switch(currState)
        {
            case Mode.Idle:
                AnimIdle();
                break;
            case Mode.Walk:
                AnimWalk();
                break;
        }
    }
    void AnimWalk()
    {
        walkTimer += Time.deltaTime * walkSpeed * input.sqrMagnitude;

        MoveFoot footWave = (foot, time) => {
            float y = Mathf.Cos(time) * .4f * walkSpreadY;
            float lateral = Mathf.Sin(time) * .6f * walkSpreadZ;

            Vector3 localDir = foot.transform.parent.InverseTransformDirection(input);
            Vector3 separateDir = Vector3.Cross(Vector3.up, localDir);

            float x = lateral * localDir.x * separateDir.x * footSeparateAmt;
            float z = lateral * localDir.z * separateDir.z * footSeparateAmt;

            if (y < 0) y = 0;

            foot.SetOffsetPosition(new Vector3(x, y, z));
        };

        footWave.Invoke(footLeft, walkTimer);
        footWave.Invoke(footRight, walkTimer + Mathf.PI);
    }

    void AnimIdle()
    {
        footLeft.BackToHome();
        footRight.BackToHome();
        walkTimer = 0;
    }
}
