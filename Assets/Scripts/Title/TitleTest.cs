using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TitleTest : MonoBehaviour
    {
        public string sceneName;
        public LoadingManager.SpawnPoint spawnPoint;
        public LoadingManager.WithWalkOut withWalkOut;
        public LoadingManager.TransitionMode transitionMode;
        
        public void LoadScene()
        {
            LoadingManager.Instance.LoadSceneAsync(sceneName, spawnPoint, withWalkOut, transitionMode, inDelay: 0.25f, outDelay: 0.25f);
        }
    }
}
