using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActionPart.Mob;

namespace ActionPart
{
    public class Boss : KinematicObject, IWithStateMachine, IDamageAble
    {
        public StateMachine stateMachine;
        public Health health;
        public Animator animator;

        Coroutine lifeCoroutine;

        #region Flags
        public bool isStopped;
        #endregion

        public IDamageAble.DamageInfo damageInfo;

        private void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
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
    }
}
