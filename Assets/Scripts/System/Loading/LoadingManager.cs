using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionPart
{
    public class LoadingManager : MonoBehaviour
    {
        private static LoadingManager instance;
        // Private constructor to prevent creating instances via the 'new' keyword
        public static LoadingManager Instance
        {
            get
            {
                return instance;
            }
        }

        public LoadingScene loadingScene;
        public VirtualCameraControl virtualCameraControl;
        public PlayerWithStateMachine player;
        public bool loadDone { get; private set; }

        private SceneSetting sceneSetting;
        private ParallaxBackground parallaxBackground;
        private string loadedSceneName;
        private Coroutine coroutine;

        private void Awake()
        {
            #region Singleton

            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
            #endregion // Singleton

            LoadSceneAsync("메인 타이틀", SpawnPoint.None, WithWalkOut.None, mode: TransitionMode.FromLeft, inDelay: 0.25f, outDelay: 0.25f);
        }

        public void LoadSceneAsync(string sceneName, SpawnPoint spawnPoint, WithWalkOut walkOut, TransitionMode mode = TransitionMode.Direct, float inDelay = 0f, float outDelay = 0f)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(LoadSceneCoroutine(sceneName, spawnPoint, walkOut, mode, inDelay, outDelay));


            IEnumerator LoadSceneCoroutine(string sceneName, SpawnPoint spawnPoint, WithWalkOut walkOut, TransitionMode mode, float inDelay, float outDelay)
            {
                // 캐릭터 조작 비활성화
                switch (mode)
                {
                    case TransitionMode.FromLeft:
                        loadingScene.FromLeftWipeIn(0.5f);
                        break;
                    case TransitionMode.FromRight:
                        loadingScene.FromRightWipeIn(0.5f);
                        break;
                    case TransitionMode.FadeIn:
                        // 페이드인 함수 만들어야함
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectIn();
                        break;
                }
                yield return new WaitUntil(loadingScene.CheckisDone);

                loadDone = false;

                loadingScene.LoadingObjectsOn();
                loadingScene.LoadingProgressApply(0f);
                yield return new WaitForSeconds(inDelay);

                // 이전 씬 언로드 필요
                if(loadedSceneName != null)
                {   
                    AsyncOperation async1 = SceneManager.UnloadSceneAsync(loadedSceneName);
                    async1.allowSceneActivation = false;

                    // Wait until the asynchronous scene fully loads
                    while (!async1.isDone)
                    {
                        float progress = Mathf.Clamp01(async1.progress / 0.9f); // Normalize progress between 0 and 1
                        loadingScene.LoadingProgressApply(progress / 2);
                        Debug.Log("로딩 진행도 : " + progress / 2);

                        if (async1.progress >= 0.9f)
                        {
                            async1.allowSceneActivation = true;
                        }

                        yield return new WaitForSeconds(0.01f); // Wait for the next frame
                    }

                    // 씬 언로드 하면서 해야할 것들
                }

                // 새 씬 로드
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                asyncLoad.allowSceneActivation = false;

                // Wait until the asynchronous scene fully loads
                while (!asyncLoad.isDone)
                {
                    float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Normalize progress between 0 and 1
                    loadingScene.LoadingProgressApply(0.5f + progress / 2);
                    Debug.Log("로딩 진행도 : " + 0.5f + progress / 2);

                    if (asyncLoad.progress >= 0.9f)
                    {   asyncLoad.allowSceneActivation = true;
                        loadedSceneName = sceneName;
                    }

                    yield return new WaitForSeconds(0.01f); // Wait for the next frame
                }

                // 씬 로딩하면서 초기화해야할 것들
                // 플레이어를 먼저 소환해야 함
                if (!sceneName.Equals("메인 타이틀"))
                {
                    // 플레이어 위치 세팅
                    sceneSetting = GameObject.FindGameObjectWithTag("SceneSetting").GetComponent<SceneSetting>();
                    player.gameObject.SetActive(true);
                    switch (spawnPoint)
                    {
                        case SpawnPoint.None:
                            break;
                        case SpawnPoint.Left:
                            player.transform.localPosition = new Vector3(sceneSetting.LeftSpawnPoint.localPosition.x, sceneSetting.LeftSpawnPoint.localPosition.y, -4);
                            player.LookRight();
                            break;
                        case SpawnPoint.Right:
                            player.transform.localPosition = new Vector3(sceneSetting.RightSpawnPoint.localPosition.x, sceneSetting.RightSpawnPoint.localPosition.y, -4);
                            player.LookLeft();
                            break;
                    }
                    
                    virtualCameraControl.SetConfiner();
                    parallaxBackground = GameObject.FindGameObjectWithTag("Maps").GetComponent<ParallaxBackground>();
                    parallaxBackground.SetCamera();
                }
                else
                {
                    player.gameObject.SetActive(false);
                }

                yield return new WaitForSeconds(outDelay);
                loadingScene.LoadingObjectOff();

                switch (mode)
                {
                    case TransitionMode.FromLeft:
                        loadingScene.ToLeftWipeOut(0.5f);
                        break;
                    case TransitionMode.FromRight:
                        loadingScene.ToRightWipeOut(0.5f);
                        break;
                    case TransitionMode.FadeIn:
                        // 페이드 아웃 필요
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectOut();
                        break;
                }
                // 걸어나오는 기능 추가
                switch (walkOut)
                {
                    case WithWalkOut.Left:
                        break;
                    case WithWalkOut.Right:
                        player.playerMoveState.MoveXFromTo(sceneSetting.RightSpawnPoint, sceneSetting.RightWalkOutPoint);
                        break;
                    case WithWalkOut.None:
                        break;
                }
                yield return new WaitUntil(loadingScene.CheckisDone);
                yield return new WaitUntil(player.playerMoveState.IsCoroutineDone);

                // 캐릭터 조작 활성화
                loadDone = true;
            }
        }

        public enum TransitionMode
        {
            FromLeft,
            FromRight,
            ToLeft,
            ToRight,
            FadeIn,
            FadeOut,
            Direct,
        }

        public enum SpawnPoint
        {
            Left,
            Right, 
            Top, 
            Bottom,
            None
        }

        public enum WithWalkOut
        {
            Left,
            Right,
            None
        }
    }
}