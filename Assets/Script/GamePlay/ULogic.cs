using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class ULogic : Singleton<ULogic>
{
    private static List<int> SCORE_CUOCS = new List<int>
    {
        2, 3, 3,
        4, 4, 4 /**?3*/, 4 /**?3*/,
        4, 6, 12 /*=8đỏ2lèo*/, 7, 8, 12 /*=8đỏ2lèo*/,
        20 /*=10 suông*/, 30 /*=15 suông*/, 20 /*như hoa*/, 30,
        5, 4, 3 /**tt=4*/, 3 /**tt=4*/, 3 /**tt=4*/, 12
    };

    private static List<int> DICH_CUOCS = new List<int>
    {
        0, 1, 1,
        1, 1, 1, 1,
        1, 3, 9 /*=8đỏ2lèo*/, 4, 5, 9 /*=8đỏ2lèo*/,
        17 /*=10 suông*/, 27 /*=15 suông*/, 17 /*như hoa*/, 27,
        2, 1, 1, 1, 1, 9
    };

    public static List<string> CUOC_NAMES = new List<string>
    {
        "Suông", "Thông", "Trì", //2
        "Thiên ù", "Địa ù", "Chíu ù", "Ù bòn", //6
        "Bạch thủ", "Bạch thủ Chi", "Thập thành", "Bạch định", "Tám đỏ", "Kính tứ Chi", //12
        "Hoa rơi cửa Phật", "Nhà lầu xe hơi HRCP", "Cá lội Sân Đình", "Ngư ông bắt cá", //16
        "Lèo", "Tôm", "Thiên khai", "Chíu", "Bòn", "Phá Thiên" //22
    };


    public static int XUONG = 0;
    public static int THONG = 1;
    public static int CHI = 2; //chì
    public static int THIEN_U = 3;
    public static int DIA_U = 4;
    public static int CHIU_U = 5;
    public static int U_BON = 6;
    public static int BACH_THU = 7;
    public static int BACH_THU_CHI = 8;
    public static int THAP_THANH = 9;
    public static int BACH_DINH = 10;
    public static int TAM_DO = 11;
    public static int KINH_TU_CHI = 12;
    public static int HOA = 13; //hoa rơi cửa phật
    public static int NHA = 14; //nhà lầu xe hơi hoa rơi cửa phật
    public static int CA = 15; //cá lội sân đình
    public static int NGU = 16; //Ngư ông bắt cá
    public static int LEO = 17;
    public static int TOM = 18;
    public static int THIEN_KHAI = 19;
    public static int CHIU = 20;
    public static int BON = 21;
    public static int PHA_THIEN = 22;

    public static int SCORE_GA = 5;

    public static List<int> MULTIPLIABLE_CUOCS = new List<int>{LEO, TOM, THIEN_KHAI, CHIU, BON};

    private UModel uModel = UModel.Instance;
    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
    private PlayModel playModel = PlayModel.Instance;

    private SDPlayer p;
    private List<int> redCards;
    private int ucv;

    public void checkBoThienU(SDPlayer p)
    {
        if(canThienU(p))
        {
            p.boU = -1;
        }
    }

    public void checkBoUToActIdx(int uIdx, int aIdx)
    {
        try
        {
            int myPrevActIdx;
            for(myPrevActIdx = aIdx - 1; myPrevActIdx > 0; myPrevActIdx++)
                if(playModel.acts[myPrevActIdx].uIdx == uIdx)
                    break;
            PlayVO a;
            for(var i = myPrevActIdx; i < aIdx; i++)
            {
                a = playModel.acts[i];
                if(a.cardInst == null)
                    continue;
                if(a.type != PlayVO.DRAW && a.uIdx == uIdx)
                    continue;
                    checkBoU(a.cardInst, gamePlayModel.sdplayers[uIdx]);
            }
        }
        catch (Exception e)
        {
            SDLogger.LogError(e.Message);
        }
    }

    private void checkBoU(SDCard c, SDPlayer p)
    {
        if(p.boU == -2 && canUWithCard(p,c))
            p.boU = c.v;
    }

    public bool canThienUAdequateScore(SDPlayer p)
    {
        return canThienU(p) && (gamePlayModel.minScoreU == 2 ||
                                getScoreU(getCuocs(p, null)) >= gamePlayModel.minScoreU);
    }

    public bool canThienU(SDPlayer p)
    {
        return p.chans.Count >= 12 && p.cas.Count == 20 - p.chans.Count;
    }

    public bool canUAdequateScoreWithCard(SDPlayer p, SDCard c)
    {
        return canUWithCard(p, c) && (gamePlayModel.minScoreU == 2 ||
                                        getScoreU(getCuocs(p,c)) >= gamePlayModel.minScoreU);
    }

    public bool canUWithCard(SDPlayer p, SDCard c)
    {
        if(!uModel.cardToPlayActIdxDic.ContainsKey(c))
            return false;
        return p.IsTronBaiWith(c.v) && (c.isInNoc || p.CanChiu(c.v));
    }

    public bool canUWithAct(SDPlayer p, int aIdx)
    {
        return canUWithCard(p,playModel.acts[aIdx].cardInst);
    }

    public static int getScoreU(List<int> cuocs)
    {
        var maxCuocIdx = -1;
        var maxCuocScore = -1;
        for(var i = 0; i < cuocs.Count;i++)
        {
            if(cuocs[i] <= 0 || maxCuocScore >= SCORE_CUOCS[i]) continue;
            maxCuocScore = SCORE_CUOCS[i];
            maxCuocIdx = i;
        }

        var ret = SCORE_CUOCS[maxCuocIdx] + (cuocs[maxCuocIdx] - 1) * DICH_CUOCS[maxCuocIdx];
        for(var i = 0; i < cuocs.Count; i++)
        {
            if(cuocs[i] > 0 && i != maxCuocIdx)
            {
                ret += cuocs[i] * DICH_CUOCS[i];
            }
        }
        return ret;
    }

    public static int calculateScoreU(List<int> cuocs)
    {
        var ret = 0;
        if(cuocs[THAP_THANH] > 0)
        {
            for (var i = 0; i< cuocs.Count;i++)
            {
                if(cuocs[i] > 0)
                {
                    if(i == THAP_THANH || i == BACH_DINH || i == TAM_DO || i == KINH_TU_CHI)
                    {
                        ret += cuocs[i] * SCORE_CUOCS[i];
                    }
                    else
                    {
                        ret += cuocs[i] * DICH_CUOCS[i];
                    }
                }
            }

            return ret;
        }
        else
        {
            ret = getScoreU(cuocs);
            if(cuocs[HOA] > 0 || cuocs[NHA] > 0 || cuocs[CA] > 0 || cuocs[NGU] > 0)
            {
                if(cuocs[CHI] > 0 || cuocs[BACH_THU] > 0)
                {
                    var newCuocs = new List<int>();
                    newCuocs.CopyTo(cuocs.ToArray(), 0);
                    newCuocs[CHI] = 0;
                    newCuocs[BACH_THU] = 0;
                    ret = getScoreU(newCuocs);
                }
            }

            return ret;
        }
    }

    public static int calculateScoreGa(List<int> cuocs)
    {
        var ret = 0;
        if(cuocs[U_BON] > 0 && (cuocs[BACH_THU] > 0 ||
                                cuocs[HOA] > 0 || cuocs[NHA] > 0 || cuocs[CA] > 0 || cuocs[NGU] > 0))
            ret += 1;
        if(cuocs[U_BON] > 0 && cuocs[BACH_THU_CHI] > 0) ret += 1;
        if(cuocs[THAP_THANH] > 0) ret += 1;
        if(cuocs[KINH_TU_CHI] > 0) ret += 1;
        if(cuocs[PHA_THIEN] > 0) ret += 1;
        if(cuocs[HOA] > 0) ret += 1;
        if(cuocs[NHA] > 0) ret += 1;
        if(cuocs[CA] > 0) ret += 1;
        if(cuocs[NGU] > 0) ret += 1;
        if(cuocs[BACH_DINH] > 0) ret += 1;
        if(cuocs[TAM_DO] > 0) ret += 1;
        if(cuocs[BACH_THU_CHI] > 0) ret += 1;
        if (cuocs[CHI] > 0 && cuocs[BACH_THU] > 0) ret += 2; //chì bạch thủ
        if (cuocs[CHI] > 0 && cuocs[BACH_THU_CHI] > 0) ret += 2; //chì bạch thủ chi
        if (cuocs[HOA] > 0 || cuocs[NHA] > 0 || cuocs[CA] > 0 || cuocs[NGU] > 0)

        if(cuocs[CHI] > 0 && cuocs[BACH_THU] > 0)
        {
            ret -= 2;
        }

        return ret;
    }

    public static int GetScore(List<int> cuocs)
    {
        return calculateScoreU(cuocs) + calculateScoreGa(cuocs);
    }

    public List<int> getCuocs(SDPlayer p, SDCard c)
    {
        var ret = new List<int>();
        for(var i = 0; i < CUOC_NAMES.Count;i++)
            ret.Add(0);
        this.p = p;
        if(c == null)
        {
            ret[THIEN_U] = 1;
            ucv = -1;
        }
        else
        {
            var a = uModel.cardToPlayActIdxDic[c];
            ucv = c.v;
            if (isDiaU(a)) ret[DIA_U] = 1;
            if (isChiuU()) ret[CHIU_U] = 1;
            if (isUBon()) ret[U_BON] = 1;
            if (isBachThuChi()) ret[BACH_THU_CHI] = 1;
            if (isHoaRoiCuaPhat(a)) ret[HOA] = 1;
            if (isNhaLauXeHoiHoaRoiCuaPhat(a)) ret[NHA] = 1;
            if (isCaLoiSanDinh(a)) ret[CA] = 1;
            if (isNguOngBatCa(a)) ret[NGU] = 1;
            if (isPhaThien()) ret[PHA_THIEN] = 1;
            //Nếu đã hoa nhà cá ngư thì bỏ chì & bạc
            if (ret[HOA] == 0 && ret[NHA] == 0 && ret[CA] == 0 && ret[NGU] == 0 && isChif(a))
                ret[CHI] = 1;
            ret[CHIU] = p.chiuCount;
            ret[BON] = countBon();
        }

    
        if (isThong()) ret[THONG] = 1;
        //Nếu đã hoa nhà cá thì bỏ chì & bạch thủ đi
        if (ret[HOA] == 0 && ret[NHA] == 0 && ret[CA] == 0 && ret[NGU] == 0 && isBachThu())
            ret[BACH_THU] = 1;
        if (isThapThanh()) ret[THAP_THANH] = 1;
        findRedCards();
        if (isBachDinh()) ret[BACH_DINH] = 1;
        if (isTamDo()) ret[TAM_DO] = 1;
        if (isKinhTuChi()) ret[KINH_TU_CHI] = 1;
        redCards = null;
        ret[LEO] = countLeo();
        ret[TOM] = countTom();
        ret[THIEN_KHAI] = countThienKhai();
        this.p = null;
        ret[XUONG] = 1;
        for (var i = 1; i < CUOC_NAMES.Count; i++)
            if (ret[i] > 0)
            {
                ret[XUONG] = 0;
                break;
            }

        return ret;
    }

    private bool isChiBachThu(int a)
    {
        if(isChif(a) && isBachThu()) return true;
        if(isChif(a) && isBachThuChi()) return true;
        return isNguOngBatCa(a) || isCaLoiSanDinh(a) || isNhaLauXeHoiHoaRoiCuaPhat(a) ||
               isHoaRoiCuaPhat(a); //check ù Hoa, Nhà, Cá, Ngư
    }
    public void updateMissingAndIncorrectCuocs(SumUpVO vo, SDCard c)
    {
        if (vo.autoXuong) return;
        var cuocHos = vo.cuocHos;
        //các cước đúng
        var cuocs = getCuocs(gamePlayModel.myPlayer, c);
        //Nếu ù hoa nhà cá ngư mà có hô cước đó
        var hoDung = false;
        foreach (var i in new List<int> {HOA, NHA, CA, NGU})
            if (cuocs[i] == 1 && cuocHos[i] == 1)
                hoDung = true;
        if (cuocs[HOA] == 1 || cuocs[NHA] == 1 || cuocs[CA] == 1 || cuocs[NGU] == 1)
        {
            if (hoDung) //thì bỏ cước chì, bạch thủ trong cuocHos đi (nếu có)
                cuocHos[CHI] = cuocHos[BACH_THU] = 0;
            else //ngược lại, tức không hô hoa nhà cá, thì coi việc hô chì bạch thủ (nếu có) là xướng đúng
                cuocs[CHI] = cuocs[BACH_THU] = 1;
        }

        //update cước sai (thừa)
        vo.incorrectCuocIndexes = new List<int>();
        for (var i = 1; i < cuocs.Count; i++)
            if (cuocHos[i] > cuocs[i])
                vo.incorrectCuocIndexes.Add(i);
        //update cước thiếu
        vo.missingCuocIndexes = new List<int>();
        foreach (var i in new List<int> {THIEN_U, CHIU_U, BACH_THU_CHI, U_BON})
            if (cuocHos[i] < cuocs[i])
                vo.missingCuocIndexes.Add(i);
        if (hoDung || cuocHos[BACH_THU] > 0)
            return;
        //Ù hoa nhà cá ngư mà không hô, lại cũng k hô bạch thủ thì là thiếu cước bạch thủ
        foreach (var i in new List<int> {HOA, NHA, CA, NGU})
            if (cuocHos[i] < cuocs[i])
            {
                vo.missingCuocIndexes.Add(BACH_THU);
                return;
            }

        //ù bạch thủ mà không hô bt thì là thiếu cước bt
        //Tuy nhiên, nếu thằng này thiên ù thì không coi là thiếu bt dù nó có hô thiên ù hay không
        if (cuocHos[BACH_THU] < cuocs[BACH_THU] && cuocs[THIEN_U] == 0)
            vo.missingCuocIndexes.Add(BACH_THU);
    }

    public SDCard getCardUAfterSumUpCommand()
    {
        return uModel.uvo == null || uModel.uvo.isThienU ? null : playModel.acts[uModel.uvo.aIdx].cardInst;
    }

    //++++các hàm check cước hô++++//
    private bool isThong()
    {
        return gamePlayModel.prevWinnerId == p.uid;
    }

    public bool isThapThanh()
    {
        return p.chanCount == 10 ||
               p.chanCount == 9 && p.ques.Count == 1 && p.ques[0].v == ucv;
    }

    /**check ù chì. đk: k thiên ù
	 * @param aIdx - play action index in playModel.acts (not validate)*/
    private bool isChif(int aIdx)
    {
        var act = playModel.acts[aIdx];
        var uIdx = gamePlayModel.sdplayers.IndexOf(p);
        return act.type == PlayVO.DRAW && act.uIdx == uIdx ||
               //act.type == PlayVO.DANH && ù quân đánh thì k bao giờ là Trì! @see issue #145
               act.type == PlayVO.TRACUA && act.door == uIdx;
    }

    private bool isDiaU(int aIdx)
    {
        var acts = playModel.acts;
        //Check không phải thiên Ù.
        //Nếu trong các action từ 0 đến trước aIdx có 1 action Bốc (của bất kỳ ai) thì k phải địa ù.
        if (aIdx == -1) return false;
        for (var i = 0; i < aIdx; i++)
            if (acts[i].type == PlayVO.DRAW)
                return false;
        return true;
    }

    private bool isChiuU()
    {
        return p.CanChiu(ucv);
    }

    private bool isUBon()
    {
        return p.eatenChans.IndexOf(ucv) != -1 && (p.ques.Count == 0 || p.ques[0].v == ucv);
    }

    private bool isBachThu()
    {
        return ucv == -1 && p.chanCount == 6 //thiên ù bạch thủ
               || p.chanCount == 5 && p.badaus.Count == 0
                                   && ucv != SDCard.V_CHI_CHI; //Bạch thủ chi mà chỉ hô bạch thủ là xướng sai
    }

    private bool isBachThuChi()
    {
        return p.chanCount == 5 && ucv == SDCard.V_CHI_CHI;
    }

    private void findRedCards()
    {
        redCards = new List<int>();
        for (var i = 0; i < p.cards.Count; i++)
            if (p.cards[i].isRed)
                redCards.Add(p.cards[i].v);
        if (ucv == -1)
            return;
        if (SDCard.isRed2(ucv))
            redCards.Add(ucv);
        for (var i = 0; i < p.eatenChans.Count; i++)
            if (SDCard.isRed2(p.eatenChans[i]))
                redCards.Add(p.eatenChans[i]);
        for (var i = 0; i < p.eatenCas.Count; i++)
            if (SDCard.isRed2(p.eatenCas[i]))
                redCards.Add(p.eatenCas[i]);
    }

    private bool isBachDinh()
    {
        return redCards.Count == 0;
    }

    /**pre-require: findRedCards()*/
    private bool isTamDo()
    {
        return redCards.Count == 8;
    }

    /**pre-require: findRedCards()*/
    private bool isKinhTuChi()
    {
        if (redCards.Count != 4)
            return false;
        for (var i = 0; i < redCards.Count; i++)
            if (redCards[i] != SDCard.V_CHI_CHI)
                return false;
        return true;
    }

    private bool isDinhChua()
    {
        int i;
        for (i = 0; i < p.eatenChans.Count; i++)
            if (p.eatenChans[i] == SDCard.V_NGU_VANJ)
                return true;
        for (i = 0; i < p.eatenCas.Count; i++)
            if (p.eatenCas[i] == SDCard.V_NGU_VANJ)
                return true;
        return false;
    }

    private bool isHoaRoiCuaPhat(int aIdx)
    {
        return ucv == SDCard.V_NHI_VANJ && isChif(aIdx) && isBachThu() && isDinhChua();
    }

    private bool isCaLoiSanDinh(int aIdx)
    {
        return ucv == SDCard.V_BAT_VANJ && isChif(aIdx) && isBachThu() && isDinhChua();
    }

    /**trên bài có chắn ngũ vạn, chắn tứ vạn, trì bạch thủ nhị vạn.
	 * Note: Theo cách này, có thể vừa nhà, lại vừa hoa!*/
    private bool isNhaLauXeHoiHoaRoiCuaPhat(int aIdx)
    {
        return ucv == SDCard.V_NHI_VANJ && isChif(aIdx) && isBachThu()
               && hasOnHandChan(SDCard.V_NGU_VANJ) && hasOnHandChan(SDCard.V_TU_VANJ);
    }

    /**Hàm này dùng để check cước nhà lầu & cước ngư ông
	 * @return có chắn v trên tay không*/
    private bool hasOnHandChan(int v)
    {
        for (var i = 0; i < p.chans.Count; i++)
            if (p.chans[i].v == v)
                return true;
        return false;
    }

    /**trên bài có chắn chi chi, chắn ngũ sách, trì bạch thủ bát vạn.
	 * Note: Theo cách này, có thể vừa cá lội, lại vừa ngư ông!*/
    private bool isNguOngBatCa(int aIdx)
    {
        return ucv == SDCard.V_BAT_VANJ && isChif(aIdx) && isBachThu()
               && hasOnHandChan(SDCard.V_CHI_CHI) && hasOnHandChan(SDCard.V_NGU_SACH);
    }

    /** if SDPlayer.noChan and Ù đúng */
    private bool isPhaThien()
    {
        return p.noChan;
    }

    private int countCard(int v)
    {
        var ret = 0;
        for (var i = 0; i < p.cards.Count; i++)
            if (p.cards[i].v == v)
                ret++;
        if (ucv == -1)
            return ret;
        if (ucv == v)
            ret++;
        for (var i = 0; i < p.eatenChans.Count; i++)
            if (p.eatenChans[i] == v)
                ret++;
        for (var i = 0; i < p.eatenCas.Count; i++)
            if (p.eatenCas[i] == v)
                ret++;
        return ret;
    }

    private int countLeo()
    {
        return Mathf.Min(countCard(SDCard.V_CUU_VANJ), countCard(SDCard.V_BAT_SACH), countCard(SDCard.V_CHI_CHI));
    }

    private int countTom()
    {
        return Mathf.Min(countCard(SDCard.V_TAM_VANJ), countCard(SDCard.V_TAM_SACH), countCard(SDCard.V_THAT_VAN));
    }

    private int countThienKhai()
    {
        var ret = 0;
        for (var i = 0; i < p.chans.Count - 3; i++)
        {
            for (var j = i + 2; j < p.chans.Count; j++)
                if (p.chans[i].isChan(p.chans[j]))
                {
                    ret++;
                    i++;
                    break;
                }
        }

        return ret;
    }

    private int countBon()
    {
        var ret = 0;
        for (var i = 0; i < p.eatenChans.Count - 3; i++)
        {
            for (var j = i + 2; j < p.eatenChans.Count; j++)
                if (p.eatenChans[i] == p.eatenChans[j])
                {
                    ret++;
                    i++;
                    break;
                }
        }

        return ret - p.chiuCount;
    }

    public int doorDistance(UVO uvo)
    {
        if (uvo.aIdx < 0 || uvo.aIdx >= playModel.acts.Count)
            return 0;
        //act là action sinh ra quân ù
        //act.type chỉ có thể là DRAW, DANH, TRACUA
        var act = playModel.acts[uvo.aIdx];
        //door là cửa của quân ù
        var door = act.type == PlayVO.TRACUA ? act.door : act.uIdx;
        return (uvo.uIdx - door + gamePlayModel.sitCount) % gamePlayModel.sitCount;
    }

    private static List<string> getCuocsAsStringArr(List<int> cuocs)
    {
        var ret = new List<string>();
        foreach (var i in new List<int> {XUONG, THONG, CHI, THIEN_U, PHA_THIEN})
        {
            if (cuocs[i] == 1)
            {
                ret.Add(CUOC_NAMES[i]);
            }
        }

        // nếu cả địa ù & chíu ù thì hô là địa chíu ù
        if (cuocs[DIA_U] == 1 && cuocs[CHIU_U] == 1)
            ret.Add("Địa chíu ù");
        else if (cuocs[DIA_U] == 1 && cuocs[U_BON] == 1)
            ret.Add("Địa ù bòn");
        else if (cuocs[DIA_U] == 1)
            ret.Add(CUOC_NAMES[DIA_U]);
        else if (cuocs[CHIU_U] == 1)
            ret.Add(CUOC_NAMES[CHIU_U]);
        else if (cuocs[U_BON] == 1)
            ret.Add(CUOC_NAMES[U_BON]);

        foreach (var i in new List<int>
            {BACH_THU, BACH_THU_CHI, THAP_THANH, BACH_DINH, TAM_DO, KINH_TU_CHI, HOA, NHA, CA, NGU}
        ) // ù bòn đến ngư ông + phá thiên
        {
            if (cuocs[i] == 1) ret.Add(CUOC_NAMES[i]);
        }

        foreach (var i in new List<int> {LEO, TOM, THIEN_KHAI, CHIU, BON}) // lèo, tôm, thiên khai, chíu, bòn
        {
            if (cuocs[i] == 1)
                if (i != LEO && i != TOM) ret.Add("có " + CUOC_NAMES[i]); //không hô "có lèo" hay "có tôm"
                else ret.Add(CUOC_NAMES[i]);
            else if (cuocs[i] > 1) ret.Add(cuocs[i] + " " + CUOC_NAMES[i]);
        }

        // luôn có ít nhất 1 cước (tính cả suông) nên msg luôn != ""
        return ret;
    }

    public static string getCuocsStr(List<int> cuocs)
    {
        return cuocs == null ? "Thoát trước khi xướng" : string.Join(" ", getCuocsAsStringArr(cuocs));
    }
    
    public static string GetCuocsAfterSplit(List<int> cuocs)
    {
        if (cuocs == null)
        {
            return "Thoát trước khi xướng";
        }
        else
        {
            var ls = getCuocsAsStringArr(cuocs);
            var maxStr = string.Join(" ", ls);
            if (maxStr.Length < 45)
                return maxStr;

            var str = "";
            var current = "";
            for (var i = 0; i < ls.Count; i++)
            {
                var s = ls[i];
                current += s + (i == ls.Count - 1 ? "" : " ");
                if (current.Length >= 45)
                {
                    str += current + (i == ls.Count - 1 ? "" : "\n");
                    current = "";
                }
            }

            str += current;
            return str;
        }
    }

    public static SpecialCuoc GetSpecialCuoc(string cuocsMsg)
    {
        int tempCount = 0;
        // tempCount để tính index của sprite trong specialCuocImgList
        // thứ tự: cá-ngư, nhà-hoa, ngư, cá, hoa

        if (cuocsMsg.Contains(CUOC_NAMES[CA]) && cuocsMsg.Contains(CUOC_NAMES[NGU]))
        {
            return SpecialCuoc.CaLoiNguOng;
        }
        else
        {
            var cuocUf = new List<int> {NHA, NGU, CA, HOA};
            tempCount = 1;
            foreach (var i in cuocUf)
            {
                if (cuocsMsg.Contains(CUOC_NAMES[i]))
                {
                    return (SpecialCuoc) tempCount;
                }

                tempCount++;
            }
        }

        return SpecialCuoc.None;
    }

}

public enum SpecialCuoc
{
    CaLoiNguOng = 0,
    NhaLauXeHoi,
    NguOng,
    CaLoi,
    HoaRoiCuaPhat,
    None
}