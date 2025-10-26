using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Kecepatan jalan saat patrol")]
    public float moveSpeed = 2f;
    
    [Header("Random Movement")]
    [Tooltip("Radius area untuk random movement dari posisi awal")]
    public float roamRadius = 10f;
    
    [Tooltip("Jarak minimal ke target sebelum ganti tujuan baru")]
    public float reachDistance = 0.5f;
    
    [Tooltip("Waktu tunggu sebelum cari target baru")]
    public float waitTime = 2f;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    [Header("Rotation")]
    [Tooltip("Kecepatan rotasi menghadap target")]
    public float rotationSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private Vector2 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        GenerateNewTarget();
    }

    private void Update()
    {
        if (isWaiting)
        {
            // Tunggu dulu sebelum jalan lagi
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                GenerateNewTarget();
            }
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        
        if (distanceToTarget <= reachDistance)
        {
            // Sampai target, mulai tunggu
            isWaiting = true;
            if (rb != null) rb.velocity = Vector2.zero;
        }
        else
        {
            // Bergerak ke target
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        
        // Rotate menghadap target (optional, tergantung sprite)
        //if (direction != Vector2.zero)
        //{
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}

        // Gerakkan enemy
        if (rb != null)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            // Fallback pakai transform kalau ga ada rigidbody
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void GenerateNewTarget()
    {
        // Generate random position dalam radius
        Vector2 randomDirection = Random.insideUnitCircle * roamRadius;
        targetPosition = startPosition + randomDirection;
        
        //Debug.Log($"New patrol target: {targetPosition}");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi patrol area
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(center, roamRadius);
        
        // Draw line ke target saat patrol
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.3f);
        }
    }

    private void OnDisable()
    {
        if (rb != null) rb.velocity = Vector2.zero;
    }
}