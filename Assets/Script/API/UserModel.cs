using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class UserModel : Singleton<UserModel>
{
    public UserGameVO gVO = new UserGameVO();
    public string uid;
    public double ip;
    public string name;
    public string gender;
    public int debt;
    public int roundPlay;
    public int currentSelectBorder = 0;
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
}
