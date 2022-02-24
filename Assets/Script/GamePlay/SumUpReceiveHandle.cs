using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sumup Receive From Server
public class SumUpReceiveHandle
{
    private CanUCardsModel canUCardsModel = CanUCardsModel.Instance;
    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
    private SumupReceivedSignal sumupReceivedSignal = Signals.Get<SumupReceivedSignal>();
    private HideTimeCounterSignal hideTimeCounterSignal = Signals.Get<HideTimeCounterSignal>();
    private ReceiveChatMsgSignal receiveChatMsgSignal = Signals.Get<ReceiveChatMsgSignal>();

    public void Execute(SumUpVO vo)
    {
        canUCardsModel.stopCheckSumUpTimer();
        hideTimeCounterSignal.Dispatch();
        sumupReceivedSignal.Dispatch();

        string txtXuongChinh = "", txtXuongPhu = "";
        if (vo.type == SumUpVO.XUONG_DUNG)
        {
            txtXuongChinh = ULogic.getCuocsStr(vo.cuocHos);
        }

        Signals.Get<ShowSumupSignal>().Dispatch(OnMsgSumUp(vo), txtXuongChinh);
        var msg = ShowResultOnChat(vo, gamePlayModel.sdplayers);
        receiveChatMsgSignal.Dispatch(msg, false, true, false);

        Signals.Get<ShowTimeOutMsgSignal>().Dispatch("");
        gamePlayModel.status =
            BoardStatus.SUM_UPPED; //khi nhận được kết quả của ván thì khi thằng ù thoát ko gửi Sumup Req lên server nữa
        Signals.Get<KickOutTourUserAfterSumUpSignal>().Dispatch();
        Signals.Get<OnSetUpCaptureShareScreenShot>().Dispatch(true, vo);
    }

    private string OnMsgSumUp(SumUpVO vo)
    {
        var ps = gamePlayModel.sdplayers;
        var htmlTxt = SumUpVO.getSumUpMsg(vo, GamePlayModel.Instance) + "\n";
        if (vo.type == SumUpVO.XUONG_DUNG && vo.anGa > 0)
            htmlTxt += "Ăn gà nuôi <b>" + StringUtils.FormatMoney(vo.anGa) + " Bảo" + "</b>\n";
        if (vo.changedCoins != null)
        {
            var hasUserChangedCoin = false;
            int i;
            for (i = 0; i < ps.Count; i++)
                if (vo.changedCoins[i] != 0)
                {
                    hasUserChangedCoin = true;
                    break;
                }

            if (hasUserChangedCoin)
            {
                for (i = 0; i < ps.Count; i++)
                {
                    var coin = StringUtils.FormatMoney(vo.changedCoins[i]) + " Bảo\n";
                    htmlTxt += ps[i].name + ": " +
                               (vo.changedCoins[i] >= 0 ? "<b>+" + coin + "</b>" : coin);
                }

                if (vo.disNames != null)
                {
                    for (var j = 0; j < vo.disNames.Count; j++)
                    {
                        htmlTxt += vo.disNames[j] + ": " + StringUtils.FormatMoney(vo.changedCoins[i + j]) +
                                   " Bảo\n";
                    }
                }
            }
        }

        return htmlTxt;
    }

    private static string ShowResultOnChat(SumUpVO vo, List<SDPlayer> ps)
    {
        string msg;
        if (vo.type == SumUpVO.DRAW || vo.type == SumUpVO.ALL_BAO_DIS) msg = "Ván hòa";
        else
        {
            msg = ps[vo.idxU].name;
            switch (vo.type)
            {
                case SumUp.XUONG_DUNG:
                    msg += ULogic.getCuocsStr(vo.cuocHos) == "Thoát trước khi xướng"
                        ? " " + ULogic.getCuocsStr(vo.cuocHos)
                        : " ù " + ULogic.getCuocsStr(vo.cuocHos);
                    break;
                case SumUp.TREO_TRANH:
                    msg += " treo tranh";
                    break;
                case SumUp.U_THIEU_DIEM:
                    msg += " ù thiếu điểm";
                    break;
                case SumUp.BO_U:
                    msg += " bỏ ù";
                    break;
                case SumUp.U_LAO:
                    msg += " ù láo";
                    break;
                case SumUp.U_BAO:
                    msg += " ù báo";
                    break;
                case SumUp.XUONG_SAI:
                    msg += " xướng sai";
                    break;
                case SumUp.XUONG_THIEU_DIEM:
                    msg += " xướng thiếu điểm";
                    break;
            }
        }

        return msg;
    }

    private class SumUp
    {
        public const int XUONG_DUNG = 9,
            TREO_TRANH = 8,
            U_THIEU_DIEM = 3,
            BO_U = 6,
            U_LAO = 2,
            U_BAO = 5,
            XUONG_SAI = 7,
            XUONG_THIEU_DIEM = 4;
    }
}