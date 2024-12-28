using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ActionPart
{
    public class TimelineController : MonoBehaviour
    {
        public static TimelineController instance;
        PlayableDirector playableDirector;
        public List<TimelineAsset> timeAssets;

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

            playableDirector = gameObject.GetComponent<PlayableDirector>();

            foreach (var asset in timeAssets)
            {
                Debug.Log(asset.name);
            }
        }

        public void PlayTimeline(string timelineName)
        {
            foreach (TimelineAsset asset in timeAssets)
            {
                Debug.Log(asset.name + " " + timelineName);
                if (asset.name.Equals(timelineName))
                {
                    playableDirector.Play(asset);
                    PlayerWithStateMachine.Instance.PauseAnimator();
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
    }
}
