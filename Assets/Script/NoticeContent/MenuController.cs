using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuController: MonoBehaviour
{
    private static MenuController _ins;
    public static MenuController Ins
    {
        get
        {
            if (_ins != null)
            {
                return _ins;
            }
            else
            {
                _ins = GameObject.FindGameObjectWithTag("MenuGroup").GetComponent<MenuController>();
                return _ins;
            }
        }
    }

    public ButtonGroupController buttonGroupController;
    public Button menuBtn;
    public RectTransform moveRect;

    public float baseSizeX;
    public float baseHiddenPos, baseShowPos;
    public float timeMove;
    public float hiddenPos, showPos;
    
    public bool isShow;
    private Tween moveTween;

    private void Start()
    {
        menuBtn.onClick.AddListener(ChangeMenuState);
        UpdatePosConfig();
        HiddenImmediate();
    }

    private void ChangeMenuState()
    {
        moveTween?.Kill();
        // moveTween = moveRect.DOAnchorPosX(isShow ? hiddenPos : showPos, timeMove);
        isShow = !isShow;
    }

    public void HiddenImmediate()
    {
        isShow = false;
        moveTween?.Kill();
        // moveTween = moveRect.DOAnchorPosX(hiddenPos, timeMove);

    }
    
    public void ShowImmediate()
    {
        isShow = true;
        moveTween?.Kill();
        // moveTween = moveRect.DOAnchorPosX(showPos, timeMove);

    }

    public void UpdatePosition()
    {
        moveTween?.Kill();
        // moveRect.ChangeAnchorX(isShow ? showPos : hiddenPos);
    }

    private void UpdatePosConfig()
    {
        showPos = baseShowPos;
        hiddenPos = baseHiddenPos - (baseSizeX - moveRect.sizeDelta.x);
    }

    public void HideButton(List<ButtonGroupType> types)
    {
        buttonGroupController.HideButton(types);
        this.WaitNewFrame(() =>
        {
            UpdatePosConfig();
            UpdatePosition();
        });
    }
}