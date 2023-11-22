using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOutPoint : MonoBehaviour
{
    [SerializeField] EnemyEntry[] enemyList;
    public bool AreaCleared { get; private set; }
    private bool activePoint;
    private PlayerMove playerMovement;
    private int enemyKilled, totalEnemy;

    public void Initialize(PlayerMove value)
    {
        playerMovement = value;
    }

    private void Start()
    {
        foreach (var enemyEntry in enemyList)
        {
            enemyEntry.enemy.gameObject.SetActive(false);
            totalEnemy += !(enemyEntry.enemy is HostageScript) ? 1 : 0;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.SetPlayerMovement(false);
        }

        if (Input.GetKeyDown(KeyCode.Return) && activePoint)
        {
            playerMovement.SetPlayerMovement(true);
            AreaCleared = true;
            activePoint = false;
        }
    }

    public void StartShootOut(float timer)
    {
        activePoint = true;
        playerMovement.SetPlayerMovement(false);
        StartCoroutine(SendEnemies(timer));
        GameManager.Instance.StartTimer(timer);
        this.DelayedAction(SetAreaCleared, timer);
    }

    private IEnumerator SendEnemies(float timer)
    {
        foreach (var enemyEntry in enemyList)
        {
            yield return new WaitForSeconds(enemyEntry.delay);
            enemyEntry.enemy.gameObject.SetActive(true);
            enemyEntry.enemy.Init(this);
            Debug.Log(enemyEntry.enemy.gameObject.name + " Spawned");
        }
    }

    public void EnemyKilled()
    {
        enemyKilled++;

        if (enemyKilled == totalEnemy)
        {
            Debug.Log(gameObject.name + " cleared!");
            playerMovement.AreaCleared();
            AreaCleared = true;
            activePoint = false;
            GameManager.Instance.StopTimer();
        }
    }

    public void SetAreaCleared()
    {
        if (AreaCleared || GameManager.Instance.PlayerDead)
        {
            return;
        }

        AreaCleared = true;
        playerMovement.AreaCleared();

        foreach (var enemyEntry in enemyList)
        {
            enemyEntry.enemy.StopShooting();
        }
    }
}

[System.Serializable]
public class EnemyEntry
{
    public EnemyScript enemy;
    public float delay;
}
