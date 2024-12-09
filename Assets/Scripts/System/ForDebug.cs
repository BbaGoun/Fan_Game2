using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ActionPart
{
    public class ForDebug : MonoBehaviour
    {
        public static ForDebug Instance;
        private TMP_Text text;

        public void Initialize()
        {
            Instance = this;

            text = GetComponent<TMP_Text>();
        }

        public void AddLog(string message)
        {
            text.text += "\n";
            text.text += message;
        }

    }
}
