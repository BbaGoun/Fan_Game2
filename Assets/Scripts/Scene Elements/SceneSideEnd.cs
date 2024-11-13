using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class SceneSideEnd : MonoBehaviour
    {
        public Transform outPoint;
        public SideDirection sideDirection;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (PlayerInputPart.Instance.isCanInput)
                {
                    var player = collision.GetComponent<PlayerWithStateMachine>();
                    player.playerMoveState.MoveXFromTo(player.transform, outPoint);
                }
            }
        }

        public enum SideDirection
        {
            Left,
            Right,
        }
    }
}
