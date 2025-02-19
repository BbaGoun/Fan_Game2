using ActionPart.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Map_Interaction : MonoBehaviour
    {
        public ElevatorDoor elevatorDoor;
        [SerializeField]
        List<GameObject> locals = new List<GameObject>();
        [SerializeField]
        List<GameObject> popUps = new List<GameObject>();

        public void SelectLocal(int index)
        {
            foreach (GameObject go in locals)
            {
                go.SetActive(false);
            }

            locals[index].SetActive(true);
        }

        public void PopUpOn(int index)
        {
            foreach (GameObject go in popUps)
            {
                go.SetActive(false);
            }

            popUps[index].SetActive(true);
        }

        public void PopUp_Yes(int index)
        {
            elevatorDoor.MapOff_Button(index);
        }

        public void PopUp_No(int index)
        {
            popUps[index].SetActive(false);
        }

    }
}
