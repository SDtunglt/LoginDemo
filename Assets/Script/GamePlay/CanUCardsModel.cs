using System.Collections.Generic;
using UnityEngine;

public class CanUCardsModel : Singleton<CanUCardsModel>
{
    private GamePlayModel gamePlayModel = GamePlayModel.Instance;

    private CanUCardAddedSignal canUCardAddedSignal = Signals.Get<CanUCardAddedSignal>();
    private CanUCardRemovedSignal canUCardRemovedSignal = Signals.Get<CanUCardRemovedSignal>();
    private CheckSumUpSignal checkSumUpSignal = Signals.Get<CheckSumUpSignal>();

    public List<SDCard> canUCards = new List<SDCard>();

    /** mình đã chọn Ù (không dispatch canUCardAddedSignal nữa) */
    public bool isBtnUClicked;
    /* Khi biết ván sẽ kết thúc (hòa/ có thằng ù) cho đến khi tổng kết ván cần 1 khoảng thời gian
     * chờ xem có những ai ù rồi mới biết thằng nào đè thằng nào để mà tổng kết.
     * Biến này là timer để check sau U_TIME + 1.5s kể từ khi quân "có thể ù" cuối cùng xuất hiện
     * thì sẽ tổng kết ván */
    
    private SDTimer timer;

    /* init timer: Nếu ở khu Tự Xướng thì chỉ cần delay ~4s thôi
    * vì không cần chờ đến quân "có thể ù" cuối cùng bị removed (do ở khu Tự Xướng tự ù)
    * Hàm này chỉ được gọi 1 lần*/

    public void sdInit()
    {
        var delay = SDTimeout.U_NORMAL + SDTimeout.DELAY;
        SDTimerController.Ins.RemoveTimer(timer);
        timer = new SDTimer(delay);
    }

    public void AddCard(SDCard c, int actionIndex)
    {
        if (isBtnUClicked)
            return;
        if (gamePlayModel.resuming && !gamePlayModel.isPlayer) return;

        List<CardMediator> listCardMediator = LayoutChieuMediator.Instance.lisCardMediator;
        var lastIdx = listCardMediator.Count - 1;
        listCardMediator[lastIdx].StartClock(actionIndex);

        canUCards.Add(c);
        canUCardAddedSignal.Dispatch();
    }

    /**Được gọi khi thằng này thực hiện 1 play action
	 * (đã play thì canUCards bị reset, k thể ù những quân trước play action)
	 * Clean tức là:
	 * 	1. remove các references (TweenLite, eventListeners,..)
	 * 	2. check bỏ ù
	 * 	3. remove all item from canUCards*/
    public void CleanCanUCards()
    {
        while (canUCards.Count > 0)
        {
            SDCard sdCard = canUCards.Pop();
            canUCardRemovedSignal.Dispatch(sdCard);
        }

        List<CardMediator> listCardMediator = LayoutChieuMediator.Instance.lisCardMediator;
        foreach (CardMediator cardMediator in listCardMediator)
        {
            cardMediator.StopClock();
        }
    }

    public void removeCanUCard(SDCard c)
    {
        var i = canUCards.IndexOf(c);
        if (i == -1) return;
        canUCards.Splice(i, 1);
        canUCardRemovedSignal.Dispatch(c);
    }

    public void ReInit()
    {
        //khi prepareForNewGame (gọi hàm này) thì thực ra canUCards đã empty
        canUCards.Splice(0, canUCards.Count);
        isBtnUClicked = false;
        timer.StopTimer();
    }

    public void checkSumUp()
    {
        if (timer.running)
            timer.AddEvent(OnSumUpTimer);
        else
            checkSumUpSignal.Dispatch();
    }

    public void startCheckSumUpTimer()
    {
        timer.ResetTimer();
        timer.StartTimer();
    }

    public void stopCheckSumUpTimer()
    {
        timer.StopTimer();
    }

    /* Hàm này được gọi 2s sau khi canUCards == []
	 * && (mcNoc.isEndInDraw || nhận đc thông tin có thằng ù (có thể chính là mình))
	 * @see McOnBoardActions.onU
	 * @see CanUCard.removeFromParentCard*/
    private void OnSumUpTimer()
    {
        checkSumUpSignal.Dispatch();
    }
}