using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionPart
{
    public class additiveScene : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadSceneAsync("���� ����", LoadSceneMode.Additive);
        }
    }
}
