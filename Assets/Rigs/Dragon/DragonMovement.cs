using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DragonMovement : MonoBehaviour
{
    public Joint leftLeg, leftArm, rightLeg, rightArm, neck;
    private TwoBoneIKConstraint leftLegConstraint, leftArmConstraint, rightLegConstraint, rightArmConstraint, neckConstraint;
    
    private DragonAI controller;
    private Animator masterAnim;

    private float moveTimer = 10f;

    private float walkTimer = 0;
    private float attackCooldown = 7;

    private Vector3 prevLocation;
    private Vector3 currLocation;
    private Vector3 nextLocation;

    private Vector3 clawControl, clawEnd;
    private bool hasSetClaw = false;
    private Joint currClaw;

    private Vector3 moveDir;

    private PlayerMovement playerRef;

    public bool isAttackingBreath;
    public bool isAttackingClaw;
    public bool isStunned;
    public bool isInCutscene = false;

    public bool isDead = false;

    private float breathTimer;
    private float clawTimer;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<DragonAI>();
        masterAnim = GetComponent<Animator>();

        leftLegConstraint = leftLeg.GetComponent<TwoBoneIKConstraint>();
        leftArmConstraint = leftArm.GetComponent<TwoBoneIKConstraint>();
        rightLegConstraint = rightLeg.GetComponent<TwoBoneIKConstraint>();
        rightArmConstraint = rightArm.GetComponent<TwoBoneIKConstraint>();
        neckConstraint = neck.GetComponent<TwoBoneIKConstraint>();

        playerRef = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!isDead)
        {
            if(!isInCutscene)
            {
                if (moveTimer > 0) moveTimer -= Time.deltaTime;
                else
                {
                    moveTimer = 10;
                    Vector3 randPos = transform.position + new Vector3(Random.Range(-15, 15), transform.position.y, Random.Range(-15, 15));
                    SetNewLocation(randPos);
                }

                if (currLocation != nextLocation && attackCooldown != 0)
                {
                    if (walkTimer < 1) walkTimer += Time.deltaTime / 2;
                    else walkTimer = 1;
                    currLocation = AnimMath.Lerp(prevLocation, nextLocation, walkTimer);

                    moveDir = (nextLocation - currLocation);
                    SetAnimController(moveDir.magnitude);
                    moveDir.Normalize();
                }
                else walkTimer = 1;
                transform.position = currLocation;

                RunAI();


                if (isStunned) AnimDamage();
                else if (isAttackingClaw) AnimClaw();
                else if (isAttackingBreath) AnimBreath();
                else if (walkTimer != 0 && walkTimer != 1) AnimWalk();
                else AnimIdle();
            }
            
        }
        else AnimDeath();
        
    }

    private void RunAI()
    {
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
        else if(attackCooldown < 0)
        {
            if (walkTimer != 0 && walkTimer != 1) return;
            else
            {
                float dist = Vector3.Distance(playerRef.transform.position, transform.position);
                if (dist < 10) isAttackingClaw = true;
                else isAttackingBreath = true;
                attackCooldown = 0;
            }
        }
    }
    void AnimIdle()
    {
        FaceHeadTowardPlayer();
        EaseAllConstraintsToWeight(0, .0001f);
        breathTimer = 0;
        clawTimer = 0;
        SetAnimController("gotHit", isStunned);
    }
    void AnimWalk()
    {
        FaceHeadTowardPlayer();
        EaseAllConstraintsToWeight(0, .0001f);
        Quaternion faceDirection = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .001f);
    }
    void AnimBreath()
    {
        breathTimer += Time.deltaTime;
        FaceHeadTowardPlayer();
        
        
        if(breathTimer > 2.5f)
        {
            isAttackingBreath = false;
            attackCooldown = 8;
        }
        else if (breathTimer > .75f)
        {
            controller.BreathAttack();
        }
        SetAnimController("breathAttack", isAttackingBreath);
    }
    void AnimClaw()
    {
        clawTimer += Time.deltaTime;
        if (!hasSetClaw) SetupClaw();

        FaceHeadTowardPlayer();
        Vector3 playerDir = playerRef.transform.position - transform.position;
        Quaternion faceDirection = Quaternion.LookRotation(playerDir, Vector3.up);
        transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .0001f);

        if (clawTimer <= .6f)
        {
            leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 1, .0001f);
            rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 1, .0001f);
            float p = clawTimer * 4f;
            p = Mathf.Clamp(p, 0, 1);
            currClaw.EaseToNewPosition(FindPointOnCurve(currClaw, p), .001f);

            if (clawTimer > .3f) controller.ClawAttack();
        }
        else if(clawTimer < .7666f)
        {
            currClaw.EaseToStartPosition(.0001f);
            leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, 0, .0001f);
            rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, 0, .0001f);
            
        }
        else
        {
            currClaw = null;
            clawControl = Vector3.zero;
            isAttackingClaw = false;
            hasSetClaw = false;
            controller.clawHitbox.SetActive(false);
            attackCooldown = 8;
        }

        SetAnimController("clawAttack", isAttackingClaw);
    }
    void AnimDamage()
    {
        SetAnimController("gotHit", isStunned);
        EaseAllConstraintsToWeight(0, .0001f);
        
    }
    void AnimDeath()
    {
        SetAnimController("isDead", isDead);
    }

    void SetNewLocation(Vector3 newPos)
    {
        walkTimer = 0;
        prevLocation = currLocation;
        nextLocation = newPos;

    }

    void SetupClaw()
    {
        float randHand = (float)Random.Range(0f, 1f);

        Joint hand = (randHand < .5) ? leftArm : rightArm;
        currClaw = hand;

        clawEnd = new Vector3(0, 1, 8);

        Vector3 dirToEnd = clawEnd - hand.transform.position;
        clawControl = new Vector3(dirToEnd.x * .75f, 3, dirToEnd.z * .75f);
        controller.clawTimer = 0;
        hasSetClaw = true;
    }

    void SetAnimController(string val, bool newVal)
    {
        masterAnim.SetBool(val, newVal);
    }
    void SetAnimController(float newVal)
    {
        masterAnim.SetFloat("speed", newVal);
    }

    void EaseAllConstraintsToWeight(float newWeight, float p, bool doNeck = false)
    {
        leftLegConstraint.weight = AnimMath.Ease(leftLegConstraint.weight, newWeight, p);
        leftArmConstraint.weight = AnimMath.Ease(leftArmConstraint.weight, newWeight, p);
        rightLegConstraint.weight = AnimMath.Ease(rightLegConstraint.weight, newWeight, p);
        rightArmConstraint.weight = AnimMath.Ease(rightArmConstraint.weight, newWeight, p);
        if(doNeck) neckConstraint.weight = AnimMath.Ease(neckConstraint.weight, newWeight, p);
    }

    void FaceHeadTowardPlayer()
    {
        neckConstraint.weight = AnimMath.Ease(neckConstraint.weight, 1, .0001f);

        Vector3 directionToPlayer = playerRef.transform.position - neck.transform.position;
        directionToPlayer.Normalize();
        directionToPlayer.y = 0;
        

        neck.EaseToNewPosition(neck.startPos - directionToPlayer, .001f);

        Quaternion lookRot = Quaternion.FromToRotation(neck.transform.position, playerRef.transform.position);

        lookRot.eulerAngles -= new Vector3(50, 0, 180);

        neck.EaseToNewRotation(lookRot, .001f);

    }
    Vector3 FindPointOnCurve(Joint bone, float p)
    {
        Vector3 a = AnimMath.Lerp(bone.startPos, clawControl, p);
        Vector3 b = AnimMath.Lerp(clawControl, clawEnd, p);

        return AnimMath.Lerp(a, b, p);
    }
}
