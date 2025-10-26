using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Animator component untuk enemy")]
    public Animator animator;
    
    [Tooltip("SpriteRenderer untuk flip horizontal")]
    public SpriteRenderer spriteRenderer;
    
    [Tooltip("Rigidbody2D untuk deteksi movement")]
    public Rigidbody2D rb;
    
    [Tooltip("Enemy Controller untuk cek state")]
    public EnemyController enemyController;

    [Header("Animation Parameters")]
    [Tooltip("Nama parameter bool untuk berjalan ke belakang/atas")]
    public string isWalkingBackParam = "IsWalkingBack";
    
    [Tooltip("Nama parameter bool untuk isChasing (optional)")]
    public string isChasingParam = "IsChasing";

    [Header("Settings")]
    [Tooltip("Threshold kecepatan minimum untuk dianggap bergerak")]
    public float movementThreshold = 0.1f;

    private Vector2 lastDirection = Vector2.down;

    private void Start()
    {
        // Auto-assign components kalau belum di-set
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (enemyController == null)
            enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null || rb == null) return;

        Vector2 velocity = rb.velocity;
        float speed = velocity.magnitude;

        bool isMoving = speed > movementThreshold;

        if (isMoving)
        {
            lastDirection = velocity.normalized;
        }

        // Tentukan apakah jalan ke belakang (atas) atau depan/samping
        bool isWalkingBack = false;
        
        if (isMoving)
        {
            // Kalau velocity Y positif (jalan ke atas), pakai Ghost_Back
            if (lastDirection.y > 0.5f)
            {
                isWalkingBack = true;
            }
            // Kalau ke bawah, kiri, atau kanan, pakai Ghost_Walk
            else
            {
                isWalkingBack = false;
            }

            // Handle flip untuk kiri/kanan
            if (spriteRenderer != null)
            {
                // Kalau bergerak ke kiri (velocity X negatif)
                if (lastDirection.x < -0.1f)
                {
                    spriteRenderer.flipX = true;
                }
                // Kalau bergerak ke kanan (velocity X positif)
                else if (lastDirection.x > 0.1f)
                {
                    spriteRenderer.flipX = false;
                }
                // Kalau gerak vertikal, biarkan flip terakhir
            }
        }

        if (!string.IsNullOrEmpty(isWalkingBackParam))
        {
            animator.SetBool(isWalkingBackParam, isWalkingBack);
        }

        if (!string.IsNullOrEmpty(isChasingParam) && enemyController != null)
        {
            bool isChasing = (enemyController.currentState == EnemyState.CHASE);
            animator.SetBool(isChasingParam, isChasing);
        }
    }

    public Vector2 GetCurrentDirection()
    {
        return lastDirection;
    }
}
