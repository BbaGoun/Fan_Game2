using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Mob : KinematicObject, IWithStateMachine, IDamageAble
    {
        public StateMachine stateMachine;
        public Health health;
        public Animator animator;

        Coroutine lifeCoroutine;

        #region Flags
        public bool isStopped;
        #endregion

        public IDamageAble.DamageInfo damageInfo;
        public MobState state;
        public MobRole role;

        public float distance;

        private void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
        }

        public float GetDistance()
        {
            distance = Mathf.Abs(PlayerWithStateMachine.Instance.transform.localPosition.x - transform.localPosition.x);
            return distance;
        }

        public void ReadyAttack()
        {
            switch (state)
            {
                case MobState.Move:
                case MobState.Guard:
                    Attack();
                    break;
                default:
                    break;
            }
        }

        protected virtual void Attack()
        {

        }

        public void GetDamage(float _hpDelta, Vector2 _direction)
        {

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

        public virtual void ChangeStateOfStateMachine(MobState state)
        {
            switch (state)
            {
                case MobState.Move:
                    //stateMachine.ChangeState();
                    //playerState = PlayerState.Move;
                    break;
            }
        }

        public enum MobState
        {
            Move,
            Attack,
            Guard,
            Damaged,
            Groggy,
            Death,
        }

        public enum MobRole
        {
            Forward,
            Support,
        }
    }
}
