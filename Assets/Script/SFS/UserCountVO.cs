using Sfs2X.Entities.Data;

public class UserCountVO : ISFSObjVO
{
    public string uc;
    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        uc = o.GetUtfString("c");
    }
}