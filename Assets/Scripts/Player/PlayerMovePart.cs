using ActionPart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovePart : KinematicObject
{
    /*PlayerInputPart inputPart;

    [Header("Move Parameter")]
    [SerializeField]
    // 이동속도
    private float moveSpeed;
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

    VirtualCameraControl vcControl;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        inputPart = PlayerInputPart.Instance;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public bool CheckWillMove()
    {
        return inputPart.inputVec.x != 0f;
    }

    public bool CheckWillJump()
    {
        if (jumpState == JumpState.Landed || jumpState == JumpState.Grounded)
            return false;
        return true;
    }

    public bool CheckWillDash()
    {
        if (dashState == DashState.Idle || dashState == DashState.CoolTime)
            return false;
        return true;
    }

    public void DoMovePart(bool isControllable)
    {
        if (isControllable)
            ControlDash();
        UpdateDashState();

        if (dashState == DashState.Idle || dashState == DashState.CoolTime)
        {
            if (isControllable)
                ControlMoving();

            if (isControllable)
                ControlJump();
            UpdateJumpState();
        }
    }

    public bool isCanDoOtherThings()
    {
        switch (dashState)
        {
            case DashState.PrepareDash:
                return false;
            case DashState.Dashing:
                return false;
            default:
                break;
        }
        return false;
    }

    void ControlDash()
    {
        dashStarted = inputPart.dashStarted;

        if (dashState == DashState.Idle && dashStarted)
        {
            dashStarted = false;
            dashState = DashState.PrepareDash;
        }
    }

    void ControlMoving()
    {
        moveVec = inputPart.inputVec;

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
        else if (Mathf.Abs(moveVec.x) > 0.01f)
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
    }

    void ControlJump()
    {
        jumpStarted = inputPart.jumpStarted;
        jumpCanceled = inputPart.jumpCanceled;

        if (jumpState == JumpState.Grounded && jumpStarted && jumpCount > 0)
        {
            jumpState = JumpState.PrepareToJump;
        }
        else if (jumpState == JumpState.Falling && jumpStarted && jumpCount > 0)
        {
            jumpState = JumpState.PrepareToDoubleJump;
        }
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
                if (dashTimer >= dashDuration)
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
        if (jumpState == JumpState.Grounded && !isGrounded)
        {
            jumpState = JumpState.Falling;
        }

        switch (jumpState)
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

    protected override void ComputeVelocity()
    {
        animator.SetBool("isGrounded", isGrounded);

        if (dashState == DashState.Dashing)
        {
            float targetSpeed = transform.localScale.x * dashSpeed;

            float speedDif = targetSpeed - velocity.x;

            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? dashAcceleration : dashDeceleration;

            if (dashTimer >= dashDuration / 2)
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
            if (isGrounded && Mathf.Abs(moveVec.x) < 0.01f)
            {
                float amount = Mathf.Min(Mathf.Abs(velocity.x), Mathf.Abs(frictionAmount));

                amount *= Mathf.Sign(velocity.x);

                velocity.x += -amount * Time.fixedDeltaTime;
            }

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

    #region States
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
    #endregion*/
}
