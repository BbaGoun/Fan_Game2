using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class EventRemember : MonoBehaviour
    {
        public int æ»»÷∫πµµCount;

        static public EventRemember Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }
    }
}
