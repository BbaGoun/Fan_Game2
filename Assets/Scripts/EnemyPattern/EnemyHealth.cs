using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField]
        private float maxHP;
        [SerializeField]
        private float currentHP;
        [SerializeField]
        private float maxStamina;
        [SerializeField]
        private float currentStamina;

        [SerializeField]
        private float staminaRecoveryTime;
        private float staminaRecoveryTimer;
        Coroutine staminaRecoveryCoroutine;

        public delegate void DelHPChange();
        public event DelHPChange eventHPChange;

        public delegate void DelHPChangeDot();
        public event DelHPChangeDot eventHPChangeDot;

        public delegate void DelStaminaChange();
        public event DelStaminaChange eventStaminaChange;

        public delegate void DelStaminaChangeDot();
        public event DelStaminaChangeDot eventStaminaChangeDot;

        private bool isAlive => currentHP > 0;

        private bool isGroggy => currentStamina <= 0;

        private bool isInvincible;

        [SerializeField]
        private bool isCanRecoveryStamina;
        [SerializeField]
        private bool isStopRecoveryStamina;

        Coroutine invincibleCoroutine;

        private float invincibleTimer;
        private DamageFlash _damageFlash;

        private void Awake()
        {
            currentHP = maxHP;
            _damageFlash = GetComponent<DamageFlash>();
            isCanRecoveryStamina = true;
        }

        private void OnEnable()
        {
            staminaRecoveryCoroutine = StartCoroutine(IEStaminaRecovery());
            eventStaminaChange += StaminaRecoveryDelay;
        }

        private void OnDisable()
        {
            StopCoroutine(staminaRecoveryCoroutine);
        }

        private void OnValidate()
        {
            eventHPChange?.Invoke();

            eventStaminaChange?.Invoke();

            staminaRecoveryTimer = 0f;
            isCanRecoveryStamina = false;
        }

        private void HPIncrement(float hpDelta)
        {
            currentHP = Mathf.Clamp(currentHP + hpDelta, 0, maxHP);
        }

        private void HPDecrement(float hpDelta)
        {
            currentHP = Mathf.Clamp(currentHP - hpDelta, 0, maxHP);
        }

        private void StaminaIncrement(float staminaDelta)
        {
            currentStamina = Mathf.Clamp(currentStamina + staminaDelta, 0, maxStamina);
        }

        private void StaminaDecrement(float staminaDelta)
        {
            currentStamina = Mathf.Clamp(currentStamina - staminaDelta, 0, maxStamina);
        }

        private void StaminaDecrementOnlyTo1(float staminaDelta)
        {
            currentStamina = Mathf.Clamp(currentStamina - staminaDelta, 1, maxStamina);
        }

        public float GetMaxHp()
        {
            return maxHP;
        }

        public float GetCurrentHp()
        {
            return currentHP;
        }

        public float GetMaxStamina()
        {
            return maxStamina;
        }

        public float GetCurrentStamina()
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

        public void Heal_HP(float hpDelta)
        {
            HPIncrement(hpDelta);

            eventHPChange?.Invoke();
        }

        public void Heal_Stamina(float staminaDelta)
        {
            StaminaIncrement(staminaDelta);

            eventStaminaChange?.Invoke();
        }

        public bool Hurt_Hp(float hpDelta, float invincibleDuration, float waitFlashTime, float flashFrequency, float flashRepetition, float maxFlash)
        {
            if (isInvincible)
                return false;

            StartCoroutine(Invincible(invincibleDuration));
            _damageFlash.CallDamageFlash(waitFlashTime, flashFrequency, flashRepetition, maxFlash);
            HPDecrement(hpDelta);

            eventHPChange?.Invoke();

            staminaRecoveryTimer = 0f;
            isCanRecoveryStamina = false;

            return true;
        }

        public void Hurt_Stamina(float staminaDelta)
        {
            StaminaDecrement(staminaDelta);

            eventStaminaChange?.Invoke();

            staminaRecoveryTimer = 0f;
            isCanRecoveryStamina = false;
        }

        public void Hurt_StaminaOnlyTo1(float staminaDelta)
        {
            StaminaDecrementOnlyTo1(staminaDelta);

            eventStaminaChange?.Invoke();

            staminaRecoveryTimer = 0f;
            isCanRecoveryStamina = false;
        }

        public void OnInvincible(float invincibleDuration)
        {
            if (invincibleCoroutine != null)
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
            while (invincibleTimer < invincibleDuration)
            {
                invincibleTimer += Time.deltaTime;
                yield return null;
            }
            isInvincible = false;
        }

        IEnumerator IEStaminaRecovery()
        {
            while (true)
            {
                if (!isStopRecoveryStamina)
                {
                    if (isCanRecoveryStamina)
                    {
                        if (currentStamina < maxStamina)
                        {
                            StaminaIncrement(0.7f * currentHP / maxHP + 0.3f);
                            eventStaminaChangeDot?.Invoke();
                        }
                        yield return new WaitForSeconds(0.033f);
                    }
                    else
                    {
                        staminaRecoveryTimer += Time.deltaTime;
                        if (staminaRecoveryTimer > staminaRecoveryTime)
                            isCanRecoveryStamina = true;
                        yield return null;
                    }
                }
                yield return null;
            }
        }

        public void StopRecoveryStamina()
        {
            isStopRecoveryStamina = true;
        }

        public void UnStopRecoveryStamina()
        {
            isStopRecoveryStamina = false;
        }

        void StaminaRecoveryDelay()
        {
            staminaRecoveryTimer = staminaRecoveryTime - 0.5f;
            isCanRecoveryStamina = false;
        }
    }
}
