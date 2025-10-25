using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("Lose Panel Buttons")]
    public Button loseRestartButton;
    public Button loseMainMenuButton;

    [Header("Win Panel Buttons")]
    public Button winContinueButton;
    public Button winRestartButton;
    public Button winMainMenuButton;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string nextLevelSceneName = "Level2";

    private void Start()
    {
        if (losePanel != null) losePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (loseRestartButton != null) loseRestartButton.onClick.AddListener(OnLoseRestart);
        if (loseMainMenuButton != null) loseMainMenuButton.onClick.AddListener(OnBackToMainMenu);
        if (winContinueButton != null) winContinueButton.onClick.AddListener(OnWinContinue);
        if (winRestartButton != null) winRestartButton.onClick.AddListener(OnWinRestart);
        if (winMainMenuButton != null) winMainMenuButton.onClick.AddListener(OnBackToMainMenu);
    }

    // === SHOW PANELS ===
    public void ShowLosePanel()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
            PlayButtonSound();
        }
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            PlayButtonSound();
        }
    }

    // === BUTTON LOGIC ===
    private void OnLoseRestart() { PlayButtonSound(); RestartCurrentLevel(); }
    private void OnWinContinue() { PlayButtonSound(); LoadNextLevel(); }
    private void OnWinRestart() { PlayButtonSound(); RestartCurrentLevel(); }
    private void OnBackToMainMenu() { PlayButtonSound(); LoadMainMenu(); }

    // === SCENE HANDLING ===
    private void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextLevel()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextSceneIndex);
            else
                LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            SceneManager.LoadScene(0);
    }

    // === AUDIO ===
    private void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFootstep();
    }

    // === PUBLIC METHODS ===
    public void OnGameOver() => ShowLosePanel();
    public void OnGameWin() => ShowWinPanel();
}
