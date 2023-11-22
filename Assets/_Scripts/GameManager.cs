using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Serialized fields
    [SerializeField] private GameState state;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private int playerHealth = 10; // Default health value
    [SerializeField] private UIManager uiManager = null;
    [SerializeField] private VolumeSettings volSetting = null;

    // Game Stats
    private float currentHealth;
    private int enemyHit, shotsFired, enemyKilled, totalEnemy, hostageKilled;

    private TimerObject timerObject = new TimerObject();

    // Public properties
    public bool GamePaused { get; private set; }
    public bool PlayerDead { get; private set; }

    private void Awake()
    {
        // Initialize current instance
        Instance = this;
    }

    private void Start()
    {
        SwitchState(GameState.Start);
        Init();
    }

    private void Init()
    {
        currentHealth = playerHealth;
        uiManager.Init(currentHealth); // Initialize health value
        // Show EndScreen when the level is finished (subscribe)
        volSetting.Init();
        PlayerMove.OnLevelFinished += ShowEndScreen;
    }

    private void OnDisable()
    {
        // Unbind events
        uiManager.RemoveEvent();
        // Unsubscribe
        PlayerMove.OnLevelFinished -= ShowEndScreen;
    }

    #region Game State Management

    public void SwitchState(GameState newState)
    {
        if (state == newState)
            return;

        state = newState;
        switch (state)
        {
            case GameState.Start:
                HandleStartState();
                break;
            case GameState.Gameplay:
                HandleGameplayState();
                break;
            case GameState.LevelEnd:
                // Handle LevelEnd state
                break;
        }
    }

    private void HandleStartState()
    {
        Debug.Log("Game Start");
        playerMove.enabled = false;
        this.DelayedAction(() => SwitchState(GameState.Gameplay), 3f); // Switch to Gameplay State After 3 seconds
    }

    private void HandleGameplayState()
    {
        Debug.Log("State: Gameplay " + Time.time);
        playerMove.enabled = true;
    }

    #endregion

    #region Gameplay Events

    public void ShotHit()
    {
        // Used to calculate the accuracy of shots
        enemyHit++;
    }

    public void ShotsFired()
    {
        shotsFired++;
    }

    public void PlayerHit(float damage)
    {
        // Take damage when the player is hit
        currentHealth -= damage;
        uiManager.UpdateHealth(currentHealth); // Change UI
        playerScript.ShakeCamera(0.5f, 0.2f, 5f);

        if (currentHealth <= 0f)
        {
            ShowEndScreen();
            PlayerDead = true;
        }
    }

    #endregion

    #region Timer Management

    public void StartTimer(float duration)
    {
        timerObject.StartTimer(this, duration);
    }

    public void StopTimer()
    {
        timerObject.StopTimer(this);
    }

    #endregion

    #region Enemy and Hostage Management

    public void RegisterEnemy()
    {
        totalEnemy++;
    }

    public void HostageKilled(Vector3 worldPos)
    {
        hostageKilled++;
        ShowHostageKilled(worldPos, true);
        this.DelayedAction(() => ShowHostageKilled(worldPos, false), 3f);
    }

    public void EnemyKilled()
    {
        enemyKilled++;
    }

    #endregion

    #region UI Management

    private void ShowHostageKilled(Vector3 pos, bool show)
    {
        Vector3 screenPos = playerMove.GetComponent<Camera>().WorldToScreenPoint(pos);
        uiManager.ShowHostageKilled(screenPos, show);
    }

    private void ShowEndScreen()
    {
        this.DelayedAction(() => uiManager.ShowEndScreen(enemyKilled, totalEnemy, hostageKilled, shotsFired, enemyHit), 0.2f);
    }

    private void Update()
    {
        HandlePauseInput();
        uiManager.MoveCrosshair(Input.mousePosition);
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 0f;

        if (volSetting.Panel != null)
        {
            volSetting.Panel.SetActive(Mathf.Approximately(Time.timeScale, 0f));
        }

        GamePaused = Mathf.Approximately(Time.timeScale, 0f);
    }

    #endregion
}

public enum GameState
{
    Default,
    Start,
    Gameplay,
    LevelEnd
}
