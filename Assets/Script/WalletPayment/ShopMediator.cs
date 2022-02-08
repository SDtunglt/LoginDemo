using System;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopMediator : UIPopup
{
    [SerializeField] private PayCardMediator payCard;
    [SerializeField] private PayModMediator payMod;
    [SerializeField] private MyIAPManager iap;
    [SerializeField] private Toggle tgWallet;
    [SerializeField] private ScrollRect cardShopScroll;
    [SerializeField] private Toggle[] _toggles;
    [SerializeField] private GameObject[] _background;
    private Action onClose;

        public static void Open()
    {
        //LoadingEffect.Open();
        PaymentData.CheckAndGetData(() =>
        {
            //LoadingEffect.CloseLast(); 
            ViewCreator.OpenPopup(PopupId.ShopPopup, view =>
            {
                var shop = view.Cast<ShopMediator>();
                shop.SetDataPay(PaymentData.PayModData);
                shop.onClose = null;
                if (ScreenManager.Instance.IsOnScreen(ScreenManager.GAMEPLAY))
                {
                    MenuGroupController.ShowImmediate("LobbyScene");
                    shop.onClose = () => { MenuGroupController.ShowImmediate("GamePlayScene"); };
                }

                shop.OpenView();
            });
        }, () =>
        {
            //LoadingEffect.CloseLast();
            DOVirtual.DelayedCall(.5f,
                () => BasicPopup.Open("Thông báo", "Lấy thông tin nạp Bảo thất bại", "Đồng ý"));
        });
        
    }

    private void SetDataPay(DataPayReceived data)
    {
        payMod.InitView(data);
        tgWallet.gameObject.SetActive(!GameUtils.IsWeb());
    }
}
