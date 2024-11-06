using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionPart.UI
{
    public class MetaGameController : MonoBehaviour
    {
        /// <summary>
        /// The main UI object which used for the menu.
        /// </summary>
        public MainMenuController mainMenuController;
        public GameObject interfaces;
        public PlayerInput playerInput;

        bool showMainCanvas = false;

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
                //playerInput.SwitchCurrentActionMap("UI");
            }
            else
            {
                mainMenuController.ToggleMainMenu(false);
                interfaces.SetActive(true);
                //playerInput.SwitchCurrentActionMap("Player");
            }
            this.showMainCanvas = show;
        }

        public void MainMenuButton(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("´­·¶´ç");
                ToggleMainMenu(show: !showMainCanvas);
            }
        }
    }
}
