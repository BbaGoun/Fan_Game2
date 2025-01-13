using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class EnemyHealthInterface : MonoBehaviour
    {
        [SerializeField]
        EnemyHealth enemyHealth;
        [SerializeField]
        Slider sliderHP;
        [SerializeField]
        Slider sliderStamina;
        [SerializeField]
        private float underDelay;

        [SerializeField, ReadOnly(false)]
        RectTransform underHP;
        [SerializeField, ReadOnly(false)]
        RectTransform underStamina;

        private float startUnderHP;
        private float endUnderHP;
        private float startUnderStamina;
        private float endUnderStamina;

        private float currentHP;
        private Coroutine coroutineHP;
        private Coroutine coroutineHPUnder;

        private float currentStamina;
        private Coroutine coroutineStamina;
        private Coroutine coroutineStaminaUnder;

        private void Awake()
        {
            underHP = sliderHP.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
            underStamina = sliderStamina.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();

            startUnderHP = sliderHP.GetComponent<RectTransform>().rect.width;
            endUnderHP = 0f;
            startUnderStamina = sliderStamina.GetComponent<RectTransform>().rect.width;
            endUnderStamina = 0f;

            sliderHP.maxValue = enemyHealth.GetMaxHp();
            sliderStamina.maxValue = enemyHealth.GetMaxStamina();

            currentHP = enemyHealth.GetCurrentHp();
            currentStamina = enemyHealth.GetCurrentStamina();

            sliderHP.value = currentHP;
            SetUnderHPBar(currentHP);

            sliderStamina.value = currentStamina;
            SetUnderStaminaBar(currentStamina);

            enemyHealth.eventHPChange += ChangeHP;
            enemyHealth.eventHPChangeDot += ChangeHPDot;

            enemyHealth.eventStaminaChange += ChangeStamina;
            enemyHealth.eventStaminaChangeDot += ChangeStaminaDot;
            // 도트 딜 중에 데미지 받기, 회복 중에 도트 회복 얻기 등을 할 시 문제가 발생할 것으로 예상 됨
        }

        void SetUnderHPBar(float value)
        {
            var max = sliderHP.maxValue;
            var percent = value / max;
            Utility.SetRectRight(underHP, Mathf.Lerp(startUnderHP, endUnderHP, percent));
        }

        void SetUnderStaminaBar(float value)
        {
            var max = sliderStamina.maxValue;
            var percent = value / max;
            Utility.SetRectRight(underStamina, Mathf.Lerp(startUnderStamina, endUnderStamina, percent));
        }

        void ChangeHP()
        {
            var changedHP = enemyHealth.GetCurrentHp();

            if (coroutineHP != null)
                StopCoroutine(coroutineHP);
            if (coroutineHPUnder != null)
                StopCoroutine(coroutineHPUnder);

            if (currentHP < changedHP)
            {
                // 회복 했을 때
                coroutineHPUnder = StartCoroutine(IEChangeUnderHP(currentHP, changedHP, 0f));
                coroutineHP = StartCoroutine(IEChangeHP(currentHP, changedHP, underDelay));
            }
            else if (currentHP > changedHP)
            {
                // 피해를 받았을 때
                coroutineHP = StartCoroutine(IEChangeHP(currentHP, changedHP, 0f));
                coroutineHPUnder = StartCoroutine(IEChangeUnderHP(currentHP, changedHP, underDelay));
            }

            currentHP = changedHP;
        }

        void ChangeHPDot()
        {
            var changedHP = enemyHealth.GetCurrentHp();

            if (coroutineHP != null)
                StopCoroutine(coroutineHP);

            currentHP = changedHP;
            sliderHP.value = currentHP;
            SetUnderHPBar(currentHP);
        }

        void ChangeStamina()
        {
            var changedStamina = enemyHealth.GetCurrentStamina();

            if (coroutineStamina != null)
                StopCoroutine(coroutineStamina);
            if (coroutineStaminaUnder != null)
                StopCoroutine(coroutineStaminaUnder);

            if (currentStamina < changedStamina)
            {
                // 회복 했을 때
                coroutineStaminaUnder = StartCoroutine(IEChangeUnderStamina(currentStamina, changedStamina, 0f));
                coroutineStamina = StartCoroutine(IEChangeStamina(currentStamina, changedStamina, underDelay));
            }
            else if (currentStamina > changedStamina)
            {
                // 피해를 받았을 때
                coroutineStamina = StartCoroutine(IEChangeStamina(currentStamina, changedStamina, 0f));
                coroutineStaminaUnder = StartCoroutine(IEChangeUnderStamina(currentStamina, changedStamina, underDelay));
            }

            currentStamina = changedStamina;
        }

        void ChangeStaminaDot()
        {
            var changedStamina = enemyHealth.GetCurrentStamina();

            if (coroutineStamina != null)
                StopCoroutine(coroutineStamina);

            currentStamina = changedStamina;
            sliderStamina.value = currentStamina;
            SetUnderStaminaBar(currentStamina);
        }

        IEnumerator IEChangeHP(float start, float end, float delay)
        {
            yield return new WaitForSeconds(delay * Time.timeScale);

            float multiplier;

            if (delay == 0f)
                multiplier = 50f;
            else
                multiplier = 35f;

            sliderHP.value = start;

            var gap = (end - start) / multiplier;
            for (int i = 0; i < multiplier; i++)
            {
                sliderHP.value += gap;
                yield return null;
            }
        }

        IEnumerator IEChangeUnderHP(float start, float end, float delay)
        {
            yield return new WaitForSeconds(delay * Time.timeScale);

            float multiplier;

            if (delay == 0f)
                multiplier = 50f;
            else
                multiplier = 35f;

            var current = start;
            var gap = (end - start) / multiplier;
            for (int i = 1; i <= multiplier; i++)
            {
                current = current + gap;
                SetUnderHPBar(current);
                yield return null;
            }
        }

        IEnumerator IEChangeStamina(float start, float end, float delay)
        {
            yield return new WaitForSeconds(delay * Time.timeScale);

            float multiplier;

            if (delay == 0f)
                multiplier = 50f;
            else
                multiplier = 35f;

            sliderStamina.value = start;

            var gap = (end - start) / multiplier;
            for (int i = 0; i < multiplier; i++)
            {
                sliderStamina.value += gap;
                yield return null;
            }
        }

        IEnumerator IEChangeUnderStamina(float start, float end, float delay)
        {
            yield return new WaitForSeconds(delay * Time.timeScale);

            float multiplier;

            if (delay == 0f)
                multiplier = 50f;
            else
                multiplier = 35f;

            var current = start;
            var gap = (end - start) / multiplier;
            for (int i = 1; i <= multiplier; i++)
            {
                current = current + gap;
                SetUnderStaminaBar(current);
                yield return null;
            }
        }
    }
}
