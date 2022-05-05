using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class PlayerCombatHitbox : MonoBehaviour
{
    public int damage = 0;
    private SphereCollider hitbox;
    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<SphereCollider>();
        DeactivateHitbox();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<DragonAI>().ApplyDamage(damage);
        }
    }

    public void ActivateHitbox()
    {
        if (hitbox.enabled) return;

        hitbox.enabled = true;
    }

    public void DeactivateHitbox()
    {
        if (!hitbox.enabled) return;

        hitbox.enabled = false;
    }
}
