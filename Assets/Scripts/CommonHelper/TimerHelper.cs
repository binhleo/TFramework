using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TimerFunctionDelegate();
public class TimerHelper 
{
    static private int m_timerID;
    static private Dictionary<int, TimerFunctionDelegate> m_timerFuncDic;
    static private Dictionary<int, GameObject> m_timerObj;
    static TimerHelper()
    {
        m_timerID = 1;
        m_timerFuncDic = new Dictionary<int, TimerFunctionDelegate>();
        m_timerObj = new Dictionary<int, GameObject>();
    }


    static public int SetTimer(TimerFunctionDelegate func, float fTime, bool ignoreTimeScale)
    {   
        int timerID = int.MaxValue;

        if(func !=null && fTime > 0.0f)
        {
            timerID = GetTimerID();
            GameObject timerTempObj = new GameObject();
            GameObject.DontDestroyOnLoad(timerTempObj);
            timerTempObj.name = timerID.ToString();
            TimerBehavior timerBehavior = timerTempObj.AddComponent<TimerBehavior>();
            m_timerFuncDic.Add(timerID, func);
            m_timerObj.Add(timerID, timerTempObj);
            timerBehavior.BeginTimer(OnTimerEnd, timerID, fTime, ignoreTimeScale);
        }
        return timerID;
    }

    static public int SetTimer(TimerFunctionDelegate func, float fTime)
    {
        return SetTimer(func, fTime, false);
    }

    static public int SetRepeatTimer(TimerFunctionDelegate func, float fTime, bool ignoreTimeScale)
    {        
        int timerID = -1;
        if (func != null && fTime > 0.0f)
        {
            timerID = GetTimerID();
            GameObject timerTempObj = new GameObject();
            GameObject.DontDestroyOnLoad(timerTempObj);
            timerTempObj.name = timerID.ToString();
            TimerBehavior timerBehavior = timerTempObj.AddComponent<TimerBehavior>();
            m_timerFuncDic.Add(timerID, func);
            m_timerObj.Add(timerID, timerTempObj);
            timerBehavior.BeginRepeatTimer(OnRepeatTimerEvent, timerID, fTime, ignoreTimeScale);
        }
        return timerID;
    }
    public static int SetRepeatTimer(TimerFunctionDelegate func, float fTime)
    {
        return SetRepeatTimer(func, fTime, false);
    }

    public static void KillTimer(int timerID)
    {
        //Todo
    }
    public static int GetTimerID()
    {
        return m_timerID++;
    }
    static void OnRepeatTimerEvent(int timerID)
    {
        if(m_timerFuncDic.ContainsKey(timerID))
        {
            m_timerFuncDic[timerID]();
        }
    }
    static void OnTimerEnd(int timerID)
    {
        if(m_timerFuncDic.ContainsKey(timerID))
        {
            m_timerFuncDic[timerID]();
            m_timerFuncDic.Remove(timerID);
        }
        if(m_timerObj.ContainsKey(timerID))
        {
            GameObject timerTempObj = m_timerObj[timerID];
            m_timerObj.Remove(timerID);
            GameObject.Destroy(timerTempObj);
            timerTempObj = null;
        }
    }
}
