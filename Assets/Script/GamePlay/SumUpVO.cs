using System;
using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;

public class SumUpVO : ISFSObjVO
{
    public static int ALL_BAO_DIS = 0;
    public static int DRAW = 1;
    public static int U_LAO = 2;
    public static int XUONG_THIEU_DIEM = 4;
    public static int U_THIEU_DIEM = 3;
    public static int U_BAO = 5;
    public static int BO_U = 6;
    public static int XUONG_SAI = 7;
    public static int TREO_TRANH = 8;
    public static int XUONG_DUNG = 9;
    public int type;
    public int idxU = -1;
    public List<bool> baos;
    public List<int> cuocHos;
    public List<int> incorrectCuocIndexes = new List<int>();
    public List<int> missingCuocIndexes = new List<int>();
    public PlayErrVO err;
    public int boUCard = -2;
    public int score = -1;
    public List<long> changedCoins;
    public long anGa;
    public bool autoXuong = false;
    public List<string> disNames;
    public string logId;
    public SumUpVO(int type = -1)
    {
        this.type = type;
    }

    public bool isDraw()
    {
        return type == DRAW || type == ALL_BAO_DIS;
    }

    public bool isXuong()
    {
        return !isDraw() && type != U_LAO && !autoXuong;
    }

    public static string getSumUpMsg(SumUpVO vo, GamePlayModel gamePlayModel)
    {
        if (vo.type == DRAW)
            return AppMsg.GAMEDRAW;
        if (vo.type == ALL_BAO_DIS)
            return AppMsg.ALLBAOORDIS;
        var ps = gamePlayModel.sdplayers;
        var msg = "<b>" + ps[vo.idxU].name + "</b>";
        if (vo.autoXuong)
        {
            msg += " thoát trước khi xướng";
            if (vo.type == U_THIEU_DIEM)
            {
                msg += AppMsg.PHAT8DO2LEO + gamePlayModel.KhongChoiU();
            }
            else if (vo.type == TREO_TRANH)
            {
                msg += ". Nghỉ ăn tiền vì " + vo.err.msg; //Note TREO_TRANH thì vo.err phải != null
            }
            else if (vo.type == XUONG_DUNG)
            {
                msg += ", ù đúng";
                if (gamePlayModel.minScoreU > 2) msg += ", đủ điểm";
                msg += ":\n +" + vo.score + " điểm";
            }
            else if (vo.type == BO_U)
            {
                if (vo.boUCard == -1)
                    msg += AppMsg.PHAT8DO2LEO + "bỏ thiên ù";
                else
                    msg += AppMsg.PHAT8DO2LEO + "bỏ ù " + SDCard.cardName(vo.boUCard);
            }
            else if (vo.type == U_BAO)
                msg += AppMsg.PHAT8DO2LEO + vo.err.msg;
        }
        else if (vo.type == U_LAO)
        {
            msg += SDMsg.Join(AppMsg.PHATULAO, vo.score);
        }
        else
        {
            msg += " xướng:\n" + "<b>" + ULogic.getCuocsStr(vo.cuocHos) + "</b>";
            if (vo.type == U_THIEU_DIEM)
            {
                msg += AppMsg.PHAT8DO2LEO + gamePlayModel.KhongChoiU() + "\n" + getCuocSaiMsg(vo);
            }
            else if (vo.type == XUONG_THIEU_DIEM)
            {
                msg += " = " + vo.score + " điểm" + AppMsg.PHAT8DO2LEO + gamePlayModel.KhongChoiU();
            }
            else if (vo.type == TREO_TRANH)
            {
                msg += ". Nghỉ ăn tiền vì " + vo.err.msg; //Note TREO_TRANH thì vo.err phải != null
            }
            else if (vo.type == XUONG_DUNG)
            {
                msg += " +" + vo.score + " điểm";
                // Ăn gà nuôi chuyển ra SumUpMediator để thay đổi unit theo Bảo hoặc Điểm
            }
            else if (vo.type == BO_U)
            {
                if (vo.boUCard == -1)
                    msg += AppMsg.PHAT8DO2LEO + "bỏ thiên ù";
                else
                    msg += AppMsg.PHAT8DO2LEO + "bỏ ù " + SDCard.cardName(vo.boUCard);
            }
            else if (vo.type == U_BAO)
                msg += AppMsg.PHAT8DO2LEO + vo.err.msg;
            else //XUONG_SAI
                msg += " -" + vo.score + " điểm\n" + getCuocSaiMsg(vo);
        }

        var baos = "";
        for (var i = 0; i < vo.baos.Count; i++)
            if (i != vo.idxU && vo.baos[i])
                baos += ps[i].name + " báo, ";
        if (baos != "")
        {
            baos = baos.Substring(0, baos.Length - 2);
            msg += "\n" + baos;
        }

        return msg;
    }

    public static string getCuocSaiMsg(SumUpVO vo)
    {
        var msg = "";
        int ci; //index của cước sai/thiếu, ex 16 là lèo
        if (vo.incorrectCuocIndexes.Count > 0)
        {
            msg += "Cước sai: ";
            for (var i = 0; i < vo.incorrectCuocIndexes.Count; i++)
            {
                ci = vo.incorrectCuocIndexes[i];
                var cn = vo.cuocHos[ci]; //số cước ci thằng này hô, ex 3 là 3 lèo/tôm/..
                //cn phải lớn hơn 0!
                if (cn == 1)
                    msg += ULogic.CUOC_NAMES[ci];
                else
                    msg += cn + " " + ULogic.CUOC_NAMES[ci];
                msg += ", ";
            }
        }
        else
        {
            //Chỉ show cước thiếu khi không có cước sai
            //=> thực ra không cần gửi cước thiếu lên server & cho làng khi đã có cước sai
            msg += "Cước thiếu: ";
            for (var i = 0; i < vo.missingCuocIndexes.Count; i++)
            {
                ci = vo.missingCuocIndexes[i];
                if (ci == ULogic.HOA || ci == ULogic.NHA || ci == ULogic.CA || ci == ULogic.NGU)
                    ci = ULogic.BACH_THU;
                msg += ULogic.CUOC_NAMES[ci] + ", ";
            }
        }

        return msg.Substring(0, msg.Length - 2);
    }

    public static SumUpVO fromData(int version, string data)
    {
        var arr = data.Split(ReplayModel.DELIMITER_LEVEL_1);
        var vo = new SumUpVO();
        var i = 0;
        var temp = "";
        vo.type = int.Parse(arr[i++]);
        vo.idxU = int.Parse(arr[i++]);
        temp = arr[i++];
        if (isDefined(temp)) vo.baos = toBoolArray(temp.Split(ReplayModel.DELIMITER_ARRAY).ToList());
        temp = arr[i++];
        if (isDefined(temp))
        {
            vo.cuocHos = toCuocs(temp);
        }

        temp = arr[i++];
        if (isDefined(temp)) vo.err = PlayErrVO.fromData(version, temp);
        vo.boUCard = int.Parse(arr[i++]);
        temp = arr[i++];
        if (isDefined(temp)) vo.incorrectCuocIndexes = toIntArray(temp.Split(ReplayModel.DELIMITER_ARRAY).ToList());
        temp = arr[i++];
        if (isDefined(temp)) vo.missingCuocIndexes = toIntArray(temp.Split(ReplayModel.DELIMITER_ARRAY).ToList());
        vo.score = int.Parse(arr[i++]);
        temp = arr[i++];
        if (isDefined(temp))
            vo.changedCoins = temp.Split(ReplayModel.DELIMITER_ARRAY).Select(s => long.Parse(s)).ToList();
        var x = arr[i++];
        vo.anGa = long.Parse(string.IsNullOrEmpty(x) ? "0" : x);
        if (arr.Length > i)
        {
            vo.autoXuong = (arr[i] == "1");
        }
        else
        {
            vo.autoXuong = true;
        }
        return vo;
    }
    
    private static bool isDefined(string s)
    {
        return !string.IsNullOrEmpty(s);
    }

    private static List<int> toCuocs(string s)
    {
        var arr = new List<int>();
        arr.SetCount(23, 0);
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c != '(') arr[ReplayModel.b36(c.ToString())] = 1;
            else
            {
                c = s[i + 2];
                arr[ReplayModel.b36(c.ToString())] = int.Parse(s[i + 1].ToString());
                i += 3;
            }
        }

        return arr;
    }

    private static List<bool> toBoolArray(List<string> arr)
    {
        return arr.Select(s => s == "1").ToList();
    }

    private static List<int> toIntArray(List<string> arr)
    {
        return arr.Select(int.Parse).ToList();
    }

    public ISFSObject toSFSObject()
    {
        var o = new SFSObject();
        o.PutByte("t", (byte) type);
        //if isDraw then not send to server
        o.PutByte("u", (byte) idxU);
        if (score != -1)
            o.PutShort("r", (short) score); // trong trường hợp auto xướng, cần score ù để xem có ù thiếu điểm không
        if (boUCard != -2) o.PutByte("c", (byte) boUCard);
        if (err != null) o.PutByteArray("e", err.toByteArray());
        if (autoXuong) o.PutBool("ax", true);
        if (cuocHos != null)
        {
            o.PutByteArray("x", GameUtils.intArrToByteArray(cuocHos));
            if (type == U_BAO || type == BO_U || type == XUONG_SAI)
            {
                if (incorrectCuocIndexes.Count > 0)
                {
                    var numCuocsSai = 0;
                    foreach (var i in incorrectCuocIndexes)
                    {
                        if (i != ULogic.THONG && i != ULogic.DIA_U)
                            numCuocsSai++;
                    }

                    if (numCuocsSai > 1)
                        o.PutByte("n", (byte) numCuocsSai);
                }
            }

            //U_BAO & BO_U không cần send incorrectCuocIndexes lên server
            if (type == U_THIEU_DIEM)
                o.PutByteArray("i", GameUtils.intArrToByteArray(incorrectCuocIndexes));
            else if (type == XUONG_SAI)
            {
                if (incorrectCuocIndexes.Count > 0)
                    o.PutByteArray("i", GameUtils.intArrToByteArray(incorrectCuocIndexes));
                if (missingCuocIndexes.Count > 0)
                    o.PutByteArray("m", GameUtils.intArrToByteArray(missingCuocIndexes));
            }
        }

        
        return o;
    }

    public void fromSFSObject(ISFSObject o)
    {

    }
}
