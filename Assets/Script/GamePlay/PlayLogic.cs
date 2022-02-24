using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PlayLogic : Singleton<PlayLogic>
{
	private PlayModel playModel = PlayModel.Instance;
	private NocModel nocModel = NocModel.Instance;
	private GamePlayModel gamePlayModel = GamePlayModel.Instance;
	
    private SmartFoxConnection sfs;
    public Tween delaySendExtCmd;
    private int oldPlayIdx = -2;

    public void Init()
    {
        sfs = SmartFoxConnection.Instance;
        ResetOldActionIndex();
    }
    
    /**check nhái gà. Req: chưa nocModel.draw & chưa PlayReceivedCommand.checkChangeCurCard*/
	public bool isNhaiGa()
    {
	    if (playModel?.curCard == null) return false;
	    if (nocModel?.cards == null) return false;
	    return playModel.curCard.v == nocModel.cards[nocModel.cards.Count - 1];
	}
	public void checkChangeNhaiGa(PlayVO act)
	{
		if(act.type == PlayVO.DRAW)
			if (isNhaiGa())
			{
				playModel.nhaiGa++;
				if (!gamePlayModel.resuming)
				{
					//SoundManager.Instance.PlayNhaiSound("nhai");
				}
			}
			else
			{
				playModel.nhaiGa = 0;
			}
	}
	public void SendPlay(PlayVO act)
	{
		// clock.stopClock();
		// Thằng bị báo không bao giờ send play action
		Debug.Log($"Send PlayAct: myidx {gamePlayModel.myIdx} uidx {act.uIdx}, status {gamePlayModel.status}, isPlayer {gamePlayModel.isPlayer}, isBao {gamePlayModel.myPlayer.bao}, oldPlayIdx {oldPlayIdx}");
		if(gamePlayModel.status != BoardStatus.PLAYING || !gamePlayModel.isPlayer || gamePlayModel.myPlayer.bao)
			return;
		act.actionIndex = playModel.curActIdx;
		//FIXME 1 thằng đã báo | dis rồi thì có tính gà nhái không?
		//(không cần xét gà chíu, vì báo | dis không chíu đc
		// & nếu thằng khác chíu thì nó vẫn phải vào gà như thường)
		if(gamePlayModel.isNuoiGa)
			CheckVaoGa(act);
		if(act.uIdx == gamePlayModel.myIdx)
		{
			if (act.actionIndex == oldPlayIdx) return;
			sfs.SendExt(ExtCmd.Play, act);
			oldPlayIdx = act.actionIndex; 
			Debug.Log($"Send PlayAct Success");

		}
		else
		{	
			//then boardModel.sdplayers[playVO.uIdx].dis
			//Cần kill nếu có thằng đã send
			//@see PlayReceivedCommand
			var delay = MyOrderInCanPlayPlayers() * 3;
			//Note: delayedCall(0, func) không gọi func ngay lập tức vì delayedCall gọi sang to() với immediateRender = false
			//Muốn ngay lập tức thì: TweenLite.to(func, 0, {delay:0, onComplete:func, overwrite:"none"})
			delaySendExtCmd?.Kill();
			delaySendExtCmd = DOVirtual.DelayedCall(delay, () =>
			{
				if (act.actionIndex == oldPlayIdx) return;
				sfs.SendExt(ExtCmd.Play, act);
				oldPlayIdx = act.actionIndex;
			});
		}
	}
	/**@see net.sandinh.board.model.BoardModel.myOrderInConnectedPlayers()
	 * @see net.sandinh.board.controller.UserExitedBoardCommand*/
	private int MyOrderInCanPlayPlayers(){
		var ret = 0;
		for(var i = 0; i < gamePlayModel.myIdx; i++)
		{
			if(!gamePlayModel.sdplayers[i].dis && !gamePlayModel.sdplayers[i].bao)
				ret++;
		}
		return ret;
	}
	
	/**update act.vaoGa*/
	private void CheckVaoGa(PlayVO act)
	{
		VaoGaVO vo = null;
		if(act.type == PlayVO.CHIU){
			vo = new VaoGaVO();
			if(playModel.curCard.isInNoc)
				UpdateVaoGaVOIds(vo, act.uIdx);
			else
				vo.ids.Add(gamePlayModel.sdplayers[playModel.curAct.uIdx].uid);
			vo.score = 1;
		}else if(act.type == PlayVO.DRAW){
			//check gà nhái
			if(isNhaiGa()){
				vo = new VaoGaVO();
				UpdateVaoGaVOIds(vo, act.uIdx);
				vo.score = playModel.nhaiGa + 1;
			}
		}
		if(vo != null){
			if(playModel.curCard.isChiChi)
				vo.score *= 2;
			act.vaoGa = vo;
		}
	}
	private void UpdateVaoGaVOIds(VaoGaVO vo, int actUIdx)
	{
		for(var i = 0; i < gamePlayModel.sitCount; i++){
			if(i == actUIdx)
				continue;
			vo.ids.Add(gamePlayModel.sdplayers[i].uid);
		}
	}
	
	/**Nếu startClock lần đầu cho player thì tính thêm thời gian xếp bài
	 * Time xếp bài bắt đầu đếm tại GiveCardsCompletedCommand
   * @param p
	 * @param timerOffset - trường hợp đếm giờ tiếp cho 1 player khác sau khi COUNT_FOR_MYPLAYER_CHIU
	 * 		thì sẽ đếm thực là (timeLeft - chiuTime) với timeLeft phải được tính tại thời điểm bắt đầu
	 * 		COUNT_FOR_MYPLAYER_CHIU. Nhưng do gọi hàm này tại ClockTimeOutCommand SAU khi đã
	 * 		COUNT_FOR_MYPLAYER_CHIU nên cần có param timerOffset*/
	public int GetClockTimeLeft(SDPlayer p, int timerOffset = 0)
	{
		if(p.isPlayed || gamePlayModel.resuming && p.u.IsItMe)
			return gamePlayModel.playTime;
		//Thời gian xếp bài còn lại của thằng này
		timerOffset = gamePlayModel.TimeSortAndPlayFirst() - timerOffset;
		//Thời gian đếm giờ lần đầu sẽ là max của thời gian chơi & thời gian còn lại ở trên
		return timerOffset < gamePlayModel.playTime ? gamePlayModel.playTime : timerOffset;
	}

	public void ResetOldActionIndex()
	{
		oldPlayIdx = -1;
	}
}