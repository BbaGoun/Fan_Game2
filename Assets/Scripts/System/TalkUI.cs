using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ActionPart
{
    public class TalkUI : MonoBehaviour
    {
        [SerializeField]
        private Image[] leftSpeakers;
        [SerializeField]
        private Image[] rightSpeakers;
        [SerializeField]
        private GameObject talkBox;
        [SerializeField]
        private TMP_Text speaker;
        [SerializeField]
        private TMP_Text context;

        public void SetTalkBoxOff()
        {
            foreach (var Lspeaker in leftSpeakers)
                Lspeaker.gameObject.SetActive(false);
            foreach (var Rspeaker in rightSpeakers)
                Rspeaker.gameObject.SetActive(false);

            talkBox.SetActive(false);
        }

        public void SetTalkBoxOn()
        {
            talkBox.SetActive(true);
        }

        public void SetSpeaker(string _speaker)
        {
            speaker.text = _speaker;
        }

        public void SetContext(string _context)
        {
            context.text = _context;
        }

        public void AddContextChar(char add)
        {
            context.text += add;
        }

        public void SetLeftSpeaker(string speaker, string face, int index)
        {
            //이름과 표정에 맞게 바꾸기
        }

        public void SetRightSpeaker(string speaker, string face, int index)
        {
            //이름과 표정에 맞게 바꾸기
        }
    }
}
