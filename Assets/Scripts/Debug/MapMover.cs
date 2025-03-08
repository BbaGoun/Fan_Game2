using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace ActionPart
{
    public class MapMover : MonoBehaviour
    {
        public TMPro.TMP_InputField inputField;

        public void FocusInputField()
        {
            PlayerInputPart.Instance.CantInput();
        }

        public void UnFocusInputField()
        {
            PlayerInputPart.Instance.CanInput();
        }

        public void MoveMap()
        {
            var mapName = inputField.text;
            LoadingManager.Instance.LoadSceneAsync(mapName, LoadingManager.SpawnPoint.Left, LoadingManager.WithWalkOut.Left, LoadingManager.TransitionMode.FromRight);
            inputField.text = "";
        }
    }
}
