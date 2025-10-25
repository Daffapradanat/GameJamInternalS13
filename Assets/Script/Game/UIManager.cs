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
    }

    // === SHOW PANELS ===
    public void ShowLosePanel()
    {
        if (losePanel != null)
            losePanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    // === BUTTON METHODS (dipanggil dari OnClick) ===
    
    // Restart current level
    public void RestartLevel()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Continue ke level berikutnya
    public void ContinueToNextLevel()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            // Load level selanjutnya berdasarkan build index
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                // Kalau udah level terakhir, balik ke main menu
                Debug.Log("No more levels! Going back to main menu.");
                BackToMainMenu();
            }
        }
    }

    // Back to main menu
    public void BackToMainMenu()
    {
        PlayButtonSound();
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