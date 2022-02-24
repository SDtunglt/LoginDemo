using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModel : Singleton<PlayModel>
{
    private GamePlayModel boardModel = GamePlayModel.Instance;

    public int curTurn;
    public bool isMyTurn => curTurn == boardModel.myIdx;

    private int nextTurn => (curTurn + 1) % boardModel.sitCount;
    public bool isMyNextTurn => nextTurn == boardModel.myIdx;

    public int timeStartSortMyCards;

    public SDCard liftedCard;
    public SDCard curCard;
    public List<PlayVO> acts = new List<PlayVO>();
    public int curActIdx;
    public PlayVO curAct;

    public int getCurRealTurn()
    {
        return curAct != null && curAct.type == PlayVO.CHIU ? curAct.uIdx : curTurn;
    }

    public bool isChiuWhenDraw;

    public void updateChiuWhenDraw(){
        if(curAct.type == PlayVO.DRAW || curAct.type == PlayVO.DUOI)
            isChiuWhenDraw = true;
        else if(curAct.type == PlayVO.DANH)
            isChiuWhenDraw = false;
    }

    public int nhaiGa = 0;

    public void checkChangeCurTurn()
    {
        if(curAct.type == PlayVO.DANH || curAct.type == PlayVO.DUOI)
            curTurn = nextTurn;
        else if( curAct.type == PlayVO.CHIU && acts[acts.Count - 2].type == PlayVO.DUOI)
            curTurn = (curTurn - 1 + boardModel.sitCount) % boardModel.sitCount;
    }

    public bool isCurActCauseChangeCurCard()
    {
        return curAct != null
            &&(curAct.type == PlayVO.DRAW
            || curAct.type == PlayVO.DANH
            || curAct.type == PlayVO.TRACUA);
    }

    public void ReInit()
    {
        curCard = null;
        curAct = null;
        curActIdx = 0;
        acts.Splice(0, acts.Count);
        nhaiGa = 0;
    }
}
