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

        [SerializeField]
        private LoadingScene loadingScene;
        [SerializeField]
        private VirtualCameraControl virtualCameraControl;
        [SerializeField]
        private PlayerWithStateMachine player;
        private bool isLoadDone;
        private bool isCamSetDone;

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

            LoadSceneAsync("���� Ÿ��Ʋ", SpawnPoint.None, WithWalkOut.None, mode: TransitionMode.FromLeft, inDelay: 0.25f, outDelay: 0.25f);
        }

        public string GetLoadedSceneName()
        {
            return loadedSceneName;
        }

        public bool CheckIsLoadDone()
        {
            return isLoadDone;
        }

        public bool CheckIsCamSetDone()
        {
            return isCamSetDone;
        }

        public void LoadSceneAsync(string sceneName, SpawnPoint spawnPoint, WithWalkOut walkOut, TransitionMode mode = TransitionMode.Direct, float inDelay = 0.25f, float outDelay = 0.25f)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(LoadSceneCoroutine(sceneName, spawnPoint, walkOut, mode, inDelay, outDelay));


            IEnumerator LoadSceneCoroutine(string sceneName, SpawnPoint spawnPoint, WithWalkOut walkOut, TransitionMode mode, float inDelay, float outDelay)
            {
                // ĳ���� ���� ��Ȱ��ȭ �߰� �ʿ�?

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

                // �� ���� �� �ʱ�ȭ
                isLoadDone = false;
                isCamSetDone = false;

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

                    // �� ��ε� �ϸ鼭 �ؾ��� �͵� �߰� �ʿ�?
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

                // �� �ε��ϸ鼭 �ʱ�ȭ�ؾ��� �͵� �߰� �ʿ�?
                if (!sceneName.Equals("���� Ÿ��Ʋ"))
                {
                    // �÷��̾� ��ġ ����
                    sceneSetting = GameObject.FindGameObjectWithTag("SceneSetting").GetComponent<SceneSetting>();

                    // �÷��̾ ���� ��ȯ�ؾ� ��
                    player.gameObject.SetActive(true);
                    // �÷��̾� ũ�� ���� + ī�޶� ũ�� ����
                    if (sceneName.Equals("���ּ� ����"))
                    {
                        player.SetSpeedMultiplier(1.25f);
                        player.transform.localScale = new Vector3(2, 2, 1);
                        virtualCameraControl.SetCamSize(10f);
                    }
                    else
                    {
                        player.SetSpeedMultiplier(1.0f);
                        player.transform.localScale = Vector3.one;
                        virtualCameraControl.SetCamSize(5f);
                    }

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
                    // ī�޶� ���� ��
                    isCamSetDone = true;
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
                        // ���̵� �ƿ� �ʿ�
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectOut();
                        break;
                }
                // �ɾ���� ��� �߰���
                switch (walkOut)
                {
                    case WithWalkOut.Left:
                        player.playerMoveState.MoveXFromTo(sceneSetting.LeftSpawnPoint, sceneSetting.LeftWalkOutPoint);
                        break;
                    case WithWalkOut.Right:
                        player.playerMoveState.MoveXFromTo(sceneSetting.RightSpawnPoint, sceneSetting.RightWalkOutPoint);
                        break;
                    case WithWalkOut.None:
                        break;
                }
                yield return new WaitUntil(loadingScene.CheckisDone);
                yield return new WaitUntil(player.playerMoveState.IsCoroutineDone);

                // ĳ���� ���� Ȱ��ȭ �߰� �ʿ�?
                isLoadDone = true;
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