using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Boss_남궁 : Boss
    {
        public PlayerWithStateMachine player;

        #region states
        [SerializeField]
        Boss_남궁_MoveState moveState;
        // 공격
        // 피격
        // 그로기
        // 사망
        #endregion

        public StateMachine stateMachine;
        public Health health;
        public Animator animator;
        public AudioSource audioSource;

        public bool isStopped;

        public IDamageAble.DamageInfo damageInfo;

        private void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();

            player = PlayerWithStateMachine.Instance;

            moveState.Inintialize(this);

            stateMachine.InitState(moveState);

            Initialize();
        }
        protected override void Start()
        {
            base.Start();
        }

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
                if (Time.timeScale == 0f)
                {
                    yield return null;
                    continue;
                }

                stateMachine.StateFrameUpdate();

                yield return null;
            }
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

        public void LookRight()
        {
            var scaleX = transform.localScale.x;

            if (scaleX < 0)
                scaleX = -scaleX;

            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }

        public void LookLeft()
        {
            var scaleX = transform.localScale.x;

            if (scaleX > 0)
                scaleX = -scaleX;

            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }

        public void GetDamage(float _hpDelta, Vector2 _direction)
        {

        }

        public void ResetDamage()
        {
            damageInfo.isDamaged = false;
            damageInfo.hpDelta = 0;
            damageInfo.knockbackDirection = Vector2.zero;
            damageInfo.hitType = IDamageAble.HitType.Normal;
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

        /*public virtual void ChangeStateOfStateMachine(MobState state)
        {
            switch (state)
            {
                case MobState.Move:
                    //stateMachine.ChangeState();
                    //playerState = PlayerState.Move;
                    break;
            }
        }*/
    }
}
