using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemWallet : MonoBehaviour
{
    [SerializeField] private TMP_Text txtPrice;
    [SerializeField] private TMP_Text txtAmount, txtKm;
    [SerializeField] private GameObject objKm;

    public string id;
    public int price;
    public long amount;

    public void SetupView(string _id, int _price, long _amount)
    {
        id = _id;
        price = _price;
        amount = _amount;
        // if (!GameModel.Instance.IsNormalPlayer()) amount = price * 100;
        txtPrice.text = StringUtils.FormatMoney(price) + " VND";
        txtAmount.text = StringUtils.FormatMoney(amount);
        SDLogger.LogError(_id + "  " + _price + "  " + _amount);
        if (string.IsNullOrEmpty(UserModel.Instance.kmValue)) return;
        var km = UserModel.Instance.kmValue;
        if (GameModel.Instance.IsNormalPlayer())
        {
            objKm.SetActive(!string.IsNullOrEmpty(km) && int.Parse(km) > 0);
        }
        else objKm.SetActive(false);
        // txtKm.text = $"{UserModel.Instance.kmValue} Tỷ lệ";
        txtKm.text = $"{UserModel.Instance.kmValue}";
    }

    public void OnClickItem()
    {
#if UNITY_WEBGL
        BasicPopup.Open("Thông báo", "Tính năng này chỉ có cho phiên bản trên smartphone.", "Đồng ý");
#endif

#if UNITY_ANDROID || UNITY_IOS
        Debug.LogError("Unity Buy: " + id);
        MyIAPManager.Instance.BuyProductID(id);
#endif
    }

    // public void SetupLocalizedView(string _id, string _price, long _amount)
    // {
    //     id = _id;
    //     amount = _amount;
    //     // if (!GameModel.Instance.IsNormalPlayer()) amount = price * 100;
    //     txtPrice.text = _price;
    //     txtAmount.text = StringUtils.FormatMoney(amount);
    //     SDLogger.LogError(_id + "  " + _price + "  " + _amount);
    //     if (string.IsNullOrEmpty(UserModel.Instance.kmValue)) return;
    //     var km = UserModel.Instance.kmValue;
    //     if (GameModel.Instance.IsNormalPlayer())
    //     {
    //         objKm.SetActive(!string.IsNullOrEmpty(km) && int.Parse(km) > 0);
    //     }
    //     else objKm.SetActive(false);
    //     // txtKm.text = $"{UserModel.Instance.kmValue} Tỷ lệ";
    //     txtKm.text = $"{UserModel.Instance.kmValue}";
    // }

    // public void SetLocalizedPrices(string _price)
    // {
    //     txtPrice.text = _price;
    // }
}