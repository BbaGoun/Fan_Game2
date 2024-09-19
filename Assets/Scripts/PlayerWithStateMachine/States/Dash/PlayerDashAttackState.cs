using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static AttackEffect;

public class PlayerDashAttackState : State
{
    PlayerWithStateMachine player;

    private PlayerDashState playerDashState;
    private PlayerAttackState playerAttackState;
    
    [Header("DashAttack Parameter")]
    [SerializeField]
    private DashAttackState dashAttackState;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private int attackMoveDelayFrame;
    [SerializeField]
    private int attackMoveFrame;
    [SerializeField]
    private GameObject attackObject;
    private AttackEffect attackEffect;

    [SerializeField]
    private float shakeDuration;
    [SerializeField]
    private float shakeIntensity;

    private bool isAttackHit;
    private float timer;
    private bool isBackStep;

    private void Awake()
    {
        attackEffect = attackObject.GetComponent<AttackEffect>();
    }

    public void Initialize(PlayerWithStateMachine _playerWithStateMachine, PlayerDashState _playerDashState, PlayerAttackState _playerAttackState)
    {
        player = _playerWithStateMachine;
        playerDashState = _playerDashState;
        playerAttackState = _playerAttackState;
    }

    public override void EnterState()
    {
        dashAttackState = DashAttackState.PrepareAttack;
        attackEffect.eventAttackHit += OnAttackHit;

        base.EnterState();
    }

    public override void ExitState()
    {
        attackEffect.eventAttackHit -= OnAttackHit;

        attackObject.SetActive(false);

        dashAttackState = DashAttackState.Idle;
        player.ResetAnimator();

        playerDashState.SetLastDashTime();
        playerAttackState.SetLastAttackTime();

        base.ExitState();
    }

    public override void FrameUpdate()
    {
        #region State Change
        if(dashAttackState == DashAttackState.Idle)
        {
            player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
        }
        if (player.damageInfo.isDamaged)
        {
            player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
        }
        #endregion

        attackEffect.SetShakeDuration(shakeDuration);
        attackEffect.SetShakeIntensity(shakeIntensity);

        UpdateDashAttackState();
    }

    public override void PhysicsUpdate()
    {
        switch (dashAttackState)
        {
            case DashAttackState.Attacking:
                timer += Time.deltaTime;
                var attackDuration = attackMoveFrame / 60f;
                var timePer = timer / attackDuration;
                timePer = Mathf.Clamp01(timePer);
                var rate = 1 - Mathf.Pow(timePer, 2f);
                var lookDirection = 1 * Mathf.Sign(gameObject.transform.localScale.x);

                if (isAttackHit)
                    player.velocity.x = -lookDirection * attackSpeed * rate / 2f;
                else
                    player.velocity.x = lookDirection * attackSpeed * rate;
                player.velocity.y = 0f;
                break;
            default:
                player.velocity.x = 0f;
                player.velocity.y = Physics2D.gravity.y;
                break;
        }

    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    void UpdateDashAttackState()
    {
        switch (dashAttackState)
        {
            case DashAttackState.Idle:
                break;
            case DashAttackState.PrepareAttack:
                player.SetAnimatorTrigger("isDashAttack");
                isAttackHit = false;
                timer = 0f;
                dashAttackState = DashAttackState.Attacking;
                break;
            case DashAttackState.Attacking:
                break;
            case DashAttackState.PrepareIdle:
                timer += Time.deltaTime;
                if(timer > waitTime)
                {
                    timer = 0f;
                    dashAttackState = DashAttackState.Idle;
                }
                break;
        }
    }

    public void SetBackStep(bool _isBackStep)
    {
        isBackStep = _isBackStep;
    }

    void OnAttackHit()
    {
        isAttackHit = true;
    }

    void DashAttack()
    {
        attackObject.SetActive(true);
    }

    void DashAttackDone()
    {
        attackObject.SetActive(false);
        dashAttackState = DashAttackState.PrepareIdle;
        timer = 0f;
    }


    private enum DashAttackState
    {
        Idle,
        PrepareDash,
        Dashing,
        PrepareAttack,
        Attacking,
        PrepareIdle,
    }
}
