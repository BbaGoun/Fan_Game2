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

        private PlayableDirector _playableDirector;
        [SerializeField]
        private List<TimelineAsset> _timelineAssets;
        private TimelineAsset _currentTimelineAsset;
        public LocalTimelineController _currentLocalTimelineController;

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

            _playableDirector = GetComponent<PlayableDirector>();
        }

        public void ChangeCurrentLocalTimelineController(LocalTimelineController other)
        {
            _currentLocalTimelineController = other;
        }

        private void ChangeCurrentTimelineAsset(string assetName)
        {
            _currentTimelineAsset = _timelineAssets.Find(timelineAsset => timelineAsset.name == assetName);
            if(_currentTimelineAsset == null)
            {
                Debug.LogError("Change Current TimelineAsset Error : " + assetName);
                return;
            }
            _playableDirector.playableAsset = _currentTimelineAsset;
        }

        public void PlayTimeline(string timelineName)
        {
            ChangeCurrentTimelineAsset(timelineName);

            if(_currentTimelineAsset == null)
            {
                Debug.LogError("TimelineAsset not found: " + timelineName);
                return;
            }

            TimelineBars.Instance.BarsOn();

            _playableDirector.Play();
        }

        public void EndTimeLine(){
            switch(_currentTimelineAsset.name)
            {
                case "안휘성_집무실First":
                    TalkManager.Instance.TalkStart("튜토리얼_SC1.", null);
                    break;
                case "안휘성_연무장First":
                    TalkManager.Instance.TalkStart("튜토리얼_SC1-2.", null);
                    break;
                default:
                    PlayerCanMove();
                    TimelineBars.Instance.BarsOff();
                    break;
            }
            VirtualCameraControl.Instance.OffTimelineCam();
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
