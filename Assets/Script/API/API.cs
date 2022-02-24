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
    public static void GetThiDinhRank(Action<JObject> onSuccess, Action<string> onFailure)
    {
        Send("GET", "thi-dinh-rank", onSuccess, onFailure);
    }
    public static void GetListBorderAvatar(Action<JObject> onSuccess, Action<string> onFailure)
    {
        Send("GET", "list-frame-avatar?v=2_15_2", onSuccess, onFailure);
    }

    public static void GetRelay(string matchId, Action<JObject> onSuccess, Action<string> onFailure)
    {
        Send("GET", $"replay/{matchId}", onSuccess, onFailure);
    }
    
    public static void GetTotalReceived(Action<JObject> onSuccess, Action<string> onFailure, int hinhThuc, string nhaMang, string changeDisplay)
    {
        var data = hinhThuc + "," + nhaMang + "," + changeDisplay;
        Send("GET", "coin-received-estimate-new/" + data, onSuccess, onFailure);
    }
    
    public static void GetEstimateWallet(Action<JObject> onSuccess, Action<string> onFailure)
    {
        Send("GET", "coin-estimate-wallet-new", onSuccess, onFailure);
    }

    public static void GetCoinMod(Action<JObject> onSuccess, Action<string> onFailure)
    {
        Send("GET", "coin-estimate-mod", onSuccess, onFailure);
    }
    
    public static void PayCardNew(Action<JObject> onSuccess, Action<string> onFailure, string data)
    {
        Send("POST", "pay-card-bb2", onSuccess, onFailure, data);
    }
    public static void UploadAvatar(Action<JObject> onSuccess, Action onFailure, string filename,
    byte[] bytes)
    {
        HttpUtils.UploadFileToServer(GlobalDataManager.Ins.API_URL + "upload-avatar", filename, "picture", "image/jpeg", bytes, onSuccess, onFailure);
    }
    public static void UpdateBorderAvatar(Action<JObject> onSuccess, Action<string> onFailure, string jsonData)
    {
        Send("POST", "update-frame-avatar", onSuccess, onFailure, jsonData);
    }

    public static void GetBorderById(Action<JObject> onSuccess, Action<string> onFailure, string jsonData)
    {
        Send("POST", "get-frame-avatar-by-uids", onSuccess, onFailure, jsonData);
    }

    public static void DeleteAvatar(Action<JObject> successCb, Action<string> failCb)
    {
        Send("POST", "delete-avatar", successCb, failCb);
    }

    public static void GetFeatureConfig(Action<JObject> onSuccess, Action<string> onFailure, string jsonData) {
    Send("POST", "config", onSuccess, onFailure, jsonData);
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
                if (data["firstCharge"] != null)
                {
                    UserModel.Instance.firstCharge = (bool) data["firstCharge"];
                }
                else
                {
                    UserModel.Instance.firstCharge = false;
                }

                if (data["wave"] != null)
                {
                    UserModel.Instance.kmValue = (string) data["wave"];
                }
                else
                {
                    UserModel.Instance.kmValue = "";
                }
                if (data["app"] != null)
                {
                    GameModel.Instance.totalPlay = (int) data["app"]["t"];
                }
                else
                {
                    GameModel.Instance.totalPlay = 0;
                }
                
                Signals.Get<OnGetInitDataComplete>().Dispatch();
                Debug.Log("initdata: " + data.ToString());
            }, s =>
            {
                reTryNum++;
                if (reTryNum <= 3) GetInitData();
            }, 
            agent.ToJson());
        GetListBorderAvatar(data =>
        {
            UserModel.Instance.unlockBorders = data.GetValue("a").Select(s => (int) s).ToList();
            var x = (int) data.GetValue("c");
            UserModel.Instance.currentSelectBorder = GlobalDataManager.Ins.khungAvatarData.infos.Any(s => s.id == x) ? x : 0;
        }, Debug.Log);
    }

}
