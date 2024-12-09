using ActionPart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static SettingContainer;

[Serializable]
public class SettingData
{
    public float masterVolume;
    public float BGMVolume;
    public float effectVolume;

    [SerializeField]
    public List<Resolution> resolutionList;
    public int resolutionIndex;
    public FullScreenMode screenMode;

    public int windowResolutionIndex;

    public SettingData() 
    {
        this.resolutionList = new List<Resolution>();
    }

    public SettingData(SettingData other) 
    {   
        this.masterVolume = other.masterVolume;
        this.BGMVolume = other.BGMVolume;
        this.effectVolume = other.effectVolume;

        this.resolutionList = new List<Resolution>();
        this.resolutionList.AddRange(other.resolutionList);
        this.resolutionIndex = other.resolutionIndex;
        this.screenMode = other.screenMode;

        this.windowResolutionIndex = other.windowResolutionIndex;
    }
}

public class SettingContainer:MonoBehaviour
{
    public static SettingContainer instance;

    [SerializeField]
    public SettingData m_SettingData;
    public bool isAwakeDone;

    private string dataPath;

    public void Initialize()
    {
        #region Singleton without DontDestroy
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        #endregion

        dataPath = Application.persistentDataPath + "/setting";

        // 데이터가 있으면 불러오기
        if (File.Exists(dataPath) && false) // 나중에 빌드하면서 확인
        {
            string sData = File.ReadAllText(dataPath);
            m_SettingData = JsonUtility.FromJson<SettingData>(sData);
        }
        // 데이터가 없으면 만들고 저장하기
        else
        {
            m_SettingData = new SettingData();
            m_SettingData.resolutionList = new List<Resolution>();

            m_SettingData.masterVolume = 0.75f;
            m_SettingData.BGMVolume = 0.75f;
            m_SettingData.effectVolume = 0.75f;

            Resolution[] reses = Screen.resolutions;
            var resList = m_SettingData.resolutionList;
            foreach (Resolution res in reses)
            {
                if (res.width < 1000f)
                    continue;
                if (Mathf.Round(res.width * 0.5625f) == res.height)
                {
                    if (resList.Count > 0)
                    {
                        var lastRes = resList[resList.Count - 1];
                        if (lastRes.height == res.height && lastRes.refreshRateRatio.value < res.refreshRateRatio.value)
                            resList[resList.Count - 1] = res;
                        else
                            resList.Add(res);
                    }
                    else
                    {
                        resList.Add(res);
                    }
                }
            }

            foreach(Resolution res in resList)
            {
                Debug.Log(res.ToString());
            }

            m_SettingData.resolutionIndex = m_SettingData.resolutionList.Count - 1;
            m_SettingData.windowResolutionIndex = m_SettingData.resolutionIndex;
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) // window
            {
                m_SettingData.screenMode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) // macOS
            {
                m_SettingData.screenMode = FullScreenMode.MaximizedWindow;
            }
            //m_SettingData.screenMode = FullScreenMode.Windowed;

            string sData = JsonUtility.ToJson(m_SettingData);
            File.WriteAllText(dataPath, sData);
        }

        isAwakeDone = true;
    }
}
