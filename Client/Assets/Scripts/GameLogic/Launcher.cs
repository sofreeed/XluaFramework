using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class Launcher : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    string scenePath1 = "Assets/Arts/Scene/Scene1";
    string scenePath2 = "Assets/Arts/Scene/Scene2";
    
    string prefabPath1 = "Assets/Arts/Prefab/Cube.prefab";
    string prefabPath2 = "Assets/Arts/Prefab/Sphere.prefab";

    private AssetOperationHandle handle;
    private AssetOperationHandle handle2;
    void OnGUI()
    {
        Rect r = new Rect(100, 100, 100, 100);
        if (GUI.Button(r, ""))
        {
            
            //Global.ResManager.LoadSceneAsync(scenePath1);
            //Global.ResManager.LoadAssetAsync<GameObject>(prefabPath1, OnAssetLoadCallback);
            handle = Global.ResManager.package.LoadAssetAsync<GameObject>(prefabPath1);
            handle.Completed += Handle_Completed;
            
            
        }
        
        Rect r1 = new Rect(200, 100, 100, 100);
        if (GUI.Button(r1, ""))
        {
            
            //Global.ResManager.LoadSceneAsync(scenePath2);
            //Global.ResManager.LoadAssetAsync<GameObject>(prefabPath2, OnAssetLoadCallback);
            handle2 = Global.ResManager.package.LoadAssetAsync<GameObject>(prefabPath1);
            handle2.Completed += Handle_Completed;
        }
        
        Rect r2 = new Rect(300, 100, 100, 100);
        if (GUI.Button(r2, ""))
        {
            handle.Release();
            //handle2.Release();
            
            //Global.ResManager.LoadSceneAsync(scenePath2);
            //Global.ResManager.LoadAssetAsync<GameObject>(prefabPath2, OnAssetLoadCallback);
            Global.ResManager.package.UnloadUnusedAssets();
        }
    }

    void Handle_Completed(AssetOperationHandle handle)
    {
        //GameObject prefab = handle.AssetObject as GameObject;
        GameObject prefab = handle.InstantiateSync();
        //GameObject.Instantiate(prefab);
    }
    
    void OnAssetLoadCallback(GameObject go)
    {
        //string name = go.name;
        GameObject.Instantiate(go);
    }

    void GameStart()
    {
        Logger.Log("开始游戏...");
    }
}
