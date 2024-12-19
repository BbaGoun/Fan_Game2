using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class ElevatorDoor : MonoBehaviour
    {
        public void CloseDoor()
        {
            this.gameObject.SetActive(false);
        }
    }
}
