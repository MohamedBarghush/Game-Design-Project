using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool sprintInput;
    public bool crouchInput;
    public bool jumpInput;
    public bool auraInput;
    public bool targetInput;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        sprintInput = value.isPressed;
    }

    public void OnCrouch(InputValue value)
    {
        crouchInput = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }

    public void OnAura(InputValue value)
    {
        auraInput = value.isPressed;
    }

    public void OnTarget(InputValue value)
    {
        targetInput = value.isPressed;
    }

    public void Update()
    {
        if (moveInput == Vector2.zero)
        {
            sprintInput = false;
        }
    }
}
