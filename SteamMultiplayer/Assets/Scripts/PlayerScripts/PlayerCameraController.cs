using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private float mouseSens;     //300f
    [SerializeField] private float multiplier;    //0.01f

    [SerializeField] private Transform arms;
    [SerializeField] private Transform body;

    [SerializeField] Camera cam;
    [SerializeField] Camera ragdollCam;

    [SerializeField] PlayerMovementController playerMovement;
    private AudioListener audioListener;

    private float xRot;
    private float yRot;

    private void Awake()
    {
        audioListener = GetComponentInChildren<AudioListener>();
        playerMovement = GetComponentInChildren<PlayerMovementController>();
    }

    private void Start()
    {
        cam.enabled = false;
        ragdollCam.enabled = false;
        audioListener.enabled = false;
    }

    private void Update()
    {

        if (SceneManager.GetActiveScene().name == "TestMap")
        {
            if (hasAuthority)     //individual player
            {
                if (cam.enabled == false && !playerMovement.ragdollActive)
                {
                    cam.enabled = true;
                    LockCursor();
                }
                if (audioListener.enabled == false)
                {
                    audioListener.enabled = true;
                }

                if (!playerMovement.ragdollActive) 
                {
                    HandleMouseLook();
                }

                if (playerMovement.ragdollActive)
                {
                    cam.enabled = false;
                    ragdollCam.enabled = true;
                }
                else 
                {
                    cam.enabled = true;
                    ragdollCam.enabled = false;
                }

            }

        }

    }

    private void HandleMouseLook()
    {
        cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yRot += mouseX * mouseSens * multiplier;
        xRot -= mouseY * mouseSens * multiplier;

        xRot = Mathf.Clamp(xRot, -90f, 90f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
