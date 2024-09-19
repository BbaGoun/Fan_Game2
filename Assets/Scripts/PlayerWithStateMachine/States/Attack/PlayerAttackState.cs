using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : State
{
    PlayerWithStateMachine player;

    [Header("Attack Parameter")]
    [SerializeField]
    private bool attackStarted;
    [SerializeField]
    private bool attackCanceled;
    [SerializeField]
    private AttackState attackState;
    private float attackTimer;
    [SerializeField]
    private float attackGapDelay; // 공격 간 딜레이
    [SerializeField]
    private float attackWaitTime; // 공격 간 연결 대기 시간
    [SerializeField]
    private float attackMoveSpeed;
    [SerializeField]
    private int attackMoveDelayFrame;
    [SerializeField]
    private int attackMoveFrame;
    private float attackMoveTimer;
    [SerializeField]
    private float attackCoolTime;
    private float lastAttackTime;
    [SerializeField]
    private bool isAttackHit;
    [SerializeField]
    private GameObject attackObject1;
    private AttackEffect attackEffect1;
    [SerializeField]
    private GameObject attackObject2;
    private AttackEffect attackEffect2;
    [SerializeField]
    private GameObject attackObject3;
    private AttackEffect attackEffect3;
    [SerializeField]
    private float shakeDuration;
    [SerializeField]
    private float shakeIntensity;

    private void Awake()
    {
        attackEffect1 = attackObject1.GetComponent<AttackEffect>();
        attackEffect2 = attackObject2.GetComponent<AttackEffect>();
        attackEffect3 = attackObject3.GetComponent<AttackEffect>();
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
        attackEffect3.eventAttackHit += OnAttackHit;

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
        attackEffect3.eventAttackHit -= OnAttackHit;

        attackObject1.SetActive(false);
        attackObject2.SetActive(false);
        attackObject3.SetActive(false);
        attackState = AttackState.Idle;
        player.ResetAnimator();

        SetLastAttackTime();
        isAttackHit = false;

        base.ExitState();
    }

    public override void FrameUpdate()
    {
        #region State Change
        if (!player.isGrounded)
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

        attackEffect3.SetShakeDuration(shakeDuration);
        attackEffect3.SetShakeIntensity(shakeIntensity);

        ControlAttack();
        UpdateAttackState();
    }

    public override void PhysicsUpdate()
    {
        switch (attackState)
        {
            case AttackState.Attacking1:
            case AttackState.Attacking2:
            case AttackState.Attacking3:
                attackMoveTimer += Time.deltaTime;
                float delay = attackMoveDelayFrame / 60f;
                if (attackMoveTimer > delay)
                {
                    float duration = attackMoveFrame / 60f;
                    float timePer = (attackMoveTimer - delay) / duration;
                    timePer = Mathf.Clamp01(timePer);
                    float rate = 1 - Mathf.Pow(timePer, 3);
                    float lookDirection = 1 * Mathf.Sign(gameObject.transform.localScale.x);

                    if (isAttackHit)
                        player.velocity.x = -lookDirection * attackMoveSpeed * rate / 2f;
                    else
                        player.velocity.x = lookDirection * attackMoveSpeed * rate; 
                    player.velocity.y = 0f;
                }
                break;
            case AttackState.PrepareAttack2:
            case AttackState.PrepareAttack3:
                player.velocity.x = 0f;
                player.velocity.y = Physics2D.gravity.y;
                break;
        }
    }

    public bool CheckCanAttack()
    {
        return Time.time - lastAttackTime > attackCoolTime;
    }

    public void SetLastAttackTime()
    {
        lastAttackTime = Time.time;
    }

    void ControlAttack()
    {
        if (attackState == AttackState.PrepareAttack2 && attackStarted && attackTimer > attackGapDelay)
        {
            attackStarted = false;
            attackState = AttackState.Attack2;
        }
        else if (attackState == AttackState.PrepareAttack3 && attackStarted && attackTimer > attackGapDelay)
        {
            attackStarted = false;
            attackState = AttackState.Attack3;
        }
    }

    void UpdateAttackState()
    {
        switch (attackState)
        {
            case AttackState.Attack1:
                player.SetAnimatorTrigger("isAttack1");
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
                if(attackTimer > attackWaitTime)
                {
                    attackTimer = 0f;
                    attackState = AttackState.Idle;
                }
                break;

            case AttackState.Attack2:
                player.SetAnimatorTrigger("isAttack2");
                attackTimer = 0f;
                attackMoveTimer = 0f;
                isAttackHit = false;
                //Debug.Log("공격2 애니메이션 시작");
                attackState = AttackState.Attacking2;
                break;
            case AttackState.Attacking2:
                break;
            case AttackState.PrepareAttack3:
                attackTimer += Time.deltaTime;
                if (attackTimer > attackWaitTime)
                {
                    attackTimer = 0f;
                    attackState = AttackState.Idle;
                }
                break;

            case AttackState.Attack3:
                player.SetAnimatorTrigger("isAttack3");
                attackTimer = 0f;
                attackMoveTimer = 0f;
                isAttackHit = false;
                //Debug.Log("공격3 애니메이션 시작");
                attackState = AttackState.Attacking3;
                break;
            case AttackState.Attacking3:
                break;
            case AttackState.PrepareIdle:
                attackTimer += Time.deltaTime;
                if(attackTimer > attackWaitTime)
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
    void Attack1()
    {
        attackObject1.SetActive(true);
        //Debug.Log("attackObject1 On");
    }

    void Attack1Done()
    {
        attackObject1.SetActive(false);
        attackState = AttackState.PrepareAttack2;
        //Debug.Log("attackObject1 Off");
    }

    void Attack2()
    {
        attackObject2.SetActive(true);
        //Debug.Log("attackObject2 On");
    }

    void Attack2Done()
    {
        attackObject2.SetActive(false);
        attackState = AttackState.PrepareAttack3;
        //Debug.Log("attackObject2 Off");
    }

    void Attack3()
    {
        attackObject3.SetActive(true);
        //Debug.Log("attackObject3 On");
    }

    void Attack3Done()
    {
        attackObject3.SetActive(false);
        attackState = AttackState.PrepareIdle;
        //Debug.Log("attackObject3 Off");
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
        PrepareAttack3,
        Attack3,
        Attacking3,
        PrepareIdle,
    }
}
