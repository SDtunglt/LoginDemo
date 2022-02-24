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
    public static int COIN_RATIO = 20;
    public static int MaxBoardInRoom = 30;
    public static long MIN_COIN = 20000;
    public const int CHALLENGE_RATIO = 10;
    
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
    public static int[][] ZoneStake;
    public static List<VuongConfigVO> VuongCfg = new List<VuongConfigVO>();
    public static List<string> arrayGroupSubscribe = new List<string>();

    public const string VAR_SITCOUNT = "c";
    public const string VAR_STAKE = "k";
    public const string VAR_MIN_STAKE = "i";
    public const string VAR_MAX_STAKE = "a";
    public const string VAR_PLAYERS_POS = "o";
    public const string VAR_TIME = "t";
    public const string VAR_SCORE = "s";
    public const string VAR_DIS = "l";
    public const string VAR_EXP = "x";
    public const string VAR_MIN_U = "u";
    public const string VAR_GA_NUOI = "u";
    public static string VAR_BOC_NOC = "b";
    public const string VAR_GAME_MODE = "m";
    public const string VAR_PREV_PLAYERS = "p";
    public const string VAR_AUTO_XEP = "x";
    public const string VAR_U_LAO = "f";
    public const string VAR_UNAME = "n";
    public const string VAR_GA_GOP = "cg";
    public const string VAR_GAME_NO = "n";
    public const string VAR_COIN = "c";
    public const string VAR_PREV_WIN = "w";
    public const string VAR_IP = "i";
    public const string VAR_IS_NUOI_GA = "@";
    public const int MIN_PLAYER_TO_START_GAME = 2;
    public const int MAX_PLAYER_IN_GAME = 4;

    public static string NameDefault = "Chắn Sân Đình";
    public static List<int> MODS = new List<int>()
    {
        1, 2, 12, 19, 20, 21, 22, 28, 37, 30452, 74186, 211560, 525502, 647246, 2661484, 10, 4460959, 4060719, 1985,4844018, 8
    };

    public static bool IsTourZoneId(int z)
    {
        return z == IdRoomThiHuong || z == IdRoomThiHoi || z == IdRoomThiDinh;
    }
}
