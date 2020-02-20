using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IMainMenuSysCallBack
{
    void OnPlayBtnlick();
    void OnSettingBtnClick();
    void OnQuitBtnClick();
}
public class MainMenuSysUI
{
    private static string m_mainMenuPrefabName = "Prefabs/UI/MainMenu";

    private GameObject m_mainMenuGO;

    public MainMenuSysUI(IMainMenuSysCallBack callBack)
    {
        GameResourceMgr resMgr = TGameCore.GetInstance().GetResourceMgr();
        GameObject UIParent = TGameCore.GetInstance().GetUIParentGO();
        GameObject prefab = resMgr.GetResourceByPath<GameObject>(m_mainMenuPrefabName);
        m_mainMenuGO = TGCommonFunc.InstantiatePrefab(prefab, UIParent);






    }
}
