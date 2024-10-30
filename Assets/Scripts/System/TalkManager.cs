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
        public TextAsset csvFile;
        public AudioSource audioSource;
        public AudioClip typingSound;
        public GameObject talkBox;
        public TMP_Text speaker;
        public TMP_Text content;
        public Image image;
        
        Dictionary<string, TalkEvent> talkDictionary = new Dictionary<string, TalkEvent>();
        [SerializeField]
        List<TalkEvent> talkEventList;

        public TalkEvent currentTalkEvent = null;
        int currentTalkDataIndex;
        int currentContextIndex;
        public int letterTypeSpeed = 10;
        public bool istalking { get; private set; }
        bool isTypingStarted;
        bool isTypingDone;
        Coroutine coroutine;
        bool isCoroutine;

        [System.Serializable]
        public class TalkData
        {
            public string name;
            public string[] contexts;
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
            talkBox.SetActive(false);
            audioSource = talkBox.GetComponent<AudioSource>();
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
                speaker.text = name;

                if (isTypingStarted && !isTypingDone)
                {
                    if (Input.GetButtonDown("Attack") || Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump"))
                    {
                        if (isCoroutine)
                        {
                            StopCoroutine(coroutine);
                            content.text = context;
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
                                talkBox.SetActive(false);
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
            // 아래 한 줄 빼기
            string text = csvFile.text.Substring(0, csvFile.text.Length - 1);
            // 줄바꿈(한 줄)을 기준으로 csv 파일을 쪼개서 string배열에 줄 순서대로 담음
            string[] rows = text.Split(new char[] { '\n' });

            // 엑셀 파일 1번째 줄은 편의를 위한 분류이므로 i = 1부터 시작
            for (int i = 1; i < rows.Length; i++)
            {
                // A, B, C, D열을 쪼개서 배열에 담음
                string[] rowValues = rows[i].Split(new char[] { '|' });

                // 유효한 이벤트 이름이 나올때까지 반복
                if (rowValues[0].Trim() == "" || rowValues[0].Trim() == "end") continue;

                string eventName = rowValues[0].Trim();
                List<TalkData> talkDataList = new List<TalkData>();

                while (rowValues[0].Trim() != "end") // talkDataList 하나를 만드는 반복문
                {
                    // 캐릭터가 한번에 치는 대사의 길이를 모르므로 리스트로 선언
                    List<string> contextList = new List<string>();

                    TalkData talkData = new TalkData();
                    talkData.name = rowValues[1].Trim(); // 캐릭터 이름이 있는 B열

                    do // talkData 하나를 만드는 반복문
                    {
                        contextList.Add(rowValues[2].ToString().Trim());
                        if (++i < rows.Length)
                            rowValues = rows[i].Split(new char[] { '|' });
                        else break;
                    } while (rowValues[1] == "" && rowValues[0] != "end");

                    talkData.contexts = contextList.ToArray();
                    talkDataList.Add(talkData);
                }
                string nextEvent = rowValues[3].Trim();

                TalkEvent talkEvent = new TalkEvent(eventName, talkDataList.ToArray(), nextEvent);
                talkDictionary.Add(eventName, talkEvent);
            }
        }

        public void TalkStart(string eventName)
        {
            if (talkDictionary.ContainsKey(eventName))
            {
                talkBox.SetActive(true);
                currentTalkEvent = talkDictionary[eventName];
                currentTalkDataIndex = 0;
                currentContextIndex = 0;
                istalking = true;
                isTypingStarted = false;
                isTypingDone = false;
                //Schedule<TalkStart>();
            }
            else
                Debug.LogWarning("찾을 수 없는 이벤트 이름 : " + eventName);
        }

        IEnumerator TypeDialog(string dialog)
        {
            isCoroutine = true;
            isTypingStarted = true;
            content.text = "";
            foreach (var letter in dialog.ToCharArray())
            {
                content.text += letter;
                //Schedule<TextTyping>();
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
