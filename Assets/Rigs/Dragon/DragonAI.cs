using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAI : MonoBehaviour
{
    public int health = 50;
    public HealthManager healthbar;
    private DragonMovement mover;
    public GameObject flameBase;
    public Transform flameStart;
    public GameObject clawHitbox;

    private PlayerMovement playerRef;
    private bool isStunned;
    private float stunTimer = 0;
    private float breathTimer = 0;
    public float clawTimer = 0;

    public bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<DragonMovement>();
        playerRef = FindObjectOfType<PlayerMovement>();
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
        
    }

    public void BreathAttack()
    {
        if (breathTimer > 0) breathTimer -= Time.deltaTime;
        else
        {
            Vector3 dir = playerRef.transform.position - flameStart.transform.position;
            dir.Normalize();
            GameObject newFlame = Instantiate(flameBase, flameStart.position, Quaternion.identity);
            newFlame.GetComponent<FlameBehavior>().direction = dir;
            breathTimer = .03f;
        }
    }
    public void ClawAttack()
    {
        clawTimer += Time.deltaTime;
        if (clawTimer > .3f) clawHitbox.SetActive(false);
        else clawHitbox.SetActive(true);
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
