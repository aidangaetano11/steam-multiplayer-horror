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
    [SerializeField] private bool canUseHeadbob = true; 
    public bool isSprinting = false;
    public bool isCrouching = false;
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
    public bool isGrounded;

    Vector3 moveDirection;
    private Vector3 velocity = Vector3.zero;


    [Header("Camera and Headbob")]
    public Camera cam;
    public float walkBobSpeed = 14f;
    public float walkBobAmount = 0.005f;
    public float sprintBobSpeed = 18f;
    public float sprintBobAmount = 0.011f;
    public float crouchBobSpeed = 8f;
    public float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;


    [Header("Model")]
    public Collider playerCollider;
    public float playerHeight = 2f;
    public GameObject PlayerModel;
    public static Transform instance;

    public Text playerNameText;
    public Light playerLight;

    public SkinnedMeshRenderer PlayerMesh;
    public Animator anim;
    public Material[] playerColors;

    Rigidbody rb;

    public GameObject deathPanel;

    [SyncVar (hook =nameof(OnDeath))]
    public bool isDead = false;

    [Header("Audio")]
    public AudioSource heartbeatSource;
    public bool isPlayingHeartbeat = false;


    public void OnDeath(bool oldValue, bool newValue) 
    {
        isDead = newValue;
        if (isDead)
        {
            playerLight.enabled = false;
            PlayerMesh.enabled = false;
            playerNameText.enabled = false;
            gameObject.GetComponent<Interactor>().enabled = false;
            if (isLocalPlayer) deathPanel.SetActive(true);
        }
        else 
        {
            playerLight.enabled = true;
            if (!isLocalPlayer) playerLight.enabled = false;
            PlayerMesh.enabled = true;
            if (isLocalPlayer) PlayerMesh.enabled = false;
            playerNameText.enabled = true;
            gameObject.GetComponent<Interactor>().enabled = true;
            if (isLocalPlayer) deathPanel.SetActive(false);
        }
        
     
    }

    private void Start()
    {
        PlayerModel.SetActive(false);
        cam = GetComponentInChildren<Camera>();
        defaultYPos = cam.transform.localPosition.y;
        stamina = staminaMax;
        anim.enabled = true;
        deathPanel.SetActive(false);
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (isLocalPlayer)   //plays hearbeat sound if is close to monster
            {
                Vector3 distanceToMonster = FindObjectOfType<MonsterAI>().transform.position - transform.position;

                if (distanceToMonster.magnitude < 15f)
                {
                    if (!isPlayingHeartbeat)
                    {
                        heartbeatSource.Play();
                        isPlayingHeartbeat = true;
                    }
                }
                else
                {
                    heartbeatSource.Stop();
                    isPlayingHeartbeat = false;
                }
            }

            if (PlayerModel.activeSelf == false && !isDead)
            {
                SetPosition();                      //this is called for every player on the scene
                PlayerModel.SetActive(true);
                instance = this.transform;
                rb = GetComponent<Rigidbody>();
                rb.freezeRotation = true;
                PlayerCosmeticsSetup();

                if (isLocalPlayer) PlayerMesh.enabled = false;
                else playerLight.enabled = false;
            }

            if (hasAuthority)       //should be called for individual player
            {
                HandleJumping();
                HandleDrag();
                HandleSprintCrouch();
                GroundCheck();

                if (canUseHeadbob)
                    HandleHeadbob();
            }
        }
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {           
            if (hasAuthority)
            {
                Movement();
                HandleAnimations();
            }
        }
    }


    public void SetPosition()    //called on game load
    {
        if (isServer)      //if we are server, we will call rpc to move clients
        {
            RpcSetPosition();   
        }
        else CmdSetPosition();   //if we are client, we will call command
    }

    [ClientRpc]
    public void RpcSetPosition()    //called on every client to move to this position
    {
        transform.position = new Vector3(0f, 1f, 0f);
    }

    [Command]
    public void CmdSetPosition()     //if we are client, we will send command to same function to act as server to teleport clients to spawn
    {
        SetPosition();
    }

    public void Movement()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * -gravity);   //extra gravity

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
            rb.AddForce(Vector3.down * Time.deltaTime * -gravity);   //extra gravity
        }

    }

    public void HandleAnimations() 
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            anim.SetInteger("MoveState", 1);
            anim.SetBool("IsMoving", true);
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            anim.SetInteger("MoveState", 0);
            anim.SetBool("IsMoving", true);
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
        {
            anim.SetInteger("MoveState", 2);
            anim.SetBool("IsMoving", true);
        }
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D))
        {
            anim.SetInteger("MoveState", 3);
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
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

    public void HandleHeadbob() 
    {
        if (!isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f) 
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            cam.transform.localPosition = new Vector3(
                cam.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),
                cam.transform.localPosition.z);
        }
    }

    public void HandleSprintCrouch() 
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina > 0)
            {
                isSprinting = true;
                anim.speed = 1.5f;
                speed = sprintSpeed;
                //stamina -= staminaDrain;
            }
            else
            {            
                speed = walkSpeed;
                StartCoroutine("RechargeStamina", staminaRechargeTime);
            }
        }

        else if (Input.GetKey(KeyCode.LeftControl))
        {
            speed = crouchSpeed;
            isCrouching = true;
            //PlayerMesh.transform.localScale = new Vector3(1f, 0.5f, 1f);
            //cam.transform.localPosition = new Vector3(0f, 0.2f, 0.15f);
        }
        else
        {
            speed = walkSpeed;
            isCrouching = false;
            isSprinting = false;
            //PlayerMesh.transform.localScale = new Vector3(1f, 1f, 1f);
            //cam.transform.localPosition = new Vector3(0f, 0.77f, 0.15f);
            anim.speed = 1;

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


    private void GroundCheck() 
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
    }

    public void PlayerCosmeticsSetup() 
    {
        PlayerMesh.material = playerColors[GetComponent<PlayerObjectController>().playerColor];
        playerNameText.text = GetComponent<PlayerObjectController>().PlayerName;
        playerNameText.color = Color.white;

        if (isLocalPlayer) 
        {
            playerNameText.enabled = false;   //disables name text from local player view
        }
    }
}
