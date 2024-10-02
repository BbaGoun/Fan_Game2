using ActionPart;
using ActionPart.MemoryPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlayerGuardState : State
    {
        PlayerWithStateMachine player;

        #region parameters
        [Header("Stiffness Parameter")]
        [SerializeField]
        List<Stiffness> stiffnessList;
        Stiffness currentStiffness;
        private float knockBackTimer;
        private float knockBackDirection;

        [Header("Else")]
        [SerializeField]
        private float waitTime;
        private float waitTimer;
        [SerializeField]
        private float slowScale;
        private float slowTimer;
        [SerializeField]
        private float justGuardTime;
        private float justGuardTimer;
        [SerializeField]
        private float parryInvincibleTime;
        [SerializeField]
        private float parryShakeDuration;
        [SerializeField]
        private float parryShakeIntensity;

        private Health health;
        private float hpDelta;

        [SerializeField]
        private GuardState guardState;
        #endregion


        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
            health = gameObject.GetComponent<Health>();
        }

        public override void EnterState()
        {
            PlayerInputPart.Instance.EventGuardKeyUp += GuardKeyUp;
            player.SetAnimatorTrigger("isGuardStart");
            player.SetAnimatorBool("isGuard", true);
            guardState = GuardState.PrepareJustGuard;
            base.EnterState();
        }

        public override void ExitState()
        {
            PlayerInputPart.Instance.EventGuardKeyUp -= GuardKeyUp;
            player.SetAnimatorBool("isGuard", false);
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change
            switch (guardState)
            {
                case GuardState.Idle:
                case GuardState.JustGuard:
                    if (!player.isGuard)
                    {
                        waitTimer = 0f;
                        guardState = GuardState.PrepareStateOut;
                    }
                    break;
                default:
                    break;
            }
            if (!player.isGrounded && guardState == GuardState.Idle)
            {
                waitTimer = 0f;
                guardState = GuardState.PrepareStateOut;
            }
            if (health.CheckIsGroggy())
            {
                // 아직 그로기 상태가 준비 안 됌.
                //player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Groggy);
            }
            #endregion

            GetDamageInfo();
            UpdateGuardState();
            base.FrameUpdate();
        }

        public override void PhysicsUpdate()
        {
            switch (guardState)
            {
                case GuardState.Parrying:
                case GuardState.KnockBacked:
                    knockBackTimer += Time.deltaTime;
                    var duration = currentStiffness.knockBackDurationFrame / 60f;
                    var timePer = knockBackTimer / duration;
                    timePer = Mathf.Clamp01(timePer);
                    var rate = 1 - Mathf.Pow(timePer, 2f);

                    if (guardState == GuardState.Parrying)
                        player.velocity.x = knockBackDirection * currentStiffness.knockBackVector.x / 2 * rate;
                    if (guardState == GuardState.KnockBacked)
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


        public void GetDamageInfo()
        {
            if (player.damageInfo.isDamaged)
            {
                if (player.damageInfo.knockbackDirection.x <= 0f && transform.localScale.x != 1f)
                {
                    // 오른쪽에서 공격을 당했는데, 오른쪽을 보고있지 않았을 경우
                    player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
                }
                else if (player.damageInfo.knockbackDirection.x > 0f && transform.localScale.x != -1f)
                {
                    // 왼쪽에서 공격을 당했는데, 왼쪽을 보고있지 않았을 경우
                    player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
                }
                else
                {
                    hpDelta = player.damageInfo.hpDelta;

                    foreach (Stiffness stiffness in stiffnessList)
                    {
                        if (hpDelta <= stiffness.damageThreshold)
                        {
                            currentStiffness = stiffness;
                            Debug.Log("Stiffness Type : " + currentStiffness.StiffnessName);
                            break;
                        }
                    }

                    if (player.damageInfo.knockbackDirection.x <= 0)
                    {
                        knockBackDirection = -1f;
                    }
                    else if (player.damageInfo.knockbackDirection.x > 0)
                    {
                        knockBackDirection = 1f;
                    }

                    if (guardState == GuardState.JustGuard)
                        guardState = GuardState.PrepareParry;
                    else
                        guardState = GuardState.Damaged;
                }

                player.ResetDamage();
            }
        }

        void UpdateGuardState()
        {
            switch (guardState)
            {
                case GuardState.PrepareJustGuard:
                    justGuardTimer = 0f;
                    guardState = GuardState.JustGuard;
                    break;
                case GuardState.JustGuard:
                    // 이동 기능 넣기
                    justGuardTimer += Time.deltaTime;
                    if (justGuardTimer >= justGuardTime)
                    {
                        guardState = GuardState.Idle;
                    }
                    break;
                case GuardState.PrepareParry:
                    knockBackTimer = 0f;

                    player.SetAnimatorTrigger("isParry");

                    health.Hurt_StaminaOnlyTo1(hpDelta);
                    health.OnInvincible(parryInvincibleTime);
                    // 적 스테미나 감소 시키기 필요
                    var parryEffect = ObjectPoolManager.Instance.GetObject("Player_Parry_Effect");
                    parryEffect.transform.position = player.transform.position + new Vector3(-1 * knockBackDirection * 1f, 0.5f, 0f);
                    parryEffect.transform.localScale = new Vector3(-1 * knockBackDirection, 1, 1);

                    VirtualCameraControl.Instance.ShakeCamera(parryShakeDuration, parryShakeIntensity);

                    slowTimer = 0f;
                    TimeController.Instance.SetTimeScale(slowScale);

                    guardState = GuardState.Parrying;
                    break;
                case GuardState.Parrying:
                    if (TimeController.Instance.GetTimeScale() != 0f)
                    {
                        slowTimer += Time.unscaledDeltaTime;
                        if (slowTimer >= currentStiffness.slowTime)
                        {
                            TimeController.Instance.SetTimeScale(1f);
                        }
                    }
                    // 패링 애니메이션에 트리거 붙여놓음
                    break;
                case GuardState.Idle:
                    //이동 기능 넣기
                    break;
                case GuardState.Damaged:
                    knockBackTimer = 0f;

                    var canHurt = health.Hurt_Hp(hpDelta / 2, currentStiffness.invincibleDuration,
                        currentStiffness.waitFlashTime, currentStiffness.flashFrequency, currentStiffness.flashRepetition, currentStiffness.maxFlash);
                    health.Hurt_Stamina(hpDelta);

                    VirtualCameraControl.Instance.ShakeCamera(currentStiffness.shakeDuration, currentStiffness.shakeIntensity);

                    player.SetAnimatorTrigger("isGuardDamaged");

                    slowTimer = 0f;
                    TimeController.Instance.SetTimeScale(slowScale);

                    guardState = GuardState.KnockBacked;
                    break;
                case GuardState.KnockBacked:
                    if (TimeController.Instance.GetTimeScale() != 0f)
                    {
                        slowTimer += Time.unscaledDeltaTime;
                        if (slowTimer >= currentStiffness.slowTime)
                        {
                            TimeController.Instance.SetTimeScale(1f);
                        }
                    }
                    // 넉백 애니메이션에 트리거 붙여놓음
                    break;
                case GuardState.PrepareIdle:
                    waitTimer += Time.deltaTime;
                    if (waitTimer > waitTime)
                    {
                        waitTimer = 0f;
                        guardState = GuardState.Idle;
                    }
                    break;
                case GuardState.PrepareStateOut:
                    waitTimer += Time.deltaTime;
                    if (waitTimer > waitTime)
                    {
                        waitTimer = 0f;
                        player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
                    }
                    break;
            }
        }

        #region Animation Events
        void ParryDone()
        {
            guardState = GuardState.PrepareStateOut;
            waitTimer = 0f;
        }

        void GuardDone()
        {
            guardState = GuardState.PrepareIdle;
            waitTimer = 0f;
        }
        #endregion

        #region Key Event
        public void GuardKeyDown()
        {
            player.isGuard = true;
        }

        public void GuardKeyUp()
        {
            player.isGuard = false;
        }


        #endregion

        enum GuardState
        {
            PrepareJustGuard,
            JustGuard,
            PrepareParry,
            Parrying,
            Idle,
            Damaged,
            KnockBacked,
            PrepareIdle,
            PrepareStateOut,
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

            public float slowTime;
        }
    }
}
