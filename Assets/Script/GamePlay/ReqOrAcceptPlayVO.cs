using Sfs2X.Entities.Data;

public class ReqOrAcceptPlayVO : ISFSObjVO
{
    /** ids of reg play user */
    public int uid;
    public ISFSObject toSFSObject()
    {
        var o = new SFSObject();
        o.PutInt("u", uid);
        return o;
    }

    public void fromSFSObject(ISFSObject o)
    {
        uid = o.GetInt("u");
    }
}