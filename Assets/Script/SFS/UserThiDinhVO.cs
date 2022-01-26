public class UserThiDinhVO
{
    public int uid;
    public string name;
    public double coin;
    public int point;
    public int rank;
    public bool isBaoDanh;

    public UserThiDinhVO(int uid, int rank, string name, int point, double coin, bool isBaoDanh) {
        this.uid = uid;
        this.name = name;
        this.coin = coin;
        this.rank = rank;
        this.point = point;
        this.isBaoDanh = isBaoDanh;
    }
}