using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class PlayerCollision : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var layer = collision.gameObject.layer;

            switch(layer)
            {
                default:
                    break;
            }
        }
    }
}
