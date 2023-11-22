using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class Proto2CSUtil
{
    [MenuItem("Tools/Proto2CS")]
    public static void Proto2Cs()
    {
        //Proto2CS的根目录
        var protoRootDir = Path.Combine(Environment.CurrentDirectory, "Proto2CS/");

        var protoDir = Path.Combine(protoRootDir, "Proto/"); //Proto文件的目录
        var protoCsDir = Path.Combine(protoRootDir, "ProtoCS/"); //输出CS的目录
        var protocDir = Path.Combine(protoRootDir,
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "protoc.exe" : "protoc"); //Protoc.exe


        var directoryInfo = new DirectoryInfo(protoDir);
        foreach (var fileInfo in directoryInfo.GetFiles("*.proto"))
        {
            var argument2 = $"--csharp_out=\"{protoCsDir}\" --proto_path=\"{protoDir}\" " + fileInfo.Name;
            //Run(protocDir, argument2, waitExit: true);
            ProcessUtil.RunCmd(protocDir, argument2);
        }

        UnityEngine.Debug.Log("proto2cs succeed!");
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Proto2CS-Bat")]
    public static void Proto2CsBat()
    {
        //Proto2CS的根目录
        var protoRootDir = Path.Combine(Environment.CurrentDirectory, "Proto2CS/");

        var protoDir = Path.Combine(protoRootDir, "Proto/"); //Proto文件的目录
        var protoCsDir = Path.Combine(protoRootDir, "ProtoCS/"); //输出CS的目录
        var protocDir = Path.Combine(protoRootDir, "gen.bat"); //gen.bat

        string[] res = ProcessUtil.RunCmd(protocDir, "");
        
        UnityEngine.Debug.Log("proto2cs succeed!" + res[0]);
        UnityEngine.Debug.Log("proto2cs succeed!" + res[1]);
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Tools/PlatformTest")]
    public static void PlatformTest()
    {
        RuntimePlatform platform = Application.platform;

        bool isMobilePlatform =  Application.isMobilePlatform;

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        
        Debug.LogError(platform);

    }
}