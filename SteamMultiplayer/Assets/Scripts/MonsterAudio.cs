using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Mirror;
public class MonsterAudio : NetworkBehaviour
{
    public MonsterAI ai;
    public NavMeshAgent agent;

    public AudioClip[] footsteps;
    private AudioSource footstep;
    public AudioSource mapMonsterGroanSound;
    public AudioClip[] monsterGroans;
    public AudioSource monsterChaseSound;

    public float footStepDelay;
    public float footStepMaxDistance = 1f;

    public bool chaseSoundPlayed = false;

    private void Awake()
    {
        footstep = GetComponent<AudioSource>();
        ai = GetComponent<MonsterAI>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        footstep.maxDistance = footStepMaxDistance;
        StartCoroutine("PlayMonsterGroan", Random.Range(10, 30));
    }

    public IEnumerator PlayChaseSound() 
    {
        while (true) 
        {
            yield return null;
            if (!chaseSoundPlayed) HandleChaseSound();   //if we start the chase, then we call handlechasesound
            chaseSoundPlayed = true;
            StopCoroutine("PlayChaseSound");
        }
    }

    public void HandleChaseSound() //handles the chase sound.
    {
        if (isServer)              //if we are server, then we will call rpc function
        {
            RpcPlayChaseSound();
        }
        else CmdPlayChaseSound();     //if we are client, we will call command
    }

    [ClientRpc]
    public void RpcPlayChaseSound()   //rpc is called by server, and will play sound on all clients
    {
        monsterChaseSound.Play();
    }

    [Command]
    public void CmdPlayChaseSound()    //command is called by client, and calls handle chase sound function again, but as server
    {
        HandleChaseSound();
    }

    public void PlayFootsteps() 
    {      
        footstep.clip = footsteps[Random.Range(0, footsteps.Length)];
        footstep.Play();
    }

    public IEnumerator PlayMonsterGroan(float delay) 
    {
        mapMonsterGroanSound.clip = monsterGroans[Random.Range(0, monsterGroans.Length)];
        while (true) 
        {
            yield return new WaitForSeconds(delay);
            mapMonsterGroanSound.Play();
            StopCoroutine("PlayMonsterGroan");
            StartCoroutine("PlayMonsterGroan", Random.Range(10, 30));

        }
    }
}
