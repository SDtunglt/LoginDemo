using Newtonsoft.Json;

public class AgentData
{
    public int app;
    public string os;
    public string version;
    public string deviceId;
    public string npt;
    public string packageName;

    public static AgentData Create()
    {
        var obj = new AgentData();
        obj.app = GameConfig.APP_ID;
        obj.os = GameUtils.GetPlatform();
        obj.version = GameUtils.GetVersion();
        obj.deviceId = GameUtils.GetDeviceId();
        obj.packageName = GameUtils.GetBundleId();
        obj.npt = NPTConfig.GetNPT(obj.packageName);
        return obj;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
public class LoginDataRequest : AgentData
{
    public string username;
    public string pass;
    public AgentData agent;

    public static LoginDataRequest Create(string u, string p)
    {
        var agentData = AgentData.Create();
        // agentData.app = 4;
        agentData.app = 6;
        return new LoginDataRequest
        {
            username = u,
            pass = p,
            agent = agentData
        };
    }

}
