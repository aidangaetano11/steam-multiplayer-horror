using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Mirror;
public class MonsterAI : NetworkBehaviour
{
    public NavMeshAgent agent;
    public Collider monsterCollider;
    private FieldOfView fov;
    public MonsterAudio monsterAudio;

    public Transform player;
    public Transform[] waypoints;

    public LayerMask whatIsGround, whatIsPlayer;

    public float extendedChaseTime = 5f;

    public bool justKilled = false;

    [Header("Monster Body")]
    public Transform neck;
    public Transform spine;

    [Header("Leg Manager")]
    public List <GroundSearchManager> legs = new List<GroundSearchManager> ();
    public float legDelay = 0.1f;

    [Header("Movement")]
    public float walkSpeed = 8f;
    public float runSpeed = 15f;

    [Header("States")]
    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public bool hasNewTarget = false;

    //Attacking


    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool wasChasingPlayer;


    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        monsterAudio = GetComponent<MonsterAudio>();
    }

    private void Start()
    {
        legs[0].hasMoved = true;
        legs[1].hasMoved = false;
        legs[2].hasMoved = false;
        legs[3].hasMoved = false;
    }

    private void FixedUpdate()
    {
        if (legs[1].hasMoved && legs[0].distanceFromRaycast > legs[0].maxDistanceFromBody)
        {
            legs[1].hasMoved = false;
            legs[3].hasMoved=false;
            legs[0].StartCoroutine("MoveTargetToRaycast", legDelay);
            legs[2].StartCoroutine("MoveTargetToRaycast", legDelay);
        }
        else if (legs[0].hasMoved && legs[1].distanceFromRaycast > legs[1].maxDistanceFromBody)
        {
            legs[0].hasMoved = false;
            legs[2].hasMoved = false;
            legs[1].StartCoroutine("MoveTargetToRaycast", legDelay);
            legs[3].StartCoroutine("MoveTargetToRaycast", legDelay);
        }
    }

    private void Update()
    {
        FindClosestPlayer();

        //Check for sight and attack range
        if (fov.visibleTargets.Count > 0)
        {
            playerInSightRange = true;
            fov.viewAngle = 360;
            //Debug.Log("Player in Sight");
        }
        else 
        {
            playerInSightRange = false;
            fov.viewAngle = 120;
            //Debug.Log("Player out of Sight");
        }
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && !wasChasingPlayer && !justKilled) 
        {
            Patrolling();
            monsterAudio.chaseSoundPlayed = false;
        }
        
        if (playerInSightRange && !playerInAttackRange && !justKilled || wasChasingPlayer && !justKilled) 
        {
            ChasePlayer();
            monsterAudio.StartCoroutine("PlayChaseSound");
            agent.speed = runSpeed;
        }
        
        //if (playerInSightRange && playerInAttackRange) AttackPlayer();

        if (!playerInSightRange && !playerInAttackRange && wasChasingPlayer && !justKilled) StartCoroutine("ChasePlayerNotInSight", Random.Range(extendedChaseTime, extendedChaseTime*4));
    }


    public List<GameObject> GetAllPlayers()      //makes a list of all the players on the map
    {
        List<GameObject> players = new List<GameObject>();
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))       //finds players based on their tag
        {
             players.Add(p);        
        }

        if (players.Count <= 0) 
        {
            Debug.Log("All Players Dead.");
            this.enabled = false;
        }

        return players;
    }

    void FindClosestPlayer()                     //finds the distance of every player in the list and calculates the shortest one from the monster
    {
        Transform closestPlayer = null;
        float minDistance = Mathf.Infinity;
        Vector3 seekerPos = transform.position;

        foreach (GameObject player in GetAllPlayers())
        {
            float dist = Vector3.Distance(player.transform.position, seekerPos);
            if (dist < minDistance)
            {
                closestPlayer = player.transform;
                minDistance = dist;
            }
        }

        //Debug.Log("Target Name: " + closestPlayer.name);
        player = closestPlayer;                                   //the monster will track the closest player

    }

    private void Patrolling() 
    {
        if (hasNewTarget) return;


        agent.speed = walkSpeed;
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
           walkPointSet = false;
    }

    public void RunToSpecificTarget(Vector3 targetPosition) 
    {
        hasNewTarget = true;
        agent.speed = runSpeed;
        walkPointSet = false;
        agent.SetDestination(targetPosition);  
    }

    private void SearchWalkPoint() 
    {
        int randomWaypoint = Random.Range(0, waypoints.Length);
        walkPoint = waypoints[randomWaypoint].position;

        walkPointSet = true;
    }

    private void ChasePlayer() 
    {
        hasNewTarget = false;
        agent.SetDestination(player.position);
        neck.LookAt(player.position);
        wasChasingPlayer = true;
    }

    IEnumerator ChasePlayerNotInSight(float delay) 
    {
        while (true)                                         //while coroutine is active
        {
            yield return new WaitForSeconds(delay);         //we will return every after 5 seconds
            wasChasingPlayer = false;
           // Debug.Log("Chasing the player: " + wasChasingPlayer);
            StopCoroutine("ChasePlayerNotInSight");
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player") 
        {
            AttackPlayer(collision.collider.gameObject);
        }
    }

    public void AttackPlayer(GameObject collider)    //kill player
    {
        if (isServer)
        {
            PlayerMovementController controller = collider.GetComponent<PlayerMovementController>();

            if (collider.GetComponentInChildren<Interactor>().currentItemInHand)    //drops item in players hand if player has an item
            {
                collider.GetComponentInChildren<Interactor>().HandleItemWhenDropped(collider.GetComponentInChildren<Interactor>().currentItemInHand);
            }

            controller.isDead = true;
            controller.gameObject.tag = "DeadPlayer";
            controller.gameObject.layer = 13;   //sets player layer to DeadPlayer
            

            agent.speed = 0f;      //monster will take a break to "eat" player
            justKilled = true;

            StartCoroutine("WaitAfterKill", 5f);     //monster eats for delay amount  
        }
        else CmdAttackPlayer(collider);
        
    }

    public IEnumerator WaitAfterKill(float delay)    //after the delay, the monster will start to patrol again
    {
        while (true) 
        {
            yield return new WaitForSeconds(delay);
            agent.speed = walkSpeed;
            justKilled = false;
            wasChasingPlayer=false;
            StopCoroutine("WaitAfterKill");
        }
    }

    [Command]
    public void CmdAttackPlayer(GameObject collider) 
    {
        AttackPlayer(collider);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(walkPoint, 1f);
    }
}
