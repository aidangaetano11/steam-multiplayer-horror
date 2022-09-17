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
    public float speedMultiplier;
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
    public Camera ragdollCam;


    [Header("Model")]
    public Collider playerCollider;
    public float playerHeight = 2f;
    public GameObject PlayerModel;
    public static Transform instance;
    public bool ragdollActive = false;

    public MeshRenderer PlayerMesh;
    public Animator anim;


    [Header("RigidBodies")]
    [SerializeField] Collider main_col;
    [SerializeField] Rigidbody main_rb;

    [SerializeField] private Rigidbody[] _ragdollRB;
    [SerializeField] private Rigidbody[] _ragdollLegRB;

    private void Start()
    {
        PlayerModel.SetActive(false);
        anim.enabled = true;

        //main_col.enabled = true;

        foreach (var rb in _ragdollRB)            //enables all limbs rigidbody and collisions
        {
            rb.gameObject.SetActive(true);
            rb.isKinematic = true;
            anim.enabled = true;
        }

        foreach (var rb in _ragdollLegRB)       //disables both legs rigidbody and collisions
        {
            rb.gameObject.SetActive(false);
            rb.isKinematic = false;
        }
    }

    private void Update()
    {
        if (PlayerModel.activeSelf == false) 
        {
            SetPosition();
        }
        if (SceneManager.GetActiveScene().name == "TestMap")
        {
            if (PlayerModel.activeSelf == false)
            {                   
                PlayerModel.SetActive(true);
                instance = this.transform;
                main_rb = GetComponent<Rigidbody>();
                main_rb.gameObject.SetActive(true);
                main_rb.freezeRotation = true;
            }

            if (hasAuthority)       //should be called for individual player
            {
                if (!ragdollActive) 
                {
                    HandleJumping();
                    HandleDrag();
                    GroundCheck();
                }            
            }
        }
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "TestMap")
        {
            if (hasAuthority)
            {
                if (!ragdollActive) 
                {
                    Movement();
                    HandleAnimations();
                }
                HandleRagdoll();
            }
        }
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5,5), 5f, Random.Range(-15,7));
    }

    public void Movement()
    {
        //rb.AddForce(Vector3.down * Time.deltaTime * -gravity);   //extra gravity

        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        moveDirection = transform.forward * zDirection + transform.right * xDirection;


        if (isGrounded)
        {
            main_rb.AddForce(moveDirection.normalized * speed * speedMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            main_rb.AddForce(moveDirection.normalized * speed * speedMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    public void HandleRagdoll() 
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            ragdollActive = !ragdollActive;

            if (ragdollActive)
            {
                EnableRagdoll();
            }
            else
            {
                DisableRagdoll();
            }
        }

        
    }

    public void EnableRagdoll() 
    {
        anim.enabled = false;
        //main_col.enabled = false;

        foreach (var rb in _ragdollRB)
        {
            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
            anim.enabled = false;
        }
    }

    public void DisableRagdoll() 
    {
        anim.enabled = true;
        //main_col.enabled = true;

        foreach (var rb in _ragdollRB)
        {
            rb.gameObject.SetActive(true);
            rb.isKinematic = true;
            anim.enabled = true;
        }

        foreach (var rb in _ragdollLegRB)
        {
            rb.gameObject.SetActive(false);
            rb.isKinematic = false;
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
            main_rb.velocity = new Vector3(main_rb.velocity.x, 0, main_rb.velocity.z);
            main_rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);
        }

    }

    public void HandleDrag()
    {
        if (isGrounded)
        {
            main_rb.drag = movementDrag;
        }
        else
        {
            main_rb.drag = airDrag;
        }

    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
    }
}
