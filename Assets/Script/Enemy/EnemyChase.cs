using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Chase Settings")]
    [Tooltip("Kecepatan saat ngejar player")]
    public float chaseSpeed = 5f;
    
    [Header("Catch Settings")]
    [Tooltip("Jarak untuk menangkap player")]
    public float catchDistance = 1f;

    [Header("Rotation")]
    [Tooltip("Kecepatan rotasi menghadap player")]
    public float rotationSpeed = 8f;

    [Header("Prediction")]
    [Tooltip("Prediksi gerakan player (lebih smart)")]
    public bool usePrediction = true;
    [Tooltip("Seberapa jauh prediksi ke depan")]
    public float predictionTime = 0.3f;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Transform player;
    private EnemyController controller;
    private EnemyAudio audioController;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<EnemyController>();
        audioController = GetComponent<EnemyAudio>();
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }

    private void OnEnable()
    {
        // Start chase audio saat script enabled (mulai ngejar)
        if (audioController != null)
        {
            audioController.StartChaseAudio();
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Chase player
        ChasePlayer();

        // Cek apakah sudah tangkep player
        CheckCatchPlayer();
    }

    private void ChasePlayer()
    {
        Vector2 targetPosition = player.position;

        if (usePrediction && playerRb != null)
        {
            Vector2 playerVelocity = playerRb.velocity;
            targetPosition += playerVelocity * predictionTime;
        }

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Rotate menghadap player
        // if (direction != Vector2.zero)
        // {
        //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //     Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // }

        // Gerakkan enemy
        if (rb != null)
        {
            rb.velocity = direction * chaseSpeed;
        }
        else
        {
            transform.Translate(direction * chaseSpeed * Time.deltaTime, Space.World);
        }
    }

    private void CheckCatchPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= catchDistance)
        {
            CatchPlayer();
        }
    }

    private void CatchPlayer()
    {
        //Debug.Log("Player Caught!");
        
        if (audioController != null)
        {
            audioController.PlayCaughtSound();
        }
        
        if (controller != null)
        {
            controller.OnPlayerCaught();
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);

        if (player != null && this.enabled)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    private void OnDisable()
    {
        if (rb != null) rb.velocity = Vector2.zero;
        
        if (audioController != null)
        {
            audioController.StopChaseAudio();
        }
    }
}