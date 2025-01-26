using ActionPart;
using ActionPart.MemoryPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlayerDamagedState : State
    {
        PlayerWithStateMachine player;

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
        [SerializeField]
        private float slowScale;
        private float slowTimer;

        private Health health;
        private float hpDelta;

        private IDamageAble.HitType hitType;

        private DamagedState damagedState;
        GameObject hittedEffect;
        #endregion


        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
            health = gameObject.GetComponent<Health>();
        }

        public override void EnterState()
        {
            damagedState = DamagedState.Damaged;
            GetDamageInfo();
            player.ResetDamage();
            base.EnterState();
        }

        public override void ExitState()
        {
            player.ResetDamage();
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change
            if (damagedState == DamagedState.Idle)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
            }
            #endregion

            UpdateDamagedState();
            base.FrameUpdate();
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

                    player.velocity.x = knockBackDirection * currentStiffness.knockBackVector.x * rate;
                    player.velocity.y = currentStiffness.knockBackVector.y * rate + Physics2D.gravity.y;
                    break;
                default:
                    player.velocity.x = 0f;
                    player.velocity.y = Physics2D.gravity.y;
                    break;
            }
            base.PhysicsUpdate();
        }

        void GetDamageInfo()
        {
            hpDelta = player.damageInfo.hpDelta;
            bool isStiffnessSelected = false;

            foreach (Stiffness stiffness in stiffnessList)
            {
                if (hpDelta <= stiffness.damageThreshold)
                {
                    isStiffnessSelected = true;
                    currentStiffness = stiffness;
                    Debug.Log("Stiffness Type : " + currentStiffness.stiffnessName);
                    break;
                }
            }

            if(!isStiffnessSelected)
            {
                Debug.Log("최대 데미지로 맞음");
                currentStiffness = stiffnessList[stiffnessList.Count - 1];
            }

            if (player.damageInfo.knockbackDirection.x <= 0)
            { // 오른쪽에서 온 충격
                player.LookRight();
                knockBackDirection = -1f;
            }
            else if (player.damageInfo.knockbackDirection.x > 0)
            { // 왼쪽에서 온 충격
                player.LookLeft();
                knockBackDirection = 1f;
            }

            hitType = player.damageInfo.hitType;
        }

        void UpdateDamagedState()
        {
            switch (damagedState)
            {
                case DamagedState.Idle:
                    break;
                case DamagedState.Damaged:
                    knockBackTimer = 0f;
                    var canHurt = health.Hurt_Hp(hpDelta, currentStiffness.invincibleDuration,
                        currentStiffness.waitFlashTime, currentStiffness.flashFrequency, currentStiffness.flashRepetition, currentStiffness.maxFlash);

                    VirtualCameraControl.Instance.ShakeCamera(currentStiffness.shakeDuration, currentStiffness.shakeIntensity);

                    if(!health.CheckIsAlive())
                    {
                        player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Death);
                        return;
                    }

                    player.SetAnimatorTrigger(currentStiffness.animationTriggerName);

                    switch (hitType)
                    {
                        case IDamageAble.HitType.Special:
                            hittedEffect = ObjectPoolManager.Instance.GetObject("Player_Healing_Hitted_Effect");
                            hittedEffect.transform.position = gameObject.transform.position;
                            break;
                        default:
                            if (currentStiffness.stiffnessName.Equals("Big"))
                            {
                                hittedEffect = ObjectPoolManager.Instance.GetObject("Player_Hitted_Strong");
                                hittedEffect.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -5);
                                hittedEffect.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1, 1);
                            }
                            else if (currentStiffness.stiffnessName.Equals("Small"))
                            {
                                hittedEffect = ObjectPoolManager.Instance.GetObject("Player_Hitted_Weak");
                                hittedEffect.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -5);
                                hittedEffect.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1, 1);
                            }
                            else
                            {
                                Debug.Log("stiffnessName : " + currentStiffness.stiffnessName);
                            }
                            break;
                    }

                    slowTimer = 0f;
                    TimeController.Instance.SetTimeScale(slowScale);
                    damagedState = DamagedState.KnockBacked;
                    break;
                case DamagedState.KnockBacked:
                    if (TimeController.Instance.GetTimeScale() != 0f)
                    {
                        slowTimer += Time.unscaledDeltaTime;
                        if (slowTimer >= currentStiffness.slowTime)
                        {
                            TimeController.Instance.SetTimeScale(1f);
                        }
                    }
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
            public string stiffnessName;
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

            public float slowTime;
        }
    }
}