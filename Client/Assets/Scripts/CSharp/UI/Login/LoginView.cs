using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoginView : UIBaseView
{
    public Text userNameText;
    public Text passwordText;
    public Button confirmBtn;

    private LoginCtrl _loginCtrl;
    private LoginData _loginData;
    
    #region 生命周期

    protected override void OnCreate()
    {
        _loginCtrl = new LoginCtrl();
        _loginData = DataMgr.Instance.LoginData;
        
        userNameText.text = _loginData.UserName;
        passwordText.text = _loginData.Password;
        confirmBtn.onClick.AddListener(OnLoginBtnClick);
    }

    protected override void OnClose()
    {
    }

    protected override void OnAddListener()
    {
        //this.AddListener<WindowOpen>(OnWindowOpen).UnRegisterWhenGameObjectDestroyed(this);
    }

    protected override void OnRemoveListener()
    {
        //使用自动反注册
        //this.RemoveListener<WindowOpen>(OnWindowOpen);
    }

    #endregion

    #region 业务逻辑

    private void OnLoginBtnClick()
    {
        _loginCtrl.Login(userNameText.text, passwordText.text);
    }

    #endregion
}