using System;

public static class aSDMsg
{
    public const string CANTJOINZONEVIP = "Bạn phải có ít nhất <b>{0}</b> điểm Công Thần để vào khu <b>{1}</b>";
    public const string CANTJOINZONECOIN = "Bạn phải có ít nhất <b>{0} Bảo</b> để vào khu <b>{1}</b>";
    public const string CANTJOINZONE = "Bạn phải có ít nhất <b>{0} Bảo</b> và <b>{1}</b> điểm Công Thần để vào khu <b>{2}</b>";
    public const string CANTJOINZONEREVIEW = "Bạn không thể vào khu";
    public const string CANTJOINZONELEVEL = "Bạn cần nhiều hơn <b>550</b> điểm kinh nghiệm để vào khu <b>{1}</b>";
    public static string BOARDCONFIGCHANGED = "Cài đặt bàn chơi đã được thay đổi. Chọn nút Cài Đặt hoặc khu vực thông tin cược để xem chi tiết.";
    public static string ScoreHisWarning = "Bàn chơi có người mới vào.\nĐiểm sẽ được tính lại từ đầu.\nBạn có muốn bắt đầu ván mới không?";
    public static string LogoutConfirm = "Bạn có muốn đăng xuất tài khoản hiện tại không?";
    public static string NEED_RESUME = "Bạn đang chơi dở ở <b>{0}</b> và chỉ được vào bàn khác khi ván đó kết thúc. Bạn có muốn vào lại bàn đang chơi dở không?";
    public static string BANED = "Tài khoản của bạn đã bị khóa !";
    public static string FORCEDIS = "Vào bàn mà không chơi trong vài phút sẽ bị thoát";
    public static string DISLOGIN = "Bị ngắt kết nối đến máy chủ.\nVui lòng đăng nhập lại";
    public static string DISINROOM = "Kết nối đến máy chủ đã bị ngắt do bạn ngồi lâu quá mà không chơi";
    public static string KICKEDLOGIN = "Bạn bị đá do tài khoản của bạn vừa được đăng nhập từ một nơi khác!";
    public static string UserNotFound = "Không tìm được người chơi có ID là <b>{0}</b>";
    public static string NOTSUMUP = "Tổng kết ván có lỗi! Bạn vui lòng thông báo với Mod để xử lý!";
    public static string CANTQUICKJOIN = "Không tìm thấy bàn theo yêu cầu của bạn.";

    public static string TourPlaying = "Bạn đang dự {0}.\nBạn có muốn tiếp tục tham gia không?";
    public static string TourOpen = "Đã đến giờ {0}.\nTham gia ngay để trở thành\n{1} Sân Đình.";
    public static string TourPlayed = "Đã đến giờ {0}, bạn đã đỗ kỳ {1}.\nBạn có muốn thi tiếp kỳ {2} không?";
    public static string TourDinhPlayed = "Đã đến giờ Thi Đình.\nBạn có muốn thi tiếp không?";
    public static string TourFeeConfirm = "Lệ phí <b>{0}</b> là <b>{1} Bảo</b> .\nBạn có đồng ý tham gia không?\nLệ phí sẽ bị trừ khi bạn được xếp bàn.\n Số dư tối thiểu sau khi trừ lệ phí là <b>{2} Bảo</b>.";
    public static string TourFeeConfirmReview = "Bạn có đồng ý tham gia không?";
    public static string TourSpecsCount = "Có <color=#f60124>{0}</color> người đang xem";
    public static string CanDidateThiDinhNotChoice = "Bạn không được chọn vào thi.";
    public static string TourCantArrangeDinh = "Không đủ người tham gia giải đấu để xếp bàn!\nBạn vui lòng tham gia lại vào kỳ sau";
    public static string TourCantArrange = "Bạn chưa được xếp bàn vì thiếu chân.\nVui lòng quay lại sau {0} nữa.";
    public static string TourClose = "Khoa thi đã đóng";
    public static string TourCantJoin = "Bạn không đủ điều kiện để tham gia thi.";
    public static string TourLossAllLeave = "Bạn bị trượt vì tất cả người chơi thoát hết.";
    public static string THIDINH_LOBBY = "";
    public static string TOURHELPTHIDINH = "";

    public static string NOTENOUGHCOIN = "Bạn chưa đủ <b>Bảo</b>, <b>kinh nghiệm</b> hoặc <b>điểm Công thần</b> để vào bàn này. Bạn cần tối thiểu <b>20 lần</b> cược để vào bàn.";
    public static string NOTENOUGH40TRIEU = "Bạn cần tối thiểu <b>40 triệu Bảo</b> để vào bàn";
    public static string NOTENOUGHCOINDIEN = "Bạn chưa đủ <b>Bảo</b>, <b>kinh nghiệm</b> hoặc <b>điểm Công thần</b> để vào bàn này. Bạn cần tối thiểu <b>100 lần</b> cược để vào bàn.";
    public static string NOTINVUONGLIST = "Bạn chưa đủ điều kiện để vào <b>Vương Gia Bảng</b>.Bạn cần ở trong danh sách <b>Vương Gia Bảng</b> để vào bàn này";
    public static string CANTJOINBOARDREVIEW = "Bạn không thể vào bàn";

    public static string TransactionFail = "Giao dịch thất bại";
    public static string SameId = "Giao dịch thất bại\nBạn không thể tặng Bảo cho chính mình";
    public static string WrongPass = "Giao dịch thất bại\nSai mật khẩu";
    public static string NoRight = "Giao dịch thất bại\nBạn không có quyền thực hiện giao dịch này";
    public static string HasDebt = "Giao dịch thất bại\nBạn đang nợ Bảo";
    public static string NoCoin = "Giao dịch thất bại\nBạn không đủ Bảo";
    public static string TransactionSucess = "Giao dịch thành công.\n Số Bảo của bạn là: <b>{0} Bảo</b>";
    
    public static string LeaveHocChoi = "Bạn có muốn thoát khỏi Học chơi?\nNếu thoát thì bạn sẽ không nhận được phần thưởng từ Sân Đình";
    public static string ChallengeNotFound = "Không tìm thấy thách đấu phù hợp!\nHãy tham gia phòng khác.";
    public static string ChallengeFull = "Thách đấu hết chỗ!\nHãy tham gia phòng khác.";
    public static string ChallengeDisable = "Tính năng thách đấu đang tạm đóng.";
    public static string ChallengeAllLost = "Thách đấu hòa khi tất cả người chơi thoát ra.";
    public static string BoardNotExist = "Bàn không tồn tại.";

    public static string Join(string msg, params object[] list)
    {
        if (list == null) return msg;
        string[] parts = msg.Split('{');
        string result = parts[0];
        string part;
        string[] sub;
        int index;
        for (int i = 1; i < parts.Length; i++) {
            part = parts[i];
            sub = part.Split('}');
            index = Int32.Parse(sub[0]);
            if (list[index] == null) result += "{" + index + "?}";
            else result += list[index];
            result += sub[1];
        }
        return result;
    }
}