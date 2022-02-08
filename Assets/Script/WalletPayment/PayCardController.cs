using DG.Tweening;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PayCardController : UIPopup
{
    [SerializeField] private TMP_Text txtInfo, txtWarning, txtPromoInfo;
    [SerializeField] private Toggle tgViettel, tgVina;
    [SerializeField] private TMP_InputField ipfSeri, ipfMaThe;
    [SerializeField] private Button btnNap, btnCallMod;

    private const string VIETTEL = "viettel";
    private const string VINA = "vina";

    private int id;
    public static void Open(int idPay)
    {
        ViewCreator.OpenPopup(PopupId.PayCardPopup, view =>
        {
            var shop = view.Cast<PayCardController>();
            shop.SetupView(idPay);
            shop.OpenView();
            //LoadingEffect.CloseLast();
        });
        
    }

    private void SetupView(int idPay)
    {
        id = idPay;

        btnCallMod.interactable = !GameUtils.IsWeb();
        txtInfo.text = $"Bạn đang nạp thẻ mệnh giá {StringUtils.FormatMoney(idPay)} VNĐ";
        if(PayCardMediator.selectedPromo != null)
        {
            txtPromoInfo.gameObject.SetActive(true);
            int value = PayCardMediator.selectedPromo.pay_percent;
            string info = value == -1 ? "x2 tỷ lệ" : $"+{value} tỷ lệ";
            /*txtPromoInfo.text = SwitchUI.CurrentType == ComponentType.Chan2 ? 
                $"Lần nạp này sử dụng khuyến mại <color=#854635><b>{info}</b></color>" : 
                $"Lần nạp này sử dụng khuyến mãi <color=#F3C700><b>{info}</b></color>";*/
        }
        else
        {
            txtPromoInfo.gameObject.SetActive(false);
        }
        UpdateVina(GameModel.Features[GameModel.VinaFeature]); 
    }
    
    public void OnCallToMod()
    {
        Application.OpenURL("tel://+84988153993");
    }

    public void UpdateVina(bool IsActive)
    {
        GameUtils.IsCTDCActive = IsActive;
        if (!IsActive)
        {
            tgVina.gameObject.SetActive(false);
            
        }
        else
        {
            tgVina.gameObject.SetActive(true);
        }
    }

    public void OnPayCard()
    {
        var cardType = ""; 
        if (tgViettel.isOn) cardType = VIETTEL;
        else if (tgVina.isOn) cardType = VINA;

        var seri = ipfSeri.text;
        var pin = ipfMaThe.text;
        
        var warn = "";
        if (seri == "") warn = "Vui lòng nhập sê ri";
        else if (seri.Length < 10) warn = "Số seri không hợp lệ";
        if (pin == "") warn = "Vui lòng nhập mã thẻ";
        else if (pin.Length < 10) warn = "Mã thẻ không hợp lệ";
        if(cardType == "") warn += " - Vui lòng chọn nhà mạng";
        ShowWarning(warn);

        if (string.IsNullOrEmpty(warn))
        {
            var data = new PayCardNewData(cardType, seri, pin, id, int.Parse(UserModel.Instance.uid), PayCardMediator.selectedPromo != null?PayCardMediator.selectedPromo.id:0).ToJson();
            API.PayCardNew(OnTransactionSuccess, OnTransactionFail, data);
            //LoadingEffect.Open();
            ShowWarning("Giao dịch đang được xử lý");
            btnNap.interactable = false;
        }
    }

    private void OnTransactionSuccess(JObject data)
    {
        CallApiDone();
        if (data["status"].ToString().ToLower() == "ok") {
            ipfSeri.text = "";
            ipfMaThe.text = "";
            var msg = SDMsg.Join(SDMsg.TransactionSucess, StringUtils.FormatMoney(long.Parse(data["balance"].ToString())));
            if(data["msg"] != null && !string.IsNullOrEmpty(data["msg"].ToString()))
                msg = data["msg"].ToString();
            DOVirtual.DelayedCall(.5f, () => BasicPopup.Open("Thông báo", msg, "Đồng ý"));
            Signals.Get<PaySuccessSignal>().Dispatch();
            Close();
        } else {
            DOVirtual.DelayedCall(.5f, () => BasicPopup.Open("Thông báo", (string) data["reason"], "Đồng ý"));
            Signals.Get<PayResponsedSignal>().Dispatch();
        }
    }
    
    private void OnTransactionFail(string s)
    {
        CallApiDone();
        DOVirtual.DelayedCall(.5f, () => BasicPopup.Open("Thông báo", "Giao dịch thất bại", "Đồng ý"));
    }
    
    private void CallApiDone() {
        //LoadingEffect.CloseLast();
        ShowWarning();
        DOVirtual.DelayedCall(2f, () => {
            btnNap.interactable = true;
        });
    }

    private void ShowWarning(string warn = "")
    {
        txtWarning.text = warn;
        txtWarning.gameObject.SetActive(!string.IsNullOrEmpty(warn));
    }

    public void ClosePopup()
    {
        ipfSeri.text = "";
        ipfMaThe.text = "";
        ShowWarning();
        Close();
    }
}