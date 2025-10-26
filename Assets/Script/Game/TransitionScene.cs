using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionScene : MonoBehaviour
{
    public static TransitionScene Instance;

    [Header("Transition Settings")]
    [Tooltip("Durasi transisi (detik)")]
    public float transitionDuration = 1f;

    [Header("UI References")]
    [Tooltip("Image untuk wipe effect")]
    public Image transitionImage;
    
    [Tooltip("Canvas Group (optional)")]
    public CanvasGroup canvasGroup;

    [Header("Optional Loading")]
    [Tooltip("Tampilkan loading text")]
    public bool showLoadingScreen = false;
    
    [Tooltip("Loading text")]
    public Text loadingText;

    private bool isTransitioning = false;
    private RectTransform imageRect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SetupTransition();
            
            StartCoroutine(WipeIn());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupTransition()
    {
        if (transitionImage != null)
        {
            imageRect = transitionImage.GetComponent<RectTransform>();
            transitionImage.color = Color.black;
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void LoadSceneWithTransition(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToScene(sceneName));
        }
    }

    public void LoadSceneWithTransition(int sceneIndex)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToScene(sceneIndex));
        }
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;

        yield return StartCoroutine(WipeOut());

        if (showLoadingScreen)
        {
            yield return StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

        yield return StartCoroutine(WipeIn());

        isTransitioning = false;
    }

    private IEnumerator TransitionToScene(int sceneIndex)
    {
        isTransitioning = true;

        yield return StartCoroutine(WipeOut());

        if (showLoadingScreen)
        {
            yield return StartCoroutine(LoadSceneAsync(sceneIndex));
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }

        yield return StartCoroutine(WipeIn());

        isTransitioning = false;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            if (loadingText != null)
            {
                loadingText.text = $"Loading... {Mathf.RoundToInt(asyncLoad.progress * 100)}%";
            }
            yield return null;
        }

        if (loadingText != null)
        {
            loadingText.text = "Loading... 100%";
        }

        yield return new WaitForSeconds(0.3f);
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            if (loadingText != null)
            {
                loadingText.text = $"Loading... {Mathf.RoundToInt(asyncLoad.progress * 100)}%";
            }
            yield return null;
        }

        if (loadingText != null)
        {
            loadingText.text = "Loading... 100%";
        }

        yield return new WaitForSeconds(0.3f);
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator WipeOut()
    {
        if (imageRect == null) yield break;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        float elapsed = 0f;

        imageRect.pivot = new Vector2(1f, 0f);
        imageRect.anchorMin = new Vector2(1f, 0f);
        imageRect.anchorMax = new Vector2(1f, 0f);
        imageRect.anchoredPosition = Vector2.zero;

        Canvas canvas = imageRect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 screenSize = canvasRect.sizeDelta;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            t = 1f - Mathf.Pow(1f - t, 3f);

            float diagonal = Mathf.Sqrt(screenSize.x * screenSize.x + screenSize.y * screenSize.y);
            float size = diagonal * 1.5f * t;

            imageRect.sizeDelta = new Vector2(size, size);

            yield return null;
        }

        float finalDiagonal = Mathf.Sqrt(screenSize.x * screenSize.x + screenSize.y * screenSize.y) * 1.5f;
        imageRect.sizeDelta = new Vector2(finalDiagonal, finalDiagonal);
    }

    private IEnumerator WipeIn()
    {
        if (imageRect == null) yield break;

        float elapsed = 0f;

        imageRect.pivot = new Vector2(0f, 1f); // Pivot di kiri atas
        imageRect.anchorMin = new Vector2(0f, 1f); // Anchor kiri atas
        imageRect.anchorMax = new Vector2(0f, 1f);
        imageRect.anchoredPosition = Vector2.zero;

        Canvas canvas = imageRect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 screenSize = canvasRect.sizeDelta;

        float diagonal = Mathf.Sqrt(screenSize.x * screenSize.x + screenSize.y * screenSize.y);
        float startSize = diagonal * 1.5f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            t = Mathf.Pow(t, 3f);

            float size = startSize * (1f - t);

            imageRect.sizeDelta = new Vector2(size, size);

            yield return null;
        }

        imageRect.sizeDelta = Vector2.zero;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadSceneWithTransition(currentSceneName);
    }

    public void QuitGameWithTransition()
    {
        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator QuitGameCoroutine()
    {
        yield return StartCoroutine(WipeOut());
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}