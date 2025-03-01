using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart.UI
{
    public class ElevatorDoor : MonoBehaviour
    {
        public Elevator elevator;

        [SerializeField]
        GameObject background;
        [SerializeField]
        GameObject map;
        Animator animator;
        int mapIndex = 7;


        private void Awake()
        {
            animator = GetComponent<Animator>();    
        }

        public void MapOn_CloseDoor()
        {
            background.SetActive(true);
            map.SetActive(true);
        }

        public void MapOn_OpenDoor()
        {

        }

        public void MapOff_Button(int index)
        {
            mapIndex = index;
            animator.SetTrigger("isClosed");
        }

        public void MapOff_CloseDoor()
        {
            background.SetActive(false);
            map.SetActive(false);
        }

        public void MapOff_OpenDoor()
        {
            if (mapIndex == 7)
            {
                elevator.SetIsInteracting(false);
            }
            else
            {
                elevator.ElevatorDown(mapIndex);
            }

            PlayerInputPart.Instance.CanInput();
            MetaGameController.instance.ShowInterface();
            this.gameObject.SetActive(false);
        }
    }

}
