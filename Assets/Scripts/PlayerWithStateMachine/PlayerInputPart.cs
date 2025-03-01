using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionPart
{
    public class PlayerInputPart : MonoBehaviour
    {
        public static PlayerInputPart Instance { get; private set; }

        public void Initialize()
        {
            if (Instance != null)
            {
                if (Instance != this)
                    Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            isCanInput = true;
        }

        public bool isCanInput;

        public delegate void DelArrowKey();
        public event DelArrowKey EventArrowKey;
        public Vector2 inputVec { get; private set; }

        public delegate void DelJumpKeyDown();
        public event DelJumpKeyDown EventJumpKeyDown;
        public delegate void DelJumpKeyUp();
        public event DelJumpKeyUp EventJumpKeyUp;

        public delegate void DelDashKeyDown();
        public event DelDashKeyDown EventDashKeyDown;
        public delegate void DelDashKeyUp();
        public event DelDashKeyUp EventDashKeyUp;

        public delegate void DelAttackKeyDown();
        public event DelAttackKeyDown EventAttackKeyDown;
        public bool attackHolding { get; private set; }
        public delegate void DelAttackKeyHolding();
        public event DelAttackKeyHolding EventAttackKeyHolding;
        public delegate void DelAttackKeyUp();
        public event DelAttackKeyUp EventAttackKeyUp;

        public delegate void DelGuardKeyDown();
        public event DelGuardKeyDown EventGuardKeyDown;
        public delegate void DelGuardKeyUp();
        public event DelGuardKeyUp EventGuardKeyUp;

        public delegate void DelHealKeyDown();
        public event DelHealKeyDown EventHealKeyDown;
        public delegate void DelHealKeyUp();
        public event DelHealKeyUp EventHealKeyUp;

        public delegate void DelTalkKeyDown();
        public event DelTalkKeyDown EventTalkKeyDown;
        public delegate void DelTalkKeyUp();
        public event DelTalkKeyUp EventTalkKeyUp;

        public delegate void DelKeyUpConfirm();
        public event DelKeyUpConfirm EventKeyUpConfirm;

        private void Update()
        {
            if (attackHolding)
                EventAttackKeyHolding?.Invoke();
        }

        public void CanInput()
        {
            isCanInput = true;
        }

        public void CantInput()
        {
            isCanInput = false;
        }

        public void ActionMove(InputAction.CallbackContext context)
        {
            //이걸 멈추면 오히려 계속 앞으로 나아가네
            /*if (Time.timeScale == 0 || !isCanInput)
                return;
            */
            inputVec = context.ReadValue<Vector2>();
        }

        public void ActionJump(InputAction.CallbackContext context)
        {
            if (Time.timeScale == 0 || !isCanInput)
            {
                return;
            }

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
            if (Time.timeScale == 0 || !isCanInput)
            {
                return;
            }

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
            if (Time.timeScale == 0 || !isCanInput)
            {
                attackHolding = false;
                return;
            }

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

        public void ActionGuard(InputAction.CallbackContext context)
        {
            if (Time.timeScale == 0 || !isCanInput)
            {
                return;
            }

            if (context.started)
            {
                EventGuardKeyDown?.Invoke();
            }
            else if (context.canceled)
            {
                EventGuardKeyUp?.Invoke();
            }
        }

        public void ActionHeal(InputAction.CallbackContext context)
        {
            if (Time.timeScale == 0 || !isCanInput)
            {
                return;
            }

            if (context.started)
            {
                EventHealKeyDown?.Invoke();
            }
            else if (context.canceled)
            {
                EventHealKeyUp?.Invoke();
            }
        }

        public void ActionTalk(InputAction.CallbackContext context)
        {
            if (Time.timeScale == 0 || !isCanInput)
            {
                return;
            }

            if (context.started)
            {
                EventTalkKeyDown?.Invoke();
            }
            else if (context.canceled)
            {
                EventTalkKeyUp?.Invoke();
            }
        }

        public void KeyUpConfirm(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                EventKeyUpConfirm?.Invoke();
            }
        }
    }
}