using Sfs2X.Entities.Data;

public class VuongConfigVO
{
    public string name;
    public int numBoard;
    public int minStake;
    public int maxStake;
    public int coinStakeJoin;
    public int vipToJoin;
    public bool isOpen;
    
    public VuongConfigVO(ISFSObject a)
    {
        name = a.GetUtfString("n");
        numBoard = a.GetByte("nb");
        minStake = a.GetInt("s");
        maxStake = a.GetInt("ms");
        vipToJoin = a.GetInt("v");
        coinStakeJoin = a.GetInt("c");
        isOpen = a.GetBool("o");
    }
}