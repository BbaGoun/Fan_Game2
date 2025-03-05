using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Boss : KinematicObject, IWithStateMachine, IDamageAble
    {
        public virtual bool CheckCanGetDamage() 
        {
            return false;
        }

        public virtual void GetDamage(float _hpDelta, Vector2 _direction)
        {

        }

        public virtual void Initialize() { }
    }
}
