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
using DG.Tweening;
using UnityEngine.Purchasing;

public class SmartFoxConnection : MonoBehaviour
{
    private static SmartFoxConnection _Instance;
    private SmartFox sfs;
    private Coroutine connectionCoroutine;
    public bool isConnected => sfs != null && sfs.IsConnected;
    public string currentIp => sfs?.CurrentIp;
    public Room lastJoinedRoom => sfs?.LastJoinedRoom;
    private long oldMyCoin;

    private const float TIME_CONNECT = 5.0f;
    private GameModel gameModel = GameModel.Instance;
    private UserModel userModel = UserModel.Instance;
    private ScreenManager screenManager;

    private Tween renewSessionTween;

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

    private void Awake()
    {
        screenManager = ScreenManager.Instance;
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
        renewSessionTween?.Kill();
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

    public void SendExt(string cmd, ISFSObjVO vo = null)
    {
        sfs.Send(new ExtensionRequest(cmd, vo == null ? new SFSObject() : vo.toSFSObject()));
    }

    public void SendExt(string cmd, SFSObject vo)
    {
        sfs.Send(new ExtensionRequest(cmd,vo ?? new SFSObject()));
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

    private void UnSubcribeGroup(string groupId)
    {
        GameConfig.arrayGroupSubscribe.Remove(groupId);
        sfs.Send(new UnsubscribeRoomGroupRequest(groupId));
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

        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitedRoom);
        sfs.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, RoomGroupSubcribed);
        sfs.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR,RoomGroupSubcribedError);
        sfs.AddEventListener(SFSEvent.ROOM_GROUP_UNSUBSCRIBE, OnRoomGroupUnsubcribed);
        sfs.AddEventListener(SFSEvent.ROOM_GROUP_UNSUBSCRIBE_ERROR, OnRoomGroupUnsubCribedError);
        sfs.AddEventListener(SFSEvent.ROOM_REMOVE,OnRoomRemove);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomError);
        sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomError);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoined);

        sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE,OnUserVariableUpdate);
         sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE,OnExtensionResponse);
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
           var cmd = (string) evt.Params["cmd"];
        SFSObject data = (SFSObject) evt.Params["params"];
        // SDLogger.Log("SFS: " + cmd);

        switch (cmd)
        {
            case ExtCmd.InitGame:
                HandleGetZoneConfig(data);
                break;
            case ExtCmd.NotResume:
                HandleNotResume(data);
                break;
            case ExtCmd.UserCount:
                HandleUserCount(data);
                break;
            default:
                Signals.Get<GamePlayExtensionResponseSignal>().Dispatch(cmd, data);
                break;
        }
    }

    private void HandleUserCount(ISFSObject data)
    {
        var vo = new UserCountVO();
        vo.fromSFSObject(data);

        var arrAll = vo.uc.Split((';')).ToList();
        var result = new List<List<string>>();
        for (var i = 0; i < arrAll.Count; i++)
        {
            var arr = arrAll[i].Split((',')).ToList();
            result.Add(arr);
        }

        if (result.Count == 0) return;
        UserCountsModel.Instance.uCounts = result;

    }

    private void HandleNotResume(ISFSObject data)
    {
        var z = screenManager.joinVO.zone;
        if(screenManager.CheckJoinZone(z))
        {
            FindBoardMediator.OpenPopup(z);
        }

        screenManager.CancelJoin();
    }

     private void HandleGetZoneConfig(ISFSObject data)
    {
        // SDLogger.Log("HandleGetZoneConfig");
        // Debug.Log("Init Data: ");
        // Debug.Log(data.ToJson());
        var initConfig = new InitGameInExt(data);
        GameConfig.ZoneCfg = initConfig.arrZone;
        GameConfig.ZoneStake = initConfig.arrStake;

        // SDLogger.Log(initConfig.arrZone);

        userModel.gVO.coin = initConfig.coin;
        userModel.gVO.exp = initConfig.exp;
        userModel.gVO.giftCount = initConfig.giftCount;
        userModel.gVO.vipScore = initConfig.vipScore;

        Signals.Get<UpdatePlayerInfoSignal>().Dispatch();
        // GameData.SetRoundPlayForMe(initConfig.roundPlay);
        screenManager.GoToScreen(ScreenManager.LOBBY);
    }

    private void OnUserVariableUpdate(BaseEvent evt)
    {
        var user = (User) evt.Params["user"];
        if(user.IsItMe)
        {
            userModel.gVO.coin = (long) user.GetVariable(GameConfig.VAR_COIN).GetDoubleValue();
            userModel.ip = (double) user.GetVariable(GameConfig.VAR_IP).GetDoubleValue();
            Signals.Get<RefreshCoinSignal>().Dispatch();
        }
    }

    public void AddEventListener(string e, EventListenerDelegate listener)
    {
        sfs.AddEventListener(e, listener);
    }

    public void RemoveEventListener(string e, EventListenerDelegate listener)
    {
        sfs.RemoveEventListener(e, listener);
    }

    public Room GetLastJoinedRoom()
    {
        return sfs.LastJoinedRoom;
    }

    public void Send(IRequest obj)
    {
        sfs.Send(obj);
    }

    public void LeaveCurRoom()
    {
        if (lastJoinedRoom != null)
        {
            sfs.Send(new LeaveRoomRequest(lastJoinedRoom));
        }
    }

    public void JoinRoom(int z, int r)
    {
        //Trên server không check đủ Bảo & exp khi vào phòng (chỉ check phía client, tức có thể hack)
        sfs.Send(new JoinRoomRequest(z + "_" + r));
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
        if (evt.Type == SFSEvent.CONNECTION_RETRY)
        {
            if (screenManager.isJoining)
                screenManager.CancelJoin();
        }
        else
        {
            Signals.Get<CancelJoinTourSignal>().Dispatch();
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        var reason = (string) evt.Params["reason"];
        Debug.Log($"Connection was lost, reason is: {reason}");
        if(reason == ClientDisconnectionReason.KICK)
        {
            screenManager.GoToScreen(ScreenManager.LOGIN);
        }
        else if(reason == ClientDisconnectionReason.BAN)
        {
            Debug.Log($"Thông báo {SDMsg.BANED}");
            screenManager.GoToScreen(ScreenManager.LOGIN);
        }
        else if (reason == ClientDisconnectionReason.MANUAL)
        {
            //Manual disconnection is usually ignored
        }
        else if (reason == ClientDisconnectionReason.UNKNOWN)
        {
            Signals.Get<LostConnectionSignal>().Dispatch();
            Signals.Get<CancelJoinTourSignal>().Dispatch();
            Debug.Log($"Thông Báo {SDMsg.DISLOGIN} ");
            OnReConnect();
        }
        else if (reason == ClientDisconnectionReason.IDLE)
        {
            Signals.Get<LostConnectionSignal>().Dispatch();
            if (screenManager.inBoard)
            {
                Debug.Log($"Thông Báo {SDMsg.FORCEDIS} ");
                OnReConnect();
            }
            else if (screenManager.inRoom)
            {
                Debug.Log($"Thông Báo {SDMsg.DISINROOM} ");
                OnReConnect();
            }
            else
            {
                Debug.Log($"Thông Báo {SDMsg.DISLOGIN} ");
                OnReConnect();
            }
        }
    }

    private void OnReConnect()
    {
        Debug.Log("On Reconnect");
        if (isConnected) Disconnect();

        GameConfig.LOGIN_COUNT = 0;
        screenManager.GoToScreen(ScreenManager.LOGIN);
    }

    private void OnLogin(BaseEvent evt)
    {
        Signals.Get<LoginSuccessSignal>().Dispatch();
    }

    private void OnLoginError(BaseEvent evt)
    {
        if (sfs.IsConnected) sfs.Disconnect();
    }

    private void OnUserExitedRoom(BaseEvent evt)
    {
        var user = (User) evt.Params["user"];
        if(!user.IsItMe)
        {
            return;
        }

        var room = (Room) evt.Params["room"];

        if(!room.IsGame)
        {
            var arr = room.GroupId.Split('_');
            var zoneId = int.Parse(arr[0]);
            if(zoneId == GameConfig.IdRoomVuongPhu && GameConfig.arrayGroupSubscribe.Count > 0)
            {
                foreach (var groupId in GameConfig.arrayGroupSubscribe.ToArray())
                {
                    
                }

                GameConfig.arrayGroupSubscribe = new List<string>();
            }
            else
            {
                UnSubcribeGroup(room.GroupId);
            }
        }

        if( screenManager.joinVO != null)
            screenManager.ApplyChangeScene();
        else if (lastJoinedRoom == null)
        {
            screenManager.joinVO = new NormalJoinVO(-1);
            screenManager.ApplyChangeScene();
        }
    }

    private void RoomGroupSubcribed(BaseEvent evt)
    {
        var groupId = (string) evt.Params["groupId"];

        var groupData = groupId.Split('_');
        var z = int.Parse(groupData[0]);
        var r = groupData[1];
        if(z == GameConfig.IdRoomVuongPhu)
        {
            GameConfig.arrayGroupSubscribe.Add(r);
        }

        var rooms = (List<Room>) evt.Params["newRooms"];
        StartCoroutine(RoomGroupSub(rooms));
    }

    private static IEnumerator RoomGroupSub(List<Room> rooms)
    {
        yield return new WaitUntil(() => ScreenManager.Instance.IsOnScreen(ScreenManager.ROOM));
        BoardInfoModel.Instance.UpdateBoardInfoByGroup(rooms);
    }

    private static void RoomGroupSubcribedError(BaseEvent evt)
    {
        var error = (string) evt.Params["erroMessage"];
    }

    private void OnRoomGroupUnsubcribed(BaseEvent evt)
    {
        var groupId = (string) evt.Params["groupId"];
    }

    private static void OnRoomGroupUnsubCribedError(BaseEvent evt)
    {
        var error = (string) evt.Params["errorMessage"];
    }

    private void OnRoomRemove(BaseEvent evt)
    {
        if(!screenManager.inRoom) return;
        var room = (Room) evt.Params["room"];
        if(room.IsGame) BoardInfoModel.Instance.ResetBoardInfo(room);
    }

    private void OnRoomError(BaseEvent evt)
    {
        screenManager.CancelJoin();
        var msgError = (string) evt.Params["errorMessage"];
        Debug.Log(msgError);
    }

    private void OnRoomJoined(BaseEvent evt)
    {
        var room = (Room) evt.Params["room"];

        if(room.IsGame && screenManager.isJoining)
        {
            var vo = GamePlayInfo.fromBoardName(room.Name);
            screenManager.ApplyJoin(vo.z, vo.r, vo.b);
            return;
        }

        if(!room.IsGame)
        {
            var groupData = room.GroupId.Split('_');
            var z = int.Parse(groupData[0]);
            if(z == GameConfig.IdRoomVuongPhu && GameConfig.arrayGroupSubscribe.Count == 0)
            {
                subscribeRoomGroup(z + "_" + GameConfig.NgoaiDienGroupId);
                DOVirtual.DelayedCall(0.5f, () => subscribeRoomGroup(z + "_" + GameConfig.NoiDienGroupId));
            }
            else
            {
                subscribeRoomGroup(room.GroupId);
            }
        }
        if (screenManager.joinVO != null)
        {
            screenManager.ApplyChangeScene();
        }
    }

    public bool IsJoinedInRoom(string name)
    {
        return mySelf.IsJoinedInRoom(sfs.GetRoomByName(name));
    }
    
    
    private void subscribeRoomGroup(string groupId)
    {
        // SDLogger.Log("subscribe room group groupId: " + groupId);
        sfs.Send(new SubscribeRoomGroupRequest(groupId));
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
