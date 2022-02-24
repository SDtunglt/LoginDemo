using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName, txtCoin;
    [SerializeField] private Button bg, avatar;

    private ItemReqPlaySignal itemReqPlaySignal = Signals.Get<ItemReqPlaySignal>();
    
    public int uid;
    public void UpdateInfo(string name, double coin)
    {
        txtName.text = name;
        txtCoin.text = StringUtils.FormatMoney(coin);
    }

    public void OnClick()
    {
        itemReqPlaySignal.Dispatch(uid);
        bg.interactable = avatar.interactable = false;
    }
}