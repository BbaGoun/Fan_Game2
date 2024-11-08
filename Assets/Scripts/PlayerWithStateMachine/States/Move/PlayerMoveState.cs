using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using static Cinemachine.CinemachineOrbitalTransposer;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ActionPart
{
    public class PlayerMoveState : State
    {
        PlayerWithStateMachine player;

        #region parameters
        [Header("Speed Parameter")]
        [SerializeField]
        private float moveSpeed;
        [SerializeField]
        private float acceleration;
        [SerializeField]
        private float deceleration;
        [SerializeField]
        private float velPower;
        [SerializeField]
        private float frictionAmount;
        Vector2 moveVec;

        [Header("Jump Parameter")]
        [SerializeField]
        private float jumpForce; // 점프력
        [SerializeField]
        private float doubleJumpForce; // 더블 점프력
        private int remainJump; // 현재 남은 점프 횟수
        [SerializeField]
        private int maxJumpCount; // 최대 점프 횟수
        [SerializeField]
        private float minJumpDuration; // 최소 점프 지속 시간
        private float jumpTimer; // 현재 점프 지속 시간
        [SerializeField, Range(-100, -9.81f)]
        private float maxFallSpeed;
        [SerializeField]
        private float gravityModifier;
        [SerializeField]
        private float jumpHoldGravity;
        [SerializeField]
        private float jumpCancelGravity;
        [SerializeField]
        private JumpState jumpState;
        [SerializeField]
        private bool jumpStarted;
        [SerializeField]
        private bool jumpPerformed;
        [SerializeField]
        private bool jumpCanceled;
        #endregion

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
        }

        public override void EnterState()
        {
            PlayerInputPart.Instance.EventJumpKeyDown += JumpKeyDown;
            PlayerInputPart.Instance.EventJumpKeyUp += JumpKeyUp;
            PlayerInputPart.Instance.EventAttackKeyDown += AttackKeyDown;
            PlayerInputPart.Instance.EventDashKeyDown += DashKeyDown;
            PlayerInputPart.Instance.EventHealKeyDown += HealKeyDown;

            //Debug.Log("Enter Player Move State");
        }

        public override void ExitState()
        {
            PlayerInputPart.Instance.EventJumpKeyDown -= JumpKeyDown;
            PlayerInputPart.Instance.EventJumpKeyUp -= JumpKeyUp;
            PlayerInputPart.Instance.EventAttackKeyDown -= AttackKeyDown;
            PlayerInputPart.Instance.EventDashKeyDown -= DashKeyDown;
            PlayerInputPart.Instance.EventHealKeyDown -= HealKeyDown;


            player.SetAnimatorBool("isMove", false);
            //Debug.Log("Exit Player Move State");
        }

        public override void FrameUpdate()
        {
            #region Change State
            if (player.isGrounded && player.isGuard)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Guard);
            }
            if (player.isCharged)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.ChargeAttack);
            }
            if (player.damageInfo.isDamaged)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
            #endregion

            player.SetAnimatorBool("isGrounded", player.isGrounded);

            XControl();

            YControl();
            YStateUpdate();
        }

        public override void PhysicsUpdate()
        {
            float targetSpeed = moveVec.x * moveSpeed;

            float speedDif = targetSpeed - player.velocity.x;

            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

            player.velocity.x += movement * Time.deltaTime;

            // 멈추려고 하는 경우
            if (player.isGrounded && Mathf.Abs(moveVec.x) < 0.01f)
            {
                float amount = Mathf.Min(Mathf.Abs(player.velocity.x), Mathf.Abs(frictionAmount));

                amount *= Mathf.Sign(player.velocity.x);

                player.velocity.x += -amount * Time.deltaTime;
            }

            switch (jumpState)
            {
                case JumpState.Falling:
                    player.velocity.y = Mathf.Max(player.velocity.y + gravityModifier * Physics2D.gravity.y * Time.deltaTime, maxFallSpeed);
                    break;
                case JumpState.Jumping:
                    if (jumpPerformed)
                        player.velocity.y += jumpHoldGravity * Physics2D.gravity.y * Time.deltaTime;
                    else
                        player.velocity.y += jumpCancelGravity * Physics2D.gravity.y * Time.deltaTime;
                    break;
                default:
                    break;
            }

            player.SetAnimatorFloat("velocityY", player.velocity.y);
        }


        void XControl()
        {
            moveVec = PlayerInputPart.Instance.inputVec;
            var isLookRight = Mathf.Sign(player.transform.localScale.x) == 1;

            if (moveVec.x < -0.01f && isLookRight)
            {
                //왼쪽으로 돌기
                //Debug.Log("왼쪽으로 돌기");
                if (player.isGrounded)
                    player.SetAnimatorTrigger("isTurn");
                player.SetAnimatorBool("isMove", false);
                Flip(!isLookRight);
            }
            else if (moveVec.x > 0.01f && !isLookRight)
            {
                //오른쪽으로 돌기
                //Debug.Log("오른쪽으로 돌기");
                if (player.isGrounded)
                    player.SetAnimatorTrigger("isTurn");
                player.SetAnimatorBool("isMove", false);
                Flip(!isLookRight);
            }
            else if (Mathf.Abs(moveVec.x) > 0.01f)
            {
                // 가는 방향 그대로
                player.SetAnimatorBool("isMove", true);
            }
            else
            {
                // 가만히 서있음
                player.SetAnimatorBool("isMove", false);
            }
        }

        void Flip(bool isLookRight)
        {
            if (isLookRight)
            {
                player.LookRight();
            }
            else
            {
                player.LookLeft();
            }
        }

        void YControl()
        {
            if (jumpState == JumpState.Grounded && !player.isGrounded)
            {
                //Debug.Log("떨어지기 시작한다");
                jumpState = JumpState.Falling;
            }
            else if (jumpState == JumpState.Grounded && jumpStarted && remainJump > 0)
            {
                //Debug.Log("우린 점프 준비로 간다");
                jumpState = JumpState.PrepareToJump;
            }
            else if (jumpState == JumpState.Falling && jumpStarted && remainJump > 0)
            {
                //Debug.Log("우린 더블 점프 준비로 간다");
                jumpState = JumpState.PrepareToDoubleJump;
            }
        }

        void YStateUpdate()
        {
            switch (jumpState)
            {
                case JumpState.Falling:
                    jumpCanceled = false;
                    remainJump = Mathf.Min(remainJump, maxJumpCount - 1);

                    //Debug.Log("낙하 준비");
                    if (player.isGrounded)
                        jumpState = JumpState.Landed;
                    break;

                case JumpState.PrepareToJump:
                    jumpStarted = false;
                    jumpPerformed = true;
                    jumpCanceled = false;
                    remainJump--;
                    player.velocity.y = jumpForce;
                    jumpTimer = 0f;
                    // 점프 관련 행동 추가 (사운드, 이펙트)
                    player.SetAnimatorTrigger("isJump");
                    //Debug.Log("점프 준비!");
                    jumpState = JumpState.Jumping;
                    break;

                case JumpState.Jumping:
                    if (jumpCanceled && jumpTimer >= minJumpDuration)
                    {
                        jumpPerformed = false;
                        jumpCanceled = false;
                    }
                    else if (player.isHeading)
                    {
                        player.velocity.y = Mathf.Min(player.velocity.y, 0f);
                    }

                    jumpTimer += Time.deltaTime;

                    if (player.velocity.y < 0f)
                        jumpState = JumpState.Falling;
                    break;

                case JumpState.PrepareToDoubleJump:
                    jumpStarted = false;
                    jumpPerformed = true;
                    jumpCanceled = false;
                    remainJump--;
                    player.velocity.y = doubleJumpForce;
                    jumpTimer = 0f;
                    // 더블 점프 관련 행동 추가
                    player.SetAnimatorTrigger("isDoubleJump");
                    player.delegateJump?.Invoke();
                    //Debug.Log("더블 점프 준비!");
                    jumpState = JumpState.Jumping;
                    break;

                case JumpState.Landed:
                    jumpStarted = false;
                    jumpPerformed = false;
                    jumpCanceled = false;
                    // 착지 관련 행동 추가
                    remainJump = maxJumpCount;
                    jumpState = JumpState.Grounded;
                    break;

                case JumpState.Grounded:
                    player.velocity.y = Physics2D.gravity.y;
                    break;
            }
        }

        #region Key Event
        void JumpKeyDown()
        {
            //Debug.Log("눌렀다니까");
            jumpStarted = true;
            jumpCanceled = false;
        }

        void JumpKeyUp()
        {
            //Debug.Log("땠다니까");
            jumpStarted = false;
            jumpCanceled = true;
        }

        void AttackKeyDown()
        {
            if (player.isGrounded)
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Attack);
            else if (!player.isGrounded)
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.JumpAttack);
        }

        void DashKeyDown()
        {
            player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Dash);
        }

        void HealKeyDown()
        {
            if (player.isGrounded)
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Heal);
        }
        #endregion

        /*public void JumpTrigger()
        {// 입력을 기다리는 사이에 끼어들어, 반응을 높히기 위한 함수
            StartCoroutine(IEJumpTrigger());
        }

        IEnumerator IEJumpTrigger()
        {
            player.isJumped = true;
            yield return new WaitForSeconds(Time.deltaTime);
            player.isJumped = false;
        }*/

        enum JumpState
        {
            Falling,
            Landed,
            Grounded,
            PrepareToJump,
            PrepareToDoubleJump,
            Jumping
        }
    }
}