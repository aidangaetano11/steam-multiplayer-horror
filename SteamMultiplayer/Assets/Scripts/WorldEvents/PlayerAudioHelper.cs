using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerAudioHelper : MonoBehaviour
{
    public AudioSource footstepSound;
    public List<AudioClip> footstepClips;


    public void PlaySound() 
    {
        footstepSound.clip = footstepClips[Random.Range(0, footstepClips.Count)];
        footstepSound.Play();
    }
}
