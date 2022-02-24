using System;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;

public class ChoChoiMediator : MonoBehaviour
{
    [SerializeField] private Transform content;

    private ScreenManager screenManager;
    private SmartFoxConnection sfs;

    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
    
    private ItemReqPlaySignal itemReqPlaySignal = Signals.Get<ItemReqPlaySignal>();

    private UserListInBoardVO vo = new UserListInBoardVO();
    [SerializeField] private PlayerInfoMediator itemReqPlay;

    private void Awake()
    {
        sfs = SmartFoxConnection.Instance;
        screenManager = ScreenManager.Instance;

        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtension);
        
        itemReqPlaySignal.AddListener(OnAcceptClick);
    }

    private void OnDestroy()
    {
        sfs.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtension);
        
        itemReqPlaySignal.RemoveListener(OnAcceptClick);
    }

    private void OnEnable()
    {
        if (!screenManager.inTour() && !screenManager.InChallenge()) sfs.SendExt(ExtCmd.UserInBoard);
    }

    private void OnExtension(BaseEvent evt)
    {
        var cmd = (string) evt.Params["cmd"];
        var data = (SFSObject) evt.Params["params"];

        switch (cmd)
        {
            case ExtCmd.RequestPlay:
                OnRequestPlay(data);
                break;
            case ExtCmd.UserInBoard:
                OnInitView(data);
                break;
            case ExtCmd.AcceptPlay:
                HandleAcceptPlay(data);
                break;
            case ExtCmd.Start:
                this.Hide();
                break;
        }
    }

    private void OnRequestPlay(SFSObject data)
    {
        var reqVO = new ReqOrAcceptPlayVO();
        reqVO.fromSFSObject(data);

        vo?.addReqUser(reqVO.uid);
        foreach (var u in gamePlayModel.game.UserList)
        {
            if (int.Parse(u.Name) != reqVO.uid) continue;
            AddUser(u);
            break;
        }
    }

    private void HandleAcceptPlay(SFSObject data)
    {
        var acVO = new ReqOrAcceptPlayVO();
        acVO.fromSFSObject(data);

        vo?.addAcceptUser(acVO.uid);
    }

    private void OnInitView(SFSObject data)
    {
        vo.fromSFSObject(data);
        OnDraw();
    }

    private void OnDraw()
    {
        foreach (var u in gamePlayModel.game.UserList)
        {
            if (vo.reqUserList.IndexOf(int.Parse(u.Name)) != -1) AddUser(u);
        }
    }

    private void AddUser(User u)
    {
        if (CheckItemAvaliable(int.Parse(u.Name))) return;
        var uView = itemReqPlay;
        uView.uid = int.Parse(u.Name);
        uView.UpdateInfo(u.GetVariable(GameConfig.VAR_UNAME).GetStringValue(),
            u.GetVariable(GameConfig.VAR_COIN).GetDoubleValue());
        Instantiate(uView, content);
    }

    private void OnAcceptClick(int uid)
    {
        for (var i = 0; i < content.childCount; i++)
        {
            var u = content.GetChild(i).GetComponent<PlayerInfoMediator>();
            if (u.uid != uid) continue;
            var accVo = new ReqOrAcceptPlayVO {uid = u.uid};
            sfs.SendExt(ExtCmd.AcceptPlay, accVo);
            break;
        }
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        var user = (User) evt.Params["user"];
        RemoveUser(user);
    }

    private void RemoveUser(User u)
    {
        var uid = int.Parse(u.Name);
        for (var i = content.childCount - 1; i >= 0; i--)
        {
            var uView = content.GetChild(i).GetComponent<PlayerInfoMediator>();
            if (uid != uView.uid) continue;
            DestroyImmediate(content.transform.GetChild(i).gameObject);
            break;
        }
    }

    private bool CheckItemAvaliable(int uid)
    {
        for (var i = 0; i < content.childCount; i++)
        {
            var u = content.GetChild(i).GetComponent<PlayerInfoMediator>();
            if (u.uid == uid) return true;
        }
        return false;
    }

    public void ClosePopup()
    {
        for (var i = content.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(content.transform.GetChild(i).gameObject);
        }

        this.Hide();
    }
}