using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;


public class WaitingUsersVO : ISFSObjVO
{
    public List<WaitingUserVO> users = new List<WaitingUserVO>();
    public List<int> leaveUids = new List<int>();
    public int count = 0;

    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        users = new List<WaitingUserVO>();
        if (o != null && o.ContainsKey("v"))
        {
            var a = o.GetSFSArray("v");
            var n = a.Size() / 3;
            for (var i = 0; i < n; i++)
            {
                var u = new WaitingUserVO();
                u.FromSFSArray(a, 3 * i);
                users.Add(u);
            }
        }

        if (o != null && o.ContainsKey("i"))
        {
            leaveUids = o.GetIntArray("i").ToList();
        }

        if (o != null && o.ContainsKey("u"))
        {
            count = o.GetInt("u");
        }
    }
}