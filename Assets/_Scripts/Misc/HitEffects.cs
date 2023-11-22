using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour, InterfaceHitableObj
{
    [SerializeField] GameObject effectsPrefab; // The prefab for hit effects
    [SerializeField] AudioGetter hitSfx; // The audio clip to play when hit

    private ParticleSystem effectsCache; // A cache for the hit effects particle system

    public void Hit(RaycastHit hit, int damage = 1)
    {
        if (effectsCache != null)
        {
            // Position the hit effects at the hit point and align with the surface normal
            effectsCache.transform.position = hit.point;
            effectsCache.transform.rotation = Quaternion.LookRotation(hit.normal);

            // Play the hit sound effect with the effects as the audio source
            AudioPlayer.Instance.PlaySFX(hitSfx, effectsCache.transform);

            // Play the hit effects
            effectsCache.Play();
        }
    }

    void Start()
    {
        if (effectsPrefab != null)
        {
            // Instantiate the hit effects prefab and get the associated particle system
            GameObject effectsTemp = Instantiate(effectsPrefab, transform);
            effectsCache = effectsTemp.GetComponent<ParticleSystem>();
        }
    }
}
