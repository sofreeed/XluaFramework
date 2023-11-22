using System;
using System.Collections;
using System.Collections.Generic;
using Networks;
using UnityEngine;

public class NetMgr : Singleton<NetMgr>
{
    private HjTcpNetwork _network;
    private Dictionary<int, Action<int, byte[]>> _handlers = new Dictionary<int, Action<int, byte[]>>();

    public void RegisterHandlers()
    {
        var loginHandler = new LoginHandler(_handlers);
        var heroHandler = new HeroHandler(_handlers);
    }

    public void Connect(string ip, int port, Action<object, int, string> onConnect,
        Action<object, int, string> onClosed)
    {
        _network ??= new HjTcpNetwork
        {
            ReceivePkgHandle = OnReceiveMsg
        };

        _network.OnConnect = onConnect;
        _network.OnClosed = onClosed;
        _network.SetHostPort(ip, port);
        _network.Connect();
    }

    public void SendMsg(int msgId, byte[] bytes)
    {
        byte[] sendBytes = null;
        //TODO：组织包体，加密等
        if (_network != null)
        {
            _network.SendMessage(sendBytes);
        }
    }

    private void OnReceiveMsg(byte[] bytes)
    {
        //TODO:解包
        int msgId = 1000;
        byte[] newBytes = null;
        if (_handlers.TryGetValue(msgId, out var handler))
        {
            handler.Invoke(msgId, newBytes);
        }
    }

    private void Update()
    {
        _network?.UpdateNetwork();
    }

    public void Close()
    {
        _network?.Close();
    }

    public override void Dispose()
    {
        _network?.Dispose();
        _network = null;
        _handlers.Clear();
    }
}