using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIBaseAnimation : UIDefaultAnimation
{
    public RectTransform content;
    [SerializeField] protected CanvasGroup canvasGroup;
    protected Sequence mainSequence;

    private void OnValidate()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override Sequence OnStart()
    {
        mainSequence?.Kill();
        canvasGroup.blocksRaycasts = true;
        mainSequence = DOTween.Sequence();
        return mainSequence;
    }

    public override Sequence OnReverse()
    {
        canvasGroup.blocksRaycasts = false;
        mainSequence?.Kill();
        mainSequence = DOTween.Sequence();
        return mainSequence;
    }

    public override Sequence OnStop()
    {
        mainSequence?.Pause();
        return mainSequence;
    }
}