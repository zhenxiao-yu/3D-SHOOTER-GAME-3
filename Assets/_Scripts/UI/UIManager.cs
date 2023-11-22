using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UIManager
{
    [Header("Health Bar")]
    [SerializeField] private Slider healthBar;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Hostage Killed Text")]
    [SerializeField] private RectTransform hostageKilledText;

    [Header("Weapon HUD")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private GameObject reloadWarning;
    [SerializeField] private RectTransform crossHair;

    [Header("Score Properties")]
    [SerializeField] private TextMeshProUGUI enemyKilledText;
    [SerializeField] private TextMeshProUGUI shotsFiredText;
    [SerializeField] private TextMeshProUGUI shotsHitText;
    [SerializeField] private TextMeshProUGUI accuracyText;

    [Header("End Screen")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Button backButton;
    [SerializeField] private string titleSceneName;

    private WeaponData currentWeapon;

    public void Init(float maxHealth)
    {
        if (crossHair != null)
            Cursor.visible = false;

        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        hostageKilledText.gameObject.SetActive(false);
        PlayerScript.OnWeaponChanged += UpdateWeapon;
        TimerObject.OnTimerChanged += UpdateTimer;

        backButton.onClick.AddListener(GoToTitleScene);
    }

    public void RemoveEvent()
    {
        PlayerScript.OnWeaponChanged -= UpdateWeapon;
        TimerObject.OnTimerChanged -= UpdateTimer;
        currentWeapon.OnWeaponFired -= UpdateAmmo;
    }

    public void UpdateHealth(float value)
    {
        healthBar.value = value;
    }

    public void UpdateTimer(int currentTimer)
    {
        timerText.SetText(currentTimer.ToString("00"));
    }

    public void UpdateWeapon(WeaponData weaponData)
    {
        if (currentWeapon != null)
            currentWeapon.OnWeaponFired -= UpdateAmmo;

        currentWeapon = weaponData;
        currentWeapon.OnWeaponFired += UpdateAmmo; // Update Bullet Count  
        weaponIcon.sprite = currentWeapon.GetIcon; // Update Weapon Icon
    }

    public void UpdateAmmo(int ammo)
    {
        // Show reload warning when ammo is 0
        reloadWarning.SetActive(ammo <= 0);
        // Format bullet count
        ammoText.SetText(ammo.ToString("00"));
    }

    public void ShowHostageKilled(Vector3 position, bool show)
    {
        hostageKilledText.gameObject.SetActive(show);

        if (!show)
            return;

        hostageKilledText.position = position;
        Vector2 adjustedPosition = Extensions.GetPositionInsideScreen(new Vector2(1920f, 1080f), hostageKilledText, 25f);
        // Apply to anchored position
        hostageKilledText.anchoredPosition = adjustedPosition;
    }

    public void ShowEndScreen(int enemyKilled, int totalEnemy, int hostageKilled, int shotsFired, int shotsHit)
    {
        // End screen stats calculation
        endScreenPanel.SetActive(true);
        float enemyKilledPercentage = ((enemyKilled / (float)totalEnemy) * 100f);
        enemyKilledText.SetText(enemyKilledPercentage.ToString("00") + "%");
        hostageKilledText.SetText(hostageKilled.ToString());
        shotsFiredText.SetText(shotsFired.ToString());
        shotsHitText.SetText(shotsHit.ToString());
        float accuracy = (shotsHit / (shotsFired == 0 ? 1f : (float)shotsFired)) * 100f;
        accuracyText.SetText(accuracy.ToString("00") + "%");

        CalculateScore(enemyKilledPercentage, accuracy);
    }

    private void CalculateScore(float enemyKilledPercentage, float accuracy)
    {
        /* Rank System
        * Hostage Killed Decreases the score by 15
        A - Enemy Killed > 90% && Accuracy > 80% = Total Average 170/2 = 85
        B - Enemy Killed > 75% < 90% && Accuracy > 70% < 80% = Total Average > 72% < 85%
        C - Enemy Killed > 60% < 75% && Accuracy > 55% < 70% = Total Average > 57% < 72%
        D - Enemy Killed < 60% && Accuracy < 55% Total Average < 57%
        */

        float hostagePenalty = hostageKilled * 15f;
        float enemyKillRatio = enemyKilledPercentage - hostagePenalty;
        float accuracyRatio = accuracy - hostagePenalty;
        float totalAverage = (enemyKillRatio + accuracyRatio) / 2f;

        if (totalAverage >= 85f)
        {
            rankText.SetText("A");
        }
        else if (totalAverage >= 72f && totalAverage < 85f)
        {
            rankText.SetText("B");
        }
        else if (totalAverage >= 57f && totalAverage < 72f)
        {
            rankText.SetText("C");
        }
        else if (totalAverage == 0f)
        {
            rankText.SetText("F");
        }
        else if (totalAverage < 57f)
        {
            rankText.SetText("D");
        }
    }

    public void MoveCrosshair(Vector3 mousePosition)
    {
        if (crossHair != null)
            crossHair.position = mousePosition;
    }

    private void GoToTitleScene()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}

