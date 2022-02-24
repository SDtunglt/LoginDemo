using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PlayHandle
{
    private PlayModel playModel = PlayModel.Instance;
    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
    private ULogic uLogic = ULogic.Instance;
    private UModel uModel = UModel.Instance;
    private CanUCardsModel canUCardsModel = CanUCardsModel.Instance;
    private PlayLogic playLogic = PlayLogic.Instance;
    private NocModel nocModel = NocModel.Instance;
    private Clock clock;

    private ShowTimeOutMsgSignal showTimeOutMsgSignal = Signals.Get<ShowTimeOutMsgSignal>();
    private StopPlaySignal stopPlaySignal = Signals.Get<StopPlaySignal>();
    private RemoveCardsFromMyCardsSignal removeCardsFromMyCardsSignal = Signals.Get<RemoveCardsFromMyCardsSignal>();
    private PlayReceiveSignal playReceiveSignal = Signals.Get<PlayReceiveSignal>();

    public void Execute(PlayVO vo)
    {
        clock = Clock.Instance;
        playReceiveSignal.Dispatch(vo);
        //@see PlayLogic.sendPlay
        // TweenLite.killDelayedCallsTo(sfs.SendExt);
        //@see ClockTimeOutCommand
        //fixes #170
        //Other note:
        //Khi thằng A - đã báo or disconnected- đến lượt, có con được bốc/đánh/trả
        //(autoPlayIfBaoOrDis được gọi) mà lại có thằng khác có thể chíu
        //thì chỉ autoPlay cho thằng A sau khi đã hết thời gian chíu
        //Trong khoảng thời gian đó, nếu có thằng chíu thì phải hủy lệnh autoPlay này đi
        //Note khi có thằng ù thì cũng cần hủy lệnh (& stop clock)
        playLogic.delaySendExtCmd?.Kill();

        playModel.curActIdx = vo.actionIndex + 1; // set client action index to the NEW server action index
        if (vo.vaoGa != null) ShowVaoGaMsg(vo);
        
        var p = gamePlayModel.sdplayers[vo.uIdx];
        p.isPlayed = true;
        p.CheckIllegalPlay(vo);
        if (vo.type == PlayVO.CHIU) playModel.updateChiuWhenDraw();
        if (playModel.acts.Count == 0)
        {
            uLogic.checkBoThienU(p);
        }
        else
        {
            uLogic.checkBoUToActIdx(vo.uIdx, playModel.acts.Count);
        }
        
        if (vo.uIdx == gamePlayModel.myIdx) canUCardsModel.CleanCanUCards();
        if (vo.type == PlayVO.TRACUA)
        {
            var prevAct = playModel.acts[playModel.acts.Count - 2];
            vo.door = prevAct.type == PlayVO.TRACUA ? prevAct.door : prevAct.uIdx;
        }
        
        playModel.acts.Add(vo);
        playModel.curAct = vo;
        p.CheckChangeLawLists(vo);
        if (gamePlayModel.isNuoiGa)
        {
            playLogic.checkChangeNhaiGa(vo);
        }
        var isCurCardChanged = playModel.isCurActCauseChangeCurCard();
        if (isCurCardChanged) ChangeCurCard();
        PlayerPlay(vo.uIdx, vo);
        playModel.checkChangeCurTurn();
        if (vo.uIdx == gamePlayModel.myIdx && gamePlayModel.myPlayer.bao)
        {
            showTimeOutMsgSignal.Dispatch("");
            showTimeOutMsgSignal.Dispatch(SDMsg.Join(AppMsg.BAOTHENCANTPLAY, "Bạn", gamePlayModel.myPlayer.err.msg));
        }
        
        if (isCurCardChanged)
        {
            canUCardsModel.startCheckSumUpTimer();
            if (playModel.curAct.type == PlayVO.DRAW || playModel.curAct.uIdx != gamePlayModel.myIdx)
            {
                canUCardsModel.AddCard(playModel.curCard, vo.actionIndex);
            }
        }
        
        if (nocModel.isEndInDraw())
        {
            stopPlaySignal.Dispatch();
            showTimeOutMsgSignal.Dispatch(AppMsg.NOCEMPTY);
            canUCardsModel.checkSumUp();
        }
        
        if (gamePlayModel.status == BoardStatus.PLAYING) //!nocModel.isEndInDraw() && !uModel.havePlayerU
        {
            ChangeClock();
        }
        else if(gamePlayModel.status == BoardStatus.WAIT_U)
        {
            clock.StopCountDown();
        }
    }

    private void ShowVaoGaMsg(PlayVO act)
    {
        var vo = act.vaoGa;
        var msg = "";
        foreach (var p in gamePlayModel.sdplayers)
            if (vo.ids.IndexOf(p.uid) != -1)
                msg += ", " + p.name;
        if (msg.Length > 2) msg = msg.Substring(2); //remove ", "
        if (vo.ids.Count > 1)
        {
            msg += " - mỗi người vào gà ";
        }
        else
        {
            msg += " vào gà ";
        }

        msg = (act.type == PlayVO.CHIU ? "Chíu: " : "Nhái: ") + msg;
        msg += vo.score + " điểm";
        showTimeOutMsgSignal.Dispatch(msg);
    }

    /**migrate from Player.play*/
    private void PlayerPlay(int uIdx, PlayVO act)
    {
        var curCard = playModel.curCard;
        var p = gamePlayModel.sdplayers[uIdx];
        // var pView = GamePlayMediator.Instance.playerViews[p.seat];
        List<SDCard> onHandCards;
        switch (act.type)
        {
            case (int) PlayKey.DRAW:
                LayoutChieuMediator.Instance.AddCardFromNocToDanhArea(curCard, p.seat, true);
                break;
            case (int) PlayKey.DANH:
            case (int) PlayKey.TRACUA:
                onHandCards = new List<SDCard> {curCard};

                if (GamePlayModel.IsReplay)
                {
                    Signals.Get<RemoveCardsFromCardsInHandSignal>().Dispatch(p.seat, onHandCards);
                }
                else
                {
                    if (uIdx == gamePlayModel.myIdx)
                    {
                        removeCardsFromMyCardsSignal.Dispatch(onHandCards);
                    }
                    else
                    {
                        p.RemoveCards(onHandCards);
                    }
                }

                if (act.type == PlayVO.DANH)
                {
                    LayoutChieuMediator.Instance.AddCardToDanhArea(curCard, p.seat);
                }
                else
                {
                    LayoutChieuMediator.Instance.AddCardToTraCuaArea(curCard, gamePlayModel.Seat(act.uIdx), gamePlayModel.Seat(act.door));
                    // pView.playerViews[act.door].rightHandZone.addCard(curCard, !boardModel.resuming);
                }

                p.ReinitChanCaLists();
                break;
            case (int) PlayKey.EAT:
                var card = p.GetCardByValue(act.card);
                if(card == null) return;
                onHandCards = new List<SDCard> {card};

                if (GamePlayModel.IsReplay)
                {
                    Signals.Get<RemoveCardsFromCardsInHandSignal>().Dispatch(p.seat, onHandCards);
                }
                else
                {
                    if (uIdx == gamePlayModel.myIdx)
                    {
                        removeCardsFromMyCardsSignal.Dispatch(onHandCards);
                    }
                    else
                    {
                        p.RemoveCards(onHandCards);
                    }
                }
                LayoutChieuMediator.Instance.AddCardToAnArea(onHandCards, p.seat, 60f);

                // pView.eatenZone.eatCard(onHandCards, curCard, !boardModel.resuming);

                p.ReinitChanCaLists();
                break;
            case (int) PlayKey.CHIU:
                onHandCards = p.GetAllCardsByValue(curCard.v);
                
                if (GamePlayModel.IsReplay)
                {
                    Signals.Get<RemoveCardsFromCardsInHandSignal>().Dispatch(p.seat, onHandCards);
                }
                else
                {
                    if (uIdx == gamePlayModel.myIdx)
                    {
                        removeCardsFromMyCardsSignal.Dispatch(onHandCards);
                    }
                    else
                    {
                        p.RemoveCards(onHandCards);
                    }
                }

                // pView.eatenZone.eatCard(onHandCards, curCard, !boardModel.resuming);
                LayoutChieuMediator.Instance.AddCardToAnArea(onHandCards, p.seat, 25f);


                p.ReinitChanCaLists();
                break;
            case (int) PlayKey.DUOI:
                //do nothing
                break;
        }
    }

    /**curCard change ONLY when receive playAction (from fms) bốc, đánh, trả cửa
	 * @see CanUCard.addTo(c: Card)*/
    private void ChangeCurCard()
    {
        if (playModel.curAct.type == PlayVO.DRAW)
            playModel.curCard = nocModel.draw();
        else
        {
            playModel.curCard = gamePlayModel.sdplayers[playModel.curAct.uIdx]
                .GetCardByValue(playModel.curAct.card);
            
        }
        // SDLogger.Log("Current Card: " + playModel.curCard.n);
        playModel.curAct.cardInst = playModel.curCard;
        if(playModel.curCard == null) return;
        uModel.cardToPlayActIdxDic[playModel.curCard] = playModel.acts.Count - 1;
    }

    /**startClock ở vị trí, thời gian thích hợp tùy theo curAct
	 * Note: clock là đếm cho thằng có quyền thực hiện playAction tiếp theo
	 * 		 nên checkChangeCurTurn rồi mới check to start clock
	 * @see onPlayAction*/
    private void ChangeClock()
    {
        //Nếu current card changed, k phải do mình bốc, đánh/ trả cửa
        //mà mình có thể chíu thì start clock chíu tại vị trí của mình
        //Tuy nhiên, nếu isMyTurn (sau khi đã checkChangeCurTurn) lại cũng chính là mình
        //Thì vẫn start clock as normal
        if (gamePlayModel.isPlayer &&
            playModel.isCurActCauseChangeCurCard() && playModel.curAct.uIdx != gamePlayModel.myIdx
            && !playModel.isMyTurn && gamePlayModel.myPlayer.CanChiu(playModel.curCard.v))
        {
            //if canChiu, but myPlayer in turn => startClock as normal
            //@see McTimer.startCount
            //the real time left is boardModel.chiuTime - 1
            clock.StartClock(Clock.COUNT_FOR_MYPLAYER_CHIU, gamePlayModel.chiuTime + 1, gamePlayModel.myPlayer);
        }
        else
        {
            var p = gamePlayModel.sdplayers[playModel.getCurRealTurn()];
            var timeLeft = playLogic.GetClockTimeLeft(p);
            clock.StartClock(Clock.COUNT_TURN, timeLeft, p);
        }
    }

    public enum PlayKey
    {
        DRAW = 0,
        EAT = 1,
        CHIU = 2,
        DUOI = 3,
        DANH = 4,
        TRACUA = 5
    }
}