using System;

public class ShowTimeCounterVO
{
    /**index của thằng cần đếm giờ*/
    public int uIdx;

    /**thời gian đếm (giây)
	 * 	Note:  timeLeft sẽ tự được thay đổi:
	 * 		Nếu là đếm cho chính mình thì timeLeft--
	 * 		Else timeLeft += 2*/
    public int timeLeft;

    /**@param autoFunc - hàm đc gọi khi đếm cho chính mình & hết giờ
	 * 	Note: Nếu hết giờ khi đếm cho thằng khác thì sẽ send ExtEvent.CMD_NOTIFY_OTHER_COULD_EXITED*/
    public Action autoFunc;

    public ShowTimeCounterVO(int _uIdx, int _timeLeft, Action callback)
    {
        uIdx = _uIdx;
        timeLeft = _timeLeft;
        autoFunc = callback;
    }
}