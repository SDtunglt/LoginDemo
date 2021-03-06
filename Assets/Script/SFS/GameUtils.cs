using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Sfs2X.Util;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = System.Random;

public static class GameUtils
{
    public static bool IsCTDCActive ;
    public static bool IsUI2 { get; private set; }
    private const string SaveCard = "card_saved";

    private static readonly List<int> ALPHA_CHAR_CODES = new List<int>()
        { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70 };
    public static bool IsWeb()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }
    public static string GetPlatform()
    {
        // switch (Application.platform)
        // {
        //     case RuntimePlatform.IPhonePlayer:
        //         return "IOS";
        //
        //     case RuntimePlatform.Android:
        //         return "Android";
        // }
#if UNITY_IOS
        return "IOS";
#elif UNITY_ANDROID
        return "Android";
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
        return "Web";
#endif
        return "Android";
    }

    public static bool IsAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }

    public static bool IsIOS()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }

    public static void OpenUrl(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenURLInExternalWindow( url);
        return;
#endif
        Application.OpenURL(url);
    }

    public static string GetAvatarUrl(string uid, string t, int date = -1)
    {
        if (uid == UserModel.Instance.uid && date == -1) return AvatarPicture(uid, t, GameModel.Instance.myAvatarDate);
        return AvatarPicture(uid, t, date);
    }

    public static string AvatarPicture(string uid, string t = "m", int date = -1)
    {
        var base_ava_url = GlobalDataManager.Ins.AVT_API;

        if (date < 0) return base_ava_url + "game/avatar?id=" + uid + "&type=" + t;
        return base_ava_url + "game/avatar?id=" + uid + "&type=" + t + "&date=" + date;
    }

    public static long TotalSeconds(this DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)(diff.TotalSeconds);
    }

    public static bool IsMod(int uid)
    {
        return uid > 0 && GameConfig.MODS.IndexOf(uid) != -1;
    }
    
    public static string GetDeviceId()
    {
        if (!IsWeb()) return SystemInfo.deviceUniqueIdentifier;
        var s = LocalStorageUtils.GetDeviceID();
        if (string.IsNullOrEmpty(s))
        {
            s = IsWeb() ? CreateID() : SystemInfo.deviceUniqueIdentifier;
            LocalStorageUtils.SetDeviceID(s);
        }

        Debug.Log("device id: " + s);
        return s;
    }

    public static List<int> byteArrayToIntArr(ByteArray barr)
    {
        var ret = new List<int>();
        barr.Position = 0; //Note!! khi d??ng ByteArray ph???i ch?? ?? ??i???u n??y
        while (barr.BytesAvailable > 0)
            ret.Add(barr.ReadByte());
        return ret;
    }
    
    public static string CreateID()
    {
        var uid = new int[36];
        var index = 0;
        var rand = new Random();
        for (var i = 0; i < 8; i++)
        {
            uid[index++] = ALPHA_CHAR_CODES[rand.Next(0, 15)];
        }

        for (var i = 0; i < 3; i++)
        {
            uid[index++] = 45; // charCode for "-"
            for (var j = 0; j < 4; j++)
            {
                uid[index++] = ALPHA_CHAR_CODES[rand.Next(0, 15)];
            }
        }

        uid[index++] = 45; // charCode for "-"
                           //        var time:Number = new Date().getTime();
        var time = DateTime.Now.Millisecond;
        // Note: time is the number of milliseconds since 1970,
        // which is currently more than one trillion.
        // We use the low 8 hex digits of this number in the UID.
        // Just in case the system clock has been reset to
        // Jan 1-4, 1970 (in which case this number could have only
        // 1-7 hex digits), we pad on the left with 7 zeros
        // before taking the low digits.
        var timeString = ("0000000" + time.ToString("X16")).ToUpper().Substring(0, 8);
        for (var i = 0; i < 8; i++)
        {
            uid[index++] = timeString[i];
        }

        for (var i = 0; i < 4; i++)
        {
            uid[index++] = ALPHA_CHAR_CODES[rand.Next(0, 15)];
        }

        var res = uid.Aggregate("", (current, c) => current + Convert.ToChar(c));
        return res;
    }


    public static string GetVersion()
    {
        return Application.version;
    }
    public static string GetBundleId()
    {
        return Application.identifier;
    }
    
    public static bool IsEditor()
    {
        return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor;
    }

    private static void OpenShopReview()
    {
        //LoadingEffect.Open();
        //WalletPopup.Open();
    }

    public static void SetCardSaved(List<int> listCard)
    {
        if (listCard.Count <= 0)
        {
            PlayerPrefs.DeleteKey(SaveCard);
            return;
        }

        var data = string.Join(",", listCard);
        SetLocalStorage(SaveCard, data);
    }


    private static void SetLocalStorage(string id, string value)
    {
        PlayerPrefs.SetString(id, value);
    }

    public static ByteArray intArrToByteArray(List<int> arr)
    {
        var ret = new ByteArray();
        foreach (var a in arr)
        {
            ret.WriteByte((byte)a);
        }

        return ret;
    }

    public static List<int> GetCardSaved()
    {
        var data = GetLocalStorage(SaveCard);
        if (string.IsNullOrEmpty(data)) return new List<int>();

        var listData = data.Split(',').ToList();
        var listCard = new List<int>();
        for (var i = 0; i < listData.Count; i++)
        {
            if (string.IsNullOrEmpty(listData[i])) continue;
            listCard.Add(int.Parse(listData[i]));
        }

        return listCard;
    }

    private static string GetLocalStorage(string key)
    {
        var value = PlayerPrefs.GetString(key);
        return value;
    }

    public static bool ListIntContains(List<int> a, List<int> b)
    {
        return a.Count >= b.Count && b.All(x => a.IndexOf(x) != -1);
    }

    private static void OpenShopNormal()
    {
        
        ShopMediator.Open();
    }

    public static void OnShopClick()
    {
        if (IsIOS())
        {
            OpenShopReview();
            
            
            return;
        }
        if (((GameModel.Instance.IsNormalPlayer() && GameModel.Instance.payEnable != 2) || GameUtils.IsWeb()))
        {
            /*OpenShopNormal();
        }
        else
        {*/
            OpenShopReview();
        }
    }
    
}
