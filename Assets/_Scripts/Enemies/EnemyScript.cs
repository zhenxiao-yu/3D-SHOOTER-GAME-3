using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour, InterfaceHitableObj
{
    [Header("Enemy Health")]
    [SerializeField] private int maxHealth = 100;

    [Header("Target Position")]
    [SerializeField] private Transform targetPos;

    [Header("Shooting Properties")]
    [SerializeField] private IntervalRange interval = new IntervalRange(1.5f, 2.7f);
    [SerializeField] private float shootAccuracy = 0.5f;
    [SerializeField] private ParticleSystem shotFx;

    private int currentHealth;
    private Transform player;
    private bool isDead;
    private NavMeshAgent agent;
    private ShootOutPoint playerShootingSpot;
    private Animator anim;
    private Vector3 movementLocal;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = Camera.main.transform;
        anim = GetComponentInChildren<Animator>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void Start()
    {
        if (agent != null)
        {
            InitEnemy();
        }
    }

    private void InitEnemy()
    {
        currentHealth = maxHealth;
        playerShootingSpot = GetComponent<ShootOutPoint>();
        agent.SetDestination(targetPos.position);
        StartCoroutine(Shoot());
        GameManager.Instance.RegisterEnemy();
    }

    private void Update()
    {
        if (player != null && !isDead)
        {
            LookAtPlayer();
        }
        UpdateAnimationBlend();
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void UpdateAnimationBlend()
    {
        if (anim == null || !anim.enabled || !agent.enabled)
        {
            return;
        }

        if (agent.remainingDistance > 0.01f)
        {
            UpdateMovementLocal();
        }
        else
        {
            movementLocal = Vector3.Lerp(movementLocal, Vector3.zero, 1.3f * Time.deltaTime);
        }

        anim.SetFloat("X Speed", movementLocal.x);
        anim.SetFloat("Z Speed", movementLocal.z);
    }

    private void UpdateMovementLocal()
    {
        movementLocal = Vector3.Lerp(movementLocal, transform.InverseTransformDirection(agent.velocity).normalized, 2f * Time.deltaTime);
        agent.nextPosition = transform.position;
    }

    public void Hit(RaycastHit hit, int damage = 1)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Enemy Shot!");

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
        else
        {
            anim.SetTrigger("Shot");
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        agent.enabled = false;
        playerShootingSpot.EnemyKilled();
        StopShooting();
        anim.SetTrigger("Dead");
        anim.SetBool("Is Dead", true);
        Destroy(gameObject, 3f);
        GameManager.Instance.EnemyKilled();
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => agent.remainingDistance < 0.02f);

        while (!isDead)
        {
            if (GameManager.Instance.PlayerDead)
            {
                StopShooting();
            }

            AdjustShotEffectDirection();
            if (Random.Range(0f, 1f) < shootAccuracy)
            {
                FireShot();
            }

            shotFx.Play();
            yield return new WaitForSeconds(interval.GetValue);
        }
    }

    private void AdjustShotEffectDirection()
    {
        shotFx.transform.rotation = Quaternion.LookRotation(transform.forward + Random.insideUnitSphere * 0.1f);
    }

    private void FireShot()
    {
        shotFx.transform.rotation = Quaternion.LookRotation(player.position - shotFx.transform.position);
        GameManager.Instance.PlayerHit(1f);
        Debug.Log("Player Hit");
    }

    public void StopShooting()
    {
        StopAllCoroutines();
    }
}

[System.Serializable]
public struct IntervalRange
{
    [SerializeField] private float min;
    [SerializeField] private float max;

    public IntervalRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float GetValue => Random.Range(min, max);
}
