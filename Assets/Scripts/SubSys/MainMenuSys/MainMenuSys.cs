using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSys : SubSysBase, IMainMenuSysCallBack
{
    private MainMenuSysUI m_MainMenuSysUI;

    public MainMenuSys()
        : base(ESubSys.MAINMENU_SYS)
    {

    }

    public override void EnterSys(ChangeSysPara changeSysPara)
    {
        base.EnterSys(changeSysPara);
        m_MainMenuSysUI = new MainMenuSysUI(this);
    }
    public override void ExitSys()
    {
        base.ExitSys();
    }

    public override void UpdateSys(double time)
    {
        base.UpdateSys(time);
    }

    public void OnPlayBtnlick()
    {
        throw new System.NotImplementedException();
    }

    public void OnQuitBtnClick()
    {
        throw new System.NotImplementedException();
    }

    public void OnSettingBtnClick()
    {
        throw new System.NotImplementedException();
    }

    


}
