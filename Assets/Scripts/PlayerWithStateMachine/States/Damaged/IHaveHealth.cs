using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    public void GetDamage(int hpDelta, Vector2 direction);

    public struct DamageInfo
    {
        public bool isDamaged;
        public int hpDelta;
        public Vector2 direction;
    }
}
