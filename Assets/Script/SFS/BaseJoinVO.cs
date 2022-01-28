using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sfs2X.Entities.Data;

public class BaseJoinVO : ISFSObjVO
{
    public static int CHECK_JOIN = 0;
    public static int NORMAL_JOIN = 1;
    public static int QUICK_JOIN = 2;
    public static int TOUR_JOIN = 3;
    public static int RESUME_JOIN = 4;
    public static int ARENA_JOIN = 5;
    public static int CHALLENGE_CREATE_JOIN = 9;
    public static int CHALLENGE_REQ_JOIN = 10;
        
    public int zone = -1;
    public int tpe = -1;
    public bool resume = false;

    public bool IsInEntrance()
    {
        return zone == -1;
    }
    
    public virtual ISFSObject toSFSObject(){
        var o = new SFSObject();
        o.PutByte("z", (byte) zone);
        return o;
    }

    public void fromSFSObject(ISFSObject o){}
}
