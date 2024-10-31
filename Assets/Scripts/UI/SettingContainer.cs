using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public static class SettingContainer
{
    static private float _masterVolume;
    static private float _BGMVolume;
    static private float _effectVolume;
    static private int _resolutionIndex;
    static private ScreenMode _screenMode;
    static private int _mainCount;

    static public float masterVolume
    {
        get { return _masterVolume; }
        set { _masterVolume = value; }
    }
    static public float BGMVolume
    {
        get { return _BGMVolume; }
        set { _BGMVolume = value; }
    }
    static public float effectVolume
    {
        get { return _effectVolume; }
        set { _effectVolume = value; }
    }
    static public int resolutionIndex
    {
        get { return _resolutionIndex; }
        set { _resolutionIndex = value; }
    }
    static public ScreenMode screenMode
    {
        get { return _screenMode; }
        set { _screenMode = value; }
    }
    static public int mainCount
    {
        get { return _mainCount; }
        set { _mainCount = value; }
    }
    static public List<Resolution> resolutionList = new List<Resolution>();

    static SettingContainer(){
        masterVolume = 0.75f;
        BGMVolume = 0.75f;
        effectVolume = 0.75f;
        resolutionList.Add(new Resolution(1280, 720));
        resolutionList.Add(new Resolution(1600, 900));
        resolutionList.Add(new Resolution(1920, 1080));
        mainCount = 0;
    }

    public enum ScreenMode
    {
        FullScreen,
        ExclusiveFullScreen,
        Windowed,
    };

    [System.Serializable]
    public class Resolution
    {
        public int width, height;

        public Resolution(int _width, int _height)
        {
            width = _width;
            height = _height;
        }
    }
}
