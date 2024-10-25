using ActionPart.MemoryPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlayerGroggyState : State
    {
        #region parameters
        private PlayerWithStateMachine player;
        private Health health;

        [SerializeField]
        private float moreDamageRate;
        
        [SerializeField]
        private float groggyGaugeMax;
        [SerializeField]
        private float currentGroggyGauge;
        [SerializeField]
        private float perGauge;

        [SerializeField]
        private float keyDelayTime;
        private float keyDelayTimer;

        [SerializeField]
        private bool canKeyInput;
        [SerializeField]
        private bool isLeftKeyTurn;

        [SerializeField]
        private float waitTime;
        private float waitTimer;

        [SerializeField]
        private GroggyState groggyState;
        [SerializeField]
        private PlayerGroggyBar playerGroggyBar;
        #endregion

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
            health = gameObject.GetComponent<Health>();
        }

        public override void EnterState()
        {
            health.StopRecoveryStamina();
            var hittedEffect = ObjectPoolManager.Instance.GetObject("Player_GuardBreak_Effect");
            hittedEffect.transform.position = gameObject.transform.position;
            groggyState = GroggyState.GroggyStart;
            base.EnterState();
        }

        public override void ExitState()
        {
            health.Heal_Stamina(health.GetMaxStamina());
            health.UnStopRecoveryStamina();
            base.ExitState();
        }
 
        public override void FrameUpdate()
        {
            GetDamageInfo();
            UpdateGroggyState();
            base.FrameUpdate();
        }

        public override void PhysicsUpdate()
        {
            player.velocity.x = 0f;
            player.velocity.y = 0f;
            base.PhysicsUpdate();
        }

        void GetDamageInfo()
        {
            if (player.damageInfo.isDamaged)
            {
                player.damageInfo.hpDelta *= moreDamageRate;

                player.damageInfo.hitType = IDamageAble.HitType.Special;

                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
        }

        void UpdateGroggyState()
        {
            switch (groggyState)
            {
                case GroggyState.GroggyStart:
                    currentGroggyGauge = 0f;
                    keyDelayTimer = 0f;
                    player.SetAnimatorTrigger("isGroggyStart");
                    player.SetAnimatorBool("isGroggy", true);
                    isLeftKeyTurn = true;
                    playerGroggyBar.gameObject.SetActive(true);
                    playerGroggyBar.Reset();
                    groggyState = GroggyState.Grogging;
                    break;
                case GroggyState.Grogging:
                    CheckArrowKey();
                    if(currentGroggyGauge >= groggyGaugeMax)
                    {
                        waitTimer = 0f;
                        groggyState = GroggyState.PrepareStateOut;
                        player.SetAnimatorBool("isGroggy", false);
                    }
                    break;
                case GroggyState.PrepareStateOut:
                    waitTimer += Time.deltaTime;
                    if(waitTimer > waitTime)
                    {
                        playerGroggyBar.gameObject.SetActive(false);
                        // 스테미나 풀 회복
                        player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
                    }
                    break;
            }
        }

        void CheckArrowKey()
        {
            if (!canKeyInput)
            {
                keyDelayTimer += Time.deltaTime;
                if(keyDelayTimer > keyDelayTime)
                {
                    keyDelayTimer = 0f;
                    canKeyInput = true;
                }
            }
            if (canKeyInput)
            {
                var arrowKeyX = PlayerInputPart.Instance.inputVec.x;
                if(isLeftKeyTurn && arrowKeyX == -1)
                {
                    currentGroggyGauge += perGauge;
                    isLeftKeyTurn = false;
                    canKeyInput = false;
                    playerGroggyBar.ChangeProgress(currentGroggyGauge / groggyGaugeMax, isLeftKeyTurn);
                }
                else if(!isLeftKeyTurn && arrowKeyX == 1)
                {
                    currentGroggyGauge += perGauge;
                    isLeftKeyTurn = true;
                    canKeyInput = false;
                    playerGroggyBar.ChangeProgress(currentGroggyGauge / groggyGaugeMax, isLeftKeyTurn);
                }
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
