using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class UserDetailVO
{
    public UserContainerVO cVO;
    public UserGameVO gVO;

    public UserDetailVO(JObject o, string uid)
    {
        cVO = new UserContainerVO(uid, o["n"].ToString(), o["g"].ToString());
        gVO = new UserGameVO
        {
            coin = long.Parse(o["b"].ToString()),
            exp = int.Parse(o["e"].ToString()),
            //debt = int.Parse(o["d"].ToString()),
            win = int.Parse(o["w"].ToString()),
            total = int.Parse(o["t"].ToString())
        };
        gVO.UpdateExtraInfo(o["x"]);
    }
}