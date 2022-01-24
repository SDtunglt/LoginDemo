using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LocalStorageUtils : MonoBehaviour
{
    private const string _FACEBOOK = "_fb";
    private const string _USERNAME = "_u";
    private const string _PASSWORD = "_p";
    private const string _CLIENT_VERSION = "_v";
    private const string _APPLE_ID = "_a_id";
    private const string _FCM_TOKEN = "fcm_token";
    private const string _DEVICE_ID = "_dv_id";
    private const string _OFF_NOTICE_TIME = "_notice_time";
    private const string _KICKED_ROOM = "_kicked_room";
    private const string _RATED_APP = "_rate_app";
    private const string _SHOW_TUTORIAL_PREFIX = "_s_tut_";
    private const string _SHOW_KHUNG_AVT = "_s_avt";
    private const string _FPS_SETTING = "_fps_setting";

    public static void SetFps(int fps)
    {
        PlayerPrefs.SetInt(_FPS_SETTING, fps);
    }

    public static int GetFps()
    {
        return PlayerPrefs.GetInt(_FPS_SETTING, 30);
    }
    public static string GetString(string key)
    {
        var value = PlayerPrefs.GetString(key);
        if (!string.IsNullOrEmpty(value))
        {
            return EncryptUtils.Decrypt(value);
        }

        return "";
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, EncryptUtils.Encrypt(value));
    }

    public static void SetUsername(string u)
    {
        SetString(_USERNAME, u);
    }

    public static string GetUsername()
    {
        return GetString(_USERNAME);
    }

    public static void SetDataFb(string data)
    {
        SetString(_FACEBOOK, data);
    }

    public static string GetDataFb()
    {
        return GetString(_FACEBOOK);
    }

    public static void SetChangeLogVersion(string v)
    {
        SetString(_CLIENT_VERSION, v);
    }

    public static string GetChangeLogVersion()
    {
        return GetString(_CLIENT_VERSION);
    }

    public static void SetPassword(string p)
    {
        SetString(_PASSWORD, p);
    }

    public static string GetPassword()
    {
        return GetString(_PASSWORD);
    }

    public static void ClearClientVersion()
    {
        PlayerPrefs.DeleteKey(_CLIENT_VERSION);
    }

    public static void SetAppleId(string appleId)
    {
        SetString(_APPLE_ID, appleId);
    }

    public static string GetAppleId()
    {
        return GetString(_APPLE_ID);
    }

    public static void DeleteAppleId()
    {
        PlayerPrefs.DeleteKey(_APPLE_ID);
    }

    public static void SetFCMToken(string fcmToken)
    {
        SetString(_FCM_TOKEN, fcmToken);
    }

    public static string GetFCMToken()
    {
        return GetString(_FCM_TOKEN);
    }

    public static void SetDeviceID(string id)
    {
        
    }

    public static string GetDeviceID()
    {
        return GetString(_DEVICE_ID);
    }

    public static float OffAutoNotify{ 
        get { 
            return PlayerPrefs.GetFloat(_OFF_NOTICE_TIME,-1);
        }
        set
        {
            PlayerPrefs.SetFloat(_OFF_NOTICE_TIME, value);
        }
    }

    public static Dictionary<string, long> GetKickedRoom()
    {
        if (PlayerPrefs.HasKey(_KICKED_ROOM))
        {
            var d = JsonConvert.DeserializeObject(PlayerPrefs.GetString(_KICKED_ROOM), typeof(Dictionary<string, long>));
            return d as Dictionary<string, long>;
        }
        else
        {
            return new Dictionary<string, long>();
        }
    }
    
    public static void SaveKickedRoom(Dictionary<string, long> kickedRooms)
    {
        PlayerPrefs.SetString(_KICKED_ROOM, JsonConvert.SerializeObject(kickedRooms));
    }
    
    public static List<string> GetRatedApp()
    {
        if (PlayerPrefs.HasKey(_RATED_APP))
        {
            var d = JsonConvert.DeserializeObject(PlayerPrefs.GetString(_KICKED_ROOM), typeof(List<string>));
            return d as List<string>;
        }
        else
        {
            return new List<string>();
        }
    }
    
    public static void AddRatedApp(string uid)
    {
        var ls = GetRatedApp();
        if (!ls.Contains(uid))
        {
            ls.Add(uid);
        }
        
        PlayerPrefs.SetString(_RATED_APP, JsonConvert.SerializeObject(ls));
    }

    public static bool IsShowRate(string uid)
    {
        return GetRatedApp().Contains(uid);
    }

    public static bool IsShowTutorialYet(string uid)
    {
        return PlayerPrefs.HasKey(_SHOW_TUTORIAL_PREFIX + uid);
    }

    public static void AddShowTutorial(string uid)
    {
        PlayerPrefs.SetInt(_SHOW_TUTORIAL_PREFIX + uid, 1);
    }

}
