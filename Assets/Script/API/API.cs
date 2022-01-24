using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;


public static class API
{
    public static void Send(string method, string url,
        Action<JObject> onSuccess, Action<string> onFailure = null, string jsonData = "")
    {
        Debug.Log(GlobalDataManager.Ins.API_URL + url);
            HttpUtils.SendRequest(method,GlobalDataManager.Ins.API_URL + url,(responseText) =>
        {
            SDLogger.Log(responseText);
            try
            {
                // SDLogger.Log(responseText);
                onSuccess?.Invoke(JObject.Parse(responseText));
            }
            catch (Exception e)
            {
                SDLogger.LogError(e.Message);
            }
        }, onFailure, jsonData);
    }

    public static void Login(Action<JObject> onSuccess, Action<string> onFailure,string jsondata)
    {
        Debug.Log($"Login data: {jsondata}");
        Send("POST","login",onSuccess,onFailure,jsondata);
    }

    public static void GetInitData(Action<JObject> onSuccess, Action<string> onFailure, string data)
    {
        Send("POST", "get-init-data2", onSuccess, onFailure, data);
    }

    public static void GetUserDetail(Action<JObject> onSuccess, Action<string> onFailure, string uid, string app)
    {
        Send("GET", "detail/" + UnityWebRequest.EscapeURL(EncryptUtils.Encrypt(uid + "," + app)), onSuccess, onFailure);
    }

    public static void GetNotice(Action<JObject> onSuccess, Action<string> onFailure)
    {
        Send("GET", "notice/1", onSuccess, onFailure); // workaround appid 1
    }

    public static void GetInitData()
    {
        var agent = AgentData.Create();
        agent.app = 1;
        var reTryNum = 0;
        GetInitData(
            data =>
            {
                if (data["status"].ToString().ToLower() != "ok") return;
                
                Signals.Get<OnGetInitDataComplete>().Dispatch();
                Debug.Log("initdata: " + data.ToString());
            }, s =>
            {
                reTryNum++;
                if (reTryNum <= 3) GetInitData();
            }, 
            agent.ToJson());
    }

}
