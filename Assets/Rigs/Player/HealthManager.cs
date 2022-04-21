using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public GameObject target;
    public Image healthBar;
    private PlayerMovement player;
    // Add enemyAI when created

    private float prevHealth;
    private float currHealth;
    private float maxHealth;
    void Start()
    {
        if (target.GetComponent<PlayerMovement>())
        {
            player = target.GetComponent<PlayerMovement>();
            maxHealth = player.health;
        }

        currHealth = maxHealth;
        prevHealth = maxHealth;

    }

    private void Update()
    {
        if(currHealth != prevHealth) prevHealth = AnimMath.Ease(prevHealth, currHealth, .001f);
        if (Mathf.Abs(prevHealth - currHealth) < .001f) prevHealth = currHealth;
        
        float p = prevHealth / maxHealth;
        healthBar.transform.localScale = new Vector3(p, 1, 1);
    }

    public void SetHealth(float newHealth)
    {
        prevHealth = currHealth;
        currHealth =newHealth;
    }

    
}
