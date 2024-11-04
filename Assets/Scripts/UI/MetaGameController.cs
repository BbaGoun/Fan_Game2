using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart.UI
{
    public class MetaGameController : MonoBehaviour
    {
        /// <summary>
        /// The main UI object which used for the menu.
        /// </summary>
        public MainMenuController mainMenuController;
        public InterfaceController interfaceController;

        /// <summary>
        /// A list of canvas objects which are used during gameplay (when the main ui is turned off)
        /// </summary>
        public Canvas[] gamePlayCanvases;

        bool showMainCanvas = false;

        /// <summary>
        /// Turn the main menu on or off.
        /// </summary>
        /// <param name="show"></param>
        public void ToggleMainMenu(bool show)
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
                foreach (var i in gamePlayCanvases)
                    i.gameObject.SetActive(false);
            }
            else
            {
                mainMenuController.ToggleMainMenu(false);
                foreach (var i in gamePlayCanvases)
                    i.gameObject.SetActive(true);
            }
            this.showMainCanvas = show;
        }

        void Update()
        {
            if (Input.GetButtonDown("Menu"))
            {
                ToggleMainMenu(show: !showMainCanvas);
            }
        }

        public void OptionButton()
        {
            ToggleMainMenu(show: !showMainCanvas);
        }
    }
}
