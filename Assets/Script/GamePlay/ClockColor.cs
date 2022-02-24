using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ClockColor : MonoBehaviour
{
    [SerializeField] private Image imgProgressBar;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color warningColor;
    private int actIndex;
    private float lastTime;
    private float maxTime;

    private Sequence seq;
    private Action callBack;

    public void StartCountDown(int index, float t, float max, Action action = null)
    {
        gameObject.SetActive(true);
        actIndex = index;
        lastTime = t;
        maxTime = max;
        callBack = action;
        seq?.Kill();
        imgProgressBar.DOKill();
        imgProgressBar.color = normalColor;
        imgProgressBar.DOFillAmount(0, t).From(t / max).OnComplete(() => { callBack?.Invoke(); }).SetEase(Ease.Linear);

        var t2 = t - max * 0.55f;
        seq = DOTween.Sequence().AppendInterval(t2 < 0 ? 0 : t2).OnStepComplete(() =>
        {
            imgProgressBar.color = warningColor;
            seq = null;
        }).SetEase(Ease.Linear);
    }

    public void StopCountDown()
    {
        seq?.Kill();
        imgProgressBar.DOKill();
    }

    private void OnEnable()
    {
        Signals.Get<OnChangeCardClockTime>().AddListener(OnChangeClockTime);
    }

    private void OnChangeClockTime(int index, float t)
    {
        if (actIndex == index)
        {
            if (t < lastTime)
            {
                seq?.Kill();
                imgProgressBar.DOKill();
                imgProgressBar.color = normalColor;
                Debug.Log("Change clock color progress");
                t = lastTime - t;
                imgProgressBar.DOFillAmount(0, t).From(t / maxTime).OnComplete(() => { callBack?.Invoke(); }).SetEase(Ease.Linear);

                var t2 = t - maxTime * 0.55f;
                seq = DOTween.Sequence().AppendInterval(t2 < 0 ? 0 : t2).OnStepComplete(() =>
                {
                    imgProgressBar.color = warningColor;
                    seq = null;
                }).SetEase(Ease.Linear);
            }
            else
            {
                callBack?.Invoke();
                gameObject.Hide();
            }
        }
    }

    private void OnDisable()
    {
        StopCountDown();
        Signals.Get<OnChangeCardClockTime>().RemoveListener(OnChangeClockTime);
    }
}