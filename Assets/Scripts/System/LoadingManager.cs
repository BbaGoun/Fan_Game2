using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager instance;
    private bool isLoading = false;

    // Private constructor to prevent creating instances via the 'new' keyword
    public static LoadingManager Instance
    {
        get
        {
            return instance;
        }
    }

    public string sceneName;

    private void Awake()
    {
        #region Singleton

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else if(instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion // Singleton
    }

    public void LoadSceneAsync(string sceneName, float delay = 0f)
    {
        if(!isLoading)
          StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    IEnumerator LoadSceneCoroutine(string sceneName, float delay=0f)
    {
        yield return new WaitForSeconds(delay);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Normalize progress between 0 and 1
            //Debug.Log("Loading progress: " + (progress * 100f).ToString("F0") + "%");

            yield return null; // Wait for the next frame
        }

        isLoading = false;
        // Scene has finished loading, perform any necessary actions
        Debug.Log($"{sceneName} Scene loaded!");
    }
}
