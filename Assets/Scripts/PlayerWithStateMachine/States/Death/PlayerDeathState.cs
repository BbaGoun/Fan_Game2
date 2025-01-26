using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionPart.MemoryPool;

namespace ActionPart
{
    public class PlayerDeathState : State
    {
        PlayerWithStateMachine player;
        public float slowTime;
        public float slowScale;
        float slowTimer;

        public void Initialize(PlayerWithStateMachine _playerWithStateMachine)
        {
            player = _playerWithStateMachine;
        }

        public override void EnterState()
        {
            var hittedEffect = ObjectPoolManager.Instance.GetObject("Player_Death_Effect");
            hittedEffect.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -5);
            hittedEffect.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1, 1);
            
            player.SetAnimatorTrigger("isDead");


            slowTimer = 0f;
            TimeController.Instance.SetTimeScale(slowScale);
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            if (TimeController.Instance.GetTimeScale() != 0f && TimeController.Instance.GetTimeScale() != 1f)
            {
                slowTimer += Time.unscaledDeltaTime;
                if (slowTimer >= slowTime)
                {
                    TimeController.Instance.SetTimeScale(1f);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            player.velocity.x = 0f;
            player.velocity.y = Physics2D.gravity.y;
        }

    }
}
