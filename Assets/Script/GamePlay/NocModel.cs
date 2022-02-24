using System.Collections.Generic;

public class NocModel : Singleton<NocModel>
{
    private CardDrawnSignal cardDrawnSignal = Signals.Get<CardDrawnSignal>();
    public int count;

    /**int values. migrate from McNoc.cards.
	 * Được set tại CardGivingMediator.latCai
	 * Note không cần reinit = null khi prepareForNewGame*/
    public List<int> cards = new List<int>();

    /**@return count remains cards == 1
	 * Note: Chỉ dùng 23 quân làm nọc!
	 * (không được playAction nữa, chỉ được ù. Nếu k có thằng nào ù thì ván chơi hòa)*/
    public bool isEndInDraw()
    {
        return cards.Count == 1;
    }

    public SDCard draw()
    {
        var card = new SDCard(cards.Pop());
        card.isInNoc = true;
        cardDrawnSignal.Dispatch(card);
        return card;
    }

    public int nextDrawCard => cards[cards.Count - 1];
}