using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableObject : MonoBehaviour
{
    public float currHealth, prevHealth, maxHealth;
    public bool isDead = false;


    // Update is called once per frame
    void Update()
    {
        if (prevHealth <= 0) isDead = true;
        
    }

    public void ApplyDamage(int damage)
    {
        prevHealth = currHealth;
        currHealth -= damage;
    }


    
}
