using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed player bergerak")]
    public float speed = 5f;
    [Tooltip("Apakah player bisa bergerak?")]
    public bool canMove = true;

    [Header("References")]
    [Tooltip("Rigidbody player")]
    public Rigidbody2D rb;
    [Tooltip("Animator player")]
    public Animator anim;
    [Tooltip("Player Joystick Component")]
    public PlayerJoystick playerJoystick;

    private bool isMoving;
    private int direction = 0;
    
    private Vector2 rawInput = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (playerJoystick == null)
        {
            playerJoystick = GetComponent<PlayerJoystick>();
        }
    }

    void Update()
    {
        if (canMove)
        {
            GetInput();
            Move();
        }
        
        UpdateAnimation();
    }

    void GetInput()
    {
        Vector2 joystickInput = Vector2.zero;
        if (playerJoystick != null)
        {
            joystickInput = playerJoystick.GetJoystickInput();
        }

        if (joystickInput.magnitude > 0.01f)
        {
            rawInput = joystickInput;
        }
        else
        {
            rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    public Vector2 GetProcessedInput()
    {
        return rawInput;
    }

    public void SetCustomInput(Vector2 input)
    {
        rawInput = input;
    }

    void Move()
    {
        Vector2 processedInput = rawInput;
        
        if (processedInput.magnitude > 0.1f)
        {
            rb.velocity = new Vector2(processedInput.x * speed, processedInput.y * speed);
            isMoving = true;
            
            UpdateDirection(processedInput);
        }
        else
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
        }
    }

    void UpdateDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            direction = (input.x > 0) ? 1 : 3;
        }
        else
        {
            direction = (input.y > 0) ? 0 : 2;
        }
    }

    void UpdateAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);
            if (isMoving)
            {
                anim.SetInteger("direction", direction);
            }
        }
        
        if (isMoving)
            PlayFootstep();
        else
            StopFootstep();
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