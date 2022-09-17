using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
public class MonsterAudio : MonoBehaviour
{
    public MonsterAI ai;
    public NavMeshAgent agent;

    public AudioClip[] footsteps;
    private AudioSource footstep;

    public float footStepDelay;
    public float footStepMaxDistance = 1f;

    private void Awake()
    {
        footstep = GetComponent<AudioSource>();
        ai = GetComponent<MonsterAI>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        footstep.maxDistance = footStepMaxDistance;
    }


    public void PlayFootsteps() 
    {      
        footstep.clip = footsteps[Random.Range(0, footsteps.Length)];
        footstep.Play();
    }
}
