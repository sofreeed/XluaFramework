using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginHandler
{
    public LoginHandler(IDictionary<int, Action<int, byte[]>> handlers)
    {
        handlers.Add(MsgIdDefine.LoginRspLogin, OnLogin);
        handlers.Add(MsgIdDefine.LoginRspLogout, OnLogout);
    }

    private void OnLogin(int msgId, byte[] data)
    {
        //TODO：反序列化pb
        DataMgr.Instance.LoginData.SetAccountInfo("", "");
    }

    private void OnLogout(int msgId, byte[] data)
    {
    }
}