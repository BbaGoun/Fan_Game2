using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class DamageFlash : MonoBehaviour
    {
        [ColorUsage(true, true), SerializeField]
        private Color _flashColor;

        private SpriteRenderer _spriteRenderer;
        private Material _material;

        private Coroutine _damageFlashCoroutine;

        private void Awake()
        {
            if(_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            _material = _spriteRenderer.material;
        }

        public void CallDamageFlash(float waitFlashTime, float flashFrequency, float flashRepetition, float maxFlash)
        {
            if (_damageFlashCoroutine != null)
                StopCoroutine(_damageFlashCoroutine);
            _damageFlashCoroutine = StartCoroutine(DamageFlashCoroutine(waitFlashTime, flashFrequency, flashRepetition, maxFlash));
        }

        IEnumerator DamageFlashCoroutine(float waitFlashTime, float flashFrequency, float flashRepetition, float maxFlash)
        {
            yield return new WaitForSeconds(waitFlashTime);

            SetFlashColor();

            float currentFlashAmount = 0f;
            float timer = 0f;
            float flashTime = flashFrequency * flashRepetition;
            while(timer < flashTime)
            {
                timer += Time.deltaTime;

                var rate = Mathf.Sin(2 * (1 / flashFrequency) * Mathf.PI * (timer - (0.25f * flashFrequency))) / 2 + 0.5f;

                currentFlashAmount = maxFlash * rate;
                SetFlashAmount(currentFlashAmount);

                yield return null;
            }
        }

        private void SetFlashColor()
        {
            _material.SetColor("_FlashColor", _flashColor);
        }

        private void SetFlashAmount(float amount)
        {
            _material.SetFloat("_FlashAmount", amount);
        }
    }
}
