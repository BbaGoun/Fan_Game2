using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionPart.UI
{
    public class MetaGameController : MonoBehaviour
    {
        public static MetaGameController instance;
        /// <summary>
        /// The main UI object which used for the menu.
        /// </summary>
        public MainMenuController mainMenuController;
        public GameObject interfaces;
        public PlayerInput playerInput;

        bool showMainCanvas = false;

        private void Awake()
        {
            #region Singleton
            if (instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(this.gameObject);
            }
            #endregion
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
                playerInput.SwitchCurrentActionMap("UI");
            }
            else
            {
                mainMenuController.ToggleMainMenu(false);
                interfaces.SetActive(true);
                playerInput.SwitchCurrentActionMap("Player");
            }
            this.showMainCanvas = show;
        }

        public void ShowInterface()
        {
            interfaces.SetActive(true);
        }

        public void DisShowInterface()
        {
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
                ToggleMainMenu(show: !showMainCanvas);
            }
        }

        public void MainMenuButton()
        {
            ToggleMainMenu(show: !showMainCanvas);
        }
    }
}
