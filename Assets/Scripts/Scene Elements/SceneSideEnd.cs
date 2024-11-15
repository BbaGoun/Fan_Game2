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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (LoadingManager.Instance.CheckIsLoadDone())
                {
                    var player = collision.GetComponent<PlayerWithStateMachine>();

                    player.playerMoveState.MoveXFromTo(player.transform, outPoint);
                    while (player.playerMoveState.IsCoroutineDone()) ;

                    LoadingManager.Instance.LoadSceneAsync(nextSceneName, nextSpawnPoint, walkOut, transitionMode);
                }
            }
        }
    }
}
