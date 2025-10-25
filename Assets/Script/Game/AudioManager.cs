using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource footstepSource; // Dedicated source untuk footstep

    [Header("BGM Clips")]
    public AudioClip normalBGM;
    public AudioClip tenseBGM;

    [Header("SFX Clips")]
    public AudioClip jumpScareSFX;
    public AudioClip buttonClickSFX;
    public AudioClip collectCandySFX;
    public AudioClip footstepSFX;

    [Header("Audio Cooldown (Anti-Spam)")]
    [Tooltip("Minimum jarak waktu antar jumpscare (detik)")]
    public float jumpscareCooldown = 1f;

    // Track last play time
    private float lastJumpscareTime = -999f;

    private const string BGM_MUTE_KEY = "BGM_MUTE";
    private const string SFX_MUTE_KEY = "SFX_MUTE";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup footstep source jika belum ada
        if (footstepSource == null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.playOnAwake = false;
        }
    }

    void Start()
    {
        // Load mute settings
        if (bgmSource != null)
            bgmSource.mute = PlayerPrefs.GetInt(BGM_MUTE_KEY, 0) == 1;

        if (sfxSource != null)
            sfxSource.mute = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;

        if (footstepSource != null)
            footstepSource.mute = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;

        // Play default BGM
        if (bgmSource != null && normalBGM != null && bgmSource.clip != normalBGM)
        {
            bgmSource.clip = normalBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // === SWITCH MUSIC ===
    public void PlayNormalBGM()
    {
        if (bgmSource == null || normalBGM == null) return;
        if (bgmSource.clip == normalBGM) return;

        bgmSource.Stop();
        bgmSource.clip = normalBGM;
        bgmSource.Play();
    }

    public void PlayTenseBGM()
    {
        if (bgmSource == null || tenseBGM == null) return;
        if (bgmSource.clip == tenseBGM) return;

        bgmSource.Stop();
        bgmSource.clip = tenseBGM;
        bgmSource.Play();
    }

    // === SFX ===
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayJumpScare()
    {
        // Anti-spam: cek cooldown
        if (Time.time - lastJumpscareTime < jumpscareCooldown)
            return;

        PlaySFX(jumpScareSFX);
        lastJumpscareTime = Time.time;
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayCollectCandy()
    {
        PlaySFX(collectCandySFX);
    }

    public void PlayFootstep()
    {
        // Cek apakah footstep masih playing
        if (footstepSource != null && footstepSFX != null)
        {
            // Hanya play jika audio sebelumnya sudah selesai
            if (!footstepSource.isPlaying)
            {
                footstepSource.clip = footstepSFX;
                footstepSource.Play();
            }
        }
    }

    public void StopFootstep()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }

    // === TOGGLE AUDIO ===
    public void ToggleBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !bgmSource.mute;
            PlayerPrefs.SetInt(BGM_MUTE_KEY, bgmSource.mute ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void ToggleSFX()
    {
        bool newMuteState = false;
        
        if (sfxSource != null)
        {
            sfxSource.mute = !sfxSource.mute;
            newMuteState = sfxSource.mute;
        }

        // Apply ke footstep source juga
        if (footstepSource != null)
        {
            footstepSource.mute = newMuteState;
        }

        PlayerPrefs.SetInt(SFX_MUTE_KEY, newMuteState ? 1 : 0);
        PlayerPrefs.Save();
    }
}