using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void OnLevelLoaded(int levelIdx);


public enum LevelIdx : int
{
    MainMenu = 0,
    Level1 = 1,
    Level2 = 2,
    Level3 = 3,

    TypeMax = int.MaxValue,
}

public class LevelMgr : MonoBehaviour
{
    private int m_actualLevelIdx;
    public string loadedLevelName;

    void Awake()
    {
        m_actualLevelIdx = 0;
    }

    public void LoadLevel(string LevelName)
    {
        
        if (SceneManager.GetActiveScene().name == LevelName)
        {
            TGameCore.LogWarning("Debug Warning Scene will be reload ，Name" + LevelName);
        }
        TGameCore.Log("Load Scene:" + LevelName);        
        SceneManager.LoadScene(LevelName);
        loadedLevelName = SceneManager.GetActiveScene().name;        
    }

    public void LoadTargetLevelAsync(OnLevelLoaded onlevelLoaded)
    {
        m_actualLevelIdx = (m_actualLevelIdx == 0) ? 1 : 0;
        StartCoroutine(DoLoadTargetLevelAsync(m_actualLevelIdx, onlevelLoaded));
    }

    public void LoadTargetLevelAsync(int levelIdx, OnLevelLoaded onlevelLoaded)
    {
        StartCoroutine(DoLoadTargetLevelAsync(levelIdx, onlevelLoaded));
    }

    private IEnumerator DoLoadTargetLevelAsync(int levelIdx, OnLevelLoaded onlevelLoaded)
    {
        
        if (SceneManager.GetActiveScene().buildIndex == levelIdx)
        {
            TGameCore.LogWarning("Debug Warining:Scene will be reloaded - Index" + levelIdx);        }
        
        AsyncOperation async = SceneManager.LoadSceneAsync(levelIdx);
        while (!async.isDone)
            yield return async;
        loadedLevelName = SceneManager.GetActiveScene().name;        
        if (onlevelLoaded != null)
        {
            onlevelLoaded(levelIdx);
        }
    }

}
