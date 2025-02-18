using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ActionPart
{
    public class CutSceneController : MonoBehaviour
    {
        public string sceneName;
        public LoadingManager.SpawnPoint spawnPoint;
        public LoadingManager.WithWalkOut withWalkOut;
        public LoadingManager.TransitionMode transitionMode;

        public List<CutSceneItem> scenes = new List<CutSceneItem>();
        public int sceneIndex;
        public Image background;
        public GameObject blur;
        public float backgroundOpacity;

        bool isCoroutineRunning;
        bool isAlreadyClicked;
        bool isNext;

        public void Awake()
        {
            sceneIndex = 0;

            background.gameObject.SetActive(true);
            blur.gameObject.SetActive(true);
            background.color = new Color(0f, 0f, 0f, backgroundOpacity);

            PlayerInputPart.Instance.EventKeyUpConfirm += CutSceneConfirm;
        }

        public void OnDisable()
        {
            PlayerInputPart.Instance.EventKeyUpConfirm -= CutSceneConfirm;
        }

        public void CutSceneConfirm()
        {
            Debug.Log("뭐지");
            isNext = true;
        }

        public void Update()
        {
            if (LoadingManager.Instance.CheckIsLoadDone())
            {
                if (isNext)
                {
                    if (!isCoroutineRunning)
                    {
                        StartCoroutine(IEShowScene());
                    }

                    isNext = false;
                }
            }
        }

        public IEnumerator IEShowScene()
        {
            isCoroutineRunning = true;

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
                    if (isAlreadyClicked)
                        break;
                    isAlreadyClicked = true;
                    if (sceneName.Equals(""))
                        LoadingManager.Instance.UnLoadCartoonSceneAsync(transitionMode, inDelay: 0.25f, outDelay: 0.25f);
                    else
                    {
                        LoadingManager.Instance.LoadSceneAsync(sceneName, spawnPoint, withWalkOut, transitionMode, inDelay: 0.25f, outDelay: 0.25f);
                    }
                    // 컷신 종료에 대한 행동
                    break;
            }

            isCoroutineRunning = false;
        }

    }
}
