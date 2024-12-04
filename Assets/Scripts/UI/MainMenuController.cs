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
        public Image fullScreen, windowScreen;
        public Sprite[] fullScreenImages = new Sprite[2];
        public Sprite[] windowScreenImages = new Sprite[2];
        public TMP_Text resolution;
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

            maxScreenIndex = SettingContainer.instance.m_SettingData.resolutionList.Count - 1;

            foreach (var resolution in SettingContainer.instance.m_SettingData.resolutionList)
            {
                //var newOption = new List<string> { $"{resolution.width} * {resolution.height}" };
                //resolutions.AddOptions(newOption);
            }

            Resolution res = SettingContainer.instance.m_SettingData.resolutionList[SettingContainer.instance.m_SettingData.resolutionIndex];
            resolution.text = $"{res.width} * {res.height}";
            //resolutions.value = SettingContainer.instance.m_SettingData.resolutionIndex;

            switch (SettingContainer.instance.m_SettingData.screenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    fullScreen.sprite = fullScreenImages[1];
                    windowScreen.sprite = windowScreenImages[0];
                    //screenModes.value = 1;
                    break;
                case FullScreenMode.MaximizedWindow:
                    fullScreen.sprite = fullScreenImages[1];
                    windowScreen.sprite = windowScreenImages[0];
                    //screenModes.value = 1;
                    break;
                case FullScreenMode.Windowed:
                    fullScreen.sprite = fullScreenImages[0];
                    windowScreen.sprite = windowScreenImages[1];
                    //screenModes.value = 2;
                    break;
            }

            //resolutions.interactable = SettingContainer.instance.m_SettingData.screenMode == FullScreenMode.Windowed;

            master.value = SettingContainer.instance.m_SettingData.masterVolume;
            bgm.value = SettingContainer.instance.m_SettingData.BGMVolume;
            effectSound.value = SettingContainer.instance.m_SettingData.effectVolume;
        }

        private void Start()
        {
            SetMaster(SettingContainer.instance.m_SettingData.masterVolume);
            SetBGM(SettingContainer.instance.m_SettingData.BGMVolume);
            SetEffectSound(SettingContainer.instance.m_SettingData.effectVolume);
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

        public void ShowTitleAlert()
        {
            PlayClickSound();
            foreach (var button in buttons)
            {
                button.interactable = false;
            }
            titleAlert.SetActive(true);
        }

        public void NoTitle()
        {
            PlayClickSound();
            foreach (var button in buttons)
            {
                button.interactable = true;
            }
            titleAlert.SetActive(false);
        }

        public void YesTitle()
        {
            PlayClickSound();
            Time.timeScale = 1;
            //LoadingManager.Instance.sceneName = "메인 타이틀";
            //LoadingManager.Instance.LoadSceneAsync("메인 타이틀");
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
            var res = SettingContainer.instance.m_SettingData.resolutionList[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
            SettingContainer.instance.m_SettingData.resolutionIndex = index;
            resolutionIndex = index;
            // 창모드 해상도 저장
            SettingContainer.instance.m_SettingData.windowResolutionIndex = index;
        }

        public void ChangeScreenMode(int index)
        {
            var resMax = SettingContainer.instance.m_SettingData.resolutionList[maxScreenIndex];
            switch (index)
            {
                case 0:
                    Screen.SetResolution(resMax.width, resMax.height, FullScreenMode.FullScreenWindow);
                    SettingContainer.instance.m_SettingData.screenMode = FullScreenMode.FullScreenWindow;
                    
                    resolutions.interactable = false;

                    // 창모드 해상도 저장
                    SettingContainer.instance.m_SettingData.windowResolutionIndex = resolutionIndex;

                    resolutionIndex = maxScreenIndex;
                    SettingContainer.instance.m_SettingData.resolutionIndex = maxScreenIndex;
                    resolutions.value = resolutionIndex;

                    break;
                case 1:
                    if(Application.platform == RuntimePlatform.WindowsPlayer) // window
                    {
                        Screen.SetResolution(resMax.width, resMax.height, FullScreenMode.ExclusiveFullScreen);
                        SettingContainer.instance.m_SettingData.screenMode = FullScreenMode.ExclusiveFullScreen;
                    }
                    else if(Application.platform == RuntimePlatform.OSXPlayer) // macOS
                    {
                        Screen.SetResolution(resMax.width, resMax.height, FullScreenMode.MaximizedWindow);
                        SettingContainer.instance.m_SettingData.screenMode = FullScreenMode.MaximizedWindow;
                    }

                    resolutions.interactable = false; 
                    
                    // 창모드 해상도 저장
                    SettingContainer.instance.m_SettingData.windowResolutionIndex = resolutionIndex;

                    resolutionIndex = maxScreenIndex;
                    SettingContainer.instance.m_SettingData.resolutionIndex = maxScreenIndex;
                    resolutions.value = resolutionIndex;
                    break;
                case 2:
                    // 저장해놨던 창모드 해상도를 불러옴
                    var winIndex = SettingContainer.instance.m_SettingData.windowResolutionIndex;
                    var winRes = SettingContainer.instance.m_SettingData.resolutionList[winIndex];
                    Screen.SetResolution(winRes.width, winRes.height, FullScreenMode.Windowed);
                    
                    resolutions.interactable = true;

                    resolutionIndex = winIndex;
                    SettingContainer.instance.m_SettingData.resolutionIndex = winIndex;
                    resolutions.value = winIndex;
                    
                    break;
            }
        }

        public void SetMaster(float sliderValue)
        {
            SettingContainer.instance.m_SettingData.masterVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetMasterVolume(volume);
        }

        public void SetBGM(float sliderValue)
        {
            SettingContainer.instance.m_SettingData.BGMVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetBGMVolume(volume);
        }

        public void SetEffectSound(float sliderValue)
        {
            SettingContainer.instance.m_SettingData.effectVolume = sliderValue;
            float volume = Mathf.Log10(sliderValue) * 20; // Convert slider value to decibels
            audioController.SetEffectVolume(volume);
        }

        public void ApplySetting()
        {
            // 임시 settingData를 확정시켜 데이터로도 저장시키는 역할
        }
    }
}
