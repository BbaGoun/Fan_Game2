using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class GameLoad : MonoBehaviour
    {
        public void Load(int Index)
        {
            DataManager.instance.GameLoad(Index - 1);
        }
    }
}
