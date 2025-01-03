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

        private void Awake()
        {
            upArrow = transform.GetChild(0).gameObject;
            upArrow.SetActive(false);
        }

        public void SetIsInteracting(bool _isInteracting)
        {
            isInteracting = _isInteracting;
        }

        public void ElevatorDown()
        {
            SetIsInteracting(true);
            elevatorAnim.SetTrigger("Down");
        }

        public void ElevatorDownGo()
        {
            StartCoroutine(IEElevatorDownGo());

            IEnumerator IEElevatorDownGo()
            {
                var gap = 3.625f / 40f;
                for(int i = 0; i < 40; i++)
                {
                    elevator.localPosition = new Vector3(elevator.localPosition.x, elevator.localPosition.y - gap, elevator.localPosition.z);
                    yield return new WaitForFixedUpdate();
                }
            }
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
