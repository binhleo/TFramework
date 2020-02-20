using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnTimerEnd(int timerID);
public delegate void OnRepeatTimerEvent(int timerID);

public class TimerBehavior : MonoBehaviour
{
    private OnTimerEnd m_onTimerEnd;
    private OnRepeatTimerEvent m_onRepeatTimerEvent;

    private int m_timerID;
    private float timer;

    public void BeginTimer(OnTimerEnd timerListener,  int timerID, float fTime, bool ignoreTimeScale)
    {
        if(timerListener!=null && fTime > 0.0f)
        {
            m_onTimerEnd = timerListener;
            m_timerID = timerID;
            Invoke("EndTimer", fTime * GetTimeScale(ignoreTimeScale));
        }
    }

    private void EndTimer()
    {
        if(m_onTimerEnd !=null)
        {
            m_onTimerEnd(m_timerID);
        }
    }

    private float GetTimeScale(bool ignoreTimeScale)
    {
        float timeScale = 1.0f;
        if(ignoreTimeScale && Time.timeScale >0)
        {
            timeScale = Time.timeScale;
        }
        return timeScale;
    }

    public void BeginRepeatTimer(OnRepeatTimerEvent timerListener, int timeID, float fTime, bool ignoreTimeScale)
    {
        if(timerListener !=null && fTime > 0.0f)
        {
            m_onRepeatTimerEvent = timerListener;
            m_timerID = timeID;
            InvokeRepeating("EndRepeatTimer", 0.01f, fTime * GetTimeScale(ignoreTimeScale));
        }
    }

    private void EndRepeatTimer()
    {
        if(m_onRepeatTimerEvent!=null)
        {
            m_onRepeatTimerEvent(m_timerID);
        }
    }

}
