using Sfs2X.Entities.Data;

public class WaitingUserVO : ISFSObjVO
{
    public int id;
    public string name;
    public double coin;

    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        if (o.ContainsKey("i"))
            id = o.GetInt("i"); //when leave
        else
            FromSFSArray(o.GetSFSArray("a"), 0);
    }

    public void FromSFSArray(ISFSArray a, int i)
    {
        id = a.GetInt(i);
        name = a.GetUtfString(i + 1);
        coin = a.GetLong(i + 2);
    }
}