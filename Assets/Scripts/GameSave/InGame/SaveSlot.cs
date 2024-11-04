using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public GameObject[] m_gSlotImageGroup_HasDataContents;
    public GameObject[] m_gSlotImageGroup_NoDataContents;


    public TMP_Text[] m_tPlayerNameText;
    public TMP_Text[] m_tDateText;
    public TMP_Text[] m_tStageText;

    bool[] m_bSaveExistence = new bool[3];

    // Start is called before the first frame update

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(DataManager.instance.m_sPath + $"{i}"))
            {
                m_bSaveExistence[i] = true;

                DataManager.instance.LoadData(i);
                m_tPlayerNameText[i].text = "남궁혁";
                m_tDateText[i].text = DataManager.instance.m_Data.m_sDate;
                // 씬 데이터 정보를 여기서 보여줌 (SaveSlot / Slot씬에 있음 같이 수정)
                m_tStageText[i].text = DataManager.instance.m_Data.m_sStage;

                m_gSlotImageGroup_HasDataContents[i].SetActive(true);
                m_gSlotImageGroup_NoDataContents[i].SetActive(false);
            }
            else
            {
                m_tDateText[i].text = "---- : ---- : ---- : ----";
                m_tStageText[i].text = "----";
                m_gSlotImageGroup_HasDataContents[i].SetActive(false);
                m_gSlotImageGroup_NoDataContents[i].SetActive(true);
            }
        }
        DataManager.instance.ClearData();
    }



}
