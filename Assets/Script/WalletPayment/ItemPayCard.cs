using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPayCard : MonoBehaviour
{
    [SerializeField] private TMP_Text txtCoin, txtInfo, txtPrince, txtSaleOff, txtKm;
    [SerializeField] private GameObject objKm;
    [SerializeField] private Image icon;
    [SerializeField] private List<Sprite> listIcons;

    public int id;

    public void SetValue(int index, long coin, int prince, string info)
    {
        id = prince;
        txtCoin.text = StringUtils.FormatMoney(coin);
        txtPrince.text = StringUtils.FormatMoney(prince);
        txtSaleOff.text = StringUtils.FormatMoney(prince * 100);
        txtInfo.text = $"{info} + <color=#F3B400><b>{StringUtils.FormatMoney((int) (prince / 1000))}</b></color> ĐCT";

        if (index > listIcons.Count - 1) index = listIcons.Count - 1;
        icon.sprite = listIcons[index];
        icon.SetNativeSize();

        if (string.IsNullOrEmpty(UserModel.Instance.kmValue)) return;
        var km = UserModel.Instance.kmValue;
        objKm.SetActive(!string.IsNullOrEmpty(km) && int.Parse(km) > 0);
        txtKm.text = $"KM\n{UserModel.Instance.kmValue} Tỷ lệ";
    }

    public void OnPayCard()
    {
        //LoadingEffect.Open();
        PayCardController.Open(id);
    }
}