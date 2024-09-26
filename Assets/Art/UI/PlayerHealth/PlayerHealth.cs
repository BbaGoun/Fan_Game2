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
        }

        private void Update()
        {
            if (playerHealth)
            {
                var nextHP = playerHealth.GetCurrentHp();
                var nextStamina = playerHealth.GetCurrentStamina();
                
                if(currentHP != nextHP)
                {
                    if (coroutineHP != null)
                        StopCoroutine(coroutineHP);
                    coroutineHP = StartCoroutine(ChangeHP(currentHP, nextHP));
                    currentHP = nextHP;
                }

                if(currentStamina != nextStamina)
                {
                    if (coroutineStamina != null)
                        StopCoroutine(coroutineStamina);
                    coroutineStamina = StartCoroutine(ChangeStamina(currentStamina, nextStamina));
                    currentStamina = nextStamina;
                }
            }
        }

        IEnumerator ChangeHP(float start, float end)
        {
            var gap = (end - start) / 20f;
            for(int i = 0; i < 20; i++)
            {
                sliderHP.value += gap;
                yield return new WaitForSeconds(0.01f);
            }
        }

        IEnumerator ChangeStamina(float start, float end)
        {
            var gap = (end - start) / 20f;
            for (int i = 0; i < 20; i++)
            {
                sliderStamina.value += gap;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
    }
}
