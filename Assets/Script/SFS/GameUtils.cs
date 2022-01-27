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

public class GameUtils
{
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
    
}
