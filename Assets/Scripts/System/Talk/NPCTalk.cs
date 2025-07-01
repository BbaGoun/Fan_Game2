using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ActionPart
{
    public class NPCTalk : MonoBehaviour, ITalkAble
    {
        public string npcName { get; private set; }

        private NPCTalkData npcTalkData;

        private GameObject talkBalloon;
        private GameObject willTalk;
        private GameObject talking;
        private GameObject upArrow;
        private PlayerWithStateMachine player;
        
        [SerializeField]
        private bool isInTalkArea;
        [SerializeField]
        private bool isTalking;

        private void Awake()
        {
            npcName = gameObject.name;
            player = PlayerWithStateMachine.Instance;

            talkBalloon = transform.GetChild(0).gameObject;

            willTalk = transform.GetChild(0).GetChild(0).gameObject;
            willTalk.SetActive(false);

            talking = transform.GetChild(0).GetChild(1).gameObject;
            talking.SetActive(false);

            upArrow = transform.GetChild(1).gameObject;
            upArrow.SetActive(false);
        }

        public void TalkStart()
        {
            SetIsTalking(true);
        }

        public void TalkDone()
        {
            NPCTalkDataManager.Instance.UpNPCTalkCount(npcName);
            SetIsTalking(false);
        }

        private void SetIsTalking(bool value)
        {
            isTalking = value;
        }

        private void Update()
        {
            npcTalkData = NPCTalkDataManager.Instance.GetNPCTalkData(npcName);
            if (npcTalkData == null)
            {
                Debug.Log($"{npcName}, NPCTalkData 정보 없음");
                return;
            }
            if (npcTalkData.isWaitCutScene)
            {
                talkBalloon.SetActive(false);
                willTalk.SetActive(false);
                talking.SetActive(false);
                upArrow.SetActive(false);

                this.transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x) * Mathf.Sign(player.transform.localPosition.x - this.transform.localPosition.x), this.transform.localScale.y, this.transform.localScale.z);
            }
            else
            {
                var talkCount = npcTalkData.talkCount;
                if (talkCount == 0)
                {
                    if (isInTalkArea)
                    {
                        if (!isTalking && !player.isTalking)
                        {
                            talkBalloon.SetActive(false);
                            willTalk.SetActive(false);
                        }
                    }
                    else
                    {
                        talkBalloon.SetActive(true);
                        willTalk.SetActive(true);
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                if (isTalking || player.isTalking)
                {
                    upArrow.SetActive(false);
                    talkBalloon.SetActive(true);
                    talking.SetActive(true);

                }
                else
                {
                    isInTalkArea = true;
                    upArrow.SetActive(true);
                    talkBalloon.SetActive(false);
                    willTalk.SetActive(false);
                    talking.SetActive(false);
                    player.InTalkArea();

                    if (player.CheckReadyTalk() && player.isGrounded)
                    {
                        this.transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x) * Mathf.Sign(player.transform.localPosition.x - this.transform.localPosition.x), this.transform.localScale.y, this.transform.localScale.z);
                        upArrow.SetActive(false);
                        talkBalloon.transform.localScale = new Vector3(this.transform.localScale.x < 0 ? -1 * Mathf.Sign(talkBalloon.transform.localScale.x) * talkBalloon.transform.localScale.x : Mathf.Sign(talkBalloon.transform.localScale.x) * talkBalloon.transform.localScale.x,
                            talkBalloon.transform.localScale.y, talkBalloon.transform.localScale.z);
                        talkBalloon.SetActive(true);
                        talking.SetActive(true);
                        TalkManager.Instance.TalkStart(npcTalkData.currentTalkEvent, this);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                isInTalkArea = false;
                upArrow.SetActive(false);
                talkBalloon.SetActive(false);
                talking.SetActive(false);
                player?.OutTalkArea();
            }
        }
    }
}
