using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

        private float currentHP;
        private Coroutine coroutineHP;

        private float currentStamina;
        private Coroutine coroutineStamina;

        private void Awake()
        {
            sliderHP.maxValue = playerHealth.GetMaxHp();
            sliderStamina.maxValue = playerHealth.GetMaxStamina();

            currentHP = playerHealth.GetCurrentHp();
            currentStamina = playerHealth.GetCurrentStamina();

            sliderHP.value = currentHP;
            sliderStamina.value = currentStamina;

            playerHealth.eventHPChange += ChangeHP;
            playerHealth.eventHPChangeDot += ChangeHPDot;

            playerHealth.eventStaminaChange += ChangeStamina;
            playerHealth.eventStaminaChangeDot += ChangeStaminaDot;
            // 도트 딜 중에 데미지 받기, 회복 중에 도트 회복 얻기 등을 할 시 문제가 발생할 것으로 예상 됨
        }

        void ChangeHP()
        {
            var changedHP = playerHealth.GetCurrentHp();

            if (coroutineHP != null)
                StopCoroutine(coroutineHP);

            coroutineHP = StartCoroutine(IEChangeHP(currentHP, changedHP));
            currentHP = changedHP;
        }

        void ChangeHPDot()
        {
            var changedHP = playerHealth.GetCurrentHp();

            if (coroutineHP != null)
                StopCoroutine(coroutineHP);

            currentHP = changedHP;
            sliderHP.value = currentHP;
        }

        void ChangeStamina()
        {
            var changedStamina = playerHealth.GetCurrentStamina();

            if (coroutineStamina != null)
                StopCoroutine(coroutineStamina);

            coroutineStamina = StartCoroutine(IEChangeStamina(currentStamina, changedStamina));
            currentStamina = changedStamina;
        }

        void ChangeStaminaDot()
        {
            var changedStamina = playerHealth.GetCurrentStamina();

            if (coroutineStamina != null)
                StopCoroutine(coroutineStamina);

            currentStamina = changedStamina;
            sliderStamina.value = currentStamina;
        }

        IEnumerator IEChangeHP(float start, float end)
        {
            var gap = (end - start) / 40f;
            for(int i = 0; i < 40; i++)
            {
                sliderHP.value += gap;
                yield return new WaitForSeconds(0.01f * Time.timeScale);
            }
        }

        IEnumerator IEChangeStamina(float start, float end)
        {
            var gap = (end - start) / 40f;
            for (int i = 0; i < 40; i++)
            {
                sliderStamina.value += gap;
                yield return new WaitForSeconds(0.01f * Time.timeScale);
            }
        }
    }
}
