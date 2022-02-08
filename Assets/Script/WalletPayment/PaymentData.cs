using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static DataPayReceived;

public class PaymentData
{
    private const int PAY_CARD = 4;
    private const string NHA_MANG = "viettel";
    public static DataPayReceived PayCardData;
    public static DataPayReceived PayWalletData;
    public static DataPayReceived PayModData;

    public static void CheckAndGetData(Action onComplete = null, Action onFailure = null)
    {
        if (PayCardData == null)
        {
            GetPayCardData();
        }

        if (PayWalletData == null)
        {
            GetPayWalletData();
        }

        if (PayModData == null)
        {
            GetPayModData(onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public static void ForceGetPayData(Action onComplete = null, Action onCompletePayCard = null, Action onCompleteWallet = null)
    {
        GetPayCardData(onCompletePayCard);
        GetPayModData(onComplete);
        GetPayWalletData(onCompleteWallet);
    }

    public static void GetPayCardData(Action onComplete = null)
    {
        API.GetTotalReceived(data =>
            {
                //data["p"] = @"[[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":6600000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":8800000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":13500000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":20250000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":32700000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":65400000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":65700000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":131400000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":173700000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":347400000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":173700000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":347400000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":173700000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":347400000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":173700000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":347400000}],[{""id"":3,""pay_percent"":300,""expired"":1640054806,""coin"":173700000},{""id"":6,""pay_percent"":-1,""expired"":1639816047,""coin"":347400000}]]";
                PayCardData = new DataPayReceived
                {
                    arrVNDCard = data["vnd"].ToList().Select(x => (int)x).ToList(),
                    vip = data["v"].ToList().Select(x => (int)x).ToList(),
                    coinReceived = data["b"].ToList().Select(x => (long)x).ToList(),
                    menhGia = data["t"].ToList().Select(x => (int)x).ToList(),
                    promotions = data["p"] != null ? JsonConvert.DeserializeObject<List<List<Promotion>>>(data["p"].ToString()) : // dung jtoken cha hieu sao ko parse duoc
                    new List<List<Promotion>>()
                };

                DOVirtual.DelayedCall(.5f, () => Signals.Get<NoticeHaveKm>().Dispatch());
                onComplete?.Invoke();
            },
            s => { SDLogger.LogError(s.ToString()); }, PAY_CARD, NHA_MANG,
            "true");
    }

    public static void GetPayWalletData(Action onComplete = null)
    {
        API.GetEstimateWallet(data =>
            {
                //LoadingEffect.CloseLast();
                var listBao = GameUtils.IsAndroid()
                    ? data["8"]["b"].ToList().Select(x => (long)x).ToList()
                    : data["7"]["b"].ToList().Select(x => (long)x).ToList();
                var listMenhGia = GameUtils.IsAndroid()
                    ? data["8"]["vnd"].ToList().Select(x => (int)x).ToList()
                    : data["7"]["vnd"].ToList().Select(x => (int)x).ToList();
                var listPromo = new List<List<Promotion>>();
                if ((GameUtils.IsAndroid() || GameUtils.IsEditor()) && data["8"]["p"] != null)
                {
                    listPromo = JsonConvert.DeserializeObject<List<List<Promotion>>>(data["8"]["p"].ToString());

                }
                PayWalletData = new DataPayReceived
                {
                    arrVNDCard = listMenhGia,
                    coinReceived = listBao,
                    promotions = listPromo
                };
                onComplete?.Invoke();
                // WalletPopup.Open(dataPayWallet);
            },
            s => { SDLogger.LogError(s.ToString()); });
    }

    private static void GetPayModData(Action onComplete, Action onFailure = null)
    {
        API.GetCoinMod(data =>
        {
            PayModData = new DataPayReceived
            {
                arrVNDCard = data["vnd"].ToList().Select(x => (int)x).ToList(),
                vip = data["v"].ToList().Select(x => (int)x).ToList(),
                coinReceived = data["b"].ToList().Select(x => (long)x).ToList(),
                menhGia = data["t"].ToList().Select(x => (int)x).ToList(),
                minMod = int.Parse(data["mm"].ToString()),
                kmEvent = int.Parse(data["e"].ToString())
            };
            onComplete?.Invoke();
        }, s =>
        {
            SDLogger.LogError(s);
            onFailure?.Invoke();
        });
    }
}