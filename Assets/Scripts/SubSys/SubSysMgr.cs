using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESubSys : byte
{
    COMMONSYS = 0,
    PREPARE_SYS = 1,
    MAINMENU_SYS = 2,
    BATTE_SYS = 3,

    NONESYS = 255,
}

public class ChangeSysPara
{
    public readonly ESubSys m_targetSys;
    public ESubSys m_lastSys;
    public int m_childSysType = 0;
    public bool m_bForward;

    public object m_optionalParam;
    public bool m_forceChange;

    public ChangeSysPara(ESubSys subSys)
    {
        m_bForward = true;
        m_targetSys = subSys;
    }
    public ChangeSysPara(bool bForward, ESubSys subSys)
    {
        m_bForward = bForward;
        m_targetSys = subSys;
    }
    public ChangeSysPara(bool bForward, ESubSys subSys, int childSysType)
    {
        m_bForward = bForward;
        m_targetSys = subSys;
        m_childSysType = childSysType;
    }
}

public abstract class SubSysBase //:IEventHandler 
{
    public readonly ESubSys m_subSysType;
    private Action<bool> m_finishCal;

    public SubSysBase(ESubSys subSysType)
    {
        m_subSysType = subSysType;
    }
    public virtual void PrepareSys(Action<bool> finishCall)
    {
        m_finishCal = finishCall;
        FinishPrepare(true);
    }
    public void FinishPrepare(bool success)
    {
        if (m_finishCal != null)
        {
            m_finishCal(success);
            m_finishCal = null;
        }
    }
    public virtual void EnterSys(ChangeSysPara changeSysPara)
    {

    }
    public virtual void UpdateSys(double time)
    {

    }
    public virtual void FixedUpdateSys(double time)
    {

    }
    public virtual void LateUpdateSys(double time)
    {

    }
    public virtual void ExitSys()
    {

    }
    public virtual void onApplicationPause(bool pauseStatus)
    {

    }
}

public class SubSysMgr : IEventHandler
{
    private Dictionary<int, SubSysBase> m_subSysList;
    private SubSysBase m_preSubSys;
    private SubSysBase m_curSubSys;
    private ChangeSysPara m_sysPara;

    private Stack<ChangeSysPara> m_subSysStack;

    private int mTimerId;

    public SubSysMgr()
    {
        m_subSysList = new Dictionary<int, SubSysBase>();

        //Add SubSystem Here

        SubSysBase subSys = new PrepareSys();
        m_subSysList.Add((int)subSys.m_subSysType, subSys);

        subSys = new MainMenuSys();
        m_subSysList.Add((int)subSys.m_subSysType, subSys);

        m_curSubSys = null;
        m_subSysStack = new Stack<ChangeSysPara>();

        TGameCore.GetInstance().GetEventMgr().RegisterHandler(this, EventMainType.ChangeSys, EventMainType.SystemMessageEvent);


        mTimerId = TimerHelper.SetTimer(AfterDestroyLogo, 0.1f);
    }
    public void AfterDestroyLogo()
    {
        TimerHelper.KillTimer(mTimerId);

        SwitchToSubSys(new ChangeSysPara(ESubSys.PREPARE_SYS));
    }

    public T GetSubSys<T>() where T : SubSysBase
    {
        foreach (var subSys in m_subSysList)
        {
            if (typeof(T) == subSys.Value.GetType())
                return (T)subSys.Value;
        }
        return null;
    }

    public T GetSubSys<T>(ESubSys type) where T : SubSysBase
    {
        return (T)m_subSysList[(int)type];
    }
    public ESubSys GetCurSubSys()
    {
        if (m_curSubSys != null)
        {
            return m_curSubSys.m_subSysType;
        }
        return ESubSys.NONESYS;
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (m_curSubSys != null)
        {
            m_curSubSys.onApplicationPause(pauseStatus);
        }
    }
    private void SwitchToSubSys(ChangeSysPara subsysPara)
    {
        m_preSubSys = m_curSubSys;
        m_curSubSys = m_subSysList[(int)subsysPara.m_targetSys];
        m_sysPara = subsysPara;
        m_curSubSys.PrepareSys(PrepareFinish);
    }

    private void PrepareFinish(bool finish)
    {
        if (finish)
        {
            if (m_preSubSys != null)
            {
                m_sysPara.m_lastSys = m_preSubSys.m_subSysType;
                m_preSubSys.ExitSys();
                m_preSubSys = null;
            }
            m_curSubSys.EnterSys(m_sysPara);
        }
        else
        {
            ChangeSysPara para = m_subSysStack.Peek();
            if (para == m_sysPara)
            {
                m_subSysStack.Pop();
            }
            m_curSubSys = m_preSubSys;
            m_sysPara = null;
            m_preSubSys = null;
        }
    }
    private void ForwardToSubSys(ChangeSysPara subSysPara)
    {
        if (m_subSysList.ContainsKey((int)subSysPara.m_targetSys))
        {
            if (m_curSubSys == null || m_curSubSys.m_subSysType != subSysPara.m_targetSys || subSysPara.m_forceChange)
            {
                if (SysStackExist(subSysPara))
                {
                    Stack<ChangeSysPara> tempStack = new Stack<ChangeSysPara>();
                    ChangeSysPara temp = null;

                    while (m_subSysStack.Count > 0)
                    {
                        temp = m_subSysStack.Pop();
                        if (temp.m_targetSys == subSysPara.m_targetSys)
                        {
                            break;
                        }
                        tempStack.Push(temp);
                    }
                    while (tempStack.Count > 0)
                    {
                        m_subSysStack.Push(tempStack.Pop());
                    }
                }
                m_subSysStack.Push(subSysPara);
                SwitchToSubSys(subSysPara);
            }
        }
    }
    private bool SysStackExist(ChangeSysPara subSysPara)
    {
        bool exist = false;
        var enumerator = m_subSysStack.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current != null && enumerator.Current.m_targetSys == subSysPara.m_targetSys)
            {
                exist = true;
                break;
            }
        }
        return exist;
    }
    private void BackToLastSys(ChangeSysPara subSysPara)
    {
        ChangeSysPara sysPara = null;
        if (m_subSysList.ContainsKey((int)subSysPara.m_targetSys))
        {
            ChangeSysPara backSysPara = null;
            while (m_subSysStack.Count > 0)
            {
                backSysPara = m_subSysStack.Peek();
                if (backSysPara.m_targetSys == subSysPara.m_targetSys)
                {
                    sysPara = subSysPara;
                    break;
                }
            }
            m_subSysStack.Pop();
        }
        else
        {
            if (m_subSysStack.Count > 0)
            {
                m_subSysStack.Pop();
            }

            if (m_subSysStack.Count > 0)
            {
                sysPara = m_subSysStack.Peek();
                sysPara.m_bForward = false;
            }
            else
            {
                sysPara = null;
            }
        }

        if (sysPara != null)
        {
            SwitchToSubSys(sysPara);
        }

    }

    public void UpdateSubSysMgr(double time)
    {
        if (m_curSubSys != null && m_preSubSys == null)
        {
            m_curSubSys.UpdateSys(time);
        }
    }
    public void FixedUpdateSubSysMgr(double time)
    {
        if (m_curSubSys != null && m_preSubSys == null)
        {
            m_curSubSys.FixedUpdateSys(time);
        }
    }
    public void LateUpdateSubSysMgr(double time)
    {
        if (m_curSubSys != null)
        {
            m_curSubSys.LateUpdateSys(time);
        }
    }

    bool IEventHandler.HandleMessage(GameEvent evt)
    {
        bool bHandled = false;
        switch (evt.EvtType)
        {
            case EventMainType.ChangeSys:
                {
                    ChangeSysPara subSysPara = (ChangeSysPara)evt.Para;

                    if (subSysPara.m_bForward)
                    {
                        ForwardToSubSys(subSysPara);
                    }
                    else
                    {
                        BackToLastSys(subSysPara);
                    }
                    bHandled = true;
                }
                break;
            case EventMainType.SystemMessageEvent:
                {
                    TGameCore.Log("Subsystem recv SystemMessageEvent ");
                }
                break;            
        }
        return bHandled;
    }

    bool IEventHandler.IsHasHandler(GameEvent evt)
    {
        bool bHasHandler = false;
        switch (evt.EvtType)
        {
            case EventMainType.ChangeSys:
                {
                    bHasHandler = true;
                }
                break;
            case EventMainType.SystemMessageEvent:
                {
                    bHasHandler = true;
                }
                break;
        }
        return bHasHandler;
    }
}
