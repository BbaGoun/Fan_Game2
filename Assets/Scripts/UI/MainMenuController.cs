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
        private GameObject titleAlert;
        private GameObject quitAlert;
        private GameObject option;
        private GameObject saveGame;

        public TMP_Dropdown resolutions, screenModes;
        public Slider master, bgm, effectSound;
        int resolutionIndex;
        int maxScreenIndex;

        private SettingData tmpSettingData = new SettingData();

        private void Awake()
        {
            audioController = GameObject.Find("==== Audio ====").GetComponent<AudioController>();

            background = transform.GetChild(0).gameObject;
            background.SetActive(false);

            mainMenu = transform.GetChild(1).gameObject;
            mainMenu.SetActive(false);
            titleAlert = mainMenu.transform.GetChild(2).GetChild(5).gameObject;
            titleAlert.SetActive(false);
            quitAlert = mainMenu.transform.GetChild(2).GetChild(6).gameObject;
            quitAlert.SetActive(false);

            option = transform.GetChild(2).gameObject;
            option.SetActive(false);

            saveGame = transform.GetChild(3).gameObject;
            saveGame.SetActive(false);

            master.minValue = 0.001f;
            bgm.minValue = 0.001f;
            effectSound.minValue = 0.001f;

            maxScreenIndex = SettingContainer.instance.settingData.resolutionList.Count - 1;

            foreach (var resolution in SettingContainer.instance.settingData.resolutionList)
            {
                var newOption = new List<string> { $"{resolution.width} * {resolution.height}" };
                resolutions.AddOptions(newOption);
            }

            resolutions.value = SettingContainer.instance.settingData.resolutionIndex;

            switch (SettingContainer.instance.settingData.screenMode)
            {
                case FullScreenMode.FullScreenWindow:
                    screenModes.value = 0;
                    break;
                case FullScreenMode.ExclusiveFullScreen:
                    screenModes.value = 1;
                    break;
                case FullScreenMode.MaximizedWindow:
                    screenModes.value = 1;
                    break;
                case FullScreenMode.Windowed:
                    screenModes.value = 2;
                    break;
            }

            resolutions.interactable = SettingContainer.instance.settingData.screenMode == FullScreenMode.Windowed;

            master.value = SettingContainer.instance.settingData.masterVolume;
            bgm.value = SettingContainer.instance.settingData.BGMVolume;
            effectSound.value = SettingContainer.instance.settingData.effectVolume;
        }

        private void Start()
        {
            SetMaster(SettingContainer.instance.settingData.masterVolume);
            SetBGM(SettingContainer.instance.settingData.BGMVolume);
            SetEffectSound(SettingContainer.instance.settingData.effectVolume);
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
                titleAlert.SetActive(false);
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
            titleAlert.SetActive(true);
        }

        public void NoMain()
        {
            PlayClickSound();
            foreach (var button in buttons)
            {
                button.interactable = true;
            }
            titleAlert.SetActive(false);
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
            titleAlert.SetActive(false);
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
            titleAlert.SetActive(false);
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
            var res = SettingContainer.instance.settingData.resolutionList[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
            SettingContainer.instance.settingData.resolutionIndex = index;
            resolutionIndex = index;
            // 창모드 해상도 저장
            SettingContainer.instance.settingData.windowResolutionIndex = index;
        }

        public void ChangeScreenMode(int index)
        {
            var resMax = SettingContainer.instance.settingData.resolutionList[maxScreenIndex];
            switch (index)
            {
                case 0:
                    Screen.SetResolution(resMax.width, resMax.height, FullScreenMode.FullScreenWindow);
                    SettingContainer.instance.settingData.screenMode = FullScreenMode.FullScreenWindow;
                    
                    resolutions.interactable = false;

                    // 창모드 해상도 저장
                    SettingContainer.instance.settingData.windowResolutionIndex = resolutionIndex;

                    resolutionIndex = maxScreenIndex;
                    SettingContainer.instance.settingData.resolutionIndex = maxScreenIndex;
                    resolutions.value = resolutionIndex;

                    break;
                case 1:
                    if(Application.platform == RuntimePlatform.WindowsPlayer) // window
                    {
                        Screen.SetResolution(resMax.width, resMax.height, FullScreenMode.ExclusiveFullScreen);
                        SettingContainer.instance.settingData.screenMode = FullScreenMode.ExclusiveFullScreen;
                    }
                    else if(Application.platform == RuntimePlatform.OSXPlayer) // macOS
                    {
                        Screen.SetResolution(resMax.width, resMax.height, FullScreenMode.MaximizedWindow);
                        SettingContainer.instance.settingData.screenMode = FullScreenMode.MaximizedWindow;
                    }

                    resolutions.interactable = false; 
                    
                    // 창모드 해상도 저장
                    SettingContainer.instance.settingData.windowResolutionIndex = resolutionIndex;

                    resolutionIndex = maxScreenIndex;
                    SettingContainer.instance.settingData.resolutionIndex = maxScreenIndex;
                    resolutions.value = resolutionIndex;
                    break;
                case 2:
                    // 저장해놨던 창모드 해상도를 불러옴
                    var winIndex = SettingContainer.instance.settingData.windowResolutionIndex;
                    var winRes = SettingContainer.instance.settingData.resolutionList[winIndex];
                    Screen.SetResolution(winRes.width, winRes.height, FullScreenMode.Windowed);
                    
                    resolutions.interactable = true;

                    resolutionIndex = winIndex;
                    SettingContainer.instance.settingData.resolutionIndex = winIndex;
                    resolutions.value = winIndex;
                    
                    break;
            }
        }

        public void SetMaster(float sliderValue)
        {
            SettingContainer.instance.settingData.masterVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetMasterVolume(volume);
        }

        public void SetBGM(float sliderValue)
        {
            SettingContainer.instance.settingData.BGMVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetBGMVolume(volume);
        }

        public void SetEffectSound(float sliderValue)
        {
            SettingContainer.instance.settingData.effectVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetEffectVolume(volume);
        }

        public void ApplySetting()
        {
            // 임시 settingData를 확정시켜 데이터로도 저장시키는 역할
        }
    }
}
