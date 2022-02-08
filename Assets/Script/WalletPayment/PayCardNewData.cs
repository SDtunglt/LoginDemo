public class PayCardNewData : ADataRequest
{
    public int uid;
    public string cardType;
    public string seri;
    public string pin;
    public int amount;
    public int payPercentId;

    public PayCardNewData(string type, string _seri, string _pin, int _amount, int _uid, int _payPercentId = 0) {
        uid = _uid;
        cardType = type;
        seri = _seri;
        pin = _pin;
        amount = _amount;
        payPercentId = _payPercentId;
    }
}