using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerAudioHelper : MonoBehaviour
{
    public AudioSource footstepSound;
    public List<AudioClip> footstepClips;


    public void PlaySound()
    {
        if (gameObject.GetComponentInParent<PlayerMovementController>().isGrounded)    //wont play footsteps if player is in the air
        {
            footstepSound.clip = footstepClips[Random.Range(0, footstepClips.Count)];
            footstepSound.Play();
        }  
        //gameObject.GetComponentInParent<Interactor>().PlayFootstep();     //call interactor to see if this player has control  
    }

    public void AuthorityPlaySound()    //if it does, we get sent back to play the sound.
    {
        footstepSound.clip = footstepClips[Random.Range(0, footstepClips.Count)];
        footstepSound.Play();
    }
}
