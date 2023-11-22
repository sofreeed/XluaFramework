using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LoginEvent
{
    public string UserName;
    public string Password;

    public LoginEvent(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }
}

public class HeroInfoAddEvent
{
    public int HeroID;
    public HeroInfoItemData HeroItem;
}