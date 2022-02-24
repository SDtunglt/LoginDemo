using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class ChiaBaiVO : ISFSObjVO
{
    public List<int> cards;
    public int calcScoreMode = 0;
    public int tourLevel = -1;
    public ISFSObject toSFSObject()
    {
        var o = new SFSObject();
        o.PutByte("m", (byte) calcScoreMode);
        return o;
    }

    public void fromSFSObject(ISFSObject o)
    {
        if (cards != null && cards.Count > 0) cards = new List<int>();
        cards = GameUtils.byteArrayToIntArr(o.GetByteArray("p"));
        if (o.ContainsKey("t")){
            tourLevel = o.GetByte("t");
        }
    }
    
    public static ChiaBaiVO FromData(int version, string data) {
        var vo = new ChiaBaiVO {cards = new List<int>()};
        foreach (var t in data)
        {
            vo.cards.Add(ReplayModel.b36(t.ToString()));
        }
        return vo;
    }
}