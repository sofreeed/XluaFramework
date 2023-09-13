using UnityEngine;
using System.Collections;
using XLua;
using YooAsset;

[Hotfix]
[LuaCallCSharp]
public class GameLaunch : MonoBehaviour
{
    public static TypeEventSystem EventSystem;
    
    const string launchPrefabPath = "Assets/AssetsPackage/UI/Prefabs/UILoading/UILoading.prefab";
    const string noticeTipPrefabPath = "Assets/AssetsPackage/UI/Prefabs/Common/UINoticeTip.prefab";
    const string LaunchLayerPath = "UIRoot/TopLayer";
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
        
        EventSystem = TypeEventSystem.Global;
    }
    
    IEnumerator Start ()
    {
        // 启动资源管理模块
        yield return YooAssetUpdater.Instance.Init();
        yield return InitLaunchPrefab();
        yield return InitNoticeTipPrefab();

        // 开始更新
        YooAssetUpdater.Instance.HotUpdate(OnUpdateComplete);
	}

    private void OnUpdateComplete()
    {
        XLuaManager.Instance.Startup();
        XLuaManager.Instance.OnInit();
        //XLuaManager.Instance.StartHotfix();
        XLuaManager.Instance.StartGame();
        CustomDataStruct.Helper.Startup();
    }
    
    IEnumerator InitNoticeTipPrefab()
    {
        AssetOperationHandle handle = YooAssets.LoadAssetAsync<GameObject>(noticeTipPrefabPath);
        yield return handle;
        UINoticeTip.Instance.UIGameObject = InstantiateGameObject(handle.AssetObject as GameObject);
        handle.Release();
    }

    IEnumerator InitLaunchPrefab()
    {
        AssetOperationHandle handle = YooAssets.LoadAssetAsync<GameObject>(launchPrefabPath);
        yield return handle;
        UILauncher.Instance.UIGameObject = InstantiateGameObject(handle.AssetObject as GameObject);
        handle.Release();
    }
    
    GameObject InstantiateGameObject(GameObject prefab)
    {
        Transform luanchLayer = transform.Find(LaunchLayerPath);
        GameObject go = Instantiate(prefab, luanchLayer);
        go.name = prefab.name;
        return go;
    }
    
}
