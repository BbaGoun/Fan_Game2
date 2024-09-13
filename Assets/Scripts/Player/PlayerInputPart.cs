using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputPart : MonoBehaviour
{
    public static PlayerInputPart Instance {  get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            if (Instance != this)
                Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public delegate void DelArrowKey();
    public event DelArrowKey EventArrowKey;
    public Vector2 inputVec { get; private set; } = Vector2.zero;

    public bool jumpKeyDown {  get; private set; }
    public delegate void DelJumpKeyDown();
    public event DelJumpKeyDown EventJumpKeyDown;
    public bool jumpKeyUp {  get; private set; }
    public delegate void DelJumpKeyUp();
    public event DelJumpKeyUp EventJumpKeyUp;

    public bool dashStarted {  get; private set; }
    public delegate void DelDashKeyDown();
    public event DelDashKeyDown EventDashKeyDown;
    public bool dashCanceled {  get; private set; }
    public delegate void DelDashKeyUp();
    public event DelDashKeyUp EventDashKeyUp;

    public bool attackStarted {  get; private set; }
    public delegate void DelAttackKeyDown();
    public event DelAttackKeyDown EventAttackKeyDown;

    public bool attackHolding { get; private set; }
    public delegate void DelAttackKeyDowning();
    public event DelAttackKeyDowning EventAttackKeyHolding;
    public bool attackCanceled {  get; private set; }
    public delegate void DelAttackKeyUp();
    public event DelAttackKeyUp EventAttackKeyUp;

    public bool skillStarted {  get; private set; }
    public bool skillCanceled {  get; private set; }

    private void Update()
    {
        if (attackHolding)
            EventAttackKeyHolding?.Invoke();
    }

    public void ActionMove(InputAction.CallbackContext context)
    {
        inputVec = context.ReadValue<Vector2>();
    }

    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventJumpKeyDown?.Invoke();
        }
        else if (context.canceled)
        {
            EventJumpKeyUp?.Invoke();
        }
    }

    public void ActionDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventDashKeyDown?.Invoke();
        }
        else if (context.canceled)
        {
            EventDashKeyUp?.Invoke();
        }
    }

    public void ActionAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventAttackKeyDown?.Invoke();
            attackHolding = true;
        }
        else if (context.canceled)
        {
            attackHolding = false;
            EventAttackKeyUp?.Invoke();
        }
    }

    public void ActionSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            skillStarted = true;
        }
        else if (context.canceled)
        {
            skillStarted = false;
        }
    }
}
