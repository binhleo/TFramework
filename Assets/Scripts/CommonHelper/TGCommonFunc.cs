using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TGCommonFunc 
{
    public static T GetComponentByName<T>(GameObject go, string name)
        where T : Component
    {
        T[] buffer = go.GetComponentsInChildren<T>(true);
        if (buffer != null)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != null && buffer[i].name == name)
                {
                    return buffer[i];
                }
            }
        }
        return null;
    }

    public static T[] GetComponentsByName<T>(GameObject go)
        where T : Component
    {
        T[] buffer = go.GetComponentsInChildren<T>(true);
        return buffer;
    }
    public static GameObject GetGameObjectByName(GameObject objInput, string strFindName)
    {
        GameObject ret = null;
        if (objInput != null)
        {
            Transform[] objChildren = objInput.GetComponentsInChildren<Transform>(true);
            if (objChildren != null)
            {
                for (int i = 0; i < objChildren.Length; ++i)
                {
                    if ((objChildren[i].name == strFindName))
                    {
                        ret = objChildren[i].gameObject;
                        break;
                    }
                }
            }
        }
        return ret;
    }
    public static List<GameObject> GetGameObjectsByName(GameObject objInput, string strFindName)
    {
        List<GameObject> list = new List<GameObject>();
        Transform[] objChildren = objInput.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < objChildren.Length; ++i)
        {
            if ((objChildren[i].name.Contains(strFindName)))
            {
                list.Add(objChildren[i].gameObject);
            }
        }
        return list;
    }

    public static T AddSingleComponent<T>(GameObject go)
       where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    public static float FloatLerp(float from, float to, float t)
    {
        t = Mathf.Clamp(t, 0.0f, 1.0f);
        float ret = from + (to - from) * t;
        return ret;
    }
    public static GameObject InstantiatePrefab(GameObject prefab, GameObject parent)
    {
        GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        obj.name = prefab.name;
        if (parent != null)
        {            
            obj.transform.SetParent(parent.transform);
        }
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        return obj;
    }
    public static string RemoveBlankChar(string oldString)
    {
        string newString = oldString.Replace("\r", string.Empty);
        newString = newString.Replace("\n", string.Empty);
        newString = newString.Replace("\t", string.Empty);
        return newString;
    }

}
