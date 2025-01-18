using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Boss_남궁_MoveState : State
    {
        Boss_남궁 boss;

        [SerializeField]
        AudioClip runAudio;

        #region parameters
        [Header("Speed Parameter")]
        [SerializeField]
        private float moveSpeed;
        [SerializeField]
        private float acceleration;
        [SerializeField]
        private float deceleration;
        [SerializeField]
        private float velPower;
        [SerializeField]
        private float frictionAmount;
        [SerializeField]
        private float slowMultiplier;
        public Vector2 moveVec;
        #endregion

        MoveState moveState;

        public void Inintialize(Boss_남궁 _boss)
        {
            boss = _boss;
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void ExitState()
        {
            base.ExitState();

            boss.SetAnimatorBool("isMove", false);
        }

        public override void FrameUpdate()
        {
            if (boss.damageInfo.isDamaged)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Damaged);
            }
            if (boss.isGroggy)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Groggy);
            }
            if (boss.isDeath)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Death);
            }

            XControl();
            UpdateMoveState();
        }

        void XControl()
        {
            moveVec = new Vector2(Mathf.Sign(boss.player.transform.position.x - this.transform.position.x), 0);

            if (boss.isStopped || Time.timeScale == 0f)
                moveVec = Vector2.zero;

            var isLookRight = Mathf.Sign(transform.localScale.x) == 1;

            if (moveVec.x < -0.01f && isLookRight)
            {
                //왼쪽으로 돌기
                //Debug.Log("왼쪽으로 돌기");
                /*if (player.isGrounded)
                    player.SetAnimatorTrigger("isTurn");*/
                boss.SetAnimatorBool("isMove", false);
                Flip(!isLookRight);
            }
            else if (moveVec.x > 0.01f && !isLookRight)
            {
                //오른쪽으로 돌기
                //Debug.Log("오른쪽으로 돌기");
                /*if (player.isGrounded)
                    player.SetAnimatorTrigger("isTurn");*/
                boss.SetAnimatorBool("isMove", false);
                Flip(!isLookRight);
            }
            else if (Mathf.Abs(moveVec.x) > 0.01f)
            {
                // 가는 방향 그대로
                boss.SetAnimatorBool("isMove", true);
            }
            else
            {
                // 가만히 서있음
                boss.SetAnimatorBool("isMove", false);
            }
        }

        void Flip(bool isLookRight)
        {
            if (isLookRight)
            {
                boss.LookRight();
            }
            else
            {
                boss.LookLeft();
            }
        }

        void UpdateMoveState()
        {
            if (!boss.InRange.isPlayerIn)
            {
                moveState = MoveState.InRangeOut;
                // 플레이어에게 다가오기
            }
            else if (boss.InRange.isPlayerIn && !boss.OutRange.isPlayerIn)
            {
                moveState = MoveState.InRangeIn;
                // 느리게 머뭇거리기
            }
        }

        public override void PhysicsUpdate()
        {
            if (moveState == MoveState.InRangeOut)
            {
                float targetSpeed = moveVec.x * moveSpeed;

                float speedDif = targetSpeed - boss.velocity.x;

                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

                float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

                boss.velocity.x += movement * Time.deltaTime;
            }
            else if (moveState == MoveState.InRangeIn)
            {
                float targetSpeed = moveVec.x * moveSpeed * slowMultiplier;

                float speedDif = targetSpeed - boss.velocity.x;

                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

                float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

                boss.velocity.x += movement * Time.deltaTime;
            }
            
            boss.velocity.y = Physics2D.gravity.y;
        }

        enum MoveState
        {
            InRangeOut,
            InRangeIn,
        }
    }
}
