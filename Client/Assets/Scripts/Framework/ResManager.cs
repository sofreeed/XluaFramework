using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using YooAsset;

public class ResManager : MonoBehaviour
{
    public ResourcePackage package;

    public void Init(ResourcePackage package)
    {
        this.package = package;
    }

    public void LoadAssetAsync<T>(string assetPath, Action<T> callback = null) where T : UnityEngine.Object
    {
        StartCoroutine(DoLoadAssetASync<T>(assetPath, callback));
    }

    IEnumerator DoLoadAssetASync<T>(string assetPath, Action<T> callback) where T : UnityEngine.Object
    {
        AssetOperationHandle handle = package.LoadAssetAsync<T>(assetPath);
        yield return handle;
        //GameObject go = handle.InstantiateSync();
        if (callback != null)
        {
            callback(handle.AssetObject as T);
        }
        handle.Release();
    }

    public T LoadAssetSync<T>(string assetPath) where T : UnityEngine.Object
    {
        AssetOperationHandle handle =  package.LoadAssetSync<T>(assetPath);
        return handle.AssetObject as T; 
    }

    /// <summary>
    /// TODO：加载完释放自动释放其他资源？
    /// </summary>
    public void LoadSceneAsync(string assetPath)
    {
        StartCoroutine(DoLoadSceneAsync(assetPath));
    }

    IEnumerator DoLoadSceneAsync(string assetPath)
    {
        var sceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single;
        bool activateOnLoad = true;
        SceneOperationHandle handle = package.LoadSceneAsync(assetPath, sceneMode, activateOnLoad);
        yield return handle;
        //切换场景...
    }


    public void UnloadAsset(string assetPath)
    {
        StartCoroutine(DoUnloadAsset(assetPath));
    }

    IEnumerator DoUnloadAsset(string assetPath)
    {
        AssetOperationHandle handle = package.LoadAssetAsync<UnityEngine.Object>(assetPath);
        yield return handle;
        handle.Release();
    }

    public void UnloadUnusedAssets()
    {
        package.UnloadUnusedAssets();
    }
    
    //private float tickTime = 0f;
    //void Update()
    //{
    //    tickTime += Time.deltaTime;
    //    //每3s调用一次
    //    if (tickTime > 3f)
    //    {
    //        if (package != null)
    //        {
    //            //TODO：切换场景的时候也加上调用
    //            
    //            //可以在切换场景之后调用资源释放方法或者写定时器间隔时间去释放。
    //            //注意：只有调用资源释放方法，资源对象才会在内存里被移除。
    //            package.UnloadUnusedAssets();
    //        }
    //        tickTime = 0f;
    //    }
    //}

}
