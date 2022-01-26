using DG.Tweening;
using UnityEngine;

public interface IAnimation
{
    Sequence OnStart();
    
    Sequence OnReverse();
    Sequence OnStop();
}