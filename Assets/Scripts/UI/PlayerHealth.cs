using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace ActionPart
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        Health playerHealth;
        [SerializeField]
        Slider sliderHP;
        [SerializeField]
        Slider sliderStamina;
        [SerializeField]
        private float underDelay;

        RectTransform underHP;
        RectTransform underStamina;

        [SerializeField]
        private float startUnderHP;
        [SerializeField]
        private float endUnderHP;
        [SerializeField]
        private float startUnderStamina;
        [SerializeField]
        private float endUnderStamina;

        private float currentHP;
        private Coroutine coroutineHP;
        private Coroutine coroutineHPUnder;

        private float currentStamina;
        private Coroutine coroutineStamina;
        private Coroutine coroutineStaminaUnder;

        private void Awake()
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();

            underHP = sliderHP.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
            underStamina = sliderStamina.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();

            startUnderHP = Utility.GetRectRight(underHP);
            endUnderHP = 0f;
            startUnderStamina = Utility.GetRectRight(underStamina);
            endUnderStamina = 0f;

            sliderHP.maxValue = playerHealth.GetMaxHp();
            sliderStamina.maxValue = playerHealth.GetMaxStamina();

            currentHP = playerHealth.GetCurrentHp();
            currentStamina = playerHealth.GetCurrentStamina();

            sliderHP.value = currentHP;
            SetUnderHPBar(currentHP);

            sliderStamina.value = currentStamina;
            SetUnderStaminaBar(currentStamina);

            playerHealth.eventHPChange += ChangeHP;
            playerHealth.eventHPChangeDot += ChangeHPDot;

            playerHealth.eventStaminaChange += ChangeStamina;
            playerHealth.eventStaminaChangeDot += ChangeStaminaDot;
            // ��Ʈ �� �߿� ������ �ޱ�, ȸ�� �߿� ��Ʈ ȸ�� ��� ���� �� �� ������ �߻��� ������ ���� ��
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
            var changedHP = playerHealth.GetCurrentHp();

            if (coroutineHP != null)
                StopCoroutine(coroutineHP);
            if (coroutineHPUnder != null)
                StopCoroutine(coroutineHPUnder);

            if (currentHP < changedHP)
            {
                // ȸ�� ���� ��
                coroutineHPUnder = StartCoroutine(IEChangeUnderHP(currentHP, changedHP, 0f));
                coroutineHP = StartCoroutine(IEChangeHP(currentHP, changedHP, underDelay));
            }
            else if (currentHP > changedHP)
            {
                // ���ظ� �޾��� ��
                coroutineHP = StartCoroutine(IEChangeHP(currentHP, changedHP, 0f));
                coroutineHPUnder = StartCoroutine(IEChangeUnderHP(currentHP, changedHP, underDelay));
            }

            currentHP = changedHP;
        }

        void ChangeHPDot()
        {
            var changedHP = playerHealth.GetCurrentHp();

            if (coroutineHP != null)
                StopCoroutine(coroutineHP);

            currentHP = changedHP;
            sliderHP.value = currentHP;
            SetUnderHPBar(currentHP);
        }

        void ChangeStamina()
        {
            var changedStamina = playerHealth.GetCurrentStamina();

            if (coroutineStamina != null)
                StopCoroutine(coroutineStamina);
            if (coroutineStaminaUnder != null)
                StopCoroutine(coroutineStaminaUnder);

            if (currentStamina < changedStamina)
            {
                // ȸ�� ���� ��
                coroutineStaminaUnder = StartCoroutine(IEChangeUnderStamina(currentStamina, changedStamina, 0f));
                coroutineStamina = StartCoroutine(IEChangeStamina(currentStamina, changedStamina, underDelay));
            }
            else if (currentStamina > changedStamina)
            {
                // ���ظ� �޾��� ��
                coroutineStamina = StartCoroutine(IEChangeStamina(currentStamina, changedStamina, 0f));
                coroutineStaminaUnder = StartCoroutine(IEChangeUnderStamina(currentStamina, changedStamina, underDelay));
            }

            currentStamina = changedStamina;
        }

        void ChangeStaminaDot()
        {
            var changedStamina = playerHealth.GetCurrentStamina();

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

            var gap = (end - start) / multiplier;
            for(int i = 0; i < multiplier; i++)
            {
                sliderHP.value += gap;
                yield return new WaitForSeconds(0.01f * Time.timeScale);
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
            for(int i = 1; i<= multiplier; i++)
            {
                current = current + gap;
                SetUnderHPBar(current);
                yield return new WaitForSeconds(0.01f * Time.timeScale);
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

            var gap = (end - start) / multiplier;
            for (int i = 0; i < multiplier; i++)
            {
                sliderStamina.value += gap;
                yield return new WaitForSeconds(0.01f * Time.timeScale);
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
                yield return new WaitForSeconds(0.01f * Time.timeScale);
            }
        }
    }
}
