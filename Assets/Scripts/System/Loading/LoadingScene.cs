using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class LoadingScene : MonoBehaviour
    {
        [SerializeField]
        private Image wipe;
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
            wipe.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            isDone = true;
        }

        public void DirectOut()
        {
            wipe.rectTransform.anchoredPosition = new Vector2(1920f, 0f);
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

                wipe.rectTransform.anchoredPosition = new Vector2(-1920f, 0f);
                wipe.color = new Color(0f, 0f, 0f, wipeOpacity);

                var opacity = wipeOpacity;
                var opacityGap = (1 - wipeOpacity) / 100f;
                var moveGap = 1920 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    opacity += opacityGap;
                    wipe.rectTransform.anchoredPosition += Vector2.right * moveGap;
                    wipe.color = new Color(0f, 0f, 0f, opacity);
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

                wipe.rectTransform.anchoredPosition = new Vector2(1920f, 0f);
                wipe.color = new Color(0f, 0f, 0f, wipeOpacity);

                var opacity = wipeOpacity;
                var opacityGap = (1 - wipeOpacity) / 100f;
                var moveGap = 1920 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    opacity += opacityGap;
                    wipe.rectTransform.anchoredPosition -= Vector2.right * moveGap;
                    wipe.color = new Color(0f, 0f, 0f, opacity);
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

                wipe.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                wipe.color = new Color(0f, 0f, 0f, 1f);

                var opacity = 1f;
                var opacityGap = (1-wipeOpacity) / 100f;
                var moveGap = 1920 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    opacity -= opacityGap;
                    wipe.rectTransform.anchoredPosition -= Vector2.right * moveGap;
                    wipe.color = new Color(0f, 0f, 0f, opacity);
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

                wipe.rectTransform.anchoredPosition = new Vector2(0f, 0f);
                wipe.color = new Color(0f, 0f, 0f, 1f);

                var opacity = 1f;
                var opacityGap = (1-wipeOpacity) / 100f;
                var moveGap = 1920 / 100f;
                var timeGap = duration / 100f;
                for (int i = 0; i < 100; i++)
                {
                    opacity -= opacityGap;
                    wipe.rectTransform.anchoredPosition += Vector2.right * moveGap;
                    wipe.color = new Color(0f, 0f, 0f, opacity);
                    yield return new WaitForSeconds(timeGap);
                }
                isDone = true;
            }
        }
    }
}
