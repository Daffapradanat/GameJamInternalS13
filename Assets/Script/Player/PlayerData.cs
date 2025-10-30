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

    [Header("Tag Settings")]
    [Tooltip("Tag objek yang bisa diambil oleh player")]
    public string candyTag = "Candy";

    void Update()
    {
        DisplayTimer();
        DisplayCandyCount();

        if (!GameManager.Instance.isGameOver)
        {
            Timer();
        }
    }

    void Timer()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
            GameManager.Instance.GameOver();
    }

    public void AddCandy(int value)
    {
        candy += value;
        
        if(candy >= goal)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameWin();
        }
    }

    void DisplayTimer()
    {
        if (_TimerUI != null)
        {
            _TimerUI.text = Mathf.Ceil(timer).ToString();
        }
    }

    void DisplayCandyCount()
    {
        if (_CandyUI != null)
        {
            _CandyUI.text = $"{candy}/{goal}";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(candyTag))
        {
            Candy candyComponent = other.GetComponent<Candy>();
            
            if (candyComponent == null)
            {
                candyComponent = other.GetComponentInChildren<Candy>();
            }
            
            if (candyComponent == null)
            {
                candyComponent = other.GetComponentInParent<Candy>();
            }
            
            if (candyComponent != null)
            {
                int value = candyComponent.candyValue;
                AddCandy(value);
                candyComponent.OnCollected();
            }
            else
            {
                AddCandy(1);
                
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCollectCandy();
                }
                
                Destroy(other.transform.root.gameObject);
            }
        }
    }
}