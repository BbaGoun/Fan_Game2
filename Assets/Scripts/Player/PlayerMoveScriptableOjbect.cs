using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerMoveScriptableOjbect", order = 1)]
public class PlayerMoveScriptableOjbect : ScriptableObject
{
    [Header("Move Parameter")]
    [SerializeField]
    // 이동속도
    private float moveSpeed;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float deceleration;
    [SerializeField]
    private float velPower;
    [SerializeField]
    private float frictionAmount;

    [Header("Jump Parameter")]
    [SerializeField]
    private float jumpForce; // 점프력
    [SerializeField]
    private float doubleJumpForce; // 더블 점프력
    [SerializeField]
    private float minJumpDuration; // 최소 점프 지속 시간
    [SerializeField, Range(-100, -9.81f)]
    private float maxFallSpeed;
    [SerializeField]
    private float gravityModifier;
    [SerializeField]
    private float jumpHoldGravity;
    [SerializeField]
    private float jumpCancelGravity;

    [Header("Dash Parameter")]
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashAcceleration;
    [SerializeField]
    private float dashDeceleration;
    [SerializeField]
    private float dashVelPower;
    [SerializeField]
    private float dashDuration;
    [SerializeField]
    private float dashCoolTime;
}
