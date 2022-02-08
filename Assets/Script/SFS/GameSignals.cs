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
public class PlayerJoinedSignal : ASignal{}
public class ScreenChangedSignal : ASignal<NormalJoinVO>{}
public class KickPlayerOutBoardSignal : ASignal{}
public class StopCountDownSignal : ASignal{}
public class GaScoreChangeSignal : ASignal<int, bool>{}
public class SyncBoardConfigSignal : ASignal{}
public class GameNoChangedSignal : ASignal{}
public class ShowTimeOutMsgSignal : ASignal<string>{}
public class HideTimeCounterSignal : ASignal{}
public class ShowEffectPlayerU : ASignal<int>{}

public class StopGameSignal : ASignal{}
public class StopPlaySignal : ASignal{}

public class BoardStateChangedSignal : ASignal<bool>{} //new playing state
public class ChonNocSignal : ASignal{}
public class ShowXuongPanelSignal : ASignal{}
public class ShowSumupSignal : ASignal<string, string>{}
public class CheckSumUpSignal : ASignal{}
// public class UserClickJoinBoardSignal : ASignal{}
public class CanUCardAddedSignal : ASignal{}
public class SumupReceivedSignal : ASignal{}
public class ReceivedTimeCountDownSignal : ASignal<int>{}
public class ShowTextCountDown : ASignal<int, float, Action>{}
public class ClockTimeOutSignal : ASignal<int>{} //countingCode
public class GiveCardsCompletedSignal : ASignal{}
public class BocCaiSignal : ASignal{}
public class ResumeCompletedSignal : ASignal{}
public class ReceiveChatMsgSignal : ASignal<string, bool, bool, bool>{}
public class ItemReqPlaySignal : ASignal<int>{}
public class MqttSubscribeSignal : ASignal{}
public class ChangeStatusVGSignal : ASignal{}
public class CancelJoinTourSignal : ASignal{}
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
public class TourVOChangedSignal : ASignal<TourVO>{}
public class MissionFeaturesUpdatedSignal : ASignal<bool>{}
public class MultiActiveToursVOChangedSignal : ASignal<List<TourVO>>{}
public class CongthanUpdatedSignal : ASignal<bool>{}
public class UpdateQuestDataSignal : ASignal{}

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
public class OnUserSelectImageFromStorage : ASignal<Texture2D>{}
public class UpdateMenuMediatorSignal : ASignal<MenuType>{}
public class OnLeaveBoardGame : ASignal{}
public class OnPlayerLeaveBoard : ASignal{}
public class OnFriendStatusChangeSignal : ASignal<int>{}
public class OnChangeCharacterInLobby : ASignal<bool>{}
public class OnChangeKhungAvatar : ASignal<int>{}
public class OnReceiveMessageFromJsLib : ASignal<string>{}
public class OnChangeClockTime : ASignal<float>{}
public class OnChangeCardClockTime : ASignal<int, float>{}
public class OnGetInitDataComplete : ASignal{}
public class OnChangeChallengeInvited : ASignal{}
public class OnChangeChallengeInviteCount : ASignal<int>{}
public class NoticeHaveKm : ASignal{}
