using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class UserModel : Singleton<UserModel>
{
    public UserGameVO gVO = new UserGameVO();

    public string name;
    public string gender;
    public string uid;
    public double ip;
    public List<int> unlockBorders = new List<int>();
    public int currentSelectBorder = 0;

    public int debt;
    public int roundPlay;
    //public List<VuongGiaData> vuongGiaData;
    public bool firstCharge = false;
    public string kmValue = "";
    public string giftCode = "";
    public static int[] joinGame = new[] {-1, -1, -1, -1};
    public static bool isVuongVip = false;
    public static int vuongBlockEnd = 0;
    public static bool invitableOn = true;
    public List<UserThiDinhVO> dsBaoDanh = new List<UserThiDinhVO>();
    public List<UserThiDinhVO> bxhThiDinh = new List<UserThiDinhVO>();

    public static string GetGender(string _gender)
    {
        switch(_gender)
        {
            case "M":
                return "Nam";
            case "F":
                return "Nữ";
        }

        return "Không";
    }

    public void InitDataVG(JObject data)
    {
        
    }
}
