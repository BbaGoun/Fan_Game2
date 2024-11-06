using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static SettingContainer;

public class SettingData
{
    public float masterVolume;
    public float BGMVolume;
    public float effectVolume;

    public List<Resolution> resolutionList;
    public int resolutionIndex;
    public FullScreenMode screenMode;

    public int windowResolutionIndex;
}

public class SettingContainer:MonoBehaviour
{
    public static SettingContainer instance;

    public SettingData settingData = new SettingData();

    private string dataPath;

    private void Awake()
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
        if (File.Exists(dataPath))
        {
            string sData = File.ReadAllText(dataPath);
            settingData = JsonUtility.FromJson<SettingData>(sData);
        }
        // 데이터가 없으면 만들고 저장하기
        else
        {
            settingData.masterVolume = 0.75f;
            settingData.BGMVolume = 0.75f;
            settingData.effectVolume = 0.75f;

            Resolution[] reses = Screen.resolutions;
            foreach (Resolution res in reses)
            {
                if (res.width < 1000f)
                    continue;
                if (Mathf.Round(res.width * 0.5625f) == res.height)
                {
                    settingData.resolutionList.Add(res);
                }
            }
            settingData.resolutionIndex = settingData.resolutionList.Count - 1;
            settingData.windowResolutionIndex = settingData.resolutionIndex;
            settingData.screenMode = FullScreenMode.Windowed;

            string sData = JsonUtility.ToJson(settingData);
            File.WriteAllText(dataPath, sData);
        }
    }

    public void SettingChange()
    {
        // 데이터 갱신하기
    }
}
