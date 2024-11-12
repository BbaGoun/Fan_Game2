using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TitleTest : MonoBehaviour
    {
        public string sceneName;
        
        public void LoadScene()
        {
            LoadingManager.Instance.LoadSceneAsync(sceneName, LoadingManager.SpawnPoint.Right, LoadingManager.TransitionMode.FromRight, inDelay: 0.5f, outDelay: 0.5f);
        }
    }
}
