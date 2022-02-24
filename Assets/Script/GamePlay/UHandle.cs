using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UHandle
{
    private PlayModel playModel = PlayModel.Instance;
    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
    private ULogic uLogic = ULogic.Instance;
    private UModel uModel = UModel.Instance;
    private CanUCardsModel canUCardsModel = CanUCardsModel.Instance;

    private SumUpSignal sumUpSignal = Signals.Get<SumUpSignal>();
    private ShowTimeOutMsgSignal showTimeOutMsgSignal = Signals.Get<ShowTimeOutMsgSignal>();
    private StopPlaySignal stopPlaySignal = Signals.Get<StopPlaySignal>();

    public void Execute(UVO uvo)
    {
        Debug.Log("  sdPlayerCount: " + gamePlayModel.sdplayers.Count + ",  acsCount: " +
                  playModel.acts.Count + "  aIdx: " + uvo.aIdx + "  uidx: " + uvo.uIdx);
        if (gamePlayModel.status != BoardStatus.WAIT_U)
            stopPlaySignal.Dispatch();
        uModel.uvos.Add(uvo);
        var needShowMsg = uvo.uIdx != gamePlayModel.myIdx || GamePlayModel.IsReplay;
        Clock.Instance.StopCountdown();

        if (uvo.isThienU)
        {
            {
                if (needShowMsg)
                    showTimeOutMsgSignal.Dispatch(SDMsg.Join(AppMsg.DAU, gamePlayModel.sdplayers[uvo.uIdx].name));
            }
            sumUpSignal.Dispatch(uvo);
        }
        else
        {
            if (needShowMsg)
            {
                try
                {
                    SDLogger.Log(SDMsg.Join(AppMsg.DAUPLSWAIT, gamePlayModel.sdplayers[uvo.uIdx].name,
                        playModel.acts[uvo.aIdx].cardInst.n));
                    showTimeOutMsgSignal.Dispatch(SDMsg.Join(AppMsg.DAUPLSWAIT, gamePlayModel.sdplayers[uvo.uIdx].name,
                        playModel.acts[uvo.aIdx].cardInst.n));
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "  sdPlayerCount: " + gamePlayModel.sdplayers.Count + ",  acsCount: " +
                                   playModel.acts.Count + "  aIdx: " + uvo.aIdx + "  uidx: " + uvo.uIdx);
                }
            }

            if (uvo.aIdx > 0) //Trường hợp chíu địa ù quân đầu
                uLogic.checkBoUToActIdx(uvo.uIdx, uvo.aIdx);
            canUCardsModel.checkSumUp();
        }
    }
}