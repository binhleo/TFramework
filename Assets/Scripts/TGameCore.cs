using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EGameLogLevel
{
    EGameLogLevel_0,
    EGameLogLevel_1,
    EGameLogLevel_Normal,
    EGameLogLevel_3,
    EGameLogLevel_4,
    Normal,
    Warning,
    Error,
    EGameLogLevel_Max
}


public class TGameCore 
{
    private static TGameCore m_gameCore = null;
    public GameObject m_gameObject;

    private GameEventMgr m_eventMgr;
    private SubSysMgr m_subSysMgr;
    private GameResourceMgr m_resMgr;
    private GameAudioMgr m_audioMgr;
    private LevelMgr m_levelMgr;

    private GameObject m_uiParent;



    
    //private GameObject m_uiCameraParent;
    //private GameObject m_bothCameraParent;
    //private GameObject m_uiCameraGO;
    //private TGDataMgr m_dataMgr;
    //private EffectMgr m_effectMgr;

    private TGameCore()
    {
        m_eventMgr = new GameEventMgr();
#if UNLIMITED_FPS
        Application.targetFrameRate = -1;
#else
        Application.targetFrameRate = 30;
#endif
    }
    
    public static TGameCore GetInstance()
    {
        if(m_gameCore == null)
        {
            m_gameCore = new TGameCore();
        }
        return m_gameCore;
    }

    public void InitGameCore(GameObject gameObject)
    {
        m_gameObject = gameObject;
        m_levelMgr = m_gameObject.AddComponent<LevelMgr>();       
        m_resMgr = new GameResourceMgr();
        m_audioMgr = new GameAudioMgr();

        InitTGameUI();

        SplashScreen splashScreen = new SplashScreen(m_uiParent, ContinueInit);
    }

    public void ContinueInit()
    {
        m_subSysMgr = new SubSysMgr();
        
    }

    public void InitTGameUI()
    {
        m_uiParent = new GameObject("TGameUI");

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystem.transform.parent = m_uiParent.transform;
        GameObject.DontDestroyOnLoad(m_uiParent);
    }

    public void StartLoadConfig()
    {
        TGameCore.Log("Load All Game Config Here");
        //TGLocalDataManager.getInstance().StartLoadConfig(m_resMgr, OnAllConfigLoaded);
    }

    public void UpdateGame()
    {
        float time = Time.time;
        if(m_subSysMgr!=null)
        {
            m_subSysMgr.UpdateSubSysMgr(time);
        }
    }
    public void FixedUpdateGame()
    {
        float time = Time.time;
        if (m_subSysMgr != null)
        {
            m_subSysMgr.FixedUpdateSubSysMgr(time);
        }
    }

    public void LateUpdateGame()
    {
        float time = Time.time;
        if (m_subSysMgr != null)
        {
            m_subSysMgr.LateUpdateSubSysMgr(time);
        }
    }

    public void OnApplicationQuit()
    {
        if (m_eventMgr != null)
        {
            m_eventMgr.ClearAll();
        }
    }
    public void OnApplicationPause(bool pauseStatus)
    {        
        if(m_subSysMgr!=null)
        {
            m_subSysMgr.OnApplicationPause(pauseStatus);
        }

    }

    public ESubSys CurSubSys
    {
        get { return m_subSysMgr != null ? m_subSysMgr.GetCurSubSys() : ESubSys.NONESYS; }
    }

    public T GetSubSys<T>() where T : SubSysBase
    {
        return m_subSysMgr.GetSubSys<T>();
    }

    public T GetSubSys<T>(ESubSys subSysType) where T : SubSysBase
    {
        return m_subSysMgr.GetSubSys<T>(subSysType);
    }
    #region Get Mgr Object
    public GameEventMgr GetEventMgr()
    {
        return m_eventMgr;
    }

    public GameResourceMgr GetResourceMgr()
    {
        return m_resMgr;
    }

    public LevelMgr GetLevelMgr()
    {
        return m_levelMgr;
    }

    public GameObject GetUIParentGO()
    {
        return m_uiParent;
    }

    #endregion

    #region Write/ShowLog
    public void writeLineLog(EGameLogLevel log_level , string fmt, params object[] args)
    {
        switch (log_level)
        {
            case EGameLogLevel.EGameLogLevel_0:
            case EGameLogLevel.EGameLogLevel_1:
            case EGameLogLevel.EGameLogLevel_Normal:
            case EGameLogLevel.Normal:
                {
                    Debug.Log(string.Format(fmt, args));
                }
                break;
            case EGameLogLevel.Warning:
                {
                    Debug.LogWarning(string.Format(fmt, args));
                }
                break;
            case EGameLogLevel.Error:
                {
                    Debug.LogError(string.Format(fmt, args));
                }
                break;
        };
    }

    public static void Log(object message)
    {
        GetInstance().writeLineLog(EGameLogLevel.Normal, "{0}", message);
    }
    public static void LogError(object message)
    {
        GetInstance().writeLineLog(EGameLogLevel.Error, "{0}", message);
    }
    public static void LogWarning(object message)
    {
        GetInstance().writeLineLog(EGameLogLevel.Warning, "{0}", message);
    }
 #endregion




}
