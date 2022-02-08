using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class WalletPopup : UIPopup
{
    [SerializeField] private MyIAPManager myIAPManager;
    private Action onClose;

    public static void Open()
    {
        PaymentData.ForceGetPayData(null, null, () =>
        {
            //LoadingEffect.CloseLast();
            ViewCreator.OpenPopup(PopupId.WalletPopup, view =>
            {
                var shop = view.Cast<WalletPopup>();
                // shop.myIAPManager.SetupView(data);
                shop.myIAPManager.SetupView(PaymentData.PayWalletData);
                shop.onClose = null;
                if (ScreenManager.Instance.IsOnScreen(ScreenManager.GAMEPLAY))
                {
                    MenuGroupController.ShowImmediate("LobbyScene");
                    shop.onClose = () => { MenuGroupController.ShowImmediate("GamePlayScene"); };
                }
                shop.OpenView();
            });
        });
    }
}
