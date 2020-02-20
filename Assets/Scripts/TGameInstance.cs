using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGameInstance : MonoBehaviour
{
    private TGameCore m_gameCore;

    // Start is called before the first frame update
    public void Awake()
    {
        m_gameCore = TGameCore.GetInstance();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }


    void Start()
    {
        StartCoroutine(InitGameCore());
    }

    private IEnumerator InitGameCore()
    {
        yield return 1;
        m_gameCore.InitGameCore(gameObject);
    }

        
    void Update()
    {
        m_gameCore.UpdateGame();    
    }
    private void FixedUpdate()
    {
        m_gameCore.FixedUpdateGame();
    }
    private void LateUpdate()
    {
        m_gameCore.LateUpdateGame();
    }

    public void OnApplicationPause(bool pause)
    {
        m_gameCore.OnApplicationPause(pause);
    }
    public void OnApplicationQuit()
    {
        m_gameCore.OnApplicationQuit();
    }
}
