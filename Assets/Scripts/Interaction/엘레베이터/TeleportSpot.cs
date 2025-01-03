using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TeleportSpot : MonoBehaviour
    {
        private string teleportSpotName;

        public void SetTeleportSpotName(string other)
        {
            teleportSpotName = other;
        }


    }
}
