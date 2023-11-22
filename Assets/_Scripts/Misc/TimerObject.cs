using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimerObject
{
    // Event to notify when the timer changes
    public static System.Action<int> OnTimerChanged = delegate { };

    public int displayTimer; // The current displayed timer value
    private Coroutine timer; // Coroutine reference for the timer

    // Start a timer with the specified duration
    public void StartTimer(MonoBehaviour mb, float duration)
    {
        // If a timer is already running, do nothing
        if (timer != null)
        {
            return;
        }

        // Start a new coroutine to run the timer
        timer = mb.StartCoroutine(TimerRuns(duration));
    }

    // Stop the currently running timer
    public void StopTimer(MonoBehaviour mb)
    {
        // If there is no timer running, do nothing
        if (timer == null)
        {
            return;
        }

        // Stop the coroutine and reset the timer reference
        mb.StopCoroutine(timer);
        timer = null;
    }

    // Coroutine that runs the timer
    private IEnumerator TimerRuns(float duration)
    {
        while (duration > 0f)
        {
            // If the player is dead, exit the coroutine prematurely
            if (GameManager.Instance.PlayerDead)
                yield break;

            // Notify subscribers that the timer has changed
            OnTimerChanged((int)duration);

            // Update the displayed timer value
            displayTimer = (int)duration;

            // Decrease the timer by 1 second
            duration -= 1f;

            // Wait for 1 second before the next iteration
            yield return new WaitForSeconds(1f);
        }

        // Reset the timer reference when the timer has finished
        timer = null;
    }
}
