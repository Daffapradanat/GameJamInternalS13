using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button bgmButton;
    public Button sfxButton;

    [Header("Icons")]
    public Sprite iconOn;
    public Sprite iconOff;

    private AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.Instance;

        UpdateIcons();

        bgmButton.onClick.AddListener(() =>
        {
            audioManager.ToggleBGM();
            UpdateIcons();
        });

        sfxButton.onClick.AddListener(() =>
        {
            audioManager.ToggleSFX();
            UpdateIcons();
        });
    }

    void UpdateIcons()
    {
        bgmButton.image.sprite = audioManager.bgmSource.mute ? iconOff : iconOn;
        sfxButton.image.sprite = audioManager.sfxSource.mute ? iconOff : iconOn;
    }
}

