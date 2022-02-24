using System.Collections.Generic;

public class UModel: Singleton<UModel>
{
    /**@see ù topic in cpr.mmap
	 * Should use as getter only*/
    public List<UVO> uvos = new List<UVO>();

    /**uvo cần store lại tại SumUpCommand
     * để nếu thằng ù thoát khi chưa xướng thì những thằng khác biết đường mà xướng Suông thay
	 * @see net.sandinh.board.controller.SumUpCommand.sumUpWhenU()*/
    public UVO uvo;

    /**Lưu những thằng thoát khi ván đã stop
	 * @see issue #139*/
    public List<int> disPlayerWhenStopPlayIndexes = new List<int>();

    /**Việc check xem 1 player có ù đc 1 Card nào đó hay không
	 * còn phải tính xem nếu ù thì ù bao nhiêu điểm (có lớn hơn boardModel.minPointU không)
	 * Để tính số điểm thì phải tính ù những cước nào.
	 * Để biết có ù cước chỉ, hay địa ù không thì lại phải biết quân ấy ở cửa nào,
	 * 	và biết những play actions trước đó.
	 * Solution: Mỗi quân bốc/ đánh/ trả ra sẽ lưu thêm thông tin play action index tương ứng
	 * Note: check thiên ù không nằm trong trường hợp này*/
    public Dictionary<SDCard, int> cardToPlayActIdxDic = new Dictionary<SDCard, int>();

    public void ReInit()
    {
        uvos.Splice(0, uvos.Count);
        cardToPlayActIdxDic = new Dictionary<SDCard, int>(); //FIXME will garabe collect?
        uvo = null;
        disPlayerWhenStopPlayIndexes = new List<int>();
    }
}