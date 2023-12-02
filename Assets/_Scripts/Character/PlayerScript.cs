using System.Collections;
using UnityEngine;
using Kino;

public class PlayerScript : MonoBehaviour
{
    public static System.Action<WeaponData> OnWeaponChanged = delegate { };

    [SerializeField] private WeaponData defaultWeapon;
    [SerializeField] private AnimationCurve glitchCurve;
    [SerializeField] private AudioGetter damageSfx;

    private Camera cam;
    private WeaponData currentWeapon;
    private Transform childFx;
    private DigitalGlitch glitchFx;

    private void Start()
    {
        InitializeReferences();
        this.DelayedAction(SwitchToDefaultWeapon, 0.1f);
    }

    private void Update()
    {
        if (CanUpdateWeapon())
        {
            currentWeapon.WeaponUpdate();
        }
    }

    private void InitializeReferences()
    {
        cam = GetComponent<Camera>();
        glitchFx = GetComponent<DigitalGlitch>();
    }

    private bool CanUpdateWeapon()
    {
        return currentWeapon != null && !GameManager.Instance.GamePaused && !GameManager.Instance.PlayerDead;
    }

    private void SwitchToDefaultWeapon()
    {
        SwitchWeapon(defaultWeapon);
    }

    public void SwitchWeapon(WeaponData weapon = null)
    {
        currentWeapon = weapon != null ? weapon : defaultWeapon;
        OnWeaponChanged(currentWeapon);
        currentWeapon.SetupWeapon(cam, this);
    }

    public void SetMuzzleFx(Transform fx)
    {
        if (childFx != null)
        {
            Destroy(childFx.gameObject);
        }

        fx.SetParent(transform);
        childFx = fx;
    }

    private IEnumerator DoCameraShake(float timer, float amp, float freq)
    {
        AudioPlayer.Instance.PlaySFX(damageSfx, transform);
        Vector3 initPos = transform.position;
        Vector3 newPos = transform.position;
        float duration = timer;

        yield return new WaitForSeconds(0.2f);

        while (duration > 0f)
        {
            if ((newPos - transform.position).sqrMagnitude < 0.01f)
            {
                newPos = initPos;
                newPos.x += Random.Range(-1f, 1f) * amp;
                newPos.y += Random.Range(-1f, 1f) * amp;
            }

            transform.position = Vector3.Lerp(transform.position, newPos, freq * Time.deltaTime);

            glitchFx.intensity = glitchCurve.Evaluate(duration / timer);

            duration -= Time.deltaTime;
            yield return null;
        }

        ClearGlitchEffect();
        ResetTransformPosition(initPos);
    }

    public void ShakeCamera(float timer, float amp, float freq)
    {
        StartCoroutine(DoCameraShake(timer, amp, freq));
    }

    private void ClearGlitchEffect()
    {
        glitchFx.intensity = 0f;
    }

    private void ResetTransformPosition(Vector3 position)
    {
        transform.position = position;
    }
}
