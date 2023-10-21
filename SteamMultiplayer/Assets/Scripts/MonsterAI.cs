using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Mirror;

public enum MonsterState : byte
{
    PATROL,
    WAIT,
    CHASEPLAYER,
    CHASEPLAYERPOS,
    CHASETARGET,
    ATTACKPLAYER
}

public class MonsterAI : NetworkBehaviour
{
    public NavMeshAgent agent;
    public Collider monsterCollider;
    private FieldOfView fov;
    public MonsterAudio monsterAudio;

    public Transform player;
    public Transform visiblePlayer;
    public Transform playerMonsterIsChasing;
    public GameObject playerListeningTo;
    public Transform[] waypoints;

    public List<Transform> currentPlayersMonsterCanHear = new List<Transform>();

    public LayerMask whatIsGround, whatIsPlayer;

    public float extendedChaseTime = 5f;

    public bool justKilled = false;
    public bool isWaiting = false;

    [SyncVar]
    public MonsterState monsterState;

    [Header("Monster Body")]
    public Transform neck;
    public Transform spine;

    [Header("Leg Manager")]
    public List<GroundSearchManager> legs = new List<GroundSearchManager>();
    public float legDelay = 0.1f;

    [Header("Movement")]
    public float walkSpeed = 8f;
    public float runSpeed = 15f;

    [Header("States")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public bool hasNewTarget = false;
    public bool canWait = true;

    [Header("Sound Manager")]
    public bool chaseSoundPlayed = false;
    public bool waitSoundPlayed = false;


    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool ChasingPlayer = false;


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

        monsterState = MonsterState.PATROL;
    }

    private void FixedUpdate()
    {
        if (legs[1].hasMoved && legs[0].distanceFromRaycast > legs[0].maxDistanceFromBody)
        {
            legs[1].hasMoved = false;
            legs[3].hasMoved = false;
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
        FindClosestPlayer();  //searches all players and finds the closest one

        if (monsterState == MonsterState.PATROL)    //   ** PATROL STATE **
        {
            Patrol();
        }
        else if (monsterState == MonsterState.CHASEPLAYER)   //  ** CHASE PLAYER STATE **
        {
            ChasePlayer();
        }
        else if (monsterState == MonsterState.CHASEPLAYERPOS)  //  ** CHASE PLAYER POSITION STATE **
        {
            ChasePlayerPos();
        }
        else if (monsterState == MonsterState.WAIT && !isWaiting)   //  ** WAIT STATE ** if we are not already waiting, we will wait
        {
            Wait();
        }
    }

    
    private void Patrol()  //   ** PATROL STATE **
    {
        agent.speed = walkSpeed;    //set our monster speed to walk speed
        playerMonsterIsChasing = null;  //reset player that monster was chasing
        chaseSoundPlayed = false;   //reset chase sound so it can play again
        waitSoundPlayed = false;    //reset wait sound so it can play again

        if (!walkPointSet) SearchWalkPoint();    //if we dont have a walk waypoint, we will search for one

        if (walkPointSet) agent.SetDestination(walkPoint);   //once we have a waypoint destination, we will set our monster destination to that point

        Vector3 distanceToWalkPoint = transform.position - walkPoint;    //we will keep checking how close the monster is to that point

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)         //once we have reached that point, we will disable our point and repeat this function
            walkPointSet = false;


        //Check for sight and attack range
        if (fov.visibleTargets.Count > 0)        //if monster can see a player
        {
            monsterState = MonsterState.CHASEPLAYER;    //GOTO state: chase player
        }
        //playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);  //wait to add this (ATTACK STATE)


        SearchPlayersInRadius();   //searches every player in radius and finds players sprinting

        if (playerListeningTo && canWait)    //if we can hear player, we will set state to WAIT
        {
            monsterState = MonsterState.WAIT;  //change state to WAIT
        }
        
    }

    private void SearchWalkPoint()         //search through all the waypoints and return a random waypoint position
    {
        int randomWaypoint = Random.Range(0, waypoints.Length);
        walkPoint = waypoints[randomWaypoint].position;

        walkPointSet = true;
    }

    private void Wait()
    {
        agent.speed = 0f;    //we will stop monster
        isWaiting = true;

        if (!waitSoundPlayed)
        {
            monsterAudio.HandleWaitSound();
            waitSoundPlayed = true;
        }


        StartCoroutine("CheckForPlayerSoundAfterDelay", 2f);   //if not we will wait for a delay before checking for player sounds again
    }


    private IEnumerator CheckForPlayerSoundAfterDelay(float delay) 
    {
        while (true) 
        {
            agent.speed = 0f;

            FindClosestPlayerInView();   //monster will still check if any players are in view

            if (visiblePlayer)
            {
                monsterState = MonsterState.CHASEPLAYER;  //if there is a player in view, monster will chase visible player
                isWaiting = false;   //monster will stop waiting and just chase the player in view
                yield break;       //make sure coroutine doesn't finish
            }

            yield return new WaitForSeconds(delay);
            StartCoroutine("CheckForPlayerSounds", Random.Range(3f,5f));    //after player grace period, we will check for player sounds for random range seconds
            StopCoroutine("CheckForPlayerSoundAfterDelay");  // we will stop the current coroutine
        }
    }

    private IEnumerator CheckForPlayerSounds(float delay) 
    {
        while (true) 
        {
            SearchPlayersInRadius();

            if (playerListeningTo) 
            {
                if (playerListeningTo.GetComponent<PlayerMovementController>().isSprinting)  //if we hear player after grace period
                {
                    monsterState = MonsterState.CHASEPLAYERPOS;
                    isWaiting = false;
                    yield break;
                }
            }
        
            yield return new WaitForSeconds(delay);          //if we don't see or hear anymore players, we will go back to patrolling
            isWaiting = false;
            monsterState = MonsterState.PATROL;
        }
    }

    private void ChasePlayer()    //  ** CHASE PLAYER STATE **
    {
        agent.speed = runSpeed;

        FindClosestPlayerInView();        //finds the closest player in view

        if (visiblePlayer)         //if that player exists
        {
            if (!chaseSoundPlayed) 
            {
                monsterAudio.HandleChaseSound();
                chaseSoundPlayed = true;
            }

            Debug.Log("Chasing Player");
            agent.SetDestination(visiblePlayer.position);   //then we will set the monster destination to that player
            ChasingPlayer = true;      // we set that the monster is currently chasing a player
            neck.LookAt(visiblePlayer);   //we set the neck to look at visible player
            playerMonsterIsChasing = visiblePlayer;
        }

        //ATTACK CHECK
        if (Physics.CheckSphere(transform.position, attackRange, whatIsPlayer))       // checks if player is close enough to monster
        {
            monsterState = MonsterState.ATTACKPLAYER;   // We will attack the player if in attack range
        }

        if (!visiblePlayer && ChasingPlayer)    //if we lost sight of player, but we were just chasing them
        {
            monsterState = MonsterState.CHASEPLAYERPOS;      //we will set monster state to chase player position
        }


    }


    private void ChasePlayerPos()   //  ** CHASE PLAYER POSITION STATE **
    {
        agent.speed = runSpeed;

        FindClosestPlayerInView();        //finds the closest player in view

        if (visiblePlayer) //if we can see the player again, we will switch back to chase state
        {
            monsterState = MonsterState.CHASEPLAYER;
        }
        else if (playerMonsterIsChasing)          //if player was visible, then it will call this if statement
        {
            StartCoroutine("ChasePlayerPosOnTimer", extendedChaseTime);      //if we cant we will chase the closest player that the monster was just chasing
            agent.SetDestination(playerMonsterIsChasing.position);
            canWait = false;
        }
        else                                        //if we are chasing the player pos from the wait state
        {
            StartCoroutine("ChasePlayerPosOnTimer", extendedChaseTime);      //then the monster will chase the player pos that is just heard
            agent.SetDestination(playerListeningTo.transform.position);
            canWait = false;
        }


    }

    private IEnumerator ChasePlayerPosOnTimer(float delay)
    {
        while (true)
        {
            if (visiblePlayer) yield break;  //if we see player, we will stop coroutine before the timer runs out
            else yield return new WaitForSeconds(delay);
            monsterState = MonsterState.PATROL;   //monster will now patrol
            playerMonsterIsChasing = null;       //monster is now chasing no one
            ChasingPlayer = false;
            Debug.Log("No Longer chasing player");
            StartCoroutine("WaitStateCooldown", 5f);     //after player escapes, the monster must wait a delay before it is possible to enter wait state again
            StopCoroutine("ChasePlayerPosOnTimer");
        }
    }

    private IEnumerator WaitStateCooldown(float delay)    //this is to prevent the monster to immediatly wait after player just escaped
    {
        while (true) 
        {
            yield return new WaitForSeconds(delay);
            canWait = true;
            StopCoroutine("WaitStateCooldown");
        }
    }




    /* PLAYER SEARCH FUNCTIONS */

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

    void FindClosestPlayerInView()                     //finds the distance of every player in the list and calculates the shortest one from the monster
    {
        Transform closestPlayer = null;
        float minDistance = Mathf.Infinity;
        Vector3 seekerPos = transform.position;

        foreach (Transform player in fov.visibleTargets)
        {
            float dist = Vector3.Distance(player.transform.position, seekerPos);
            if (dist < minDistance)
            {
                closestPlayer = player.transform;
                minDistance = dist;
            }
        }

        //Debug.Log("Target Name: " + closestPlayer.name);
        visiblePlayer = closestPlayer;                                   //the monster will track the closest player

    }

    void SearchPlayersInRadius() //searches every player in radius, and sets playerListeningTo variable as the last player sprinting in radius
    {
        visiblePlayer = null;   //make sure we reset visible player is null before we check again
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, fov.viewRadius, whatIsPlayer);  //checks all players in total radius
        currentPlayersMonsterCanHear.Clear();  //clear list so players dont stay in that list forever

        foreach (Collider c in targetsInViewRadius)  //searches through each player
        {
            if (c.gameObject.GetComponent<PlayerMovementController>().isSprinting)   //if a player is sprinting (monster can "hear" player)
            {
                currentPlayersMonsterCanHear.Add(c.transform);
            }
        }

        FindClosestPlayerThatMonsterCanHear();  //this will search all players that are sprinting in monster radius and finds the closest one to the monster
    }

    void FindClosestPlayerThatMonsterCanHear()                     //finds the distance of every player in the list and calculates the shortest one from the monster
    {
        Transform closestPlayer = null;
        float minDistance = Mathf.Infinity;
        Vector3 seekerPos = transform.position;

        foreach (Transform player in currentPlayersMonsterCanHear)
        {
            float dist = Vector3.Distance(player.position, seekerPos);
            if (dist < minDistance)
            {
                closestPlayer = player;
                minDistance = dist;
            }
        }
        if (currentPlayersMonsterCanHear.Count == 0) playerListeningTo = null;       //if we have no one the monster can hear, then playerListeningTo is null
        else playerListeningTo = closestPlayer.gameObject;  //the monster will track the closest player
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
