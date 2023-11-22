using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] float damageRadius; // Effective range of the explosion
    [SerializeField] float delayUntilDestroy; // Delay before destroying the explosion object

    // Start is called before the first frame update
    void Start()
    {
        // Schedule the destruction of this explosion object after a delay
        Destroy(gameObject, delayUntilDestroy);

        // Apply damage to nearby objects within the specified radius
        DamageNearbyObjects();
    }

    // Apply damage to nearby objects
    void DamageNearbyObjects()
    {
        // Find colliders within the damage radius of the explosion
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (var collider in colliders)
        {
            // Check if the object has IHitable components
            InterfaceHitableObj[] hitables = collider.GetComponents<InterfaceHitableObj>();

            RaycastHit hit;

            // Perform a raycast to determine the hit point
            if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hit))
            {
                // Check if there are any IHitable components and apply damage if valid
                if (hitables != null && hitables.Length > 0)
                {
                    foreach (var hitable in hitables)
                    {
                        // Apply damage to the hitable objects (e.g., enemies)
                        hitable.Hit(hit, 50); // Deal 50 damage (adjust as needed)
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the explosion radius in the editor using Gizmos
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
