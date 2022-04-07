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

    public Joint leftShoulder, leftElbow, leftWrist, rightShoulder, rightElbow, rightWrist;
    public Joint spine1, spine2, spine3, jointNeck;
    public Joint leftHip, leftKnee, leftAnkle, rightHip, rightKnee, rightAnkle;

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
                break;
            case Mode.Walk:
                WalkAnim();
                break;
            case Mode.Air:
                AirAnim();
                break;
            case Mode.Block:
                BlockAnim();
                break;
        }
        animController.SetBool("isBlocking", isBlocking);
    }

    void WalkAnim()
    {
        speed = 7f;
        Quaternion faceDirection = Quaternion.LookRotation(inputDir, Vector3.up);
        transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .001f);
    }

    void IdleAnim()
    {
        EaseAllJointsToStart(.001f);
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
        Quaternion rightShoulderGoal = Quaternion.Euler(138, 71, 150);
        Quaternion rightElbowGoal = Quaternion.Euler(73, 20, -85);

        //rightShoulder.EaseToNewRotation(rightShoulderGoal, .001f);
        //rightElbow.EaseToNewRotation(rightElbowGoal, .001f);

        rightShoulder.SetCurrentRotation(rightShoulderGoal);
        rightElbow.SetCurrentRotation(rightElbowGoal);
    }

    void SetAllJointsToStart()
    {
        spine1.ResetToStart();
        spine2.ResetToStart();
        spine3.ResetToStart();

        leftShoulder.ResetToStart();
        leftElbow.ResetToStart();
        leftWrist.ResetToStart();
        rightShoulder.ResetToStart();
        rightElbow.ResetToStart();
        rightWrist.ResetToStart();

        leftHip.ResetToStart();
        leftKnee.ResetToStart();
        leftAnkle.ResetToStart();
        rightHip.ResetToStart();
        rightKnee.ResetToStart();
        rightAnkle.ResetToStart();
    }
    void EaseAllJointsToStart(float timer)
    {
        spine1.EaseToStartRotation(timer);
        spine2.EaseToStartRotation(timer);
        spine3.EaseToStartRotation(timer);

        leftShoulder.EaseToStartRotation(timer);
        leftElbow.EaseToStartRotation(timer);
        leftWrist.EaseToStartRotation(timer);
        rightShoulder.EaseToStartRotation(timer);
        rightElbow.EaseToStartRotation(timer);
        rightWrist.EaseToStartRotation(timer);

        leftHip.EaseToStartRotation(timer);
        leftKnee.EaseToStartRotation(timer);
        leftAnkle.EaseToStartRotation(timer);
        rightHip.EaseToStartRotation(timer);
        rightKnee.EaseToStartRotation(timer);
        rightAnkle.EaseToStartRotation(timer);
    }
}
