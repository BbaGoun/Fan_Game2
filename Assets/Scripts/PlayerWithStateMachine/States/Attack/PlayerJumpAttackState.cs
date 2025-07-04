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
        private float shakeDuration;
        [SerializeField]
        private float shakeIntensity;

        private void Awake()
        {
            attackEffect1 = attackObject1.GetComponent<AttackEffect>();
        }

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
        }

        public override void EnterState()
        {

            attackEffect1.eventAttackHit += OnAttackHit;

            base.EnterState();
            attackState = AttackState.Attack1;
        }

        public override void ExitState()
        {

            attackEffect1.eventAttackHit -= OnAttackHit;

            attackObject1.SetActive(false);
            attackState = AttackState.Idle;

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

            UpdateAttackState();
        }

        public override void PhysicsUpdate()
        {
            switch (attackState)
            {
                case AttackState.Attacking1:
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

        void UpdateAttackState()
        {
            switch (attackState)
            {
                case AttackState.Attack1:
                    player.SetAnimatorTrigger("isJumpAttack1");
                    attackMoveTimer = 0f;
                    isAttackHit = false;
                    attackState = AttackState.Attacking1;
                    break;
                case AttackState.Attacking1:
                    break;
                case AttackState.PrepareIdle:
                    attackState = AttackState.Idle;
                    break;
            }
        }

        void OnAttackHit()
        {
            canJumpAttack = true;
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
            attackState = AttackState.PrepareIdle;
            //Debug.Log("attackObject1 Off");
        }

        #endregion

        private enum AttackState
        {
            Idle,
            Attack1,
            Attacking1,
            PrepareIdle,
        }
    }
}
