using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlatformGround : MonoBehaviour
    {
        PlayerWithStateMachine player;
        BoxCollider2D box;

        private void Awake()
        {
            player = PlayerWithStateMachine.Instance;
            box = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if(player.velocity.y <= 0f)
            {
                box.enabled = true;
            }
            else if(player.velocity.y > 0f)
            {
                box.enabled = false;
            }
        }
    }
}
