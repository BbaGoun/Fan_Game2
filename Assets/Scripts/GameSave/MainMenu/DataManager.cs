using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;



public class PlayerData{
    public string m_sStage;
    public string m_sDate;
}


public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public string m_sPath;
    
    
    public PlayerData m_Data = new PlayerData();


    private void Awake(){
        #region Singleton 

        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

        } else if(instance != this)
        {
            Destroy(instance.gameObject);
        }
        
        #endregion // !Singleton 
        m_sPath = Application.persistentDataPath+"/save";
    }


    public void SaveData(int SaveIndex)
    {
        m_Data.m_sDate = DateTime.Now.ToString(("yyyy-MM-dd HH:mm"));
        m_Data.m_sStage = SceneManager.GetActiveScene().name;
        string Data = JsonUtility.ToJson(m_Data);
        File.WriteAllText(m_sPath + SaveIndex.ToString(), Data);
    }



    public void LoadData(int Index)
    {
        string Data = File.ReadAllText(m_sPath + Index.ToString());
        m_Data = JsonUtility.FromJson<PlayerData>(Data);
    }

    public void GameLoad(int index)
    {
        if (File.Exists(instance.m_sPath + $"{index}"))
        {
            string Data = File.ReadAllText(m_sPath + index.ToString());
            m_Data = JsonUtility.FromJson<PlayerData>(Data);
            var stageName = m_Data.m_sStage;

            switch (stageName)
            {
                case "포켓몬 배틀":
                    LoadingManager.Instance.sceneName = "포켓몬 인트로";
                    LoadingManager.Instance.LoadSceneAsync("포켓몬 로딩");
                    break;
                case "스타듀 대화1":
                case "스타듀 대화2":
                    LoadingManager.Instance.sceneName = stageName;
                    LoadingManager.Instance.LoadSceneAsync("스타듀 로딩");
                    break;
                case "스타듀 낚시":
                    LoadingManager.Instance.sceneName = "스타듀 가이드";
                    LoadingManager.Instance.LoadSceneAsync("스타듀 로딩");
                    break;
                case "반반":
                case "반반 엔딩":
                    LoadingManager.Instance.sceneName = stageName;
                    LoadingManager.Instance.LoadSceneAsync("반반 로딩");
                    break;
                case "니디걸 보스전":
                    LoadingManager.Instance.sceneName = "니디걸 가이드";
                    LoadingManager.Instance.LoadSceneAsync("일반 로딩");
                    break;
                default:
                    LoadingManager.Instance.sceneName = stageName;
                    LoadingManager.Instance.LoadSceneAsync("일반 로딩");
                    break;
            }
        }
    }

    public void ClearData()
    {
        m_Data = new PlayerData();
    }
    
}
