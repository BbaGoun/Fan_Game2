using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    public void GetDamage(int hpDelta, Vector2 direction);
}
