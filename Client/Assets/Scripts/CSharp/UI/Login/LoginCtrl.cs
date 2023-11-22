using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCtrl : UIBaseCtrl
{
    private LoginData _loginData;

    public LoginCtrl()
    {
        _loginData = DataMgr.Instance.LoginData;
    }

    public void Login(string userName, string passwork)
    {
        NetMgr.Instance.SendMsg(100,null);
    }
}
