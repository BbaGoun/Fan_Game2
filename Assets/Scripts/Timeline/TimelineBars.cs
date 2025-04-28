using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TimelineBars : MonoBehaviour
    {
        public static TimelineBars Instance;

        public RectTransform upSide;
        public RectTransform downSide;
        public float defaultY;
        private bool isBarsOn;
        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(this.gameObject);
                }
            }

            #endregion

            upSide = transform.GetChild(0).GetComponent<RectTransform>();
            downSide = transform.GetChild(1).GetComponent<RectTransform>();
            upSide.gameObject.SetActive(false);
            downSide.gameObject.SetActive(false);
        }

        public void BarsOn()
        {
            if(isBarsOn)
                return;

            upSide.gameObject.SetActive(true);
            upSide.anchoredPosition = new Vector3(0, defaultY, 0);

            downSide.gameObject.SetActive(true);
            downSide.anchoredPosition = new Vector3(0, -defaultY, 0);

            isBarsOn = true;
            StartCoroutine(IEBarsOn());

            IEnumerator IEBarsOn()
            {
                float gap = defaultY / 100f;
                for (int i = 0; i < 100; i++)
                {
                    upSide.anchoredPosition = new Vector3(0, upSide.anchoredPosition.y - gap, 0);
                    downSide.anchoredPosition = new Vector3(0, downSide.anchoredPosition.y + gap, 0);
                    yield return null;
                }
            }
        }

        public void BarsOff()
        {
            if(!isBarsOn)
                return;

            upSide.gameObject.SetActive(true);
            upSide.anchoredPosition = new Vector3(0, 0, 0);

            downSide.gameObject.SetActive(true);
            downSide.anchoredPosition = new Vector3(0, 0, 0);

            isBarsOn = false;
            StartCoroutine(IEBarsOff());

            IEnumerator IEBarsOff()
            {
                float gap = defaultY / 100f;
                for (int i = 0; i < 100; i++)
                {
                    upSide.anchoredPosition = new Vector3(0, upSide.anchoredPosition.y + gap, 0);
                    downSide.anchoredPosition = new Vector3(0, downSide.anchoredPosition.y - gap, 0);
                    yield return null;
                }
            }
        }
    }
}
