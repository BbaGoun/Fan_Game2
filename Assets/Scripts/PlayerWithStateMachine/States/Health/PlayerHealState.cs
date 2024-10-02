using ActionPart.MemoryPool;
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
        [SerializeField]
        private HealState healState;
        [SerializeField]
        private float waitTime;
        private float waitTimer;

        Health health;

        [SerializeField]
        HealEffect healEffect;
        [SerializeField]
        GameObject hittedEffect;
        Coroutine hittedCoroutine;

        [SerializeField]
        float shakeIntensity;
        [SerializeField]
        float shakeFrequency;
        // 이펙트 오브젝트 연결
        #endregion

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
            health = gameObject.GetComponent<Health>();
        }

        public override void EnterState()
        {
            base.EnterState();
            healState = HealState.PrepareCharge;
            PlayerInputPart.Instance.EventHealKeyUp += HealKeyUp;

            player.SetAnimatorTrigger("isHealStart");
            healEffect.animator.SetBool("isHealState", true);
        }

        public override void ExitState()
        {
            PlayerInputPart.Instance.EventHealKeyUp -= HealKeyUp;
            VirtualCameraControl.Instance.SetShakeCameraDirect(0f, 0f);
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            #region State Change

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

                hittedEffect.SetActive(true);
                if(hittedCoroutine != null)
                    StopCoroutine(hittedCoroutine);
                hittedCoroutine = StartCoroutine(IEHittedEffect());

                player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Damaged);
            }
        }

        void UpdateHealState()
        {
            switch(healState)
            {
                case HealState.PrepareCharge:
                    healTimer = 0f;
                    healState = HealState.Charging;

                    VirtualCameraControl.Instance.SetShakeCameraDirect(shakeIntensity, shakeFrequency);

                    player.SetAnimatorBool("isHealing", true);
                    healEffect.animator.SetTrigger("isCharging");
                    break;
                case HealState.Charging:
                    healTimer += Time.deltaTime;
                    if(healTimer > healTime)
                    {
                        healEffect.animator.SetTrigger("isDone");
                        healState = HealState.Heal;
                    }
                    break;
                case HealState.Heal:
                    waitTimer = 0f;
                    healState = HealState.PrepareIdle;
                    health.Heal_HP(healAmount);
                    health.Heal_Stamina(healAmount);
                    break;
                case HealState.PrepareIdle:
                    player.SetAnimatorBool("isHealing", false);
                    healEffect.animator.SetBool("isHealState", false);
                    waitTimer += Time.deltaTime;
                    if(waitTimer > waitTime)
                    {
                        player.ChangeStateOfStateMachine(PlayerWithStateMachine.PlayerState.Move);
                    }
                    break;
            }
        }

        IEnumerator IEHittedEffect()
        {
            var timer = 0f;
            while (true)
            {
                timer += Time.deltaTime;
                if (timer > 0.5f)
                {
                    hittedEffect.SetActive(false);
                    break;
                }
                yield return null;
            }
            yield return null;
        }

        void ResetCharge()
        {
            healEffect.animator.Rebind();
            healEffect.animator.Update(0f);
        }

        #region Key Event
        void HealKeyUp()
        {
            if (healState != HealState.PrepareIdle)
            {
                waitTimer = 0f;
                healState = HealState.PrepareIdle;
            }
        }
        #endregion

        enum HealState
        {
            PrepareCharge,
            Charging,
            Heal,
            PrepareIdle,
        }
    }
}
