using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EventMainType : byte
{
    SystemMessageEvent = 0, 
    ChangeSys          = 1,
}

public class GameEvent 
{
    private static int m_currentTokenID = 0;
    private readonly int m_tokenID;
    public GameEvent()
    {
        m_tokenID = m_currentTokenID++;
    }
    public int TokenId { get { return m_tokenID; } }
    public EventMainType EvtType { get; set; }
    public object Para { get; set; }
    

}
public interface IEventHandler
{
    bool HandleMessage(GameEvent evt);
    bool IsHasHandler(GameEvent evt);
}

public class GameEventMgr
{
    private Dictionary<int, List<IEventHandler>> m_handlerMap;

    public GameEventMgr()
    {
        m_handlerMap = new Dictionary<int, List<IEventHandler>>();
    }

    public void RegisterHandler(IEventHandler handler, params EventMainType[] types)
    {
        for(int i = 0; i < types.Length;i++)
        {
            RegisterHandler(types[i], handler);
        }
    }
    private void RegisterHandler(EventMainType type, IEventHandler handler)
    {
        if(handler!=null)
        {
            if(!m_handlerMap.ContainsKey((int)type))
            {
                m_handlerMap.Add((int)type, new List<IEventHandler>());
            }
            if(!m_handlerMap[(int)type].Contains(handler))
            {
                m_handlerMap[(int)type].Add(handler);
            }
        }
    }

    public void UnregisterHandler(IEventHandler handler)
    {
        using(var enumeratorHandler = m_handlerMap.GetEnumerator())
        {
            List<IEventHandler> list;
            while (enumeratorHandler.MoveNext())
            {
                list = enumeratorHandler.Current.Value;
                list.Remove(handler);
            }
        }
    }

    public void UnregisterHandler(IEventHandler handler, params EventMainType[] types)
    {
        EventMainType type;
        for (int i = 0; i < types.Length; i++)
        {
            type = types[i];
            if (m_handlerMap.ContainsKey((int)type) && m_handlerMap[(int)type].Contains(handler))
            {
                m_handlerMap[(int)type].Remove(handler);
            }
        }
    }

    public void SendEvent(GameEvent evt)
    {
        bool bEventHandle = false;
        List<IEventHandler> handlers;

        if(evt !=null && m_handlerMap.TryGetValue((int)evt.EvtType,out handlers))
        {
            for(int index = 0; index < handlers.Count;index ++ )
            {
                bEventHandle = handlers[index].HandleMessage(evt);
            }            
        }
        if(!bEventHandle)
        {
            if(evt!=null)
            {
                switch (evt.EvtType)
                {
                    case EventMainType.ChangeSys:
                        {
                           //??
                        }
                        break;
                    default:
                        {
                            TGameCore.Log("Debug  :Not handle Evt: " + evt.EvtType);
                        }
                        break;
                };

            }
        }
    }

    public bool isHasHandler(GameEvent evt)
    {
        bool bHasHandler = false;
        List<IEventHandler> handlers;
        if (evt != null && m_handlerMap.TryGetValue((int)evt.EvtType, out handlers))
        {
            for (int index = 0; index < handlers.Count; index++)
            {
                bHasHandler = bHasHandler || handlers[index].IsHasHandler(evt);
            }
        }
        return bHasHandler;
    }

    public void ClearAll()
    {
        if(m_handlerMap !=null)
        {
            m_handlerMap.Clear();
        }

    }

      
}
