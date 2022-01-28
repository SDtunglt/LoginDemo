using System;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class UIPopup : UIBaseView, IAnimate
{
    public static string Opening;

    private void OnValidate()
    {
        popupAnim = GetComponent<UIDefaultAnimation>();
        if (!popupAnim)
        {
            popupAnim = gameObject.AddComponent<UIScaleAnimation>();
            if (popupAnim.GetComponent<UIScaleAnimation>() && !popupAnim.Cast<UIScaleAnimation>().content)
            {
                popupAnim.Cast<UIScaleAnimation>().content = FindInnChild(transform, "Panel").GetComponent<RectTransform>();
            }
           
        }
    }

    private Transform FindInnChild(Transform t, string s)
    {
        foreach (Transform child in t)
        {
            if (child.name == s)
            {
                return child;
            }
            else
            {
                var m = FindInnChild(child, s);
                if (m)
                {
                    return m;
                }
            }
        }

        return null;
    }
    
    public override void OpenView()
    {
        base.OpenView();
        OnStart();
    }

    public override void Close()
    {
        base.OpenView();
        OnClose();
    }
    
    public void OnStart()
    {
        popupAnim.OnStop();
        popupAnim.OnStart();
    }

    public void OnStop()
    {
        
    }
    
    public virtual void OnClose()
    {
        popupAnim.OnStop();
        popupAnim.OnReverse().OnComplete(() =>
        {
            if (GetGameObject().activeSelf)
            {
                LeanPool.Despawn(this);
            }
        });
    }

}