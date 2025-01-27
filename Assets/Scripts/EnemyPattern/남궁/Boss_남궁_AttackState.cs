using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Boss_남궁_AttackState : State
    {
        Boss_남궁 boss;

        [SerializeField]
        AudioClip attackAudio;

        [Header("Attack Parameter")]
        [SerializeField]
        private AttackState attackState;
        private float attackTimer;
        [SerializeField]
        private float attackWaitTime;

        [SerializeField]
        private float attackMoveSpeed;
        [SerializeField]
        private int attackMoveDelayFrame;
        [SerializeField]
        private int attackMoveFrame;
        private float attackMoveTimer;

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

        public void Initialize(Boss_남궁 _boss)
        {
            boss = _boss;
        }

        public override void EnterState()
        {
            attackEffect1.eventAttackHit += OnAttackHit;

            base.EnterState();

            boss.UpAttackCount();
            attackState = AttackState.Attack1;
        }

        public override void ExitState()
        {
            attackEffect1.eventAttackHit -= OnAttackHit;

            attackObject1.SetActive(false);
            attackState = AttackState.Idle;
            boss.ResetAnimator();

            isAttackHit = false;

            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change
            if (attackState == AttackState.Idle)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Move);
            }
            if (false)
            {
                // 보스가 쓰러짐
            }
            if (boss.isGroggy)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Groggy);
            }
            if (boss.isDeath)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Death);
            }
            #endregion

            attackEffect1.SetShakeDuration(shakeDuration);
            attackEffect1.SetShakeIntensity(shakeIntensity);

            UpdateAttackState();
        }

        void UpdateAttackState()
        {
            switch (attackState)
            {
                case AttackState.Attack1:
                    boss.SetAnimatorTrigger("isAttack");
                    attackTimer = 0f;
                    attackMoveTimer = 0f;
                    isAttackHit = false;
                    //Debug.Log("공격1 애니메이션 시작");
                    attackState = AttackState.Attacking1;
                    break;
                case AttackState.Attacking1:
                    break;
                case AttackState.PrepareIdle:
                    attackTimer += Time.deltaTime;
                    if (attackTimer > attackWaitTime)
                    {
                        attackTimer = 0f;
                        attackState = AttackState.Idle;
                    }
                    break;
            }
        }

        public override void PhysicsUpdate()
        {
            switch (attackState)
            {
                case AttackState.Attacking1:
                    attackMoveTimer += Time.deltaTime;
                    float delay = attackMoveDelayFrame / 60f;
                    if (attackMoveTimer > delay)
                    {
                        float duration = attackMoveFrame / 60f;
                        float timePer = (attackMoveTimer - delay) / duration;
                        timePer = Mathf.Clamp01(timePer);
                        float rate = 1 - Mathf.Pow(timePer, 3);
                        float lookDirection = 1 * Mathf.Sign(boss.GetDirection());

                        boss.velocity.x = lookDirection * attackMoveSpeed * rate;
                        boss.velocity.y = 0f;
                    }
                    break;
            }
        }
        

        void OnAttackHit()
        {
            isAttackHit = true;
        }

        #region Animation Events
        void Attack1()
        {
            attackObject1.SetActive(true);
            //Debug.Log("attackObject1 On");
        }

        void Attack1Done()
        {
            attackObject1.SetActive(false);
            attackState = AttackState.PrepareIdle;
            //Debug.Log("attackObject1 Off");
        }
        #endregion

        #region
        public void AttackAudio()
        {
            boss.audioSource.PlayOneShot(attackAudio, 1f);
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
