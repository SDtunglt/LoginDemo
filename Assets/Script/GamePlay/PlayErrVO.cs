using Sfs2X.Util;

public class PlayErrVO
{
    public static string[] ERRS = new[]
    {
        "ăn treo tranh", //0
        "chíu được nhưng lại ăn thường", //lỗi 0, 1 nghỉ ăn tiền
        "ăn chọn cạ",
        "ăn cạ chuyển chờ",
        "có chắn cấu cạ", //lỗi 0,1,2,3,4 chỉ bị phát hiện khi thằng này ù
        "bỏ chắn ăn chắn", //5
        "bỏ chắn ăn cạ",
        "bỏ cạ ăn cạ",
        "bỏ chắn đánh chắn",
        "đánh cạ ăn cạ",
        "đánh rồi lại ăn cạ cùng hàng", //10
        "đánh rồi lại ăn đúng quân đó",
        "đánh cả chắn đi",
        "ăn rồi lại đánh đúng quân đó",
        "ăn cạ rồi lại ăn chắn cùng hàng",
        "đánh cạ khi đã ăn cạ", //15
        "ăn cạ đánh quân cùng hàng",
        "ăn 2 cạ cùng hàng",
        "ăn cạ rồi lại chíu quân cùng hàng"
    };

    public static int NOT_ERR = -1;
    public static int AN_TREO_TRANH = 0;
    public static int AN_CHIU_DUOC = 1;
    public static int AN_CHON_CA = 2;
    public static int AN_CA_CCHO = 3;
    public static int CO_CHAN_CAU_CA = 4;
    public static int BO_CHAN_AN_CHAN = 5;
    public static int BO_CHAN_AN_CA = 6;
    public static int BO_CHAN_DANH_CHAN = 8;
    public static int DANH_CA_AN_CA = 9;
    public static int DANH_AN_CA_CUNG_HANG = 10;
    public static int DANH_ROI_AN = 11;
    public static int DANH_CHAN = 12;
    public static int AN_ROI_DANH = 13;
    public static int AN_CA_AN_CHAN = 14;
    public static int AN_CA_DANH_CA = 15;
    public static int AN_CA_DANH_CUNG_HANG = 16;
    public static int AN_2_CA = 17;
    public static int AN_CA_CHIU_CUNG_HANG = 18;

    public static bool isGreater(PlayErrVO e1, PlayErrVO e2)
    {
        return e1 != null && (e2 == null || e1.isBao && !e2.isBao || e1.isUBao && !e2.isUBao);
    }

    private int err;
    private int card;
    private int eatenCard = -1;
    public bool isBao => err >= BO_CHAN_AN_CHAN;
    public bool isUBao => err >= AN_CHON_CA;

    public string msg
    {
        get
        {
            var ret = ERRS[err] + ", " + SDCard.cardName(card);
            if(eatenCard != -1)
                ret += " ăn " + SDCard.cardName(eatenCard);
            return ret;
        }
    }

    public PlayErrVO(int err, int card, int eatenCard = -1)
    {
        this.err = err;
        this.card = card;
        if(card != eatenCard)
            this.eatenCard = eatenCard;
    }

    public ByteArray toByteArray()
    {
        var ret = new ByteArray();
        ret.WriteByte((byte) err);
        ret.WriteByte((byte) card);
        if(eatenCard != -1)
            ret.WriteByte((byte) eatenCard);
        return ret;
    }

    public static PlayErrVO fromByteArray(ByteArray e)
    {
        e.Position = 0;
        var err = e.ReadByte();
        var card = e.ReadByte();
        var eatenCard = -1;
        if(e.Length > 2)
            eatenCard = e.ReadByte();
        return new PlayErrVO(err, card, eatenCard);
    }

    public static PlayErrVO fromData(int version, string data)
    {
        var arr = data.Split(',');
        return arr.Length > 2
            ? new PlayErrVO(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]))
            : new PlayErrVO(int.Parse(arr[0]), int.Parse(arr[1]));
    }

}

