using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Boss : KinematicObject, IWithStateMachine, IDamageAble
    {
        public virtual void GetDamage(float _hpDelta, Vector2 _direction)
        {

        }
    }
}
