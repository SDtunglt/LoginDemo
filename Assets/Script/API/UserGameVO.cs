using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class UserGameVO
{
    public long coin;
    private int _exp;

    public int exp
    {
        get { return _exp; }
        set
        {
            _exp = value;
            _level = CalculateLevel(_exp);
        }
    }

    private int _level;
    public int giftCount;
    private int _debt;
    private const uint UpgradeTime = 1392192000;


    private static int[] EXP_BOUNDS = new[] {50, 130, 250, 400, 550, 700, 850, 1000};
    private static int CHANH_TONG_EXP = 13200;

    public int level => _level;



    public int win;
    public int total;

    /**@param v - ex, "17,[2,[11,[2,16]],[1,[12]]]"
     * => set _strBigCuoc = "17 điểm. 2 ván [tám đỏ 2 lèo], 1 ván [kính tứ chi]"*/
    private string _strBigU;
    public string strBigU
    {
        get { return _strBigU; }
        set { _strBigU = GetBigWinBigCuoc(value); }
    }

    private string _strBigWin;
    public string strBigWin
    {
        get { return _strBigWin; }
        set { _strBigWin = GetBigWinBigCuoc(value); }
    }

    public void UpdateExtraInfo(JToken x)
    {
        if (x == null)
        {
            strBigU = "";
            strBigWin = "";
        }
        else
        {
            strBigU = x["c"].ToString();
            strBigWin = x["w"].ToString();
        }
    }

    private int _vipScore;
    public int vipScore
    {
        get { return _vipScore; }
        set
        {
            _vipScore = value; 
            vip = VipType.FromScore(value);
        }
    }
    public VipType vip = VipType.Dan;

    public static int CalculateLevel(int exp)
    {
        for (var i = 0; i < 8; i++)
            if (exp < EXP_BOUNDS[i])
                return i + 1;
        if (exp > CHANH_TONG_EXP)
            return (int) Math.Floor((double) (exp - CHANH_TONG_EXP) / 400) + 70;
        return (int) Math.Floor((double) (exp - 1000) / 200) + 9;
    }

    public static string GetLevelNameBy(int exp, int _level = -1)
    {
        string lvlName = "";
        if (exp >= 500000) lvlName = "Tổng Đốc";
        else if (exp >= 250000) lvlName = "Tuần Phủ";
        else if (exp >= 100000) lvlName = "Tri Phủ";
        else if (exp >= 50000) lvlName = "Tri Huyện";
        else lvlName = GetLevelName(_level);
        return lvlName;
    }

    private static List<string> LEVEL_NAMES = new List<string>() {
            "Nô bộc", "Hành khất", "Hàng rong", "Tiểu nhị", "Cửu vạn",
            "Mõ làng", "Phú nông", "Xã tuần", "Hương trưởng", "Phó lý", "Lý Trưởng",
            "Bá hộ", "Nha vệ", "Đề hạt", "Phó tổng", "Chánh Tổng"};

    public static int GetIndexLevelName(string levelName)
    {
        if (LEVEL_NAMES.Contains(levelName)) return LEVEL_NAMES.IndexOf(levelName);
        switch (levelName)
        {
            case "Tri Huyện": return 16;
            case "Tri Phủ": return 17;
            case "Tuần Phủ": return 18;
            case "Tổng Đốc": return 19;
        }
        return 0;
    }

//	private static const LEVEL_BOUNDS:Array = [12,17,27,39,52,69];
    public static string GetLevelName(int level) {
        if(level < 1)
            return LEVEL_NAMES[0];
        if(level <= 10)
            return LEVEL_NAMES[level - 1];
        if(level <= 12)
            return LEVEL_NAMES[9];
        if(level <= 17)
            return LEVEL_NAMES[10];
        if(level <= 27)
            return LEVEL_NAMES[11];
        if(level <= 39)
            return LEVEL_NAMES[12];
        if(level <= 52)
            return LEVEL_NAMES[13];
        if(level <= 69)
            return LEVEL_NAMES[14];
        return LEVEL_NAMES[15];
    }
    
    public static string GetLevelNameBy(int exp) {
        var lvlName = "";
        var _level = CalculateLevel(exp);
        if (exp >= 500000) lvlName = "Tổng Đốc";
        else if (exp >= 250000) lvlName = "Tuần Phủ";
        else if (exp >= 100000) lvlName = "Tri Phủ";
        else if (exp >= 50000) lvlName = "Tri Huyện";
        else lvlName = GetLevelName(_level);
        return lvlName;
    }
    
    private static string GetBigWinBigCuoc(string strBig){
        if(string.IsNullOrEmpty(strBig))
            return "";
        JArray bigs;
        try{
            bigs = JArray.Parse('[' + strBig + ']');
        }catch(Exception){
            return "...";
        }
        string ret = StringUtils.FormatMoney(Double.Parse(bigs[0].ToString()));
        ret += "/";
        for(int i = 1; i < bigs.Count; i++){
            //ret += bigs[i][0] + ' ván [';
            ret +=  "";
            JArray cuocs = bigs[i][1] as JArray;//các cước
            for(int j = 0; j < cuocs.Count; j++)
            {
                if (cuocs[j].ToString().Contains(","))
                {
                    
                    string[] cuoc = cuocs[j].ToString().Substring(1, cuocs[j].ToString().Length - 2).Split(',');
                    ret += cuoc[0].Trim() + ' ' + Utils.CUOC_NAMES[int.Parse(cuoc[1].Trim())];
                }
                else
                    ret += Utils.CUOC_NAMES[int.Parse(cuocs[j].ToString())];
                ret += ' ';
            }
            //xóa dấu ' ' ở cuối
            //ret = ret.substr(0, ret.length - 1) + '],';
            ret = ret.Substring(0, ret.Length - 1) + ',';
        }
        //xóa dấu ',' ở cuối
        return ret.Substring(0, ret.Length - 1);
    }
    
    public static string VipScoreToText(int vipScore, bool isMod){
        if (isMod) return StringUtils.FormatMoney(vipScore);
        if (vipScore >= 50000) return "50.000+";
        if (vipScore >= 20000) return "20.000+";
        if (vipScore >= 10000) return "10.000+";
        return StringUtils.FormatMoney(vipScore);
    }
    
    private string _trophy;
    public string trophy
    {
        get => _trophy;
        set
        {
            var ret = "";
            for(var i = 7; i >= 0; i--)
            {
                var count = value.Split(i.ToString()[0]).Length - 1;
                ret += count + ",";
                if (count > 0 && i > maxTrophy) maxTrophy = i;
            }
            ret = ret.Substring(0, ret.LastIndexOf(','));
            _trophy = ret;
        }
    }
    public int maxTrophy = -1;
    
    public static string TrophyName(int id)
    {
        switch (id)
        {
            case 0: return "Tú Tài";
            case 1: return "Cử Nhân";
            case 2: return "Phó Bảng";
            case 3: return "Tiến Sỹ";
            case 4: return "Hoàng Giáp";
            case 5: return "Thám Hoa";
            case 6: return "Bảng Nhãn";
            case 7: return "Trạng Nguyên";
            default: return "";
        }
    }
}

public class VipType
{
    public int id;
    public string vipName;
    public int score;
    public int debt;
    public bool transferable;
    private static VipType Nhat = new VipType(1, "Nhất Phẩm", 10000, 1000000000, true);
    private static VipType Nhi = new VipType(2, "Nhị Phẩm", 5000, 500000000, true);
    private static VipType Tam = new VipType(3, "Tam Phẩm", 4000, 400000000, true);
    private static VipType Tu = new VipType(4, "Tứ Phẩm", 3000, 300000000, true);
    private static VipType Ngu = new VipType(5, "Ngũ Phẩm", 2000, 200000000, true);
    private static VipType Luc = new VipType(6, "Lục Phẩm", 1500, 150000000, true);
    private static VipType That = new VipType(7, "Thất Phẩm", 1000, 100000000, true);
    private static VipType Bat = new VipType(8, "Bát Phẩm", 500, 50000000, true);
    private static VipType Cuu = new VipType(9, "Cửu Phẩm", 200, 0, true);
    public static VipType Dan = new VipType(0, "Thường Dân", 0, 0, false);
    private static List<VipType> VIP_TYPES = new List<VipType>() {Dan, Nhat, Nhi, Tam, Tu, Ngu, Luc, That, Bat, Cuu};
    public static List<VipType> LIST_VIP = new List<VipType>() {Dan, Cuu, Bat, That, Luc, Ngu, Tu, Tam, Nhi, Nhat};

    public VipType(int id, string _vipName, int score, int debt, bool transferable)
    {
        this.id = id;
        this.vipName = _vipName;
        this.score = score;
        this.debt = debt;
        this.transferable = transferable;
    }

    public static VipType FromScore(int score)
    {
        if (score >= Nhat.score) return Nhat;
        if (score >= Nhi.score) return Nhi;
        if (score >= Tam.score) return Tam;
        if (score >= Tu.score) return Tu;
        if (score >= Ngu.score) return Ngu;
        if (score >= Luc.score) return Luc;
        if (score >= That.score) return That;
        if (score >= Bat.score) return Bat;
        if (score >= Cuu.score) return Cuu;
        return Dan;
    }

    public static VipType fromId(int id)
    {
        return VIP_TYPES[id];
    }
    
}