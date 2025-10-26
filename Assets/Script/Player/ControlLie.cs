using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLie : MonoBehaviour
{
    [Header("Control Chaos Settings")]
    [Tooltip("Waktu dalam detik sebelum control mulai berubah jika tidak mengambil candy")]
    public float timeBeforeChaos = 10f;
    
    [Tooltip("Apakah control saat ini dalam mode chaos?")]
    public bool isControlChaos = false;
    
    [Header("Chaos Types")]
    [Tooltip("Inversi kontrol (atas jadi bawah, kiri jadi kanan)")]
    public bool canInvertControls = true;
    
    [Tooltip("Random direction saat menekan tombol")]
    public bool canRandomDirection = true;
    
    [Tooltip("Kontrol berputar 90 derajat (atas jadi kanan, kanan jadi bawah, dll)")]
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
    
    // Private variables
    private float chaosTimer;
    private float lastCandyCount;
    private int currentChaosType = 0; 
    private Queue<Vector2> delayedInputs = new Queue<Vector2>();
    private float inputDelayTimer = 0f;
    private float inputDelayAmount = 0.5f;
    private Vector2 lastRandomDirection = Vector2.zero;
    private float randomDirectionChangeTime = 0.3f;
    private float randomDirectionTimer = 0f;
    
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
        
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;
        
        CheckCandyCollection();
        UpdateChaosTimer();
        UpdateWarningUI();
        
        if (isControlChaos)
        {
            ApplyChaosControl();
        }
        else
        {
            if (playerMovement != null)
            {
                playerMovement.useCustomInput = false;
                playerMovement.inputModifier = Vector2.one;
            }
        }
    }

    void CheckCandyCollection()
    {
        if (playerData == null) return;
        
        // Jika candy bertambah, reset timer
        if (playerData.candy > lastCandyCount)
        {
            chaosTimer = timeBeforeChaos;
            isControlChaos = false;
            lastCandyCount = playerData.candy;
            
            // Reset ke kontrol normal
            currentChaosType = 0;
            delayedInputs.Clear();
            
            if (playerMovement != null)
            {
                playerMovement.useCustomInput = false;
                playerMovement.inputModifier = Vector2.one;
            }
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
        
        // Pilih tipe chaos random dari yang diaktifkan
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
        
        // Baca input original
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 originalInput = new Vector2(horizontal, vertical);
        Vector2 modifiedInput = Vector2.zero;
        
        switch (currentChaosType)
        {
            case 1: // Inverted
                playerMovement.useCustomInput = false;
                playerMovement.inputModifier = new Vector2(-1, -1);
                break;
                
            case 2: // Random Direction
                if (horizontal != 0 || vertical != 0)
                {
                    modifiedInput = GetRandomDirection(originalInput);
                    playerMovement.useCustomInput = true;
                    playerMovement.customInput = modifiedInput;
                }
                else
                {
                    playerMovement.useCustomInput = false;
                }
                break;
                
            case 3: // Rotated 90 degrees (Up->Right, Right->Down, etc)
                playerMovement.useCustomInput = true;
                playerMovement.customInput = new Vector2(-vertical, horizontal);
                break;
                
            case 4: // Delayed
                if (horizontal != 0 || vertical != 0)
                {
                    modifiedInput = GetDelayedInput(originalInput);
                    playerMovement.useCustomInput = true;
                    playerMovement.customInput = modifiedInput;
                }
                else
                {
                    playerMovement.useCustomInput = false;
                }
                break;
                
            default:
                playerMovement.useCustomInput = false;
                playerMovement.inputModifier = Vector2.one;
                break;
        }
    }

    Vector2 GetRandomDirection(Vector2 currentInput)
    {
        // Update arah random setiap beberapa waktu
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
        
        return lastRandomDirection;
    }

    Vector2 GetDelayedInput(Vector2 currentInput)
    {
        // Tambahkan input ke queue
        delayedInputs.Enqueue(currentInput);
        
        // Batasi ukuran queue
        if (delayedInputs.Count > 10)
            delayedInputs.Dequeue();
        
        inputDelayTimer += Time.deltaTime;
        
        // Ambil delayed input
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
            // Warning: chaos akan aktif
            warningUI.gameObject.SetActive(true);
            warningUI.text = $"COLLECT CANDY! ({Mathf.Ceil(chaosTimer)}s)";
            warningUI.color = Color.Lerp(originalWarningColor, warningColor, Mathf.PingPong(Time.time * 2, 1));
        }
        else if (isControlChaos)
        {
            // Chaos aktif
            warningUI.gameObject.SetActive(true);
            warningUI.text = $"CURSED! {GetChaosTypeName()}";
            warningUI.color = warningColor;
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

    // Method untuk force activate chaos (untuk testing)
    public void ForceActivateChaos(int chaosType = -1)
    {
        if (chaosType >= 0 && chaosType <= 4)
            currentChaosType = chaosType;
        else
            ActivateChaos();
        
        isControlChaos = true;
    }

    // Method untuk reset ke normal
    public void ResetToNormal()
    {
        isControlChaos = false;
        currentChaosType = 0;
        chaosTimer = timeBeforeChaos;
        delayedInputs.Clear();
        
        if (playerMovement != null)
        {
            playerMovement.useCustomInput = false;
            playerMovement.inputModifier = Vector2.one;
        }
    }
}