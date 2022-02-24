using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayMediator : ConnectionCpnt, IPlayersContainer
{
    [SerializeField] private Image bgIm;
    [SerializeField] private Button btnStart, btnQuest;
    [SerializeField] private TextCountDown txtCountDown;
    [SerializeField] private TMP_Text txtLocation;
    [SerializeField] private MyCardMediator cardInHand;
    [SerializeField] private DealCards dealCardsPrefab;
    [SerializeField] private XuongMediator xuongMediator;
    [SerializeField] private SumupMediator sumupMediator;
    [SerializeField] private CuocViewMediator cuocViewMediator;
    [SerializeField] private Clock clock;
    [SerializeField] private NocMediator nocMediator;
    [SerializeField] private RectTransform nocRect;
    [SerializeField] private Button btnXinChoi, btnChoChoi, btnFreeCoin, btnInvite;
    [SerializeField] private TMP_Text txtXinChoi, txtGiftCount;
    [SerializeField] private ChoChoiMediator choChoiPopup;
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private TinhDiemMediator tinhDiem;
    [SerializeField] private Image imgQuestNoti;
    [SerializeField] private ConfirmDealCardMediator confirmDealCard;
    [SerializeField] private float timePauseForUser = 5f;
    [SerializeField] private Image imgBackCard;
    public List<PlayerMediator> playerViews;
    private DealCards currentDealCards { get; set; }
    private readonly UserModel userModel = UserModel.Instance;
    private readonly TourModel tourModel = TourModel.Instance;
    private readonly GamePlayModel gamePlayModel = GamePlayModel.Instance;
    private readonly CanUCardsModel canUCardsModel = CanUCardsModel.Instance;
    private readonly UModel uModel = UModel.Instance;
    private readonly PlayModel playModel = PlayModel.Instance;
    private ScreenManager screenManager;
    private GamePlayLogic gamePlayLogic;
    private SmartFoxConnection sfs;
    private bool isWaitforComfirmMouseIdle = false;
    private SDTimer timer;
    private Tween twDelayAppPause;

    private PlayerLeavedSignal playerLeavedSignal = Signals.Get<PlayerLeavedSignal>();
    private PlayerJoinedSignal playerJoinedSignal = Signals.Get<PlayerJoinedSignal>();
    private BoardStateChangedSignal boardStateChangedSignal = Signals.Get<BoardStateChangedSignal>();
    private StopGameSignal stopGameSignal = Signals.Get<StopGameSignal>();
    private ShowXuongPanelSignal showXuongPanelSignal = Signals.Get<ShowXuongPanelSignal>();
    private ShowSumupSignal showSumupSignal = Signals.Get<ShowSumupSignal>();
    private ShowTimeOutMsgSignal showTimeOutMsgSignal = Signals.Get<ShowTimeOutMsgSignal>();
    private SyncBoardConfigSignal syncBoardConfigSignal = Signals.Get<SyncBoardConfigSignal>();
    private ScoreChangeSignal scoreChangeSignal = Signals.Get<ScoreChangeSignal>();
    private GiveCardsCompletedSignal giveCardsCompletedSignal = Signals.Get<GiveCardsCompletedSignal>();
    private ShowEffectPlayerU showEffectPlayerU = Signals.Get<ShowEffectPlayerU>();
    private RefreshCoinSignal refreshCoinSignal = Signals.Get<RefreshCoinSignal>();
    private ReceivedTimeCountDownSignal receivedTimeCountDownSignal = Signals.Get<ReceivedTimeCountDownSignal>();
    private HideTimeCounterSignal hideTimeCounterSignal = Signals.Get<HideTimeCounterSignal>();
    private ShowTimeCounterSignal showTimeCounterSignal = Signals.Get<ShowTimeCounterSignal>();
    private UpdatePlayerInBoardSignal updatePlayerInBoardSignal = Signals.Get<UpdatePlayerInBoardSignal>();
    private TourVOChangedSignal tourVOChangedSignal = Signals.Get<TourVOChangedSignal>();
    private ResumeCompletedSignal resumeCompletedSignal = Signals.Get<ResumeCompletedSignal>();
    private MissionCompleteSignal missionCompleteSignal = Signals.Get<MissionCompleteSignal>();
    private MissionFeaturesUpdatedSignal missionFeaturesUpdatedSignal = Signals.Get<MissionFeaturesUpdatedSignal>();
    private UserListInBoardVO askJoinVO;
    private bool isCanShowInviteBtn = true;

    private void Awake()
    {
        Signals.Get<OnJoinGamePlaySignal>().Dispatch();
        screenManager = ScreenManager.Instance;
        gamePlayLogic = GamePlayLogic.Instance;
        sfs = SmartFoxConnection.Instance;

        canUCardsModel.sdInit();
        canUCardsModel.ReInit();
        playModel.ReInit();
        uModel.ReInit();

        playerJoinedSignal.AddListener(OnPlayerJoined);
        playerLeavedSignal.AddListener(OnPlayerLeaveBoard);
        boardStateChangedSignal.AddListener(OnBoardStateChanged);
        showXuongPanelSignal.AddListener(OnShowXuongPanel);
        showSumupSignal.AddListener(OnShowTongKet);
        stopGameSignal.AddListener(OnStopGame);
        showTimeOutMsgSignal.AddListener(OnShowMsg);
        syncBoardConfigSignal.AddListener(OnSyncBoardConfig);
        giveCardsCompletedSignal.AddListener(OnShowCardPlayer);
        showEffectPlayerU.AddListener(OnShowEffectPlayer);
        refreshCoinSignal.AddListener(OnRefreshCoin);
        receivedTimeCountDownSignal.AddListener(OnReceiveBoardStartCounter);
        showTimeCounterSignal.AddListener(ShowCountDown);
        hideTimeCounterSignal.AddListener(OnHideTimeCounter);
        updatePlayerInBoardSignal.AddListener(UpdatePlayerInBoard);
        tourVOChangedSignal.AddListener(OnTourVOChange);
        resumeCompletedSignal.AddListener(OnStartTimerIdle);
        missionCompleteSignal.AddListener(OnMissionComplete);
        missionFeaturesUpdatedSignal.AddListener(OnMissionFeaturesUpdated);
        OnBoardJoined();

        mapListener.Add(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        mapListener.Add(SFSEvent.EXTENSION_RESPONSE, OnExtension);
        mapListener.Add(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        Signals.Get<GamePlayBackgroundChangeSignal>().AddListener(OnChangeBgIm);
        Signals.Get<KickOutTourUserAfterSumUpSignal>().AddListener(OnKickOutUserAfterSumUp);
        //Signals.Get<OnTriggerShareGame>().AddListener(OnShowShareGameButton);

        // if (SwitchUI.CurrentType == ComponentType.Chan2 && GameUtils.GameBG <= 2) // khi mà đang ở chắn 2, nhưng mà nền  <2 => vừa từ chắn 5 qua
        // {
        //     int lastBG = GameUtils.GameBG; // cái bg user chọn ở chắn 5
        //     if (GameUtils.GameBG2 >= 2) // nếu đã từng lưu 
        //     { // nghĩa là ở chắn 2 trước đó đã từng dùng bg là GameBG2
        //         GameUtils.GameBG = GameUtils.GameBG2; // nên là gán cái đấy vào
        //     }
        //     else
        //     { // còn nếu không
        //         if (screenManager.zone == 0)
        //         {
        //             GameUtils.GameBG = 4;
        //         }
        //         if (screenManager.zone == 1)
        //         {
        //             GameUtils.GameBG = 5;
        //         }
        //         if (screenManager.zone == 2)
        //         {
        //             GameUtils.GameBG = 6;
        //         }
        //         if (screenManager.zone == 3)
        //         {
        //             GameUtils.GameBG = 7;
        //         }
        //         if (screenManager.zone == 4)
        //         {
        //             GameUtils.GameBG = 8;
        //         }
        //     }

        //     GameUtils.GameBG2 = lastBG; // lưu lại cái bg chọn lần cuối ở chắn 5 vào GameBG2
        // }
        // else if (SwitchUI.CurrentType == ComponentType.Chan5 && GameUtils.GameBG >= 2)
        // {
            int lastBG = GameUtils.GameBG;
            if (GameUtils.GameBG2 <= 2)
            {
                GameUtils.GameBG = GameUtils.GameBG2;
            }
            else if (GameUtils.GameBG < 0 || GameUtils.GameBG > 2)
            {
                GameUtils.GameBG = 0;
            }
            GameUtils.GameBG2 = lastBG;

        // }

        if (screenManager.IsOnChanhTongTriPhuDaiLau())
        {
            GameUtils.GameBG = 2;
            GameUtils.GameBC = 1;
        }
        else
        {
            GameUtils.ClearGameBGPhu();
            GameUtils.ClearGameBCPhu();
        }
        
        OnChangeBgIm(GameUtils.GameBG);
  
    }

    private void Start()
    {
        scoreChangeSignal.Dispatch(gamePlayModel.GETScoreVo(), gamePlayModel.tinhDiem);

        timer = new SDTimer(SDTimeout.IDLE_IN_BOARD, true);
        timer.AddEvent(ShowMsgIdle);

        //if (screenManager.IsOnChanhTongTriPhuDaiLau())
        //{
        //    GameUtils.GameBG = 2;
        //    GameUtils.GameBC = 1;
        //}
        //else
        //{
            
        //}
        //OnChangeBgIm(GameUtils.GameBG);
        
    }


    private void OnChangeBgIm(int index)
    {
        bgIm.sprite = GlobalDataManager.Ins.backgrounds[index];
    }

    private float timeOnPause;

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            SetUpOnUserUnPause();
        }
        else
        {
            SetUpOnUserPause();
        }
    }

    private void SetUpOnUserPause()
    {
        var obj = SFSObject.NewInstance();
        obj.PutLong("t", DateTime.Now.TotalSeconds());
        obj.PutBool("b", true);
        SmartFoxConnection.Instance.SendExt(ExtCmd.UserPause, obj);
    }

    private void SetUpOnUserUnPause()
    {
        var obj = SFSObject.NewInstance();
        obj.PutLong("t", DateTime.Now.TotalSeconds());
        obj.PutBool("b", false);
        SmartFoxConnection.Instance.SendExt(ExtCmd.UserPause, obj);
    }

    private void KickAndReJoin()
    {
        screenManager.roomResume = new RoomResume
        {
            z = screenManager.zone,
            r = screenManager.room,
            b = screenManager.board
        };
        screenManager.OnLeave();
    }

    private void OnUserVariableUpdate(BaseEvent evt)
    {
        var user = (User) evt.Params["user"];
        foreach (var pView in playerViews.Where(pView => pView.sdPlayer != null && pView.sdPlayer.u == user))
        {
            pView.UpdateCoin(pView.sdPlayer.coin);
            break;
        }
    }

    private void OnDestroy()
    {
        showTimeOutMsgSignal.Dispatch("");
        playerJoinedSignal.RemoveListener(OnPlayerJoined);
        playerLeavedSignal.RemoveListener(OnPlayerLeaveBoard);
        boardStateChangedSignal.RemoveListener(OnBoardStateChanged);
        showXuongPanelSignal.RemoveListener(OnShowXuongPanel);
        showSumupSignal.RemoveListener(OnShowTongKet);
        stopGameSignal.RemoveListener(OnStopGame);
        showTimeOutMsgSignal.RemoveListener(OnShowMsg);
        receivedTimeCountDownSignal.RemoveListener(OnReceiveBoardStartCounter);
        showTimeCounterSignal.RemoveListener(ShowCountDown);
        hideTimeCounterSignal.RemoveListener(OnHideTimeCounter);
        syncBoardConfigSignal.RemoveListener(OnSyncBoardConfig);
        giveCardsCompletedSignal.RemoveListener(OnShowCardPlayer);
        showEffectPlayerU.RemoveListener(OnShowEffectPlayer);
        refreshCoinSignal.RemoveListener(OnRefreshCoin);
        updatePlayerInBoardSignal.RemoveListener(UpdatePlayerInBoard);
        tourVOChangedSignal.RemoveListener(OnTourVOChange);
        resumeCompletedSignal.RemoveListener(OnStartTimerIdle);
        missionCompleteSignal.RemoveListener(OnMissionComplete);
        missionFeaturesUpdatedSignal.RemoveListener(OnMissionFeaturesUpdated);


        Signals.Get<GamePlayBackgroundChangeSignal>().RemoveListener(OnChangeBgIm);
        Signals.Get<KickOutTourUserAfterSumUpSignal>().RemoveListener(OnKickOutUserAfterSumUp);
        // Signals.Get<OnTriggerShareGame>().RemoveListener(OnShowShareGameButton);
        PlayLogic.Instance.delaySendExtCmd?.Kill();
        timer?.StopTimer();
        shareGameBtn.ClearAll();
        ReInitOnDestroy();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        cuocViewMediator.Hide();
        chatManager.sendMsgFunc = SendMsgChat;
        OnRefreshCoin();
        if (!(screenManager.inTour() || screenManager.InChallenge())) return;
        // xét nếu là trong danh sách tính điểm thì setting tourModel.playing = true.
        if (gamePlayModel.tinhDiem)
        {
            var scoreVO = gamePlayModel.GetScoreVO();
            if (scoreVO != null && scoreVO.uids.Count > 0
                                && scoreVO.uids.IndexOf(int.Parse(userModel.uid)) >= 0)
            {
                if (!tourModel.Playing) tourModel.Playing = true;
            }
        }

        if (gamePlayModel.isPlayer)
        {
            if (!tourModel.Playing) tourModel.Playing = true;
        }

        btnXinChoi.interactable = true;
        btnXinChoi.gameObject.SetActive(false);
        btnStart.gameObject.SetActive(false);
        screenManager.roomResume = null;

        // testProcessCuocsStr("Trì Thông Bạch Thủ Chi Tám Đỏ 2 Tôm");
    }

    private static void SendMsgChat(string str)
    {
        SmartFoxConnection.Instance.SendNoticeMsg(str);
    }

    public void OnFreeCoinClick()
    {
        // FirebaseAnalyticsExtension.Instance.FreeCoin();
        //FirebaseAnalyticsExtension.Instance.LogEvent(FirebaseEvent.FreeCoin);
        sfs.GetFreeCoin();
    }

    private void OnRefreshCoin()
    {
        if (userModel.gVO.coin >= GameConfig.MIN_COIN)
        {
            btnFreeCoin.gameObject.SetActive(false);
        }
        else
        {
            txtGiftCount.text = userModel.gVO.giftCount.ToString();
            btnFreeCoin.gameObject.SetActive(true);
        }
    }

    private void OnMissionComplete(bool status)
    {
        if (imgQuestNoti != null) imgQuestNoti.gameObject.SetActive(status);
    }

    private void OnMissionFeaturesUpdated(bool status)
    {
        if (btnQuest != null) btnQuest.gameObject.SetActive(false);
    }

    private void OnShowEffectPlayer(int idx)
    {
        playerViews[idx].OnShowEffectU();
    }

    private void OnShowCardPlayer()
    {
        nocMediator.Show();

        nocRect.localPosition = (gamePlayModel.isPlayer) ? new Vector3(0, 149, 0) : new Vector3(0, 0, 0);

        btnStart.Hide();
        cardInHand.gameObject.SetActive(gamePlayModel.isPlayer);
        playerViews[0].gameObject.SetActive(!gamePlayModel.isPlayer);
        if (gamePlayModel.isPlayer) showTimeOutMsgSignal.Dispatch(AppMsg.XEPCHANCA);
        clock.StartClock(Clock.COUNT_FIRST, gamePlayModel.TimeSortAndPlayFirst(),
            gamePlayModel.sdplayers[playModel.curTurn]);
    }

    private void OnShowGamesResult(GamesResultVO vo, string name)
    {
        GamesResultMediator.OpenPopup(vo, name);
    }

    private void OnSyncBoardConfig()
    {
        UpdateXinChoi();
        tinhDiem.gameObject.SetActive(gamePlayModel.tinhDiem);
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        if (askJoinVO == null) return;
        askJoinVO.removeUser((User) evt.Params["user"]);
        UpdateXinChoi();
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
                HandleUserListInBoard(data);
                break;
            case ExtCmd.AcceptPlay:
                HandleAcceptPlay(data);
                break;
            case ExtCmd.Start:
                btnStart.Hide();
                isCanShowInviteBtn = false;
                UpdateXinChoi();
                OnHideTimeCounter();
                break;
            case ExtCmd.Kick:
                HandleVoteKick(data);
                break;
        }
    }

    private void OnRequestPlay(SFSObject data)
    {
        var vo = new ReqOrAcceptPlayVO();
        vo.fromSFSObject(data);

        if (askJoinVO == null) return;
        askJoinVO.addReqUser(vo.uid);
        UpdateXinChoi();
        txtXinChoi.text = askJoinVO.reqUserList.Count + " người xin chơi";
    }

    private void HandleAcceptPlay(SFSObject data)
    {
        var vo = new ReqOrAcceptPlayVO();
        vo.fromSFSObject(data);

        if (askJoinVO == null) return;
        askJoinVO.addAcceptUser(vo.uid);
        UpdateXinChoi();
    }

    private void HandleUserListInBoard(SFSObject data)
    {
        var vo = new UserListInBoardVO();
        vo.fromSFSObject(data);

        askJoinVO = vo;
        UpdateXinChoi();
    }

    private void UpdateXinChoi()
    {
        var total = gamePlayModel.sdplayers.Count;
        if (gamePlayModel.xinChoi && total < GameConfig.MAX_PLAYER_IN_GAME &&
            (!gamePlayModel.isPlayer ||
             !gamePlayModel.isPlaying && askJoinVO != null && askJoinVO.reqUserList.Count > 0))
        {
            btnXinChoi.gameObject.SetActive(!screenManager.inTour() && !screenManager.InChallenge());
            if (gamePlayModel.isPlayer)
            {
                BtnAskPlay(false);
            }
            else if (askJoinVO != null && askJoinVO.reqUserList.IndexOf(int.Parse(userModel.uid)) == -1)
            {
                BtnAskPlay(true);
            }

            Vector3 p;
            var rec = btnXinChoi.transform as RectTransform;
            if (total > 2)
            {
                rec.SetAnchor(GameExtension.AnchorPresets.MiddleLeft);
                p = playerViews[3].tfCountdown.transform.position;
            }
            else
            {
                rec.SetAnchor(GameExtension.AnchorPresets.MiddleRight);
                p = playerViews[1].tfCountdown.transform.position;
            }

            btnXinChoi.transform.position = p;
        }
        else
        {
            btnXinChoi.Hide();
            btnChoChoi.Hide();
        }

        UpdateInvite();
    }

    private void UpdateInvite()
    {
        var total = gamePlayModel.sdplayers.Count;

        if ((!gamePlayModel.isBoardOwner && !screenManager.InChallenge() )|| gamePlayModel.isPlaying || total >= GameConfig.MAX_PLAYER_IN_GAME ||
            screenManager.inTour() || !isCanShowInviteBtn)
        {
            btnInvite.Hide();
        }
        else
        {
            ShowInviteBtn();
        }
    }

    private void ShowInviteBtn()
    {
        var total = gamePlayModel.sdplayers.Count;

        if (btnChoChoi.gameObject.activeInHierarchy)
        {
            if (total > 2)
            {
                btnInvite.Hide();
            }
            else
            {
                btnInvite.Show();
                Vector3 p;
                var rec = btnInvite.transform as RectTransform;
                rec.SetAnchor(GameExtension.AnchorPresets.MiddleLeft);
                p = playerViews[1].tfCountdown.transform.position;
                btnInvite.transform.position = p;
            }
        }
        else
        {
            btnInvite.Show();
            Vector3 p;
            var rec = btnInvite.transform as RectTransform;
            if (total > 2)
            {
                rec.SetAnchor(GameExtension.AnchorPresets.MiddleLeft);
                p = playerViews[3].tfCountdown.transform.position;
            }
            else
            {
                rec.SetAnchor(GameExtension.AnchorPresets.MiddleRight);
                p = playerViews[1].tfCountdown.transform.position;
            }

            btnInvite.transform.position = p;
        }
    }

    private void BtnAskPlay(bool isXinChoi)
    {
        btnXinChoi.gameObject.SetActive(isXinChoi && !screenManager.inTour() && !screenManager.InChallenge());
        btnChoChoi.gameObject.SetActive(!isXinChoi && !screenManager.inTour() && !screenManager.InChallenge());
    }

    public void OnXinChoiClick()
    {
        if (gamePlayModel.isPlayer) return;
        btnXinChoi.interactable = false;
        sfs.SendExt(ExtCmd.RequestPlay);

        var msg = userModel.name + " xin chơi";
        sfs.SendNoticeMsg(msg);
    }

    private void OnShowMsg(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            Toast.HideAllToast();
            return;
        }

        if (!gamePlayModel.resuming) Toast.ShowFloat(msg, 4f);
    }

    private void OnDealCard()
    {
        currentDealCards = Instantiate(dealCardsPrefab, btnStart.transform.parent);

        currentDealCards.SetData(ChiaBaiModel.Instance.cai, GetSeatHaveCai(),
            gamePlayModel.UserBocCaiIdx() == gamePlayModel.myIdx
                ? ""
                : gamePlayModel.sdplayers[gamePlayModel.UserBocCaiIdx()].name,
            playerViews, OnDealCardDone);
    }

    public void OnDealCardDone()
    {
        OnShowCardPlayer();
        OnHideTimeCounter();
        gamePlayModel.status = BoardStatus.PLAYING;
        DestroyImmediate(currentDealCards.gameObject);
        currentDealCards = null;
        if (gamePlayModel.isPlayer)
        {
            isWaitforComfirmMouseIdle = false;
            timer.StartTimer();
            playerViews[0].gameObject.SetActive(false);
        }

        // clock.StartClock(Clock.COUNT_FIRST, gamePlayModel.TimeSortAndPlayFirst(),
        //     gamePlayModel.sdplayers[playModel.curTurn]);
        if (currentDealCards)
        {
            DestroyImmediate(currentDealCards.gameObject);
        }

        currentDealCards = null;
    }

    private void OnStartTimerIdle()
    {
        if (!gamePlayModel.isPlayer) return;
        isWaitforComfirmMouseIdle = false;
        timer.StartTimer();
    }

    private int GetSeatHaveCai()
    {
        if (gamePlayModel.myIdx == -1)
        {
            if (gamePlayModel.sitCount == 2)
            {
                return ChiaBaiModel.Instance.playerHaveCaiIdx == 0 ? 0 : 2;
            }

            return (ChiaBaiModel.Instance.playerHaveCaiIdx + gamePlayModel.sitCount) % gamePlayModel.sitCount;
        }
        else
        {
            if (gamePlayModel.sitCount == 2)
            {
                return ChiaBaiModel.Instance.playerHaveCaiIdx == gamePlayModel.myIdx ? 0 : 2;
            }

            return (ChiaBaiModel.Instance.playerHaveCaiIdx - gamePlayModel.myIdx + gamePlayModel.sitCount) %
                   gamePlayModel.sitCount;
        }
    }

    private void SetLocation()
    {
        var z = screenManager.zone;
        var r = screenManager.room;
        var b = screenManager.board;
        var nameRoom = GameConfig.ZoneCfg[z].name;

        if (screenManager.InChallenge())
        {
            txtLocation.text = $"{nameRoom} - Bàn {b + 1}";
            return;
        }

        var vo = tourModel.tourVO;

        if (!screenManager.inTour())
        {
            txtLocation.text = nameRoom + " - " + GameConfig.ZoneCfg[z].rooms[r] + " - Bàn " + (b + 1);
        }
        else
        {
            if (tourModel.IsThiDinh && !string.IsNullOrEmpty(vo.ThiDinhName))
            {
                txtLocation.text = nameRoom + " - " + char.ToUpper(vo.ThiDinhName[0]) + vo.ThiDinhName.Substring(1);
            }
            else if (tourModel.IsThiHoi || tourModel.IsThiHuong)
            {
                txtLocation.text = nameRoom + " - Kỳ " + (vo.lastRound + 1);
            }
        }
    }

    private void OnTourVOChange(TourVO obj)
    {
        SetLocation();
    }

    private void OnBoardJoined()
    {
        gamePlayModel.game = sfs.lastJoinedRoom;
        gamePlayModel.zoneInfo = screenManager.zoneInfo;
        gamePlayModel.InitGameState();
        gamePlayLogic.updatePlayers();
        gamePlayModel.UpdateNuoiGa();
        // Vì có thể Board Mediator chưa được Register Event, cần chờ 1 Frame mới cập nhật lại boardInfo
        this.WaitNewFrame(() => { syncBoardConfigSignal.Dispatch(); });

        //Nếu mình là thằng vào thứ 2
        if (gamePlayModel.minScoreU == 3)
            showTimeOutMsgSignal.Dispatch(SDMsg.Join(AppMsg.NOTUBELOW, gamePlayModel.minScoreU));
        else if (gamePlayModel.minScoreU > 3)
            showTimeOutMsgSignal.Dispatch(SDMsg.Join(AppMsg.NOTUBELOW, gamePlayModel.minScoreU));
        UpdateXinChoi();
        SetLocation();
        // SetUpForChallenge();
    }

    // private void SetUpForChallenge()
    // {
    //     this.WaitNewFrame(() =>
    //     {
    //         if (screenManager.InChallenge() && !gamePlayModel.isPlaying)
    //         {
    //             ChallengeInfoPopupMediator.Open(gamePlayModel.ChallengeRoomID.ToString());
    //             RequestChallengeLink(gamePlayModel.ChallengeRoomID);
    //         }
    //     });

    //     if (screenManager.InChallenge())
    //     {
    //         ReInviteChallenge();
    //         ChallengeModel.Instance.UpdateGameInfo(gamePlayModel);
    //     }
    // }
    
    // private void RequestChallengeLink(long challengeId)
    // {
    //     var model = GamePlayModel.Instance;
    //     var roomInfo = new JoinAbleChallengeInvite(long.Parse(UserModel.Instance.uid), challengeId,
    //         UserModel.Instance.name, model.stake, model.GaGop, model.minScoreU);
    //     Api.GetChallengeLink(GameUtils.Base64Encode(roomInfo.toSFSObject().ToJson()), o =>
    //     {
    //         if ((string) o["status"] == "OK")
    //         {
    //             var link = o["challengeLink"].ToString();
    //             ChallengeModel.Instance.UpdateChallengeLink(link);
    //         }
    //         else
    //         {
    //             Debug.LogError("Create Share Link Fail");
    //         }
    //     }, s =>
    //     {
    //         Debug.LogError("Create Share Link Fail");
    //     });
    // }

    // private void ReInviteChallenge()
    // {
    //     ChallengeModel.Instance.ReChallengeInvite();
    // }

    /**Gọi khi mình mới vào bàn hoặc khi đang WAITING & có thằng vào bàn hoặc khi reset lại bàn lúc stop game*/
    private void OnPlayerJoined()
    {
        UpdatePlayerInBoard();
        UpdateBtnStartVisibility();
        if (askJoinVO == null && gamePlayModel.isPlayer && !screenManager.inTour() && !screenManager.InChallenge()) sfs.SendExt(ExtCmd.UserInBoard);
        UpdateXinChoi();
        UpdateStartCountdown();
    }

    private void OnBoardStateChanged(bool isPlaying)
    {
        SDLogger.Log("OnBoardStateChanged: " + isPlaying + " - " + gamePlayModel.status);
        if (isPlaying)
        {
            OnDealCard();
        }
        else
        {
            stopGameSignal.Dispatch();
        }
    }

    private Coroutine waitKickOutInTour;

    private void OnStopGame()
    {
        if (!screenManager.inTour() || !screenManager.InChallenge())
        {
            if (isWaitforComfirmMouseIdle) OnKickOutBoard();
            isWaitforComfirmMouseIdle = false;
        }

        timer.StopTimer();
        if (gamePlayModel.game.PlayerList.Count >= GameConfig.MIN_PLAYER_TO_START_GAME) UpdateStartCountdown();

        //@see StopGameSignal, SumUpCommand
        if (currentDealCards) Destroy(currentDealCards.gameObject);
        if (!screenManager.InChallenge() && !screenManager.inTour())
        {
            isCanShowInviteBtn = true;
        }
        canUCardsModel.ReInit();
        playModel.ReInit();
        uModel.ReInit();
        //fix trường hợp chơi 2, thằng kia báo, mình leave rồi vào lại luôn
        //khiến cho clock vẫn chạy & ở bên trên OnBoardActionsView
        clock.StopCountDown();
        //Note: Nhiều thằng gửi b.stop. Trên server sẽ check nếu đã stop thì k làm gì
        //reiniting boardModel.sdplayers & updateBtnStartVisibility
        gamePlayLogic.updatePlayers();
        Signals.Get<OnSetUpCaptureShareScreenShot>().Dispatch(false, null);
    }

    private void ReInitOnDestroy()
    {
        canUCardsModel.ReInit();
        playModel.ReInit();
        uModel.ReInit();
        clock.StopCountDown();
        Signals.Get<OnSetUpCaptureShareScreenShot>().Dispatch(false, null);
        Signals.Get<OnLeaveBoardGame>().Dispatch();
    }

    private void OnKickOutUserAfterSumUp()
    {
        if (screenManager.inTour() || screenManager.InChallenge())
        {
            if (isWaitforComfirmMouseIdle) OnKickOutBoard();
            isWaitforComfirmMouseIdle = false;
        }
    }

    private void OnPlayerLeaveBoard(SDPlayer pl)
    {
        var pIdx = -1;
        var idx = -1;
        foreach (var pv in playerViews)
        {
            if (pv.sdPlayer?.uid != pl.uid) continue;
            pIdx = playerViews.IndexOf(pv);
            break;
        }

        if (pIdx != -1)
        {
            idx = playerViews[pIdx].sdPlayer.idx;
        }

        UpdatePlayerInBoard();
        UpdateBtnStartVisibility();
        UpdateStartCountdown();
        UpdateXinChoi();
        // logic đếm giờ chia bài ở chủ bàn
        if (gamePlayModel.status != BoardStatus.WAITING)
            return;
        //Nếu chỉ còn mình mình thì stop đếm giờ
        if (gamePlayModel.game.PlayerList.Count < GameConfig.MIN_PLAYER_TO_START_GAME)
            OnHideTimeCounter();
        else if (idx == 0) //Nếu chủ bàn leave thì start đếm giờ chia bài ở chủ bàn mới
        {
            ShowBoardStartCounter();
        }
    }

    public void OnStartGame()
    {
        btnInvite.Hide();
        isCanShowInviteBtn = false;
        if (!gamePlayModel.isBoardOwner || gamePlayModel.isPlaying) return;
        if (gamePlayModel.tinhDiem)
        {
            var scoreVO = gamePlayModel.GetScoreVO();
            if (scoreVO != null && scoreVO.uids.Count > 0)
            {
                //không có ai là người mới, và có ít nhất 1 user tham gia ván trước nhưng không tham gia ván này
                if (!HasNewUser(scoreVO) && HasPlayerNotContinue())
                {
                    // show popup confirm start
                    confirmDealCard.Show();
                    confirmDealCard.AddCallback(ok =>
                    {
                        if (ok)
                        {
                            btnStart.Hide();
                            OnHideTimeCounter();
                        }
                        else UpdateBtnStartVisibility();
                    });
                    return;
                }

                if (HasNewUser(scoreVO))
                {
                    // có người mới tham gia
                    BasicPopup.Open(
                        "Thông Báo",
                        SDMsg.ScoreHisWarning,
                        "Đóng", UpdateBtnStartVisibility,
                        "Chia bài", () =>
                        {
                            sfs.SendExt(ExtCmd.Start);
                            btnStart.Hide();
                            OnHideTimeCounter();
                        });
                    return;
                }

                sfs.SendExt(ExtCmd.Start);
            }
            else
            {
                sfs.SendExt(ExtCmd.Start);
            }
        }
        else
        {
            sfs.SendExt(ExtCmd.Start);
        }

        // if (screenManager.InChallenge())
        // {
        //     FirebaseAnalyticsExtension.Instance.LogEvent(FirebaseEvent.Challenge_Start);
        // }
        
        btnStart.Hide();
        OnHideTimeCounter();
    }

    private bool HasNewUser(ScoreVO scoreVO)
    {
        foreach (var pl in gamePlayModel.sdplayers)
        {
            if (scoreVO.uids.IndexOf(int.Parse(pl.uid)) == -1) return true;
        }

        return false;
    }

    private bool HasPlayerNotContinue()
    {
        if (gamePlayModel.gameNo < 1) return false;
        var prevUids = gamePlayModel.getPrevPlayers;
        if (prevUids == null) return false;

        var numPlExit = 0;
        foreach (var pl in gamePlayModel.sdplayers)
        {
            foreach (var uid in prevUids)
            {
                if (pl.uid == uid.ToString())
                {
                    numPlExit++;
                    break;
                }
            }
        }

        return numPlExit != prevUids.Count;
    }

    private void UpdatePlayerInBoard()
    {
        foreach (var view in playerViews)
        {
            view.Hide();
            view.ResetUser();
        }

        foreach (var player in gamePlayModel.sdplayers)
        {
            if (player.seat >= playerViews.Count) continue;
            playerViews[player.seat].Show();
            playerViews[player.seat].UpdateUser(player);
            // SDLogger.Log("UpdatePlayerInBoard: " + player.name + " - " + player.seat);
        }
    }

    private void UpdateBtnStartVisibility()
    {
        // if (screenManager.InChallenge())
        // {
        //     var x = gamePlayModel.isBoardOwner && gamePlayModel.game.PlayerList.Count >=
        //        gamePlayModel.MinPlayerToStartChallenge;
        //     btnStart.gameObject.SetActive(x);
        // }
        // else
        // {
        //     var x = !screenManager.inTour() && gamePlayModel.isBoardOwner
        //                                     && gamePlayModel.game.PlayerList.Count >=
        //                                     GameConfig.MIN_PLAYER_TO_START_GAME;
        //     btnStart.gameObject.SetActive(x);
        // }
    }

    private void ShowBoardStartCounter()
    {
        if (screenManager.inTour()) return;
        ShowCountDown(gamePlayModel.isBoardOwner
            ? new ShowTimeCounterVO(0, SDTimeout.START_GAME, OnBoardStartCounterTimeOut)
            : new ShowTimeCounterVO(0, SDTimeout.START_GAME, null));
    }

    private void OnBoardStartCounterTimeOut()
    {
        screenManager.GoEntrance();
        BasicPopup.Open("Thông Báo", AppMsg.KICKNOTCHIABAI);
    }

    // private float timeLeftStartGame = 0;

    private void UpdateStartCountdown()
    {
        if (gamePlayModel.status != BoardStatus.WAITING) return;
        if (!screenManager.InChallenge())
        {
            if (gamePlayModel.game.PlayerList.Count >= GameConfig.MIN_PLAYER_TO_START_GAME)
            {
                if (!txtCountDown.isOnCountDown)
                {
                    ShowBoardStartCounter();
                }
            }
        }
        else
        {
            if (gamePlayModel.game.PlayerList.Count >= GameConfig.MAX_PLAYER_IN_GAME)
            {
                if (!txtCountDown.isOnCountDown)
                {
                    ShowBoardStartCounter();
                }
            }
        }
        
    }

    private void OnReceiveBoardStartCounter(int timeLeft)
    {
        txtCountDown.Hide();
        if (!screenManager.InChallenge())
        {
            ShowCountDown(new ShowTimeCounterVO(0, timeLeft, null));
        }
        else
        {
            if (gamePlayModel.game.PlayerList.Count >= GameConfig.MAX_PLAYER_IN_GAME)
            {
                ShowCountDown(new ShowTimeCounterVO(0, timeLeft, null));
            }
        }
    }

    public void OnHideTimeCounter()
    {
        txtCountDown.Hide();
    }

    public void OnShowChoChoi()
    {
        choChoiPopup.Show();
    }

    private void OnShowXuongPanel()
    {
        xuongMediator.Show();
    }

    public float timeResetTxtU = 8;
    public float tShowSumUp = 2;

    private void OnShowTongKet(string msg, string cuocsMsg)
    {
        xuongMediator.Hide();
        cuocViewMediator.ShowCuocU(cuocsMsg);
        // nếu không có cước đặc biệt nào
        sumupMediator.ShowMsg(msg);
        Invoke(nameof(OnShowSumUp), tShowSumUp);
    }

    private void OnShowSumUp()
    {
        // if (!GameModel.Instance.IsNormalPlayer()) return;
        sumupMediator.Show();
    }

    private void ResetTextU()
    {
        cuocViewMediator.Hide();
        // txt_u_upper.text = "";
        // txt_u_upper.Hide();
    }

    private void ShowCountDown(ShowTimeCounterVO vo)
    {
        // OnHideTimeCounter();
        var idxTimer = 0;
        for (var i = 0; i < playerViews.Count; i++)
        {
            if (playerViews[i].sdPlayer == null || gamePlayModel.sdplayers.Count == 0) continue;
            if (!gamePlayModel.sdplayers[vo.uIdx].uid.Equals(playerViews[i].sdPlayer.uid)) continue;
            idxTimer = i;
            break;
        }

        txtCountDown.Hide();
        // Đợi thêm 1 frame cho generate hết rồi mới hiển thị ra
        this.WaitNewFrame(() =>
        {
            txtCountDown.transform.position = playerViews[idxTimer].tfCountdown.position;
            txtCountDown.Show();
            if (vo.autoFunc == null)
            {
                txtCountDown.ShowCountDown(vo.timeLeft);
            }
            else
            {
                txtCountDown.ShowCountDownWithCallback(vo.timeLeft, vo.autoFunc);
            }
        });
        // if (txtCountDown.gameObject.activeInHierarchy) return;
    }

    private void ShowMsgIdle()
    {
        isWaitforComfirmMouseIdle = true;
        BasicPopup.Open("Thông Báo", "Bạn đã không thao tác trong 2 phút.\nHãy ấn <b>Xác Nhận</b> để tiếp tục Chơi.\n" +
                                     "Nếu không Xác Nhận, bạn sẽ tự động rời bàn \nkhi ván chơi kết thúc.", "Xác Nhận",
            OnConfirmToServer);
    }

    private void OnConfirm()
    {
        isWaitforComfirmMouseIdle = false;
        timer.ResetTimer();
        timer.StartTimer();
    }

    private void OnConfirmToServer()
    {
        isWaitforComfirmMouseIdle = false;
        timer.ResetTimer();
        timer.StartTimer();
        sfs.SendExt(ExtCmd.Ping);
    }

    private void OnKickOutBoard()
    {
        // FirebaseAnalyticsExtension.Instance.GetKicked();
        //FirebaseAnalyticsExtension.Instance.LogEvent(FirebaseEvent.GetKicked);
        BasicPopup.Update("Thông Báo", "Bạn bị đá khỏi bàn vì ngồi quá lâu mà không thao tác.");
        isWaitforComfirmMouseIdle = false;
        screenManager.OnLeave();
    }

    private void Update()
    {
        if (gamePlayModel.isPlayer && gamePlayModel.status == BoardStatus.PLAYING &&
            (Input.anyKeyDown || Input.touchCount > 0)) OnConfirm();

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     SetUpOnUserPause();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     SetUpOnUserUnPause();
        // }

        // Make sure user is on Android platform
        if (Application.platform != RuntimePlatform.Android) return;

        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DOVirtual.DelayedCall(.2f, () =>
            {
                BasicPopup.Open("Thông báo", "Bạn có muốn thoát khỏi trò chơi?", "Đồng ý", () =>
                {
                    // Quit the application
                    Application.Quit();
                });
            });
        }
    }

    public List<PlayerMediator> GetPlayers()
    {
        return playerViews;
    }

    public PlayerMediator GetPlayer(int idx)
    {
        return playerViews[idx];
    }

    // public ShareGameButton shareGameBtn;

    // [Button]
    // private void TestShareBtn()
    // {
    //     var ls = new List<int>();
    //     for (int i = 0; i < 23; i++)
    //     {
    //         ls.Add(i == ULogic.BACH_THU_CHI || i == ULogic.TAM_DO ? 1 : 0);
    //     }

    //     OnShowShareGameButton(SharePopupType.FinishMatch, "l1r10p4x_5t_ib");
    // }

    // private void OnShowShareGameButton(SharePopupType type, string matchId)
    // {
    //     if (type == SharePopupType.FinishMatch)
    //     {
    //         this.SetTimeout(4, () => shareGameBtn.OnShow(matchId));
    //     }
    // }

    private void HandleVoteKick(SFSObject data)
    {
        KickVO kickVo = KickVO.ParseFromSFSObject(data);
        SDPlayer target = gamePlayModel.GetPlayer(kickVo.TargetUid);
        if (target != null)
        {
            KickPopupMediator.Open(new UserContainerVO(target.uid, target.name, ""),
                gamePlayModel.GetPlayer(kickVo.RequesterUid));
        }
    }

    public void OpenInvitePopup()
    {
        if (!screenManager.InChallenge())
        {
            InvitePlayerPopupMediator.Open();
        }
        else
        {
            ChallengeInfoPopupMediator.Open(gamePlayModel.ChallengeRoomID.ToString());
        }
    }
}