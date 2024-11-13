using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public Vector2 moveVec;

        [Header("Jump Parameter")]
        [SerializeField]
        private float jumpForce; // ������
        [SerializeField]
        private float doubleJumpForce; // ���� ������
        [SerializeField]
        private int remainJump; // ���� ���� ���� Ƚ��
        [SerializeField]
        private int maxJumpCount; // �ִ� ���� Ƚ��
        [SerializeField]
        private float minJumpDuration; // �ּ� ���� ���� �ð�
        private float jumpTimer; // ���� ���� ���� �ð�
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

        Coroutine moveCoroutine;
        bool isCoroutineDone;

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

            // ���߷��� �ϴ� ���
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
            if (!PlayerInputPart.Instance.isCanInput || Time.timeScale == 0f)
                moveVec = Vector2.zero;
            
            var isLookRight = Mathf.Sign(player.transform.localScale.x) == 1;

            if (moveVec.x < -0.01f && isLookRight)
            {
                //�������� ����
                //Debug.Log("�������� ����");
                if (player.isGrounded)
                    player.SetAnimatorTrigger("isTurn");
                player.SetAnimatorBool("isMove", false);
                Flip(!isLookRight);
            }
            else if (moveVec.x > 0.01f && !isLookRight)
            {
                //���������� ����
                //Debug.Log("���������� ����");
                if (player.isGrounded)
                    player.SetAnimatorTrigger("isTurn");
                player.SetAnimatorBool("isMove", false);
                Flip(!isLookRight);
            }
            else if (Mathf.Abs(moveVec.x) > 0.01f)
            {
                // ���� ���� �״��
                player.SetAnimatorBool("isMove", true);
            }
            else
            {
                // ������ ������
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
                //Debug.Log("�������� �����Ѵ�");
                jumpState = JumpState.Falling;
            }
            else if (jumpState == JumpState.Grounded && jumpStarted && remainJump > 0)
            {
                //Debug.Log("�츰 ���� �غ�� ����");
                jumpState = JumpState.PrepareToJump;
            }
            else if (jumpState == JumpState.Falling && jumpStarted && remainJump > 0)
            {
                //Debug.Log("�츰 ���� ���� �غ�� ����");
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

                    //Debug.Log("���� �غ�");
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
                    // ���� ���� �ൿ �߰� (����, ����Ʈ)
                    player.SetAnimatorTrigger("isJump");
                    //Debug.Log("���� �غ�!");
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
                    // ���� ���� ���� �ൿ �߰�
                    player.SetAnimatorTrigger("isDoubleJump");
                    player.delegateJump?.Invoke();
                    //Debug.Log("���� ���� �غ�!");
                    jumpState = JumpState.Jumping;
                    break;

                case JumpState.Landed:
                    jumpStarted = false;
                    jumpPerformed = false;
                    jumpCanceled = false;
                    // ���� ���� �ൿ �߰�
                    jumpState = JumpState.Grounded;
                    break;

                case JumpState.Grounded:
                    remainJump = maxJumpCount;
                    player.velocity.y = Physics2D.gravity.y;
                    break;
            }
        }

        public void MoveXFromTo(Transform from, Transform to)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            StartCoroutine(IEMoveXFromTo(from, to));
            
            IEnumerator IEMoveXFromTo(Transform from, Transform to)
            {
                isCoroutineDone = false;
                PlayerInputPart.Instance.CantInput();
                player.isStopped = true;

                this.transform.localPosition = from.localPosition;
                var direction = Mathf.Sign(to.localPosition.x - from.localPosition.x);
                if (direction >= 0)
                    player.LookRight();
                else
                    player.LookLeft();
                player.SetAnimatorBool("isMove", true);

                var moveGap = direction * moveSpeed / 2 * Time.fixedDeltaTime;
                var moveCount = (to.localPosition.x - from.localPosition.x) / moveGap;

                for(int i = 0; i < moveCount; i++)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x + moveGap, this.transform.localPosition.y, this.transform.localPosition.z);
                    yield return new WaitForFixedUpdate();
                }

                player.SetAnimatorBool("isMove", false);
                PlayerInputPart.Instance.CanInput();
                player.isStopped = false;

                isCoroutineDone = true;
            }
        }

        public bool IsCoroutineDone()
        {
            if (isCoroutineDone)
            {
                isCoroutineDone = false;
                return true;
            }
            else
                return false;
        }

        #region Key Event
        void JumpKeyDown()
        {
            //Debug.Log("�����ٴϱ�");
            jumpStarted = true;
            jumpCanceled = false;
        }

        void JumpKeyUp()
        {
            //Debug.Log("���ٴϱ�");
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
        {// �Է��� ��ٸ��� ���̿� ������, ������ ������ ���� �Լ�
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