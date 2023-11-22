using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] GameObject optionPanel;
    [SerializeField] string gamePlayScene;

    [SerializeField] Button startButton, optionButton, optionCloseButton, quitButton;
    [SerializeField] AudioGetter clickSfx;

    void Start()
    {
        Cursor.visible = true;
        optionPanel.SetActive(false);
        startButton.Select();

        // Add click listeners to buttons
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(OpenOptionPanel);
        optionCloseButton.onClick.AddListener(CloseOptionPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        if (optionPanel.activeInHierarchy)
            return;

        AudioPlayer.Instance.PlaySFX(clickSfx);

        // Load the gameplay scene
        SceneManager.LoadScene(gamePlayScene);
    }

    void OpenOptionPanel()
    {
        AudioPlayer.Instance.PlaySFX(clickSfx);
        optionCloseButton.Select();
        optionPanel.SetActive(true);
    }

    void CloseOptionPanel()
    {
        AudioPlayer.Instance.PlaySFX(clickSfx);
        optionPanel.SetActive(false);
        optionButton.Select();
    }

    void QuitGame()
    {
        AudioPlayer.Instance.PlaySFX(clickSfx);
        Application.Quit();
    }
}
