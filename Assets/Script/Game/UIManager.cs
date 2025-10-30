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
    [Tooltip("Panel yang muncul saat pause")]
    public GameObject pausePanel;

    [Header("Scene Names")]
    [Tooltip("Nama scene main menu")]
    public string mainMenuSceneName = "MainMenu";
    [Tooltip("Nama scene level berikutnya (untuk continue)")]
    public string nextLevelSceneName = "Level2";

    private bool isPaused = false;

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

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

    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            PlayButtonSound();
        }
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            PlayButtonSound();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void RestartLevel()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ContinueToNextLevel()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        isPaused = false;
        
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                //Debug.Log("No more levels! Going back to main menu.");
                BackToMainMenu();
            }
        }
    }

    public void BackToMainMenu()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        isPaused = false;
        
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    private void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void OnGameOver()
    {
        ShowLosePanel();
    }

    public void OnGameWin()
    {
        ShowWinPanel();
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}