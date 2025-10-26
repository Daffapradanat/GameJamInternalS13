using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public EnemyPatrol patrolScript;
    public EnemyVision visionScript;
    public EnemyChase chaseScript;
    public Transform player;

    [Header("State Settings")]
    public EnemyState currentState = EnemyState.PATROL;
    
    [Header("Lost Player Timer")]
    [Tooltip("Waktu sebelum kembali patrol saat kehilangan player")]
    public float lostPlayerTime = 3f;
    private float lostPlayerTimer = 0f;

    [Header("Circle Light (State Indicator)")]
    [Tooltip("Objek lampu lingkaran yang mengitari enemy")]
    public GameObject circleLight;
    [Tooltip("Renderer/SpriteRenderer dari circle light")]
    public SpriteRenderer circleLightRenderer;
    [Tooltip("Light2D component (optional)")]
    public UnityEngine.Rendering.Universal.Light2D circleLightComponent;
    
    [Header("Light Colors")]
    public Color patrolColor = Color.green;
    public Color chaseColor = Color.red;

    [Header("Speed Settings")]
    [Tooltip("Kecepatan saat patrol")]
    public float patrolSpeed = 2f;
    [Tooltip("Kecepatan saat chase (harus lebih cepat!)")]
    public float chaseSpeed = 5f;

    private void Start()
    {
        if (patrolScript == null) patrolScript = GetComponent<EnemyPatrol>();
        if (visionScript == null) visionScript = GetComponent<EnemyVision>();
        if (chaseScript == null) chaseScript = GetComponent<EnemyChase>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (patrolScript != null) patrolScript.moveSpeed = patrolSpeed;
        if (chaseScript != null) chaseScript.chaseSpeed = chaseSpeed;

        SwitchState(EnemyState.PATROL);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        switch (currentState)
        {
            case EnemyState.PATROL:
                HandlePatrolState();
                break;
            case EnemyState.CHASE:
                HandleChaseState();
                break;
        }
    }

    private void HandlePatrolState()
    {
        if (patrolScript != null && !patrolScript.enabled)
            patrolScript.enabled = true;

        if (chaseScript != null && chaseScript.enabled)
            chaseScript.enabled = false;

        if (visionScript != null && visionScript.CanSeePlayer())
        {
            SwitchState(EnemyState.CHASE);
        }
    }

    private void HandleChaseState()
    {
        if (patrolScript != null && patrolScript.enabled)
            patrolScript.enabled = false;

        if (chaseScript != null && !chaseScript.enabled)
            chaseScript.enabled = true;

        if (visionScript != null && !visionScript.CanSeePlayer())
        {
            lostPlayerTimer += Time.deltaTime;
            
            if (lostPlayerTimer >= lostPlayerTime)
            {
                SwitchState(EnemyState.PATROL);
                lostPlayerTimer = 0f;
            }
        }
        else
        {
            lostPlayerTimer = 0f;
        }
    }

    public void SwitchState(EnemyState newState)
    {
        currentState = newState;

        if (AudioManager.Instance != null)
        {
            if (newState == EnemyState.CHASE)
            {
                AudioManager.Instance.PlayTenseBGM();
            }
            else if (newState == EnemyState.PATROL)
            {
                AudioManager.Instance.PlayNormalBGM();
            }
        }

        UpdateVisualIndicator();

        //Debug.Log($"Enemy State: {newState}");
    }

    private void UpdateVisualIndicator()
    {
        Color targetColor = currentState == EnemyState.PATROL ? patrolColor : chaseColor;

        if (circleLightRenderer != null)
        {
            circleLightRenderer.color = targetColor;
        }

        if (circleLightComponent != null)
        {
            circleLightComponent.color = targetColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerCaught();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerCaught();
        }
    }

    public void OnPlayerCaught()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        //Debug.Log("GAME OVER - Player Caught!");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayJumpScare();

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}

public enum EnemyState
{
    PATROL,
    CHASE
}