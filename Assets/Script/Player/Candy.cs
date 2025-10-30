using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [Header("Candy Value")]
    [Tooltip("Nilai permen yang diberikan kepada pemain")]
    public int candyValue = 1;

    [Header("Float Animation")]
    [Tooltip("Aktifkan animasi naik-turun")]
    public bool enableFloatAnimation = true;
    
    [Tooltip("Kecepatan animasi naik-turun")]
    public float floatSpeed = 2f;
    
    [Tooltip("Jarak maksimal naik-turun (dalam unit)")]
    public float floatAmount = 0.3f;

    [Header("Rotation Animation (Optional)")]
    [Tooltip("Aktifkan rotasi candy")]
    public bool enableRotation = false;
    
    [Tooltip("Kecepatan rotasi (derajat per detik)")]
    public float rotationSpeed = 50f;

    private Vector3 startPosition;
    private float randomOffset;

    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        if (enableFloatAnimation)
        {
            float newY = startPosition.y + Mathf.Sin((Time.time * floatSpeed) + randomOffset) * floatAmount;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        if (enableRotation)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnCollected()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollectCandy();
        }

        Destroy(gameObject);
    }
}