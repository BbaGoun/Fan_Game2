using ActionPart.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class ElevatorMom : MonoBehaviour
    {
        public Elevator elevator;

        public void ElevatorDownGo()
        {
            elevator.ElevatorDownGo();
        }
    }
}
