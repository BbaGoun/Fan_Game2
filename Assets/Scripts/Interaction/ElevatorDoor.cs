using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class ElevatorDoor : MonoBehaviour
    {
        [SerializeField]
        GameObject background;
        [SerializeField]
        GameObject map;
        Animator animator;


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

        public void MapOff_Button()
        {
            animator.SetTrigger("isClosed");
        }

        public void MapOff_CloseDoor()
        {
            background.SetActive(false);
            map.SetActive(false);
        }

        public void MapOff_OpenDoor()
        {
            this.gameObject.SetActive(false);
        }
    }

}
