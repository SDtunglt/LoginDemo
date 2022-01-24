using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPTConfig
{
    private static Dictionary<string, string> _nptMap = null;

    public static string GetNPT(string packageName)
	{
        if (_nptMap == null)
		{
            InitMap();
		}


        if (_nptMap.ContainsKey(packageName))
        {
            return _nptMap[packageName];
        }

        return null;
        
	}

    private static void InitMap()
    {
        _nptMap = new Dictionary<string, string>();

        _nptMap.Add("com.vietsao.chandangian", "vietsao");
        _nptMap.Add("com.sandinh.chan", "sandinh");
        _nptMap.Add("com.dangian.chan5g", "chan5g");
        _nptMap.Add("com.sds.chan5g", "chan5g");
        _nptMap.Add("com.haohao.chan5", "haohao");

    }
}
