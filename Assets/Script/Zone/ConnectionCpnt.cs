using System.Collections.Generic;
using Sfs2X.Core;
using Sfs2X.Entities;
using UnityEngine;

public class ConnectionCpnt : MonoBehaviour
{
    protected Dictionary<string,EventListenerDelegate> mapListener = new Dictionary<string, EventListenerDelegate>();
    protected Room currentRoom;

    protected virtual void OnEnable()
    {
        if(!SmartFoxConnection.IsConnected) return;
        InitListeners();
    }

    public void InitListeners()
    {
        foreach (var item in mapListener)
        {
            SmartFoxConnection.Instance.AddEventListener(item.Key,item.Value);
        }
    }

    protected virtual void OnDisable()
    {
        if(!SmartFoxConnection.IsConnected) return;
        foreach(var item in mapListener)
        {
            SmartFoxConnection.Instance.RemoveEventListener(item.Key,item.Value);
        }
    }
}
