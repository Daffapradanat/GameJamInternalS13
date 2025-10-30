using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoystick : MonoBehaviour
{
    [Header("Joystick Reference")]
    [Tooltip("Base Joystick - otomatis detect semua tipe")]
    public Joystick joystick;

    [Header("Settings")]
    [Tooltip("Aktifkan joystick input?")]
    public bool enableJoystick = true;

    private Vector2 joystickInput = Vector2.zero;

    private void Awake()
    {
        if (joystick == null)
        {
            joystick = FindAnyJoystick();
        }
    }

    private void Update()
    {
        if (enableJoystick && joystick != null)
        {
            UpdateJoystickInput();
        }
        else
        {
            joystickInput = Vector2.zero;
        }
    }

    void UpdateJoystickInput()
    {
        if (Mathf.Abs(joystick.Horizontal) > 0.01f || Mathf.Abs(joystick.Vertical) > 0.01f)
        {
            joystickInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            joystickInput = Vector2.zero;
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

    public Vector2 GetJoystickInput()
    {
        return joystickInput;
    }

    public void EnableJoystick(bool state)
    {
        enableJoystick = state;
    }
}