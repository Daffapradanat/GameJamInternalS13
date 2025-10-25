using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("UI teks timer")]
    public TextMeshProUGUI _TimerUI;
    [Tooltip("UI teks permen")]
    public TextMeshProUGUI _CandyUI;

    [Header("Data Pemain")]
    [Tooltip("Waktu hitung mundur dalam detik")]
    public float timer = 60;
    [Tooltip("Jumlah permen yang dikumpulkan")]
    public float candy;
    [Tooltip("Jumlah permen yang dibutuhkan untuk menang")]
    [SerializeField] private float goal = 5;

    void Update()
    {
        DisplayTimer();
        DisplayCandyCount();

        if (!GameManager.Instance.isGameOver)
        {
            Timer();
        }
    }

    // Timer waktu hitung mundur
    void Timer()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
            GameManager.Instance.GameOver();
    }

    // fungsi tambah permen
    public void AddCandy(int value)
    {
        candy += value;
        if(candy >= goal)
        {
            // Menang
        }
    }

    void DisplayTimer()
    {
        if (_TimerUI != null)
        {
            _TimerUI.text = "Time: " + Mathf.Ceil(timer).ToString();
        }
    }

    void DisplayCandyCount()
    {
        if (_CandyUI != null)
        {
            _CandyUI.text = $"Candy: {candy}/{goal}";
        }
    }
}
