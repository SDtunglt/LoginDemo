using System.Collections.Generic;
using UnityEngine;

public class ChiaBaiHandle
{
    private ScreenManager screenManager;
    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
    private ChiaBaiModel chiaBaiModel = ChiaBaiModel.Instance;
    private NocModel nocModel = NocModel.Instance;
    private PlayModel playModel = PlayModel.Instance;
    private TourModel tourModel = TourModel.Instance;

    private BoardStateChangedSignal boardStateChangedSignal = Signals.Get<BoardStateChangedSignal>();

    public ChiaBaiHandle()
    {
        screenManager = ScreenManager.Instance;
    }

    public void Execute(ChiaBaiVO vo, bool isResume = false)
    {
        SDLogger.Log("Chia Bai VO: " + vo + " - Count: " + vo.cards.Count);
        GiveCardsLogically(vo);
        if(!isResume) boardStateChangedSignal.Dispatch(true);
        if (vo.tourLevel != -1)
        {
            if (screenManager.inTour())
            {
                tourModel.tourVO.tourLevel = vo.tourLevel;
                tourModel.Dispatch();
            }
        }
        gamePlayModel.status = BoardStatus.CHIA_BAI;
    }

    /**migrate from GiveCardsCompletedCommand*/
    private void GiveCardsLogically(ChiaBaiVO vo)
    {
        //đúng ra là splice(nocIdx * 19, 24), nhưng cũng có thể lấy ngay 24 quân cuối vào nọc
        //bằng cách này, việc chia bài sẽ không phụ thuộc vào nocIdx
        //(nocIdx sẽ chỉ được dùng để chia bài graphically)
        chiaBaiModel.cai = vo.cards.Pop();
        chiaBaiModel.playerHaveCaiIdx = CalculatePlayerHaveCaiIdx(chiaBaiModel.cai);
        nocModel.cards = vo.cards.Splice(-23, 23);
        for (var i = 0; i < gamePlayModel.sitCount; i++)
        {
            //chia bài logically cho người thứ i: Lấy 19 quân cuối của mảng cards
            var cardValues = vo.cards.Splice(-19, 19);
            //chia bài logically cho người thứ i
            if (i == chiaBaiModel.playerHaveCaiIdx)
                cardValues.Add(chiaBaiModel.cai);
            gamePlayModel.sdplayers[i].GiveCards(cardValues);
        }

        //	Chỉ dùng 23 quân làm nọc!
        playModel.curTurn = chiaBaiModel.playerHaveCaiIdx;
        SDLogger.Log("CHia Bai Idx: " + playModel.curTurn + "  "  + chiaBaiModel.playerHaveCaiIdx);
    }

    /**Cho bài: nhị tiến, thất đối,..
	 * Nghĩa là, bốc được nhị thì người bên phải người bốc được cái
	 * => cách tính: bài cái (givenParts[baiCaiIdx]) được đưa cho sdplayers[(cai.so-1)%4]
	 * nếu sdplayers[0] là thằng bốc
	 * Tổng quát, thằng có cái là: sdplayers[(cai.num-1-bcIdx) % sitCount]
	 * với bcIdx là index của thằng bốc cái
	 * @require: việc cho cái đã hoàn tất,
	 * tức các biến prevWinnerIdx, nocIdx, caiIdx, CardShuffler.cards, sitCount đã có*/
    private int CalculatePlayerHaveCaiIdx(int cai)
    {
        var playerBocCaiIdx = gamePlayModel.UserBocCaiIdx();
        var caiNum = Mathf.Floor(cai / SDCard.NUM_PROP);
        if (caiNum == 0) //trong bí tứ, chi chi được đếm là 1 khi cho cái
            caiNum = 1;
        //+ sitCount vì % éo giống mod trong số học: -1 % 4 = -1 (éo = 3)
        return (int) (caiNum - 1 + playerBocCaiIdx + gamePlayModel.sitCount) % gamePlayModel.sitCount;
    }
}