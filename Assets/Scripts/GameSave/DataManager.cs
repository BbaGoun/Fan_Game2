using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;


namespace ActionPart
{
    public class PlayerData
    {
        public string m_sStage;
        public string m_sDate;
    }


    public class DataManager : MonoBehaviour
    {
        public static DataManager instance;
        public string m_sPath;

        public PlayerData m_Data = new PlayerData();


        private void Awake()
        {
            #region Singleton 

            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(instance.gameObject);
            }

            #endregion // !Singleton 
            m_sPath = Application.persistentDataPath + "/save";
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
                    default:
                        break;
                }
            }
        }

        public void ClearData()
        {
            m_Data = new PlayerData();
        }

    }
}