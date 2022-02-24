using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardMediator : MonoBehaviour
{
    [SerializeField] private ClockColor _clockColor;
    [SerializeField] private Image mark;
    private SDCard _sdCard;

    private CardUEffectSignal cardUEffectSignal = Signals.Get<CardUEffectSignal>();
    
    private Sequence sequence;
    private float dt = .5f;

    private void Awake()
    {
        cardUEffectSignal.AddListener(ShowMark);
    }

    private void OnDestroy()
    {
        cardUEffectSignal.RemoveListener(ShowMark);
        mark.Hide();
        sequence?.Kill();
    }
    public void SetData(SDCard sdCard)
    {
        _sdCard = sdCard;
    }

    public SDCard GetSDCard()
    {
        return _sdCard;
    }

    public void StartClock(int actIndex)
    {
        _clockColor.StartCountDown(actIndex,SDTimeout.U_NORMAL, SDTimeout.U_NORMAL, () =>
        {
            _clockColor.gameObject.SetActive(false);
            CanUCardsModel.Instance.removeCanUCard(_sdCard);
        });
    }

    public void StopClock()
    {
        _clockColor.gameObject.SetActive(false);
    }

    private void ShowMark(SDCard c)
    {
        if (c != _sdCard) return;
        mark.Show();
        sequence?.Kill();
        sequence = DOTween.Sequence()
            .Append(mark.DOFade(0, dt).OnComplete(FadeOn).SetEase(Ease.Linear));
    }

    private void FadeOn()
    {
        mark.DOFade(1, dt).OnComplete(FadeOff).SetEase(Ease.Linear);
    }
    
    private void FadeOff()
    {
        mark.DOFade(0, dt).OnComplete(FadeOn).SetEase(Ease.Linear);
    }
}
