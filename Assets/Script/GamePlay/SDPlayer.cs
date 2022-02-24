using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities;
using UnityEngine;

public class SDPlayer
{
    private PlayModel playModel = PlayModel.Instance;
    private ShowTimeOutMsgSignal ShowTimeOutMsgSignal = Signals.Get<ShowTimeOutMsgSignal>();
    private PlayerStatusChangedSignal playerStatusChangedSignal = Signals.Get<PlayerStatusChangedSignal>();
    public bool isPlayed;
    public User u;
    public double coin => u.GetVariable(GameConfig.VAR_COIN).GetDoubleValue();

    public string uid => u.Name;
    public int idx;
    public string name => u.GetVariable(GameConfig.VAR_UNAME).GetStringValue();
    public int seat;
    private bool _dis;

    public bool dis
    {
        get => _dis;
        set
        {
            if (_dis == value) return;
            _dis = value;
            playerStatusChangedSignal.Dispatch(this);
        }
    }

    public double ip
    {
        get
        {
            if (u.ContainsVariable(GameConfig.VAR_IP))
            {
                return u.GetVariable(GameConfig.VAR_IP).GetDoubleValue();
            }
            else
            {
                return 0;
            }
        }
    }

    public PlayErrVO err;

    public bool bao => err != null && err.isBao;

    public int boU;
    public bool isBoU()
    {
        return boU != -2;
    }

    public List<int> eatenCas;
    public List<int> eatenChans;
    private List<int> notEatCards;
    public List<int> playedCards;

    private bool isPlayedCa;
    public List<SDCard> cards;
    public List<SDCard> chans;
    public List<SDCard> cas;
    public List<SDCard> badaus;
    public List<SDCard> ques;

    public int chanCount => (chans.Count + eatenChans.Count) / 2;

    public int chiuCount;
    public bool noChan = false;

    public SDPlayer()
    {
        boU = -2;
        eatenCas = new List<int>();
        eatenChans = new List<int>();
        notEatCards = new List<int>();
        playedCards = new List<int>();
        cards = new List<SDCard>();
        cas = new List<SDCard>();
        badaus = new List<SDCard>();
        ques = new List<SDCard>();
    }

    public void GiveCards(List<int> cardValues)
    {
        cards = new List<SDCard>();
        foreach ( int t in cardValues)
        {
            int i1 = t;
            cards.Add(new SDCard(i1));
        }

        ReinitChanCaLists();
        noChan = chans.Count == 0;
    }

    public bool CanChiu(int cardValue)
    {
        if(dis || bao) return false;
        var count = cards.Count(t => t.v == cardValue);
        return count == 3;
    }

    public void CheckIllegalPlay(PlayVO act)
    {
        if(this.bao) return;
        int errCode;
        PlayErrVO e = null;
        if(act.type ==PlayVO.EAT)
        {
            errCode = IsIllegalEat(act.card,playModel.curCard.v);
            if(errCode != PlayErrVO.NOT_ERR)
                e = new PlayErrVO(errCode,act.card, playModel.curCard.v);
        }
        else if( act.type == PlayVO.CHIU)
        {
            if(IsEatenCaHang(act.card))
                e = new PlayErrVO(PlayErrVO.AN_CA_CHIU_CUNG_HANG,act.card);
        }
        else if( act.type == PlayVO.DANH || act.type == PlayVO.TRACUA)
        {
            errCode = IsIllegalPlay(act.card);
            if(errCode != PlayErrVO.NOT_ERR)
                e = new PlayErrVO(errCode, act.card);
        }

        if(PlayErrVO.isGreater(e, err))
        {
            err = e;
            if(e != null && !e.isBao) return;
            playerStatusChangedSignal.Dispatch(this);
            if(!u.IsItMe)
            {
                ShowTimeOutMsgSignal.Dispatch(SDMsg.Join(AppMsg.BAOTHENCANTPLAY,this.name,err.msg));
            }
        }
    }

    public void CheckChangeLawLists(PlayVO act)
    {
        if(act.type == PlayVO.DANH || act.type == PlayVO.TRACUA)
        {
            playedCards.Add(act.card);
            if(!isPlayedCa)
                foreach(var t in playedCards)
                {
                    if(SDCard.isCa2(t,act.card)) isPlayedCa = true;
                }
        }
        else
        {
            if(playModel?.curAct == null) return;
            var v = playModel.curCard.v;
            if(act.type == PlayVO.CHIU)
            {
                eatenChans.Add(v);
                eatenChans.Add(v);
                eatenChans.Add(v);
                eatenChans.Add(v);
                chiuCount++;
            }
            else if(act.type == PlayVO.EAT)
            {
                if(act.card == v)
                {
                    eatenChans.Add(act.card);
                    eatenChans.Add(act.card);
                }
                else
                {
                    eatenCas.Add(v);
                    eatenCas.Add(act.card);
                }
            }
            else
                notEatCards.Add(v);
        }
    }

    private int IsIllegalEat(int eater, int eaten)
    {
        if(IsPlayedCard(eaten))
            return PlayErrVO.DANH_ROI_AN;
        if(IsBoChan(eater))
        {
            if(eater == eaten)
                return PlayErrVO.BO_CHAN_AN_CHAN;
            else
                return PlayErrVO.BO_CHAN_AN_CA;
        }

        if(SDCard.isCa2(eater,eaten))
        {
            if(isPlayedCa)
                return PlayErrVO.DANH_CA_AN_CA;
            if(IsBoCa(eater))
                return PlayErrVO.BO_CHAN_AN_CA;
            if(IsPlayedCardHang(eater))
                return PlayErrVO.DANH_AN_CA_CUNG_HANG;
        }

        if(IsEatenCaHang(eaten))
        {
            if(eater == eaten)
                return PlayErrVO.AN_CA_AN_CHAN;
            if(SDCard.isCa2(eater,eaten))
                return PlayErrVO.AN_2_CA;
        }

        if(SDCard.isCa2(eater,eaten))
        {
            if(IsCoChanCauCa(eater))
                return PlayErrVO.CO_CHAN_CAU_CA;
            if(IsAnChonCa(eater))
                return PlayErrVO.AN_CHON_CA;
            if(IsWaitToU())
                return PlayErrVO.AN_CA_CCHO;
        }

        if(CanChiu(eaten))
            return PlayErrVO.AN_CHIU_DUOC;
        if(SDCard.isCa2(eater,eaten) && IsAnTreoTranh(eaten))
            return PlayErrVO.AN_TREO_TRANH;
        return PlayErrVO.NOT_ERR;
    }

    private bool IsEatenCaHang(int v)
    {
        foreach (var t in eatenCas)
        {
            if(SDCard.isChanCa2(t,v))
                return true;
        }

        return false;
    }

    private bool IsPlayedCardHang(int v)
    {
        foreach (var t in playedCards)
        {
            if(SDCard.isChanCa2(t,v))
                return true;
        }

        return false;
    }

    private bool IsBoCa(int v)
    {
        foreach(var t in notEatCards)
            if(SDCard.isCa2(t,v))
                return true;
        return false;
    }

    private bool IsBoChan(int v)
    {
        return notEatCards.IndexOf(v) != -1;
    }

    private bool IsDanhCa(int v)
    {
        foreach (var t in playedCards)
        {
            if(SDCard.isCa2(t,v))
                return true;
        }

        return false;
    }
    

    private bool IsPlayedCard(int v)
    {
        return playedCards.IndexOf(v) != -1;
    }

    private bool IsEatencard(int v)
    {
        return eatenChans.IndexOf(v) != -1 || eatenCas.IndexOf(v) % 2 == 0;
    }

    private int IsIllegalPlay(int card)
    {
        if(IsBoChan(card))
            return PlayErrVO.BO_CHAN_DANH_CHAN;
        if(IsPlayedCard(card))
            return PlayErrVO.DANH_CHAN;
        if(IsEatencard(card))
            return PlayErrVO.AN_ROI_DANH;
        if(eatenCas.Count > 0)
        {
            if(IsDanhCa(card))
                return PlayErrVO.AN_CA_DANH_CA;
            if(IsEatenCaHang(card))
                return PlayErrVO.AN_CA_DANH_CUNG_HANG;
        }

        return PlayErrVO.NOT_ERR;
    }

    public int GetAutoPlayCard()
    {
        if(!bao && playModel.liftedCard != null && IsIllegalPlay(playModel.liftedCard.v) == PlayErrVO.NOT_ERR)
            return playModel.liftedCard.v;
        int i;
        for(i = ques.Count - 1; i >= 0; i--)
            if(IsIllegalPlay(ques[i].v) == PlayErrVO.NOT_ERR)
                return ques[i].v;
        for(i = badaus.Count - 1; i >= 0;i--)
            if(IsIllegalPlay(badaus[i].v) == PlayErrVO.NOT_ERR)
                return badaus[i].v;
        for(i = cas.Count - 1; i >= 0; i--)
            if(IsIllegalPlay(cas[i].v) == PlayErrVO.NOT_ERR)
                return cas[i].v;
        for(i = chans.Count - 1; i >= 0; i--)
            if(IsIllegalPlay(chans[i].v) == PlayErrVO.NOT_ERR)
                return chans[i].v;
        return cards[cards.Count - 1].v;
    }

    private void CleanChanCaLists()
    {
        chans = new List<SDCard>();
        cas = new List<SDCard>();
        badaus = new List<SDCard>();
        ques = new List<SDCard>();
    }

    public void ReinitChanCaLists()
    {
        CleanChanCaLists();
        SDCard.countChanCas(cards,chans,cas,badaus,ques);
    }

    private static int Count(List<SDCard> list, int v)
    {
        var count = 0;
        for (var i = 0;i < list.Count;i++)
            if(list[i].v == v)
                count++;
        return count;
    }

    private bool IsCoChanCauCa(int eater)
    {
        var c = Count(cards,eater);
        return c == 2 || c == 4;
    }

    private bool IsAnChonCa(int eater)
    {
        return Count(ques,eater) == 0;
    }

    public bool IsTronBaiWith(int card)
    {
        if(!IsWaitToU())
            return false;
        if(chanCount > 5 && card == SDCard.V_CHI_CHI)
            return false;
        if(ques.Count == 1)
        {
            if(chanCount == 5)
                return ques[0].v == card;
            return SDCard.isChanCa2(ques[0].v,card);
        }
        return SDCard.isChanCa2(badaus[0].v,card);
    }

    private bool IsWaitToU()
    {
        return 3 * ques.Count + badaus.Count == 3 && chanCount >= 5;
    }

    private bool IsAnTreoTranh(int eaten)
    {
        return Count(cards, eaten) > 0;
    }

    public SDCard GetCardByValue(int v)
    {
        for(var i = cards.Count - 1; i >= 0;i--)
            if(cards[i].v == v)
                return cards[i];
        SDLogger.LogError("Sao trả về card lại Null: Count " + cards.Count + "Value: " + v);
        return null;
    }

    public List<SDCard> GetAllCardsByValue(int v)
    {
        var ret = new List<SDCard>();
        for(var i = cards.Count - 1;i >= 0;i--)
            if(cards[i].v == v)
                ret.Add(cards[i]);
        return ret;
    }

    public void RemoveCards(List<SDCard> _cards)
    {
        foreach(var card in _cards)
        {
            var idx = cards.IndexOf(card);
            cards.Splice(idx,1);
        }
    }
}
