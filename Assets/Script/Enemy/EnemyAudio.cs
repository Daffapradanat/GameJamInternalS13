using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource ambientSource;  // Buat idle/patrol sound
    public AudioSource chaseSource;    // Buat chase sound

    [Header("Audio Clips")]
    public AudioClip idleSound;        // Suara saat idle/patrol
    public AudioClip chaseSound;       // Suara saat ngejar
    public AudioClip detectedSound;    // SFX saat pertama detect player
    public AudioClip caughtSound;      // SFX saat player ketangkep

    private bool isChasing = false;

    void Start()
    {
        if (ambientSource != null && idleSound != null)
        {
            ambientSource.clip = idleSound;
            ambientSource.loop = true;
            ambientSource.Play();
        }

        if (chaseSource != null && chaseSound != null)
        {
            chaseSource.clip = chaseSound;
            chaseSource.loop = true;
        }
    }

    // Dipanggil pas enemy mulai ngejar
    public void StartChaseAudio()
    {
        if (isChasing) return;
        isChasing = true;

        // Play detected sound (jumpscare)
        if (detectedSound != null)
            AudioManager.Instance?.PlaySFX(detectedSound);

        // Switch ke tense BGM
        AudioManager.Instance?.PlayTenseBGM();

        // Stop idle sound
        if (ambientSource != null)
            ambientSource.Stop();

        // Play chase sound
        if (chaseSource != null && !chaseSource.isPlaying)
            chaseSource.Play();
    }

    // Dipanggil pas enemy berhenti ngejar
    public void StopChaseAudio()
    {
        if (!isChasing) return;
        isChasing = false;

        AudioManager.Instance?.PlayNormalBGM();

        if (chaseSource != null)
            chaseSource.Stop();

        if (ambientSource != null && idleSound != null)
        {
            ambientSource.clip = idleSound;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    public void PlayCaughtSound()
    {
        if (caughtSound != null)
            AudioManager.Instance?.PlaySFX(caughtSound);
        
        if (ambientSource != null)
            ambientSource.Stop();
        if (chaseSource != null)
            chaseSource.Stop();
    }

    void OnDestroy()
    {
        // Clean up saat enemy mati
        StopChaseAudio();
    }
}