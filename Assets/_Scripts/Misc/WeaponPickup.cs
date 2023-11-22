using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, InterfaceHitableObj
{
    [SerializeField] WeaponData weapon; // The weapon type associated with this pickup
    [SerializeField] float rotateSpeed = 90f; // How fast the pickup rotates
    [SerializeField] AudioGetter pickupSfx; // Sound effect played when picked up

    private PlayerScript player; // Reference to the player

    void Start()
    {
        // Find the player in the scene
        player = FindObjectOfType<PlayerScript>();
    }

    void Update()
    {
        // Rotate the pickup object
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    public void Hit(RaycastHit hit, int damage = 1)
    {
        // Switch to the custom weapon when the pickup is hit
        player.SwitchWeapon(weapon);

        // Play the pickup sound effect
        AudioPlayer.Instance.PlaySFX(pickupSfx);

        // Destroy the pickup object
        Destroy(gameObject);
    }
}
