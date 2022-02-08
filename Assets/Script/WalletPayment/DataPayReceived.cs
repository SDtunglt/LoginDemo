using System.Collections.Generic;

public class DataPayReceived
{
    public List<int> arrVNDCard;
    public List<int> vip;
    public List<long> coinReceived;
    public List<int> dataCongThan;
    public List<int> menhGia;
    public List<List<Promotion>> promotions;
    public int minMod;
    public int kmEvent;

}
public class Promotion
{
    public int id;
    public int pay_percent;
    public long expired;
    public long coin;
}
