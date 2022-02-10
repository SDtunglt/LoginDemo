using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public static int LOGIN_COUNT = 0;
    public const int APP_ID = 1;
    public const int VERSION = 312;
    public const string ZONE_DEFAULT = "sfsak";
    public static int PORT = 9012;
    public static string HOST = "";
    public static int MaxBoardInRoom = 30;
    
    public const int IdRoomChanh = 4;
    public const int IdRoomVuongPhu = 5;
    public const int IdRoomThiHuong = 6;
    public const int IdRoomThiHoi = 7;
    public const int IdRoomThiDinh = 8;
    public const int IdRoomChalenge = 9;

    public const int PhuDeGroupId = 2;
    public const int NoiDienGroupId = 1;
    public const int NgoaiDienGroupId = 0;
    
    public static int NormalZoneCount = 6;
    public static ZoneInfo[] ZoneCfg;
    public static int[][] ZOneStake;
    public static List<VuongConfigVO> VuongCfg = new List<VuongConfigVO>();
    public static List<string> arrayGroupSubscribe = new List<string>();

    public const string VAR_SITCOUNT = "c";
    public const string VAR_STAKE = "k";
    public const string VAR_MIN_STAKE = "i";
    public const string VAR_MAX_STAKE = "a";
    public const string VAR_TIME = "t";
    public const string VAR_MIN_U = "u";
    public const string VAR_GA_NUOI = "u";
    public const string VAR_GAME_MODE = "m";
    public const string VAR_COIN = "c";
    public const string VAR_IP = "i";
    public const string VAR_IS_NUOI_GA = "@";

    public static bool IsTourZoneId(int z)
    {
        return z == IdRoomThiHuong || z == IdRoomThiHoi || z == IdRoomThiDinh;
    }
}
