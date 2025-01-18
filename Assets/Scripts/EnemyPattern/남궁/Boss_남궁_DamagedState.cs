using System;
using ActionPart.MemoryPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Boss_남궁_DamagedState : State
    {
        Boss_남궁 boss;

        #region parameters
        [Header("Stiffness Parameter")]
        private float knockBackTimer;
        private float knockBackDirection;
        [SerializeField]
        List<Stiffness> stiffnessList;
        Stiffness currentStiffness;

        [Header("Else")]
        [SerializeField]
        private float waitTime;
        private float waitTimer;

        private EnemyHealth health;
        private float hpDelta;

        private IDamageAble.HitType hitType;

        private DamagedState damagedState;
        GameObject hittedEffect;
        #endregion


        public void Initialize(Boss_남궁 _boss)
        {
            boss = _boss;
            health = gameObject.GetComponent<EnemyHealth>();
        }

        public override void EnterState()
        {
            damagedState = DamagedState.Damaged;
            GetDamageInfo();
            boss.ResetDamage();
            base.EnterState();
        }

        public override void ExitState()
        {
            boss.ResetDamage();
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change
            if (damagedState == DamagedState.Idle)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Move);
            }
            if (boss.damageInfo.isDamaged)
            {
                GetDamageInfo();
                boss.ResetDamage();
                damagedState = DamagedState.Damaged;
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

            UpdateDamagedState();
        }

        void UpdateDamagedState()
        {
            switch (damagedState)
            {
                case DamagedState.Idle:
                    break;
                case DamagedState.Damaged:
                    boss.UpDamageCount();
                    
                    knockBackTimer = 0f;
                    var canHurt = health.Hurt_Hp(hpDelta, currentStiffness.invincibleDuration,
                        currentStiffness.waitFlashTime, currentStiffness.flashFrequency, currentStiffness.flashRepetition, currentStiffness.maxFlash);
                    health.Hurt_Stamina(hpDelta);

                    boss.SetAnimatorTrigger(currentStiffness.animationTriggerName);

                    damagedState = DamagedState.KnockBacked;
                    break;
                case DamagedState.KnockBacked:
                    break;
                case DamagedState.PrepareIdle:
                    waitTimer += Time.deltaTime;
                    if (waitTimer > waitTime)
                    {
                        waitTimer = 0f;
                        damagedState = DamagedState.Idle;
                    }
                    break;
            }
        }

        public override void PhysicsUpdate()
        {
            switch (damagedState)
            {
                case DamagedState.KnockBacked:
                    knockBackTimer += Time.deltaTime;
                    var duration = currentStiffness.knockBackDurationFrame / 60f;
                    var timePer = knockBackTimer / duration;
                    timePer = Mathf.Clamp01(timePer);
                    var rate = 1 - Mathf.Pow(timePer, 2f);

                    boss.velocity.x = knockBackDirection * currentStiffness.knockBackVector.x * rate;
                    boss.velocity.y = currentStiffness.knockBackVector.y * rate + Physics2D.gravity.y;
                    break;
                default:
                    boss.velocity.x = 0f;
                    boss.velocity.y = Physics2D.gravity.y;
                    break;
            }
        }

        void GetDamageInfo()
        {
            hpDelta = boss.damageInfo.hpDelta;
            bool isStiffnessSelected = false;

            foreach (Stiffness stiffness in stiffnessList)
            {
                if (hpDelta <= stiffness.damageThreshold)
                {
                    isStiffnessSelected = true;
                    currentStiffness = stiffness;
                    //Debug.Log("Stiffness Type : " + currentStiffness.StiffnessName);
                    break;
                }
            }

            if (!isStiffnessSelected)
            {
                //Debug.Log("최대 데미지로 맞음");
                currentStiffness = stiffnessList[stiffnessList.Count - 1];
            }

            if (!boss.CheckIsSuperArmour())
            {
                if (boss.damageInfo.knockbackDirection.x <= 0)
                { // 오른쪽에서 온 충격
                    boss.LookRight();
                    knockBackDirection = -1f;
                }
                else if (boss.damageInfo.knockbackDirection.x > 0)
                { // 왼쪽에서 온 충격
                    boss.LookLeft();
                    knockBackDirection = 1f;
                }

                hitType = boss.damageInfo.hitType;
            }
        }

        public void JustDamage()
        {
            GetDamageInfo();

            var canHurt = health.Hurt_Hp(hpDelta, currentStiffness.invincibleDuration,
                currentStiffness.waitFlashTime, currentStiffness.flashFrequency, currentStiffness.flashRepetition, currentStiffness.maxFlash);
            health.Hurt_Stamina(hpDelta);

            switch (hitType)
            {
                default:
                    hittedEffect = ObjectPoolManager.Instance.GetObject("Enemy_Hitted_Effect");
                    hittedEffect.transform.position = gameObject.transform.position;
                    break;
            }
        }
        

        #region Animation Events
        void KnockBackDone()
        {
            damagedState = DamagedState.PrepareIdle;
            waitTimer = 0f;
        }
        #endregion

        enum DamagedState
        {
            Idle,
            Damaged,
            KnockBacked,
            PrepareIdle,
        }

        [Serializable]
        struct Stiffness
        {
            public string StiffnessName;
            public float damageThreshold;
            public Vector2 knockBackVector;

            public float knockBackDurationFrame;

            public float invincibleDuration;
            public string animationTriggerName;

            public float shakeDuration;
            public float shakeIntensity;

            public float waitFlashTime;
            public float flashFrequency;
            public float flashRepetition;
            [Range(0f, 1f)]
            public float maxFlash;
        }
    }
}
