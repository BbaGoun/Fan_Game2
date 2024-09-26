using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace ActionPart
{
    public class PlayerJumpAttackState : State
    {
        PlayerWithStateMachine player;

        [Header("Jump Attack Parameter")]
        [SerializeField]
        private bool attackStarted;
        [SerializeField]
        private bool attackCanceled;
        [SerializeField]
        private AttackState attackState;
        private float attackTimer;
        [SerializeField]
        private float attackGapDelay;
        [SerializeField]
        private float attackWaitTime; // 공격 간 연결 대기 시간
        [SerializeField]
        private Vector2 attackMoveVector;
        [SerializeField]
        private int attackMoveDelayFrame;
        [SerializeField]
        private int attackMoveFrame;
        private float attackMoveTimer;
        private bool canJumpAttack;
        [SerializeField]
        private bool isAttackHit;
        [SerializeField]
        private GameObject attackObject1;
        private AttackEffect attackEffect1;
        [SerializeField]
        private GameObject attackObject2;
        private AttackEffect attackEffect2;
        [SerializeField]
        private float shakeDuration;
        [SerializeField]
        private float shakeIntensity;

        private void Awake()
        {
            attackEffect1 = attackObject1.GetComponent<AttackEffect>();
            attackEffect2 = attackObject2.GetComponent<AttackEffect>();
        }

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
        }

        public override void EnterState()
        {
            PlayerInputPart.Instance.EventAttackKeyDown += AttackKeyDown;
            PlayerInputPart.Instance.EventAttackKeyUp += AttackKeyUp;
            PlayerInputPart.Instance.EventDashKeyDown += DashKeyDown;

            attackEffect1.eventAttackHit += OnAttackHit;
            attackEffect2.eventAttackHit += OnAttackHit;

            base.EnterState();
            attackState = AttackState.Attack1;
        }

        public override void ExitState()
        {
            PlayerInputPart.Instance.EventAttackKeyDown -= AttackKeyDown;
            PlayerInputPart.Instance.EventAttackKeyUp -= AttackKeyUp;
            PlayerInputPart.Instance.EventDashKeyDown -= DashKeyDown;

            attackEffect1.eventAttackHit -= OnAttackHit;
            attackEffect2.eventAttackHit -= OnAttackHit;

            attackObject1.SetActive(false);
            attackObject2.SetActive(false);
            attackState = AttackState.Idle;
            player.ResetAnimator();

            canJumpAttack = false;

            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change
            if (player.isGrounded)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
            }
            else if (attackState == AttackState.Idle)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
            }
            if (player.damageInfo.isDamaged)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
            #endregion

            attackEffect1.SetShakeDuration(shakeDuration);
            attackEffect1.SetShakeIntensity(shakeIntensity);

            attackEffect2.SetShakeDuration(shakeDuration);
            attackEffect2.SetShakeIntensity(shakeIntensity);

            ControlAttack();
            UpdateAttackState();
        }

        public override void PhysicsUpdate()
        {
            switch (attackState)
            {
                case AttackState.Attacking1:
                case AttackState.Attacking2:
                    attackMoveTimer += Time.deltaTime;
                    var delay = attackMoveDelayFrame / 60f;
                    if (attackMoveTimer > delay)
                    {
                        var duration = attackMoveFrame / 60f;
                        var timePer = (attackMoveTimer - delay) / duration;
                        timePer = Mathf.Clamp01(timePer);
                        var rate = 1 - Mathf.Pow(timePer, 3);
                        var lookDirection = 1 * Mathf.Sign(gameObject.transform.localScale.x);

                        if (isAttackHit)
                            player.velocity.x = -lookDirection * attackMoveVector.x * rate / 2f;
                        else
                            player.velocity.x = lookDirection * attackMoveVector.x * rate;
                        player.velocity.y = attackMoveVector.y * rate;
                    }
                    break;
                case AttackState.PrepareAttack2:
                    player.velocity.x = 0f;
                    player.velocity.y = Physics2D.gravity.y;
                    break;
            }
        }

        public bool CheckCanAttack()
        {
            return canJumpAttack;
        }

        public void ResetCanJumpAttack()
        {
            canJumpAttack = true;
        }

        void ControlAttack()
        {
            if (attackState == AttackState.PrepareAttack2 && attackStarted && attackTimer > attackGapDelay)
            {
                attackStarted = false;
                attackState = AttackState.Attack2;
            }
        }

        void UpdateAttackState()
        {
            switch (attackState)
            {
                case AttackState.Attack1:
                    player.SetAnimatorTrigger("isJumpAttack1");
                    attackTimer = 0f;
                    attackMoveTimer = 0f;
                    isAttackHit = false;
                    //Debug.Log("공격1 애니메이션 시작");
                    attackState = AttackState.Attacking1;
                    break;
                case AttackState.Attacking1:
                    break;
                case AttackState.PrepareAttack2:
                    attackTimer += Time.deltaTime;
                    if (attackTimer > attackWaitTime)
                    {
                        attackTimer = 0f;
                        attackState = AttackState.Idle;
                    }
                    break;

                case AttackState.Attack2:
                    player.SetAnimatorTrigger("isJumpAttack2");
                    attackTimer = 0f;
                    attackMoveTimer = 0f;
                    isAttackHit = false;
                    //Debug.Log("공격2 애니메이션 시작");
                    attackState = AttackState.Attacking2;
                    break;
                case AttackState.Attacking2:
                    break;
                case AttackState.PrepareIdle:
                    attackTimer += Time.deltaTime;
                    if (attackTimer > attackWaitTime / 2f)
                    {
                        attackTimer = 0f;
                        attackState = AttackState.Idle;
                    }
                    break;
            }
        }

        void OnAttackHit()
        {
            isAttackHit = true;
        }

        #region Animation Events
        void JumpAttack1()
        {
            attackObject1.SetActive(true);
            //Debug.Log("attackObject1 On");
        }

        void JumpAttack1Done()
        {
            attackObject1.SetActive(false);
            attackState = AttackState.PrepareAttack2;
            //Debug.Log("attackObject1 Off");
        }

        void JumpAttack2()
        {
            attackObject2.SetActive(true);
            //Debug.Log("attackObject2 On");
        }

        void JumpAttack2Done()
        {
            attackObject2.SetActive(false);
            attackState = AttackState.PrepareIdle;
            //Debug.Log("attackObject2 Off");
        }
        #endregion

        #region Key Event
        void AttackKeyDown()
        {
            attackStarted = true;
            attackCanceled = false;
        }

        void AttackKeyUp()
        {
            attackStarted = false;
            attackCanceled = true;
        }

        void DashKeyDown()
        {
            player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Dash);
        }
        #endregion

        private enum AttackState
        {
            Idle,
            Attack1,
            Attacking1,
            PrepareAttack2,
            Attack2,
            Attacking2,
            PrepareIdle,
        }
    }
}
