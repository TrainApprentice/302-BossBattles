using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GoonMovement : MonoBehaviour
{

    public FootRaycast footLeft, footRight;

    private CharacterController pawn;
    private float walkSpeed = 4f;

    public float walkSpreadX = .2f;
    public float walkSpreadZ = .7f;

    float walkTimer = 0;
    float idleTimer = 0;

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

        if (direction.magnitude > 0) AnimWalk();
        else IdleAnim();
    }
    delegate void MoveFoot(FootRaycast foot, float x, float time);

    void AnimWalk()
    {
        walkTimer += Time.deltaTime * walkSpeed;

        MoveFoot footWave = (foot, x, time) => {
            float y = Mathf.Cos(time) * .4f * walkSpreadZ;
            float z = Mathf.Sin(time) * .6f * walkSpreadZ;

            if (y < 0) y = 0;
            y += .177f;

            foot.transform.localPosition = new Vector3(x, y, z);
        };

        footWave.Invoke(footLeft, -walkSpreadX, walkTimer);
        footWave.Invoke(footRight, walkSpreadX, walkTimer + Mathf.PI);
    }

    void IdleAnim()
    {
        


    }
}
