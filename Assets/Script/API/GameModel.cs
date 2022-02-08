using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Sfs2X.Entities.Data;
using UnityEngine;

public class GameModel : Singleton<GameModel>
{
    public string loginSession = "";
    public string mqttHost = "";
    public int mqttPort = 0;
    public string mqttUserName = "";
    public string mqttPassWord = "";
    public bool mqttEnable = false;
    public int payEnable = 3;
    public int totalPlay = 0;
    public int myAvatarDate = -1;
    public int minPlay = 10;
    

    private UserModel  userModel = UserModel.Instance;

    public static Dictionary<string, bool> Features = new Dictionary<string, bool>()
    {
        {Mission, false},
        {PayHelpReq, false},
        {PayCard, false},
        {PayCardNew, false},
        {ArenaNew, false},
        {BlackList, false},
        {LoginGift, false},
        {IntroductoryGift, false},
        {PhoneAuthentication, false},
        {TopWeekVuong, false},
        {payNew, false},
        {firstPurchase, false},
        {moQuanNoc, false},
        {lyNguFeature, false},
        {tamHoaFeature, false},
        {congthanFeature, false},
        {VinaFeature, false},
        {Event10YFeature, false},
        {ChallengeFeature, false},       
        {TetFeature, false}
    };

    public const string Mission = "1";
    public const string PayHelpReq = "2";
    public const string PayCard = "3";
    public const string PayCardNew = "4";
    public const string ArenaNew = "12";
    public const string BlackList = "10";
    public const string LoginGift = "11";
    public const string IntroductoryGift = "14";
    public const string PhoneAuthentication = "15";
    public const string TopWeekVuong = "13";
    public const string payNew = "16";
    public const string firstPurchase = "17";
    public const string moQuanNoc = "18";
    public const string lyNguFeature = "19";
    public const string tamHoaFeature = "20";
    public const string VinaFeature = "28";
    public const string congthanFeature = "29";
    public const string Event10YFeature = "30";
    public const string ChallengeFeature = "31";
    public const string TetFeature = "32";


    public void Init(JObject obj)
    {
        Debug.Log($"Game model init: {obj.ToString()}");
        loginSession = (string) obj["sid"];

        userModel.name = (string) obj["name"];    
        userModel.uid = (string) obj["uid"];
        GameConfig.HOST = (string) obj["host"];
        GameConfig.PORT = int.Parse(obj["port"].ToString());
        var encodeConfig = (string) obj["cfg"];
        if(encodeConfig == null)
        {
            Debug.Log("encode config is null");
            return;
        }

        var jsonConfig = EncryptUtils.Decrypt(encodeConfig);
        var config = JObject.Parse(jsonConfig);

        if(config["host"] != null)
        {
            GameConfig.HOST = (string) config["host"];
            Debug.Log($"cfg host: {(string) config["host"]}");
            Debug.Log($"cfg port: {(string) config["port"]}");
        }

        if(config["port"] != null) GameConfig.PORT = int.Parse(config["port"].ToString());

        SetUpHostPortWebGL();

        Debug.Log($"Game config host: {GameConfig.HOST}");
        Debug.Log($"Game config port: {GameConfig.PORT}");

        if(config["mqtt"] != null)
        {
            var mqttCfg = ((string) config["mqtt"]).Split(':');
            mqttHost = mqttCfg[0];
            mqttPort = int.Parse(mqttCfg[1]);
            mqttPassWord = "";
            if(mqttCfg.Length >= 2)
            {
                mqttUserName = mqttCfg[2];
                for(var i = 3; i < mqttCfg.Length;i++)
                {
                    mqttPassWord = mqttPassWord + mqttCfg[i];
                }
            }

            mqttEnable = true;
        }
    }
    private bool isConnectMqttLost;

    public bool IsNormalPlayer()
    {
#if UNITY_IOS
        return IsNormalPlayeriOS();
#endif
#if UNITY_ANDROID
        return IsNormalPlayerAndroid();
#endif
        return true;
    }

    public bool IsNormalPlayerAndroid()
    {
        return IsEnableCardOnly || IsEnablePay || (IsPayEnableUnKnown && !IsShowHideForReview());
    }

    public bool IsNormalPlayeriOS()
    {
        Debug.Log("payEnable: " + payEnable);
        return !(payEnable == 3 && IsShowHideForReview() || payEnable == 2);
    }
    private bool IsEnableCardOnly => payEnable == 4;
    private bool IsEnablePay => payEnable == 1;
    private bool IsPayEnableUnKnown => payEnable == 3;

    private bool IsShowHideForReview()
    {
        // Debug.Log("review total play: " + totalPlay);
        //số ván đánh < minPlay + có điểm kn <= 50 hoặc 550 <= myUserModel.gVO.exp && myUserModel.gVO.exp <= 590
        // đề phòng trường hợp được cộng điểm kn để thành mõ làng trên server.

        return ((userModel.gVO.exp <= 100) || (550 <= userModel.gVO.exp && userModel.gVO.exp <= 650)) &&
               (totalPlay < minPlay);
    }



    public void SetUpHostPortWebGL()
    {        
        if (GameUtils.IsWeb())
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var x = GetHostPortConfig();
            if (x != "")
            {
                var cfgGameServer = EncryptUtils.Decrypt(x);
                
                var config = cfgGameServer.Split(':');
                GameConfig.HOST = config[0];
                GameConfig.PORT = int.Parse(config[1]);
                Debug.Log("xen host: " + GameConfig.HOST);
                Debug.Log("xen port: " + GameConfig.PORT);
            }
#endif  
        }
    }
}
