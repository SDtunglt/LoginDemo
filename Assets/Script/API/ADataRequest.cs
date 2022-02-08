using Newtonsoft.Json;

public abstract class ADataRequest
{
   public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
