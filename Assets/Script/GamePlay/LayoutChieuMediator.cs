using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LayoutChieuMediator : MonoBehaviour
{
    // [SerializeField] private MyCardMediator cardInHand;
    [SerializeField] private Vector2 cardSize;
    [SerializeField] private List<RectTransform> anArea, danhArea;
    [SerializeField] private List<bool> isAnRight, isDanhRight;
    private IPlayersContainer _gamePlayMediator;
    [SerializeField] private GameObject playerContainer;

    private readonly StopGameSignal stopGameSignal = Signals.Get<StopGameSignal>();

    private GameObject currCard;
    public List<CardMediator> lisCardMediator;

    private static LayoutChieuMediator _Instance;
    private bool isCardLoaded;
    private GameObject _card;

    private GameObject card
    {
        get
        {
            if (!isCardLoaded)
            {
                _card = Resources.Load<GameObject>("Card");
            }

            return _card;
        }
    }

    public static LayoutChieuMediator Instance => _Instance;

    private void Awake()
    {
        _Instance = this;
        _gamePlayMediator = playerContainer.GetComponent<IPlayersContainer>();
        stopGameSignal.AddListener(OnStopGame);
    }

    private void OnDestroy()
    {
        stopGameSignal.RemoveListener(OnStopGame);
    }

    private void OnStopGame()
    {
        while (lisCardMediator.Count > 0)
        {
            lisCardMediator.RemoveAt(lisCardMediator.Count - 1);
        }

        for (var i = 0; i < anArea.Count; i++)
        {
            for (var j = anArea[i].childCount - 1; j >= 0; j--)
            {
                Destroy(anArea[i].GetChild(j).gameObject);
            }

            for (var j = danhArea[i].childCount - 1; j >= 0; j--)
            {
                Destroy(danhArea[i].GetChild(j).gameObject);
            }
        }
    }

    [Button]
    private void AddCardToDanhArea(int c, int idx, bool isDraw)
    {
        card.GetComponent<RectTransform>().sizeDelta = cardSize;
        var cardObject = Instantiate(card, danhArea[idx]);
        currCard = cardObject;
        lisCardMediator.Add(cardObject.GetComponent<CardMediator>());

        cardObject.transform.position = Vector3.zero;
        float xPos = danhArea[idx].sizeDelta.x / 2 - cardSize.x / 2 -
                     (cardSize.x - 3) * (danhArea[idx].transform.childCount - 1);
        cardObject.transform.localPosition = isDanhRight[idx] ? new Vector3(xPos, 0, 0) : new Vector3(-xPos, 0, 0);
        if(danhArea[idx].childCount > danhArea[idx].sizeDelta.x / cardSize.x)
        {
            UpdateCardPositionDanhAreaImmediate(danhArea[idx], isDanhRight[idx]);
        }
    }

    public void AddCardFromNocToDanhArea(SDCard c, int idx, bool isDraw = false)
    {
        card.GetComponent<RectTransform>().sizeDelta = cardSize;
        var cardObject = Instantiate(card, danhArea[idx]);
        cardObject.GetComponent<Image>().sprite = c.GetSprite();
        if (GamePlayModel.Instance.MoBocNoc) cardObject.GetComponent<Image>().color = new Color(.7f, .7f, .7f);
        currCard = cardObject;
        lisCardMediator.Add(cardObject.GetComponent<CardMediator>());
        cardObject.GetComponent<CardMediator>().SetData(c);

        cardObject.transform.position = Vector3.zero;
        float xPos = danhArea[idx].sizeDelta.x / 2 - cardSize.x / 2 -
                     (cardSize.x - 3) * (danhArea[idx].transform.childCount - 1);
        if (isDanhRight[idx]) cardObject.transform.DOLocalMove(new Vector3(xPos, 0, 0), 0.3f);
        else cardObject.transform.DOLocalMove(new Vector3(-xPos, 0, 0), 0.3f);
        if(danhArea[idx].childCount > danhArea[idx].sizeDelta.x / cardSize.x)
        {
            UpdateCardPositionDanhArea(danhArea[idx], isDanhRight[idx]);
        }
    }

    public void AddCardToDanhArea(SDCard c, int idx)
    {
        card.GetComponent<RectTransform>().sizeDelta = cardSize;
        var cardObject = Instantiate(card, danhArea[idx]);
        cardObject.GetComponent<Image>().sprite = c.GetSprite();
        currCard = cardObject;
        lisCardMediator.Add(cardObject.GetComponent<CardMediator>());
        cardObject.GetComponent<CardMediator>().SetData(c);

        cardObject.transform.position = _gamePlayMediator.GetPlayer(idx).tfCountdown.position;
        float xPos = danhArea[idx].sizeDelta.x / 2 - cardSize.x / 2 -
                     (cardSize.x - 3) * (danhArea[idx].transform.childCount - 1);
        if (isDanhRight[idx]) cardObject.transform.DOLocalMove(new Vector3(xPos, 0, 0), 0.3f);
        else cardObject.transform.DOLocalMove(new Vector3(-xPos, 0, 0), 0.3f);
        if(danhArea[idx].childCount > danhArea[idx].sizeDelta.x / cardSize.x)
        {
            UpdateCardPositionDanhArea(danhArea[idx], isDanhRight[idx]);
        }
    }

    public void AddCardToTraCuaArea(SDCard c, int playerTraCuaIdx, int idx)
    {
        card.GetComponent<RectTransform>().sizeDelta = cardSize;
        var cardObject = Instantiate(card, danhArea[idx]);
        cardObject.GetComponent<Image>().sprite = c.GetSprite();
        currCard = cardObject;
        lisCardMediator.Add(cardObject.GetComponent<CardMediator>());
        cardObject.GetComponent<CardMediator>().SetData(c);

        cardObject.transform.position = _gamePlayMediator.GetPlayer(playerTraCuaIdx).transform.position;
        float xPos = danhArea[idx].sizeDelta.x / 2 - cardSize.x / 2 -
                     (cardSize.x - 3) * (danhArea[idx].transform.childCount - 1);
        if (isDanhRight[idx]) cardObject.transform.DOLocalMove(new Vector3(xPos, 0, 0), 0.3f);
        else cardObject.transform.DOLocalMove(new Vector3(-xPos, 0, 0), 0.3f);
        if(danhArea[idx].childCount > danhArea[idx].sizeDelta.x / cardSize.x)
        {
            UpdateCardPositionDanhArea(danhArea[idx], isDanhRight[idx]);
        }
    }

    public void AddCardToAnArea(List<SDCard> cards, int idx, float cardDistance)
    {
        currCard.transform.SetParent(anArea[idx]);

        float xPos = anArea[idx].sizeDelta.x / 2 - cardSize.x / 2 - cardSize.x * (anArea[idx].transform.childCount - 1);
        if (isAnRight[idx]) currCard.transform.DOLocalMove(new Vector3(xPos, 0, 0), 0.3f);
        else currCard.transform.DOLocalMove(new Vector3(-xPos, 0, 0), 0.3f);

        for (int i = 0; i < cards.Count; i++)
        {
            card.GetComponent<RectTransform>().sizeDelta = cardSize;
            var cardObject = Instantiate(card, currCard.transform);
            cardObject.GetComponent<Image>().sprite = cards[i].GetSprite();
            cardObject.transform.position = _gamePlayMediator.GetPlayer(idx).transform.position;
            cardObject.transform.DOLocalMove(new Vector3(0, -(i + 1) * cardDistance, 0), 0.3f);
        }
    }

    private void UpdateCardPositionDanhArea(RectTransform danhArea, bool isRight)
    {
        float child0 = isRight ? danhArea.sizeDelta.x / 2 - cardSize.x / 2 : -danhArea.sizeDelta.x / 2 + cardSize.x / 2;
        for (int i = 1; i < danhArea.childCount; i++)
        {
            float xPos = child0 + (danhArea.sizeDelta.x - cardSize.x) / (danhArea.childCount - 1) * i;
            if (isRight)
            {
                xPos = child0 - (danhArea.sizeDelta.x - cardSize.x) / (danhArea.childCount - 1) * i;
            }
            // if(i == 1) SDLogger.Log("DanhArea: " + xPos);
            danhArea.GetChild(i).transform.DOLocalMove(new Vector3(xPos, 0, 0), 0.3f);
        }
    }
    
    private void UpdateCardPositionDanhAreaImmediate(RectTransform danhArea, bool isRight)
    {
        float child0 = isRight ? danhArea.sizeDelta.x / 2 - cardSize.x / 2 : -danhArea.sizeDelta.x / 2 + cardSize.x / 2;
        for (int i = 1; i < danhArea.childCount; i++)
        {
            float xPos = child0 + (danhArea.sizeDelta.x - cardSize.x) / (danhArea.childCount - 1) * i;
            if (isRight)
            {
                xPos = child0 - (danhArea.sizeDelta.x - cardSize.x) / (danhArea.childCount - 1) * i;
            }
            SDLogger.Log("DanhArea: " + xPos);
            danhArea.GetChild(i).transform.localPosition = new Vector3(xPos, 0, 0);
        }
    }
}