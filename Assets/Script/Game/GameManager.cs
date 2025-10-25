using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isGameWon = false;

    [Header("UI Panels")]
    [Tooltip("Panel yang muncul saat kalah")]
    public GameObject gameOverPanel;
    [Tooltip("Panel yang muncul saat menang")]
    public GameObject gameWinPanel;

    [Header("Player Reference")]
    [Tooltip("Reference ke PlayerMovement untuk disable movement")]
    public PlayerMovement playerMovement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameWinPanel != null)
            gameWinPanel.SetActive(false);

        if (playerMovement == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over!");

        FreezeGame();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void GameWin()
    {
        if (isGameWon) return;

        isGameWon = true;
        isGameOver = true;
        Debug.Log("You Win!");

        FreezeGame();

        // Play normal BGM
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayNormalBGM();

        // Show win panel
        if (gameWinPanel != null)
            gameWinPanel.SetActive(true);
    }

    private void FreezeGame()
    {
        // Stop player movement
        if (playerMovement != null)
            playerMovement.canMove = false;

        // Freeze semua enemy
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.patrolScript != null)
                enemy.patrolScript.enabled = false;
            if (enemy.chaseScript != null)
                enemy.chaseScript.enabled = false;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}