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
    
    // ===== TAMBAHAN UNTUK CONTROL LIE =====
    [HideInInspector]
    public Vector2 inputModifier = Vector2.one; // Multiplier untuk input (untuk invert, dll)
    [HideInInspector]
    public bool useCustomInput = false; // Flag apakah pakai custom input
    [HideInInspector]
    public Vector2 customInput = Vector2.zero; // Custom input dari ControlLie

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
        float horizontal;
        float vertical;
        
        // ===== CEK APAKAH ADA CUSTOM INPUT =====
        if (useCustomInput)
        {
            horizontal = customInput.x;
            vertical = customInput.y;
        }
        else
        {
            horizontal = Input.GetAxis("Horizontal") * inputModifier.x;
            vertical = Input.GetAxis("Vertical") * inputModifier.y;
        }
        
        if (horizontal != 0 || vertical != 0)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
            isMoving = true;
            
            if(horizontal > 0)
                direction = 1;
            else if (horizontal < 0)
                direction = 3;
            else if (vertical > 0)
                direction = 0;
            else if (vertical < 0)
                direction = 2;
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