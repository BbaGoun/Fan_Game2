using ActionPart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class HarmfulToPlayer : MonoBehaviour
    {
        [SerializeField]
        private float damage;
        private IDamageAble damageAble;

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                var player = collision.gameObject;

                if (damageAble == null)
                    damageAble = player.GetComponent<IDamageAble>();

                Vector2 direction = (player.transform.position - transform.position).normalized;

                damageAble.GetDamage(damage, direction);
            }
        }
    }
}
