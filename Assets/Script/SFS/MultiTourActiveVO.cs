using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class MultiTourActiveVO : ISFSObjVO
{
    public List<TourVO> activeTours;

    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        if (o == null) return;
        if (!o.ContainsKey("v")) return;
        var a = o.GetSFSArray("v");
        for (var i = 0; i < a.Size(); i++)
        {
            var u = new TourVO();
            u.fromSFSObject(a.GetSFSObject(i));
            if (activeTours == null) activeTours = new List<TourVO>();
            activeTours.Add(u);
        }
    }
}