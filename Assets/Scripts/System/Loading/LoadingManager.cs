using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        [SerializeField]
        private AudioController audioController;
        [SerializeField]
        private GameObject interfaces;
        private bool isLoadDone;
        private bool isCamSetDone;

        private SceneSetting sceneSetting;
        private LocalTimelineController localTimelineController;
        private ParallaxBackground parallaxBackground;
        private string loadedSceneName;
        private string loadedCartoonSceneName;
        private Coroutine coroutine;

        public void Initialize()
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
            // ĳ���� ���� ��Ȱ��ȭ
            PlayerInputPart.Instance.CantInput();

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(LoadSceneCoroutine(sceneName, spawnPoint, walkOut, mode, inDelay, outDelay));


            IEnumerator LoadSceneCoroutine(string sceneName, SpawnPoint spawnPoint, WithWalkOut walkOut, TransitionMode mode, float inDelay, float outDelay)
            {
                switch (mode)
                {
                    case TransitionMode.FromLeft:
                        loadingScene.FromLeftWipeIn(0.5f);
                        break;
                    case TransitionMode.FromRight:
                        loadingScene.FromRightWipeIn(0.5f);
                        break;
                    case TransitionMode.FadeIn:
                        loadingScene.FadeIn(0.5f);
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectIn();
                        break;
                }
                audioController.FadeOutBGM(0.5f);
                yield return new WaitUntil(loadingScene.CheckisDone);

                //audioController.PauseBGM();

                // �� ���� �� �ʱ�ȭ
                isLoadDone = false;
                isCamSetDone = false;

                loadingScene.LoadingObjectsOn();
                loadingScene.LoadingProgressApply(0f);
                yield return new WaitForSeconds(inDelay);

                // ���� �� ��ε� �ʿ�
                if (loadedSceneName != null)
                {
                    AsyncOperation async1 = SceneManager.UnloadSceneAsync(loadedSceneName);
                    async1.allowSceneActivation = false;

                    // Wait until the asynchronous scene fully loads
                    while (!async1.isDone)
                    {
                        float progress = Mathf.Clamp01(async1.progress / 0.9f); // Normalize progress between 0 and 1
                        loadingScene.LoadingProgressApply(progress / 2);

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

                    if (asyncLoad.progress >= 0.9f)
                    { asyncLoad.allowSceneActivation = true;
                        loadedSceneName = sceneName;
                    }

                    yield return new WaitForSeconds(0.01f); // Wait for the next frame
                }

                sceneSetting = GameObject.FindGameObjectWithTag("SceneSetting").GetComponent<SceneSetting>();

                // �� �ε��ϸ鼭 �ʱ�ȭ�ؾ��� �͵� �߰� �ʿ�?
                if (!sceneName.Equals("���� Ÿ��Ʋ") && !sceneName.Equals("���ѷα� ��ȭ��"))
                {
                    interfaces.SetActive(true);

                    // �÷��̾ ���� ��ȯ�ؾ� ��
                    player.gameObject.SetActive(true);
                    // �÷��̾� ũ�� ���� + ī�޶� ũ�� ����
                    if (sceneName.Equals("���ּ� ����"))
                    {
                        player.SetSpeedMultiplier(1.5f);
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
                            player.transform.localPosition = new Vector3(sceneSetting.NoneSpawnPoint.localPosition.x, sceneSetting.NoneSpawnPoint.localPosition.y, -4);
                            player.LookRight();
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

                    localTimelineController = GameObject.FindGameObjectWithTag("LocalTimelineController").GetComponent<LocalTimelineController>();
                    GlobalTimelineController.instance.ChangeCurrentLocalTimelineController(localTimelineController);
                }
                else if(sceneName.Equals("���� Ÿ��Ʋ"))
                {
                    interfaces.SetActive(false);
                    var titleController = GameObject.FindGameObjectWithTag("SceneSetting").GetComponent<TitleController>();
                    titleController.Initialize();
                    player.gameObject.SetActive(false);
                }
                else if(sceneName.Equals("���ѷα� ��ȭ��"))
                {
                    interfaces.SetActive(false);
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
                        player.playerMoveState.MoveXFromTo(sceneSetting.LeftSpawnPoint.localPosition, sceneSetting.LeftWalkOutPoint.localPosition);
                        break;
                    case WithWalkOut.Right:
                        player.playerMoveState.MoveXFromTo(sceneSetting.RightSpawnPoint.localPosition, sceneSetting.RightWalkOutPoint.localPosition);
                        break;
                    case WithWalkOut.None:
                        player.playerMoveState.OnIsCoroutineDone();
                        PlayerInputPart.Instance.CanInput();
                        break;
                }
                audioController.ChangeBGM(sceneSetting.bgmClip);
                audioController.FadeInBGM(0.5f);

                yield return new WaitUntil(loadingScene.CheckisDone);
                yield return new WaitUntil(player.playerMoveState.IsCoroutineDone);

                CheckEnterEvent(sceneName);

                isLoadDone = true;
            }
        }

        public void LoadCartoonSceneAsync(string sceneName, TransitionMode mode = TransitionMode.Direct, float inDelay = 0.25f, float outDelay = 0.25f)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(LoadCartoonSceneCoroutine(sceneName, mode, inDelay, outDelay));


            IEnumerator LoadCartoonSceneCoroutine(string sceneName, TransitionMode mode, float inDelay, float outDelay)
            {
                // ĳ���� ���� ��Ȱ��ȭ
                // �ƽ��� ���� �� ���� Ȱ��ȭ ��ų ����
                PlayerInputPart.Instance.CantInput();
                interfaces.SetActive(false);

                switch (mode)
                {
                    case TransitionMode.FromLeft:
                        loadingScene.FromLeftWipeIn(0.5f);
                        break;
                    case TransitionMode.FromRight:
                        loadingScene.FromRightWipeIn(0.5f);
                        break;
                    case TransitionMode.FadeIn:
                        loadingScene.FadeIn(0.5f);
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectIn();
                        break;
                }
                audioController.FadeOutBGM(0.5f);
                yield return new WaitUntil(loadingScene.CheckisDone);

                //audioController.PauseBGM();

                yield return new WaitForSeconds(inDelay);

                // �� �� �ε�
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                asyncLoad.allowSceneActivation = false;

                while (!asyncLoad.isDone)
                {
                    if (asyncLoad.progress >= 0.9f)
                    {
                        asyncLoad.allowSceneActivation = true;
                        loadedCartoonSceneName = sceneName;
                    }

                    yield return new WaitForSeconds(0.01f); // Wait for the next frame
                }

                var cartoonSetting = GameObject.FindGameObjectWithTag("CartoonSetting").GetComponent<CartoonSetting>();
                Debug.Log(cartoonSetting.name);

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
                        loadingScene.FadeOut(0.5f);
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectOut();
                        break;
                }
                
                audioController.ChangeBGM(cartoonSetting.bgmClip);
                audioController.FadeInBGM(0.5f);

                yield return new WaitUntil(loadingScene.CheckisDone);

                isLoadDone = true;
            }
        }

        public void UnLoadCartoonSceneAsync(TransitionMode mode = TransitionMode.Direct, float inDelay = 0.25f, float outDelay = 0.25f)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(UnLoadSceneCoroutine(mode, inDelay, outDelay));


            IEnumerator UnLoadSceneCoroutine(TransitionMode mode, float inDelay, float outDelay)
            {
                switch (mode)
                {
                    case TransitionMode.FromLeft:
                        loadingScene.FromLeftWipeIn(0.5f);
                        break;
                    case TransitionMode.FromRight:
                        loadingScene.FromRightWipeIn(0.5f);
                        break;
                    case TransitionMode.FadeIn:
                        loadingScene.FadeIn(0.5f);
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectIn();
                        break;
                }
                audioController.FadeOutBGM(0.5f);
                yield return new WaitUntil(loadingScene.CheckisDone);

                audioController.PauseBGM();

                yield return new WaitForSeconds(inDelay);

                //��ȭ�� �� �ε�
                AsyncOperation asyncUnLoad = SceneManager.UnloadSceneAsync(loadedCartoonSceneName);
                loadedCartoonSceneName = null;
                asyncUnLoad.allowSceneActivation = false;

                while (!asyncUnLoad.isDone)
                {
                    if (asyncUnLoad.progress >= 0.9f)
                    {
                        asyncUnLoad.allowSceneActivation = true;
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
                        loadingScene.FadeOut(0.5f);
                        break;
                    case TransitionMode.Direct:
                        loadingScene.DirectOut();
                        break;
                }

                audioController.ChangeBGM(sceneSetting.bgmClip);
                audioController.FadeInBGM(0.5f);

                PlayerInputPart.Instance.CanInput();
                interfaces.SetActive(true);

                yield return new WaitUntil(loadingScene.CheckisDone);

                isLoadDone = true;
            }
        }

        public bool CheckCartoonOn()
        {
            if (loadedCartoonSceneName == null)
                return false;
            else
                return true;
        }

        public void CheckEnterEvent(string sceneName)
        {
            switch (sceneName)
            {
                case "���ּ� ����":
                    if (EventRemember.Instance.���ֺ���First)
                    {
                        EventRemember.Instance.���ֺ���First = false;
                        TalkManager.Instance.TalkStart("Ʃ�丮��_SC1-1.", null);
                    }
                    break;
                default:
                    break;
            }
        }

        public enum TransitionMode
        {
            FromLeft,
            FromRight,
            FadeIn,
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