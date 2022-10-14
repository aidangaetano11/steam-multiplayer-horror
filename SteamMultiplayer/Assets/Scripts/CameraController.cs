using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class CameraController : NetworkBehaviour
{
    public float mouseSens;     //300f
    [SerializeField] private float multiplier;    //0.01f

    [SerializeField] private Transform arms;
    [SerializeField] private Transform body;

    Camera cam;
    private AudioListener audioListener;

    private float xRot;
    private float yRot;

    private void Awake()
    {
        audioListener = GetComponentInChildren<AudioListener>();
        cam = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        cam.enabled = false;
        audioListener.enabled = false;
    }

    private void Update()
    {
        
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (hasAuthority)     //individual player
            {
                if (PauseMenu.GameIsPaused == false) 
                {
                    LockCursor();
                }

                if (cam.enabled == false) 
                {
                    cam.enabled = true;
                }
                if (audioListener.enabled == false) 
                {
                    audioListener.enabled = true;
                }
                HandleMouseLook();

                
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

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
