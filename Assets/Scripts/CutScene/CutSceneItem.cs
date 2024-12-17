using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class CutSceneItem : MonoBehaviour
    {
        public Role role;
        public Vector2 movedPosition;
        public float moveDuration;
        public bool moveDone;

        public void Move()
        {/*
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.anchoredPosition = movedPosition;*/
            StartCoroutine(IEMove());
        }

        public IEnumerator IEMove()
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            Vector2 gap = (movedPosition - rt.anchoredPosition) / 100f; 
            for(int i = 0;i < 100; i++)
            {
                rt.anchoredPosition = rt.anchoredPosition + gap;
                yield return new WaitForSeconds(moveDuration / 100f);
            }
            moveDone = true;
        }

        public bool CheckMoveDone()
        {
            return moveDone;
        }

        public enum Role
        {
            None,
            Next,
            End,
            Move,
        }
    }
}
