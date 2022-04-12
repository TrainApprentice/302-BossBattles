using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    enum Mode
    {
        Idle,
        Walk,
        Air,
        Block
    }
    private Mode currState;
    private GameObject cam;
    CharacterController pawn;
    Animator animController;
    
    private float speed = 7f;
    private float currDodgeCooldown = 0f;
    private float baseDodgeCooldown = 0f;

    public Joint leftLeg, rightLeg, rightArm, leftArm;

    public Transform leftFoot, rightFoot;
    
    public bool isDead = false;
    public bool isInvincible = false;
    public bool isGrounded = true;

    private bool isBlocking = false;

    
    private bool dieOnce = true;
    PlayerTargeting playerTargeting;

    float airAnimTimer = 0f;
    private Vector3 inputDir;
    private float velocityVertical = 0;
    private float gravMult = -9.8f;
    

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        cam = FindObjectOfType<CameraController>().gameObject;
        playerTargeting = GetComponent<PlayerTargeting>();
        animController = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            // Movement
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            bool playerIsAiming = (playerTargeting && playerTargeting.playerWantsToAim && playerTargeting.target);
            isBlocking = Input.GetKey("left ctrl");
            //isBlocking = true;
            
            if(h!= 0 || v!= 0)
            {
                if ((pawn.collisionFlags == CollisionFlags.None)) isGrounded = false;
            }

            Vector3 forwardV = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            Vector3 rightV = Vector3.Cross(Vector3.up, forwardV);
            
            inputDir = (forwardV * v + rightV * h);

            if (inputDir.sqrMagnitude > 1) inputDir.Normalize();

            if (pawn.collisionFlags == CollisionFlags.Below) isGrounded = true;
            
            bool wantsToJump = Input.GetButtonDown("Jump");
            if (isGrounded)
            {
                velocityVertical = 0;
                airAnimTimer = 0;
                if (wantsToJump)
                {
                  
                    isGrounded = false;
                    velocityVertical += 9f;
                }
            }
            velocityVertical += gravMult * Time.deltaTime;

            Vector3 moveAmt = (inputDir * speed) + (Vector3.up * velocityVertical);
            animController.SetFloat("Speed", inputDir.sqrMagnitude);
            

            pawn.Move(moveAmt * Time.deltaTime);

            if(isBlocking) currState = Mode.Block;
            else if (isGrounded && inputDir != Vector3.zero && currDodgeCooldown <= 0) currState = Mode.Walk;
            else if (!isGrounded) currState = Mode.Air;
            else currState = Mode.Idle;

            StateMachine();
        }
        else DeathAnim();

    }

    public void TakeHit()
    {
        
    }
    void StateMachine()
    {
        switch (currState)
        {
            case Mode.Idle:
                IdleAnim();
                if(rightArm.gameObject.active) rightArm.gameObject.SetActive(false);
                if (leftArm.gameObject.active) leftArm.gameObject.SetActive(false);
                break;
            case Mode.Walk:
                WalkAnim();
                if (rightArm.gameObject.active) rightArm.gameObject.SetActive(false);
                if (leftArm.gameObject.active) leftArm.gameObject.SetActive(false);
                break;
            case Mode.Air:
                AirAnim();
                break;
            case Mode.Block:
                BlockAnim();
                if (!rightArm.gameObject.active) rightArm.gameObject.SetActive(true);
                if (!leftArm.gameObject.active) leftArm.gameObject.SetActive(true);
                break;
        }
        animController.SetBool("isBlocking", isBlocking);
        animController.SetBool("isGrounded", isGrounded);
       
    }

    void WalkAnim()
    {
        speed = 7f;
        Quaternion faceDirection = Quaternion.LookRotation(inputDir, Vector3.up);
        transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .001f);
        leftLeg.LockToTarget();
        rightLeg.LockToTarget();
    }

    void IdleAnim()
    {
        //EaseAllJointsToStart(.001f);
        leftLeg.LockToTarget();
        rightLeg.LockToTarget();
        leftArm.EaseToStartPosition(.001f);
        leftArm.EaseToStartRotation(.001f);
        rightArm.EaseToStartRotation(.001f);
        rightArm.EaseToStartPosition(.001f);
        speed = 7f;
    }
    void AirAnim()
    {
       
    }
    void DeathAnim()
    {
        
    }
    void BlockAnim()
    {
        speed = 0;
        Vector3 leftArmGoalPos = new Vector3(.323f, 1.709f, .773f);
        Vector3 rightArmGoalPos = new Vector3(.635f, 1.721f, .623f);
        Quaternion rightArmGoalRot = Quaternion.Euler(-145, 30, 155);
        Quaternion leftArmGoalRot = Quaternion.Euler(8, 135, 32);

        rightArm.EaseToNewPosition(rightArmGoalPos, .001f);
        rightArm.EaseToNewRotation(rightArmGoalRot, .001f);
        leftArm.EaseToNewPosition(leftArmGoalPos, .001f);
        leftArm.EaseToNewRotation(leftArmGoalRot, .001f);
    }

    void SetAllJointsToStart()
    {
        //spine1.ResetToStart();
        //spine2.ResetToStart();
        //spine3.ResetToStart();
        //
        //leftShoulder.ResetToStart();
        //leftElbow.ResetToStart();
        //leftWrist.ResetToStart();
        //rightShoulder.ResetToStart();
        //rightElbow.ResetToStart();
        //rightWrist.ResetToStart();
        //
        //leftHip.ResetToStart();
        //leftKnee.ResetToStart();
        //leftAnkle.ResetToStart();
        //rightHip.ResetToStart();
        //rightKnee.ResetToStart();
        //rightAnkle.ResetToStart();
    }
    void EaseAllJointsToStart(float timer)
    {
        //spine1.EaseToStartRotation(timer);
        //spine2.EaseToStartRotation(timer);
        //spine3.EaseToStartRotation(timer);
        //
        //leftShoulder.EaseToStartRotation(timer);
        //leftElbow.EaseToStartRotation(timer);
        //leftWrist.EaseToStartRotation(timer);
        //rightShoulder.EaseToStartRotation(timer);
        //rightElbow.EaseToStartRotation(timer);
        //rightWrist.EaseToStartRotation(timer);
        //
        //leftHip.EaseToStartRotation(timer);
        //leftKnee.EaseToStartRotation(timer);
        //leftAnkle.EaseToStartRotation(timer);
        //rightHip.EaseToStartRotation(timer);
        //rightKnee.EaseToStartRotation(timer);
        //rightAnkle.EaseToStartRotation(timer);
    }
}
