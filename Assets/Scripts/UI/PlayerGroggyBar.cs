using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class PlayerGroggyBar : MonoBehaviour
    {
        [SerializeField]
        private float changeDelay;
        private float progress;

        [SerializeField]
        private float startPoint;
        [SerializeField]
        private float endPoint;

        private RectTransform underBar;
        private RectTransform topBar;
        private RectTransform leftArrowKey;
        private RectTransform rightArrowKey;

        private Coroutine coroutineUnder;
        private Coroutine coroutineTop;

        private void Awake()
        {
            underBar = transform.GetChild(1).GetComponent<RectTransform>();
            topBar = transform.GetChild(2).GetComponent<RectTransform>();
            leftArrowKey = transform.GetChild(3).GetComponent<RectTransform>();
            rightArrowKey = transform.GetChild(4).GetComponent<RectTransform>();

            startPoint = Utility.GetRectRight(underBar);
            endPoint = -startPoint;

            if (this.gameObject.activeSelf)
                this.gameObject.SetActive(false);
        }

        public void Reset()
        {
            progress = 0f;
            Utility.SetRectRight(underBar, startPoint);
            Utility.SetRectRight(topBar, startPoint);
            Utility.SetRectWidthHeight(leftArrowKey, 1f, 1f);
            Utility.SetRectWidthHeight(rightArrowKey, 0.85f, 0.85f);
        }

        public void ChangeProgress(float changedProgress, bool isLeftKeyTurn)
        {
            if(coroutineUnder != null)
                StopCoroutine(coroutineUnder);
            if(coroutineTop != null)
                StopCoroutine(coroutineTop);

            if (isLeftKeyTurn)
            {
                Utility.SetRectWidthHeight(leftArrowKey, 1f, 1f);
                Utility.SetRectWidthHeight(rightArrowKey, 0.85f, 0.85f);
            }
            else
            {
                Utility.SetRectWidthHeight(leftArrowKey, 0.85f, 0.85f);
                Utility.SetRectWidthHeight(rightArrowKey, 1f, 1f);
            }

            coroutineUnder = StartCoroutine(IEChangeUnderBar(changedProgress));
            coroutineTop = StartCoroutine(IEChangeTopBar(changedProgress, changeDelay));

            progress = changedProgress;
        }

        IEnumerator IEChangeUnderBar(float changedProgress)
        {
            var start = progress;
            var end = changedProgress;
            var gap = (end - start) / 10f;
            var current = start;
            for(int i=1; i<=10; i++)
            {
                current = current + gap;
                var right = Mathf.Lerp(startPoint, endPoint, current);
                Utility.SetRectRight(underBar, right);
                yield return new WaitForSeconds(0.01f * Time.timeScale);
            }
        }

        IEnumerator IEChangeTopBar(float changedProgress, float delay)
        {
            yield return new WaitForSeconds(delay);

            var start = progress;
            var end = changedProgress;
            var gap = (end - start) / 10f;
            var current = start;
            for (int i = 1; i <= 10; i++)
            {
                current = current + gap;
                var right = Mathf.Lerp(startPoint, endPoint, current);
                Utility.SetRectRight(topBar, right);
                yield return new WaitForSeconds(0.01f * Time.timeScale);
            }
        }
    }
}
