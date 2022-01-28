using Sfs2X.Entities.Data;

public class QuickJoinVO : BaseJoinVO
{
    public int min;
    public int max;
    public int hasGa = -1;
    public int minU = -1;

    public QuickJoinVO(int _zone, int _min = 0, int _max = int.MaxValue)
    {
        zone = _zone;
        min = _min;
        max = _max;
        tpe = QUICK_JOIN;
    }

    public override ISFSObject toSFSObject()
    {
        var o = base.toSFSObject();
        o.PutInt(GameConfig.VAR_MIN_STAKE,min);
        o.PutInt(GameConfig.VAR_MAX_STAKE,max);
        if(hasGa != -1) o.PutBool(GameConfig.VAR_GA_NUOI, hasGa == 1);
        if(minU != -1) o.PutByte(GameConfig.VAR_MIN_U, (byte) minU);
        return o;
    }
}
