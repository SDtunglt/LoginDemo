using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;

public class ScoreVO
{
    public List<int> uids;
    public List<int> scores;

    public ScoreVO(ISFSObject o)
    {
        scores = o.GetIntArray("s").ToList();
        uids = o.GetIntArray("u").ToList();
    }

    public int gaScore()
    {
        var sumScore = 0;
        for(var i = 0; i < scores.Count;i++) sumScore += scores[i];
        return -sumScore;
    }
}