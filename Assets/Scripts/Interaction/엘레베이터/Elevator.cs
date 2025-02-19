using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart.UI
{
    public class Elevator : MonoBehaviour
    {
        [SerializeField]
        GameObject door;
        private GameObject upArrow;
        private PlayerWithStateMachine player;
        private bool isInteracting;

        public Transform elevator;
        public Animator elevatorAnim;
        public SceneSideEnd sceneSideEnd;

        private void Awake()
        {
            upArrow = transform.GetChild(0).gameObject;
            upArrow.SetActive(false);
        }

        public void SetIsInteracting(bool _isInteracting)
        {
            isInteracting = _isInteracting;
        }

        public void ElevatorDown(int index)
        {
            switch (index)
            {
                case 0:
                    sceneSideEnd.ChangeNextScene("√¢√µ∞¥¿‹");
                    break;                        
                case 1:                           
                    sceneSideEnd.ChangeNextScene("√¢√µ∞¥¿‹");
                    break;                        
                case 2:                           
                    sceneSideEnd.ChangeNextScene("√¢√µ∞¥¿‹");
                    break;                        
                case 3:                           
                    sceneSideEnd.ChangeNextScene("√¢√µ∞¥¿‹");
                    break;                        
                case 4:                           
                    sceneSideEnd.ChangeNextScene("√¢√µ∞¥¿‹");
                    break;                        
                case 5:                           
                    sceneSideEnd.ChangeNextScene("√¢√µ∞¥¿‹");
                    break;                        
                case 6:                           
                    sceneSideEnd.ChangeNextScene("√¢√µ∞¥¿‹");
                    break;
            }

            SetIsInteracting(true);
            elevatorAnim.SetTrigger("Down");
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
                    PlayerInputPart.Instance.CantInput();
                    isInteracting = true;
                    upArrow.SetActive(false);

                    MetaGameController.instance.DisShowInterface();
                    door.SetActive(true);
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
