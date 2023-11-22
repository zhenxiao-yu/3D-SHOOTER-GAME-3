using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicAction : MonoBehaviour
{
    [SerializeField] AudioGetter audioSfx; // The music audio clip to play
    [SerializeField] float delay; // Delay before playing the music

    // This method is called when the GameObject is enabled
    private void OnEnable()
    {
        // Schedule the music to be played after a delay
        this.DelayedAction(delegate
        {
            // Play the specified music audio clip
            AudioPlayer.Instance.PlayMusic(audioSfx);
        }, delay);
    }
}
