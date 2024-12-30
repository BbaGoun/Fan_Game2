using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ActionPart
{
    public class GlobalTimelineController : MonoBehaviour
    {
        public static GlobalTimelineController instance;

        PlayableDirector playableDirector;
        public List<TimelineAsset> timeAssets;
        public LocalTimelineController currentLocalTimelineController;

        public void Initialize()
        {
            #region Singleton
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
            #endregion

            playableDirector = GetComponent<PlayableDirector>();
        }

        public void ChangeCurrentLocalTimelineController(LocalTimelineController other)
        {
            currentLocalTimelineController = other;
        }

        public void PlayLocalTimeline(string timelineName)
        {
            PlayTimeline(timelineName);
            currentLocalTimelineController.PlayTimeline(timelineName);
        }

        public void PlayTimeline(string timelineName)
        {
            foreach (TimelineAsset asset in timeAssets)
            {
                if (asset.name.Equals(timelineName))
                {
                    playableDirector.Play(asset);
                    //PlayerWithStateMachine.Instance.PauseAnimator();
                    PlayerInputPart.Instance.CantInput();
                    break;
                }
            }
        }

        public void PlayerCanMove()
        {
            PlayerWithStateMachine.Instance.ApplyRootMotionTemp();
            PlayerWithStateMachine.Instance.PlayAnimator();
            PlayerInputPart.Instance.CanInput();
        }

        public void PlayerMoveXTo(float x)
        {
            var transformFrom = PlayerWithStateMachine.Instance.transform.localPosition;
            var transformTo = new Vector3(x, transformFrom.y, transformFrom.z);

            PlayerWithStateMachine.Instance.playerMoveState.MoveXFromTo(transformFrom, transformTo);
        }
    }
}
