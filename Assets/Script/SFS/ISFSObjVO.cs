using Sfs2X.Entities.Data;

public interface ISFSObjVO
{
    ISFSObject toSFSObject();
    void fromSFSObject(ISFSObject o); 
}
