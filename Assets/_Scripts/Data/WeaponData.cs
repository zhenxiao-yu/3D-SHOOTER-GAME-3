using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Build Table For Custom Weapons
[CreateAssetMenu(fileName = "CustomWeaponData", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    // Bullet Amount Change Event
    public System.Action<int> OnWeaponFired = delegate { };

    [Header("Weapon Properties")]
    [SerializeField] private FireTypes fireType;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int damageValue;
    [SerializeField] private bool defaultWeapon;
    [SerializeField] private GameObject muzzleFX;
    [SerializeField] private AudioGetter gunShotSfx, reloadSfx, emptySfx, reloadWarningSfx;
    [SerializeField] private float fxScale = 0.1f;
    [SerializeField] private Sprite weaponIcon;

    private Camera cam;
    private ParticleSystem cachedFX;
    private PlayerScript player;
    private int currentAmmo;
    private float nextFireTime;

    public Sprite GetIcon => weaponIcon; // Get Weapon Icon

    public void SetupWeapon(Camera cam, PlayerScript player)
    {
        this.cam = cam;
        this.player = player;
        nextFireTime = 0f;
        currentAmmo = maxAmmo;
        OnWeaponFired(currentAmmo);

        if (muzzleFX != null)
        {
            GameObject temp = Instantiate(muzzleFX);
            temp.transform.localScale = Vector3.one * fxScale;
            player.SetMuzzleFx(temp.transform);
            cachedFX = temp.GetComponent<ParticleSystem>();
        }
    }

    public void WeaponUpdate()
    {
        // Check Fire Mode
        if (fireType == FireTypes.SINGLE)
        {
            if (Input.GetMouseButtonDown(0) && currentAmmo > 0) // Left click
            {
                Fire();
                currentAmmo--;
                OnWeaponFired(currentAmmo);
            }
            else if (Input.GetMouseButtonDown(0) && currentAmmo <= 0)
            {
                // Ammo runs out
                AudioPlayer.Instance.PlaySFX(emptySfx, player.transform);
                AudioPlayer.Instance.PlaySFX(reloadWarningSfx, player.transform);
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo > 0) // Left hold
            {
                Fire();
                OnWeaponFired(currentAmmo);
                currentAmmo--;
                nextFireTime = Time.time + fireRate;
            }
            else if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo <= 0)
            {
                // Ammo runs out
                AudioPlayer.Instance.PlaySFX(emptySfx, player.transform);
                AudioPlayer.Instance.PlaySFX(reloadWarningSfx, player.transform);
            }
        }

        if (defaultWeapon && Input.GetMouseButtonDown(1)) // Reload using right-click for custom weapon
        {
            currentAmmo = maxAmmo;
            AudioPlayer.Instance.PlaySFX(reloadSfx, player.transform);
            OnWeaponFired(currentAmmo);
        }

        if (!defaultWeapon && currentAmmo <= 0)
        {
            // If run out of ammo, switch back to the default weapon
            player.SwitchWeapon();
        }
    }

    private void Fire()
    {
        AudioPlayer.Instance.PlaySFX(gunShotSfx, player.transform); // Play gunshot sound
        GameManager.Instance.ShotsFired();

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); // Cast ray based on the position of the mouse

        if (cachedFX != null)
        {
            Vector3 muzzlePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.2f)); // Set Muzzle position
            cachedFX.transform.position = muzzlePos;
            cachedFX.transform.rotation = Quaternion.LookRotation(ray.direction);
            cachedFX.Play();
        }

        if (Physics.Raycast(ray, out hit, 50f)) // 50f = max distance
        {
            if (hit.collider != null)
            {
                // Make hitables an array for objects with multiple on hit components
                InterfaceHitableObj[] hitables = hit.collider.GetComponents<InterfaceHitableObj>();
                // Check validity of hitable objects and execute hit
                if (hitables != null && hitables.Length > 0)
                {
                    foreach (var hitable in hitables)
                    {
                        hitable.Hit(hit, damageValue); // Apply damage

                        if (hitable is EnemyScript || hitable is SpawnOnHit || hitable is WeaponPickup)
                        {
                            GameManager.Instance.ShotHit();
                        }
                    }
                }

                Debug.Log(hit.collider.gameObject.name); // Check collision target name
            }
        }
    }
}

// Enumeration for fire modes
public enum FireTypes
{
    SINGLE,
    RAPID
}
