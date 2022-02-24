using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class ResumeVO : ISFSObjVO
{
    public ChiaBaiVO chiaBaiVO;
    public BocCaiVO bocCaiVO;
    public List<PlayVO> acts;
    public List<UVO> uvos;

    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        var v = o.GetSFSArray("v");
        chiaBaiVO = new ChiaBaiVO();
        chiaBaiVO.fromSFSObject(v.GetSFSObject(0));
        if (v.Size() == 1) //Đang chia bài
            return;
        bocCaiVO = new BocCaiVO();
        bocCaiVO.fromSFSObject(v.GetSFSObject(1));
        if (v.Size() == 2) //chưa chơi
            return;
        acts = new List<PlayVO>();
        var i = 2;
        for (; i < v.Size(); i++)
        {
            o = v.GetSFSObject(i);
            //o có thể là PlayVO hoặc UVO
            if (!o.ContainsKey("t"))
                break;
            var act = new PlayVO();
            act.fromSFSObject(o);
            acts.Add(act);
        }

        if (i == v.Size())
            return;
        uvos = new List<UVO>();
        for (; i < v.Size(); i++)
        {
            o = v.GetSFSObject(i);
            var uvo = new UVO();
            uvo.fromSFSObject(o);
            uvos.Add(uvo);
        }
    }
}