using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class NPCTalk : MonoBehaviour
    {
        public string talkEventName;

        private GameObject talkBalloon;
        private GameObject upArrow;
        private PlayerWithStateMachine player;
        private bool isTalking;

        private void Awake()
        {
            talkBalloon = transform.GetChild(0).gameObject;
            talkBalloon.SetActive(false);

            upArrow = transform.GetChild(1).gameObject;
            upArrow.SetActive(false);
        }

        public void SetIsTalking(bool value)
        {
            isTalking = value;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (isTalking)
            {
                upArrow.SetActive(false);
                talkBalloon.SetActive(true);
            }
            else if (collision.tag.Equals("Player"))
            {
                upArrow.SetActive(true);
                talkBalloon.SetActive(false);
                if(player == null)
                    player = collision.GetComponent<PlayerWithStateMachine>();
                player.InTalkArea();

                if (player.CheckReadyTalk() && player.isGrounded)
                {
                    upArrow.SetActive(false);
                    talkBalloon.SetActive(true);
                    this.transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x) * Mathf.Sign(player.transform.localPosition.x - this.transform.localPosition.x), this.transform.localScale.y, this.transform.localScale.z);
                    Debug.Log(this.transform.localScale);
                    TalkManager.Instance.TalkStart(talkEventName, this);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                upArrow.SetActive(false);
                talkBalloon.SetActive(false);
                player.OutTalkArea();
            }
        }
    }
}
