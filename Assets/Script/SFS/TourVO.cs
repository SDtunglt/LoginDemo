using Sfs2X.Entities.Data;

public class TourVO : ISFSObjVO
{
    /** zone id */
    public int zone = GameConfig.NormalZoneCount;

    public bool joinable;

    /** Có đang thi dở vòng r không để resume */
    public bool playing;

    /** int: thời gian mà tour đã hoặc sẽ mở (second) */
    public int openTime;

    /** int: thời gian mà tour sẽ đóng (giây) */
    public int closeTime;

    /** bao nhiêu lâu thì tour cho vào thi 1 lần. */
    public int period = 1;

    /** trong 1 period thì mở cửa bao lâu (bao lâu thì xếp bàn 1 lần) */
    public int duration = 1;

    public int waitTime;

    /** giò xếp bàn đầu tiên, nếu firstArrangeDelay - duration <= 0 => mở tour luôn.
	 * nếu không delay lần đầu tiên là firstArrangeDelay - duration */
    public int firstArrangeDelay;

    /** round hiện tại của người chơi */
    public int lastRound;

    public int tourFee;

    public int maxRound;

    /**
	 * Khi tour init được giá trị là false
	 * Khi kết thúc tour, sẽ được gán lại là true
	 */
    public bool isPlayed;

    /** -1 chưa xác định. 2: Đình vòng loại, 3: Đình tứ kết, 4: Đình bán kết, 5: Đình chung kết. */
    public int tourLevel = -1;

    public ISFSObject toSFSObject()
    {
        return null;
    }

    public void fromSFSObject(ISFSObject o)
    {
        if (o == null) return;
        zone = o.GetByte("t") + GameConfig.NormalZoneCount;
        joinable = o.GetBool("j");
        openTime = o.GetInt("o");
        closeTime = o.GetInt("c");
        period = o.GetInt("p");
        duration = o.GetInt("d");
        lastRound = o.GetByte("r");
        tourFee = o.GetInt("f");
        firstArrangeDelay = o.GetInt("e");
        playing = o.GetBool("g");
        maxRound = o.GetByte("m");
        waitTime = period - duration;
        if (o.ContainsKey("h"))
        {
            tourLevel = o.GetByte("h");
        }

        isPlayed = false;
    }

    public string ThiDinhName
    {
        get
        {
            switch (tourLevel)
            {
                case 2:
                    return "vòng loại";
                case 3:
                    return "vòng tứ kết";
                case 4:
                    return "vòng bán kết";
                case 5:
                    return "trận chung kết";
                case 6:
                    return "vòng loại 1";
                case 7:
                    return "vòng loại 2";
                case 8:
                    return "vòng loại 3";
            }

            return "";
        }
    }
}