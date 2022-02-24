using Sfs2X.Entities.Data;

public class BocCaiVO : ISFSObjVO
{
    public int baiCaiIdx;
    
    public int scoreVaoGa;

    public BocCaiVO(){}
    public BocCaiVO(int nocIdx)
    {
        baiCaiIdx = nocIdx;
    }

    public void fromSFSObject(ISFSObject o)
    {
        baiCaiIdx = o.GetByte("p");
        if(o.ContainsKey("s"))
            scoreVaoGa = o.GetByte("s");
    }

    public ISFSObject toSFSObject()
    {
        ISFSObject o = new SFSObject();
        o.PutByte("p", (byte) baiCaiIdx);
        return o;
    }
    
    public static BocCaiVO FromData(int version, string data) {
        var vo = new BocCaiVO
        {
            scoreVaoGa = string.IsNullOrEmpty(data) ? 0 : int.Parse(data)
        };
        return vo;
    }
}
