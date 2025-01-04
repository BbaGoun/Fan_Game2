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

        public TimelineBars timelineBars;
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
        }

        public void ChangeCurrentLocalTimelineController(LocalTimelineController other)
        {
            currentLocalTimelineController = other;
        }

        public void PlayTimeline(string timelineName)
        {
            StartCoroutine(IEPlayTimeline(timelineName));

            IEnumerator IEPlayTimeline(string timelineName)
            {
                timelineBars.BarsOn();

                switch (timelineName)
                {
                    default:
                        break;
                }

                timelineBars.BarsOff();
                yield return null;
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
