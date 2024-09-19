using ActionPart;
using ActionPart.MemoryPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamagedState : State, IDamageAble
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

    private DamagedState damagedState;
    #endregion


    public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
    {
        player = _playerWithStateMachine;
        health = gameObject.GetComponent<Health>();
    }

    public override void EnterState()
    {
        damagedState = DamagedState.Damaged;
        player.isDamaged = false;
        base.EnterState();
    }

    public override void ExitState()
    {
        player.isDamaged = false;
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

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public void GetDamage(int _hpDelta, Vector2 direction)
    {
        var isInvincible = health.CheckInvincible();
        if (isInvincible)
        {
            Debug.Log("무적일 때 닿음");
            return;
        }

        player.isDamaged = true;
        hpDelta = _hpDelta;

        foreach(Stiffness stiffness in stiffnessList)
        {
            if(hpDelta <= stiffness.damageThreshold)
            {
                currentStiffness = stiffness;
                Debug.Log("Stiffness Type : " + currentStiffness.StiffnessName);
                break;
            }
        }

        if (direction.x <= 0)
        { // 오른쪽에서 온 충격
            player.LookRight();
            knockBackDirection = -1f;
        }
        else if (direction.x > 0)
        { // 왼쪽에서 온 충격
            player.LookLeft();
            knockBackDirection = 1f;
        }

        var hittedEffect = ObjectPoolManager.Instance.GetObject("Player_Hitted_Effect");
        hittedEffect.transform.position = gameObject.transform.position;
    }

    void UpdateDamagedState()
    {
        switch(damagedState)
        {
            case DamagedState.Idle:
                break;
            case DamagedState.Damaged:
                knockBackTimer = 0f;
                var canHurt = health.Hurt(hpDelta, currentStiffness.invincibleDuration,
                    currentStiffness.waitFlashTime, currentStiffness.flashFrequency, currentStiffness.flashRepetition, currentStiffness.maxFlash);

                VirtualCameraControl.Instance.ShakeCamera(currentStiffness.shakeDuration, currentStiffness.shakeIntensity);
                
                player.SetAnimatorTrigger(currentStiffness.animationTriggerName);

                slowTimer = 0f;
                TimeController.Instance.SetTimeScale(slowScale);
                damagedState = DamagedState.KnockBacked;
                break;
            case DamagedState.KnockBacked:
                if (TimeController.Instance.GetTimeScale() != 0f)
                {
                    slowTimer += Time.unscaledDeltaTime;
                    if(slowTimer >= currentStiffness.slowTime)
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
