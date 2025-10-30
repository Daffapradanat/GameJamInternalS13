using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLie : MonoBehaviour
{
    [Header("Control Chaos Settings")]
    [Tooltip("Waktu dalam detik sebelum control mulai berubah jika tidak mengambil candy")]
    public float timeBeforeChaos = 10f;
    
    [Header("Chaos Types")]
    [Tooltip("Inversi kontrol (atas jadi bawah, kiri jadi kanan)")]
    public bool canInvertControls = true;
    [Tooltip("Random direction saat menekan tombol")]
    public bool canRandomDirection = true;
    [Tooltip("Kontrol berputar 90 derajat")]
    public bool canRotateControls = true;
    [Tooltip("Kontrol delay (input tertunda)")]
    public bool canDelayInput = true;
    
    [Header("Visual Feedback")]
    [Tooltip("UI teks untuk warning chaos")]
    public TMPro.TextMeshProUGUI warningUI;
    [Tooltip("Warna teks warning")]
    public Color warningColor = Color.red;
    
    [Header("References")]
    [Tooltip("Reference ke PlayerMovement")]
    public PlayerMovement playerMovement;
    [Tooltip("Reference ke PlayerData")]
    public PlayerData playerData;
    
    private bool isControlChaos = false;
    private float chaosTimer;
    private float lastCandyCount;
    private int currentChaosType = 0;
    
    private Queue<Vector2> delayedInputs = new Queue<Vector2>();
    private float inputDelayTimer = 0f;
    private const float inputDelayAmount = 0.5f;
    private Vector2 lastRandomDirection = Vector2.zero;
    private float randomDirectionTimer = 0f;
    private const float randomDirectionChangeTime = 0.3f;
    
    private Color originalWarningColor;
    private bool isInitialized = false;

    void Start()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
        if (playerData == null)
            playerData = GetComponent<PlayerData>();
        
        chaosTimer = timeBeforeChaos;
        if (playerData != null)
            lastCandyCount = playerData.candy;
        
        if (warningUI != null)
        {
            originalWarningColor = warningUI.color;
            warningUI.gameObject.SetActive(false);
        }
        
        StartCoroutine(InitializeDelay());
    }
    
    IEnumerator InitializeDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        
        CheckCandyCollection();
        UpdateChaosTimer();
        UpdateWarningUI();
        
        if (isControlChaos)
        {
            ApplyChaosControl();
        }
    }

    void CheckCandyCollection()
    {
        if (playerData == null) return;
        
        if (playerData.candy > lastCandyCount)
        {
            ResetToNormal();
            lastCandyCount = playerData.candy;
        }
    }

    void UpdateChaosTimer()
    {
        if (isControlChaos) return;
        
        chaosTimer -= Time.deltaTime;
        
        if (chaosTimer <= 0)
        {
            ActivateChaos();
        }
    }

    void ActivateChaos()
    {
        isControlChaos = true;
        
        List<int> availableChaos = new List<int>();
        if (canInvertControls) availableChaos.Add(1);
        if (canRandomDirection) availableChaos.Add(2);
        if (canRotateControls) availableChaos.Add(3);
        if (canDelayInput) availableChaos.Add(4);
        
        if (availableChaos.Count > 0)
        {
            currentChaosType = availableChaos[Random.Range(0, availableChaos.Count)];
        }
    }

    void ApplyChaosControl()
    {
        if (playerMovement == null) return;
        
        Vector2 originalInput = playerMovement.GetProcessedInput();
        
        if (originalInput.magnitude < 0.01f)
        {
            playerMovement.SetCustomInput(Vector2.zero);
            return;
        }
        
        Vector2 modifiedInput = Vector2.zero;
        
        switch (currentChaosType)
        {
            case 1: // Inverted
                modifiedInput = -originalInput;
                break;
                
            case 2: // Random Direction
                modifiedInput = GetRandomDirection(originalInput);
                break;
                
            case 3: // Rotated 90 degrees
                modifiedInput = new Vector2(-originalInput.y, originalInput.x);
                break;
                
            case 4: // Delayed
                modifiedInput = GetDelayedInput(originalInput);
                break;
                
            default:
                modifiedInput = originalInput;
                break;
        }
        
        playerMovement.SetCustomInput(modifiedInput);
    }

    Vector2 GetRandomDirection(Vector2 currentInput)
    {
        randomDirectionTimer += Time.deltaTime;
        
        if (randomDirectionTimer >= randomDirectionChangeTime || lastRandomDirection == Vector2.zero)
        {
            randomDirectionTimer = 0f;
            
            int randomDir = Random.Range(0, 4);
            switch (randomDir)
            {
                case 0: lastRandomDirection = Vector2.up; break;
                case 1: lastRandomDirection = Vector2.right; break;
                case 2: lastRandomDirection = Vector2.down; break;
                case 3: lastRandomDirection = Vector2.left; break;
            }
        }
        
        return lastRandomDirection.normalized * currentInput.magnitude;
    }

    Vector2 GetDelayedInput(Vector2 currentInput)
    {
        delayedInputs.Enqueue(currentInput);
        
        if (delayedInputs.Count > 10)
            delayedInputs.Dequeue();
        
        inputDelayTimer += Time.deltaTime;
        
        if (inputDelayTimer >= inputDelayAmount && delayedInputs.Count > 2)
        {
            inputDelayTimer = 0f;
            return delayedInputs.Dequeue();
        }
        
        return delayedInputs.Count > 0 ? delayedInputs.Peek() : Vector2.zero;
    }

    void UpdateWarningUI()
    {
        if (warningUI == null) return;
        
        if (!isControlChaos && chaosTimer <= 5f && chaosTimer > 0)
        {
            warningUI.gameObject.SetActive(true);
            warningUI.text = $"COLLECT CANDY! ({Mathf.Ceil(chaosTimer)}s)";
            warningUI.color = Color.Lerp(originalWarningColor, warningColor, Mathf.PingPong(Time.time * 2, 1));
        }
        else if (isControlChaos)
        {
            warningUI.gameObject.SetActive(true);
            warningUI.color = warningColor;
            warningUI.text = $"<size=60%>CURSED!</size>\n<size=40%>{GetChaosTypeName()}</size>";
        }
        else
        {
            warningUI.gameObject.SetActive(false);
        }
    }

    string GetChaosTypeName()
    {
        switch (currentChaosType)
        {
            case 1: return "INVERTED";
            case 2: return "RANDOM";
            case 3: return "ROTATED";
            case 4: return "DELAYED";
            default: return "NORMAL";
        }
    }

    public void ResetToNormal()
    {
        isControlChaos = false;
        currentChaosType = 0;
        chaosTimer = timeBeforeChaos;
        delayedInputs.Clear();
        lastRandomDirection = Vector2.zero;
        randomDirectionTimer = 0f;
    }

    public void ForceActivateChaos(int chaosType = -1)
    {
        if (chaosType >= 1 && chaosType <= 4)
            currentChaosType = chaosType;
        else
            ActivateChaos();
        
        isControlChaos = true;
    }
}