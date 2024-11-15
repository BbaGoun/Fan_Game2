using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using TMPro;
using UnityEngine.UI;
using Unity.Jobs.LowLevel.Unsafe;

namespace ActionPart
{
    public class TalkManager : MonoBehaviour
    {
        [SerializeField]
        private TextAsset[] csvFiles;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip typingSound;
        [SerializeField]
        private TalkUI talkUI;
        
        Dictionary<string, TalkEvent> talkDictionary = new Dictionary<string, TalkEvent>();
        [SerializeField]
        List<TalkEvent> talkEventList;

        [SerializeField]
        private TalkEvent currentTalkEvent = null;
        int currentTalkDataIndex;
        int currentContextIndex;
        [SerializeField]
        int letterTypeSpeed = 10;

        [SerializeField]
        bool istalking;
        bool isTypingStarted;
        bool isTypingDone;
        Coroutine coroutine;
        bool isCoroutine;

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

        private void Awake()
        {
            istalking = false;
            talkUI.SetTalkBoxOff();
            SetTalkDictionary();
            SetDebugTalkEvents();
        }
        
        private void Update()
        {
            if (istalking && Time.timeScale != 0)
            {
                // for debug
                /*
                if (Input.GetMouseButtonDown(1))
                {
                    TalkNextEvent(currentTalkEvent.nextEvent);
                    istalking = false;
                    currentTalkEvent = null;
                    currentTalkDataIndex = 0;
                    currentContextIndex = 0;
                    talkBox.SetActive(false);
                    return;
                }
                */

                var talkData = currentTalkEvent.talkDatas[currentTalkDataIndex];
                var context = talkData.contexts[currentContextIndex];
                var name = talkData.name;
                talkUI.SetSpeaker(name);

                if (isTypingStarted && !isTypingDone)
                {
                    if (Input.GetButtonDown("Attack") || Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump"))
                    {
                        if (isCoroutine)
                        {
                            StopCoroutine(coroutine);
                            talkUI.SetContext(context);
                            //표정도 바꾸는거 추가하기
                            //Schedule<TextTyping>();
                            isTypingDone = true;
                            return;
                        }
                        else
                            Debug.Log("텍스트 넘기기 뭔가 잘못됨");
                    }
                }

                if (!isTypingStarted)
                    coroutine = StartCoroutine(TypeDialog(context));
                else if (isTypingDone)
                {
                    if(Input.GetButtonDown("Attack") || Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump"))
                    {
                        isTypingStarted = false;
                        isTypingDone = false;
                        coroutine = null;
                        if(++currentContextIndex >= talkData.contexts.Length)
                        {
                            currentContextIndex = 0;
                            if(++currentTalkDataIndex >= currentTalkEvent.talkDatas.Length)
                            {
                                TalkNextEvent(currentTalkEvent.nextEvent);
                                istalking = false;
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
            switch(nextEvent)
            {
                case "전투 시작":
                    //Schedule<BossBattleStart>();
                    //Schedule<PlayerTalkEnd>();
                    break;
                case "승천":
                    //Schedule<BossAscend>();
                    break;
                case "엔딩":
                    //Schedule<GameEnding>();
                    break;
                default:
                    //Schedule<PlayerTalkEnd>();
                    break;
            }
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
                        } while (!name.Equals(rowValues[1]) && rowValues[0] != "end");

                        talkData.contexts = contextList.ToArray();
                        talkData.faces = faceList.ToArray();
                        talkDataList.Add(talkData);
                    }
                    string nextEvent = rowValues[3].Trim();

                    TalkEvent talkEvent = new TalkEvent(eventName, talkDataList.ToArray(), nextEvent);
                    talkDictionary.Add(eventName, talkEvent);
                }
            }
        }

        public void TalkStart(string eventName)
        {
            if (talkDictionary.ContainsKey(eventName))
            {
                talkUI.SetTalkBoxOn();
                currentTalkEvent = talkDictionary[eventName];
                currentTalkDataIndex = 0;
                currentContextIndex = 0;
                istalking = true;
                isTypingStarted = false;
                isTypingDone = false;
            }
            else
                Debug.LogWarning("찾을 수 없는 이벤트 이름 : " + eventName);
        }

        IEnumerator TypeDialog(string dialog)
        {
            isCoroutine = true;
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
    }
}
