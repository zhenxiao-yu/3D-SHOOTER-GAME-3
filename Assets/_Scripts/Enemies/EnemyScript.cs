using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour, InterfaceHitableObj
{
    [SerializeField] int maxHealth; // Enemy Health
    [SerializeField] protected Transform targetPos; // Target Position

    [Header("Shooting Properties")]
    [SerializeField] IntervalRange interval = new IntervalRange(1.5f, 2.7f); // Default Interval
    [SerializeField] float shootAccuracy = 0.5f; // Enemy Shoot Accuracy
    [SerializeField] ParticleSystem shotFx; // Shot Effect Ref

    private int currentHealth;
    private Transform player;
    private bool isDead;
    protected NavMeshAgent agent;
    private ShootOutPoint shootOutPoint;
    private Animator anim; // Enemy Animator Ref
    private Vector3 movementLocal;

    void Awake()
    {
        // Get Navigation Mesh Component
        agent = GetComponent<NavMeshAgent>();
        // Get Player Reference
        player = Camera.main.transform;
        // Get Animator Component
        anim = GetComponentInChildren<Animator>();
        // Don't change enemy position and rotation until called
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    public void Init(ShootOutPoint point)
    {
        currentHealth = maxHealth;
        shootOutPoint = point;

        // Give Enemy a Destination
        if (agent != null)
        {
            BehaviourSetup();
        }
    }

    protected virtual void BehaviourSetup()
    {
        agent.SetDestination(targetPos.position);
        StartCoroutine(Shoot());
        GameManager.Instance.RegisterEnemy();
    }

    void Update()
    {
        // The Enemy Always Facing The Player
        if (player != null && !isDead)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            transform.rotation = Quaternion.LookRotation(direction);
        }

        RunBlend();
    }

    void RunBlend()
    {
        if (anim == null || !anim.enabled || !agent.enabled)
        {
            return;
        }

        if (agent.remainingDistance > 0.01f)
        {
            movementLocal = Vector3.Lerp(movementLocal, transform.InverseTransformDirection(agent.velocity).normalized, 2f * Time.deltaTime);

            // Make NavMesh follow Animator so enemy doesn't go through walls
            agent.nextPosition = transform.position;
        }
        else
        {
            // Fix enemy animation abrupt stop problem
            movementLocal = Vector3.Lerp(movementLocal, Vector3.zero, 1.3f * Time.deltaTime);
        }

        anim.SetFloat("X Speed", movementLocal.x);
        anim.SetFloat("Z Speed", movementLocal.z);
    }

    public void Hit(RaycastHit hit, int damage = 1)
    {
        if (isDead)
            return;

        currentHealth -= damage; // Decrease health by damage value
        Debug.Log("Enemy Shot!");

        // After health reaches 0
        if (currentHealth <= 0)
        {
            isDead = true;
            agent.enabled = false;
            DeadBehaviour();
            // Change state of animator
            anim.SetTrigger("Dead");
            anim.SetBool("Is Dead", true);
            Destroy(gameObject, 3f); // Give time to play death animation
        }
        else
        {
            anim.SetTrigger("Shot");
        }
    }

    protected virtual void DeadBehaviour()
    {
        shootOutPoint.EnemyKilled();
        StopShooting(); // End coroutine when the enemy is killed
        GameManager.Instance.EnemyKilled();
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => { return agent.remainingDistance < 0.02f; });
        // Shoot player while not dead
        while (!isDead)
        {
            if (GameManager.Instance.PlayerDead)
                StopShooting();

            // Adjust shot effect direction based on Hit Condition 
            shotFx.transform.rotation = Quaternion.LookRotation(transform.forward + Random.insideUnitSphere * 0.1f);

            if (Random.Range(0f, 1f) < shootAccuracy)
            {
                shotFx.transform.rotation = Quaternion.LookRotation(player.position - shotFx.transform.position);

                GameManager.Instance.PlayerHit(1f);
                Debug.Log("Player Hit");
            }

            shotFx.Play();

            yield return new WaitForSeconds(interval.GetValue);
        }
    }

    public void StopShooting()
    {
        StopAllCoroutines();
    }
}

[System.Serializable]
public struct IntervalRange
{
    [SerializeField] float min, max;
    public IntervalRange(float min, float max) // Constructor Method
    {
        this.min = min;
        this.max = max;
    }

    public float GetValue { get => Random.Range(min, max); }
}
