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
        [SerializeField]
        Boss_남궁_AttackState attackState;
        [SerializeField]
        Boss_남궁_DamagedState damagedState;
        // 피격
        // 그로기
        // 사망
        #endregion

        public RangeArea InRange;
        public RangeArea OutRange;

        public StateMachine stateMachine;
        public EnemyHealth enemyHealth;
        public Animator animator;
        public AudioSource audioSource;

        public bool isStopped;
        public bool isAttackSuperArmour;
        public bool isDamageSuperArmour;
        public int damageCountThreshold;
        private int damageCount;
        public int attackCountThreshold;
        private int attackCount;

        public IDamageAble.DamageInfo damageInfo;

        public BossState currentState;
        public float attackTimer;
        private float timer;

        private void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
            enemyHealth = GetComponent<EnemyHealth>();
            animator = GetComponent<Animator>();

            player = PlayerWithStateMachine.Instance;

            moveState.Inintialize(this);
            attackState.Initialize(this);
            damagedState.Initialize(this);

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

                if(currentState == BossState.Move)
                  timer += Time.deltaTime;
                if(timer > attackTimer)
                {
                    timer = 0f;
                    if (InRange.isPlayerIn)
                        ChangeStateOfStateMachine(BossState.Attack);
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

        public override void GetDamage(float _hpDelta, Vector2 _direction)
        {
            Debug.Log("아우 시발");

            var isInvincible = enemyHealth.CheckInvincible();
            if (isInvincible)
            {
                return;
            }

            if (CheckIsSuperArmour())
            {
                damagedState.JustDamage();
            }
            else
            {
                damageInfo.isDamaged = true;
                damageInfo.hpDelta = _hpDelta;
                damageInfo.knockbackDirection = _direction;
            }
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

        public virtual void ChangeStateOfStateMachine(BossState state)
        {
            switch (state)
            {
                case BossState.Move:
                    stateMachine.ChangeState(moveState);
                    break;
                case BossState.Attack:
                    stateMachine.ChangeState(attackState);
                    isAttackSuperArmour = true;
                    break;
                case BossState.Damaged:
                    stateMachine.ChangeState(damagedState);
                    break;
            }
            if(state != BossState.Attack)
            {
                isAttackSuperArmour = false;
            }
        }

        public void UpAttackCount()
        {
            // 공격을 몇 번 수행한 후 슈퍼아머가 풀림
            attackCount += 1;
            if(attackCount >= attackCountThreshold)
            {
                damageCount = 0;
                isDamageSuperArmour = false;
                attackCount = 0;
            }
        }

        public void UpDamageCount()
        {
            // 몇 대 이상 맞으면 슈퍼아머
            damageCount += 1;
            if (damageCount >= damageCountThreshold)
                isDamageSuperArmour = true;
        }

        public bool CheckIsSuperArmour()
        {
            return isAttackSuperArmour || isDamageSuperArmour;
        }

        public enum BossState
        {
            Move,
            Attack,
            Damaged,
            Groggy,
            Death
        }
    }
}
