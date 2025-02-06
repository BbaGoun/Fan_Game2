using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class Map_Button : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Image image;
        public Sprite mouseEnterSprite;
        public Sprite mouseExitSprite;

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            image.alphaHitTestMinimumThreshold = 0.5f;
            Color tempColor = Color.white;
            tempColor.a = 0f;
            image.color = tempColor;
        }

        public void OnMouseEnter()
        {
            image.sprite = mouseEnterSprite;
            Color tempColor = Color.white;
            tempColor.a = 1f;
            image.color = tempColor;
            rectTransform.localScale = Vector3.one;

        }

        public void OnMouseExit() 
        {
            image.sprite = mouseExitSprite;
            Color tempColor = Color.white;
            tempColor.a = 0f;
            image.color = tempColor;
            rectTransform.localScale = new Vector3(0.825f, 0.825f, 1f);
        }
    }
}
