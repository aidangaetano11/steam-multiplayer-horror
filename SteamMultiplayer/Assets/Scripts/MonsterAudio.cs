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
            if (!chaseSoundPlayed) monsterChaseSound.Play();   //if we start the chase, then we play sound once
            chaseSoundPlayed = true;
            StopCoroutine("PlayChaseSound");
        }
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
