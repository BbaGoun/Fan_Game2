using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSave : MonoBehaviour
{
    private AudioController audioController;

    public Button closeButton;
    public GameObject Popup;
    public SaveSlot slot;
    int slotNum;
    public AudioClip clickSound;

    private void Awake()
    {
        audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
    }

    private void OnDisable()
    {
        Popup.SetActive(false);
    }

    public void PopUpSave(int index)
    {
        PlayClickSound();

        closeButton.interactable = false;
        Popup.SetActive(true);
        slotNum = index;
    }

    public void YesSave()
    {
        PlayClickSound();
        closeButton.interactable = true;
        Popup.SetActive(false);
        Save(slotNum);
        slot.Refresh();
    }

    public void NoSave()
    {
        PlayClickSound();
        closeButton.interactable = true;
        Popup.SetActive(false);
    }

    public void Save(int Index)
    {
        DataManager.instance.SaveData(Index - 1);
    }

    private void PlayClickSound()
    {
        if (audioController.effectSound != null && clickSound != null)
        {
            audioController.effectSound.PlayOneShot(clickSound, 1f);
        }
    }
}
