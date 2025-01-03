using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TimelineBars : MonoBehaviour
    {
        public RectTransform upSide;
        public RectTransform downSide;
        public float defaultY;

        private void Awake()
        {
            upSide = transform.GetChild(0).GetComponent<RectTransform>();
            downSide = transform.GetChild(1).GetComponent<RectTransform>();
            upSide.gameObject.SetActive(false);
            downSide.gameObject.SetActive(false);
        }

        public void BarsOn()
        {
            upSide.gameObject.SetActive(true);
            upSide.anchoredPosition = new Vector3(0, defaultY, 0);

            downSide.gameObject.SetActive(true);
            downSide.anchoredPosition = new Vector3(0, -defaultY, 0);

            StartCoroutine(IEBarsOn());

            IEnumerator IEBarsOn()
            {
                float gap = defaultY / 100f;
                for (int i=0; i<100; i++)
                {
                    upSide.anchoredPosition = new Vector3(0, upSide.anchoredPosition.y - gap, 0);
                    downSide.anchoredPosition = new Vector3(0, downSide.anchoredPosition.y + gap, 0);
                    yield return null;
                }
            }
        }

        public void BarsOff()
        {
            upSide.gameObject.SetActive(true);
            upSide.anchoredPosition = new Vector3(0, 0, 0);

            downSide.gameObject.SetActive(true);
            downSide.anchoredPosition = new Vector3(0, 0, 0);

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
