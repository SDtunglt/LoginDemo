using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIDefaultAnimation : MonoBehaviour, IAnimation
{
    protected Sequence mainSequence;

    [Button]
    void Play()
    {
        OnStart();
    }
    
    public virtual Sequence OnStart()
    {
        mainSequence?.Kill();
        mainSequence = DOTween.Sequence();
        return mainSequence;
    }

    public virtual Sequence OnReverse()
    {
        mainSequence?.Kill();
        mainSequence = DOTween.Sequence();
        return mainSequence;
    }

    public virtual Sequence OnStop()
    {
        mainSequence?.Pause();
        return mainSequence;
    }
}