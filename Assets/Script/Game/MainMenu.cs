using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Game Scene Name")]
    public string gameSceneName = "Game";

    public void OnNewGame()
    {
        if (TransitionScene.Instance != null)
        {
            TransitionScene.Instance.LoadSceneWithTransition(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OnContinue()
    {
        if (TransitionScene.Instance != null)
        {
            TransitionScene.Instance.LoadSceneWithTransition(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OnOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void OnCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    public void OnCloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void OnCloseCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    public void OnExit()
    {
        Debug.Log("Quit Game");
        
        if (TransitionScene.Instance != null)
        {
            TransitionScene.Instance.QuitGameWithTransition();
        }
        else
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}