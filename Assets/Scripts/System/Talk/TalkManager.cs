using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.ComponentModel;

namespace ActionPart
{
    public class TalkManager : MonoBehaviour
    {
        public static TalkManager Instance;


        [SerializeField]
        private TextAsset[] csvFiles;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip typingSound;
        [SerializeField]
        private TalkUI talkUI;
        
        Dictionary<string, TalkEvent> talkDictionary = new Dictionary<string, TalkEvent>();
        
        [SerializeField, ReadOnly(false)]
        List<TalkEvent> talkEventList;

        [SerializeField]
        private TalkEvent currentTalkEvent = null;
        int currentTalkDataIndex;
        int currentContextIndex;
        [SerializeField]
        int letterTypeSpeed = 10;

        [SerializeField]
        bool isTalking;
        bool isTypingStarted;
        bool isTypingDone;
        Coroutine typeingCoroutine;
        bool isTalkNext;

        TalkData talkData;

        private ITalkAble npc;

        [System.Serializable]
        public class TalkData
        {
            public string name;
            public string[] contexts;
            public string[] faces;   
        }

        [System.Serializable]
        public class TalkEvent
        {
            public string eventName;
            public TalkData[] talkDatas;
            public string nextEvent;

            public TalkEvent(string eventName, TalkData[] talkDatas, string nextEvent=null)
            {
                this.eventName = eventName;
                this.talkDatas = talkDatas;
                this.nextEvent = nextEvent;
            }
        }

        public void Initialize()
        {
            #region Singleton
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                if(Instance != this)
                {
                    Destroy(this.gameObject);
                }
            }

            #endregion

            isTalking = false;
            talkUI.SetTalkBoxOff();
            SetTalkDictionary();
            SetDebugTalkEvents();
        }
        
        private void Update()
        {
            if (isTalking && Time.timeScale != 0)
            {
                if (!isTypingStarted)
                {
                    talkData = currentTalkEvent.talkDatas[currentTalkDataIndex];
                    var context = talkData.contexts[currentContextIndex];
                    var face = talkData.faces[currentContextIndex];
                    var name = talkData.name;
                    talkUI.SetSpeaker(name);
                    talkUI.SetSpeakerSprite(face);

                    typeingCoroutine = StartCoroutine(TypeDialog(context));
                    // 표정 바꾸기 넣어야 함
                }
                else if (isTypingDone)
                {
                    if(isTalkNext)
                    {
                        isTalkNext = false;
                        isTypingStarted = false;
                        isTypingDone = false;
                        StopCoroutine(typeingCoroutine);
                        if(++currentContextIndex >= talkData.contexts.Length)
                        {
                            currentContextIndex = 0;
                            if(++currentTalkDataIndex >= currentTalkEvent.talkDatas.Length)
                            {
                                isTalking = false;
                                TalkNextEvent(currentTalkEvent.nextEvent);
                                currentTalkEvent = null;
                                currentTalkDataIndex = 0;
                                currentContextIndex = 0;
                                talkUI.SetTalkBoxOff();
                            }
                        }
                    }
                }
            }
        }

        void TalkNextEvent(string nextEvent)
        {
            Debug.Log(nextEvent);
            
            switch(nextEvent)
            {
                case "튜토리얼_전투":
                    BattleManager.Instance.LocalBossBattleStart();
                    break;
                case "튜토리얼_SC1.이후":
                    NPCTalkDataManager.Instance.SetNPCTalkEvent("안휘성_집무실_가주", "튜토리얼_SC1.이후");
                    break;
                default:
                    break;
            }
            
            PlayerWithStateMachine.Instance.isTalking = false;
            PlayerInputPart.Instance.CanInput();
            MetaGameController.Instance.ShowInterface();
            if (npc != null)
            {
                npc.TalkDone();
                npc = null;
            }

            TimelineBars.Instance.BarsOff();
        }

        void SetTalkDictionary()
        {
            foreach (var csvFile in csvFiles)
            {
                // 아래 한 줄 빼기
                //string text = csvFile.text.Substring(0, csvFile.text.Length - 1);

                string text = csvFile.text;
                // 줄바꿈(한 줄)을 기준으로 csv 파일을 쪼개서 string배열에 줄 순서대로 담음
                string[] rows = text.Split(new char[] { '\n' });

                // 엑셀 파일 1번째 줄은 편의를 위한 분류이므로 i = 1부터 시작
                for (int i = 1; i < rows.Length; i++)
                {
                    // A, B, C, D, E열을 쪼개서 배열에 담음
                    string[] rowValues = rows[i].Split(new char[] { '|' });

                    // 유효한 이벤트 이름이 나올때까지 반복
                    if (rowValues[0].Trim() == "" || rowValues[0].Trim() == "end") continue;

                    string eventName = rowValues[0].Trim();
                    List<TalkData> talkDataList = new List<TalkData>();

                    while (rowValues[0].Trim() != "end") // talkDataList 하나를 만드는 반복문
                    {
                        // 캐릭터가 한번에 치는 대사의 길이를 모르므로 리스트로 선언
                        List<string> contextList = new List<string>();
                        List<string> faceList = new List<string>();

                        TalkData talkData = new TalkData();
                        var name = rowValues[1].Trim();
                        talkData.name = name; // 캐릭터 이름이 있는 B열

                        do // talkData 하나를 만드는 반복문
                        {
                            contextList.Add(rowValues[2].ToString().Trim());
                            faceList.Add(rowValues[3].ToString().Trim());
                            if (++i < rows.Length)
                                rowValues = rows[i].Split(new char[] { '|' });
                            else break;
                        } while (name.Equals(rowValues[1]) && rowValues[0] != "end");

                        talkData.contexts = contextList.ToArray();
                        talkData.faces = faceList.ToArray();
                        talkDataList.Add(talkData);
                    }
                    string nextEvent = rowValues[4].Trim();

                    TalkEvent talkEvent = new TalkEvent(eventName, talkDataList.ToArray(), nextEvent);
                    talkDictionary.Add(eventName, talkEvent);
                }
            }
        }

        public void TalkStart(string eventName, ITalkAble _npc)
        {
            if (!isTalking)
            {
                if (eventName.Equals("프롤로그 만화컷"))
                {
                    LoadingManager.Instance.LoadCartoonSceneAsync("프롤로그 만화컷1", LoadingManager.TransitionMode.FadeIn, 0f, 0f);
                    return;
                }
                if (talkDictionary.ContainsKey(eventName))
                {
                    if(_npc != null)
                    {
                        Debug.Log("디버깅");
                        npc = _npc;
                        npc.TalkStart();
                    }
                    PlayerWithStateMachine.Instance.isTalking = true;
                    PlayerInputPart.Instance.CantInput();
                    talkUI.SetTalkBoxOn();
                    MetaGameController.Instance.DisShowInterface();
                    currentTalkEvent = talkDictionary[eventName];
                    currentTalkDataIndex = 0;
                    currentContextIndex = 0;
                    isTalking = true;
                    isTypingStarted = false;
                    isTypingDone = false;

                    TimelineBars.Instance.BarsOn();
                }
                else
                    Debug.LogWarning("찾을 수 없는 이벤트 이름 : " + eventName);
            }
        }

        IEnumerator TypeDialog(string dialog)
        {
            isTypingStarted = true;
            talkUI.SetContext("");
            foreach (var letter in dialog.ToCharArray())
            {
                talkUI.AddContextChar(letter);
                yield return new WaitForSeconds(1f / letterTypeSpeed);
            }
            isTypingDone = true;
            yield return null;
        }

        void SetDebugTalkEvents()
        {
            talkEventList = new List<TalkEvent>(talkDictionary.Values);
        }

        #region Key Event
        public void ActionTalkConfirm(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                if (isTypingDone)
                    isTalkNext = true;
            } 
        }

        #endregion
    }
}
