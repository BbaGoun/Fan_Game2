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

        private bool isGroggy;

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

        public bool IsAlive()
        {
            return isAlive;
        }

        public bool IsGroggy()
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

        public bool Hurt(int hpDelta, float invincibleDuration, float waitFlashTime, float flashTime, float maxFlash)
        {
            if (isInvincible)
                return false;

            StartCoroutine(Invincible(invincibleDuration));
            _damageFlash.CallDamageFlash(waitFlashTime, flashTime, maxFlash);
            HPDecrement(hpDelta);
            
            return true;
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
