using UnityEngine;

class ProcessUtil
{

    /// 构建Process对象，并执行
    private static System.Diagnostics.Process CreateCmdProcess(string cmd, string args, string workingDir = "")
    {
        var en = System.Text.UTF8Encoding.UTF8;
        if (Application.platform == RuntimePlatform.WindowsEditor)
            en = System.Text.Encoding.GetEncoding("gb2312");

        var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
        pStartInfo.Arguments = args;
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = false;
        pStartInfo.RedirectStandardError = true;
        pStartInfo.RedirectStandardInput = true;
        pStartInfo.RedirectStandardOutput = true;
        pStartInfo.StandardErrorEncoding = en;
        pStartInfo.StandardOutputEncoding = en;
        if (!string.IsNullOrEmpty(workingDir))
            pStartInfo.WorkingDirectory = workingDir;
        return System.Diagnostics.Process.Start(pStartInfo);
    }

    /// 运行命令,不返回stderr版本
    /// 命令的stdout输出
    public static string RunCmdNoErr(string cmd, string args, string workingDir = "")
    {
        var p = CreateCmdProcess(cmd, args, workingDir);
        var res = p.StandardOutput.ReadToEnd();
        p.Close();
        return res;
    }

    /// 运行命令,不返回stderr版本
    /// StandardInput
    /// 命令的stdout输出
    public static string RunCmdNoErr(string cmd, string args, string[] input, string workingDir = "")
    {
        var p = CreateCmdProcess(cmd, args, workingDir);
        if (input != null && input.Length > 0)
        {
            for (int i = 0; i < input.Length; i++)
                p.StandardInput.WriteLine(input[i]);
        }

        var res = p.StandardOutput.ReadToEnd();
        p.Close();
        return res;
    }
    
    /// 运行命令
    /// string[] res[0]命令的stdout输出, res[1]命令的stderr输出
    public static string[] RunCmd(string cmd, string args, string workingDir = "")
    {
        string[] res = new string[2];
        var p = CreateCmdProcess(cmd, args, workingDir);
        res[0] = p.StandardOutput.ReadToEnd();
        res[1] = p.StandardError.ReadToEnd();
        p.Close();
        return res;
    }
}