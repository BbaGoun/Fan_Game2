using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlatformGround : MonoBehaviour
    {
        PlayerWithStateMachine player;
        BoxCollider2D box;
        bool isAbove;

        private void Awake()
        {
            player = PlayerWithStateMachine.Instance;
            box = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            var playerBottomSideY = player.transform.localPosition.y + 0.07f - 1.9f / 2 * player.transform.localScale.y;
            var platformUpSideY = transform.localPosition.y + box.offset.y + box.size.y / 2 * transform.localScale.y;
            if (playerBottomSideY >= platformUpSideY - 0.01f)
                isAbove = true;
            else
                isAbove = false;

            // ������ �� Ȱ��ȭ
            if (player.velocity.y <= 0f && isAbove)
            {
                box.isTrigger = false;
            }
            // �ö� �� ��Ȱ��ȭ
            else
            {
                box.isTrigger = true;
            }
        }
    }
}
