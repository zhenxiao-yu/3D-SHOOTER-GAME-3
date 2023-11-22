using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Kino;

public class PlayerScript : MonoBehaviour
{
    // Weapon Change Event
    public static System.Action<WeaponData> OnWeaponChanged = delegate { };

    [SerializeField] private WeaponData defaultWeapon;
    [SerializeField] private AnimationCurve hitCurve;
    [SerializeField] private AudioGetter damageSfx;

    private Camera cam; // Main Camera Reference
    private WeaponData currentWeapon;
    private Transform childFx;
    [SerializeField] private ParticleSystem hitFx; // Hit Effect

    private void Start()
    {
        cam = GetComponent<Camera>(); // Get Camera Reference
        //hitFx = GetComponent<DigitalGlitch>();

        // Delay the initial weapon switch
        this.DelayedAction(delegate { SwitchWeapon(); }, 0.1f);
    }

    private void Update()
    {
        // Check if the game is not paused and the player is not dead to update the current weapon
        if (currentWeapon != null && !GameManager.Instance.GamePaused && !GameManager.Instance.PlayerDead)
            currentWeapon.WeaponUpdate();
    }

    public void SwitchWeapon(WeaponData weapon = null)
    {
        // If no weapon provided, switch to the default weapon
        currentWeapon = weapon != null ? weapon : defaultWeapon;

        // Broadcast event on weapon change
        OnWeaponChanged(currentWeapon);

        // Set up the selected weapon
        currentWeapon.SetupWeapon(cam, this);
    }

    public void SetMuzzleFx(Transform fx)
    {
        // Remove any existing muzzle effect and set a new one
        if (childFx != null)
            Destroy(childFx.gameObject);

        fx.SetParent(transform);
        childFx = fx;
    }

    private IEnumerator DoCameraShake(float timer, float amp, float freq)
    {
        // Play damage sound effect
        AudioPlayer.Instance.PlaySFX(damageSfx, transform);

        Vector3 initPos = transform.position;
        Vector3 newPos = transform.position;
        float duration = timer;

        // Delay camera shake for a brief moment
        yield return new WaitForSeconds(0.2f);

        while (duration > 0f)
        {
            if ((newPos - transform.position).sqrMagnitude < 0.01f)
            {
                // Randomize the camera shake position within a defined amplitude
                newPos = initPos;
                newPos.x += Random.Range(-1f, 1f) * amp;
                newPos.y += Random.Range(-1f, 1f) * amp;
            }

            // Interpolate the camera position to simulate shaking
            transform.position = Vector3.Lerp(transform.position, newPos, freq * Time.deltaTime);

            // Apply hit effect with intensity based on the remaining duration
            hitFx.intensity = hitCurve.Evaluate(duration / timer);

            duration -= Time.deltaTime;
            yield return null;
        }

        // Clear hit effect and reset camera position
        hitFx.intensity = 0f;
        transform.position = initPos;
    }

    public void ShakeCamera(float timer, float amp, float freq)
    {
        // Start a coroutine for camera shake
        StartCoroutine(DoCameraShake(timer, amp, freq));
    }
}
