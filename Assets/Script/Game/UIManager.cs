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

    [Header("Joystick UI")]
    [Tooltip("GameObject joystick UI untuk di-hide saat pause/lose/win")]
    public GameObject joystickUI;

    private bool isPaused = false;

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Auto find joystick UI jika belum di-assign
        if (joystickUI == null)
        {
            Joystick joystick = FindObjectOfType<Joystick>();
            if (joystick != null)
                joystickUI = joystick.gameObject;
        }
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
        {
            losePanel.SetActive(true);
            HideJoystick();
            PlayButtonSound();
        }
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            HideJoystick();
            PlayButtonSound();
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            
            HideJoystick();
            
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
            
            ShowJoystick();
            
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

    private void HideJoystick()
    {
        if (joystickUI != null)
            joystickUI.SetActive(false);
    }

    private void ShowJoystick()
    {
        if (joystickUI != null)
            joystickUI.SetActive(true);
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