using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ActionPart
{
    public class Boss_남궁_DeathState : State
    {
        Boss_남궁 boss;

        public void Initialize(Boss_남궁 _boss)
        {
            boss = _boss;
        }

        public override void EnterState()
        {
            // 사밍 시 할 거
            boss.SetAnimatorTrigger("isDead");
            boss.harmfulToPlayer.SetIsWork(false);
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void FrameUpdate()
        {
            base.FrameUpdate();
        }

        public override void PhysicsUpdate()
        {
            boss.velocity.x = 0f;
            boss.velocity.y = 0f;
        }
    }
}
