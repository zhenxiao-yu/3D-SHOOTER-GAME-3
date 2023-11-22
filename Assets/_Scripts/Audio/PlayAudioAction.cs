using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAction : MonoBehaviour
{
    [SerializeField] AudioGetter audioSfx; // The audio clip to play
    [SerializeField] bool twoDSound; // Should the sound be played in 2D (ignoring position)?
    [SerializeField] float delay; // Delay before playing the audio

    // This method is called when the GameObject is enabled
    private void OnEnable()
    {
        // Schedule the audio to be played after a delay
        this.DelayedAction(delegate
        {
            // Play the specified audio clip
            // If twoDSound is true, play it as a 2D sound (ignoring position)
            // If twoDSound is false, play it with the GameObject's position as the audio source position
            AudioPlayer.Instance.PlaySFX(audioSfx, twoDSound ? null : transform);
        }, delay);
    }
}
