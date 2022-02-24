using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class SDBorderLoader
{
    private static Dictionary<string, int> BorderCache = new Dictionary<string, int>();

    public static void LoadBorder(string uid, Action<int> onComplete, Action<string> onError)
    {
        if (BorderCache.ContainsKey(uid))
        {
            onComplete.Invoke(BorderCache[uid]);
            return;
        }

        if (uid == UserModel.Instance.uid)
        {
            onComplete.Invoke(UserModel.Instance.currentSelectBorder);
            return;
        }

        var d = new JObject() {{"uids", new JArray(new int[]{int.Parse(uid)})}};
        API.GetBorderById(data =>
        {
            if (data["status"].ToString() == "OK")
            {
                var x = data.GetValue("f");
                Debug.Log(x.ToString());
                foreach (int token in x)
                {
                    Debug.Log("UID: " + uid + "  " + token);
                    BorderCache.Add(uid, token);
                    onComplete.Invoke(BorderCache[uid]);
                    return;
                }
            }
            else
            {
                BorderCache.Add(uid, 0);
                onError.Invoke("");
            }
        }, (s) =>
        {
            BorderCache.Add(uid, 0);
            onError.Invoke(s);
        }, d.ToString());
    }
    
}