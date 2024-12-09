using ActionPart.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ActionPart
{
    public class TitleController : MonoBehaviour
    {
        public string sceneName;
        public LoadingManager.SpawnPoint spawnPoint;
        public LoadingManager.WithWalkOut withWalkOut;
        public LoadingManager.TransitionMode transitionMode;

        #region MainMenu Part
        private AudioController audioController;
        public AudioClip clickSound;
        private GameObject background;
        private GameObject option;
        private GameObject graphic;
        private GameObject sound;
        private GameObject control;
        private GameObject loadGame;

        public Image fullScreen, windowScreen;
        public Sprite[] fullScreenImages = new Sprite[2];
        public Sprite[] windowScreenImages = new Sprite[2];
        public Button resolutionDown, resolutionUp;
        public TMP_Text resolution;
        public Slider master, bgm, effectSound;
        int maxResolutionIndex;

        public SettingData tmpSettingData;
        #endregion

        private void Awake()
        {
            background = transform.GetChild(1).gameObject;
            background.SetActive(false);

            option = transform.GetChild(2).gameObject;
            option.SetActive(false);
            graphic = option.transform.GetChild(0).gameObject;
            graphic.SetActive(false);
            sound = option.transform.GetChild(1).gameObject;
            sound.SetActive(false);
            control = option.transform.GetChild(2).gameObject;
            control.SetActive(false);

            loadGame = transform.GetChild(3).gameObject;
            loadGame.SetActive(false);
        }

        public void Initialize()
        {
            audioController = AudioController.instance;

            maxResolutionIndex = SettingContainer.instance.m_SettingData.resolutionList.Count - 1;

            tmpSettingData = new SettingData(SettingContainer.instance.m_SettingData);

            master.value = SettingContainer.instance.m_SettingData.masterVolume;
            bgm.value = SettingContainer.instance.m_SettingData.BGMVolume;
            effectSound.value = SettingContainer.instance.m_SettingData.effectVolume;
        }


        public void LoadScene()
        {
            LoadingManager.Instance.LoadSceneAsync(sceneName, spawnPoint, withWalkOut, transitionMode, inDelay: 0.25f, outDelay: 0.25f);
        }

        #region Option
        public void ToOption()
        {
            PlayClickSound();
            background.SetActive(true);
            option.SetActive(true);

            graphic.SetActive(true);
            sound.SetActive(false);
            control.SetActive(false);

            ResetTmpSettingData();

            Resolution res = SettingContainer.instance.m_SettingData.resolutionList[SettingContainer.instance.m_SettingData.resolutionIndex];
            resolution.text = $"{res.width} * {res.height}";

            switch (SettingContainer.instance.m_SettingData.screenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    DoWhenFullScreen();
                    break;
                case FullScreenMode.MaximizedWindow:
                    DoWhenFullScreen();
                    break;
                case FullScreenMode.Windowed:
                    DoWhenWindowScreen();
                    break;
            }
        }

        public void CloseOption()
        {
            PlayClickSound();
            background.SetActive(false);
            option.SetActive(false);
            graphic.SetActive(false);
            sound.SetActive(false);
            control.SetActive(false);

            ResetTmpSettingData();
        }

        #region Graphic
        public void ToOptionGraphic()
        {
            PlayClickSound();
            graphic.SetActive(true);
            sound.SetActive(false);
            control.SetActive(false);

            ResetTmpSettingData();

            Resolution res = SettingContainer.instance.m_SettingData.resolutionList[SettingContainer.instance.m_SettingData.resolutionIndex];
            resolution.text = $"{res.width} * {res.height}";

            switch (SettingContainer.instance.m_SettingData.screenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    DoWhenFullScreen();
                    break;
                case FullScreenMode.MaximizedWindow:
                    DoWhenFullScreen();
                    break;
                case FullScreenMode.Windowed:
                    DoWhenWindowScreen();
                    break;
            }
        }

        public void SelectFullScreen()
        {
            PlayClickSound();
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) // window
            {
                tmpSettingData.screenMode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) // macOS
            {
                tmpSettingData.screenMode = FullScreenMode.MaximizedWindow;
            }
            DoWhenFullScreen();

            tmpSettingData.resolutionIndex = maxResolutionIndex;
            Resolution res = SettingContainer.instance.m_SettingData.resolutionList[maxResolutionIndex];
            resolution.text = $"{res.width} * {res.height}";
        }

        private void DoWhenFullScreen()
        {
            fullScreen.sprite = fullScreenImages[1];
            windowScreen.sprite = windowScreenImages[0];
            resolutionDown.interactable = false;
            resolutionUp.interactable = false;
            resolution.color = new Color(150 / 255f, 150 / 255f, 150 / 255f, 1f);
        }

        public void SelectWindowScreen()
        {
            PlayClickSound();
            tmpSettingData.screenMode = FullScreenMode.Windowed;
            DoWhenWindowScreen();

            tmpSettingData.resolutionIndex = SettingContainer.instance.m_SettingData.windowResolutionIndex;
            Resolution res = SettingContainer.instance.m_SettingData.resolutionList[tmpSettingData.resolutionIndex];
            resolution.text = $"{res.width} * {res.height}";
        }

        private void DoWhenWindowScreen()
        {
            fullScreen.sprite = fullScreenImages[0];
            windowScreen.sprite = windowScreenImages[1];
            resolutionDown.interactable = true;
            resolutionUp.interactable = true;
            resolution.color = new Color(50 / 255f, 50 / 255f, 50 / 255f, 1f);
        }

        public void DownResolution()
        {
            PlayClickSound();
            tmpSettingData.resolutionIndex = Mathf.Max(0, tmpSettingData.resolutionIndex - 1);
            var res = tmpSettingData.resolutionList[tmpSettingData.resolutionIndex];
            resolution.text = $"{res.width} * {res.height}";
        }

        public void UpResolution()
        {
            PlayClickSound();
            tmpSettingData.resolutionIndex = Mathf.Min(maxResolutionIndex, tmpSettingData.resolutionIndex + 1);
            var res = tmpSettingData.resolutionList[tmpSettingData.resolutionIndex];
            resolution.text = $"{res.width} * {res.height}";
        }

        public void ApplyGraphic()
        {
            PlayClickSound();
            var res = tmpSettingData.resolutionList[tmpSettingData.resolutionIndex];
            switch (tmpSettingData.screenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    Screen.SetResolution(res.width, res.height, FullScreenMode.ExclusiveFullScreen);
                    SettingContainer.instance.m_SettingData.screenMode = FullScreenMode.ExclusiveFullScreen;
                    break;

                case FullScreenMode.MaximizedWindow:
                    Screen.SetResolution(res.width, res.height, FullScreenMode.MaximizedWindow);
                    SettingContainer.instance.m_SettingData.screenMode = FullScreenMode.MaximizedWindow;
                    break;

                case FullScreenMode.Windowed:
                    Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed);
                    SettingContainer.instance.m_SettingData.screenMode = FullScreenMode.Windowed;

                    SettingContainer.instance.m_SettingData.windowResolutionIndex = tmpSettingData.resolutionIndex;
                    break;
            }
            SettingContainer.instance.m_SettingData.resolutionIndex = tmpSettingData.resolutionIndex;
        }
        #endregion

        #region Sound
        public void ToOptionSound()
        {
            PlayClickSound();
            graphic.SetActive(false);
            sound.SetActive(true);
            control.SetActive(false);

            ResetTmpSettingData();

            master.value = SettingContainer.instance.m_SettingData.masterVolume;
            bgm.value = SettingContainer.instance.m_SettingData.BGMVolume;
            effectSound.value = SettingContainer.instance.m_SettingData.effectVolume;
        }

        public void SetMaster(float sliderValue) => tmpSettingData.masterVolume = sliderValue;
        public void SetBGM(float sliderValue) => tmpSettingData.BGMVolume = sliderValue;
        public void SetEffectSound(float sliderValue) => tmpSettingData.effectVolume = sliderValue;
        public void DownMaster()
        {
            PlayClickSound();
            master.value -= 0.05f;
        }
        public void UpMaster()
        {
            PlayClickSound();
            master.value += 0.05f;
        }
        public void DownBGM()
        {
            PlayClickSound();
            bgm.value -= 0.05f;
        }

        public void UpBGM()
        {
            PlayClickSound();
            bgm.value += 0.05f;
        }
        public void DownEffectSound()
        {
            PlayClickSound();
            effectSound.value -= 0.05f;
        }
        public void UpEffectSound()
        {
            PlayClickSound();
            effectSound.value += 0.05f;
        }
        public void ApplySound()
        {
            PlayClickSound();
            float masterVolume = tmpSettingData.masterVolume;
            SettingContainer.instance.m_SettingData.masterVolume = masterVolume;
            masterVolume = Mathf.Log10(masterVolume) * 20; // Convert slider value to decibels
            audioController.SetMasterVolume(masterVolume);

            float BGMVolume = tmpSettingData.BGMVolume;
            SettingContainer.instance.m_SettingData.BGMVolume = BGMVolume;
            BGMVolume = Mathf.Log10(BGMVolume) * 20; // Convert slider value to decibels
            audioController.SetBGMVolume(BGMVolume);

            float effectVolume = tmpSettingData.effectVolume;
            SettingContainer.instance.m_SettingData.effectVolume = effectVolume;
            effectVolume = Mathf.Log10(effectVolume) * 20; // Convert slider value to decibels
            audioController.SetEffectVolume(effectVolume);
        }
        #endregion

        public void ToOptionControl()
        {
            PlayClickSound();
            graphic.SetActive(false);
            sound.SetActive(false);
            control.SetActive(true);

            ResetTmpSettingData();
        }

        private void ResetTmpSettingData()
        {
            tmpSettingData.masterVolume = SettingContainer.instance.m_SettingData.masterVolume;
            tmpSettingData.BGMVolume = SettingContainer.instance.m_SettingData.BGMVolume;
            tmpSettingData.effectVolume = SettingContainer.instance.m_SettingData.effectVolume;

            tmpSettingData.resolutionIndex = SettingContainer.instance.m_SettingData.resolutionIndex;
            tmpSettingData.screenMode = SettingContainer.instance.m_SettingData.screenMode;

            tmpSettingData.windowResolutionIndex = SettingContainer.instance.m_SettingData.windowResolutionIndex;
        }
        #endregion


        public void ToLoad()
        {
        }

        public void QuitGame()
        {

            PlayClickSound();
            Application.Quit();
        }

        private void PlayClickSound()
        {
            if (audioController.effectSound != null && clickSound != null)
            {
                audioController.effectSound.PlayOneShot(clickSound, 1f);
            }
        }
    }
}
