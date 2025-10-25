using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Over")]
    public GameObject gameOverUI;
    public float restartDelay = 2f;

    public bool isGameOver = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        
        Debug.Log("GAME OVER!");
        
        // Show game over UI
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Freeze game
        Time.timeScale = 0f;

        // Auto restart after delay (optional)
        // Invoke("RestartGame", restartDelay);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}