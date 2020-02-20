using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
public class ResourceLoader : MonoBehaviour {
    public T Load<T>(string path) where T : Object {
        return Resources.Load<T>(path);
    }
    public Object Load(string path, Type t) {
        return Resources.Load(path, t);
    }
    public Object LoadAssetAtPath(string path, Type t) {
        //??
        //return  UnityEditor.AssetDatabase.LoadAssetAtPath(path, t);
        return Resources.Load(path, t);
    }
    public void LoadWaitOneFrame(string path, Action<Object, string> callBack) {
        StartCoroutine(WaitOneFrameCall(path, callBack));
    }
    private IEnumerator WaitOneFrameCall(string path, Action<Object, string> callBack) {
        yield return 1;
        Object result = Resources.Load<Object>(path);
        callBack(result, path);
    }
}
