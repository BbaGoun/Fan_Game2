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

        public void CallDamageFlash(float waitFlashTime, float flashTime, float maxFlash)
        {
            if (_damageFlashCoroutine != null)
                StopCoroutine(_damageFlashCoroutine);
            _damageFlashCoroutine = StartCoroutine(DamageFlashCoroutine(waitFlashTime, flashTime, maxFlash));
        }

        IEnumerator DamageFlashCoroutine(float waitFlashTime, float flashTime, float maxFlash)
        {
            yield return new WaitForSeconds(waitFlashTime);

            SetFlashColor();

            float currentFlashAmount = 0f;
            float elapsedTime = 0f;
            while(elapsedTime < flashTime)
            {
                elapsedTime += Time.deltaTime;

                currentFlashAmount = Mathf.Lerp(maxFlash, 0, elapsedTime / flashTime);
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
