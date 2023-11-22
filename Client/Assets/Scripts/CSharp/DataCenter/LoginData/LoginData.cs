using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginData : BaseData
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public LoginData()
    {
        UserName = PlayerPrefs.GetString("username");
        Password = PlayerPrefs.GetString("password");
    }

    public void SetAccountInfo(string username, string password)
    {
        this.UserName = username;
        this.Password = password;
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("password", password);

        LoginEvent eventData = new LoginEvent(username, password);
        this.Broadcast(eventData);
    }

    public override void Dispose()
    {
        base.Dispose(); //清理事件注册
    }
}