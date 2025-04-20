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
        private GameObject background;
        [SerializeField]
        private Speaker leftSpeaker;
        [SerializeField]
        private Speaker rightSpeaker;
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
            background.SetActive(false);
            leftSpeaker.gameObject.SetActive(false);
            rightSpeaker.gameObject.SetActive(false);

            talkBox.SetActive(false);
        }

        public void SetTalkBoxOn()
        {
            background.SetActive(true);
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
                leftSpeaker.image.sprite = null;
                leftSpeaker.gameObject.SetActive(false);

                rightSpeaker.image.sprite = null;
                rightSpeaker.gameObject.SetActive(false);
            }
            else if(!face.Equals("/"))
            {
                string[] values = face.Split('/');

                var value = values[0];
                if (!value.Equals(""))
                {
                    if (char.Equals(value[0], '*'))
                    {
                        value = value.Substring(1);
                        if (!value.Equals(""))
                        {
                            leftSpeaker.gameObject.SetActive(true);
                            SetLeftSpeakerSprite(value);
                        }
                        leftSpeaker.CallUnDark();
                    }
                    else if (char.Equals(value[0], 'd'))
                    {
                        leftSpeaker.gameObject.SetActive(false);
                    }
                    else
                    {
                        leftSpeaker.gameObject.SetActive(true);
                        SetLeftSpeakerSprite(value);
                        leftSpeaker.CallDark();
                    }
                }
                else
                {
                    if (leftSpeaker.gameObject.activeSelf)
                        leftSpeaker.CallDark();
                }


                value = values[1];
                if (!value.Equals(""))
                {
                    if (char.Equals(value[0], '*'))
                    {
                        value = value.Substring(1);
                        if (!value.Equals(""))
                        {
                            rightSpeaker.gameObject.SetActive(true);
                            SetRightSpeakerSprite(value);
                        }
                        rightSpeaker.CallUnDark();
                    }
                    else if (char.Equals(value[0], '*'))
                    {
                        rightSpeaker.gameObject.SetActive(false);
                    }
                    else
                    {
                        rightSpeaker.gameObject.SetActive(true);
                        SetRightSpeakerSprite(value);
                        rightSpeaker.CallDark();
                    }
                }
                else
                {
                    if(rightSpeaker.gameObject.activeSelf)
                        rightSpeaker.CallDark();
                }
            }
        }

        private void SetLeftSpeakerSprite(string speaker)
        {
            leftSpeaker.image.sprite = speakerDictionary[speaker];
        }

        private void SetRightSpeakerSprite(string speaker)
        {
            rightSpeaker.image.sprite = speakerDictionary[speaker];
        }
    }
}
