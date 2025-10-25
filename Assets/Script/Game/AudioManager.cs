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

    [Header("BGM Clips")]
    public AudioClip normalBGM;
    public AudioClip tenseBGM;

    [Header("SFX Clips")]
    public AudioClip jumpScareSFX;
    public AudioClip buttonClickSFX;
    public AudioClip collectCandySFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (bgmSource != null)
            bgmSource.mute = PlayerPrefs.GetInt(BGM_MUTE_KEY, 0) == 1;

        if (sfxSource != null)
            sfxSource.mute = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;

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
        PlaySFX(jumpScareSFX);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    private const string BGM_MUTE_KEY = "BGM_MUTE";
    private const string SFX_MUTE_KEY = "SFX_MUTE";

    // Toggle BGM
    public void ToggleBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !bgmSource.mute;
            PlayerPrefs.SetInt(BGM_MUTE_KEY, bgmSource.mute ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // Toggle SFX
    public void ToggleSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.mute = !sfxSource.mute;
            PlayerPrefs.SetInt(SFX_MUTE_KEY, sfxSource.mute ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // collectCandySFX
    public void PlayCollectCandy()
    {
        PlaySFX(collectCandySFX);
    }
}