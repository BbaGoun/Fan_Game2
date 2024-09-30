using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlayerDeathState : State
    {
        PlayerWithStateMachine playerWithStateMachine;
        public PlayerDeathState(PlayerWithStateMachine _playerWithStateMachine)
        {
            playerWithStateMachine = _playerWithStateMachine;
        }

        public override void EnterState()
        {
            base.EnterState();
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
            base.PhysicsUpdate();
        }

    }
}
