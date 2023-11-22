using UnityEngine;

public class SpawnOnHit : MonoBehaviour, InterfaceHitableObj
{
    [SerializeField] GameObject prefabsToSpawn; // The prefab to spawn on hit
    [SerializeField] float yOffset = 0.5f; // Offset for spawning the prefab vertically
    [SerializeField] bool destroyOnHit; // Whether to destroy the game object on hit

    // Called when the game object is hit
    public void Hit(RaycastHit hit, int damage = 1)
    {
        if (prefabsToSpawn != null)
        {
            // Spawn the prefab at the hit point with the specified vertical offset
            Vector3 spawnPosition = hit.point + Vector3.up * yOffset;
            Instantiate(prefabsToSpawn, spawnPosition, Quaternion.identity);
        }

        if (destroyOnHit)
        {
            // Destroy the game object if destroyOnHit is true
            Destroy(gameObject);
        }
    }
}