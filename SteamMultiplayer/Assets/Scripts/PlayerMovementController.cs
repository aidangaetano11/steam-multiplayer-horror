using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;
public class PlayerMovementController : NetworkBehaviour
{
    [Header("Movement")]
    public float speed;
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float speedMultiplier;
    public float stamina = 100f;
    public float staminaMax = 100f;
    public float staminaDrain = 1f;
    public float staminaRechargeTime = 5f;
    public bool isSprinting = false;
    [SerializeField] float airMultiplier = 0.4f;

    [Header("Drag")]
    public float movementDrag;
    public float airDrag;

    [Header("Jumping")]
    public float jumpforce;         //dont really have jumping

    [Header("Gravity")]
    public float gravity;
    public float groundDistance;
    public LayerMask groundMask;
    [SerializeField] private bool isGrounded;

    Vector3 moveDirection;
    private Vector3 velocity = Vector3.zero;


    [Header("Camera")]
    public Camera cam;
    

    [Header("Model")]
    public Collider playerCollider;
    public float playerHeight = 2f;
    public GameObject PlayerModel;
    public static Transform instance;

    public Text playerNameText;

    public MeshRenderer PlayerMesh;
    public Material[] playerColors;

    Rigidbody rb;

    public Light flashlight;
    [SyncVar (hook=nameof(OnChangeFlashlight))]
    public bool flashlightEnabled;

    public void OnChangeFlashlight(bool oldValue, bool newValue)
    {
        flashlight.enabled = newValue;
    }

    private void Start()
    {
        PlayerModel.SetActive(false);
        cam = GetComponentInChildren<Camera>();
        flashlight = GetComponentInChildren<Light>();
        stamina = staminaMax;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game") 
        {
            if (PlayerModel.activeSelf == false) 
            {
                SetPosition();                      //this is called for every player on the scene
                PlayerModel.SetActive(true);
                instance = this.transform;
                rb = GetComponent<Rigidbody>();
                rb.freezeRotation = true;
                PlayerCosmeticsSetup();
            }

            if (hasAuthority)       //should be called for individual player
            {
                HandleJumping();
                HandleDrag();
                HandleSprintCrouch();
                GroundCheck();
            }
            HandleFlashlight();
        }
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {


            if (hasAuthority)
            {
                Movement();
            }
        }
    }

    public void SetPosition() 
    {
        transform.position = new Vector3(0f, 10f, 0f);
    }

    public void Movement() 
    {
        //rb.AddForce(Vector3.down * Time.deltaTime * -gravity);   //extra gravity

        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        moveDirection = transform.forward * zDirection + transform.right * xDirection;

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * speed * speedMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded) 
        {
            rb.AddForce(moveDirection.normalized * speed * speedMultiplier * airMultiplier, ForceMode.Acceleration);
        }
        
    }

    public void HandleJumping() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);
        }
        
    }

    public void HandleDrag() 
    {
        if (isGrounded)
        {
            rb.drag = movementDrag;
        }
        else 
        {
            rb.drag = airDrag;
        }
        
    }

    public void HandleSprintCrouch() 
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina > 0)
            {
                isSprinting = true;
                speed = sprintSpeed;
                stamina -= staminaDrain;
            }
            else
            {
                isSprinting = false;
                speed = walkSpeed;
                StartCoroutine("RechargeStamina", staminaRechargeTime);
            }
        }

        else if (Input.GetKey(KeyCode.LeftControl))
        {
            speed = crouchSpeed;
            PlayerMesh.transform.localScale = new Vector3(1f, 0.5f, 1f);
            cam.transform.localPosition = new Vector3(0f, 0.2f, 0.15f);
        }
        else
        {
            speed = walkSpeed;
            PlayerMesh.transform.localScale = new Vector3(1f, 1f, 1f);
            cam.transform.localPosition = new Vector3(0f, 0.77f, 0.15f);

        }
    }

    IEnumerator RechargeStamina(float delay)             //After stamina recharge time, it will reset stamina back to max stamina
    {
        while (true) 
        {
            yield return new WaitForSeconds(delay);         
            stamina = staminaMax;
            StopCoroutine("RechargeStamina");
        }
    }

    public void HandleFlashlight() 
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isServer)
            {
                if (flashlightEnabled)
                {
                    flashlightEnabled = false;
                }
                else
                {
                    flashlightEnabled = true;
                }
            }
            else CmdHandleFlashlight();
        }      
    }

    [Command]
    public void CmdHandleFlashlight() 
    {
        HandleFlashlight();
    }

    private void GroundCheck() 
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
    }

    public void PlayerCosmeticsSetup() 
    {
        PlayerMesh.material = playerColors[GetComponent<PlayerObjectController>().playerColor];
        playerNameText.text = GetComponent<PlayerObjectController>().PlayerName;
        playerNameText.color = Color.red;
        Debug.Log(playerColors[GetComponent<PlayerObjectController>().playerColor]);
    }
}
