using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class SplashScreen 
{
    private GameObject m_gameObject;
    private Action m_finishCall;

    private float delayTime = 2.0f;
    
    public SplashScreen(GameObject parent , Action finishCall)
    {
        m_finishCall = finishCall;
        m_gameObject = PubMethod.GetInstanceGObyPath("Prefabs/UI/SplashScreen", parent);
        TimerHelper.SetTimer(() =>{
            m_finishCall();
            GameObject.Destroy(m_gameObject);
            m_gameObject = null;
        },delayTime); 
    }
    



}
