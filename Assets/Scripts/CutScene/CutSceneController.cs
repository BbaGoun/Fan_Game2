using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class CutSceneController : MonoBehaviour
    {
        public List<CutSceneItem> scenes = new List<CutSceneItem>();
        public int sceneIndex;
        public Image background;
        public GameObject blur;
        public float backgroundOpacity;

        bool isCoroutineRunning;

        public void Awake()
        {
            sceneIndex = -1;
        }

        public void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if (!isCoroutineRunning)
                {
                    StartCoroutine(IEShowScene());
                }
            }    
        }

        public IEnumerator IEShowScene()
        {
            isCoroutineRunning = true;

            if (sceneIndex == -1)
            {
                background.gameObject.SetActive(true);
                blur.gameObject.SetActive(true);
                background.color = new Color(0f, 0f, 0f, backgroundOpacity);
                sceneIndex += 1;
                scenes[sceneIndex].gameObject.SetActive(true);
            }
            else
            {
                switch (scenes[sceneIndex].role)
                {
                    case CutSceneItem.Role.None:
                        sceneIndex += 1;
                        scenes[sceneIndex].gameObject.SetActive(true);
                        break;
                    case CutSceneItem.Role.Next:
                        for (int i = 0; i <= sceneIndex; i++)
                            scenes[i].gameObject.SetActive(false);
                        sceneIndex += 1;
                        scenes[sceneIndex].gameObject.SetActive(true);
                        break;
                    case CutSceneItem.Role.Move:
                        scenes[sceneIndex].Move();
                        yield return new WaitUntil(scenes[sceneIndex].CheckMoveDone);
                        sceneIndex += 1;
                        scenes[sceneIndex].gameObject.SetActive(true);
                        break;
                    case CutSceneItem.Role.End:
                        // 컷신 종료에 대한 행동
                        break;
                }
            }

            isCoroutineRunning = false;
        }

    }
}
