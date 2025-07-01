using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace ActionPart
{
    [System.Serializable]
    public class NPCTalkData
    {
        public string npcName;
        public int talkCount;
        public List<string> talkEvents = new List<string>(); // 이 NPC가 가질 수 있는 talkEvent 목록
        public string currentTalkEvent;

        public bool isWaitCutScene;
        /// <summary>
        /// 다음 talkEvent로 인덱스를 증가시킵니다. (마지막이면 더 이상 증가하지 않음)
        /// </summary>
        public void ChangeCurrentTalkEvent(string _talkEvent, int _talkCount = 0)
        {
            currentTalkEvent = talkEvents.Find(talkEvent => talkEvent == _talkEvent);
            talkCount = _talkCount;
        }
    }

    [System.Serializable]
    public class NPCTalkDataList
    {
        [ReadOnly(true)]
        public List<NPCTalkData> npcTalks = new List<NPCTalkData>();
    }

    [RequireComponent(typeof(TextAsset))]
    public class NPCTalkDataManager : MonoBehaviour
    {
        // 싱글톤 패턴 구현
        public static NPCTalkDataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        [SerializeField]
        private NPCTalkDataList dataList = new NPCTalkDataList();
        
        [SerializeField]
        private TextAsset npcTalkJson;

        [SerializeField]
        private string filePath;
        void Start()
        {
            Debug.Log("NPCTalkDataManager Awake: 에디터에서 등록한 json 데이터로 초기화합니다.");

            if (npcTalkJson != null)
            {
                try
                {
                    dataList = JsonUtility.FromJson<NPCTalkDataList>(npcTalkJson.text);
                    Debug.Log("에디터 json 데이터 불러오기 완료");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"json 파싱 실패: {e.Message}");
                    dataList = new NPCTalkDataList();
                }
            }
            else
            {
                Debug.LogWarning("npcTalkJson이 에디터에 등록되어 있지 않습니다. 빈 데이터로 초기화합니다.");
                dataList = new NPCTalkDataList();
            }

            // filePath를 생성합니다. Application.persistentDataPath를 활용하여 저장 경로를 지정합니다.
#if UNITY_EDITOR
            // 에디터에서는 s/Resources 경로에 저장 (테스트 및 개발 편의)
            filePath = System.IO.Path.Combine(Application.dataPath, "Resources/NPC_TalkData.json");
            try
            {
                // 에디터에서 등록한 json 텍스트를 그대로 파일로 저장
                File.WriteAllText(filePath, npcTalkJson.text);
                Debug.Log($"기본 NPC 대화 json 파일이 생성되었습니다: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"기본 json 파일 생성 실패: {e.Message}");
            }
#else
            // 빌드 환경에서는 persistentDataPath를 사용
            filePath = System.IO.Path.Combine(Application.persistentDataPath, "NPC_TalkData.json");

            // 에디터에서 등록한 json을 기준으로 디폴트 json을 생성하여 경로에 저장하는 코드입니다.
            // 만약 파일이 존재하지 않으면, 에디터에서 등록한 json 데이터를 파일로 저장합니다.
            if (!string.IsNullOrEmpty(filePath) && npcTalkJson != null)
            {
                if (!File.Exists(filePath))
                {
                    try
                    {
                        // 에디터에서 등록한 json 텍스트를 그대로 파일로 저장
                        File.WriteAllText(filePath, npcTalkJson.text);
                        Debug.Log($"기본 NPC 대화 json 파일이 생성되었습니다: {filePath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"기본 json 파일 생성 실패: {e.Message}");
                    }
                }
            }
#endif

            Debug.Log($"NPCTalkData filePath: {filePath}");
        }

        /// <summary>
        /// 외부에서 해당 NPC의 NPCTalkData를 조회하는 메소드
        /// </summary>
        /// <param name="npcName">조회할 NPC 이름</param>
        /// <returns>해당 NPC의 NPCTalkData, 없으면 null 반환</returns>
        public NPCTalkData GetNPCTalkData(string npcName)
        {
            return dataList.npcTalks.Find(x => x.npcName == npcName);
        }

        // 저장
        public void SaveData()
        {
            string json = JsonUtility.ToJson(dataList, true); // true: pretty print
            File.WriteAllText(filePath, json);
            Debug.Log("저장 완료: " + filePath);
        }

        // 불러오기
        public void LoadData()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                dataList = JsonUtility.FromJson<NPCTalkDataList>(json);
                Debug.Log("불러오기 완료");
            }
            else
            {
                Debug.Log("파일 없음, 에러");
            }
        }

        public void SetNPCTalkEvent(string npcName, string talkEvent, int talkCount = 0)
        {
            var npc = dataList.npcTalks.Find(x => x.npcName == npcName);
            if (npc != null)
            {
                npc.ChangeCurrentTalkEvent(talkEvent, talkCount);
            }
            else
            {
                Debug.Log($"{npcName} 대상 npc 없음");
                return;
            }
            SaveData();
        }

        /// <summary>
        /// 해당 NPC의 talkCount를 1 증가시킵니다.
        /// </summary>
        /// <param name="npcName">대상 NPC 이름</param>
        public void UpNPCTalkCount(string npcName)
        {
            var npc = dataList.npcTalks.Find(x => x.npcName == npcName);
            if (npc != null)
            {
                npc.talkCount += 1;
            }
            else
            {
                return;
            }
            SaveData();
        }

        public void WaitCutScene(List<string> actors)
        {
            foreach(string actor in actors){
                var npc = dataList.npcTalks.Find(x => x.npcName == actor);
                if (npc != null)
                {
                    Debug.Log($"WaitCutScene {actor}");
                    npc.isWaitCutScene = true;
                }
                else
                {
                    Debug.Log("못찾음 힝");
                    return;
                }
            }
        }

        public void UnWaitCutScene(List<string> actors)
        {
            foreach(string actor in actors){
                var npc = dataList.npcTalks.Find(x => x.npcName == actor);
                if (npc != null)
                {
                    npc.isWaitCutScene = false;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
