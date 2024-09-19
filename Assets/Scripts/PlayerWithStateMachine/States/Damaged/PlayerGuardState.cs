using ActionPart;
using ActionPart.MemoryPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuardState : State
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
    private int hpDelta;

    private GuardState guardState;
    #endregion


    public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
    {
        player = _playerWithStateMachine;
        health = gameObject.GetComponent<Health>();
    }

    public override void EnterState()
    {
        guardState = GuardState.Damaged;
        player.damageInfo.isDamaged = false;
        base.EnterState();
    }

    public override void ExitState()
    {
        player.damageInfo.isDamaged = false;
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        #region State Change
        if (guardState == GuardState.Idle)
        {
            player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
        }
        #endregion

        UpdateDamagedState();
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        switch (guardState)
        {
            case GuardState.KnockBacked:
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

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public void GetDamageInfo()
    {
        if (player.damageInfo.isDamaged)
        {
            if (player.damageInfo.direction.x <= 0f && transform.localScale.x != 1f)
            {
                // 오른쪽에서 공격을 당했는데, 오른쪽을 보고있지 않았을 경우
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
            else if (player.damageInfo.direction.x > 0f && transform.localScale.x != -1f)
            {
                // 왼쪽에서 공격을 당했는데, 왼쪽을 보고있지 않았을 경우
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
            else
            {
                hpDelta = player.damageInfo.hpDelta;
                if(player.damageInfo.direction.x <= 0)
                {
                    knockBackDirection = -1f;
                }
                else if(player.damageInfo.direction.x > 0)
                {
                    knockBackDirection = 1f;
                }

                guardState = GuardState.Damaged;
            }
        }
    }

    void UpdateDamagedState()
    {
        switch(guardState)
        {
            case GuardState.Idle:
                break;
            case GuardState.Damaged:
                knockBackTimer = 0f;
                var canHurt = health.Hurt(hpDelta, currentStiffness.invincibleDuration,
                    currentStiffness.waitFlashTime, currentStiffness.flashFrequency, currentStiffness.flashRepetition, currentStiffness.maxFlash);

                VirtualCameraControl.Instance.ShakeCamera(currentStiffness.shakeDuration, currentStiffness.shakeIntensity);
                
                player.SetAnimatorTrigger(currentStiffness.animationTriggerName);

                slowTimer = 0f;
                TimeController.Instance.SetTimeScale(slowScale);
                guardState = GuardState.KnockBacked;
                break;
            case GuardState.KnockBacked:
                if (TimeController.Instance.GetTimeScale() != 0f)
                {
                    slowTimer += Time.unscaledDeltaTime;
                    if(slowTimer >= currentStiffness.slowTime)
                    {
                        TimeController.Instance.SetTimeScale(1f);
                    }
                }
                break;
            case GuardState.PrepareIdle:
                waitTimer += Time.deltaTime;
                if (waitTimer > waitTime)
                {
                    waitTimer = 0f;
                    guardState = GuardState.Idle;
                }
                break;
        }
    }

    #region Animation Events
    void KnockBackDone()
    {
        guardState = GuardState.PrepareIdle;
        waitTimer = 0f;
    }
    #endregion

    enum GuardState
    {
        JustGuard,
        Parrying,
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

        public float slowTime;
    }
}
