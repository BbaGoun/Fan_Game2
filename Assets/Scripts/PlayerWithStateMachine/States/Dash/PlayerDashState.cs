using ActionPart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionPart
{
    public class PlayerDashState : State
    {
        PlayerWithStateMachine player;

        [Header("Dash Parameter")]
        [SerializeField]
        private DashState dashState;
        [SerializeField]
        private float dashSpeed;
        [SerializeField]
        private float dashDuration;
        [SerializeField]
        private float dashCoolTime;
        [SerializeField]
        private float invincibleDuration;
        private float dashTimer;
        private float lastDashTime;
        private Vector2 moveVec;
        private int dashCount;

        private Health health;
        private bool isBackStep;

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
            health = GetComponent<Health>();
        }

        public override void EnterState()
        {
            PlayerInputPart.Instance.EventAttackKeyDown += AttackKeyDown;

            dashState = DashState.PrepareDash;

            base.EnterState();
        }

        public override void ExitState()
        {
            PlayerInputPart.Instance.EventAttackKeyDown -= AttackKeyDown;

            lastDashTime = Time.time;
            dashTimer = 0f;
            dashState = DashState.Idle;
            player.ResetAnimator();

            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change
            if (dashState == DashState.Idle)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
            }
            if (player.damageInfo.isDamaged)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
            #endregion

            UpdateDashState();
        }

        public override void PhysicsUpdate()
        {
            switch (dashState)
            {
                case DashState.Dashing:
                    dashTimer += Time.deltaTime;
                    var timePer = dashTimer / dashDuration;
                    timePer = Mathf.Clamp01(timePer);
                    var rate = 1 - Mathf.Pow(timePer, 2);
                    var lookDirection = 1 * Mathf.Sign(gameObject.transform.localScale.x);
                    lookDirection *= moveVec.x == 0 ? -1 : 1;

                    player.velocity.x = lookDirection * dashSpeed * rate;
                    player.velocity.y = 0f;
                    break;
                default:
                    player.velocity.x = 0f;
                    player.velocity.y = Physics2D.gravity.y;
                    break;
            }
        }

        public bool CheckCanDash()
        {
            return (Time.time - lastDashTime > dashCoolTime) && (dashCount > 0);
        }

        public void SetLastDashTime()
        {
            lastDashTime = Time.time;
        }

        public void SetMoveVec()
        {
            moveVec = PlayerInputPart.Instance.inputVec;
        }

        public void ResetDashCount()
        {
            dashCount = 1;
        }

        void UpdateDashState()
        {
            switch (dashState)
            {
                case DashState.Idle:
                    break;
                case DashState.PrepareDash:
                    if (moveVec.x == 0f) // backStep
                    {
                        player.SetAnimatorTrigger("isBackStep");
                        isBackStep = true;
                    }
                    else // dash
                    {
                        if(Mathf.Sign(moveVec.x) == 1)
                            player.LookRight();
                        else if(Mathf.Sign(moveVec.x) == -1)
                            player.LookLeft();

                        player.SetAnimatorTrigger("isDash");
                        isBackStep = false;
                    }
                    dashTimer = 0f;
                    dashState = DashState.Dashing;
                    dashCount -= 1;
                    health.OnInvincible(invincibleDuration);
                    break;
                case DashState.Dashing:
                    if (dashTimer >= dashDuration)
                    {
                        player.SetAnimatorTrigger("isDashDone");
                        dashTimer = 0f;
                        dashState = DashState.Idle;
                    }
                    break;
            }
        }

        public bool CheckBackStep()
        {
            return isBackStep;
        }

        #region Key Event
        void AttackKeyDown()
        {
            if (player.isGrounded)
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.DashAttack);
        }
        #endregion

        private enum DashState
        {
            Idle,
            PrepareDash,
            Dashing,
        }
    }
}
