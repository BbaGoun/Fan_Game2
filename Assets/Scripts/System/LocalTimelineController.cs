using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ActionPart
{
    public class LocalTimelineController : MonoBehaviour
    {
        PlayableDirector playableDirector;
        public List<TimelineAsset> timeAssets;

        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
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
    }
}
