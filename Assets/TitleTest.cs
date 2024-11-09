using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TitleTest : MonoBehaviour
    {
        public void LoadScene()
        {
            LoadingManager.Instance.LoadSceneAsync("Town1", LoadingManager.TransitionMode.FromRight, inDelay: 0.5f, outDelay: 0.5f);
        }
    }
}
