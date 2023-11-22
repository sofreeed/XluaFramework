using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIMgr : Singleton<UIMgr>
{
    private Dictionary<string, UIWindow> _openedWindows = new Dictionary<string, UIWindow>();
    private Stack<UIWindow> _reversedWindows = new Stack<UIWindow>();


    public void OpenWindow(string windowName)
    {
        var window = GetWindow(windowName);
        if (window == null)
        {
            //加载Window
        }
        else
        {
            //让window到最上面
        }
    }

    public void CloseWindow(string windowName)
    {
        var window = GetWindow(windowName);
    }

    public void HiddenWindow(string windowName)
    {
        var window = GetWindow(windowName);
    }

    private UIWindow GetWindow(string windowName)
    {
        _openedWindows.TryGetValue(windowName, out var window);
        return window;
    }

    #region Event

    //private readonly TypeEventSystem _uiEventSystem = new TypeEventSystem();
    
    //public IUnRegister AddListener<T>(Action<T> action)
    //{
    //    return _uiEventSystem.Register<T>(action);
    //}

    //public void RemoveListener<T>(Action<T> action)
    //{
    //    _uiEventSystem.UnRegister<T>(action);
    //}

    //public void Broadcast<T>(T e)
    //{
    //    _uiEventSystem.Send<T>(e);
    //}

    #endregion

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}