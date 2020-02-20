using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResourceMgr
{
    private ResourceLoader m_loader;

    public GameResourceMgr()
    {
        GameObject resourceObj = new GameObject();
        resourceObj.name = "ResourceLoader";
        GameObject.DontDestroyOnLoad(resourceObj);
        m_loader = resourceObj.AddComponent<ResourceLoader>();
    }



    public T GetResourceByPath<T>(string resourcePath) where T : UnityEngine.Object
    {
        T resourceGo = null;
        if (string.IsNullOrEmpty(resourcePath))
        {
            TGameCore.LogError("Debug  :string.IsNullOrEmpty(resourceName)");
        }
        else
        {
            resourceGo = m_loader.Load<T>(resourcePath);
        }

        if (resourceGo == null)
        {
            Debug.LogWarning(resourcePath + " not founded");
        }
        return resourceGo;
    }
}
