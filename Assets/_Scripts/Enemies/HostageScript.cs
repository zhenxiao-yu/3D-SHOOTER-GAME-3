using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageScript : EnemyScript
{
    protected override void BehaviourSetup()
    {
        // Set the destination of the agent to the target position (e.g., where the hostage should go)
        agent.SetDestination(targetPos.position);
    }

    protected override void DeadBehaviour()
    {
        // Handle behavior when the hostage is killed

        // Notify the GameManager that the player hit the hostage
        GameManager.Instance.PlayerHit(1);

        // Notify the GameManager that the hostage was killed and provide a position for effects
        GameManager.Instance.HostageKilled(transform.position + Vector3.up * 0.4f);
    }
}
