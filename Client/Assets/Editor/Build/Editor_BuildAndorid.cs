using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

//代码打包，参考链接：
//https://blog.csdn.net/lixuanderrick/article/details/131442522

public class Editor_BuildAndorid
{
    private static string BulidTime { get => DateTime.Now.ToString("yyyyMMdd"); }
 
    private static Dictionary<string, string> andoridFiles;
    private static List<string> androidNeedUpdateFile = new List<string>() { "build.gradle", "AndroidManifest.xml" };
    private static string path_Output = "/Output/" + BulidTime;
 
    [MenuItem("工具箱/打包/一键生成AAR")]
    public static void OneKeyExportAAR()
    {
        if (BuildAndroidStudioProject())
            BuildAAR();
    }
 
    [MenuItem("工具箱/打包/1. 导出Android Studio工程")]
    public static void ExportAndroidStudioProject()
    {
        BuildAndroidStudioProject();
    }
 
    [MenuItem("工具箱/打包/2. 将Android Studio工程生成AAR")]
    public static void ExportAAR()
    {
        BuildAAR();
    }
 
    /// <summary>
    /// 导出Android Studio工程
    /// </summary>
    /// <param name="autoOpenBuildFile">是否自动打开文件夹</param>
    /// <returns></returns>
    private static bool BuildAndroidStudioProject(bool autoOpenBuildFile = true)
    {
        bool isBuildSucceed = false;
 
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUtility.DisplayDialog("打AAR包失败", "请切换到Android平台", "确认");
            return false;
        }
 
        // 设置成Gradle打包
        EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
        EditorUserBuildSettings.buildScriptsOnly = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
 
        //AutoBuilder.GetScenePaths();
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e != null && e.enabled)
                scenes.Add(e.path);
        }
        var publisPath = GetPublishPath(true);
 
        // BuildOptions必须设置为AcceptExternalModificationsToPlayer，不然的话会打出不带后缀的文件，不是导出AndroidStudio工程
        BuildReport message = BuildPipeline.BuildPlayer(scenes.ToArray(), publisPath, BuildTarget.Android, BuildOptions.None | BuildOptions.AcceptExternalModificationsToPlayer);
        BuildSummary summary = message.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build succeeded: " + summary.totalSize + " bytes");// todoLX 之后把字节转化为mb，gb等其他单位，便于观看
            isBuildSucceed = true;
            if (autoOpenBuildFile)
                OpenDirectory(publisPath);
        }
        else if (summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log("Build failed");
            isBuildSucceed = false;
        }
        return isBuildSucceed;
    }
 
    /// <summary>
    /// 生成AAR
    /// </summary>
    private static void BuildAAR()
    {
        string projectPath = GetPublishPath(false) + "/" + "unityLibrary";
        if (!Directory.Exists(projectPath))
        {
            UnityEngine.Debug.LogError("Android Studio Project was not found: " + projectPath);
            return;
        }
        else
        {
            if (SetupAndroidStudioProject(projectPath))
            {
                //int lastIndex = Application.dataPath.LastIndexOf("/");
                //string path1 = Application.dataPath.Substring(0, lastIndex) + "/BulidAAR";
                //UnityEngine.Debug.Log("path=" + path1);// todoLX 会报错，未查原因
 
                string path = Application.dataPath.Replace("/Assets", "") + "/BuildAAR";
                File.Copy(path + "/BuildAAR.bat", projectPath + "/BuildAAR.bat", true);
                Process proc = Process.Start(projectPath + "/BuildAAR.bat");
                proc.WaitForExit();
            }
        }
    }
 
 
 
    /// <summary>
    /// 设置Android Studio工程一些参数
    /// </summary>
    /// <param name="projectPath"></param>
    private static bool SetupAndroidStudioProject(string projectPath)
    {
        if (null == andoridFiles)
            andoridFiles = new Dictionary<string, string>();
        else
            andoridFiles.Clear();
 
        FindFiles(andoridFiles, projectPath, androidNeedUpdateFile);
        var count = 0;
        foreach (var temp in andoridFiles)
        {
            //UnityEngine.Debug.Log("key: " + temp.Key + "---value: " + temp.Value);
            if (temp.Key.Equals("build.gradle"))
            {
                if (UpdateBuildGradle(temp.Value))
                {
                    ++count;
                    UnityEngine.Debug.Log("修改了" + temp.Key + "文件");
                }
            }
            if (temp.Key.Equals("AndroidManifest.xml"))
            {
                if (UpdateAndroidManifest(temp.Value))
                {
                    ++count;
                    UnityEngine.Debug.Log("修改了" + temp.Key + "文件");
                }
            }
        }
 
        if (androidNeedUpdateFile.Count != count)
        {
            UnityEngine.Debug.Log("有文件漏改了");
            return false;
        }
 
        return true;
    }
 
    /// <summary>
    /// 修改build.gradle
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static bool UpdateBuildGradle(string filePath)
    {
        if (!File.Exists(filePath))
            return false;
 
        try
        {
            StreamReader reader = new StreamReader(filePath);
            var content = reader.ReadToEnd().Trim();
            reader.Close();
 
            // todoLX
            if (content.Contains("implementation fileTree(dir: 'libs', include: ['*.jar'])"))
                content = content.Replace("implementation fileTree(dir: 'libs', include: ['*.jar'])", "api fileTree(dir: 'libs', include: ['*.jar'])");
 
            // 将【Application】改成【Library】（library编译后的产物.aar可以发布到仓库供多个项目使用）
            if (content.Contains("com.android.application"))
                content = content.Replace("com.android.application", "com.android.library");
 
            //去掉applicationId（只有 Android 应用才能定义 applicationId ，不然编译会报错的）
            if (!content.Contains("//applicationId") && content.Contains("applicationId"))
                content = content.Replace("applicationId", "//applicationId");
 
            StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create));
            writer.WriteLine(content);
            writer.Flush();
            writer.Close();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("UpdateAndroidManifest - Failed: " + e.Message);
            return false;
        }
 
        return true;
    }
 
    /// <summary>
    /// 修改AndroidManifest.xml
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static bool UpdateAndroidManifest(string filePath)
    {
        if (!File.Exists(filePath))
            return false;
 
        try
        {
            StreamReader reader = new StreamReader(filePath);
            var content = reader.ReadToEnd().Trim();
            reader.Close();
 
            //去掉 intent-filter 标签
            var firstStr = "<intent-filter>";
            var secondStr = "</intent-filter>";
            var firstIndex = content.IndexOf(firstStr);
            var lastIndex = content.LastIndexOf(secondStr);
            var count = lastIndex - firstIndex + secondStr.Length;
            if (firstIndex > 0 && count > 0)
                content = content.Remove(firstIndex, count);
 
            StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create));
            writer.WriteLine(content);
            writer.Flush();
            writer.Close();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("UpdateAndroidManifest - Failed: " + e.Message);
            return false;
        }
 
        return true;
    }
 
    private static void FindFiles(Dictionary<string, string> findFiles, string path, List<string> needFindFiles)
    {
        if (!Directory.Exists(path))
            return;
 
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        var files = dirInfo.GetFileSystemInfos();
        for (int i = 0; i < files.Length; ++i)
        {
            var fileName = files[i].Name;
            if (fileName.Contains("."))//文件
            {
                string fileFullName = files[i].FullName;
                if (needFindFiles.Contains(fileName))
                {
                    if (findFiles.ContainsKey(fileName))
                        findFiles[fileName] = fileFullName;
                    else
                        findFiles.Add(fileName, fileFullName);
                }
            }
            else//文件夹，进入递归
                FindFiles(findFiles, files[i].FullName, needFindFiles);
        }
    }
 
    /// <summary>
    /// Android Studio工程输出路径
    /// </summary>
    /// <returns></returns>
    private static string GetPublishPath(bool isBulid)
    {
        int lastIndex = Application.dataPath.LastIndexOf("/");
        string publishPath = Application.dataPath.Substring(0, lastIndex) + path_Output;
        if (isBulid)
        {
            if (!Directory.Exists(publishPath))
            {
                Directory.CreateDirectory(publishPath);
            }
        }
        return publishPath;
    }
 
    #region 打开文件夹
    private static void OpenDirectory(string path)
    {
        // 新开线程防止锁死
        Thread newThread = new Thread(new ParameterizedThreadStart(CmdOpenDirectory));
        newThread.Start(path);
    }
 
    private static void CmdOpenDirectory(object obj)
    {
        Process p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = "/c start " + obj.ToString();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
 
        p.WaitForExit();
        p.Close();
    }
    #endregion
}