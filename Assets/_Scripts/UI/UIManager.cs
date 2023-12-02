using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UIManager
{
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] RectTransform hostageKilledText;

    [Header("Weapon HUD")]
    [SerializeField] Image weaponIcon;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] GameObject reloadWarning;
    [SerializeField] RectTransform crossHair;

    [Header("Score Properties")]
    [SerializeField] TextMeshProUGUI enemyKilled;
    [SerializeField] TextMeshProUGUI hostageKilled;
    [SerializeField] TextMeshProUGUI shots;
    [SerializeField] TextMeshProUGUI hit;
    [SerializeField] TextMeshProUGUI accuracy;
    [SerializeField] GameObject endScreenPanel;
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] Button backButton;
    [SerializeField] string titleSceneName;



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

    private void UpdateTimer(int currentTimer)
    {
        timerText.SetText(currentTimer.ToString("00"));
    }

    public void RemoveEvent()
    {
        PlayerScript.OnWeaponChanged -= UpdateWeapon;
        TimerObject.OnTimerChanged -= UpdateTimer;
        currentWeapon.OnWeaponFired -= UpdateAmmo;
    }

    public void UpdateWeapon(WeaponData obj)
    {
        if (currentWeapon != null)
            currentWeapon.OnWeaponFired -= UpdateAmmo;

        currentWeapon = obj;
        currentWeapon.OnWeaponFired += UpdateAmmo; //Change Bullet Count  
        weaponIcon.sprite = currentWeapon.GetIcon; //Change Weapon Icon
    }

    public void UpdateHealth(float value)
    {
        healthBar.value = value;
    }

    void UpdateAmmo(int ammo)
    {
        //enable reload warning when ammo is 0
        reloadWarning.SetActive(ammo <= 0);
        //format bullet count
        ammoText.SetText(ammo.ToString("00"));
    }

    public void ShowHostageKilled(Vector3 pos, bool show)
    {
        hostageKilledText.gameObject.SetActive(show);

        if (!show)
            return;

        hostageKilledText.position = pos;
        Vector2 adjustPos = Extensions.GetPositionInsideScreen(new Vector2(1920f, 1080f), hostageKilledText, 25f);
        //apply to anchored position
        hostageKilledText.anchoredPosition = adjustPos;
    }

    public void ShowEndScreen(int enemyKill, int totalEnemy, int hostageKill, int totalShots, int totalHit)
    {
        //endscreen stats calculaion
        endScreenPanel.SetActive(true);
        enemyKilled.SetText(((enemyKill / (float)totalEnemy) * 100f).ToString("00") + "%");
        hostageKilled.SetText(hostageKill.ToString());
        shots.SetText(totalShots.ToString());
        hit.SetText(totalHit.ToString());
        accuracy.SetText(((totalHit / (totalShots == 0 ? 1f : (float)totalShots)) * 100f).ToString("00") + "%");

        CalculateScore(enemyKill, totalEnemy, hostageKill, totalShots, totalHit);
    }

    void CalculateScore(int enemyKill, int totalEnemy, int hostageKill, int totalShots, int totalHit)
    {
        float hostagePenalty = hostageKill * 15f;
        float enemyKillRatio = ((enemyKill / (float)totalEnemy) * 100f) - hostagePenalty;
        float accuracyRatio = ((totalHit / (totalShots == 0 ? 1f : (float)totalShots)) * 100f) - hostagePenalty;
        float totalAverage = (enemyKillRatio + accuracyRatio) / 2f;

        if (totalAverage >= 90f)
        {
            rankText.SetText("A");
        }
        else if (totalAverage >= 75f && totalAverage < 90f)
        {
            rankText.SetText("B");
        }
        else if (totalAverage >= 50f && totalAverage < 75f)
        {
            rankText.SetText("C");
        }
        else if (totalAverage == 0f)
        {
            rankText.SetText("F");
        }
        else if (totalAverage < 50f)
        {
            rankText.SetText("D");
        }

    }
    public void MoveCrosshair(Vector3 mousePosition)
    {
        if (crossHair != null)
            crossHair.position = mousePosition;
    }

    void GoToTitleScene()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}