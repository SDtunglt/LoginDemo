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

    private void Start()
    {
        for (var i = 0; i < _toggles.Length; i++)
        {
            var toggle = _toggles[i];
            var i1 = i;
            toggle.onValueChanged.AddListener(b => { _background[i1].SetActive(!b); });
        }
    }

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
   
    
    public void OnClosePopup()
    {
        this.ShowFlashWithCallBack(() =>
        {
            onClose?.Invoke();
            Close();
            if (MenuGroupController.Ins.currentScreen == "GamePlayScene.Shop")
            {
                MenuGroupController.Ins.OnScreenChange("GamePlayScene");
            }
        });
    }

    public override void OpenView()
    {
        base.OpenView();
        _toggles[0].isOn = true;
        OpenPayMod(true);
        cardShopScroll.horizontalNormalizedPosition = 1;
        SDLogger.Log("Open Shop");
    }

    public override void OnClose()
    {
        popupAnim.OnStop();
        popupAnim.OnReverse().OnComplete(() =>
        {
            if (GetGameObject().activeSelf)
            {
                LeanPool.Despawn(this);
                onClose?.Invoke();
            }
        });
    }

    public void OpenPayMod(bool isOpen)
    {
        if (isOpen)
        {
            payCard.Hide();
            payMod.Show();
            iap.Hide();
        }
    }

    public void OpenPayCard(bool isOpen)
    {
        if (isOpen)
        {
            if (PaymentData.PayCardData != null)
            {
                payCard.Show();
                payMod.Hide();
                iap.Hide();
            }
            else
            {
                PaymentData.GetPayCardData(() =>
                {
                    payCard.Show();
                    payMod.Hide();
                    iap.Hide();
                });
            }
           
        }
    }

    public void OpenPayWallet(bool isOpen)
    {
        if (isOpen)
        {
            if (PaymentData.PayWalletData != null)
            {
                iap.Show();
                iap.SetupView(PaymentData.PayWalletData);
                payCard.Hide();
                payMod.Hide();
            }
            else
            {
                PaymentData.GetPayWalletData(() =>
                {
                    iap.Show();
                    iap.SetupView(PaymentData.PayWalletData);
                    payCard.Hide();
                    payMod.Hide();
                });
                
            }
            
        }
    }
}