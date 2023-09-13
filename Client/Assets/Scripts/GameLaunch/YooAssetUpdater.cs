using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;
using Debug = UnityEngine.Debug;

public class YooAssetUpdater : MonoBehaviour
{
    public static YooAssetUpdater Instance;
    
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public string AssetServerAddress = "http://127.0.0.1/CDN";

    private readonly string defaultPackageName = "DefaultPackage";
    private ResourcePackage defaultPackage;

    private string packageVersion;
    private ResourceDownloaderOperation downloader;

    private Action updateCallback;
    
    void Start()
    {
        Instance = this;
    }

    public IEnumerator Init()
    {
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);
        
        yield return InitPackage();
    }

    public void HotUpdate(Action updateCallback)
    {
        this.updateCallback = updateCallback;
        StartCoroutine(GetStaticVersion());
    }

    private IEnumerator InitPackage()
    {
        var playMode = PlayMode;

        // 创建默认的资源包
        defaultPackage = YooAssets.TryGetPackage(defaultPackageName);
        if (defaultPackage == null)
        {
            defaultPackage = YooAssets.CreatePackage(defaultPackageName);
            YooAssets.SetDefaultPackage(defaultPackage);
        }

        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var createParameters = new EditorSimulateModeParameters();
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(defaultPackageName);
            initializationOperation = defaultPackage.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            var createParameters = new OfflinePlayModeParameters();
            createParameters.DecryptionServices = null;
            initializationOperation = defaultPackage.InitializeAsync(createParameters);
        }

        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode)
        {
            var createParameters = new HostPlayModeParameters();
            createParameters.DecryptionServices = null;
            createParameters.QueryServices = null;
            createParameters.DefaultHostServer = GetAssetServerURL();
            createParameters.FallbackHostServer = GetAssetServerURL();
            initializationOperation = defaultPackage.InitializeAsync(createParameters);
        }

        yield return initializationOperation;
        if (defaultPackage.InitializeStatus == EOperationStatus.Succeed)
            Debug.Log("资源管理...  defaultPackage初始化完毕！");
        else
            Debug.LogError("资源管理...  defaultPackage初始化失败:" + initializationOperation.Error);
    }

    private IEnumerator GetStaticVersion()
    {
        var operation = defaultPackage.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            packageVersion = operation.PackageVersion;
            StartCoroutine(UpdateManifest());
        }
        else
        {
            Debug.LogError("资源管理...  获得版本失败:" + operation.Error);
        }
    }

    private IEnumerator UpdateManifest()
    {
        var operation = defaultPackage.UpdatePackageManifestAsync(packageVersion);
        yield return operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //保存当前清单的版本
            operation.SavePackageVersion();
            StartCoroutine(CreateDownloader());
        }
        else
        {
            Debug.LogError("资源管理...  更新Manifest失败:" + operation.Error);
        }
    }

    IEnumerator CreateDownloader()
    {
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

        yield return null;
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
        Debug.LogError("资源管理...  下载文件失败，文件名:" + fileName + "    失败原因：" + error);
    }

    void ClearUnusedCacheFiles()
    {
        var operation = defaultPackage.ClearUnusedCacheFilesAsync();
        operation.Completed += OnClearUnusedCacheFilesCompleted;
    }

    private void OnClearUnusedCacheFilesCompleted(AsyncOperationBase obj)
    {
        Debug.Log("资源管理... 没用的文件 清理完毕！");
        
        if (updateCallback != null)
            updateCallback();
    }


    #region 功能方法

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
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

    #endregion
}