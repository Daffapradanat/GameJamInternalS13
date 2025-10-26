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
        // Stop player movement completely
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
            
            // Stop velocity jika ada Rigidbody2D
            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            
            // Disable animator jika ada untuk freeze animasi
            Animator animator = playerMovement.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
        }

        // Freeze semua enemy
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.patrolScript != null)
                enemy.patrolScript.enabled = false;
            if (enemy.chaseScript != null)
                enemy.chaseScript.enabled = false;
                
            // Stop enemy velocity
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.velocity = Vector2.zero;
                enemyRb.angularVelocity = 0f;
            }
            
            // Disable enemy animator
            Animator enemyAnimator = enemy.GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                enemyAnimator.enabled = false;
            }
        }
    }
}