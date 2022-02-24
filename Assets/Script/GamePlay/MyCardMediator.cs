using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MyCardMediator : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IDragHandler
{
    [SerializeField] private Button btnXep , btnXong;
    [SerializeField] private GameObject grButtons, cardPrefabs;
    [SerializeField] private Transform sortManualCardsHolder;
    [SerializeField] private Clock clock;

    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
    private PlayModel playModel = PlayModel.Instance;
    private ScreenManager screenManager;

    private RemoveCardsFromMyCardsSignal removeCardsFromMyCardsSignal = Signals.Get<RemoveCardsFromMyCardsSignal>();
    private StopGameSignal stopGameSignal = Signals.Get<StopGameSignal>();
    private AutoSortCardSignal autoSortCardSignal = Signals.Get<AutoSortCardSignal>();
    private ResumeCompletedSignal resumeCompletedSignal = Signals.Get<ResumeCompletedSignal>();

    private List<SDCard> cards;
    private List<SDCard> listSortSDCards = new List<SDCard>();

    public RectTransform curveLayout;
    public float radius = 100f;
    public float arc = 60f;
    public Vector2 cardSize;
    private Transform _selectingCard;
    private bool _dragged;
    private bool _sorted;
    private void Awake()
    {
        screenManager = ScreenManager.Instance;
        removeCardsFromMyCardsSignal.AddListener(RemoveCardInHand);
        stopGameSignal.AddListener(OnStopGame);
        autoSortCardSignal.AddListener(OnAutoSortCard);
        resumeCompletedSignal.AddListener(OnResume);
        Signals.Get<OnHandlePlayResumeSignal>().AddListener(OnHandlePlayAct);
    }

    private void OnHandlePlayAct()
    {
        TurnOffXepXongBtn();
    }

    private void OnEnable()
    {
        cards = gamePlayModel.myPlayer.cards;
        btnXep.gameObject.SetActive(!isManuallySort);
        btnXong.gameObject.SetActive(isManuallySort);

        if(gamePlayModel.resuming && !isManuallySort)
        {
            SortCard();
        }
        else
        {
            SpreadCards();
        }
    }

    private void OnDisable()
    {
        playModel.liftedCard = null;
        cards = null;
    }

    private void OnDestroy()
    {
        removeCardsFromMyCardsSignal.RemoveListener(RemoveCardInHand);
        stopGameSignal.RemoveListener(OnStopGame);
        autoSortCardSignal.RemoveListener(OnAutoSortCard);
        resumeCompletedSignal.RemoveListener(OnResume);
        Signals.Get<OnHandlePlayResumeSignal>().RemoveListener(OnHandlePlayAct);
    }

    private bool isManuallySort => screenManager.InChallenge()
        ? !gamePlayModel.IsAutoXep
        : screenManager.zoneInfo.canManuallySortCards;
    
    private void LateUpdate()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        sortManualCardsHolder.position = new Vector3(pos.x, pos.y + 0.1f, 0);
    }

    private void OnResume()
    {
        if(!isManuallySort) return;
        RestorePrevCards();
    }

    private void OnStopGame()
    {
        for(var i = curveLayout.childCount - 1; i >= 0;i--)
        {
            DestroyImmediate(curveLayout.GetChild(i).gameObject);
        }

        btnXep.Show();
        grButtons.Hide();
        this.Hide();
    }

    private void SpreadCards()
    {
        //StartCoroutine(SoundManager.Instance.PlaySoundCard(new List<string> {"Xoe"}));
        _sorted = false;
        UpdateChild();

        for(var i = 0; i < curveLayout.childCount;i++)
        {
            var childRTrans = (RectTransform) curveLayout.GetChild(i);
            var targetEulerAngles = Vector3.forward * (arc * (curveLayout.childCount - 1) / 2 -i  * arc);
            childRTrans.DOKill();
            childRTrans.DOSizeDelta(cardSize,0.3f);
            childRTrans.DOLocalRotate(targetEulerAngles,0.5f).OnUpdate(() =>
            {
                childRTrans.anchoredPosition = Rotate(Vector2.up, childRTrans.localEulerAngles.z) * Mathf.Lerp(
                    -(childRTrans.anchoredPosition - Vector2.down * radius).magnitude,
                    radius,0.3f) + Vector2.down * radius;
            }).SetEase(Ease.OutQuad);
        }
    }

    private void UpdateChild()
    {
        for (var i = curveLayout.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(curveLayout.transform.GetChild(i).gameObject);
        }

        foreach (var card in cards)
        {
            var cardObject = Instantiate(cardPrefabs, curveLayout.transform);
            cardObject.GetComponent<Image>().sprite = card.GetSprite();
        }
    }

    private static Vector2 Rotate(Vector2 v, float delta)
    {
        delta *= Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    private void UpdateCards()
    {
        bool haveSelectedCard = false;
        for (var i = 0; i < curveLayout.transform.childCount; i++)
        {
            if (cards[i].IsSelected)
            {
                haveSelectedCard = true;
                playModel.liftedCard = cards[i];
            }

            var childRTrans = (RectTransform) curveLayout.transform.GetChild(i);
            var targetEulerAngles = Vector3.forward * (arc * (curveLayout.transform.childCount - 1) / 2 - i * arc);
            var targetLength = radius + (_selectingCard == childRTrans || cards[i].IsSelected ? 30 : 0);
            childRTrans.DOKill();
            childRTrans.DOSizeDelta(cardSize, 0.3f);
            childRTrans.DOLocalRotate(targetEulerAngles, 0.5f).OnUpdate(() =>
            {
                childRTrans.anchoredPosition = Rotate(Vector2.up, childRTrans.localEulerAngles.z) *
                                               Mathf.Lerp(
                                                   (childRTrans.anchoredPosition - Vector2.down * radius).magnitude,
                                                   targetLength, 0.3f) +
                                               Vector2.down * radius;
            }).SetEase(Ease.OutQuad);
        }

        if (!haveSelectedCard) playModel.liftedCard = null;
    }

    private void UpdateCardsNoDuration()
    {
        for (var i = 0; i < curveLayout.transform.childCount; i++)
        {
            if (cards[i].IsSelected) playModel.liftedCard = cards[i];
            var childRTrans = (RectTransform) curveLayout.transform.GetChild(i);
            var targetEulerAngles = Vector3.forward * (arc * (curveLayout.transform.childCount - 1) / 2 - i * arc);
            var targetLength = radius + (_selectingCard == childRTrans || cards[i].IsSelected ? 50 : 0);
            childRTrans.DOKill();
            childRTrans.sizeDelta = cardSize;
            childRTrans.localEulerAngles = targetEulerAngles;
            childRTrans.anchoredPosition = Rotate(Vector2.up, childRTrans.localEulerAngles.z) *
                                           Mathf.Lerp(
                                               (-Vector2.down * radius).magnitude,
                                               targetLength, 0.6f) +
                                           Vector2.down * radius;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isManuallySort && !_sorted)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) curveLayout.transform,
                eventData.position,
                eventData.pressEventCamera, out var localPos);
            var idx = (int) ((curveLayout.transform.GetChild(0).localEulerAngles.z + arc / 2 -
                              Vector2.SignedAngle(Vector2.up, localPos - Vector2.down * radius)) / arc);
            idx = Mathf.Clamp(idx, 0, curveLayout.transform.childCount - 1);

            if (listSortSDCards == null) listSortSDCards = new List<SDCard>();
            listSortSDCards.Add(cards[idx]);
            // cards.RemoveAt(idx);

            var cardSelect = curveLayout.GetChild(idx);
            Instantiate(cardSelect, sortManualCardsHolder);

            curveLayout.GetChild(idx).gameObject.SetActive(false);

            var childCount = sortManualCardsHolder.childCount;
            sortManualCardsHolder.GetChild(childCount - 1).DOKill();
            sortManualCardsHolder.GetChild(childCount - 1).localPosition = Vector3.up * cardSize.y / 2;
            sortManualCardsHolder.GetChild(childCount - 1).eulerAngles = Vector3.forward * Random.Range(-5f, 5f);

            // UpdateCards();
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) curveLayout.transform,
                eventData.position,
                eventData.pressEventCamera, out var localPos);
            var idx = (int) ((curveLayout.transform.GetChild(0).localEulerAngles.z + arc / 2 -
                              Vector2.SignedAngle(Vector2.up, localPos - Vector2.down * radius)) / arc);
            idx = Mathf.Clamp(idx, 0, curveLayout.transform.childCount - 1);
            if (!cards[idx].IsSelected)
            {
                DeselectAllCards();
                UpdateCards();
            }

            _selectingCard = curveLayout.transform.GetChild(idx);
            _dragged = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isManuallySort && !_sorted) return;
        if (!_dragged)
        {
            var selectingCardIdx = _selectingCard.GetSiblingIndex();
            for (var i = 0; i < curveLayout.childCount; i++)
            {
                if (i != selectingCardIdx)
                {
                    cards[i].IsSelected = false;
                }
                else
                {
                    cards[i].IsSelected = !cards[i].IsSelected;
                }
            }
        }
        else
        {
            DeselectAllCards();
        }

        _selectingCard = null;
        UpdateCards();
        if (isManuallySort) SaveSortedCards();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isManuallySort && !_sorted) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) curveLayout.transform,
            eventData.position, eventData.pressEventCamera, out var localPos);
        var childCount = curveLayout.transform.childCount;
        var firstCardAngle = arc * childCount / 2;
        var idx = (int) ((firstCardAngle - Vector2.SignedAngle(Vector2.up, localPos - Vector2.down * radius)) / arc);
        idx = Mathf.Clamp(idx, 0, childCount - 1);
        if ((localPos - Vector2.down * radius).magnitude > 100f && _selectingCard != null &&
            _selectingCard.GetSiblingIndex() != idx)
        {
            _dragged = true;

            var tmpCard = cards[_selectingCard.GetSiblingIndex()];
            cards.RemoveAt(_selectingCard.GetSiblingIndex());
            cards.Insert(idx, tmpCard);

            _selectingCard.SetSiblingIndex(idx);

            UpdateCards();
        }
    }

    public void DoneSortCard()
    {
        //StartCoroutine(SoundManager.Instance.PlaySoundCard(new List<string> {"Xoe"}));
        Signals.Get<ShowTimeOutMsgSignal>().Dispatch("");
        SortCard();
        if (isManuallySort) SaveSortedCards();
    }

    public void SortCard()
    {
        if (!isManuallySort)
        {
            SDCard.Sort(cards);
        }
        else
        {
            for (var i = sortManualCardsHolder.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(sortManualCardsHolder.GetChild(i).gameObject);
            }

            UpdateListCards();
        }

        UpdateChild();
        UpdateCards();

        btnXep.Hide();
        btnXong.Hide();
        grButtons.Show();
        _sorted = true;
        clock.UpdatePositionAfterSortCards();
    }

    private void UpdateListCards()
    {
        foreach (var c in listSortSDCards)
        {
            var i = cards.IndexOf(c);
            if (i != -1) cards.RemoveAt(i);
        }

        cards = listSortSDCards.Union(cards).ToList();
        gamePlayModel.myPlayer.cards = cards;
        listSortSDCards.Clear();
    }

    private void SaveSortedCards()
    {
        if (!isManuallySort) return;
        var tempArr = cards.Select(c => c.v).ToList();
        GameUtils.SetCardSaved(tempArr);
    }

    private void RestorePrevCards()
    {
        if (!isManuallySort) return;
        var savedCards = GameUtils.GetCardSaved();
        if (savedCards == null || savedCards.Count <= 0) return;
        var myCards = cards.Select(c => c.v).ToList();
        if (!GameUtils.ListIntContains(savedCards, myCards) && !GameUtils.ListIntContains(myCards, savedCards)) return;
        var i = 0;
        var j = 0;
        do
        {
            for (var k = j; k < cards.Count; k++)
                if (cards[k].v == savedCards[i])
                {
                    var c = cards[k];
                    cards[k] = cards[j];
                    cards[j] = c;
                    j++;
                    break;
                }

            i++;
        } while (i < savedCards.Count && j < cards.Count);

        SortCard();
    }

    private void DeselectAllCards()
    {
        for (var i = 0; i < curveLayout.childCount; i++)
        {
            cards[i].IsSelected = false;
        }
    }

    private void RemoveCardInHand(List<SDCard> cl)
    {
        foreach (var card in cl)
        {
            var i = cards.IndexOf(card);
            if (card == playModel.liftedCard) playModel.liftedCard = null;
            cards.RemoveAt(i);
            DestroyImmediate(curveLayout.GetChild(i).gameObject);
        }

        foreach (var card in cards) card.IsSelected = false;

        if (!isManuallySort) SDCard.Sort(cards);
        else if (!gamePlayModel.resuming) SaveSortedCards();
        SortCard();
        UpdateCardsNoDuration();
    }

    private void OnAutoSortCard()
    {
        if (!_sorted)
        {
            SortCard();
        }
    }

    public List<SDCard> GetCards()
    {
        return cards;
    }

    public bool Sorted()
    {
        return _sorted;
    }

    public void TurnOffXepXongBtn()
    {
        btnXong.Hide();
    }
}
