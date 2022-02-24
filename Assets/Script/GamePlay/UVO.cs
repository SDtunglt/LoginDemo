using System;
using Sfs2X.Entities.Data;
using UnityEngine;

public class UVO : ISFSObjVO
{
    public static UVO DRAW = new UVO(-2);
    public static UVO ALL_DIS_BAO = new UVO(-3);
    public static UVO DIS_WHEN_GIVE = new UVO(-4);

    public bool isDraw => uIdx == -2;
    public bool isAllDisBao => uIdx == -3;
    public bool isDisWhenGive => uIdx == -4;

    public static UVO CreatThienUVO(int uIdx)
    {
        return new UVO(uIdx, -1);
    }

    public bool isThienU => aIdx == -1;
    public int uIdx;
    public int aIdx;
    public bool chiuU;
    
    public UVO(int uIdx = -2, int aIdx = -1)
    {
        this.uIdx = uIdx;
        this.aIdx = aIdx;
    }

    public static UVO fromData(int version, string data)
    {
        return new UVO(int.Parse(data[0].ToString()),int.Parse(data.Substring(1)));
    }

    public ISFSObject toSFSObject()
    {
        var o = new SFSObject();
        o.PutByte("u",(byte) uIdx);
        o.PutInt("a", aIdx);
        if(chiuU) o.PutBool("c", chiuU);
        return o;
    }

    public void fromSFSObject(ISFSObject o)
    {
        uIdx = o.GetByte("u");
        try
        {
            aIdx = o.GetByte("a");
        }
        catch(Exception)
        {
            aIdx = o.GetInt("a");
        }
        if(o.ContainsKey("c")) chiuU = true;
    }
}