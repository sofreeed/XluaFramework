using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseData : IUnRegisterList
{
    public List<IUnRegister> UnregisterList { get; }

    protected void AddListener<T>(Action<T> action)
    {
        DataMgr.Instance.AddListener<T>(action).AddToUnregisterList(this);
    }

    protected void RemoveListener<T>(Action<T> action)
    {
        DataMgr.Instance.RemoveListener<T>(action);
    }

    protected void Broadcast<T>(T e)
    {
        DataMgr.Instance.Broadcast(e);
    }

    public virtual void Dispose()
    {
        this.UnRegisterAll();
    }
}