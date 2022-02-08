using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Purchasing.Security;
using Object = System.Object;


public class MyIAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController; // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private IAppleExtensions m_AppleExtensions;
    private IGooglePlayStoreExtensions m_GoogleExtensions;

    // ProductIDs
    [SerializeField] private List<ItemWallet> itemWallets;

    [SerializeField] private SelectBox cbbPromo;
    public static Promotion selectedPromo = null;

    // [SerializeField] private List<CustomIAPButton> customIAPButtons;
    private List<string> androidProductIds = new List<string>() { "hhhh9", "hhhh45", "hhhh89", "hhhh179", "hhhh449", "hhhh899" };
    private List<string> iosProductIds = new List<string>() { "a9", "a45", "a89", "a179", "a449", "a899" };
    private List<string> productIds = new List<string>() { "", "", "", "", "", "" };
    private List<int> prices = new List<int>() { };
    private List<long> amounts = new List<long>() { };
    private List<List<Promotion>> promotions = new List<List<Promotion>>();

    private static MyIAPManager instance;

    public static MyIAPManager Instance
    {
        get
        {
            if (instance == null) instance = new MyIAPManager();
            return instance;
        }
    }
    private void Start()
    {
        if(cbbPromo!=null)
            cbbPromo.OnSelect.AddListener(OnPromoSelect);
    }

    public void SetupView(DataPayReceived data)
    {
        SetUpProducts();
        prices = data.arrVNDCard;
        amounts = data.coinReceived;
        promotions = data.promotions;

        if ( ((GameUtils.IsAndroid() || GameUtils.IsEditor()) && cbbPromo != null) && data.promotions.Count > 0)
        {
            var sortedList = new List<Promotion>(data.promotions[0]);
            sortedList.Sort((x1, x2) => (int)(x2.coin - x1.coin));
            sortedList.Sort((x1, x2) => (int)(x1.expired - x2.expired));
            var promoOptionList = sortedList.Select(x => new SelectOptionData()
            {
                label = x.pay_percent != -1 ?
                $"<color=#C33716>+{x.pay_percent}</color> tỷ lệ (Hết hạn sau { (DateTimeOffset.FromUnixTimeSeconds(x.expired) - DateTimeOffset.Now).Days} ngày)" :
                $"<color=#C33716>x2</color> tỷ lệ (Hết hạn sau { (DateTimeOffset.FromUnixTimeSeconds(x.expired) - DateTimeOffset.Now).Days} ngày)",
                value = x

            }).ToList();
            var defaultOption = new SelectOptionData()
            {
                label = "Không sử dụng",
                value = null
            };
            // promoOptionList.Insert(0, defaultOption);
            promoOptionList.Add(defaultOption);
            cbbPromo.SetData(promoOptionList, false);
            this.WaitTimeout(() => { cbbPromo.ActiveNotify(0); }, 0.1f);

        }
        ReCalculate();
        if(GameUtils.IsAndroid() || GameUtils.IsEditor())
        {
            if (cbbPromo != null)
                cbbPromo.transform.parent.Show();
        }

    }
    void ReCalculate()
    {
        
        Debug.Log("prices count: " + prices.Count);
        Debug.Log("amounts count: " + amounts.Count);
        if (prices.Count < itemWallets.Count || amounts.Count < itemWallets.Count) return;

        for (var i = 0; i < itemWallets.Count; i++)
        {
            Promotion currentPromo = null;
            if (promotions.Count > 0 && selectedPromo != null)
            {
                currentPromo = promotions[i].Find(x => x.id == selectedPromo.id);
            }
            long amount = currentPromo == null ? amounts[i] : amounts[i] + currentPromo.coin;
            itemWallets[i].SetupView(productIds[i], prices[i], amount);
        }
    }
    void OnPromoSelect(object value)
    {
        cbbPromo.Collapse();
        selectedPromo = (Promotion)value;
        ReCalculate();
    }


    void OnEnable()
    {
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing, can use button click instead
            InitializePurchasing();
        }
        // If we haven't set up the Unity Purchasing reference
        // if (m_StoreController == null)
        // {
        //     // Begin to configure our connection to Purchasing, can use button click instead
        //     InitializePurchasing();
        // }
    }

    public void ReceiveIAP(JObject data)
    {
        //LoadingEffect.CloseLast();
        var listBao = GameUtils.IsAndroid()
            ? data["8"]["b"].ToList().Select(x => (long)x).ToList()
            : data["7"]["b"].ToList().Select(x => (long)x).ToList();
        var listMenhGia = GameUtils.IsAndroid()
            ? data["8"]["vnd"].ToList().Select(x => (int)x).ToList()
            : data["7"]["vnd"].ToList().Select(x => (int)x).ToList();

        var dataPayWallet = new DataPayReceived
        {
            arrVNDCard = listMenhGia,
            coinReceived = listBao
        };
        SetupView(dataPayWallet);
    }

    // public void MyInitialize()
    // {
    //     InitializePurchasing();
    // }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }
        SetUpProducts();
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var id in productIds)
        {
            builder.AddProduct(id, ProductType.Consumable);
        }

        Debug.Log("Starting Initialized...");
        UnityPurchasing.Initialize(this, builder);
    }

    private void SetUpProducts()
    {

#if UNITY_ANDROID
        productIds = androidProductIds;
#endif

#if UNITY_IOS
        productIds = iosProductIds;
#endif
    }

    private bool IsInitialized()
    {
        Debug.Log("m_StoreController is null: ? " + (m_StoreController != null));
        Debug.Log("m_StoreExtensionProvider is null: ? " + (m_StoreExtensionProvider != null));
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void RestorePurchases()
    {
        m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
        {
            Debug.Log(result ? "Restore purchases succeeded." : "Restore purchases failed.");
        });
    }

    public void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product:" + product.definition.id.ToString()));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log(
                    "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    // public void ListProducts()
    // {
    //
    //     foreach (UnityEngine.Purchasing.Product item in m_StoreController.products.all)
    //     {
    //         if (item.receipt != null)
    //         {
    //             Debug.Log("Receipt found for Product = " + item.definition.id.ToString());
    //         }
    //     }
    // }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_GoogleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

        // m_GoogleExtensions?.SetDeferredPurchaseListener(OnPurchaseDeferred);

        // Dictionary<string, string> dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

        // foreach (Product item in controller.products.all)
        // {
        //
        //     if (item.receipt != null)
        //     {
        //         string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];
        //
        //         if (item.definition.type == ProductType.Subscription)
        //         {
        //             SubscriptionManager p = new SubscriptionManager(item, intro_json);
        //             SubscriptionInfo info = p.getSubscriptionInfo();
        //             Debug.Log("SubInfo: " + info.getProductId().ToString());
        //             Debug.Log("isSubscribed: " + info.isSubscribed().ToString());
        //             Debug.Log("isFreeTrial: " + info.isFreeTrial().ToString());
        //         }
        //     }
        // }
    }

    // public void OnPurchaseDeferred(Product product)
    // {
    //
    //     Debug.Log("Deferred product " + product.definition.id.ToString());
    // }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        bool validPurchase = true;
        try
        {
            var result = validator.Validate(args.purchasedProduct.receipt);
            Debug.Log("Validate = " + result.ToString());

            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Invalid receipt, error is " + e.Message.ToString());
            validPurchase = false;
        }

        Debug.Log(string.Format("ProcessPurchase: " + args.purchasedProduct.definition.id));

        if (validPurchase)
        {
            Debug.Log("Valid receipt");
            // onPurchaseComplete.Invoke(args.purchasedProduct);
            foreach (IPurchaseReceipt productReceipt in validator.Validate(args.purchasedProduct.receipt))
            {
                // Unlock the appropriate content here.
#if UNITY_ANDROID
                GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                if (google != null)
                {
                    Debug.Log("full receipt json info");
                    Debug.Log(args.purchasedProduct.receipt);
                    //Get and parse the data you need to upload. Parse into string type
                    var wrapper =
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(args.purchasedProduct.receipt);
                    Debug.Log("convert wrapper successfully");
                    if (null == wrapper)
                    {
                        // return (consumePurchase) ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
                        return PurchaseProcessingResult.Pending;
                    }

                    // Corresponds to http://docs.unity3d.com/Manual/UnityIAPPurchaseReceipts.html
                    var store = (string) wrapper["Store"];
                    //The payload below is the IOS verification product information data. That is, the part we need to upload.
                    var payload =
                        (string) wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
                    //For GooglePlay payload contains more JSON 
                    Debug.Log("payload");
                    var gpDetails = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
                    var gpJson = (string) gpDetails["json"];
                    var gpSig = (string) gpDetails["signature"];

                    var googleWalletInfo = new GoogleWalletInfo();
                    googleWalletInfo.uid = int.Parse(UserModel.Instance.uid);
                    googleWalletInfo.productId = args.purchasedProduct.definition.id;
                    googleWalletInfo.signature = gpSig;
                    googleWalletInfo.orderId = google.orderID;
                    googleWalletInfo.packageName = google.packageName;
                    googleWalletInfo.purchaseTime = DateTimeHelper.GetTimeStamp(google.purchaseDate);
                    googleWalletInfo.purchaseState = GetPurchaseStateInt(google.purchaseState);
                    googleWalletInfo.purchaseToken = google.purchaseToken;
                    googleWalletInfo.agentData = AgentData.Create();
                    googleWalletInfo.payPercentId = selectedPromo==null?0:selectedPromo.id;

                    // temp fix
                    googleWalletInfo.agentData.app = 1;
                    // 

                    if (googleWalletInfo.purchaseState == 0)
                        Api.payByWallet(OnPaySuccess, OnPayFailure, googleWalletInfo.ToJson());
                    else OnPayFailure("");
                    Debug.Log("Google Wallet Info");
                    Debug.Log(googleWalletInfo.ToJson());
                    //Google verifies that the product information data is contained in gpJson and needs to be parsed on the server side. The corresponding key is "purchaseToken".
                    // StartCoroutine(PostRepict("http://www.xxxxxxxxxxxxx/purchase/Andverifytrade",
                    //     e.purchasedProduct.definition.id, gpJson));

                    // A consumable product has been purchased by this user.

                    // Return a flag indicating whether this product has completely been received, or if the application needs 
                    // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
                    // saving purchased products to the cloud, and when that save is delayed. 
                }
#endif

#if UNITY_IOS
                // AppleReceipt apple = productReceipt as AppleReceipt;
                AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                if (apple != null)
                {
                    Debug.Log("full receipt json info");
                    Debug.Log(args.purchasedProduct.receipt);
                    string transactionReceipt = m_AppleExtensions.GetTransactionReceiptForProduct (args.purchasedProduct);
                    var appleWalletInfo = new AppleWalletInfo(int.Parse(UserModel.Instance.uid), transactionReceipt);
                    Api.payByAppleWallet(OnPaySuccess, OnPayFailure, appleWalletInfo.ToJson());
                    Debug.Log("transaction receipt: " + transactionReceipt);
                }
#endif
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureReason));
    }

    private void OnPaySuccess(JObject obj)
    {
        //LoadingEffect.CloseLast();
        Debug.Log("OnPaySuccess: ");
        Debug.Log(obj.ToString());
        var status = (string)obj["status"];
        if (string.CompareOrdinal(status.ToLower(), "ok") == 0)
        {
            UserModel.Instance.firstCharge = false;
            PaymentData.ForceGetPayData(null, null, () =>
            {
                SetupView(PaymentData.PayWalletData);
            });
            BasicPopup.Open("Thông Báo",
                "Giao dịch thành công\nSố Bảo nhận được là " +
                StringUtils.FormatMoney(double.Parse(obj["amount"].ToString())) + " Bảo.");
            Signals.Get<RefreshCoinSignal>().Dispatch();
        }
        else OnPayFailure("");
    }

    private void OnPayFailure(string error)
    {
        Debug.Log("OnPayFailure: " + error);
        //LoadingEffect.CloseLast();
        BasicPopup.Open("Thông Báo", "Không kết nối được với hệ thống, vui lòng thử lại!", "Xác nhận");
    }

    private int GetPurchaseStateInt(GooglePurchaseState state)
    {
        switch (state)
        {
            case GooglePurchaseState.Purchased: return 0;
            default: return 1;
        }
    }
}