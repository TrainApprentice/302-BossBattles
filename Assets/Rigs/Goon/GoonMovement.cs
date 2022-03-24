using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GoonMovement : MonoBehaviour
{

    private CharacterController pawn;
    private float walkSpeed = 4f;

    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 direction = transform.forward * v + transform.right * h;
        if (direction.sqrMagnitude > 1) direction.Normalize();

        

        pawn.SimpleMove(direction * walkSpeed);
    }
}
