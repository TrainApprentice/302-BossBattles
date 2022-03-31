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

    /*
    public Transform jointHips, jointHipLeft, jointHipRight, jointKneeLeft, jointKneeRight;
    public Transform jointSpine, jointNeck, jointHairLeft, jointHairRight;
    public Transform jointShoulderLeft, jointShoulderRight, jointElbowLeft, jointElbowRight;
    public Transform skeletonBase;
    */

    public Transform jointNeck;
    public bool isDead = false;
    public bool isInvincible = false;
    public bool isGrounded = true;

    private bool isBlocking = false;

    
    private bool dieOnce = true;
    PlayerTargeting playerTargeting;


    float walkAnimTimer = 0f;
    float idleAnimTimer = 0f;
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
            /*
            if (playerIsAiming && currDodgeCooldown <= 0)
            {
                Vector3 toTarget = playerTargeting.target.transform.position - transform.position;
                toTarget.Normalize();
                Quaternion worldRot = Quaternion.LookRotation(toTarget);
                Vector3 euler = worldRot.eulerAngles;
                euler.x = 0;
                euler.z = 0;
                worldRot.eulerAngles = euler;
                transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .001f);

            }
            */
            /*
            if (cam && (h != 0 || v != 0))
            {
                float playerYaw = transform.eulerAngles.y;
                float camYaw = cam.transform.eulerAngles.y;

                camYaw = AnimMath.AngleWrapDegrees(playerYaw, camYaw);

                Quaternion playerRot = Quaternion.Euler(0, playerYaw, 0);
                Quaternion targetRot = Quaternion.Euler(0, camYaw, 0);
                transform.rotation = AnimMath.Ease(playerRot, targetRot, .001f);
            }
            */
            if(h!= 0 || v!= 0)
            {
                if ((pawn.collisionFlags == CollisionFlags.None)) isGrounded = false;
            }

            Vector3 forwardV = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            Vector3 rightV = Vector3.Cross(Vector3.up, forwardV);
            //Vector3 rightV = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
            inputDir = (forwardV * v + rightV * h);

            if (inputDir.sqrMagnitude > 1) inputDir.Normalize();

            


            if (pawn.collisionFlags == CollisionFlags.Below) isGrounded = true;
            
            //isGrounded = (pawn.isGrounded || transform.position.y <= -1.42f || isOnObstacle);
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

            if (isGrounded && inputDir != Vector3.zero && currDodgeCooldown <= 0) currState = Mode.Walk;
            else if (isBlocking) currState = Mode.Block;
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
    }

    void WalkAnim()
    {
        Quaternion faceDirection = Quaternion.LookRotation(inputDir, Vector3.up);
        transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .001f);
    }

    void IdleAnim()
    {
        
    }
    void AirAnim()
    {
       
    }
    void DeathAnim()
    {
        
    }
    void BlockAnim()
    {
        
    }
}
