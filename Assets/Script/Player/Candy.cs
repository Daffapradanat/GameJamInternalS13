using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [Tooltip("Nilai permen yang diberikan kepada pemain")]
    public int candyValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerData>().AddCandy(candyValue);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCollectCandy();
            }
            
            Destroy(gameObject);
        }
    }
}