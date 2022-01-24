using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sfs2X.Util;
using UnityEngine;

public class SmartFoxConnection : MonoBehaviour
{
    private static SmartFoxConnection _Instance;
    private SmartFox sfs;
    private Coroutine connectionCoroutine;

    private const float TIME_CONNECT = 5.0f;
    private GameModel gameModel = GameModel.Instance;
    private UserModel userModel = UserModel.Instance;

    public static bool IsConnected
    {
        get
        {
            if(_Instance != null && _Instance.sfs != null)
            {
                return _Instance.sfs.IsConnected;
            }
            return false;
        }
    }

    public User mySelf => sfs?.MySelf;

    public static SmartFoxConnection Instance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
                DontDestroyOnLoad(_Instance);
            }
            return _Instance;
        }
    }


    private void Update()
    {
        sfs?.ProcessEvents();
    }

    public void Disconnect()
    {
        if(sfs == null) return;
        if(sfs.IsConnected) sfs.Disconnect();
        sfs = null;
    }

    private void OnApplicationQuit()
    {
        if(_Instance != null) Destroy(_Instance.gameObject);
    }

    private void OnDestroy()
    {
        if(sfs != null)
        {
            sfs.Disconnect();
            sfs.RemoveAllEventListeners();
        }   
    }

    public void Connect()
    {
        ConfigData cfg = new ConfigData();
        cfg.Zone = GameConfig.ZONE_DEFAULT;
        cfg.Debug = false;
        cfg.Host = GameConfig.HOST;

        var global = FindObjectOfType<GlobalDataManager>();
        cfg.Port = global.isDev && global.isChanRET ? global.PortChanRet : GameConfig.PORT;

        InitSmartFox();
        var platform = "unity-" + GameUtils.GetPlatform();
        sfs.Connect(cfg);
        StartConnectionTimeout(TIME_CONNECT);
    }

    private void InitSmartFox()
    {
        sfs = new SmartFox();

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_RETRY,OnConnectionRetry);
        sfs.AddEventListener(SFSEvent.CONNECTION_RESUME,OnConnectionRetry);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

        sfs.AddEventListener(SFSEvent.LOGIN,OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR,OnLoginError);
    }

    public void AddEventListener(string e, EventListenerDelegate listener)
    {
        sfs.AddEventListener(e, listener);
    }

    public void RemoveEventListener(string e, EventListenerDelegate listener)
    {
        sfs.RemoveEventListener(e, listener);
    }

    public void Send(IRequest obj)
    {
        sfs.Send(obj);
    }

    private void OnConnection(BaseEvent evt)
    {
        StopConnectionTimeout();
        if((bool) evt.Params["success"])
        {
            if(userModel == null)
            {
                return;
            }
            var packet = new LoginVO(GameUtils.GetPlatform(),GameUtils.GetVersion(),GameConfig.VERSION,gameModel.loginSession).toSFSObject();
            sfs.Send(new LoginRequest(userModel.uid,"",GameConfig.ZONE_DEFAULT,packet));
        }
        else
        {
            Debug.Log("Kết nối thất bại");
        }
    }

    private void OnConnectionRetry(BaseEvent evt)
    {

    }

    private void OnConnectionLost(BaseEvent evt)
    {
        var reason = (string) evt.Params["reason"];
        Debug.Log($"Connection was lost, reason is: {reason}");
    }

    private void OnLogin(BaseEvent evt)
    {
        Signals.Get<LoginSuccessSignal>().Dispatch();
    }

    private void OnLoginError(BaseEvent evt)
    {
        if (sfs.IsConnected) sfs.Disconnect();
    }

    private void StopConnectionTimeout()
    {
        if (connectionCoroutine != null)
        {
            StopCoroutine(connectionCoroutine);
        }
    }
    private void StartConnectionTimeout(float dt)
    {
        StopConnectionTimeout();
        connectionCoroutine = StartCoroutine(ConnectionTimeout(dt));
    }

    private IEnumerator ConnectionTimeout(float dt)
    {
        yield return new WaitForSecondsRealtime(dt);
        SDLogger.LogError("Disconnect");
        Signals.Get<LostConnectionSignal>().Dispatch();
    }
}
