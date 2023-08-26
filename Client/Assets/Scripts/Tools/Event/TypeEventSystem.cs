using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUnRegister
{
    void UnRegister();
}

public interface IUnRegisterList
{
    List<IUnRegister> UnregisterList { get; }
}

public static class IUnRegisterListExtension
{
    public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unRegisterList)
    {
        unRegisterList.UnregisterList.Add(self);
    }

    public static void UnRegisterAll(this IUnRegisterList self)
    {
        foreach (var unRegister in self.UnregisterList)
        {
            unRegister.UnRegister();
        }

        self.UnregisterList.Clear();
    }
}

/// <summary>
/// 自定义可注销的类
/// </summary>
public struct CustomUnRegister : IUnRegister
{
    /// <summary>
    /// 委托对象
    /// </summary>
    private Action mOnUnRegister { get; set; }

    /// <summary>
    /// 带参构造函数
    /// </summary>
    /// <param name="onDispose"></param>
    public CustomUnRegister(Action onUnRegsiter)
    {
        mOnUnRegister = onUnRegsiter;
    }

    /// <summary>
    /// 资源释放
    /// </summary>
    public void UnRegister()
    {
        mOnUnRegister.Invoke();
        mOnUnRegister = null;
    }
}

public class UnRegisterOnDestroyTrigger : MonoBehaviour
{
    private readonly HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

    public void AddUnRegister(IUnRegister unRegister)
    {
        mUnRegisters.Add(unRegister);
    }

    public void RemoveUnRegister(IUnRegister unRegister)
    {
        mUnRegisters.Remove(unRegister);
    }

    private void OnDestroy()
    {
        foreach (var unRegister in mUnRegisters)
        {
            unRegister.UnRegister();
        }

        mUnRegisters.Clear();
    }
}

public static class UnRegisterExtension
{
    public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
    {
        var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

        if (!trigger)
        {
            trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
        }

        trigger.AddUnRegister(unRegister);

        return unRegister;
    }

    public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
        where T : Component
    {
        return self.UnRegisterWhenGameObjectDestroyed(component.gameObject);
    }
}

public class TypeEventSystem
{
    private readonly EasyEvents mEvents = new EasyEvents();

    public static readonly TypeEventSystem Global = new TypeEventSystem();

    public void Send<T>() where T : new()
    {
        mEvents.GetEvent<EasyEvent<T>>()?.Trigger(new T());
    }

    public void Send<T>(T e)
    {
        mEvents.GetEvent<EasyEvent<T>>()?.Trigger(e);
    }

    public IUnRegister Register<T>(Action<T> onEvent)
    {
        var e = mEvents.GetOrAddEvent<EasyEvent<T>>();
        return e.Register(onEvent);
    }

    public void UnRegister<T>(Action<T> onEvent)
    {
        var e = mEvents.GetEvent<EasyEvent<T>>();
        if (e != null)
        {
            e.UnRegister(onEvent);
        }
    }
}

public interface IOnEvent<T>
{
    void OnEvent(T e);
}

public static class OnGlobalEventExtension
{
    public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct
    {
        return TypeEventSystem.Global.Register<T>(self.OnEvent);
    }

    public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct
    {
        TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
    }
}