using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource BGM;
    public AudioSource effectSound;

    private void Awake()
    {
        BGM = transform.GetChild(0).GetComponent<AudioSource>();
        effectSound = transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", volume);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVol", volume);
    }

    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("SEVol", volume);
    }

    public void ChangeBGM(AudioClip audioClip)
    {
        BGM.clip = audioClip;
        BGM.Play();
    }

    public void PauseBGM()
    {
        BGM.Pause();
    }
}
