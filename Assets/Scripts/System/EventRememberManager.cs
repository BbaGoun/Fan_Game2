using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.ComponentModel;
using Newtonsoft.Json;

[Serializable]
public class StringBoolPair
{
    public string key;
    public bool value;

    public StringBoolPair() { }

    public StringBoolPair(string key, bool value)
    {
        this.key = key;
        this.value = value;
    }
}

namespace ActionPart
{
    [RequireComponent(typeof(TextAsset))]
    public class EventRememberManager : MonoBehaviour
    {
        static public EventRememberManager Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        public TextAsset eventRememberJson;

        private Dictionary<string, bool> eventRemember;
        
        [SerializeField, ReadOnly(true)]
        private List<StringBoolPair> eventRememberWrapper;

        [SerializeField]
        private string filePath;


        private void Start()
        {
            Debug.Log("EventRememberManager Awake: 에디터에서 등록한 json 데이터로 초기화합니다.");

            if (eventRememberJson != null)
            {
                try
                {
                    eventRemember = JsonConvert.DeserializeObject<Dictionary<string, bool>>(eventRememberJson.text);
                    Debug.Log("에디터 json 데이터 불러오기 완료");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"json 파싱 실패: {e.Message}");
                    eventRemember = new Dictionary<string, bool>();
                }
            }
            else
            {
                Debug.LogWarning("eventRememberJson이 에디터에 등록되어 있지 않습니다. 빈 데이터로 초기화합니다.");
                eventRemember = new Dictionary<string, bool>();
            }

            // filePath를 생성합니다. Application.persistentDataPath를 활용하여 저장 경로를 지정합니다.
        #if UNITY_EDITOR
            // 에디터에서는 Assets/Resources 경로에 저장 (테스트 및 개발 편의)
            filePath = System.IO.Path.Combine(Application.dataPath, "Resources/EventRemember.json");
        #else
            // 빌드 환경에서는 persistentDataPath를 사용
            filePath = System.IO.Path.Combine(Application.persistentDataPath, "EventRemember.json");

        #endif

            SaveData();
        }

        public bool IsEventTriggerTrue(string eventTrigger)
        {
            if(eventRemember.ContainsKey(eventTrigger))
                return eventRemember[eventTrigger] == true;
            else{
                Debug.LogError($"없는 eventTrigger ㅈ : {eventTrigger}");
                return false;
            }
        }

        public void SetEventTrigger(string eventTrigger, bool isOn)
        {
            if(eventRemember.ContainsKey(eventTrigger)){
                eventRemember[eventTrigger] = isOn;
                SaveData();
            }
            else{
                Debug.LogError($"없는 eventTrigger : {eventTrigger}");
            }
        }

        public void SaveData(){
            // Newtonsoft.Json을 사용하여 Dictionary<string, bool> 타입의 객체를 JSON 문자열로 변환하는 방법은 다음과 같습니다.
            // string json = JsonConvert.SerializeObject(eventRemember, Formatting.Indented);
            // 위 코드는 eventRemember 딕셔너리를 보기 좋게 들여쓰기(Indented)된 JSON 문자열로 변환합니다.
            try{
                string json = JsonConvert.SerializeObject(eventRemember, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Debug.Log("EventRemember 저장 완료: " + filePath);
#if UNITY_EDITOR
                ToList();
#endif
            }catch(Exception e){
                Debug.LogError($"EventRemember 저장 실패 : {e.Message}");
            }
        }

#if UNITY_EDITOR
        private void ToList(){
            eventRememberWrapper = new List<StringBoolPair>();
            foreach(var keyValuePair in eventRemember){
                eventRememberWrapper.Add(new StringBoolPair(keyValuePair.Key, keyValuePair.Value));
            }
        }
#endif
    }
}
