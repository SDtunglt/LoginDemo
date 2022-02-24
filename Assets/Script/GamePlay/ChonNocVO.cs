using Sfs2X.Entities.Data;

public class ChonNocVO : ISFSObjVO
{
    public int nocIdx;

    public ChonNocVO(){}
    public ChonNocVO(int nocIdx)
    {
        this.nocIdx = nocIdx;
    }

    public void fromSFSObject(ISFSObject o)
    {
        nocIdx = o.GetByte("p");
    }

    public ISFSObject toSFSObject()
    {
        ISFSObject o = new SFSObject();
        o.PutByte("p", (byte) nocIdx);
        return o;
    }
}