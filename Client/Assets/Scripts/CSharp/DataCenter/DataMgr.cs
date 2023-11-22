using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMgr : Singleton<DataMgr>
{
    public LoginData LoginData { get; set; }
    public HeroData HeroData { get; set; }

    public override void Init()
    {
        LoginData = new LoginData();
        HeroData = new HeroData();
    }

    public override void Dispose()
    {
        LoginData.Dispose();
        HeroData.Dispose();
    }

    private readonly TypeEventSystem _dataEventSystem = new TypeEventSystem();

    public IUnRegister AddListener<T>(Action<T> action)
    {
        return _dataEventSystem.Register<T>(action);
    }

    public void RemoveListener<T>(Action<T> action)
    {
        _dataEventSystem.UnRegister<T>(action);
    }

    public void Broadcast<T>(T e)
    {
        _dataEventSystem.Send<T>(e);
    }
}