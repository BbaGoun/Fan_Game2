using System;
using System.Collections;
using System.Collections.Generic;
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

            LoadSceneAsync("메인 타이틀", mode:TransitionMode.FromLeft, inDelay: 1f, outDelay: 1f);
        }

        public void LoadSceneAsync(string sceneName, TransitionMode mode = TransitionMode.Direct, float inDelay = 0f, float outDelay = 0f)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(LoadSceneCoroutine(sceneName, mode, inDelay, outDelay));


            IEnumerator LoadSceneCoroutine(string sceneName, TransitionMode mode, float inDelay, float outDelay)
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
                            // 불러온 씬에 대한 초기화 작업들
                            async1.allowSceneActivation = true;
                        }

                        yield return new WaitForSeconds(0.01f); // Wait for the next frame
                    }
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
                    {
                        // 불러온 씬에 대한 초기화 작업들
                        asyncLoad.allowSceneActivation = true;
                        loadedSceneName = sceneName;
                    }

                    yield return new WaitForSeconds(0.01f); // Wait for the next frame
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
                yield return new WaitUntil(loadingScene.CheckisDone);
                // 캐릭터 조작 활성화
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
    }
}