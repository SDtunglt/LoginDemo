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
            Debug.LogError("Load Shop Dataaa Complete");
            //LoadingEffect.CloseLast();
            ViewCreator.OpenPopup(PopupId.WalletPopup , view =>
            {
                 Debug.LogError("Load View Complete");
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

    public void ShowWallet(){
         PaymentData.ForceGetPayData(null, null, () =>
        {
                // shop.myIAPManager.SetupView(data);
                myIAPManager.SetupView(PaymentData.PayWalletData);
                // if (ScreenManager.Instance.IsOnScreen(ScreenManager.GAMEPLAY))
                // {
                //     MenuGroupController.ShowImmediate("LobbyScene");
                // }
                // OpenView();
                gameObject.SetActive(true);
        });
    }

    public override void Close()
    {
        base.Close();
        onClose?.Invoke();
    }
}
