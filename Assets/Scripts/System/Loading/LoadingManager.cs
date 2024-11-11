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
        public VirtualCameraControl virtualCameraControl;
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

            LoadSceneAsync("���ּ� ����", mode:TransitionMode.FromLeft, inDelay: 1f, outDelay: 1f);
        }

        public void LoadSceneAsync(string sceneName, TransitionMode mode = TransitionMode.Direct, float inDelay = 0f, float outDelay = 0f)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(LoadSceneCoroutine(sceneName, mode, inDelay, outDelay));


            IEnumerator LoadSceneCoroutine(string sceneName, TransitionMode mode, float inDelay, float outDelay)
            {
                // ĳ���� ���� ��Ȱ��ȭ
                switch (mode)
                {
                    case TransitionMode.FromLeft:
                        loadingScene.FromLeftWipeIn(0.5f);
                        break;
                    case TransitionMode.FromRight:
                        loadingScene.FromRightWipeIn(0.5f);
                        break;
                    case TransitionMode.FadeIn:
                        // ���̵��� �Լ� ��������
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectIn();
                        break;
                }
                yield return new WaitUntil(loadingScene.CheckisDone);

                loadingScene.LoadingObjectsOn();
                loadingScene.LoadingProgressApply(0f);
                yield return new WaitForSeconds(inDelay);

                // ���� �� ��ε� �ʿ�
                if(loadedSceneName != null)
                {   
                    AsyncOperation async1 = SceneManager.UnloadSceneAsync(loadedSceneName);
                    async1.allowSceneActivation = false;

                    // Wait until the asynchronous scene fully loads
                    while (!async1.isDone)
                    {
                        float progress = Mathf.Clamp01(async1.progress / 0.9f); // Normalize progress between 0 and 1
                        loadingScene.LoadingProgressApply(progress / 2);
                        Debug.Log("�ε� ���൵ : " + progress / 2);

                        if (async1.progress >= 0.9f)
                        {
                            async1.allowSceneActivation = true;
                        }

                        yield return new WaitForSeconds(0.01f); // Wait for the next frame
                    }

                    // �� ��ε� �ϸ鼭 �ؾ��� �͵�
                }

                // �� �� �ε�
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                asyncLoad.allowSceneActivation = false;

                // Wait until the asynchronous scene fully loads
                while (!asyncLoad.isDone)
                {
                    float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Normalize progress between 0 and 1
                    loadingScene.LoadingProgressApply(0.5f + progress / 2);
                    Debug.Log("�ε� ���൵ : " + 0.5f + progress / 2);

                    if (asyncLoad.progress >= 0.9f)
                    {   asyncLoad.allowSceneActivation = true;
                        loadedSceneName = sceneName;
                    }

                    yield return new WaitForSeconds(0.01f); // Wait for the next frame
                }

                // �� �ε��ϸ鼭 �ʱ�ȭ�ؾ��� �͵�
                // �÷��̾ ���� ��ȯ�ؾ� ��
                virtualCameraControl.SetConfiner();

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
                        // ���̵� �ƿ� �ʿ�
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectOut();
                        break;
                }
                yield return new WaitUntil(loadingScene.CheckisDone);
                // ĳ���� ���� Ȱ��ȭ
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