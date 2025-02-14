using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Portal : MonoBehaviour
    {
        [SerializeField]
        private string nextSceneName;
        [SerializeField]
        private LoadingManager.SpawnPoint nextSpawnPoint;
        [SerializeField]
        private LoadingManager.WithWalkOut walkOut;
        [SerializeField]
        private LoadingManager.TransitionMode transitionMode;
        bool alreadyDid;

        private GameObject upArrow;
        private PlayerWithStateMachine player;
        private bool isTalking;

        private void Awake()
        {
            upArrow = transform.GetChild(0).gameObject;
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
            }
            else if (collision.tag.Equals("Player"))
            {
                if (LoadingManager.Instance.CheckIsLoadDone())
                {
                    upArrow.SetActive(true);
                    if (player == null)
                        player = collision.GetComponent<PlayerWithStateMachine>();
                    player.InTalkArea();

                    if (player.CheckReadyTalk() && player.isGrounded)
                    {
                        upArrow.SetActive(false);
                        isTalking = true;
                        LoadingManager.Instance.LoadSceneAsync(nextSceneName, nextSpawnPoint, walkOut, transitionMode);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                upArrow.SetActive(false);
                player?.OutTalkArea();
            }
        }

        public void ChangeNextScene(string other)
        {
            nextSceneName = other;
        }
    }
}
