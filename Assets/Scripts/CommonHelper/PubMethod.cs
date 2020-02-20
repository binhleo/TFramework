using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PubMethod 
{
    public static GameObject GetInstanceGObyPath(string prefabPath, GameObject parent)
    {
        GameResourceMgr resMgr = TGameCore.GetInstance().GetResourceMgr();
        GameObject prefab = null;
        prefab = resMgr.GetResourceByPath<GameObject>(prefabPath);

        if(prefab == null)
        {
            TGameCore.LogError(prefabPath +  "is not found");
            return null;
        }
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        if(go == null) { return null; }
        go.name = prefab.name;
        if(parent!=null)
        {
            go.transform.SetParent(parent.transform);            
        }
        go.transform.localScale = prefab.transform.localScale;
        go.transform.localPosition = prefab.transform.localPosition;
        go.transform.localRotation = prefab.transform.localRotation;
        return go;        
    }

    public static void ToMainMenu()
    {
        TGameCore.GetInstance().GetLevelMgr().LoadLevel("MainMenu");
        TGameCore.GetInstance().GetEventMgr().SendEvent(new GameEvent { EvtType = EventMainType.ChangeSys, Para = new ChangeSysPara(true, ESubSys.MAINMENU_SYS)});
    }
}
