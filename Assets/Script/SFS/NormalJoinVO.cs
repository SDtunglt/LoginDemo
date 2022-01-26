using Sfs2X.Entities.Data;
using Sfs2X.Util;

public class NormalJoinVO : BaseJoinVO
{
    public int room;
    public int board;
    private string pw;
    private bool isInvited;

    public bool isInBoard()
    {
        return board != -1;
    }

    public bool IsInRoom()
    {
        return zone >= 0 && board == -1;
    }

    public NormalJoinVO(int z, int r = -1,int b = -1, string pw = null, bool isInvited = false)
    {
        zone = z;
        room = r;
        board = b;
        this.pw = pw;
        this.isInvited = isInvited;
        tpe = NORMAL_JOIN;
    }

    public override ISFSObject toSFSObject()
    {
        var arr = new ByteArray();
        arr.WriteByte((byte) zone);
        arr.WriteByte((byte) room);
        arr.WriteByte((byte) board);
        var ret = new SFSObject();
        ret.PutByteArray("v", arr);
        if(isInvited)
            ret.PutBool("i", true);
        else if(pw != null && pw != "")
            ret.PutUtfString("w", pw);
        return ret;
    }

    public bool equals(NormalJoinVO o)
    {
        return o != null && zone == o.zone && room == o.room && board == o.board;
    }
}
