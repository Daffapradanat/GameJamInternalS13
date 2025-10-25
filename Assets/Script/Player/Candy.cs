using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [Tooltip("Nilai permen yang diberikan kepada pemain")]
    public int candyValue = 1;
    [Tooltip("Suara efek pengambilan permen")]
    public AudioClip sfx;

    [Tooltip("AudioSource untuk memutar suara")]
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerData>().AddCandy(candyValue);
            if (audioSource != null && sfx != null)
                Sfx();
            Destroy(gameObject);
        }
    }

    void Sfx()
    {
        if(audioSource.clip == null)
            audioSource.clip = sfx;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }
}
