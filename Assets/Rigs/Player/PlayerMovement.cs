using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;



[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    enum Mode
    {
        Idle,
        Walk,
        Air,
        Block,
        Hit,
        BlockedHit,
        LightAttack,
        HeavyAttack
    }
    private Mode currState;
    private GameObject cam;
    CharacterController pawn;
    Animator animController;

    public float health = 50;
    public HealthManager healthbar;
    
    private float speed = 7f;
    private int currAttack = 0; // 1 for light, 2 for heavy, 0 for none
    private float lightAttackTimer = 0;
    private float heavyAttackTimer = 0;
    private float invTimer = 0;
   
    public Joint leftLeg, rightLeg, rightArm, leftArm;
    private TwoBoneIKConstraint leftLegConstraint, rightLegConstraint, leftArmConstraint, rightArmConstraint;

    public Transform leftFoot, rightFoot;
    public PlayerCombatHitbox rightFootHit, leftHandHit;

    public bool isDead = false;
    public bool isInvincible = false;
    public bool isGrounded = true;

    private bool isBlocking = false;
    
    private bool dieOnce = true;
    PlayerTargeting playerTargeting;

    float airAnimTimer = 0f;
    bool hasResetLegs = false;
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

        leftArmConstraint = leftArm.GetComponent<TwoBoneIKConstraint>();
        rightArmConstraint = rightArm.GetComponent<TwoBoneIKConstraint>();
        leftLegConstraint = leftLeg.GetComponent<TwoBoneIKConstraint>();
        rightLegConstraint = rightLeg.GetComponent<TwoBoneIKConstraint>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (Input.GetKeyDown("l")) ApplyDamage(10);
            // Movement
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            bool playerIsAiming = (playerTargeting && playerTargeting.playerWantsToAim && playerTargeting.target);
            isBlocking = Input.GetKey("left ctrl");

            bool playerWantsToLightAttack = Input.GetMouseButtonDown(0);
            bool playerWantsToHeavyAttack = Input.GetMouseButtonDown(1);
            if (playerWantsToLightAttack) currAttack = 1;
            else if (playerWantsToHeavyAttack) currAttack = 2;
            //else currAttack = 0;
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

            if (invTimer > 0) invTimer -= Time.deltaTime;
            else
            {
                invTimer = 0;
                isInvincible = false;
            }
            pawn.Move(moveAmt * Time.deltaTime);

            if (isInvincible) currState = (isBlocking) ? Mode.BlockedHit : Mode.Hit;
            else if (isBlocking) currState = Mode.Block;
            else if (currAttack != 0) currState = (currAttack == 1) ? Mode.LightAttack : Mode.HeavyAttack;
            else if (isGrounded && inputDir != Vector3.zero) currState = Mode.Walk;
            else if (!isGrounded) currState = Mode.Air;
            else currState = Mode.Idle;

            StateMachine();
        }
        else DeathAnim();

    }

    
    void StateMachine()
    {
        switch (currState)
        {
            case Mode.Idle:
                IdleAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 0, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 0, .0001f);
                leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 0, .0001f);
                rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 0, .0001f);
                break;
            case Mode.Walk:
                WalkAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 0, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 0, .0001f);
                leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 0, .0001f);
                rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 0, .0001f);
                break;
            case Mode.Air:
                AirAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 0, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 0, .0001f);
                break;
            case Mode.Block:
                BlockAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 1, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 1, .0001f);
                leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 0, .0001f);
                rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 0, .0001f);
                break;
            case Mode.LightAttack:
                LightAttackAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 0, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 0, .0001f);
                leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 0, .0001f);
                rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 0, .0001f);
                break;
            case Mode.HeavyAttack:
                HeavyAttackAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 0, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 0, .0001f);
                leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 0, .0001f);
                rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 0, .0001f);
                break;
            case Mode.Hit:
                DamageAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 0, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 0, .0001f);
                leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 0, .0001f);
                rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 0, .0001f);
                break;
            case Mode.BlockedHit:
                BlockedDamageAnim();
                leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 1, .0001f);
                rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 1, .0001f);
                leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 1, .0001f);
                rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 1, .0001f);
                break;

        }
        animController.SetBool("isBlocking", isBlocking);
        animController.SetBool("isGrounded", isGrounded);
        animController.SetInteger("currAttack", currAttack);
        animController.SetBool("gotHit", isInvincible);
        animController.SetBool("isDead", isDead);
    }

    void ApplyDamage(int damage)
    {
        if(!isInvincible)
        {
            health -= (isBlocking) ? damage/2 : damage;
            if (health > 0)
            {
                isInvincible = true;
                invTimer = (isBlocking) ? .75f : 1.25f;
            }
            else isDead = true;
        }
        healthbar.SetHealth(health);
        
    }

    void DamageAnim()
    {
        speed = 0;
        animController.speed = .75f;
    }
    void BlockedDamageAnim()
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
    void WalkAnim()
    {
        speed = 7f;
        Quaternion faceDirection = Quaternion.LookRotation(inputDir, Vector3.up);
        transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .001f);
        leftLeg.LockToTarget();
        rightLeg.LockToTarget();

        airAnimTimer = 0;
        lightAttackTimer = 0;
        heavyAttackTimer = 0;
    }

    void IdleAnim()
    {
        animController.speed = 1;
        leftLeg.LockToTarget();
        rightLeg.LockToTarget();
        leftArm.EaseToStartPosition(.001f);
        leftArm.EaseToStartRotation(.001f);
        rightArm.EaseToStartRotation(.001f);
        rightArm.EaseToStartPosition(.001f);
        speed = 7f;

        airAnimTimer = 0;
        lightAttackTimer = 0;
        heavyAttackTimer = 0;
    }
    void AirAnim()
    {
        animController.speed = 1;
        airAnimTimer += Time.deltaTime;
        if(airAnimTimer < .2f)
        {
            leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 0, .0001f);
            rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 0, .0001f);
        }
        
        else if(airAnimTimer < .8f)
        {
            leftLeg.LockToTarget();
            rightLeg.LockToTarget();
        }
        else if(airAnimTimer < 1.8f)
        {
            leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, 1, .0001f);
            rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, 1, .0001f);

            float wave = Mathf.Sin(airAnimTimer * 10) * .15f;


            Vector3 leftLegGoalPos = leftLeg.transform.localPosition + new Vector3(0, wave, 0); 
            Vector3 rightLegGoalPos = new Vector3(-.25f, .25f, .4f);
            leftLeg.EaseToNewPosition(leftLegGoalPos, .001f);
            rightLeg.EaseToNewPosition(rightLegGoalPos, .001f);
            
        }
        else
        {
            leftLeg.EaseToStartPosition(.001f);
            rightLeg.EaseToStartPosition(.001f);
        }
        if (airAnimTimer > 1.83f) hasResetLegs = false;
        
    }
    void DeathAnim()
    {
        if(!dieOnce)
        {
            speed = 0;
            animController.SetBool("isDead", isDead);
            isInvincible = false;
            dieOnce = true;
        }
        
    }
    void BlockAnim()
    {
        animController.speed = 1;
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

    void LightAttackAnim()
    {
        animController.speed = 1;
        speed = 0;
        currAttack = 1;
        lightAttackTimer += Time.deltaTime;
        rightFootHit.ActivateHitbox();

        if (lightAttackTimer > 1.2f)
        {
            currAttack = 0;
            rightFootHit.DeactivateHitbox();
        }
        
    }
    void HeavyAttackAnim()
    {
        animController.speed = 1;
        speed = 0;
        currAttack = 2;
        heavyAttackTimer += Time.deltaTime;
        leftHandHit.ActivateHitbox();

        if (heavyAttackTimer > 1.6f)
        {
            leftHandHit.DeactivateHitbox();
            currAttack = 0;
        }
    }

    
}
