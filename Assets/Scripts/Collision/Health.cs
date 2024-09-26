using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private int maxHP;
        [SerializeField]
        private int currentHP;
        [SerializeField]
        private int maxStamina;
        [SerializeField]
        private int currentStamina;

        private bool isAlive => currentHP > 0;

        private bool isGroggy => currentStamina <= 0;

        private bool isInvincible;

        Coroutine invincibleCoroutine;

        private float invincibleTimer;
        private DamageFlash _damageFlash;

        private void Awake()
        {
            currentHP = maxHP;
            _damageFlash = GetComponent<DamageFlash>();
        }

        private void HPIncrement(int hpDelta)
        {
            currentHP = Mathf.Clamp(currentHP + hpDelta, 0, maxHP);
        }

        private void HPDecrement(int hpDelta)
        {
            currentHP = Mathf.Clamp(currentHP - hpDelta, 0, maxHP);
        }

        private void StaminaIncrement(int staminaDelta)
        {
            currentStamina = Mathf.Clamp(currentStamina + staminaDelta, 0, maxStamina);
        }

        private void StaminaDecrement(int staminaDelta)
        {
            currentStamina = Mathf.Clamp(currentStamina - staminaDelta, 0, maxStamina);
        }

        private void StaminaDecrementOnlyTo1(int staminaDelta)
        {
            currentStamina = Mathf.Clamp(currentStamina - staminaDelta, 1, maxStamina);
        }

        public int GetMaxHp()
        {
            return maxHP;
        }

        public int GetCurrentHp()
        {
            return currentHP;
        }

        public int GetMaxStamina()
        {
            return maxStamina;
        }

        public int GetCurrentStamina()
        {
            return currentStamina;
        }

        public bool CheckIsAlive()
        {
            return isAlive;
        }

        public bool CheckIsGroggy()
        {
            return isGroggy;
        }

        public bool IsInvincible()
        {
            return isInvincible;
        }

        public void Die()
        {
            currentHP = 0;
        }

        public void Heal(int hpDelta)
        {
            HPIncrement(hpDelta);
        }

        public bool Hurt_Hp(int hpDelta, float invincibleDuration, float waitFlashTime, float flashFrequency, float flashRepetition, float maxFlash)
        {
            if (isInvincible)
                return false;

            StartCoroutine(Invincible(invincibleDuration));
            _damageFlash.CallDamageFlash(waitFlashTime, flashFrequency, flashRepetition, maxFlash);
            HPDecrement(hpDelta);
            
            return true;
        }

        public void Hurt_Stamina(int staminaDelta)
        {
            StaminaDecrement(staminaDelta);
        }

        public void Hurt_StaminaOnlyTo1(int staminaDelta)
        {
            StaminaDecrementOnlyTo1(staminaDelta);
        }

        public void OnInvincible(float invincibleDuration)
        {
            if(invincibleCoroutine != null)
                StopCoroutine(invincibleCoroutine);
            StartCoroutine(Invincible(invincibleDuration));
        }

        public bool CheckInvincible()
        {
            return isInvincible;
        }

        IEnumerator Invincible(float invincibleDuration)
        {
            isInvincible = true;
            invincibleTimer = 0f;
            while(invincibleTimer < invincibleDuration)
            {
                invincibleTimer += Time.deltaTime;
                yield return null;
            }
            isInvincible = false;
        }
    }
}
