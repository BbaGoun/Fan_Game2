using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionPart
{
    public class MetaGameController : MonoBehaviour
    {
        public static MetaGameController Instance;
        /// <summary>
        /// The main UI object which used for the menu.
        /// </summary>
        public MainMenuController mainMenuController;

        private int interfaceOffCount;
        public GameObject interfaces;
        public PlayerInput playerInput;

        bool showMainCanvas = false;

        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else if(Instance != this)
            {
                Destroy(this.gameObject);
            }
            #endregion

            interfaceOffCount = 0;
        }

        /// <summary>
        /// Turn the main menu on or off.
        /// </summary>
        /// <param name="show"></param>
        void ToggleMainMenu(bool show)
        {
            if (this.showMainCanvas != show)
            {
                _ToggleMainMenu(show);
            }
        }

        void _ToggleMainMenu(bool show)
        {
            if (show)
            {
                mainMenuController.ToggleMainMenu(true);
                interfaces.SetActive(false);
                SwitchActionMap(ActionMap.UI);
            }
            else
            {
                mainMenuController.ToggleMainMenu(false);
                interfaces.SetActive(true);
                SwitchActionMap(ActionMap.Player);
            }
            this.showMainCanvas = show;
        }

        public void SwitchActionMap(ActionMap actionMap)
        {
            switch (actionMap)
            {
                case ActionMap.Player:
                    playerInput.SwitchCurrentActionMap("Player");
                    break;
                case ActionMap.UI:
                    playerInput.SwitchCurrentActionMap("UI");
                    break;
            }
        }

        public enum ActionMap
        {
            Player,
            UI
        }

        public void ShowInterface()
        {
            interfaceOffCount = Mathf.Max(interfaceOffCount - 1, 0);
            Debug.Log($"인터페이스 켜기 : {interfaceOffCount}");
            if(interfaceOffCount == 0)
                interfaces.SetActive(true);
        }

        public void DisShowInterface()
        {
            interfaceOffCount += 1;
            Debug.Log($"인터페이스 끄기 : {interfaceOffCount}");
            interfaces.SetActive(false);
        }

        public void MainMenuKey(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                var loadedSceneName = LoadingManager.Instance.GetLoadedSceneName();

                if (loadedSceneName == null)
                    return;
                else if (loadedSceneName.Equals("메인 타이틀"))
                    return;
                else if (loadedSceneName.Contains("만화컷"))
                    return;
                else if (LoadingManager.Instance.CheckCartoonOn())
                    return;

                ToggleMainMenu(show: !showMainCanvas);
            }
        }

        public void MainMenuButton()
        {
            ToggleMainMenu(show: !showMainCanvas);
        }

        public void OffMainMenu()
        {
            mainMenuController.OffMainMenu();
        }
    }
}
