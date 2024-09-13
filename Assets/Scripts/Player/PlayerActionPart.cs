using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionPart : MonoBehaviour
{
    [Header("Attack Parameter")]
    [SerializeField]
    private bool attackStarted;
    [SerializeField]
    private bool attackCanceled;
    [SerializeField]
    private AttackState attackState;
    [SerializeField]
    private float attackTimer;
    [SerializeField]
    private float attackCooltime; // ���� ��Ÿ��
    [SerializeField]
    private float attackChargeTime; // ���� ������ ���� ���� �ð�
    private SpriteRenderer attackSprite; // �ӽ� ���� ����Ʈ

    [Header("Skill Parameter")]
    [SerializeField]
    private bool skillStarted;

    Animator animator;
    
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        attackSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        attackCanceled = true;
    }

    public void DoActionPart()
    {

    }

    void ControlAttack()
    {
        #region Attack
        if (attackState == AttackState.Idle && attackStarted)
        {
            attackStarted = false;
            attackState = AttackState.Attack;
        }
        else if (attackState == AttackState.Idle && !attackCanceled)
        {
            attackState = AttackState.PrepareChargeAttack;
        }
        else if (attackState == AttackState.PrepareChargeAttack && attackCanceled)
        {
            if (attackTimer >= attackChargeTime)
            {
                attackTimer = 0f;
                attackState = AttackState.ChargeAttack;
            }
            else
            {
                attackTimer = 0f;
                attackState = AttackState.Idle;
            }
        }
        // �������� ���� �ڵ�
        #endregion
    }

    void UpdateAttackState()
    {
        switch (attackState)
        {
            case AttackState.Idle:
                break;
            case AttackState.Attack:
                Attack();
                animator.SetTrigger("isAttack");
                attackTimer = 0f;
                attackState = AttackState.Cooltime;
                break;
            case AttackState.PrepareChargeAttack:
                // ����Ʈ
                attackTimer += Time.deltaTime;
                if (attackTimer < attackChargeTime)
                {
                    // ���� �� ����Ʈ
                }
                else
                {
                    // ���� �� ����Ʈ
                }
                break;
            case AttackState.ChargeAttack:
                ChargeAttack();
                attackState = AttackState.Cooltime;
                break;
            case AttackState.Cooltime:
                attackTimer += Time.deltaTime;
                if (attackTimer > attackCooltime)
                {
                    attackTimer = 0f;
                    attackState = AttackState.Idle;
                }
                break;
        }
    }

    void Attack()
    {
        attackSprite.gameObject.SetActive(true);
        attackSprite.transform.localScale = Vector3.one;
        attackSprite.color = Color.white;
    }

    void ChargeAttack()
    {
        attackSprite.gameObject.SetActive(true);
        attackSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        attackSprite.color = Color.red;
    }

    #region Input
    public void ActionAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attackStarted = true;
            attackCanceled = false;
        }
        else if (context.canceled)
        {
            attackStarted = false;
            attackCanceled = true;
        }
    }

    public void ActionSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            skillStarted = true;
        }
        else if (context.performed)
        {

        }
        else if (context.canceled)
        {
            skillStarted = false;
        }
    }
    #endregion

    #region States
    private enum AttackState
    {
        Idle,
        Attack,
        PrepareChargeAttack,
        ChargeAttack,
        Cooltime,
    }
    #endregion
}
