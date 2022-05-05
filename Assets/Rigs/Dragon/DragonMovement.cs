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
    private float speed = 5f;

    private Vector3 prevLocation;
    private Vector3 currLocation;
    private Vector3 nextLocation;

    private Vector3 moveDir;

    private PlayerMovement playerRef;

    public bool isAttackingBreath;
    public bool isAttackingClaw;
    public bool isStunned;

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
            if (moveTimer > 0) moveTimer -= Time.deltaTime;
            else
            {
                moveTimer = 10;
                Vector3 randPos = transform.position + new Vector3(Random.Range(-15, 15), transform.position.y, Random.Range(-15, 15));
                SetNewLocation(randPos);
            }
            if (currLocation != nextLocation)
            {
                if (walkTimer < 1) walkTimer += Time.deltaTime/2;
                else walkTimer = 1;
                currLocation = AnimMath.Lerp(prevLocation, nextLocation, walkTimer);

                moveDir = (nextLocation - currLocation);
                SetAnimController(moveDir.magnitude);
                moveDir.Normalize();
            }
            else walkTimer = 1;
            transform.position = currLocation;

            // DEBUG ONLY
            if (Input.GetKeyDown("b"))
            {
                isAttackingBreath = true;
            }

            
            if (isStunned) AnimDamage();
            else if (isAttackingClaw) AnimClaw();
            else if (isAttackingBreath) AnimBreath();
            else if (walkTimer != 0 && walkTimer != 1) AnimWalk();
            else AnimIdle();
        }
        else AnimDeath();
        
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
        }
        SetAnimController("breathAttack", isAttackingBreath);
    }
    void AnimClaw()
    {

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
}
