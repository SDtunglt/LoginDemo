using Sfs2X.Entities.Data;

public class PlayVO: ISFSObjVO
{
    public static int DRAW = 0;
    public static int EAT = 1;
    public static int CHIU = 2;
    public static int DUOI = 3;
    public static int DANH = 4;
    public static int TRACUA = 5;

    public int type;
    public int uIdx;
    public int card;
    public int door;

    public SDCard cardInst;
    public VaoGaVO vaoGa;

    public int actionIndex;
    public PlayVO(int type = -1, int uIdx = -1, int card = -1){
        this.type = type;
        this.uIdx = uIdx;
        this.card = card;
    }

    public ISFSObject toSFSObject()
    {
        var o = new SFSObject();
        o.PutByte("t", (byte) type);
        o.PutByte("u", (byte) uIdx);
        o.PutByte("c", (byte) card);
        o.PutByte("i", (byte) actionIndex);
        if(vaoGa != null)
            o.PutSFSObject("g", vaoGa.toSFSObject());
        return o;
    }

    public void fromSFSObject(ISFSObject o)
    {
        type = o.GetByte("t");
        uIdx = o.GetByte("u");
        card = o.GetByte("c");
        actionIndex = o.GetByte("i");
        if(!o.ContainsKey("g")) return;
        vaoGa = new VaoGaVO();
        vaoGa.fromSFSObject(o.GetSFSObject("g"));
    }

    public static PlayVO fromData(int version,string data)
    {
        PlayVO vo;
        var act = ReplayModel.b36(data[0].ToString());
        var card = ReplayModel.b36(data[1].ToString());
        vo = new PlayVO(act % 10, act / 10, card);
        if(data.Length > 2) vo.vaoGa = VaoGaVO.fromData(version,data.Substring(2));
        return vo;
    }
}