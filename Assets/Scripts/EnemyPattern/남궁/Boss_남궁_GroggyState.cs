using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ActionPart
{
    public class Boss_남궁_GroggyState : State
    {
        #region parameters
        private Boss_남궁 boss;
        private EnemyHealth enemyHealth;

        [SerializeField]
        private float waitTime;
        [SerializeField, ReadOnly(true)]
        private float waitTimer;

        [SerializeField]
        private GroggyState groggyState;
        #endregion

        public void Initialize(Boss_남궁 _boss)
        {
            boss = _boss;
            enemyHealth = gameObject.GetComponent<EnemyHealth>();
        }

        public override void EnterState()
        {
            enemyHealth.StopRecoveryStamina();
            
            groggyState = GroggyState.GroggyStart;
        }

        public override void ExitState()
        {
            enemyHealth.Heal_Stamina(enemyHealth.GetMaxStamina());
            enemyHealth.UnStopRecoveryStamina();
        }

        public override void FrameUpdate()
        {
            if (boss.isDeath)
            {
                boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Death);
            }
            UpdateGroggyState();
        }

        public override void PhysicsUpdate()
        {
            boss.velocity.x = 0f;
            boss.velocity.y = 0f;
        }

        void UpdateGroggyState()
        {
            switch (groggyState)
            {
                case GroggyState.GroggyStart:
                    boss.SetAnimatorTrigger("isGroggyStart");
                    boss.SetAnimatorBool("isGroggy", true);
                    
                    groggyState = GroggyState.Grogging;
                    break;
                case GroggyState.Grogging:
                    // 대충 내용 기입
                    groggyState = GroggyState.PrepareStateOut;
                    break;
                case GroggyState.PrepareStateOut:
                    waitTimer += Time.deltaTime;
                    if (waitTimer > waitTime)
                    {
                        waitTimer = 0;
                        boss.isGroggy = false;
                        boss.ChangeStateOfStateMachine(Boss_남궁.BossState.Move);
                    }
                    break;
            }
        }

        enum GroggyState
        {
            GroggyStart,
            Grogging,
            PrepareStateOut,
        }
    }
}
