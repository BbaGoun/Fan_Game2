using ActionPart;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlayerWithStateMachine : KinematicObject, IWithStateMachine, IDamageAble
    {
        public static PlayerWithStateMachine Instance;

        StateMachine stateMachine;

        #region PlayerState
        [SerializeField]
        public PlayerMoveState playerMoveState;
        [SerializeField]
        PlayerAttackState playerAttackState;
        [SerializeField]
        PlayerDashState playerDashState;
        [SerializeField]
        PlayerDashAttackState playerDashAttackState;
        [SerializeField]
        PlayerJumpAttackState playerJumpAttackState;
        [SerializeField]
        PlayerChargeAttackState playerChargeAttackState;
        [SerializeField]
        PlayerDamagedState playerDamagedState;
        [SerializeField]
        PlayerGuardState playerGuardState;
        [SerializeField]
        PlayerHealState playerHealState;
        [SerializeField]
        PlayerGroggyState playerGroggyState;
        [SerializeField]
        PlayerDeathState playerDeathState;
        

        public AudioSource playerAudioSource;

        #endregion

        #region Else Scripts
        Health health;
        #endregion

        delegate void DelegateGrounded();
        DelegateGrounded delegateGrounded;
        public delegate void DelegateJump();
        public DelegateJump delegateJump;

        public bool isCharged;
        public bool isGuard;
        public bool isStopped;
        private bool isInTalkArea;
        private bool isReadyTalk;
        public IDamageAble.DamageInfo damageInfo;

        Animator animator;
        [SerializeField]
        PlayerState playerState;

        private void Initialize()
        {
            StartCoroutine(IELifeCycle());
        }

        private IEnumerator IELifeCycle()
        {
            while (true)
            {
                if (isStopped)
                {
                    // FixedUpdate는 멈추지 않기 때문에 velocity가 남아있으면 안됨
                    velocity = Vector2.zero;
                    yield return null;
                    continue;
                }
                if (!LoadingManager.Instance.CheckIsLoadDone())
                {
                    velocity = Vector2.zero;
                    yield return null;
                    continue;
                }
                if(Time.timeScale == 0f)
                {
                    yield return null;
                    continue;
                }

                if (isGrounded)
                {
                    delegateGrounded?.Invoke();
                }
                if (!PlayerInputPart.Instance.isCanInput)
                {
                    ChargeDone();
                    playerChargeAttackState.ResetCharge();
                }
                switch (playerState)
                {
                    case PlayerState.Move:
                    case PlayerState.Dash:
                    case PlayerState.Guard:
                        playerChargeAttackState.UpdateChargeState();
                        break;
                    case PlayerState.ChargeAttack:
                        break;
                    default:
                        playerChargeAttackState.ResetCharge();
                        break;
                }
                stateMachine.StateFrameUpdate();

                yield return null;
            }
        }

        private void Awake()
        {
            #region Singleton
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                if(Instance != this)
                {
                    Destroy(this.gameObject);
                }
            }
            #endregion

            animator = GetComponent<Animator>();
            health = GetComponent<Health>();

            stateMachine = GetComponent<StateMachine>();

            playerMoveState.Initialize(this);
            playerAttackState.Initialize(this);
            playerDashState.Initialize(this);
            playerDashAttackState.Initialize(this, playerDashState, playerAttackState);
            playerJumpAttackState.Initialize(this);
            playerChargeAttackState.Initialize(this);
            playerDamagedState.Initialize(this);
            playerGuardState.Initialize(this);
            playerHealState.Initialize(this);
            playerGroggyState.Initialize(this);
            playerDeathState.Initialize(this);

            PlayerInputPart.Instance.EventGuardKeyDown += playerGuardState.GuardKeyDown;
            PlayerInputPart.Instance.EventGuardKeyUp += playerGuardState.GuardKeyUp;
            PlayerInputPart.Instance.EventTalkKeyDown += TalkKeyDown;
            PlayerInputPart.Instance.EventGuardKeyUp += TalkKeyUp;

            delegateGrounded += playerDashState.ResetDashCount;
            delegateGrounded += playerJumpAttackState.ResetCanJumpAttack;

            delegateJump += playerJumpAttackState.ResetCanJumpAttack;

            stateMachine.InitState(playerMoveState);

            playerAudioSource = GetComponent<AudioSource>();

            Initialize();
        }

        protected override void Start()
        {
            base.Start();
        }

        public void OnDisable()
        {

        }

        protected override void ComputeVelocity()
        {
            stateMachine.StatePhysicsUpdate();
        }

        protected override void FixedUpdate()
        {
            if (isStopped)
                return;

            base.FixedUpdate();
        }

        public void ChargeDone()
        {
            isCharged = false;
        }

        public void GetDamage(float _hpDelta, Vector2 _direction)
        {
            var isInvincible = health.CheckInvincible();
            if (isInvincible)
            {
                Debug.Log("무적일 때 닿음");
                return;
            }

            damageInfo.isDamaged = true;
            damageInfo.hpDelta = _hpDelta;
            damageInfo.knockbackDirection = _direction;
        }

        public void ResetDamage()
        {
            damageInfo.isDamaged = false;
            damageInfo.hpDelta = 0;
            damageInfo.knockbackDirection = Vector2.zero;
            damageInfo.hitType = IDamageAble.HitType.Normal;
        }

        public void LookRight()
        {
            var scaleX = transform.localScale.x;
            
            if(scaleX < 0)
                scaleX = -scaleX;

            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
            VirtualCameraControl.Instance.TurnCamera(1);
        }

        public void LookLeft()
        {
            var scaleX = transform.localScale.x;

            if (scaleX > 0)
                scaleX = -scaleX;

            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
            VirtualCameraControl.Instance.TurnCamera(-1);
        }

        public void SetAnimatorTrigger(string triggerName)
        {
            animator.SetTrigger(triggerName);
        }

        public void SetAnimatorBool(string boolName, bool value)
        {
            animator.SetBool(boolName, value);
        }

        public void SetAnimatorFloat(string floatName, float value)
        {
            animator.SetFloat(floatName, value);
        }

        public void ResetAnimator()
        {
            animator.Rebind();
            animator.Update(0f);
        }

        public void PauseAnimator()
        {
            animator.Rebind();
            animator.Update(0f);
            animator.speed = 0f;
        }

        public void PlayAnimator()
        {
            animator.Rebind();
            animator.Update(0f);
            animator.speed = 1f;
        }

        public void ApplyRootMotionTemp()
        {
            animator.applyRootMotion = true;

            StartCoroutine(IEUnApplyRootMotion());

            IEnumerator IEUnApplyRootMotion()
            {
                yield return null;
                animator.applyRootMotion = false;
            }
        }

        public void ChangeStateOfStateMachine(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Move:
                    stateMachine.ChangeState(playerMoveState);
                    playerState = PlayerState.Move;
                    break;
                case PlayerState.Attack:
                    if (playerAttackState.CheckCanAttack())
                    {
                        if (isGrounded)
                        {
                            stateMachine.ChangeState(playerAttackState);
                            playerState = PlayerState.Attack;
                        }
                    }
                    break;
                case PlayerState.Dash:
                    if (playerDashState.CheckCanDash())
                    {
                        playerDashState.SetMoveVec();
                        stateMachine.ChangeState(playerDashState);
                        playerState = PlayerState.Dash;
                    }
                    break;
                case PlayerState.DashAttack:
                    playerDashAttackState.SetBackStep(playerDashState.CheckBackStep());
                    stateMachine.ChangeState(playerDashAttackState);
                    playerState = PlayerState.DashAttack;
                    break;
                case PlayerState.JumpAttack:
                    if (playerJumpAttackState.CheckCanAttack())
                    {
                        stateMachine.ChangeState(playerJumpAttackState);
                        playerState = PlayerState.JumpAttack;
                    }
                    break;
                case PlayerState.ChargeAttack:
                    stateMachine.ChangeState(playerChargeAttackState);
                    playerState = PlayerState.ChargeAttack;
                    break;
                case PlayerState.Damaged:
                    stateMachine.ChangeState(playerDamagedState);
                    playerState = PlayerState.Damaged;
                    break;
                case PlayerState.Guard:
                    stateMachine.ChangeState(playerGuardState);
                    playerState = PlayerState.Guard;
                    break;
                case PlayerState.Heal:
                    stateMachine.ChangeState(playerHealState);
                    playerState = PlayerState.Heal;
                    break;
                case PlayerState.Groggy:
                    stateMachine.ChangeState(playerGroggyState);
                    playerState = PlayerState.Groggy;
                    break;
                case PlayerState.Death:
                    stateMachine.ChangeState(playerDeathState);
                    playerState = PlayerState.Death;
                    break;
                default:
                    break;
            }
        }

        #region External bool control
        public void InTalkArea()
        {
            isInTalkArea = true;
        }

        public void OutTalkArea()
        {
            isInTalkArea = false;
            isReadyTalk = false;
        }

        public bool CheckReadyTalk()
        {
            if (isReadyTalk)
            {
                isReadyTalk = false;
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Key Event
        public void TalkKeyDown()
        {
            if (isInTalkArea)
            {
                isReadyTalk = true;
            }
        }

        public void TalkKeyUp()
        {
            isReadyTalk = false;
        }

        #endregion

        public enum PlayerState
        {
            Move,
            Dash,
            Attack,
            DashAttack,
            JumpAttack,
            ChargeAttack,
            Guard,
            Damaged,
            Heal,
            Groggy,
            Death,
        }
    }
}
