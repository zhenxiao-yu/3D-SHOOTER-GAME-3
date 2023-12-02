using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : EnemyScript
{
    [SerializeField] float damageRadius;
    [SerializeField] float delayUntilDestroy;
    [SerializeField] float explosionDistance;
    protected override void BehaviourSetup()
    {
        agent.SetDestination(targetPos.position);
        GameManager.Instance.RegisterEnemy();
    }
    // // I'm guessing we'd need to make the Enemy script to have a virtual Update function for this
    // public override void Update()
    // {

    //     if (player != null && !isDead)
    //     {
    //         Vector3 direction = player.position - transform.position;
    //         direction.y = 0f;

    //         transform.rotation = Quaternion.LookRotation(direction);
    //     }
    //     if (agent.remainingDistance < explosionDistance)
    //     {
    //         // copying code from ExplosionDamage
    //         // Schedule the destruction of this explosion object after a delay
    //         Destroy(gameObject, delayUntilDestroy);

    //         // Apply damage to nearby objects within the specified radius
    //         DamageNearbyObjects();


    //     }

    //     RunBlend();
    // }
    // // Apply damage to nearby objects
    // void DamageNearbyObjects()
    // {
    //     // Find colliders within the damage radius of the explosion
    //     Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

    //     foreach (var collider in colliders)
    //     {
    //         // Check if the object has IHitable components
    //         InterfaceHitableObj[] hitables = collider.GetComponents<InterfaceHitableObj>();

    //         RaycastHit hit;

    //         // Perform a raycast to determine the hit point
    //         if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hit))
    //         {
    //             // Check if there are any IHitable components and apply damage if valid
    //             if (hitables != null && hitables.Length > 0)
    //             {
    //                 foreach (var hitable in hitables)
    //                 {
    //                     // Apply damage to the hitable objects (e.g., enemies)
    //                     hitable.Hit(hit, 50); // Deal 50 damage (adjust as needed)
    //                 }
    //             }
    //         }
    //     }
    // }
}