using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PayModMediator : MonoBehaviour
{
    [SerializeField] private TMP_Text txtVND, txtReceivedBao, txtFirstTimeNote, txtTongKM;
    [SerializeField] private TMP_Text txtKmVip, txtKmMenhGia, txtKmEvent;
    [SerializeField] private Button btnCallMod;

    private List<int> arrVNDMod = new List<int> { };
    private List<int> vip = new List<int> { };
    private List<int> menhGia = new List<int> { };
    private List<long> coinReceived = new List<long> { };

    private int idx = -1;
    private int minMod = 6;
    private int kmEvent = 0;

    public void InitView(DataPayReceived data)
    {
        btnCallMod.interactable = !GameUtils.IsWeb();
            
        arrVNDMod = data.arrVNDCard;
        vip = data.vip;
        menhGia = data.menhGia;
        coinReceived = data.coinReceived;
        minMod = data.minMod;
        kmEvent = data.kmEvent;

        idx = minMod;
        CalCoin(arrVNDMod[idx]);
    }

    public void OnCallToMod()
    {
        Application.OpenURL("tel://+84988153993");
    }

    public void OnTangClick()
    {
        OnVndChange(true);
    }

    public void OnGiamClick()
    {
        OnVndChange(false);
    }

    private void OnVndChange(bool tang)
    {
        var arr = arrVNDMod;
        if (tang && idx + 1 < arr.Count)
        {
            idx++;
        }
        else if (idx - 1 >= minMod && !tang)
        {
            idx--;
        }

        CalCoin(arr[idx]);
    }

    private void CalCoin(int value)
    {
        var _vip = idx > vip.Count - 1 ? 0 : vip[idx];
        var _menhGia = idx > menhGia.Count - 1 ? 0 : menhGia[idx];

        txtVND.text = StringUtils.FormatMoney(value) + " VNĐ";
        txtKmVip.text = _vip.ToString();
        txtKmMenhGia.text = _menhGia.ToString();
        txtKmEvent.text = kmEvent.ToString();

        txtReceivedBao.text =
            idx > coinReceived.Count - 1 ? "NaN" : StringUtils.FormatMoney(coinReceived[idx]) + " Bảo";


        int totalKM = kmEvent + _vip + _menhGia;

        txtTongKM.text = $"Tổng tỷ lệ quy đổi: <color=#FDE253>{totalKM}</color>";
        

        SDLogger.Log(idx + "   " + coinReceived.Count + "   " + UserModel.Instance.firstCharge);
        if (idx > coinReceived.Count - 1 || !UserModel.Instance.firstCharge) return;
        var bonus = coinReceived[idx] - (long) arrVNDMod[idx] * (long) totalKM;
        SDLogger.Log("Bonus: "+ bonus);
        // if (bonus > 0 && ldrModel.isPayNew) SetBonus(bonus);
        SetBonus(bonus);
    }

    private void SetBonus(long bonus)
    {
        txtFirstTimeNote.gameObject.SetActive(bonus > 0);
        txtFirstTimeNote.text = "(Đã bao gồm <b>" + StringUtils.FormatMoney(bonus) + " Bảo</b> ưu đãi Nạp lần đầu)";
    }
}