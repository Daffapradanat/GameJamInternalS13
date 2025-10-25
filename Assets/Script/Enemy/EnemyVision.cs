using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [Tooltip("Jarak pandang maksimal (radius cone)")]
    public float viewDistance = 10f;
    
    [Tooltip("Sudut pandang cone (total angle)")]
    [Range(0f, 360f)]
    public float viewAngle = 90f;
    
    [Header("Detection")]
    [Tooltip("Layer untuk obstacle yang menghalangi vision")]
    public LayerMask obstacleMask;
    
    [Tooltip("Tag player untuk detection")]
    public string playerTag = "Player";

    [Header("Raycast Settings")]
    [Tooltip("Vision TIDAK bisa tembus obstacle (recommended untuk balanced gameplay)")]
    public bool blockVisionByObstacles = true;

    [Header("Vision Light Objects")]
    [Tooltip("Objek spotlight untuk vision cone")]
    public GameObject visionLight;
    [Tooltip("Light2D component untuk vision cone")]
    public UnityEngine.Rendering.Universal.Light2D visionLightComponent;
    [Tooltip("Update light color based on detection")]
    public bool changeLightColorOnDetection = true;
    public Color normalVisionColor = Color.yellow;
    public Color detectedVisionColor = Color.red;

    [Header("Player Circle Light (Detection Indicator)")]
    [Tooltip("Circle light di player yang berubah warna saat terdeteksi")]
    public GameObject playerCircleLight;
    [Tooltip("Renderer dari player circle light")]
    public SpriteRenderer playerCircleLightRenderer;
    [Tooltip("Light2D component dari player circle light")]
    public UnityEngine.Rendering.Universal.Light2D playerCircleLightComponent;
    [Tooltip("Warna saat player aman (tidak terdeteksi)")]
    public Color playerSafeColor = Color.green;
    [Tooltip("Warna saat player terdeteksi (bahaya!)")]
    public Color playerDetectedColor = Color.red;

    private Transform player;
    private bool canSeePlayer = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            canSeePlayer = CheckPlayerInVisionCone();
            
            UpdateVisionLight();
            
            UpdatePlayerCircleLight();
        }
    }

    private void UpdateVisionLight()
    {
        if (visionLightComponent != null && changeLightColorOnDetection)
        {
            visionLightComponent.color = canSeePlayer ? detectedVisionColor : normalVisionColor;
        }
    }

    private void UpdatePlayerCircleLight()
    {
        Color targetColor = canSeePlayer ? playerDetectedColor : playerSafeColor;

        if (playerCircleLightRenderer != null)
        {
            playerCircleLightRenderer.color = targetColor;
        }

        if (playerCircleLightComponent != null)
        {
            playerCircleLightComponent.color = targetColor;
        }
    }

    private bool CheckPlayerInVisionCone()
    {
        // Hitung direction dan distance ke player
        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Cek 1: Apakah dalam jangkauan distance?
        if (distanceToPlayer > viewDistance)
        {
            return false;
        }

        // Cek 2: Apakah dalam sudut cone?
        Vector2 enemyForward = transform.up;
        
        float angleToPlayer = Vector2.Angle(enemyForward, directionToPlayer);

        if (angleToPlayer > viewAngle / 2f)
        {
            return false;
        }

        // Cek 3: Apakah ada obstacle menghalangi? (PENTING untuk balancing!)
        if (blockVisionByObstacles)
        {
            // Raycast dari enemy ke player
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, 
                directionToPlayer.normalized, 
                distanceToPlayer, 
                obstacleMask
            );
            
            // Kalau ada obstacle ketabrak sebelum sampai player = terhalang
            if (hit.collider != null)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return false;
            }
        }

        Debug.DrawLine(transform.position, player.position, Color.green);

        // Player terdeteksi!
        return true;
    }

    public bool CanSeePlayer()
    {
        return canSeePlayer;
    }

    // Visualisasi vision cone di editor
    private void OnDrawGizmos()
    {
        // Warna berdasarkan state
        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;

        // Posisi enemy
        Vector3 position = transform.position;
        
        // Direction enemy (sesuaikan dengan orientasi sprite)
        Vector3 forward = transform.up;
        
        // Draw vision radius
        Gizmos.DrawWireSphere(position, viewDistance);

        // Draw vision cone
        float halfAngle = viewAngle / 2f;
        
        // Batas kiri cone
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -halfAngle) * forward * viewDistance;
        Gizmos.DrawLine(position, position + leftBoundary);
        
        // Batas kanan cone
        Vector3 rightBoundary = Quaternion.Euler(0, 0, halfAngle) * forward * viewDistance;
        Gizmos.DrawLine(position, position + rightBoundary);

        // Draw arc untuk cone (lebih smooth)
        int segments = 20;
        float angleStep = viewAngle / segments;
        Vector3 previousPoint = position + leftBoundary;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (angleStep * i);
            Vector3 direction = Quaternion.Euler(0, 0, currentAngle) * forward * viewDistance;
            Vector3 point = position + direction;
            
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        // Draw line ke player kalau terdeteksi
        if (canSeePlayer && player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, player.position);
        }
    }

    public float GetViewDistance() => viewDistance;
    public float GetViewAngle() => viewAngle;
    public Transform GetPlayer() => player;
}