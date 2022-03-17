using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GuardianMovement : MonoBehaviour
{
    public float walkSpeed = 3f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 direction = new Vector3(h, 0, v);


        transform.position += direction * walkSpeed * Time.deltaTime;

        animator.SetFloat("Speed", direction.magnitude * walkSpeed);
    }
}
