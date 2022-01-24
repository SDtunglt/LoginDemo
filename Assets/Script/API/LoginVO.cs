using Sfs2X.Entities.Data;

public class LoginVO : ISFSObjVO
{
    private int version;
    private string session;
    private string deviceType;
    private string versionApp;
    public LoginVO(string platform, string verApp, int verGame, string sessionId = "") : base()
    {
        version = verGame;
        session = sessionId;
        deviceType = platform;
        versionApp = verApp;
    }
    public ISFSObject toSFSObject()
    {
        var ret = new SFSObject();
        ret.PutInt("v", version);
        ret.PutUtfString("dt", deviceType);
        ret.PutUtfString("va", versionApp);
        if(!string.IsNullOrEmpty(session)) ret.PutUtfString("s", session);
        return ret;
    }
    public void fromSFSObject(ISFSObject o)
    {
    }
}
