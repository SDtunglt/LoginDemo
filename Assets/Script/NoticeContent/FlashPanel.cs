using System;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class FlashPanel : MonoBehaviour
{
    private static FlashPanel prefab;
    public Image flashImage;
    private Sequence flashSequence;
    public float fadeInTime = 0.15f, fadeOutTime = 0.15f, maxOpacityTime = 0.1f;

    private void OnValidate()
    {
        flashImage = GetComponentInChildren<Image>();
    }
    
    public void ShowFlashWithCallBack(Action callBack)
    {
        flashImage.ChangeAlpha(0);
        flashSequence?.Kill();
        flashSequence = DOTween.Sequence().Append(flashImage.DOFade(1, fadeInTime).OnComplete(callBack.Invoke))
            .AppendInterval(maxOpacityTime)
            .Append(flashImage.DOFade(0, fadeOutTime).OnComplete(Close));
        flashSequence.Restart();
    }

    private void Close()
    {
        LeanPool.Despawn(gameObject);
    }

    public static FlashPanel Open()
    {
        if (!prefab)
        {
            // SDLogger.Log("Load Prefab");
            prefab = Resources.LoadAll<FlashPanel>("Panels").FirstOrDefault();
        }
        var panel = LeanPool.Spawn(prefab);
        return panel;
    }

}