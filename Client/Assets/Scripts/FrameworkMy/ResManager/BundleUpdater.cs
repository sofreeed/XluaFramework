using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;

public class BundleUpdater : MonoBehaviour
{
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public string PackageName = "DefaultPackage";
    public string AssetServerAddress = "http://127.0.0.1/CDN";

    public ResourcePackage ResourcePackage { get; set; }
    private ResourceDownloaderOperation downloader;
    private string packageVersion;
    private Action updatedCallback;
    
    
    void Start()
    {
        //HotUpdate();
    }

    void GameStart()
    {
        Debug.Log("GameStart ...");

        //AssetOperationHandle spriteAtlasHandle = package.LoadAssetAsync<SpriteAtlas>("Assets/Arts/UI/LoginTest/Atlas/LoginTestAtlas");
        //spriteAtlasHandle.Completed += OnLoadAtlasCompleted;

        //AssetOperationHandle spriteHandle = package.LoadAssetAsync<Sprite>("Assets/Arts/UI/LoginTest/Sprites/main_bottom_icon_shop.png");
        //spriteHandle.Completed += OnLoadSpriteCompleted;

        //AssetOperationHandle prefabHandle = package.LoadAssetAsync<GameObject>("Assets/Arts/UI/LoginTest/Prefabs/LoginTest");
        //prefabHandle.Completed += OnLoadCompleted;

        SpriteAtlasManager.atlasRequested += (string name, Action<SpriteAtlas> action) =>
        {
            //if (m_atalsTestAb == null)
            //{
            //    m_atalsTestAb = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "atlastest"));
            //}
//
            //SpriteAtlas spriteAtlas = m_atalsTestAb.LoadAsset<SpriteAtlas>(name);
            //action(spriteAtlas);
            //Debug.LogError(name + "   " + spriteAtlas);
        };

        StartCoroutine(DoGameStart());
    }

    IEnumerator DoGameStart()
    {
        
        //AssetOperationHandle spriteAtlasHandle =
        //    package.LoadAssetAsync<SpriteAtlas>("Assets/Arts/UI/LoginTest/Sprites/LoginTestAtlas"); 
        AssetOperationHandle spriteAtlasHandle =
           ResourcePackage.LoadAssetAsync<SpriteAtlas>("Assets/Arts/UI/LoginTest/Atlas/LoginTestAtlas");
        yield return spriteAtlasHandle;
        SpriteAtlas atlas = spriteAtlasHandle.AssetObject as SpriteAtlas;
        Sprite sprite1 = atlas.GetSprite("main_bottom_icon_activity");                      
        
        
        AssetOperationHandle spriteHandle = ResourcePackage.LoadAssetAsync<Sprite>("Assets/Arts/UI/LoginTest/Sprites/main_bottom_icon_shop.png");
        yield return spriteHandle;
        Sprite sprite = spriteHandle.AssetObject as Sprite;
        
        
        AssetOperationHandle prefabHandle =
            ResourcePackage.LoadAssetAsync<GameObject>("Assets/Arts/UI/LoginTest/Prefabs/LoginTest");
        yield return prefabHandle;
        //GameObject prefab = prefabHandle.AssetObject as GameObject;
        //GameObject go = prefabHandle.InstantiateSync(UIRoot);

        //UIRoot.GetComponentInChildren<Image>().sprite = sprite1;
        GameObject.Find("Image (1)").GetComponent<Image>().sprite = sprite;
        GameObject.Find("Image (2)").GetComponent<Image>().sprite = sprite1;
        
        yield return 1;
    }

    void OnLoadSpriteCompleted(AssetOperationHandle handle)
    {
        Sprite sprite = (Sprite)handle.AssetObject;

        //UIRoot.GetComponentInChildren<Image>().sprite = sprite;
    }

    void OnLoadAtlasCompleted(AssetOperationHandle handle)
    {
        SpriteAtlas atlas = (SpriteAtlas)handle.AssetObject;
        

        //UIRoot.GetComponentInChildren<Image>().sprite = sprite;
    }

    void OnLoadCompleted(AssetOperationHandle handle)
    {
        //GameObject go = handle.InstantiateSync(UIRoot);
        //Debug.Log($"Prefab name is {go.name}");

        //AssetOperationHandle spriteHandle = package.LoadAssetAsync<Sprite>("Assets/Arts/UI/LoginTest/Sprites/main_bottom_icon_shop.png");
        //spriteHandle.Completed += OnLoadSpriteCompleted;
    }

    public void Init()
    {
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);
        
        StartCoroutine(InitPackage());
    }

    public void HotUpdate(Action callback = null)
    {
        updatedCallback = callback;
        
        StartCoroutine(GetStaticVersion());
    }

    private IEnumerator InitPackage()
    {
        var playMode = PlayMode;

        // 创建默认的资源包
        ResourcePackage = YooAssets.TryGetPackage(PackageName);
        if (ResourcePackage == null)
        {
            ResourcePackage = YooAssets.CreatePackage(PackageName);
            YooAssets.SetDefaultPackage(ResourcePackage);
        }

        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var createParameters = new EditorSimulateModeParameters();
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(PackageName);
            initializationOperation = ResourcePackage.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            var createParameters = new OfflinePlayModeParameters();
            //createParameters.DecryptionServices = new BundleDecryptionServices();
            createParameters.DecryptionServices = null;
            initializationOperation = ResourcePackage.InitializeAsync(createParameters);
        }

        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode)
        {
            var createParameters = new HostPlayModeParameters();
            
            //createParameters.DecryptionServices = new BundleDecryptionServices();
            //createParameters.QueryServices = new QueryStreamingAssetsFileServices();
            
            createParameters.DecryptionServices = null;
            createParameters.QueryServices = null;
            createParameters.DefaultHostServer = GetAssetServerURL();
            createParameters.FallbackHostServer = GetAssetServerURL();
            initializationOperation = ResourcePackage.InitializeAsync(createParameters);
        }

        yield return initializationOperation;
        if (ResourcePackage.InitializeStatus == EOperationStatus.Succeed)
        {
            
        }
        else
        {
            Logger.LogError("BundleUpdate  Package初始化失败:" + initializationOperation.Error);
        }
    }

    private IEnumerator GetStaticVersion()
    {
        var operation = ResourcePackage.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            packageVersion = operation.PackageVersion;
            StartCoroutine(UpdateManifest());
        }
        else
        {
            Logger.LogError("BundleUpdate  版本号获取失败:" + operation.Error);
        }
    }

    private IEnumerator UpdateManifest()
    {
        var operation = ResourcePackage.UpdatePackageManifestAsync(packageVersion);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //保存当前清单的版本
            operation.SavePackageVersion();
            StartCoroutine(CreateDownloader());
        }
        else
        {
            Logger.LogError("BundleUpdate  Manifest更新失败:" + operation.Error);
        }
    }

    IEnumerator CreateDownloader()
    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        int timeout = 60;
        downloader = ResourcePackage.CreateResourceDownloader(downloadingMaxNum, failedTryAgain, timeout);

        if (downloader.TotalDownloadCount == 0)
        {
            Logger.LogError("BundleUpdate  需要更新文件个数：0");
            ClearUnusedCacheFiles();
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // TODO：注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;

            Logger.LogError("BundleUpdate  需要更新文件个数:" + totalDownloadCount + "@文件总大小：" + totalDownloadBytes);
            
            //TODO：做提示框，玩家点击后下载
            //float sizeMB = totalDownloadBytes / 1048576f;
            //sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            //string totalSizeMB = sizeMB.ToString("f1");
            //ShowMessageBox($需要更新文件个数:" + totalDownloadCount + "@文件总大小：" + totalDownloadBytes, callback);

            StartCoroutine(BeginDownload());
        }

        yield return null;
    }

    private IEnumerator BeginDownload()
    {
        // 注册下载回调
        downloader.OnDownloadOverCallback = OnDownloadOverCallback;                 //下载完成
        downloader.OnStartDownloadFileCallback = OnStartDownloadFileCallback;       //开始下载文件
        downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;               //下载文件失败
        downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;         //下载进度
        
        downloader.BeginDownload();
        yield return downloader;

        // 检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            ClearUnusedCacheFiles();
        }
        else
        {
            Logger.LogError("BundleUpdate  更新Bundle失败" + downloader.Error);
        }
        
    }

    private void OnDownloadOverCallback(bool isSucceed)
    {
        if (isSucceed)
        {
            Logger.LogError("BundleUpdate Bundle下载完成...");
        }
        else
        {
            Logger.LogError("BundleUpdate Bundle下载失败...");
        }
    }
    
    private void OnStartDownloadFileCallback(string fileName, long sizeBytes)
    {
        //Logger.Error("BundleUpdate 开始下载文件：" + fileName + "@大小：" + sizeBytes);
    }
    
    private void OnDownloadErrorCallback(string fileName, string error)
    {
        Logger.LogError("BundleUpdate 文件下载失败：" + fileName + "@失败原因：" + error);
    }
    
    private void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount,
        long totalDownloadSizeBytes, long currentDownloadSizeBytes)
    {
        //TODO：进度条
    }
    

    void ClearUnusedCacheFiles()
    {
        var operation = ResourcePackage.ClearUnusedCacheFilesAsync();
        operation.Completed += OnClearUnusedCacheFilesCompleted;
    }

    private void OnClearUnusedCacheFilesCompleted(AsyncOperationBase obj)
    {
        Debug.Log("BundleUpdate 清理完毕！");

        if (updatedCallback != null)
            updatedCallback();
    }
    
    private string GetAssetServerURL()
    {
        //TODO:packageVersion 有待验证
        string gameVersion = packageVersion;    
        //string gameVersion = "v1.0";

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{AssetServerAddress}/Android/{gameVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{AssetServerAddress}/IPhone/{gameVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{AssetServerAddress}/WebGL/{gameVersion}";
        else
            return $"{AssetServerAddress}/PC/{gameVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{AssetServerAddress}/Android/{gameVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{AssetServerAddress}/IPhone/{gameVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{AssetServerAddress}/WebGL/{gameVersion}";
		else
			return $"{AssetServerAddress}/PC/{gameVersion}";
#endif
    }
}