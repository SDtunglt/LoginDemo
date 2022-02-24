using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SumupMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txtMsg;

    private void OnEnable()
    {
        transform.DOScaleY(1, .5f).SetEase(Ease.OutBack);
    }

    public void ShowMsg(string msg)
    {
        txtMsg.text = msg;
    }

    public void Hide()
    {
        txtMsg.text = "";
        gameObject.SetActive(false);
        transform.DOScaleY(0, 0);
    }
}