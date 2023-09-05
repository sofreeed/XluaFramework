using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;

public class YooAssetUpdater : MonoBehaviour
{
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    public Transform UIRoot;

    private readonly string packageName = "DefaultPackage";
    private ResourcePackage package;

    private string packageVersion;
    private ResourceDownloaderOperation downloader;


    void Start()
    {
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);

        StartCoroutine(HotUpdate());
    }

    void GameStart()
    {
        //StartCoroutine(DoGameStart());
    }

    IEnumerator DoGameStart()
    {
        //AssetOperationHandle spriteAtlasHandle =
        //    package.LoadAssetAsync<SpriteAtlas>("Assets/Arts/UI/LoginTest/Sprites/LoginTestAtlas"); 
        AssetOperationHandle spriteAtlasHandle =
            package.LoadAssetAsync<SpriteAtlas>("Assets/Arts/UI/LoginTest/Atlas/LoginTestAtlas");
        yield return spriteAtlasHandle;
        SpriteAtlas atlas = spriteAtlasHandle.AssetObject as SpriteAtlas;
        Sprite sprite1 = atlas.GetSprite("main_bottom_icon_activity");


        AssetOperationHandle spriteHandle =
            package.LoadAssetAsync<Sprite>("Assets/Arts/UI/LoginTest/Sprites/main_bottom_icon_shop.png");
        yield return spriteHandle;
        Sprite sprite = spriteHandle.AssetObject as Sprite;


        AssetOperationHandle prefabHandle =
            package.LoadAssetAsync<GameObject>("Assets/Arts/UI/LoginTest/Prefabs/LoginTest");
        yield return prefabHandle;
        //GameObject prefab = prefabHandle.AssetObject as GameObject;
        GameObject go = prefabHandle.InstantiateSync(UIRoot);

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
        GameObject go = handle.InstantiateSync(UIRoot);
        Debug.Log($"Prefab name is {go.name}");

        //AssetOperationHandle spriteHandle = package.LoadAssetAsync<Sprite>("Assets/Arts/UI/LoginTest/Sprites/main_bottom_icon_shop.png");
        //spriteHandle.Completed += OnLoadSpriteCompleted;
    }

    IEnumerator HotUpdate()
    {
        yield return null;

        StartCoroutine(InitPackage(false));
    }

    private IEnumerator InitPackage(bool isUpdate)
    {
        yield return new WaitForSeconds(1f);

        var playMode = PlayMode;

        // 创建默认的资源包
        package = YooAssets.TryGetPackage(packageName);
        if (package == null)
        {
            package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);
        }

        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var createParameters = new EditorSimulateModeParameters();
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            var createParameters = new OfflinePlayModeParameters();
            createParameters.DecryptionServices = null;
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode)
        {
            var createParameters = new HostPlayModeParameters();
            createParameters.DecryptionServices = null;
            createParameters.QueryServices = null;
            createParameters.DefaultHostServer = GetHostServerURL();
            createParameters.FallbackHostServer = GetHostServerURL();
            initializationOperation = package.InitializeAsync(createParameters);
        }

        yield return initializationOperation;
        if (package.InitializeStatus == EOperationStatus.Succeed)
        {
            if (isUpdate)
                StartCoroutine(GetStaticVersion());
            else
                Debug.LogError("YooAssetSamples  不进行更新，初始化结束！");
        }
        else
        {
            Debug.LogError("YooAssetSamples  初始化失败:" + initializationOperation.Error);
        }
    }

    private IEnumerator GetStaticVersion()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var operation = package.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            packageVersion = operation.PackageVersion;
            StartCoroutine(UpdateManifest());
        }
        else
        {
            Debug.LogError("YooAssetSamples  获得版本失败:" + operation.Error);
        }
    }

    private IEnumerator UpdateManifest()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var operation = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //保存当前清单的版本
            operation.SavePackageVersion();
            StartCoroutine(CreateDownloader());
        }
        else
        {
            Debug.LogError("YooAssetSamples  更新Manifest失败:" + operation.Error);
        }
    }

    IEnumerator CreateDownloader()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        if (downloader.TotalDownloadCount == 0)
        {
            ClearUnusedCacheFiles();
        }
        else
        {
            Debug.Log($"Found total {downloader.TotalDownloadCount} files that need download ！");

            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;

            //float sizeMB = totalDownloadBytes / 1048576f;
            //sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            //string totalSizeMB = sizeMB.ToString("f1");
            //ShowMessageBox($"Found update patch files, Total count {msg.TotalCount} Total szie {totalSizeMB}MB", callback);
            //Debug.Log("Found update patch files, Total count {totalDownloadCount} Total szie {totalSizeMB}MB");

            StartCoroutine(BeginDownload());
        }
    }

    private IEnumerator BeginDownload()
    {
        // 注册下载回调
        downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
        downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
        downloader.BeginDownload();
        yield return downloader;

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            yield break;

        ClearUnusedCacheFiles();
    }


    public void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount,
        long totalDownloadSizeBytes, long currentDownloadSizeBytes)
    {
        //做进度条
    }

    public static void OnDownloadErrorCallback(string fileName, string error)
    {
        //下载文件失败
        Debug.LogError("YooAssetSamples  下载文件失败，文件名:" + fileName + "    失败原因：" + error);
    }

    void ClearUnusedCacheFiles()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.ClearUnusedCacheFilesAsync();
        operation.Completed += OnClearUnusedCacheFilesCompleted;
    }

    private void OnClearUnusedCacheFilesCompleted(AsyncOperationBase obj)
    {
        Debug.Log("没用的文件 清理完毕！");

        GameStart();
    }


    #region 功能方法

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
        string hostServerIP = "http://127.0.0.1";
        string gameVersion = "v1.0";

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/CDN/Android/{gameVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{gameVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/CDN/Android/{gameVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
		else
			return $"{hostServerIP}/CDN/PC/{gameVersion}";
#endif
    }

    #endregion
}