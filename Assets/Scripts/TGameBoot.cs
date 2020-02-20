using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGameBoot : MonoBehaviour
{
    public void Awake()
    {
        GameObject gameCoreObj = GameObject.Find("TGameCore");
        if(gameCoreObj == null)
        {
            gameCoreObj = new GameObject();
            DontDestroyOnLoad(gameCoreObj);
            gameCoreObj.name = "TGameCore";
            gameCoreObj.AddComponent<TGameInstance>();
#if SHOW_FPS
            gameCoreObj.AddComponent<ShowFPS>();
#endif
        }
        GameObject.Destroy(gameObject);
    }
   
}
