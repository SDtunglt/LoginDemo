using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using Sirenix.OdinInspector;
    
public class ScreenManager : MonoBehaviour 
{
    public const string LOGIN = "LoginScene";
    public const string LOBBY = "LobbyScene";
    public const string ROOM = "RoomScene";
    public const string GAMEPLAY = "GamePlayScene";
    [SerializeField] private List<ScreenDefine> screens;
    [SerializeField] private int z, r;

    [Button]
    public void JoinRoom()
    {
        if(!isJoining) CheckConnToJoin(new NormalJoinVO(z,r));
    }

    private Action screenAction = () => { };
    private string currentScreen;
    private const int MAX_KICKED_TIME = 120;

    private Dictionary<string, long> kickedRooms = new Dictionary<string, long>();
    public static ScreenManager Instance
    {
        get{
            if(instance != null) return instance;
            instance = FindObjectOfType<ScreenManager>();
            DontDestroyOnLoad(instance);
            
            return instance;
        }
    }

    private Tween delayJoin;
    public BaseJoinVO joinVO;
    public RoomResume roomResume;
    public NormalJoinVO currentVO = new NormalJoinVO(-1,-1,-1);
    private static ScreenManager instance;
    private SmartFoxConnection sfs;
    private UserModel userModel = UserModel.Instance;
    private GameModel gameModel = GameModel.Instance;
    public delegate void OnScreenChangeDefine(string screenId);
    public OnScreenChangeDefine onScreenChangeDefine;
    public static int LastZone = -1, LastRoom = -1, LastBoard;

    private void Start() 
    {
        sfs = SmartFoxConnection.Instance;
    }

    public void GoToScreen(string screen, bool isShowFlash = true)
    {
        if(screen == null) return;
        if(currentScreen != null && screen == currentScreen)
        {
            return;
        }
        currentScreen = null;

        if(isShowFlash)
        {
            this.ShowFlashWithCallBack(() =>
            {
                LoadScreenAsync(screen, () => { onScreenChangeDefine?.Invoke(screen);});
            });
        }
        else
        {
            LoadScreenAsync(screen, () => {onScreenChangeDefine?.Invoke(screen);});
        }
    }


    private void LoadScreenAsync(string screen, Action onComplete)
    {
        StartCoroutine(ILoadSceneAsync(screen, onComplete));
    }

    private IEnumerator ILoadSceneAsync(string screen, Action onComplete)
    {
        foreach (var define in screens)
        {
            if(define.id != screen)
            {
                define.screenGo.Hide();
            }
            else
            {
                define.screenGo.Show();
            }
        }

        onComplete?.Invoke();
        yield return new WaitForEndOfFrame();
        currentScreen = screen;
    }

    public int zone => currentVO.zone;
    public int room => currentVO.room;
    public int board => currentVO.board;
    public ZoneInfo zoneInfo => currentVO.zone >= 0 ? GameConfig.ZoneCfg[currentVO.zone] : null;
    public bool inRoom => !InTour() && !InChallenge() && currentVO.IsInRoom();
    public bool InEntrance => currentVO.IsInEntrance();
    public bool inBoard => currentVO.isInBoard();
    //public bool IsCanULao => !inTour() && (!InChallenge() ? GamePlayModel.Instance.IsULao : zoneInfo.canULao);

    public bool InTour()
    {
        return  currentVO.zone == GameConfig.IdRoomThiHuong ||
                currentVO.zone == GameConfig.IdRoomThiHoi ||
                currentVO.zone == GameConfig.IdRoomThiDinh;
    }

    public bool InChallenge()
    {
        return currentVO.zone == GameConfig.IdRoomChalenge;
    }

    public bool IsOnChanhTongTriPhuDaiLau()
    {
        return zone == 4 && room == 3;
    }

    public bool isJoining => joinVO != null;

    public void JoinTour(TourJoinVO vo)
    {
        CheckConnToJoin(vo);
    }

    public void CheckJoin(CheckJoinVO vo)
    {
        CheckConnToJoin(vo);
    }

    public void QuickJoin(QuickJoinVO vo)
    {
        CheckConnToJoin(vo);
    }

    public void ChallengeJoin(BaseJoinVO vo)
    {
        CheckConnToJoin(vo);
    }

    public GameObject replayScreen;

    public void OpenReplayScreen(string matchId)
    {
        if (IsOnScreen(GAMEPLAY))
        {
            //Toast.Show("Kh??ng th??? xem l???i v??n ch??i trong ph??ng ch??i!");
            return;
        }

        API.GetRelay(matchId, o =>
        {
            ReplayModel.ReceiveReplay(o);
            currentVO = new NormalJoinVO(ReplayModel.zone, ReplayModel.room, ReplayModel.board);
            replayScreen.Show();
        }, s => { SDLogger.LogError(s); });
    }

    public void CloseReplayScreen()
    {
        replayScreen.Hide();
    }

    

    public void JoinRoom(int _zone,int _room, bool isQuickJoin = false)
    {
        if(isQuickJoin)
        {
            delayJoin?.Kill();
            delayJoin = DOVirtual.DelayedCall(1f, () =>
            {
                var vo = new NormalJoinVO(_zone,_room);
                if(!vo.Equals(currentVO)) CheckConnToJoin(vo);
            });
        }
        else
        {
            var vo = new NormalJoinVO(_zone,_room);
            if(!vo.Equals(currentVO)) CheckConnToJoin(vo);
        }
    }

    public void OnLeave()
    {
        if(inRoom || InTour() || InChallenge())
        {
            GoEntrance();
        }
        else
        {
            JoinRoom(zone,room);
        }
    }
    

    public void CancelJoin()
    {
        joinVO = null;
        Signals.Get<CancelJoinTourSignal>().Dispatch();
    }

    private void CheckConnToJoin(BaseJoinVO vo)
    {
        delayJoin?.Kill();
        if(isJoining) return;
        joinVO = vo;

        if(joinVO != null && joinVO.IsInEntrance())
        {
            OnScreenChange();
            return;
        }

        if(sfs.isConnected && sfs.currentIp == GameConfig.HOST) OnScreenChange();
    }

    private void OnScreenChange()
    {
        if(joinVO == null) return;
        switch (joinVO.tpe)
        {
            case (int) BaseJoin.CHECK_JOIN:
                    sfs.SendExt(ExtCmd.Resume, (CheckJoinVO) joinVO);
                    break;
            case (int) BaseJoin.NORMAL_JOIN:
                    var normalJoin = (NormalJoinVO) joinVO;
                    if(normalJoin.IsInEntrance())
                    {
                        if(sfs.lastJoinedRoom != null) sfs.LeaveCurRoom();
                        else ApplyChangeScene();
                    }
                    else if(normalJoin.IsInRoom())
                    {
                        sfs.JoinRoom(normalJoin.zone,normalJoin.room);
                        _statusToTest = "JoinRoomSuccess";
                    }
                    else if(normalJoin.isInBoard()) sfs.SendExt(ExtCmd.Join, normalJoin);
                    break;
            case (int) BaseJoin.QUICK_JOIN:
                    sfs.SendExt(ExtCmd.QuickJoin,(QuickJoinVO) joinVO);
                    break;
            case (int) BaseJoin.TOUR_JOIN:
                    sfs.SendExt(ExtCmd.TourJoin,(TourJoinVO) joinVO);
                    break;
            case (int) BaseJoin.CHALLENGE_JOIN:
                    sfs.SendExt(ExtCmd.CreateChallenge,joinVO);
                    break;
            case (int) BaseJoin.CHALLENGE_REQUEST_JOIN:
                    sfs.SendExt(ExtCmd.JoinChallenge,joinVO);
                    break;
        }
    }

    public void ApplyJoin(int z, int r, int b)
    {
        ChangeScene(new NormalJoinVO(z,r,b));
    }

    public void ApplyChangeScene()
    {
        if(joinVO is NormalJoinVO) ChangeScene(joinVO as NormalJoinVO);
    }

    public void JoinBoard(int board_, int room_, string password = null)
    {
        // FirebaseAnalyticsExtension.Instance.GoToBoard();
        if (board_ == currentVO.board) return;
        if (room_ != -1) currentVO.room = room_;
        if (!IsCanJoinKickedRoom(zone, room_, board_))
        {
            Debug.Log("B???n v???a b??? ???? ra kh???i b??n n??y. Sau 2 ph??t m???i c?? th??? v??o l???i b??n n??y!");
            return;
        }

        CheckConnToJoin(new NormalJoinVO(currentVO.zone, currentVO.room, board_, password));
    }

    private void ChangeScene(NormalJoinVO vo)
    {
        var oldScene = currentVO;
        currentVO = vo;

        Signals.Get<ScreenChangedSignal>().Dispatch(oldScene);

        if(!oldScene.IsInRoom() && inRoom)
        {
            GoToScreen(ROOM);
        }
        else if(!oldScene.IsInEntrance() && currentVO.zone == -1)
        {
            GoToScreen(LOBBY);
        }
        else if((oldScene.IsInRoom() && !inRoom) || InTour() || InChallenge())
        {
            GoToScreen(GAMEPLAY);
        }
        else if(!oldScene.IsInRoom())
        {
            GoToScreen(GAMEPLAY);
        }

        CancelJoin();
    }

    public void GoEntrance()
    {
        if(!InEntrance) CheckConnToJoin(new NormalJoinVO(-1));
        if(!TourModel.isOutDateConfig) return;
        sfs.SendExt(ExtCmd.UpdateTourCfg);
        TourModel.isOutDateConfig = false;
    }

    public bool CheckJoinZone(int _zone)
    {
        var z = GameConfig.ZoneCfg[_zone];
        if (userModel.gVO.level >= z.level && userModel.gVO.coin >= z.coinToJoin &&
            userModel.gVO.vipScore >= z.vipToJoin) return true;
        string msg;
        if (userModel.gVO.coin < z.coinToJoin)
        {
            msg = gameModel.IsNormalPlayer()
                ? SDMsg.Join(SDMsg.CANTJOINZONECOIN, StringUtils.FormatMoney(z.coinToJoin), z.name)
                : SDMsg.CANTJOINZONEREVIEW;
            _statusToTest = "NotEnoughCoin";
        }
        else if (userModel.gVO.level < z.level)
        {
            msg = SDMsg.Join(SDMsg.CANTJOINZONELEVEL, z.level, z.name);
            _statusToTest = "NotEnoughLevel";
        }
        else
        {
            msg = SDMsg.Join(SDMsg.CANTJOINZONEVIP, StringUtils.FormatMoney(z.vipToJoin), z.name);
            _statusToTest = "NotEnoughVip";
        }

        if (zone == GameConfig.IdRoomVuongPhu)
        {
            if (userModel.gVO.coin < z.coinToJoin && userModel.gVO.vipScore < z.vipToJoin)
            {
                if (gameModel.IsNormalPlayer())
                {
                    msg = SDMsg.Join(SDMsg.CANTJOINZONE, StringUtils.FormatMoney(z.coinToJoin),
                        StringUtils.FormatMoney(z.vipToJoin), z.name);
                    _statusToTest = "NotEnoughVuongPhu";
                }
                else
                {
                    msg = SDMsg.CANTJOINZONEREVIEW;
                    _statusToTest = "Review";
                }
            }
        }

        BasicPopup.Open("Th??ng B??o",
            msg);
        return false;
    }

    public bool IsCanJoinKickedRoom(int z, int r, int b)
    {
        var v = $"{z}_{r}_{b}";
        if (kickedRooms.ContainsKey(v))
        {
            if (kickedRooms[v] + MAX_KICKED_TIME < DateTime.Now.TotalSeconds())
            {
                kickedRooms.Remove(v);
                LocalStorageUtils.SaveKickedRoom(kickedRooms);
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
    
    private enum BaseJoin
    {
        CHECK_JOIN = 0,
        NORMAL_JOIN = 1,
        QUICK_JOIN = 2,
        TOUR_JOIN = 3,
        RESUME_JOIN = 4,
        ARENA_JOIN = 5,
        CHALLENGE_JOIN = 9,
        CHALLENGE_REQUEST_JOIN = 10,
    }

    private string _statusToTest = "";
    public bool IsOnScreen(string screenId)
    {
        Debug.Log(screenId);
        return currentScreen == screenId;
    }

}

public class RoomResume
{
    public int z;
    public int r;
    public int b;
}

[Serializable]
public class ScreenDefine
{
    public string id;
    public GameObject screenGo;
}
