using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController: MonoBehaviour
{
    public PlayerTargeting player;
    public PlayerMovement playerController;

    public float mouseSensitivityX = 2f;
    public float mouseSensitivityY = 2f;
    public float scrollSensitivity = 1f;
    public Vector3 cameraOffset;

    private float shakeTimer = 0f;
    private float shakeAmount = 0f;

    private Camera cam;

    private float pitch = 0, yaw = 0;
    private float zoom = 10;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        
        if (!playerController) playerController = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isAiming = (player && player.target && player.playerWantsToAim && !playerController.isDead && !playerController.isInvincible);

        // Position
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.transform.position) > .01f) transform.position = AnimMath.Ease(transform.position, player.transform.position + cameraOffset, .001f);
        else transform.position = player.transform.position + cameraOffset;




        float playerYaw = player.transform.eulerAngles.y;
        playerYaw = AnimMath.AngleWrapDegrees(yaw, playerYaw);
        // Rig Rotation

        if (isAiming)
        {
            Quaternion tempTarget = Quaternion.Euler(20, playerYaw, 0);
            
            transform.rotation = AnimMath.Ease(transform.rotation, tempTarget, .001f);
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivityX; // Yaw (y)
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY; // Pitch (x)

            pitch = Mathf.Clamp(pitch, -89, 89);
            

            transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
        }
        




        // Dolly

        Vector2 scrollAmt = Input.mouseScrollDelta;
        zoom -= scrollAmt.y * scrollSensitivity;

        zoom = Mathf.Clamp(zoom, 2, 20);

        

        float z = (isAiming) ? -3 : -zoom;

        cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, z), .01f);

        // Rotate ONLY the camera

        if(isAiming)
        {
            Vector3 toAimTarget = player.target.transform.position - cam.transform.position;
            Quaternion worldRot = Quaternion.LookRotation(toAimTarget);

            Quaternion localRot = worldRot;

            if(cam.transform.parent)
            {
                localRot = Quaternion.Inverse(cam.transform.parent.rotation) * worldRot;
            }
            Vector3 euler = localRot.eulerAngles;
            euler.z = 0;
            localRot.eulerAngles = euler;

            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, localRot, .001f);
        }
        else
        {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, .001f);
        }

        UpdateShake();

    }
    void UpdateShake()
    {
        if (shakeTimer < 0) return;
        shakeTimer -= Time.deltaTime;

        Quaternion randomRot = AnimMath.Lerp(Random.rotation, Quaternion.identity, shakeAmount);

        cam.transform.localRotation *= randomRot;
        
    }
    public void DoShake(float time, float shakeAmt)
    {
        if(time > shakeTimer) shakeTimer = time;
        shakeAmount = shakeAmt;
    }

}
