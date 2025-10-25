using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed player bergerak")]
    public float speed;
    [Tooltip("Apakah player bisa bergerak?")]
    public bool canMove = true;

    [Space]
    [Header("References")]
    [Tooltip("Rigidbody player")]
    public Rigidbody2D rb;
    [Tooltip("Animator player")]
    public Animator anim;

    bool isMoving;
    int direction = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
            Move();

        Detail();
    }

    void Move()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
            rb.velocity = new Vector2(rb.velocity.x, Input.GetAxis("Vertical") * speed);
            isMoving = true;
            
            if(Input.GetAxis("Horizontal") > 0)
                direction = 1; // kanan
            else if (Input.GetAxis("Horizontal") < 0)
                direction = 3; // kiri
            else if (Input.GetAxis("Vertical") > 0)
                direction = 0; // atas
            else if (Input.GetAxis("Vertical") < 0)
                direction = 2; // bawah
        }
        else
        {
            isMoving = false;
            rb.velocity = Vector2.zero;
        }
    }

    void Detail()
    {
        if (isMoving)
        {
            // Animation
            if (anim != null)
            {
                anim.SetBool("isWalking", true);    
                anim.SetInteger("direction", direction);
            }
            
            // Play footstep (akan auto-handle jika masih playing)
            PlayFootstep();
        } 
        else
        {
            if(anim != null)
                anim.SetBool("isWalking", false);
            
            // Stop footstep saat berhenti
            StopFootstep();
        }
    }

    void PlayFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFootstep();
        }
    }

    void StopFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopFootstep();
        }
    }
}