using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart.UI
{
    public class ElevatorLever : MonoBehaviour
    {
        private GameObject upArrow;
        private PlayerWithStateMachine player;
        private bool isInteracting;

        public Animator elevatorAnim;

        private void Awake()
        {
            upArrow = transform.GetChild(0).gameObject;
            upArrow.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (isInteracting)
            {
                upArrow.SetActive(false);
            }
            else if (collision.tag.Equals("Player"))
            {
                upArrow.SetActive(true);
                if (player == null)
                    player = collision.GetComponent<PlayerWithStateMachine>();
                player.InTalkArea();

                if (player.CheckReadyTalk() && player.isGrounded)
                {
                    isInteracting = true;
                    upArrow.SetActive(false);

                    elevatorAnim.SetTrigger("Up");
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                upArrow.SetActive(false);
                player.OutTalkArea();
            }
        }
    }
}
