using Newtonsoft.Json.Linq;
using Sfs2X.Entities.Data;

public class ReplayVO : ISFSObjVO
{
    public string u;
    public int t;
    public string d;


    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        u = o.GetUtfString("users");
        t = o.GetInt("logTime");
        d = o.GetUtfString("data");
    }
    
    public void fromSFSObject(JObject o)
    {
        u = o.GetValue("users").ToString();
        t = int.Parse(o.GetValue("logTime").ToString());
        d = o.GetValue("data").ToString();
    }
}