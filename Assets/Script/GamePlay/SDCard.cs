using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SDCard : IComparable, IPointerClickHandler
{
    private static SpriteAtlas _cardsAtlas;
    private static SpriteAtlas CardsAtlas
    {
        get
        {
            if(_cardsAtlas == null) _cardsAtlas = Resources.Load<SpriteAtlas>("SpriteAtlas/CardsAtlas");
            return _cardsAtlas;
        }
    }

    private static List<string> NAMES = new List<string>
    {
        "chi chi", "thang thang", "ông cụ", "nhất vạn", "nhất văn", "nhất sách", //0..5
        "nhị vạn", "nhị văn", "nhị sách", "tam vạn", "tam văn", "tam sách", //6..11
        "tứ vạn", "tứ văn", "tứ sách", "ngũ vạn", "ngũ văn", "ngũ sách", //12..17
        "lục vạn", "lục văn", "lục sách", "thất vạn", "thất văn", "thất sách", //18..23
        "bát vạn", "bát văn", "bát sách", "cửu vạn", "cửu văn", "cửu sách" //24..29
    };

    public static int NUM_PROP = 3;//3 chất vạn, văn, sách. Chi chi tạm coi là 0 vạn
    public static int V_CUU_VANJ = 27;//cửu vạn
    public static int V_BAT_SACH = 26;//bát sách
    public static int V_CHI_CHI = 0;//chi chi
    public static int V_TAM_VANJ = 9;//tam vạn
    public static int V_TAM_VANW = 10;//tam văn
    public static int V_TAM_SACH = 11;//tam sách
    public static int V_THAT_VAN = 22;//thất văn
    public static int V_NHI_VANJ = 6;//nhị vạn
    public static int V_TU_VANJ = 12;//tứ vạn
    public static int V_NGU_VANJ = 15;//ngũ vạn
    public static int V_NGU_VANW = 16;//ngũ văn
    public static int V_NGU_SACH = 17;//ngũ sách
    public static int V_BAT_VANJ = 24;//bát vạn
    public int num; // chi chi 0, nhị 2, tam 3, ....
    private int prop; //vạn 0, văn 1, sách 2. prop của chi chi cũng là 0
    private bool _isInNoc;//=false

    public bool IsSelected {get; set;} = false;
    public int v => num * NUM_PROP + prop;
    public string n => NAMES[v];
    public static string cardName(int v)
    {
        return NAMES[v];
    }

    public SDCard(int value)
    {
        prop = value % NUM_PROP;
        num = (value - prop) / NUM_PROP;
    }

    public Sprite GetSprite()
    {
        return CardsAtlas.GetSprite(v.ToString());
    }

    public bool isInNoc {
        get => _isInNoc;
        set => _isInNoc = value;
    }

    public static bool isChanCa2(double v1, double v2){
        return (int) Math.Floor(v1 / NUM_PROP) == (int) Math.Floor(v2 / NUM_PROP);
    }

    public static bool isCa2(double v1, double v2){
        return v1 != v2 && (int) Math.Floor(v1 / NUM_PROP) == (int) Math.Floor(v2 / NUM_PROP);
    }

    public bool isRed => num == 0 || (num == 8 || num == 9) && (prop == 0 || prop == 2);

    public static bool isRed2(int v)
    {
        return v == 0 || v == 24 || v == 26 || v == 27 || v == 29;
    }

    public bool isChiChi => num == 0;

    public bool isChan(SDCard c) {
        return num == c.num && prop == c.prop;
    }

    public bool isCa(SDCard c) {
        return num == c.num && prop != c.prop;
    }

    public static void countChanCas(List<SDCard> cards, List<SDCard> chans, List<SDCard> cas, List<SDCard> badaus, List<SDCard> ques) {
        int i , j, k;
        bool[] marks = new bool[cards.Count];
        for(i = 0; i < cards.Count - 1; i++)//tim chan
            if(! marks[i])
                for(j = i + 1; j < cards.Count;j++)
                    if(! marks[j] && cards[i].isChan(cards[j])) {
                        chans.Add(cards[i]);
                        chans.Add(cards[j]);
                        marks[i] = marks[j] = true;
                        break;
                    }
        for(i = 0; i < cards.Count - 2; i++)// tim ba dau
            if(! marks[i]) {
                for(j = i + 1; j < cards.Count - 1; j++){
                    if(!marks[j] && cards[i].isCa(cards[j])){
                        for(k = j + 1; k < cards.Count;k++)
                        if(! marks[k] && cards[i].isCa(cards[k])) {
                            badaus.Add(cards[i]);
                            badaus.Add(cards[j]);
                            badaus.Add(cards[k]);
                            marks[i] = marks[j] = marks[k] = true;
                            break;
                        }

                        break;
                    }
                }
            }
        for(i = 0; i < cards.Count - 1; i++) // tim ca
            if(! marks[i])
                for(j = i + 1; j < cards.Count; j++)
                    if(! marks[j] && cards[i].isCa(cards[j])){
                        cas.Add(cards[i]);
                        cas.Add(cards[j]);
                        marks[i] = marks[j] = true;
                        break;
                    }
        for(i = 0; i < cards.Count; i++)//que
            if(! marks[i])
                ques.Add(cards[i]);
    }

    public int CompareTo(object obj)
    {
        var otherCard = (SDCard) obj;
        return otherCard != null ? (v % 30).CompareTo(otherCard.v % 30) : 1;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public static void Sort(List<SDCard> cards) {
        var chans = new List<SDCard>();
        var cas = new List<SDCard>();
        var badaus = new List<SDCard>();
        var ques = new List<SDCard>();
        countChanCas(cards, chans, cas, badaus, ques);
        cards.Splice(0, cards.Count);
        SortInc(chans);
        SortInc(cas);
        SortInc(badaus);
        SortInc(ques);

        cards.AddRange(chans);
        cards.AddRange(cas);
        cards.AddRange(badaus);
        cards.AddRange(ques);
        
    }

    private static void SortInc(List<SDCard> cards)
    {
        var n = cards.Count;
        for(var i = 1; i < n; i++){
            var c = cards[i];
            var v = c.v;
            int j;
            for(j = i - 1; j >= 0 && cards[j].v > v;j--)
            {
                cards[j + 1] = cards[j];
            }
            cards[j + 1] = c;
        }
    }
}
