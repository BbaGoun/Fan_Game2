using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/KinematicScriptableOjbect", order = 1)]
public class KinematicScriptableObject : ScriptableObject
{
    [Header("Ground/Slope Parameter")]
    [SerializeField]
    protected float coyoteTime;
    [SerializeField]
    protected Vector2 groundCheckBoxSize;
    [SerializeField]
    protected Vector2 headingCheckBoxSize;
    [SerializeField, Range(0f, 0.999f)]
    protected float maxUnitSlopeY = 0.6f; // 경사로 단위벡터의 최대 Y

    [SerializeField]
    protected float horizontalSlopeCheckDistance;
    [SerializeField]
    protected float verticalSlopeCheckDistance;
    [SerializeField, Min(1f)]
    protected float checkDistanceFactor = 1f;
    [SerializeField]
    protected Vector2 frontSlopeCheckOffset;
    [SerializeField]
    protected Vector2 downSlopeCheckOffset;
    [SerializeField]
    protected Vector2 backSlopeCheckOffset;
}
