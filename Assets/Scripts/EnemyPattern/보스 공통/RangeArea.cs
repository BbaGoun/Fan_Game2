using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class RangeArea : MonoBehaviour
    {
        public bool isPlayerIn;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                isPlayerIn = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                isPlayerIn = false;
            }
        }


    }
}
