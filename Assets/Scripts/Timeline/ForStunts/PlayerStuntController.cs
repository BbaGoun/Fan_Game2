using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace ActionPart
{
    public class PlayerStuntController : MonoBehaviour
    {
        [Space(10)]
        [Header("Audio")]
        AudioSource playerAudioSource;
        public AudioClip runAudio;
        public AudioClip jumpAudio;
        public AudioClip landAudio;

        private void Awake()
        {
            playerAudioSource = GetComponent<AudioSource>();
        }

        public void CallPlayer()
        {
            PlayerWithStateMachine.Instance.transform.localPosition = transform.localPosition;
            PlayerWithStateMachine.Instance.transform.localScale = transform.localScale;
            PlayerWithStateMachine.Instance.gameObject.SetActive(true);
        }

        #region Audio Event
        public void RunAudio()
        {
            playerAudioSource.PlayOneShot(runAudio, 1f);
        }
        public void JumpAudio()
        {
            playerAudioSource.PlayOneShot(jumpAudio, 1f);
        }
        public void LandAudio()
        {
            playerAudioSource.PlayOneShot(landAudio, 1f);
        }
        #endregion
    }
}
