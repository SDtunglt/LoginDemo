using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;

public class VaoGaVO : ISFSObjVO
{
    public List<string> ids = new List<string>();

    public int score;

    public ISFSObject toSFSObject()
    {
        var o = new SFSObject();
        o.PutIntArray("d", ids.Select(int.Parse).ToArray());
        o.PutByte("s",(byte) score);
        return o;
    }

    public void fromSFSObject(ISFSObject o)
    {
        var uids = o.GetIntArray("d");
        foreach(var id in uids)
        {
            ids.Add(id.ToString());
        }
        score = o.GetByte("s");
    }

    public static VaoGaVO fromData(int version, string data)
    {
        var arr = data.Split('@');
        var vo = new VaoGaVO();
        var uids = arr[0].Split(',');
        foreach (var id in uids)
        {
            vo.ids.Add(id);
        }

        vo.score = int.Parse(arr[1]);
        return vo;
    }
}