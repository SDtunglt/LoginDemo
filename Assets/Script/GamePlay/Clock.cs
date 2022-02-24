using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public static int COUNT_FIRST = 0;
    public static int COUNT_TURN = 1;
    public static int COUNT_FOR_MYPLAYER_CHIU = 2;

    public static Clock Instance { get; private set; }

    [SerializeField] private Image clockGreen;
    [SerializeField] private List<PlayerMediator> players;
    [SerializeField] private Transform myFirstCountTransform, myClockTransform;
    [SerializeField] private List<Sprite> listClockSprite;
    private MyCardMediator cardInHand;

    private ClockTimeOutSignal clockTimeOutSignal = Signals.Get<ClockTimeOutSignal>();

    public float timeLeft;
    private int countingCode;
    private float timeInThisTurn;
    private Coroutine cd;
    public float deltaTime = 1;

    private void Awake()
    {
        Instance = this;
        Signals.Get<OnChangeClockTime>().AddListener(OnChangeClockTime);
    }

    private void OnChangeClockTime(float t)
    {
        timeLeft = timeInThisTurn - t;
        clockGreen.type = Image.Type.Filled;
        clockGreen.fillMethod = Image.FillMethod.Radial360;
        clockGreen.fillOrigin = (int) Image.Origin360.Top;
        clockGreen.fillAmount = timeLeft / timeInThisTurn;
        if(timeLeft > 0) StartCountDown(timeLeft);
        clockGreen.DOKill();
        clockGreen.DOFillAmount(0,timeLeft)
            .From(timeLeft / timeInThisTurn).SetEase(Ease.Linear)
            .OnComplete(OnTimeOut);
    }

    private void OnDestroy()
    {
        Signals.Get<OnChangeClockTime>().RemoveListener(OnChangeClockTime);
    }

    public int seat = 0;

    public void StartClock(int code, float dt, SDPlayer p)
    {
        clockGreen.Show();
        countingCode = code;
        timeInThisTurn = dt;
        seat = p.seat;
        SDLogger.Log("SeatPlayer: " + seat + " " + p.name);
        cardInHand = FindObjectOfType<MyCardMediator>();
        if(GamePlayModel.Instance.isPlayer && seat == 0 && !GamePlayModel.IsReplay)
        {
            clockGreen.sprite = listClockSprite[1];
            clockGreen.transform.localScale = new Vector3(1.2f,1.2f,0);
            if (!cardInHand.Sorted()) 
            {
                clockGreen.transform.position = myFirstCountTransform.position + new Vector3(.02f, 0, 0);
            }
            else if (code == COUNT_FIRST)
            {
                clockGreen.transform.position = myFirstCountTransform.position + new Vector3(0, .1f, 0);
            }
            else
            {
                clockGreen.transform.position = myClockTransform.position;
            }
        }
        else
        {
            clockGreen.transform.position = players[seat].tfCountdown.position;
            clockGreen.sprite = listClockSprite[0];
            clockGreen.transform.localScale = new Vector3(1f,1f,0);
        }

        clockGreen.type = Image.Type.Filled;
        clockGreen.fillMethod = Image.FillMethod.Radial360;
        clockGreen.fillOrigin = (int) Image.Origin360.Top;
        clockGreen.fillAmount = 1f;
        if(dt > 0) StartCountDown(dt);
        clockGreen.DOKill();
        clockGreen.DOFillAmount(0, dt)
            .From(1).SetEase(Ease.Linear)
            .OnComplete(() => { OnTimeOut(); });
        if(p.bao || p.dis)
        {
            OnTimeOut();
        }
    }

    private void StartCountDown(float timeCd)
    {
        if(cd != null) StopCoroutine(cd);
        cd = StartCoroutine(ICountDown(timeCd));
    }

    IEnumerator ICountDown(float timeCd)
    {
        timeLeft = timeCd;
        while(timeLeft > 0)
        {
            yield return new WaitForSeconds(deltaTime);
            timeLeft -= deltaTime;
            if(GamePlayModel.Instance.isPlayer && seat == 0 && timeLeft <= 7f)
            {
                Signals.Get<AutoSortCardSignal>().Dispatch();
            }
        }
    }

    public void OnTimeOut()
    {
        clockGreen.Hide();
        clockTimeOutSignal.Dispatch(countingCode);
    }

    public void UpdatePositionAfterSortCards()
    {
        if(seat != 0) return;
        clockGreen.transform.position = myClockTransform.position;
    }

    public void StopCountDown()
    {
        if(cd != null) StopCoroutine(cd);
        clockGreen.Hide();
    }

}
