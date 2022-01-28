using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;

public class GamesResultVO : ISFSObjVO
{
    /** uid list order by seat index */
    public List<string> names;

    /** result point in the last game */
    public List<int> finalPoint;

    /** coin changed of users in games */
    public List<List<int>> gameInfos;

    /** logId in games */
    public List<string> logIds;

    public List<string> winNames;

    public List<int> winUids;

    public List<int> allUids;

    /** 0: Hương, 1: Hội, 2: Đình vòng loại, 3: Đình tứ kết, 4: Đình bán kết, 5: Đình chung kết, 6:Đình vòng loại 1,7: Đình vòng loại 2,8: Đình vòng loại 3 */
    public int tourInfo = -1;

    public bool arenaInfo = false;
    
    public bool challengeInfo = false;


    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        if (o.ContainsKey("c"))
        {
            var c = o.GetSFSArray("c");
            names = c.GetUtfStringArray(0).ToList();
            finalPoint = c.GetIntArray(c.Size() - 1).ToList();
            gameInfos = new List<List<int>>();
            logIds = new List<string>();
            var i = 1;
            while (i < c.Size() - 2)
            {
                gameInfos.Add(c.GetIntArray(i).ToList());
                logIds.Add(c.GetUtfString(i + 1));
                i += 2;
            }
        }

        if (o.ContainsKey("u"))
        {
            winUids = o.GetIntArray("u").ToList();
        }

        if (o.ContainsKey("o"))
        {
            allUids = o.GetIntArray("o").ToList();
            winNames = new List<string>();
            foreach (var uid in winUids)
            {
                var idx = allUids.IndexOf(uid);
                winNames.Add(names[idx]);
            }
        }

        if (o.ContainsKey("f"))
        {
            tourInfo = o.GetInt("f");
        }

        if (o.ContainsKey("a"))
        {
            arenaInfo = o.GetBool("a");
        }
        
        if (o.ContainsKey("s"))
        {
            challengeInfo = o.GetBool("s");
        }
    }

    public bool IsNoOneWin => winNames != null && winNames.Count == 0;

    public bool IsEndOfTour => winNames != null;

    public bool IsArenaFinal => arenaInfo;

    public bool IsDinhFinal => tourInfo == 5;

    public bool IsHuongHoiTour => tourInfo == 0 || tourInfo == 1;

    public bool IsThiDinhVongLoai => tourInfo == 2 || tourInfo == 6;

    public string DinhNextTourName()
    {
        switch (tourInfo)
        {
            case 2:
                return "tứ kết";
            case 3:
                return "bán kết";
            case 4:
                return "chung kết";
            default:
                return "";
        }
    }

    public string TourWinTrophy(int uid)
    {
        switch (tourInfo)
        {
            case 0:
                switch (winUids.IndexOf(uid))
                {
                    // "Thi Hương"
                    case 0:
                        return "Cử Nhân";
                    case 1:
                        return "Tú Tài";
                }

                break;
            case 1:
                switch (winUids.IndexOf(uid))
                {
                    // "Thi Hội"
                    case 0:
                        return "Tiến Sỹ";
                    case 1:
                        return "Phó Bảng";
                }

                break;
            case 2:
            case 6:
                return "Thi Đình vòng loại 1";
            case 7:
                return "Thi Đình vòng loại 2";
            case 8:
                return "Thi Đình vòng loại 3";
            case 3:
                return "Thi Đình vòng tứ kết";
            case 4:
                return "Thi Đình vòng bán kết";
            case 5:
                switch (winUids.IndexOf(uid))
                {
                    case 0:
                        return "Trạng nguyên";
                    case 1:
                        return "Bảng Nhãn";
                    case 2:
                        return "Thám Hoa";
                    case 3:
                        return "Hoàng Giáp";
                }

                break;
        }

        return "";
    }

    public bool IsChallenge => challengeInfo;
}