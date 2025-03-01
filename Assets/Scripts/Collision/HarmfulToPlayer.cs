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
        bool isWork = true;


        public void SetIsWork(bool _isWork)
        {
            isWork = _isWork;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (isWork)
            {
                if (collision.gameObject.tag == "Player")
                {
                    var player = collision.gameObject;

                    if (damageAble == null)
                        damageAble = player.GetComponent<IDamageAble>();

                    Vector2 direction = player.transform.position - transform.position;

                    damageAble.GetDamage(damage, direction);
                }
            }
        }
    }
}
