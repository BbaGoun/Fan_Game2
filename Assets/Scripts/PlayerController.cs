using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace ActionPart
{
    public class PlayerController : KinematicObject
    {
        
        #region Variables

        [Header("Move Parameter")]
        [SerializeField]
        // 이동속도
        private float moveSpeed;
        private Vector2 inputVec;
        private Vector2 moveVec;
        // 바라보는 방향
        private bool isLookRight;
        [SerializeField]
        private float acceleration;
        [SerializeField]
        private float deceleration;
        [SerializeField]
        private float velPower;
        [SerializeField]
        private float frictionAmount;

        [Header("Jump Parameter")]
        [SerializeField]
        private bool jumpStarted;
        [SerializeField]
        private bool jumpPerformed;
        [SerializeField]
        private bool jumpCanceled;
        [SerializeField]
        private float jumpForce; // 점프력
        [SerializeField]
        private float doubleJumpForce; // 더블 점프력
        [SerializeField]        
        private int jumpCount; // 현재 남은 점프 횟수
        [SerializeField]
        private int maxJumpCount; // 최대 점프 횟수
        [SerializeField]        
        private float minJumpDuration; // 최소 점프 지속 시간
        private float jumpTimer; // 현재 점프 지속 시간
        [SerializeField, Range(-100, -9.81f)]
        private float maxFallSpeed;
        [SerializeField]
        private float gravityModifier;
        [SerializeField]
        private float jumpHoldGravity;
        [SerializeField]
        private float jumpCancelGravity;
        [SerializeField]
        private JumpState jumpState;

        [Header("Attack Parameter")]
        [SerializeField]
        private bool attackStarted;
        [SerializeField]
        private bool attackCanceled;
        [SerializeField]
        private AttackState attackState;
        [SerializeField]
        private float attackTimer;
        [SerializeField]
        private float attackCooltime; // 공격 쿨타임
        [SerializeField]
        private float attackChargeTime; // 차지 공격을 위한 충전 시간
        private SpriteRenderer attackSprite; // 임시 공격 이팩트

        [Header("Dash Parameter")]
        [SerializeField]
        private bool dashStarted;
        [SerializeField]
        private DashState dashState;
        [SerializeField]
        private float dashSpeed;
        [SerializeField]
        private float dashAcceleration;
        [SerializeField]
        private float dashDeceleration;
        [SerializeField]
        private float dashVelPower;
        [SerializeField]
        private float dashDuration;
        [SerializeField]
        private float dashCoolTime;
        private float dashTimer;

        [Header("Hurt Parameter")]
        [SerializeField]
        private Vector2 knockbackVec;
        private Health health;


        [Header("Skill Parameter")]
        [SerializeField]
        private bool skillStarted;

        [Header("Animation Parameter")]
        private Animator animator;

        [Header("Audio Parameter")]
        private AudioClip sampleClip;

        [Header("States")]
        [SerializeField]
        private bool isDead;
        [SerializeField]
        private bool isStopped;
        [SerializeField]
        private bool isControlEnabled;
        

        [Header("Debug")]
        [SerializeField]
        private bool isWallStuck;

        private Rigidbody2D rb;
        Coroutine playerCycle;

        // 카메라 관련
        VirtualCameraControl vcControl;
        #endregion

        private void Awake()
        {
            health = GetComponent<Health>();
            rb = GetComponent<Rigidbody2D>();
            // 플레이어 정보 가져오기 부분 필요
            maxJumpCount = 2;
            jumpCount = maxJumpCount;

            // 공격 관련 초기화
            attackSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
            
            animator = GetComponent<Animator>();
            vcControl = GameObject.Find("Virtual Camera").GetComponent<VirtualCameraControl>();
        }

        protected override void Start()
        {
            LookRight();
            isControlEnabled = true;
            attackCanceled = true;
            playerCycle = StartCoroutine(PlayerCycle());
            base.Start();
        }

        IEnumerator PlayerCycle()
        {
            while (!isDead)
            {
                if (isDead)
                    break;

                if (isStopped)
                {
                    yield return null;
                    continue;
                }

                if (isControlEnabled)
                {
                    ControlDash();
                    UpdateDashState();

                    if(dashState == DashState.Idle || dashState == DashState.CoolTime)
                    {
                        ControlMoving();
                        UpdateJumpState();

                        ControlAttack();
                        UpdateAttackState();

                        if (attackState == AttackState.Idle || attackState == AttackState.Cooltime)
                        {
                            //ControlGuard();
                            //UpdateGuardState();
                        }
                    }
                }
                yield return null;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        void ControlDash()
        {
            if (dashState == DashState.Idle && dashStarted)
            {
                dashStarted = false;
                dashState = DashState.PrepareDash;
            }
        }

        void ControlMoving()
        {
            moveVec = inputVec;

            #region Look
            if (moveVec.x > 0.01f && !isLookRight)
            {
                // 왼쪽에서 오른쪽으로 회전
                Flip();
                animator.SetBool("isTurn", true);
                animator.SetBool("isMove", false);
            }
            else if (moveVec.x < -0.01f && isLookRight)
            {
                // 오른쪽에서 왼쪽으로 회전
                Flip();
                animator.SetBool("isTurn", true);
                animator.SetBool("isMove", false);
            }
            else if(Mathf.Abs(moveVec.x) > 0.01f)
            {
                // 가는 방향 그대로
                animator.SetBool("isTurn", false);
                animator.SetBool("isMove", true);
            }
            else
            {
                // 입력키 멈춤
                animator.SetBool("isTurn", false);
                animator.SetBool("isMove", false);
            }

            #endregion

            #region Jump
            if (jumpState == JumpState.Grounded && !isGrounded)
            {
                jumpState = JumpState.Falling;
            }
            else if (jumpState == JumpState.Grounded && jumpStarted && jumpCount > 0)
            {
                jumpState = JumpState.PrepareToJump;
            }
            else if (jumpState == JumpState.Falling && jumpStarted && jumpCount > 0)
            {
                jumpState = JumpState.PrepareToDoubleJump;
            }
            #endregion
        }

        void ControlAttack()
        {
            if (attackState == AttackState.Idle && attackStarted)
            {
                attackStarted = false;
                attackState = AttackState.Attack;
            }
            else if (attackState == AttackState.Idle && !attackCanceled)
            {
                attackState = AttackState.PrepareChargeAttack;
            }
            else if (attackState == AttackState.PrepareChargeAttack && attackCanceled)
            {
                if(attackTimer >= attackChargeTime)
                {
                    attackTimer = 0f;
                    attackState = AttackState.ChargeAttack;
                }
                else
                {
                    attackTimer = 0f;
                    attackState = AttackState.Idle;
                }
            }
            // 차지공격 관련 코드
        }

        void UpdateDashState()
        {
            switch (dashState)
            {
                case DashState.Idle:
                    break;
                case DashState.PrepareDash:
                    animator.SetBool("isDash", true);
                    dashTimer = 0f;
                    dashState = DashState.Dashing;
                    break;
                case DashState.Dashing:
                    dashTimer += Time.deltaTime;
                    if(dashTimer >= dashDuration)
                    {
                        animator.SetBool("isDash", false);
                        dashTimer = 0f;
                        dashState = DashState.CoolTime;
                    }
                    break;
                case DashState.CoolTime:
                    dashTimer += Time.deltaTime;
                    if (dashTimer >= dashCoolTime)
                    {
                        dashTimer = 0f;
                        dashState = DashState.Idle;
                    }
                    break;
            }
        }

        void UpdateJumpState()
        {
            switch(jumpState)
            {
                case JumpState.Falling:
                    jumpCanceled = false;
                    jumpCount = Mathf.Min(jumpCount, maxJumpCount - 1);
                    
                    if (isGrounded)
                        jumpState = JumpState.Landed;
                    break;

                case JumpState.PrepareToJump:
                    jumpStarted = false;
                    jumpPerformed = true;
                    jumpCanceled = false;
                    jumpCount--;
                    velocity.y = jumpForce;
                    jumpTimer = 0f;
                    // 점프 관련 행동 추가 (사운드, 이펙트)
                    animator.SetTrigger("isJump");
                    jumpState = JumpState.Jumping;
                    break;
                
                case JumpState.Jumping:
                    
                    if (jumpCanceled && jumpTimer >= minJumpDuration)
                    {
                        jumpPerformed = false;
                        jumpCanceled = false;
                    }
                    else if (isHeading)
                    {
                        velocity.y = Mathf.Min(velocity.y, 0f);
                    }

                    jumpTimer += Time.deltaTime;

                    if (velocity.y < 0f)
                        jumpState = JumpState.Falling;
                    break;

                case JumpState.PrepareToDoubleJump:
                    jumpStarted = false;
                    jumpPerformed = true;
                    jumpCanceled = false;
                    jumpCount--;
                    velocity.y = doubleJumpForce;
                    jumpTimer = 0f;
                    // 더블 점프 관련 행동 추가
                    animator.SetTrigger("isDoubleJump");
                    jumpState = JumpState.Jumping;
                    break;

                case JumpState.Landed:
                    jumpStarted = false;
                    jumpPerformed = false;
                    jumpCanceled = false;
                    // 착지 관련 행동 추가
                    jumpCount = maxJumpCount;
                    jumpState = JumpState.Grounded;
                    break;

                case JumpState.Grounded:
                    velocity.y = Physics2D.gravity.y;
                    break;
            }
        }

        void UpdateAttackState()
        {
            switch (attackState)
            {
                case AttackState.Idle:
                    break;
                case AttackState.Attack:
                    Attack();
                    animator.SetTrigger("isAttack");
                    attackTimer = 0f;
                    attackState = AttackState.Cooltime;
                    break;
                case AttackState.PrepareChargeAttack:
                    // 이팩트
                    attackTimer += Time.deltaTime;
                    if(attackTimer < attackChargeTime)
                    {
                        // 충전 전 이펙트
                    }
                    else
                    {
                        // 충전 후 이팩트
                    }
                    break;
                case AttackState.ChargeAttack:
                    ChargeAttack();
                    attackState = AttackState.Cooltime;
                    break;
                case AttackState.Cooltime:
                    attackTimer += Time.deltaTime;
                    if(attackTimer > attackCooltime)
                    {
                        attackTimer = 0f;
                        attackState = AttackState.Idle;
                    }
                    break;
            }
        }

        public bool GetDamage(int damage)
        {
            //return health.Hurt(damage, 0f);
            return false;
        }


        void Attack()
        {
            attackSprite.gameObject.SetActive(true);
            attackSprite.transform.localScale = Vector3.one;
            attackSprite.color = Color.white;
        }

        void ChargeAttack()
        {
            attackSprite.gameObject.SetActive(true);
            attackSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            attackSprite.color = Color.red;
        }

        protected override void ComputeVelocity()
        {
            animator.SetBool("isGrounded", isGrounded);

            #region x 관련
            if(dashState == DashState.Dashing)
            {
                float targetSpeed = transform.localScale.x * dashSpeed;

                float speedDif = targetSpeed - velocity.x;

                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? dashAcceleration : dashDeceleration;

                if(dashTimer >= dashDuration/2)
                {
                    accelRate /= 2;
                }

                float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, dashVelPower) * Mathf.Sign(speedDif);

                velocity.x += movement * Time.deltaTime;

                velocity.y = 0f;
            }
            else
            {
                float targetSpeed = moveVec.x * moveSpeed;

                float speedDif = targetSpeed - velocity.x;

                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

                float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

                velocity.x += movement * Time.fixedDeltaTime;
                
                // 멈추려고 하는 경우
                if(isGrounded && Mathf.Abs(moveVec.x) < 0.01f)
                {
                    float amount = Mathf.Min(Mathf.Abs(velocity.x), Mathf.Abs(frictionAmount));

                    amount *= Mathf.Sign(velocity.x);

                    velocity.x += -amount * Time.fixedDeltaTime;
                }
                #endregion

                #region y 관련
                // 점프 관련
                switch (jumpState)
                {
                    case JumpState.Falling:
                        velocity.y = Mathf.Max(velocity.y + gravityModifier * Physics2D.gravity.y * Time.deltaTime, maxFallSpeed);
                        break;
                    case JumpState.Jumping:
                        if (jumpPerformed)
                            velocity.y += jumpHoldGravity * Physics2D.gravity.y * Time.deltaTime;
                        else
                            velocity.y += jumpCancelGravity * Physics2D.gravity.y * Time.deltaTime;
                        break;
                    default:
                        break;
                }

                animator.SetFloat("velocityY", velocity.y);
                #endregion
            }

        }

        private void Flip()
        {
            isLookRight = !isLookRight;
            var scaleX = isLookRight ? 1 : -1;
            transform.localScale = new Vector3(scaleX, 1, 1);
            vcControl.TurnCamera(scaleX);
        }

        private void LookRight()
        {
            isLookRight = true;
            transform.localScale = new Vector3(1, 1, 1);
            vcControl.TurnCamera(1);
        }

        private void LookLeft()
        {
            isLookRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
            vcControl.TurnCamera(-1);
        }

        #region InvokeInput
        public void ActionMove(InputAction.CallbackContext context)
        {
            moveVec = Vector3.zero;
            inputVec = context.ReadValue<Vector2>();
        }

        public void ActionJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                jumpStarted = true;
                jumpCanceled = false;
            }
            else if (context.canceled)
            {
                jumpStarted = false;
                jumpCanceled = true;
            }
        }

        public void ActionAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                attackStarted = true;
                attackCanceled = false;
            }
            else if (context.canceled)
            {
                attackStarted = false;
                attackCanceled = true;
            }
        }

        public void ActionDash(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                dashStarted = true;
            }
            else if (context.performed)
            {

            }
            else if (context.canceled)
            {
                dashStarted = false;
            }
        }

        public void ActionSkill(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                skillStarted = true;
            }
            else if (context.performed)
            {

            }
            else if (context.canceled)
            {
                skillStarted = false;
            }
        }
        #endregion

        #region AnimatorControl

        #endregion

        private enum DashState
        {
            Idle,
            PrepareDash,
            Dashing,
            CoolTime,
        }

        private enum JumpState
        {
            Grounded,
            Falling,
            PrepareToJump,
            Jumping,
            PrepareToDoubleJump,
            Landed
        }

        private enum AttackState
        {
            Idle,
            Attack,
            PrepareChargeAttack,
            ChargeAttack,
            Cooltime,
        }
        
    }
}