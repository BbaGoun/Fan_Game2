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
        public SceneSideEnd leftEnd;
        public SceneSideEnd rightEnd;

        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        public void PlayLocalTimeline(string timelineName)
        {
            StartCoroutine(IEPlayLocalTimeline(timelineName));

            IEnumerator IEPlayLocalTimeline(string timelineName)
            {
                switch (timelineName)
                {
                    case "튜토리얼_연무장 이동":
                        rightEnd.ChangeNextScene("안휘성 연무장 가는 길");
                        break;
                }


                /*foreach (TimelineAsset asset in timeAssets)
                {
                    if (asset.name.Equals(timelineName))
                    {
                        playableDirector.Play(asset);
                        //PlayerWithStateMachine.Instance.PauseAnimator();
                        PlayerInputPart.Instance.CantInput();
                        break;
                    }
                }*/
                yield return null;
            }
        }
    }
}
