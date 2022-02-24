using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;

public class UserListInBoardVO : ISFSObjVO
{
    /* ids of spectator user in board sort by PropAutoSpectatorId*/
    public List<int> userList;
    /** ids of registed play user */
    public List<int> reqUserList;

    /** ids of accept play user */
    public List<int> acceptedUserList;
    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        if (o.ContainsKey("l")) userList = o.GetIntArray("l").ToList();
        if (o.ContainsKey("r")) reqUserList = o.GetIntArray("r").ToList();
        if (o.ContainsKey("a")) acceptedUserList = o.GetIntArray("a").ToList();
    }
    
    public void removeUser(User u) {
        var uidStr = u.Name;
        if (uidStr == null) return;
        var uid = int.Parse(u.Name);
        if (acceptedUserList != null) {
            var idx = acceptedUserList.IndexOf(uid);
            if (idx >= 0) acceptedUserList.Splice(idx, 1);
        }
        if (reqUserList != null) {
            var idx1 = reqUserList.IndexOf(uid);
            if (idx1 >= 0) reqUserList.Splice(idx1, 1);
        }
    }

    public void addAcceptUser(int uid) {
        if (reqUserList != null) {
            var idx = reqUserList.IndexOf(uid);
            if (idx >= 0) reqUserList.Splice(idx, 1);
        }
        if (acceptedUserList == null) acceptedUserList = new List<int> {uid};
        if (acceptedUserList.IndexOf(uid) < 0) {
            acceptedUserList.Add(uid);
        }
    }
    public void addReqUser(int uid)
    {
        if (reqUserList == null) reqUserList = new List<int> {uid};
        if (reqUserList.IndexOf(uid) < 0) {
            reqUserList.Add(uid);
        }
    }
}