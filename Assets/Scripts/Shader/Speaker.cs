using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class Speaker : MonoBehaviour
    {
        public float takeTime;
        public float waitTime;
        public float maxAmount;

        [ReadOnly(false)]
        public Image image;

        private Material _material;

        private Coroutine _darkCoroutine;
        private Coroutine _unDarkCoroutine;

        private void Awake()
        {
            if (image == null)
                image = GetComponent<Image>();
            _material = new Material(image.material);
            image.material = _material;
        }

        public void CallDark()
        {
            if (_darkCoroutine != null)
                StopCoroutine(_darkCoroutine);
            if (_unDarkCoroutine != null)
                StopCoroutine(_unDarkCoroutine);
            _darkCoroutine = StartCoroutine(IEDarkCoroutine());
        }

        IEnumerator IEDarkCoroutine()
        {
            yield return new WaitForSeconds(waitTime);

            float currentAmount = 0f;
            float timer = 0f;
            while (timer < takeTime)
            {
                timer += Time.deltaTime;

                var rate = timer / takeTime;

                currentAmount = maxAmount * rate;
                SetDarkAmount(currentAmount);

                yield return null;
            }
            SetDarkAmount(maxAmount);
        }

        public void CallUnDark()
        {
            if(_darkCoroutine != null)
                StopCoroutine(_darkCoroutine);
            if (_unDarkCoroutine != null)
                StopCoroutine(_unDarkCoroutine);
            _unDarkCoroutine = StartCoroutine(IEUnDarkCoroutine());
        }

        IEnumerator IEUnDarkCoroutine()
        {
            yield return new WaitForSeconds(waitTime);

            float currentAmount = 0f;
            float timer = 0f;
            while (timer < takeTime)
            {
                timer += Time.deltaTime;

                var rate = timer / takeTime;

                currentAmount = maxAmount * rate;
                SetDarkAmount(maxAmount - currentAmount);

                yield return null;
            }
            SetDarkAmount(0);
        }

        private void SetDarkAmount(float amount)
        {
            _material.SetFloat("_Amount", amount);
        }
    }
}
