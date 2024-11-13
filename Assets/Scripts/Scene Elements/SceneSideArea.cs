using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class SceneSideArea : MonoBehaviour
    {
        private SpriteRenderer sprite;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                var alpha = Mathf.Clamp01(this.transform.localScale.x * (this.transform.localPosition.x - collision.transform.localPosition.x) / 3);

                sprite.color = new Color(1, 1, 1, 1 - alpha);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                sprite.color = new Color(1, 1, 1, 0);
            }
        }
    }
}
