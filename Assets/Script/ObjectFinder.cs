using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFinder : MonoBehaviour
{
    public List<TagObject> objects;

    private static ObjectFinder _finder;

    private static ObjectFinder Ins
    {
        get
        {
            if (!_finder)
            {
                _finder = FindObjectOfType<ObjectFinder>();
            }

            return _finder;
        }
    }

    public static Transform GetObject(string tag)
    {
        return Ins.objects.Find(s => s.tag == tag).rect;
    }
}

[System.Serializable]
public class TagObject
{
    public string tag;
    public Transform rect;
}

public class TagId
{
    public const string ScreenHolder = "ScreenHolder";
    public const string PopupHolder = "PopupHolder";
    public const string LoadingEffect = "LoadingEffect";
    public const string PopupTypeScreen = "PopupTypeScreen";
    public const string ViewPriority = "ViewPriority";
    public const string MenuGroup = "MenuGroup";
    public const string GlobalDataManager = "GlobalDataManager";
    public const string TutorialController = "TutorialController";
    public const string StartUpController = "StartUpController";
}

public class PopupId
{
    public const string LoadingEffect = "LoadingEffect";
    public const string BasicPopup = "BasicPopup";
    public const string BasicTogglePopup = "BasicTogglePopup";
    public const string ChanCaPopup = "ChanCaPopup";
    public const string ChangeAvatarPopup = "ChangeAvatarPopup";
    public const string EditAvatarPopup = "EditAvatarPopup";
    public const string ExchangeMoney = "ExchangeMoney";
    public const string FindBoardPopup = "FindBoardPopup";
    public const string GameConfigPopup = "GameConfigPopup";
    public const string GamesResultPanel = "GamesResultPanel";
    public const string GiftcodePopup = "GiftcodePopup";
    public const string HocChoiPopup = "HocChoiPopup";
    public const string HocQuanPopup = "HocQuanPopup";
    public const string InvitePopup = "InvitePopup";
    public const string InvitePlayerPopup = "InvitePlayerPopup";
    public const string RateUsPopup = "RateUsPopup";
    public const string ReportPopup = "ReportPopup";
    public const string ShopPopup = "ShopPopup";
    public const string WalletPopup = "WalletPopup";
    public const string PayCardPopup = "PayCardPopup";
    public const string ThiDinhWaitingUserPanel = "ThiDinhWaitingUserPanel";
    public const string TongKetPanel = "TongKetPanel";
    public const string TransferByIdPopup = "TransferByIdPopup";
    public const string TransferCoinPopup = "TransferCoinPopup";
    public const string UserDetailPopup = "UserDetailPopup";
    public const string WaitingTourPanel = "WaitingTourPanel";
    public const string HeThongHocChoiPopup = "HeThongHocChoiPopup";
    public const string TourFinalsPopup = "TourFinalsPopup";
    public const string ChangePassPopup = "ChangePassPopup";
    public const string DailyQuestPopup = "DailyQuestPopup";
    public const string TaskRewardPopup = "TaskRewardPopup";
    public const string RankPopup = "RankPopup";
    public const string DsThiDinhPopup = "DsThiDinhPopup";
    public const string NewBasicPopup = "NewBasicPopup";
    public const string SharePopup = "SharePopup";
    public const string NoticePopup = "NoticePopup";
    public const string KickPopup = "KickPopup";
    public const string BoardKickPopup = "BoardKickPopup";
    public const string ConfirmRemoveFriendPopup = "ConfirmRemoveFriendPopup";
    public const string AchievementPopup = "AchievementPopup";
    public const string KhungAvatarPopup = "KhungAvatarPopup";
    public const string UserCertificatePopup = "UserCertificatePopup";
    public const string HuongDanPopup = "HuongDanPopup";
    public const string ChallengePopup = "ChallengePopup";
    public const string ChallengeInfoPopup = "ChallengeInfoPopup";
    public const string ChallengeInvitePopup = "ChallengeInvitePopup";
    public const string ChallengeSettingPopup = "ChallengeSettingPopup";
    public const string Events_Y10Event1Popup = "Events/Y10Event1Popup";
}

public class ScreenId
{
    
}