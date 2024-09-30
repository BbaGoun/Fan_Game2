using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ActionPart
{
    public class PlayerHealState : State
    {
        PlayerWithStateMachine player;

        #region parameters
        [SerializeField]
        private float healTime;
        private float healTimer;
        [SerializeField]
        private float healAmount;
        [SerializeField]
        private float moreDamageRate;
        private HealState healState;
        [SerializeField]
        private float waitTime;
        private float waitTimer;

        // 이펙트 오브젝트 연결
        #endregion

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
        }

        public override void EnterState()
        {
            base.EnterState();
            healState = HealState.PrepareHeal;
            PlayerInputPart.Instance.EventHealKeyUp += HealKeyUp;
            
            // 이제 내부 코드, 흐름, 키 입력에 대한 부분을 만들어야함.
        }

        public override void ExitState()
        {
            PlayerInputPart.Instance.EventHealKeyUp -= HealKeyUp;
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change
            if (healState == HealState.Idle)
            {
                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
            }
            #endregion

            GetDamageInfo();
            UpdateHealState();
            base.FrameUpdate();
        }

        public override void PhysicsUpdate()
        {
            player.velocity.x = 0f;
            player.velocity.y = 0f;
            base.PhysicsUpdate();
        }


        public void GetDamageInfo()
        {
            if (player.damageInfo.isDamaged)
            {
                player.damageInfo.hpDelta *= moreDamageRate;

                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
        }

        void UpdateHealState()
        {
            switch(healState)
            {
                case HealState.Idle:
                    break;
                case HealState.PrepareCharge:
                    healTimer = 0f;
                    healState = HealState.Charging;
                    break;
                case HealState.Charging:
                    healTime += Time.deltaTime;
                    if(healTimer > healTime)
                    {
                        healState = HealState.PrepareHeal;
                    }
                    break;
                case HealState.Heal:
                    waitTimer = 0f;
                    healState = HealState.PrepareIdle;
                    break;
                case HealState.PrepareIdle:
                    waitTimer += Time.deltaTime;
                    if(waitTimer > waitTime)
                    {
                        healState = HealState.Idle;
                    }
                    break;
            }
        }

        #region Key Event
        void HealKeyUp()
        {
            waitTimer = 0f;
            healState = HealState.PrepareIdle;
        }
        #endregion

        enum HealState
        {
            Idle,
            PrepareCharge,
            Charging,
            PrepareHeal,
            Heal,
            PrepareIdle,
        }
    }
}
