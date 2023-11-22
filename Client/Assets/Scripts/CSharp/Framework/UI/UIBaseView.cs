using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBaseView : MonoBehaviour
{
    public void Create()
    {
        OnCreate();
    }

    public void Active()
    {
        OnAddListener();
    }

    public void UnActive()
    {
        OnRemoveListener();
    }

    public void Close()
    {
        OnClose();
    }

    protected IUnRegister AddListener<T>(Action<T> action)
    {
        return DataMgr.Instance.AddListener<T>(action);
    }

    protected void RemoveListener<T>(Action<T> action)
    {
        DataMgr.Instance.RemoveListener<T>(action);
    }

    protected void Broadcast<T>(T e)
    {
        DataMgr.Instance.Broadcast(e);
    }

    /// <summary>
    /// 面板被实例化，Awake后，Start前
    /// </summary>
    protected virtual void OnCreate()
    {
    }

    protected virtual void OnAnimation()
    {
    }

    protected virtual void OnShow()
    {
    }

    protected virtual void OnHide()
    {
    }

    protected virtual void OnClose()
    {
    }

    protected virtual void OnReconnect()
    {
    }

    protected virtual void OnAddListener()
    {
    }

    protected virtual void OnRemoveListener()
    {
    }
}