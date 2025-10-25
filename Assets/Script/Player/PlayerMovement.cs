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
    bool wasMoving;
    int direction = 0;
    float footstepTimer = 0f;
    float footstepInterval = 0.3f;

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
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            rb.velocity = new Vector2(moveX * speed, moveY * speed);
            isMoving = true;

            if (moveX > 0) direction = 1;
            else if (moveX < 0) direction = 3;
            else if (moveY > 0) direction = 0;
            else if (moveY < 0) direction = 2;
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

            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            if (anim != null)
                anim.SetBool("isWalking", false);

            footstepTimer = 0f;
        }

        wasMoving = isMoving;
    }

    void PlayFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFootstep();
        }
    }
}