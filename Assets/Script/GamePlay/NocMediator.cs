using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class NocMediator : MonoBehaviour
{
    [SerializeField] private CardPlayerUMediator cardPlayerU;
    [SerializeField] private TMP_Text txtCount;
    [SerializeField] private Image backCardIm;
    [SerializeField] protected Button btnXemNoc;
    [SerializeField] private RectTransform nocLayout;
    [SerializeField] private GameObject cardPrefabs;
    private bool chuaShowNoc = true;
    private bool isShowComplete = false;
    private Tween delay;

    private ShowCardUSignal showCardUSignal = Signals.Get<ShowCardUSignal>();
    private StopGameSignal stopGameSignal = Signals.Get<StopGameSignal>();
    private SumUpSignal sumUpSignal = Signals.Get<SumUpSignal>();
    private ShowSumupSignal showSumupSignal = Signals.Get<ShowSumupSignal>();
    private CardDrawnSignal cardDrawnSignal = Signals.Get<CardDrawnSignal>();

    private NocModel nocModel = NocModel.Instance;

    private void Awake()
    {
        showCardUSignal.AddListener(OnShowCardU);
        stopGameSignal.AddListener(OnStopGame);
        sumUpSignal.AddListener(OnSumUp);
        showSumupSignal.AddListener(OnSumUp);
        cardDrawnSignal.AddListener(OnCardDraw);
        Signals.Get<GamePlayBackCardSignal>().AddListener(OnChangeBackCard);
    }

    private void Start()
    {
        if (ScreenManager.Instance.IsOnChanhTongTriPhuDaiLau())
        {
            OnChangeBackCard(1);
        }
        else
        {
            OnChangeBackCard(GameUtils.GameBC);
        }
    }

    private void OnDestroy()
    {
        showCardUSignal.RemoveListener(OnShowCardU);
        stopGameSignal.RemoveListener(OnStopGame);
        sumUpSignal.RemoveListener(OnSumUp);
        cardDrawnSignal.RemoveListener(OnCardDraw);
        showSumupSignal.RemoveListener(OnSumUp);
        Signals.Get<GamePlayBackCardSignal>().RemoveListener(OnChangeBackCard);
    }

    private void OnChangeBackCard(int obj)
    {
        backCardIm.sprite = GlobalDataManager.Ins.backCard[obj];
        txtCount.color = GlobalDataManager.Ins.backCardTxtColors[obj];
    }

    protected virtual void OnEnable()
    {
        isShowComplete = false;

        nocLayout.Hide();
        nocLayout.DOScaleX(0f, 0f);

        for (var i = nocLayout.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(nocLayout.GetChild(i).gameObject);
        }

        for (var i = nocModel.cards.Count - 1; i >= 0; i--)
        {
            var cardObject = Instantiate(cardPrefabs, nocLayout);
            cardObject.GetComponent<Image>().sprite =
                Resources.Load<SpriteAtlas>("SpriteAtlas/CardsAtlas").GetSprite(nocModel.cards[i].ToString());
        }

        txtCount.text = nocModel.cards.Count.ToString();

        if (GamePlayModel.IsReplay)
        {
            btnXemNoc.Show();
        }
    }

    private void LateUpdate()
    {
        if (!isShowComplete) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            delay?.Kill();
            nocLayout.DOScaleX(0f, .3f)
                .OnComplete(() => nocLayout.Hide());
            chuaShowNoc = true;
            isShowComplete = false;
            btnXemNoc.image.raycastTarget = true;
        }
    }

    private void OnShowCardU(List<SDCard> cards)
    {
        Debug.Log("Show Cards On Uf");
        cardPlayerU.Show();
        cardPlayerU.ShowCards(cards);
    }

    private void OnStopGame()
    {
        cardPlayerU.Hide();
        btnXemNoc.Hide();
        this.Hide();
    }

    private void OnSumUp(UVO uvo = null)
    {
        btnXemNoc.Show();
    }
    void OnSumUp(string msg, string cuocsMsg)
    {
        // workaround
        OnSumUp();
    }

    public void ShowNoc()
    {
        if (chuaShowNoc)
        {
            chuaShowNoc = false;
            btnXemNoc.image.raycastTarget = false;

            nocLayout.Show();
            nocLayout.DOScaleX(1f, .3f).OnComplete(() => { isShowComplete = true; });

            delay?.Kill();
            delay = DOVirtual.DelayedCall(3f, () =>
            {
                nocLayout.DOScaleX(0f, .3f)
                    .OnComplete(() => nocLayout.Hide());
                chuaShowNoc = true;
                isShowComplete = false;
                btnXemNoc.image.raycastTarget = true;
            });
        }
        else
        {
            isShowComplete = false;
            delay?.Kill();
            nocLayout.DOScaleX(0f, .3f)
                .OnComplete(() => nocLayout.Hide());
            chuaShowNoc = true;
            btnXemNoc.image.raycastTarget = true;
        }
    }

    /** eventHandlers **/
    /**mograte from showDrewCard*/
    private void OnCardDraw(SDCard card)
    {
        try
        {
            DestroyImmediate(nocLayout.GetChild(0).gameObject);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        txtCount.text = nocModel.cards.Count.ToString();
    }
}