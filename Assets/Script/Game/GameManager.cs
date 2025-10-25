using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool isGameOver = false;
    public bool isGameWon = false;

    [Header("UI Manager")]
    [Tooltip("Reference ke GameUIManager")]
    public UiManager uiManager;

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
        if (uiManager == null)
            uiManager = FindObjectOfType<UiManager>();

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

        if (uiManager != null)
            uiManager.OnGameOver();
    }

    public void GameWin()
    {
        if (isGameWon) return;

        isGameWon = true;
        isGameOver = true;
        Debug.Log("You Win!");

        FreezeGame();

        // Show win panel
        if (uiManager != null)
            uiManager.OnGameWin();
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
}