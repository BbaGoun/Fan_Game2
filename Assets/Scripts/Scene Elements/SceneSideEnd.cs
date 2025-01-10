using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class SceneSideEnd : MonoBehaviour
    {
        [SerializeField]
        private Transform outPoint;
        [SerializeField]
        private string nextSceneName;
        [SerializeField]
        private LoadingManager.SpawnPoint nextSpawnPoint;
        [SerializeField]
        private LoadingManager.WithWalkOut walkOut;
        [SerializeField]
        private LoadingManager.TransitionMode transitionMode;
        bool alreadyDid;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (LoadingManager.Instance.CheckIsLoadDone())
                {
                    if (!alreadyDid)
                    {
                        alreadyDid = true;

                        var player = collision.GetComponent<PlayerWithStateMachine>();

                        StartCoroutine(IEF());

                        IEnumerator IEF()
                        {
                            player.playerMoveState.MoveXFromTo(player.transform.localPosition, outPoint.localPosition);
                            yield return new WaitUntil(player.playerMoveState.IsCoroutineDone);

                            LoadingManager.Instance.LoadSceneAsync(nextSceneName, nextSpawnPoint, walkOut, transitionMode);
                            yield return null;
                        }
                    }
                }
            }
        }

        public void ChangeNextScene(string other)
        {
            nextSceneName = other;
        }
    }
}
