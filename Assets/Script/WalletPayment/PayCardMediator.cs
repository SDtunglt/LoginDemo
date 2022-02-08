using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PayCardMediator : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private ItemPayCard itemPayCard;
    [SerializeField] private Image progress;
    [SerializeField] private GameObject nextVip, percent;
    [SerializeField] private TMP_Text txtCurrentVip, txtNextVip, txtProgress;
    [SerializeField] private SelectBox cbbPromo;

    private List<int> arrVNDCard = new List<int>();
    private List<int> vip = new List<int>();
    private List<long> amountBao = new List<long>();
     List<int> menhGia;
    private List<List<Promotion>> promotions ;

    private UserModel userModel = UserModel.Instance;

    private const int PAY_CARD = 4;
    private const string NHA_MANG = "viettel";

    public static Promotion selectedPromo;
    
    private void OnLogout()
    {
    }

    private void Start()
    {
        cbbPromo.OnSelect.AddListener(OnPromoSelect);
    }

    private void OnEnable()
    {
        Signals.Get<PaySuccessSignal>().AddListener(OnPaySuccess);
        Signals.Get<PaySuccessSignal>().AddListener(OnPayResponse);
        Signals.Get<LogoutSignal>().AddListener(OnLogout);
        ShowView();
        content.anchoredPosition = Vector2.zero;

    }

    private void OnDisable()
    {
        Signals.Get<PaySuccessSignal>().RemoveListener(OnPaySuccess);
        Signals.Get<PaySuccessSignal>().RemoveListener(OnPayResponse);
        Signals.Get<LogoutSignal>().AddListener(OnLogout);
    }

    private void ShowView()
    {
        InitView(PaymentData.PayCardData);
    }

    private void InitView(DataPayReceived data)
    {
        arrVNDCard = data.arrVNDCard;
        vip = data.vip;
        amountBao = data.coinReceived;
        promotions = data.promotions;
        menhGia = data.menhGia;


        ReCalculate();
        selectedPromo = null;
        if (data.promotions.Count > 0)
        {
            var sortedList = new List<Promotion>(data.promotions[0]);
            sortedList.Sort((x1, x2) => (int)(x2.coin - x1.coin));
            sortedList.Sort((x1, x2) => (int)(x1.expired - x2.expired));
            // vì data trả theo kiểu mảng nosql nên phải lấy phần tử đầu tiên để get data
            var promoOptionList = sortedList.Select(x => new SelectOptionData()
            {
                label = x.pay_percent != -1 ?
                $"<color=#C33716>+{x.pay_percent}</color> tỷ lệ (Hết hạn sau { (DateTimeOffset.FromUnixTimeSeconds(x.expired) - DateTimeOffset.Now ).Days} ngày)":
                $"<color=#C33716>x2</color> tỷ lệ (Hết hạn sau { (DateTimeOffset.FromUnixTimeSeconds(x.expired) - DateTimeOffset.Now).Days} ngày)",
                value = x // chặt chẽ 1 chút, dùng id vẫn tốt hơn là index. Sau này còn sort nữa.

            }).ToList();
            
            var defaultOption = new SelectOptionData()
            {
                label = "Không sử dụng",
                value = null
            };
            promoOptionList.Add(defaultOption);
            cbbPromo.SetData(promoOptionList,false);
            this.WaitTimeout(() => { cbbPromo.ActiveNotify(0); }, 0.1f);
            // promoOptionList.Insert(0, defaultOption);


        }

        UpdateProgress();
    }

    void ReCalculate()
    {
        for (var i = content.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(content.GetChild(i).gameObject);
        }

        for (var i = 0; i < arrVNDCard.Count; i++)
        {
            var _menhGia = i > menhGia.Count - 1 ? 0 : menhGia[i];
            Promotion currentPromo = null;

            if (promotions.Count > 0 && selectedPromo!= null)
            {
                currentPromo = promotions[i].Find(x => x.id == selectedPromo.id);
            }

            var totalPercent = vip[i] + _menhGia + int.Parse(UserModel.Instance.kmValue);

            long _promoCoin = 0;

            if (currentPromo != null)
            {
                _promoCoin = currentPromo.coin;
                if(currentPromo.pay_percent == -1)
                {
                    totalPercent *= 2;
                }
                else
                {
                    totalPercent += currentPromo.pay_percent;
                }
            }

            var item = Instantiate(itemPayCard, content);
            var info = UserModel.Instance.firstCharge
                ? $"Nạp lần đầu + <color=#F3B400><b>{totalPercent}</b></color> tỷ lệ quy đổi"
                : $"+ <color=#F3B400><b>{totalPercent}</b></color> tỷ lệ quy đổi";
            item.SetValue(i, amountBao[i] + _promoCoin, arrVNDCard[i], info);
        }
        
        
        UpdateProgress();
    }

    void OnPromoSelect(object value)
    {
        cbbPromo.Collapse();
        selectedPromo = (Promotion) value;
        ReCalculate();
    }

    private void OnPaySuccess()
    {
        UserModel.Instance.firstCharge = false;
        PaymentData.ForceGetPayData(null, ShowView);
        API.GetUserDetail(data =>
            {
                var vipScore = int.Parse(data["v"].ToString());
                userModel.gVO.vipScore = vipScore;
                UpdateProgress();
            },
            s => { },
            userModel.uid, GameConfig.APP_ID.ToString());
    }

    /*
     * Trạng thái mà response chưa chắc đã success, nhưng mà cần reload
     */
    private void OnPayResponse()
    {
        PaymentData.ForceGetPayData(null, ShowView);
    }

    private void UpdateProgress()
    {
        var vipScore = userModel.gVO.vipScore;
        var idx = VipType.LIST_VIP.IndexOf(VipType.FromScore(vipScore));

        if (idx < VipType.LIST_VIP.Count - 1)
        {
            var cur = VipType.LIST_VIP[idx];
            var next = VipType.LIST_VIP[idx + 1];

            txtCurrentVip.text = $"Vip {idx}";
            txtNextVip.text = $"Vip {idx + 1}";

            txtProgress.text = $"{StringUtils.FormatMoney(vipScore)}/{StringUtils.FormatMoney(next.score)}";
            progress.fillAmount = (float) cur.score / next.score;
            
            nextVip.SetActive(true);
            // percent.SetActive(vipScore != cur.score);
        }
        else
        {
            txtProgress.text =
                $"{StringUtils.FormatMoney(vipScore)}/{StringUtils.FormatMoney(VipType.LIST_VIP[idx].score)}";
            txtCurrentVip.text = $"Vip {idx}";

            nextVip.SetActive(false);
            progress.fillAmount = 1;
        }
    }
}