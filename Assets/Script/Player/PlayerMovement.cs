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
    
    [Header("Joystick")]
    [Tooltip("Base Joystick - otomatis detect semua tipe")]
    public Joystick joystick;

    private bool isMoving;
    private int direction = 0;
    
    private Vector2 rawInput = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (joystick == null)
        {
            joystick = FindAnyJoystick();
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
        if (joystick != null && (Mathf.Abs(joystick.Horizontal) > 0.01f || Mathf.Abs(joystick.Vertical) > 0.01f))
        {
            rawInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    Joystick FindAnyJoystick()
    {
        Joystick foundJoystick = null;
        
        foundJoystick = FindObjectOfType<DynamicJoystick>();
        if (foundJoystick != null) return foundJoystick;
        
        foundJoystick = FindObjectOfType<FloatingJoystick>();
        if (foundJoystick != null) return foundJoystick;
        
        foundJoystick = FindObjectOfType<VariableJoystick>();
        if (foundJoystick != null) return foundJoystick;
        
        foundJoystick = FindObjectOfType<FixedJoystick>();
        if (foundJoystick != null) return foundJoystick;
        
        foundJoystick = FindObjectOfType<Joystick>();
        
        return foundJoystick;
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