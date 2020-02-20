using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This SubSystem use for loading data before enter the MainMenu, will implement later 

public class PrepareSys:SubSysBase
{

    private bool m_loadingState = false;
    public PrepareSys()
        :base(ESubSys.PREPARE_SYS)
    {

    }

    public override void EnterSys(ChangeSysPara changeSysPara)
    {
        base.EnterSys(changeSysPara);

        TGameCore.GetInstance().StartLoadConfig();
        m_loadingState = true;


    }
    public override void ExitSys()
    {        
        base.ExitSys();        
    }
    public override void UpdateSys(double time)
    {
        base.UpdateSys(time);
        if(m_loadingState)
        {
            PubMethod.ToMainMenu();
            m_loadingState = false;
        }
    }




}

