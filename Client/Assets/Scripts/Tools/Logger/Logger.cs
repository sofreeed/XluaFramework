using System;
using UnityEngine;

public class Logger
{
    public enum LogLevel
    {
        None = 0,
        Exception = 1,
        Error = 2,
        Warning = 3,
        Normal = 4,
        Max = 5,
    }

    private static LogLevel mLogLevel = LogLevel.Normal;

    public static LogLevel Level
    {
        get => mLogLevel;
        set => mLogLevel = value;
    }

    public static void Info(object msg, params object[] args)
    {
        if (mLogLevel < LogLevel.Normal)
        {
            return;
        }

        if (args == null || args.Length == 0)
        {
            Debug.Log(msg);
        }
        else
        {
            Debug.LogFormat(msg.ToString(), args);
        }
    }


    public static void Warning(object msg, params object[] args)
    {
        if (mLogLevel < LogLevel.Warning)
        {
            return;
        }

        if (args == null || args.Length == 0)
        {
            Debug.LogWarning(msg);
        }
        else
        {
            Debug.LogWarningFormat(msg.ToString(), args);
        }
    }


    public static void Error(object msg, params object[] args)
    {
        if (mLogLevel < LogLevel.Error)
        {
            return;
        }

        if (args == null || args.Length == 0)
        {
            Debug.LogError(msg);
        }
        else
        {
            Debug.LogError(string.Format(msg.ToString(), args));
        }
    }


    public static void Exception(Exception e)
    {
        if (mLogLevel < LogLevel.Exception)
        {
            return;
        }

        Debug.LogException(e);
    }
}