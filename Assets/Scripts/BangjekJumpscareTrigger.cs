using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BangjekJumpscareTrigger : MonoBehaviour
{
    // This is the jumpscare animation
    public Animation JumpscareAnimation;

    // This is the jumpscare sound
    public AudioSource JumpscareAudio;

    // This play when the player touch the trigger
    public void OnMouseOver(){
        // Play the jumpscare animation
        JumpscareAnimation.Play();

        // Play the jumpscare sound
        if (JumpscareAudio != null){
            JumpscareAudio.Play();
        }

        // Disable the trigger
        this.gameObject.SetActive(false);
    }   
}
