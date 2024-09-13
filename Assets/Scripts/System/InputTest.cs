using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public Vector2 inputVec;

    public bool jumpStarted;
    public bool jumpPerformed;
    public bool jumpCanceled;

    public bool dashStarted;
    public bool dashPerformed;
    public bool dashCanceled;

    public void ActionMove(InputAction.CallbackContext context)
    {
        inputVec = context.ReadValue<Vector2>();
    }

    public void ActionJump(InputAction.CallbackContext context)
    {
        jumpStarted = context.started;
        jumpPerformed = context.performed;
        jumpCanceled = context.canceled;
    }

    public void ActionDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dashStarted = true;
            dashPerformed = false;
            dashCanceled = false;
        }
        else if (context.performed)
        {
            dashStarted = false;
            dashPerformed = true;
            dashCanceled = false;
        }
        else if (context.canceled)
        {
            dashStarted = false;
            dashPerformed = false;
            dashCanceled = true;
        }
    }
}
