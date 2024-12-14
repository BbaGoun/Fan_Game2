using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class LoadingScene : MonoBehaviour
    {
        [SerializeField]
        private Image wipeLeft;
        [SerializeField]
        private Image wipeRight;
        [SerializeField]
        private Animator loadingMascot;
        [SerializeField]
        private Slider loadingBar;
        [SerializeField]
        private float wipeOpacity;

        private Coroutine coroutine;
        bool isDone;
        public bool CheckisDone()
        {
            if (isDone)
            {
                isDone = false;
                return true;
            }
            else
                return false;
        }

        public void LoadingObjectsOn()
        {
            loadingMascot.gameObject.SetActive(true);
            loadingMascot.Rebind();
            loadingMascot.Update(0f);

            loadingBar.gameObject.SetActive(true);
        }

        public void LoadingObjectOff()
        {
            loadingMascot.gameObject.SetActive(false);
            loadingBar.gameObject.SetActive(false);
        }

        public void LoadingProgressApply(float value)
        {
            loadingBar.value = value;
        }

        public void DirectIn()
        {
            wipeLeft.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            isDone = true;
        }

        public void DirectOut()
        {
            wipeLeft.rectTransform.anchoredPosition = new Vector2(-2500f, 0f);
            isDone = true;
        }

        public void FromLeftWipeIn(float duration)
        {
            isDone = false;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(IEFromLeftWipeIn(duration));


            IEnumerator IEFromLeftWipeIn(float duration)
            {
                wipeLeft.rectTransform.anchoredPosition = new Vector2(-2500f, 0f);

                var moveGap = 2500 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    wipeLeft.rectTransform.anchoredPosition += Vector2.right * moveGap;
                    yield return new WaitForSeconds(timeGap);
                }
                isDone = true;
            }
        }

        public void FromRightWipeIn(float duration)
        {
            isDone = false;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(IEFromRightWipeIn(duration));


            IEnumerator IEFromRightWipeIn(float duration)
            {

                wipeRight.rectTransform.anchoredPosition = new Vector2(0f, 0f);

                var moveGap = 2500 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    wipeRight.rectTransform.anchoredPosition -= Vector2.right * moveGap;
                    yield return new WaitForSeconds(timeGap);
                }
                isDone = true;
            }
        }

        public void ToLeftWipeOut(float duration)
        {
            isDone = false;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(IEToLeftWipeOut(duration));


            IEnumerator IEToLeftWipeOut(float duration)
            {

                wipeLeft.rectTransform.anchoredPosition = new Vector2(0f, 0f);

                var moveGap = 2500 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    wipeLeft.rectTransform.anchoredPosition -= Vector2.right * moveGap;
                    yield return new WaitForSeconds(timeGap);
                }
                isDone = true;
            }
        }

        public void ToRightWipeOut(float duration)
        {
            isDone = false;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(IEToRightWipeOut(duration));


            IEnumerator IEToRightWipeOut(float duration)
            {

                wipeRight.rectTransform.anchoredPosition = new Vector2(-2500f, 0f);

                var moveGap = 2500 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    wipeRight.rectTransform.anchoredPosition += Vector2.right * moveGap;
                    yield return new WaitForSeconds(timeGap);
                }
                isDone = true;
            }
        }
    }
}
