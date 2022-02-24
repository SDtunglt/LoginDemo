using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sfs2X.Entities.Data;
using UnityEngine;

public class ReplayModel
{
    public static int VERSION;
    public static int zone;
    public static int room;
    public static int board;
    public static int mode;
    public static int gameNo;
    public static int gaScore;
    public static int gaInitScore;
    public static string prevWin;
    public static int stake;
    public static int pot;
    public static int gameType;
    public static int minScore;
    public static List<long> beginScore;
    public static List<int> sumupScore;
    public static List<long> userCoins;
    public static List<long> endCoins;
    public static int turnTime;
    public static List<ReplayUser> users;
    public static List<string> actions;
    public static SumUpVO sumUpVo;


    /**parse data*/
    public static int TIME; 

    public static string[] uInfo;
    public static string data;

    public const char DELIMITER = '|';
    public const char DELIMITER_LEVEL_1 = '!';
    public const char DELIMITER_LEVEL_2 = '@';
    public const char DELIMITER_ARRAY = ',';

    public static void ReceiveReplay(string data)
    {
        var vo = new ReplayVO();
        var obj = SFSObject.NewFromJsonData(data);
        vo.fromSFSObject(obj);
        Init(vo);
    }
    
    public static void ReceiveReplay(JObject data)
    {
        var vo = new ReplayVO();
        vo.fromSFSObject(data);
        Init(vo);
    }

    public static void Init(ReplayVO vo)
    {
        ReinitData();
        uInfo = vo.u.Split('|');
        TIME = vo.t;
        data = vo.d;
        ParseData(data);
    }

    private static void ParseData(string d)
    {
        var arr = d.Split(DELIMITER);
        var i = 0;
        VERSION = int.Parse(arr[i++]);
        zone = int.Parse(arr[i++]);
        room = int.Parse(arr[i++]);
        board = int.Parse(arr[i++]);
        if (VERSION >= 5)
        {
            // Tinh diem
            mode = int.Parse(arr[i++]);
            SDLogger.LogError("Version Mode: " + mode);
            if (mode == 1 || mode == 2)
            {
                try
                {
                    gameNo = b36(arr[i++]);
                }
                catch (Exception e)
                {
                    gameNo = 0;
                }
                gaInitScore = b36(arr[i++]);
                SDLogger.LogError("Ga Score: " + gaScore);
                gaScore = gaInitScore;
                var arrBegin = arr[i++].Split(DELIMITER_ARRAY);
                var arrEnd = arr[i++].Split(DELIMITER_ARRAY);
                for (var j = 0; j < arrBegin.Length; j++)
                {
                    beginScore.Add(b36(arrBegin[j]));
                    sumupScore.Add(b36(arrEnd[j]));
                }
            }
        }

        userCoins = arr[i++].Split(DELIMITER_ARRAY).Select(long_b36).ToList();
        var prv = arr[i++];
        stake = int.Parse(arr[i++]);
        pot = int.Parse(arr[i++]);
        if (pot > 0) pot *= stake;
        gaScore = pot;
        gaInitScore = pot;
        minScore = int.Parse(arr[i++]);
        turnTime = b36(arr[i++]) * 1000;
        actions = new List<string>();
        actions.Add("1_" + arr[i].Substring(1) + "_" + b36(arr[i++][0].ToString())); //cards
        actions.Add("2_" + arr[i].Substring(1) + "_" + b36(arr[i++][0].ToString())); //score vao ga
        var plays = arr[i++].Split(DELIMITER_LEVEL_1);
        var autoXuong = false;
        if (plays != null)
            for (var j = 0; j < plays.Length; j++)
            {
                if (plays[j].Length > 3 && plays[j].IndexOf(DELIMITER_LEVEL_2) == -1)
                {
                    actions.Add("4_" + plays[j].Substring(2) + "_" + 36);
                    autoXuong = true;
                }
                else if (plays[j].Length > 0)
                {
                    // SDLogger.LogError(plays[j]);
                    actions.Add("3_" + plays[j].Substring(1) + "_" + b36(plays[j][0].ToString()));
                }
            }

        if (!autoXuong)
        {
            var x = arr[i++];
            SDLogger.LogError(x);

            if (!string.IsNullOrEmpty(x))
            {
                var uActs = x.Split(DELIMITER_ARRAY);
                for (var j = 0; j < uActs.Length; j++)
                {
                    actions.Add("4_" + uActs[j].Substring(1) + "_" + b36(uActs[j][0].ToString()));
                }
            }
           
            actions.Add("5_" + arr[i].Substring(1) + "_" + arr[i][0].ToString());
        }
        else actions.Add("5_" + arr[i].Substring(1) + "_" + arr[i][0].ToString());

        users = new List<ReplayUser>();
        for (int j = 0; j < userCoins.Count; j++)
        {
            users.Add(new ReplayUser
            {
                uid = uInfo[j],
                uName = uInfo[j + userCoins.Count],
                coin = userCoins[j]
            });
        }

        if (!string.IsNullOrEmpty(prv))
        {
            prevWin = users[int.Parse(prv)].uid;
        }
        SDLogger.LogError(prevWin + "  " + prv);
        
        SetUpSumUpVo();
    }

    public static int b36(string s)
    {
        var firstChar = s[0];
        if (firstChar == '-')
        {
            return -GameExtension.ToBase10(s.Substring(1), 36);
        }
        return GameExtension.ToBase10(s, 36);
    }

    public static long long_b36(string s)
    {
        var firstChar = s[0];
        if (firstChar == '-')
        {
            return -GameExtension.ToBaseInt64(s.Substring(1), 36);
        }
        return GameExtension.ToBaseInt64(s, 36);
    }

    public static void ReinitData()
    {
        prevWin = null;
        users = new List<ReplayUser>();
        actions = new List<string>();
        beginScore = new List<long>();
        sumupScore = new List<int>();
        userCoins = new List<long>();
        endCoins = new List<long>();
    }

    public static SumUpVO GetSumUpVo(string matchId)
    {
        if (sumUpVo != null && sumUpVo.logId == matchId)
        {
            return sumUpVo;
        }
        else
        {
            var v = actions.Find(s => s.StartsWith("5_"));
            if (v == null)
            {
                sumUpVo = null;
                return sumUpVo;
            }
            else
            {
                sumUpVo = SumUpVO.fromData(VERSION, v.Split('_')[1]);
                return sumUpVo;
            }
        }
    }

    private static void SetUpSumUpVo()
    {
        var v = actions.Find(s => s.StartsWith("5_"));
        if (v == null)
        {
            sumUpVo = null;
        }
        else
        {
            sumUpVo = SumUpVO.fromData(VERSION, v.Split('_')[1]);
        }
    }

    public static bool IsDuplicateMatch(string matchId)
    {
        if (sumUpVo != null && sumUpVo.logId == matchId)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class ReplayUser
{
    public string uid, uName;
    public long coin;
}