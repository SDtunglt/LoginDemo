using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;

public class LoginMediator : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipfUsername,ipfPassword;
    private GameModel gameModel = GameModel.Instance;
    private UserModel userModel = UserModel.Instance;
    private SmartFoxConnection sfs;
    private bool isRegistered = false;
    private string _loginStatusToTestToTest = "";

    private void OnEnable()
    {
        sfs = SmartFoxConnection.Instance;
        Input.multiTouchEnabled = false;

    }

    public void OnClickLogin()
    {
        var username = ipfUsername.text;
        var password = ipfPassword.text;

        if(CheckInput(username,password))
        {
            DoLogin(username,password,false);
        }
    }

    public bool CheckInput(string user, string pass)
    {
        if(user.Length == 0)
        {
            Debug.Log("Vui lòng nhập tài khoản");
            return false;
        }
        if(pass.Length == 0)
        {
            Debug.Log("Vui lòng nhập mật khẩu");
            return false;
        }

        return true;
    }

    public void DoLogin(string username, string password, bool isRegistered)
    {
        var data = LoginDataRequest.Create(username, password).ToJson();
        this.isRegistered = isRegistered;
        API.Login(OnLoginSuccess, OnLoginError, data);
    }

    private void OnLoginError(string error)
    {
        _loginStatusToTestToTest = error;
        Debug.Log($"Thông báo, không thể kết nối tới web Api\n {error}");
    }

    private void OnLoginSuccess(JObject obj)
    {
        var status = (string) obj["status"];
        if(string.CompareOrdinal(status.ToLower(),"ok") != 0)
        {
            Debug.Log((string) obj["reason"]);
            _loginStatusToTestToTest = (string) obj["reason"];
            return;
        }

        Debug.Log($"Login success obj: {obj.ToString()}");
        _loginStatusToTestToTest = "LoginSuccess";
        
        GameConfig.LOGIN_COUNT++;
        gameModel.Init(obj);

        GetInitData();
        SmartFoxConnection.Instance.Connect();
        LoginScreen.Instance.gameObject.SetActive(false);
        LoginScreen.Instance.lobby.gameObject.SetActive(true);
        //LoginScreen.Instance.success.gameObject.SetActive(true);
    }

    private void GetInitData()
    {
        API.GetInitData();
    }
}
