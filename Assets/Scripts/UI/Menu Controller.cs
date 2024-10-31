using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ActionPart.UI
{
    public class MainMenuController : MonoBehaviour
    {
        private AudioController audioController;
        public AudioClip clickSound;

        public Button[] buttons;
        private GameObject background;
        private GameObject mainMenu;
        private GameObject mainAlert;
        private GameObject quitAlert;
        private GameObject option;
        private GameObject saveGame;
        public TMP_Dropdown resolutions, screenModes;
        public Slider master, bgm, effectSound;
        int resolutionIndex;
        int maxScreenIndex;

        private void Awake()
        {
            audioController = GameObject.Find("AudioController").GetComponent<AudioController>();

            background = transform.GetChild(0).gameObject;
            background.SetActive(false);
            mainMenu = transform.GetChild(1).gameObject;
            mainMenu.SetActive(false);
            mainAlert = mainMenu.transform.GetChild(2).GetChild(5).gameObject;
            mainAlert.SetActive(false);
            quitAlert = mainMenu.transform.GetChild(2).GetChild(6).gameObject;
            quitAlert.SetActive(false);
            option = transform.GetChild(2).gameObject;
            option.SetActive(false);
            saveGame = transform.GetChild(3).gameObject;
            saveGame.SetActive(false);
            master.minValue = 0.001f;
            bgm.minValue = 0.001f;
            effectSound.minValue = 0.001f;

            maxScreenIndex = SettingContainer.resolutionList.Count - 1;

            foreach (var resolution in SettingContainer.resolutionList)
            {
                var newOption = new List<string> { $"{resolution.width} * {resolution.height}" };
                resolutions.AddOptions(newOption);
            }

            resolutions.value = SettingContainer.resolutionIndex;

            screenModes.value = (int)SettingContainer.screenMode;
            resolutions.interactable = SettingContainer.screenMode == SettingContainer.ScreenMode.Windowed;

            master.value = SettingContainer.masterVolume;
            bgm.value = SettingContainer.BGMVolume;
            effectSound.value = SettingContainer.effectVolume;
        }

        private void Start()
        {
            SetMaster(SettingContainer.masterVolume);
            SetBGM(SettingContainer.BGMVolume);
            SetEffectSound(SettingContainer.effectVolume);
        }

        public void ToggleMainMenu(bool isShowMenu)
        {
            PlayClickSound();
            if (isShowMenu)
            {
                Time.timeScale = 0;
                background.SetActive(true);
                mainMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                foreach (var button in buttons)
                {
                    button.interactable = true;
                }
                background.SetActive(false);
                mainMenu.SetActive(false);
                mainAlert.SetActive(false);
                quitAlert.SetActive(false);
                option.SetActive(false);
                saveGame.SetActive(false);
            }
        }

        public void ShowMainAlert()
        {
            PlayClickSound();
            foreach (var button in buttons)
            {
                button.interactable = false;
            }
            mainAlert.SetActive(true);
        }

        public void NoMain()
        {
            PlayClickSound();
            foreach (var button in buttons)
            {
                button.interactable = true;
            }
            mainAlert.SetActive(false);
        }

        public void YesMain()
        {
            PlayClickSound();
            Time.timeScale = 1;
            LoadingManager.Instance.sceneName = "메인 타이틀";
            LoadingManager.Instance.LoadSceneAsync("일반 로딩");
        }

        public void ToSaveGame()
        {
            PlayClickSound();
            mainMenu.SetActive(false);
            mainAlert.SetActive(false);
            quitAlert.SetActive(false);
            saveGame.SetActive(true);
        }

        public void CloseSaveGame()
        {
            PlayClickSound();
            mainMenu.SetActive(true);
            saveGame.SetActive(false);
        }

        public void ToOption()
        {
            PlayClickSound();
            mainMenu.SetActive(false);
            mainAlert.SetActive(false);
            quitAlert.SetActive(false);
            option.SetActive(true);
        }

        public void CloseOption()
        {
            PlayClickSound();
            mainMenu.SetActive(true);
            option.SetActive(false);
        }

        public void ShowQuitAlert()
        {
            PlayClickSound();
            foreach (var button in buttons)
            {
                button.interactable = false;
            }
            quitAlert.SetActive(true);
        }

        public void NoQuit()
        {
            PlayClickSound();
            foreach (var button in buttons)
            {
                button.interactable = true;
            }
            quitAlert.SetActive(false);
        }

        public void YesQuit()
        {
            PlayClickSound();
            // 저장 추가
            Application.Quit();
        }

        private void PlayClickSound()
        {
            if (audioController.effectSound != null && clickSound != null)
            {
                audioController.effectSound.PlayOneShot(clickSound, 1f);
            }
        }

        public void ChangeResolution(int index)
        {
            Screen.SetResolution(SettingContainer.resolutionList[index].width, SettingContainer.resolutionList[index].height, Screen.fullScreenMode);
            SettingContainer.resolutionIndex = index;
            resolutionIndex = index;
        }

        public void ChangeScreenMode(int index)
        {
            switch (index)
            {
                case 0:
                    Screen.SetResolution(SettingContainer.resolutionList[maxScreenIndex].width, SettingContainer.resolutionList[maxScreenIndex].height, FullScreenMode.FullScreenWindow);
                    resolutions.interactable = false;
                    resolutionIndex = maxScreenIndex;
                    SettingContainer.resolutionIndex = maxScreenIndex;
                    SettingContainer.screenMode = (SettingContainer.ScreenMode)0;
                    //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.SetResolution(SettingContainer.resolutionList[maxScreenIndex].width, SettingContainer.resolutionList[maxScreenIndex].height, FullScreenMode.ExclusiveFullScreen);
                    resolutions.interactable = false;
                    resolutionIndex = maxScreenIndex;
                    SettingContainer.resolutionIndex = maxScreenIndex;
                    SettingContainer.screenMode = (SettingContainer.ScreenMode)1;
                    //Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case 2:
                    resolutions.interactable = true;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    SettingContainer.screenMode = (SettingContainer.ScreenMode)2;
                    break;
            }
        }

        public void SetMaster(float sliderValue)
        {
            SettingContainer.masterVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetMasterVolume(volume);
        }

        public void SetBGM(float sliderValue)
        {
            SettingContainer.BGMVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetBGMVolume(volume);
        }

        public void SetEffectSound(float sliderValue)
        {
            SettingContainer.effectVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetEffectVolume(volume);
        }
    }
}
