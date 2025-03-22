using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

namespace ActionPart
{
    public class TalkUI : MonoBehaviour
    {
        Dictionary<string, Sprite> speakerDictionary = new Dictionary<string, Sprite>();

        [SerializeField]
        List<LDSprite> LDSpriteList;

        [System.Serializable]
        public class LDSprite
        {
            public string spriteName;
            public Sprite sprite;

            public LDSprite(string spriteName, Sprite sprite)
            {
                this.spriteName = spriteName;
                this.sprite = sprite;
            }
        }
        [SerializeField]
        private Speaker[] leftSpeakers = new Speaker[3];
        [SerializeField]
        private Speaker[] rightSpeakers = new Speaker[3];
        [SerializeField]
        private GameObject talkBox;
        [SerializeField]
        private TMP_Text speaker;
        [SerializeField]
        private TMP_Text context;

        private void Awake()
        {
            speakerDictionary.Clear();
            foreach(var ldSprite in LDSpriteList)
            {
                speakerDictionary.Add(ldSprite.spriteName, ldSprite.sprite);
            }
        }

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

        public void SetSpeakerSprite(string face)
        {
            // 정해진 index 외에는 image의 enabled가 false여야 함.
            // 말하는 중인 image외에는 검은색 처리가 되어야 함.
            if(face.Equals("clear"))
            {
                foreach(var ls in leftSpeakers)
                {
                    ls.image.sprite = null;
                    ls.gameObject.SetActive(false);
                }
                foreach(var rs in rightSpeakers)
                {
                    rs.image.sprite = null;
                    rs.gameObject.SetActive(false);
                }
            }
            else if(!face.Equals("/////"))
            {
                string[] values = face.Split('/');
                for (int i = 0; i < 3; i++)
                {
                    var value = values[i];
                    if (!value.Equals(""))
                    {
                        if (char.Equals(value[0], '*'))
                        {
                            value = value.Substring(1);
                            if (!value.Equals(""))
                            {
                                leftSpeakers[i].gameObject.SetActive(true);
                                SetLeftSpeakerSprite(value, i);
                            }
                            leftSpeakers[i].CallUnDark();
                        }
                        else if (char.Equals(value[0], 'd'))
                        {
                            leftSpeakers[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            leftSpeakers[i].gameObject.SetActive(true);
                            SetLeftSpeakerSprite(value, i);
                            leftSpeakers[i].CallDark();
                        }
                    }
                    else
                    {
                        if (leftSpeakers[i].gameObject.activeSelf)
                            leftSpeakers[i].CallDark();
                    }
                }
                for (int i = 3; i < 6; i++)
                {
                    var right_index = i - 3;
                    var value = values[i];
                    if (!value.Equals(""))
                    {
                        if (char.Equals(value[0], '*'))
                        {
                            value = value.Substring(1);
                            if (!value.Equals(""))
                            {
                                rightSpeakers[right_index].gameObject.SetActive(true);
                                SetRightSpeakerSprite(value, right_index);
                            }
                            rightSpeakers[right_index].CallUnDark();
                        }
                        else if (char.Equals(value[0], '*'))
                        {
                            rightSpeakers[right_index].gameObject.SetActive(false);
                        }
                        else
                        {
                            rightSpeakers[right_index].gameObject.SetActive(true);
                            SetRightSpeakerSprite(value, right_index);
                            rightSpeakers[right_index].CallDark();
                        }
                    }
                    else
                    {
                        if(rightSpeakers[right_index].gameObject.activeSelf)
                            rightSpeakers[right_index].CallDark();
                    }
                }
            }
        }

        private void SetLeftSpeakerSprite(string speaker, int index)
        {
            if (index < 0 || index > 3)
                return;
            leftSpeakers[index].image.sprite = speakerDictionary[speaker];
        }

        private void SetRightSpeakerSprite(string speaker, int index)
        {
            if (index < 0 || index > 3)
                return;
            rightSpeakers[index].image.sprite = speakerDictionary[speaker];
        }
    }
}
