using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("Panel yang muncul saat kalah")]
    public GameObject losePanel;
    [Tooltip("Panel yang muncul saat menang")]
    public GameObject winPanel;

    [Header("Lose Panel Buttons")]
    [Tooltip("Button Restart di panel lose")]
    public Button loseRestartButton;
    [Tooltip("Button Back to Main di panel lose")]
    public Button loseMainMenuButton;

    [Header("Win Panel Buttons")]
    [Tooltip("Button Continue di panel win")]
    public Button winContinueButton;
    [Tooltip("Button Restart di panel win")]
    public Button winRestartButton;
    [Tooltip("Button Back to Main di panel win")]
    public Button winMainMenuButton;

    [Header("Scene Names")]
    [Tooltip("Nama scene main menu")]
    public string mainMenuSceneName = "MainMenu";
    [Tooltip("Nama scene level berikutnya (untuk continue)")]
    public string nextLevelSceneName = "Level2";

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (loseRestartButton != null)
            loseRestartButton.onClick.AddListener(OnLoseRestart);

        if (loseMainMenuButton != null)
            loseMainMenuButton.onClick.AddListener(OnBackToMainMenu);

        if (winContinueButton != null)
            winContinueButton.onClick.AddListener(OnWinContinue);

        if (winRestartButton != null)
            winRestartButton.onClick.AddListener(OnWinRestart);

        if (winMainMenuButton != null)
            winMainMenuButton.onClick.AddListener(OnBackToMainMenu);
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

    // === LOSE PANEL BUTTONS ===
    private void OnLoseRestart()
    {
        PlayButtonSound();
        RestartCurrentLevel();
    }

    // === WIN PANEL BUTTONS ===
    private void OnWinContinue()
    {
        PlayButtonSound();
        LoadNextLevel();
    }

    private void OnWinRestart()
    {
        PlayButtonSound();
        RestartCurrentLevel();
    }

    // === SHARED BUTTONS ===
    private void OnBackToMainMenu()
    {
        PlayButtonSound();
        LoadMainMenu();
    }

    // === SCENE LOADING ===
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
            // Kalau ga ada, load level selanjutnya berdasarkan build index
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                // Kalau udah level terakhir, balik ke main menu
                Debug.Log("No more levels! Going back to main menu.");
                LoadMainMenu();
            }
        }
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    // === AUDIO ===
    private void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    // === PUBLIC METHODS untuk dipanggil dari GameManager ===
    public void OnGameOver()
    {
        ShowLosePanel();
    }

    public void OnGameWin()
    {
        ShowWinPanel();
    }
}