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
    public string gameSceneName = "Game"; // ubah sesuai nama scene

    public void OnNewGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnContinue()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void OnCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void OnCloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void OnCloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void OnExit()
    {
        Debug.Log("Quit Game");
        Application.Quit();

    }
}
