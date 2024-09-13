using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static AttackEffect;


public class PlayerChargeAttackState : State
{
    PlayerWithStateMachine player;

    [Header("Charge Attack Parameter")]
    [SerializeField]
    private float chargeTime;
    private float chargeTimer;
    [SerializeField]
    private float attackMoveSpeed;
    [SerializeField]
    private float attackDelayFrame;
    [SerializeField]
    private float attackDurationFrame;
    private float attackTimer;
    private float attackMoveTimer;
    [SerializeField]
    private float attackWaitTime;
    [SerializeField]
    private ChargeAttackState chargeAttackState;
    [SerializeField]
    private float shakeDuration;
    [SerializeField]
    private float shakeIntensity;

    [SerializeField]
    private GameObject chargeObject;
    private ChargeEffect chargeEffect;
    [SerializeField]
    private GameObject attackObject;
    private AttackEffect attackEffect;

    private void Awake()
    {
        chargeAttackState = ChargeAttackState.Idle;
        attackEffect = attackObject.GetComponent<AttackEffect>();
        chargeEffect = chargeObject.GetComponent<ChargeEffect>();
    }
    
    public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
    {
        player = _playerWithStateMachine;
        PlayerInputPart.Instance.EventAttackKeyHolding += OnCharge;
        PlayerInputPart.Instance.EventAttackKeyUp += OffCharge;
    }

    public override void EnterState()
    {
        player.ChargeDone();
        ResetCharge();
        chargeAttackState = ChargeAttackState.ChargeAttack;
        base.EnterState();
    }

    public override void ExitState()
    {
        ResetCharge();
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        attackEffect.SetShakeDuration(shakeDuration);
        attackEffect.SetShakeIntensity(shakeIntensity);
        UpdateAttackState();
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        switch (chargeAttackState)
        {
            case ChargeAttackState.ChargeAttacking:
                attackMoveTimer += Time.deltaTime;
                float delay = attackDelayFrame / 60f;
                if (attackMoveTimer > delay)
                {
                    float duration = attackDurationFrame / 60f;
                    float timePer = (attackMoveTimer - delay) / duration;
                    timePer = Mathf.Clamp01(timePer);
                    float rate = 1 - Mathf.Pow(timePer, 3);
                    float lookDirection = 1 * Mathf.Sign(gameObject.transform.localScale.x);

                    player.velocity.x = lookDirection * attackMoveSpeed * rate;
                    player.velocity.y = 0f;
                }
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

    public void UpdateChargeState()
    {
        switch (chargeAttackState)
        {
            case ChargeAttackState.Idle:
                chargeEffect.animator.Rebind();
                chargeEffect.animator.Update(0f);
                break;
            case ChargeAttackState.PrepareCharge:
                chargeTimer = 0f;
                chargeAttackState = ChargeAttackState.Charging;
                chargeEffect.animator.SetTrigger("isCharging");
                Debug.Log("차지 시작");
                //차지 이펙트 초기화
                break;
            case ChargeAttackState.Charging:
                chargeTimer += Time.deltaTime;
                if(chargeTimer > chargeTime)
                {
                    chargeEffect.animator.SetBool("isLevel1", true);
                    chargeAttackState = ChargeAttackState.Charging_Level1;
                }
                //차지 이펙트 변화
                break;
            case ChargeAttackState.Charging_Level1:
                break;
        }
    }

    void UpdateAttackState()
    {
        switch(chargeAttackState)
        {
            case ChargeAttackState.ChargeAttack:
                attackTimer = 0f;
                attackMoveTimer = 0f;
                player.SetAnimatorTrigger("isChargeAttack");
                chargeAttackState = ChargeAttackState.ChargeAttacking;
                //애니메이션 및 다른거 초기화
                break;
            case ChargeAttackState.ChargeAttacking:
                // Physics에서 진행
                break;
            case ChargeAttackState.PrepareIdle:
                attackTimer += Time.deltaTime;
                if(attackTimer > attackWaitTime)
                {
                    attackTimer = 0f;
                    player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
                }
                // 상태 나가기 전 경직
                break;
        }
    }

    public bool CheckCanChargeAttack()
    {
        return chargeTimer > chargeTime;
    }

    public void ResetCharge()
    {
        chargeAttackState = ChargeAttackState.Idle;
        chargeTimer = 0f;
        chargeEffect.animator.Rebind();
        chargeEffect.animator.Update(0f);
    }

    #region Animation Events

    void ChargeAttack()
    {
        attackObject.SetActive(true);
    }

    void ChargeAttackDone()
    {
        attackObject.SetActive(false);
        chargeAttackState = ChargeAttackState.PrepareIdle;
    }

    #endregion

    #region Key Event
    private void OnCharge()
    {
        if (chargeAttackState == ChargeAttackState.Idle)
        {
            chargeAttackState = ChargeAttackState.PrepareCharge;
            Debug.Log("차지 준비");
        }
    }

    private void OffCharge()
    {
        if(chargeAttackState == ChargeAttackState.Charging_Level1)
        {
            player.isCharged = true;
        }
        else
        {
            player.isCharged = false;
            chargeAttackState = ChargeAttackState.Idle;
        }
    }
    #endregion

    enum ChargeAttackState
    {
        Idle,
        PrepareCharge,
        Charging,
        Charging_Level1,
        ChargeAttack,
        ChargeAttacking,
        PrepareIdle,
        CantCharge,
    }
}
