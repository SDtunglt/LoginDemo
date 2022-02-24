using System;
using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using UnityEngine;

public class LoginFbSuccessSignal : ASignal<string>{}
public class LoginSuccessSignal : ASignal{}
public class LogoutSignal : ASignal{}
public class UpdatePlayerInfoSignal : ASignal{}
public class UpdateBoardInfoSignal : ASignal<int>{}
public class RefreshCoinSignal : ASignal{}
public class UpdateShowCoinSignal : ASignal<bool>{}
public class RedirectServerComplete : ASignal{}
public class ScreenChangedSignal : ASignal<NormalJoinVO>{}
public class PlayerJoinedSignal : ASignal{}
public class PlayerLeavedSignal : ASignal<SDPlayer>{}
public class KickPlayerOutBoardSignal : ASignal{}
public class StopCountDownSignal : ASignal{}
public class CardDrawnSignal : ASignal<SDCard>{}
public class ScoreChangeSignal : ASignal<ScoreVO, bool>{}
public class GaScoreChangeSignal : ASignal<int, bool>{}
public class SyncBoardConfigSignal : ASignal{}
public class GameNoChangedSignal : ASignal{}
public class ShowTimeOutMsgSignal : ASignal<string>{}
public class HideTimeCounterSignal : ASignal{}
public class ShowCardUSignal : ASignal<List<SDCard>>{}
public class ShowEffectPlayerU : ASignal<int>{}
public class CardUEffectSignal : ASignal<SDCard>{}
public class StopGameSignal : ASignal{}
public class StopPlaySignal : ASignal{}
public class RemoveCardReplaySignal : ASignal<int, List<SDCard>>{}
public class RemoveCardsFromMyCardsSignal : ASignal<List<SDCard>>{}
public class RemoveCardsFromCardsInHandSignal : ASignal<int, List<SDCard>>{}
public class BoardStateChangedSignal : ASignal<bool>{} //new playing state
public class ChonNocSignal : ASignal{}
public class ShowXuongPanelSignal : ASignal{}
public class ShowSumupSignal : ASignal<string, string>{}
public class PlayReceiveSignal : ASignal<PlayVO>{}
public class CheckSumUpSignal : ASignal{}
public class XuongSignal : ASignal<SumUpVO>{}
public class SumUpSignal : ASignal<UVO>{}
// public class UserClickJoinBoardSignal : ASignal{}
public class CanUCardRemovedSignal : ASignal<SDCard>{}
public class CanUCardAddedSignal : ASignal{}
public class SumupReceivedSignal : ASignal{}
public class ReceivedTimeCountDownSignal : ASignal<int>{}
public class ShowTimeCounterSignal : ASignal<ShowTimeCounterVO>{}
public class ShowTextCountDown : ASignal<int, float, Action>{}
public class ClockTimeOutSignal : ASignal<int>{} //countingCode
public class PlayerStatusChangedSignal : ASignal<SDPlayer>{}
public class GiveCardsCompletedSignal : ASignal{}
public class BocCaiSignal : ASignal{}
public class ResumeCompletedSignal : ASignal{}
public class ReceiveChatMsgSignal : ASignal<string, bool, bool, bool>{}
public class ItemReqPlaySignal : ASignal<int>{}
public class MqttSubscribeSignal : ASignal{}
public class ChangeStatusVGSignal : ASignal{}
public class TourVOChangedSignal : ASignal<TourVO>{}
public class CancelJoinTourSignal : ASignal{}
public class MultiActiveToursVOChangedSignal : ASignal<List<TourVO>>{}
public class ShowHeadlineSignal : ASignal<string>{}
public class ShowDieuLeVGPopup : ASignal<int, int>{}
public class AvatarChangeSignal : ASignal<int>{}
public class UpdatePlayerInBoardSignal : ASignal{}
public class AutoSortCardSignal : ASignal{}
public class HideXuongSignal : ASignal{}
public class ConnectStatusSignal : ASignal<int>{}
public class LostConnectionSignal : ASignal{}
public class PaySuccessSignal : ASignal{}
public class PayResponsedSignal : ASignal{}
public class MissionCompleteSignal : ASignal<bool>{} 
public class MissionFeaturesUpdatedSignal : ASignal<bool>{}
public class CongthanUpdatedSignal : ASignal<bool>{}
public class UpdateQuestDataSignal : ASignal{}
public class UpdateMenuMediatorSignal : ASignal<MenuType>{}

public class FeatureConfigSignal : ASignal{ }

public class GamePlayExtensionResponseSignal : ASignal<string, SFSObject>{}
public class GamePlayBackgroundChangeSignal : ASignal<int>{}
public class GamePlayBackCardSignal : ASignal<int>{}
public class OnReceiveThiDinhJoinData : ASignal{}
public class OnHandlePlayResumeSignal : ASignal{}
public class KickOutTourUserAfterSumUpSignal : ASignal{}
public class DealCardFlyDoneSignal : ASignal{}
public class ChonNocDoneSignal : ASignal{}
public class DealCardDoneSignal : ASignal{}
public class OnJoinGamePlaySignal : ASignal{}
// public class OnTriggerShareGame : ASignal<SharePopupType, string>{}
public class OnSetUpCaptureShareScreenShot : ASignal<bool,SumUpVO>{}
public class OnUserSelectImageFromStorage : ASignal<Texture2D>{}
public class OnLeaveBoardGame : ASignal{}
public class OnPlayerLeaveBoard : ASignal{}
public class OnFriendStatusChangeSignal : ASignal<int>{}
// public class SwitchUISignal : ASignal<ComponentType> {}
public class OnChangeCharacterInLobby : ASignal<bool>{}
// public class OnTakeScreenShotCertificate : ASignal<AchievementData>{}
public class OnChangeKhungAvatar : ASignal<int>{}
public class OnReceiveMessageFromJsLib : ASignal<string>{}
public class OnChangeClockTime : ASignal<float>{}
public class OnChangeCardClockTime : ASignal<int, float>{}
public class OnGetInitDataComplete : ASignal{}
public class OnChangeChallengeInvited : ASignal{}
public class OnChangeChallengeInviteCount : ASignal<int>{}
public class OnReceiveDeepLink : ASignal<string>{}
public class OnUpdatedChallengeLink : ASignal<string>{}
// public class OnReceiveLiXiResponse : ASignal<LiXiResponseVo>{}
public class NoticeHaveKm : ASignal{}
