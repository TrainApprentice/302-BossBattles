using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAI : MonoBehaviour
{
    public int health = 50;
    public HealthManager healthbar;
    private DragonMovement mover;

    private bool isAttackingBreath;
    private bool isAttackingClaw;
    private bool isStunned;
    private float stunTimer = 0;
    private float breathTimer = 0;

    private bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<DragonMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateMover();
        mover.isStunned = isStunned;

        if (Input.GetKeyDown("h")) ApplyDamage(10);
        if (stunTimer > 0) stunTimer -= Time.deltaTime;
        else
        {
            isStunned = false;
            stunTimer = 0;
        }
        print(isStunned + ", " + stunTimer);
        
    }

    private void BreathAttack()
    {
        
    }
    private void ClawAttack()
    {

    }

    private void UpdateMover()
    {
        mover.isAttackingBreath = isAttackingBreath;
        mover.isAttackingClaw = isAttackingClaw;
        mover.isStunned = isStunned;
        mover.isDead = isDead;
    }

    public void ApplyDamage(int damage) 
    {
        if(!isStunned)
        {
            health -= damage;
            healthbar.SetHealth(health);
            isStunned = true;
            stunTimer = .5f;
        }
        
        if (health <= 0) isDead = true;
        mover.isDead = isDead;
    }
}
