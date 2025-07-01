using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace ActionPart
{
    [Serializable]
    class CutScene{
        public TimelineAsset timelineAsset;
        public List<String> actors;
    }

    public class GlobalTimelineController : MonoBehaviour
    {
        public static GlobalTimelineController instance;

        private PlayableDirector _playableDirector;
        [SerializeField]
        private List<CutScene> _cutScenes;

        [SerializeField, ReadOnly(true)]
        private CutScene _currentCutScene;

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

        private void ChangeCurrentTimelineAsset(string assetName)
        {
            _currentCutScene = _cutScenes.Find(cutScene => cutScene.timelineAsset.name == assetName);
            if(_currentCutScene == null)
            {
                Debug.LogError("Change Current CutScene Error : " + assetName);
                return;
            }
            _playableDirector.playableAsset = _currentCutScene.timelineAsset;
        }

        public void PlayTimeline(string timelineName)
        {
            ChangeCurrentTimelineAsset(timelineName);

            if(_currentCutScene == null)
            {
                Debug.LogError("CutScene not found: " + timelineName);
                return;
            }

            NPCTalkDataManager.Instance.WaitCutScene(_currentCutScene.actors);

            TimelineBars.Instance.BarsOn();

            PlayerWithStateMachine.Instance.ResetAnimator();
            PlayerInputPart.Instance.CantInput();
            _playableDirector.Play();
        }

        public void EndTimeLine(){
            switch(_currentCutScene.timelineAsset.name)
            {
                case "안휘성_집무실First":
                    TalkManager.Instance.TalkStart("튜토리얼_SC1.", null);
                    break;
                case "안휘성_연무장First":
                    TalkManager.Instance.TalkStart("튜토리얼_SC1-2.", null);
                    break;
                default:
                    TimelineBars.Instance.BarsOff();
                    break;
            }
            NPCTalkDataManager.Instance.UnWaitCutScene(_currentCutScene.actors);
            PlayerCanMove();
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
