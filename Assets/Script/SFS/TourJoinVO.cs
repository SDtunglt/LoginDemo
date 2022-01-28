using Sfs2X.Entities.Data;

public class TourJoinVO : BaseJoinVO
{
    public TourJoinVO(int zone)
    {
        this.zone = zone;
        tpe = TOUR_JOIN;
    }

    public override ISFSObject toSFSObject()
    {
        var o = new SFSObject();
        o.PutByte("z", (byte) zone);
        return o;
    }
}