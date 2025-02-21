using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class EventRemember : MonoBehaviour
    {
        public bool 照番差亀First = true;
        public bool 照番失BossKilled = false;

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
