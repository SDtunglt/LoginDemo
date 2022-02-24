using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sfs2X.Entities;
using UnityEngine;

public class GamePlayModel : Singleton<GamePlayModel>
{
    private UserModel userModel = UserModel.Instance;
    public static bool IsReplay = false;

    private KickPlayerOutBoardSignal kickPlayerOutBoardSignal =
        Signals.Get<KickPlayerOutBoardSignal>();

    public bool resuming;
    public Room game;
    public ZoneInfo zoneInfo;
    public List<SDPlayer> sdplayers = new List<SDPlayer>();

    public static bool isBoardPlaying = false;
    public static bool isWaitforComfirmMouseIdle = false;
    public int chiuTime => (playTime + 5) / 3;

    public int playTime
    {
        get
        {
            if (!IsReplay)
            {
                return game.GetVariable(GameConfig.VAR_TIME).GetIntValue() / 1000;
            }
            else
            {
                return ReplayModel.turnTime / 1000;
            }
        }
    }

    public int ChiuTime => (playTime + 5) / 3;

    public int minScoreU
    {
        get
        {
            if (!IsReplay)
            {
                return game.ContainsVariable(GameConfig.VAR_MIN_U)
                    ? game.GetVariable(GameConfig.VAR_MIN_U).GetIntValue()
                    : 2;
            }
            else
            {
                return ReplayModel.minScore;
            }
        }
    }

    public bool IsAutoXep
    {
        get
        {
            if (!IsReplay)
            {
                return !game.ContainsVariable(GameConfig.VAR_AUTO_XEP) ||
                       game.GetVariable(GameConfig.VAR_AUTO_XEP).GetBoolValue();
            }

            return true;
        }
    }

    public bool IsULao
    {
        get
        {
            if (!IsReplay)
            {
                return !game.ContainsVariable(GameConfig.VAR_U_LAO) ||
                       game.GetVariable(GameConfig.VAR_U_LAO).GetBoolValue();
            }

            return false;
        }
    }

    public long GaGop
    {
        get
        {
            if (!IsReplay)
            {
                return game.ContainsVariable(GameConfig.VAR_GA_GOP)
                    ? (long) game.GetVariable(GameConfig.VAR_GA_GOP).GetDoubleValue() : 0;
            }

            return 0;
        }
    }

    public List<int> getPrevPlayers
    {
        get
        {
            var prevPlayersVar = game.GetVariable(GameConfig.VAR_PREV_PLAYERS);
            return prevPlayersVar?.GetSFSObjectValue().GetIntArray("v").ToList();
        }
    }

    public int[] GETPositions()
    {
        var posisionVar = game.GetVariable(GameConfig.VAR_PLAYERS_POS);
        return posisionVar == null ? new int[] { } : posisionVar.GetSFSObjectValue().GetIntArray("p");
    }

    public bool isNuoiGa;

    public void UpdateNuoiGa()
    {
        isNuoiGa = game.ContainsVariable(GameConfig.VAR_GA_NUOI);
    }

    public void UpdateReplayNuoiGa()
    {
        isNuoiGa = ReplayModel.gaScore >= 0;
    }

    public bool MoBocNoc
    {
        get
        {
            if (!IsReplay)
            {
                return !game.ContainsVariable(GameConfig.VAR_BOC_NOC) ||
                       game.GetVariable(GameConfig.VAR_BOC_NOC).GetIntValue() != 0;
            }
            else return true;
        }
    }

    public bool EnableMoBocNoc
    {
        get
        {
            if(!IsReplay)
            {
                return game.ContainsVariable(GameConfig.VAR_BOC_NOC) && 
                        game.GetVariable(GameConfig.VAR_BOC_NOC).GetIntValue() != -1;
            }
            else
            {
                return true;
            }
        }
    }

    public double gaNuoi
    {
        get
        {
            if(!IsReplay)
            {
                return game.ContainsVariable(GameConfig.VAR_GA_NUOI) 
                    ? game.GetVariable(GameConfig.VAR_GA_NUOI).GetDoubleValue() 
                    : 0;
            }
            else
            {
                return ReplayModel.gaScore;
            }
        }
    }

    public int stake
    {
        get
        {
            if(!IsReplay)
            {
                return game.GetVariable(GameConfig.VAR_STAKE).GetIntValue();
            }
            else
            {
                return ReplayModel.stake;
            }
        }
    }

    public bool isLocked
    {
        get
        {
            if(!IsReplay)
            {
                return game.IsPasswordProtected;
            }
            else
            {
                return false;
            }
        }
    }

    public int sitCount;
    
    private int _status = BoardStatus.NOT_INIT;

    public int status
    {
        get => _status;
        set
        {
            _status = value;
            isBoardPlaying = _status != BoardStatus.WAITING && isPlayer;
        }
    }

    public bool isPlaying => _status != BoardStatus.WAITING;

    public string prevWinnerId
    {
        get
        {
            if(!IsReplay)
            {
                return game.ContainsVariable(GameConfig.VAR_PREV_WIN)
                    ? game.GetVariable(GameConfig.VAR_PREV_WIN).GetStringValue()
                    : null;
            }
            else
            {
                return ReplayModel.prevWin;
            }
        }
    }

    public int myIdx = -1;
    public bool isBoardOwner => myIdx == 0;
    public SDPlayer myPlayer => myIdx == -1 ? null : sdplayers[myIdx];
    public bool isPlayer => myIdx != -1;

    private int PrevWinnerIdx()
    {
        if(prevWinnerId == null)
            return -1;
        for(var i = sdplayers.Count - 1; i >= 0; i--)
            if(prevWinnerId == sdplayers[i].uid)
                return -i;
        return -1;
    }

    public int UserBocCaiIdx()
    {
        var i = PrevWinnerIdx();
        return i == -1 ? 0 : i;
    }

    public bool AmIBocCai() => UserBocCaiIdx() == myIdx;

    public int GETPlayerIdx(User u)
    {
        for (var i = sdplayers.Count - 1; i >= 0; i--)
            if(sdplayers[i].uid.Equals(u.Name))
                return i;
        return -1;
    }

    public int GETPlayerIdx(string uid)
    {
        for(var i = sdplayers.Count - 1;i >= 0; i--)
            if(sdplayers[i].uid.Equals(uid))
                return i;
        return -1;
    }

    public SDPlayer GetPlayer(string uid)
    {
        for(var i = sdplayers.Count - 1; i >= 0; i--)
            if(sdplayers[i].uid.Equals(uid))
                return sdplayers[i];
        return null;
    }

    public void InitGameState()
    {
        status = game?.GetVariable(GameConfig.VAR_SITCOUNT).GetIntValue() == -1
        ? BoardStatus.WAITING
        : BoardStatus.NOT_INIT;
    }

    public void initReplayState()
    {
        status = BoardStatus.NOT_INIT;
    }

    private void UpdateMyIdx()
    {
        myIdx = -1;
        for(var i = sdplayers.Count - 1; i>= 0; i--)
            if(sdplayers[i].u.IsItMe)
            {
                myIdx = i;
                break;
            }
    }

    public void UpdateSeats()
    {
        UpdateMyIdx();
        for(var i =0; i < sdplayers.Count; i++)
        {
            sdplayers[i].seat = Seat(i);
        }
    }

    public int Seat(int i)
    {
        if(isPlayer)
        {
            Debug.Log("Update Seat With Player");
            i -= myIdx;
            if(i < 0)
                i += sitCount;
        }

        if(i != 1) return i;

        return sitCount == 2 ? 2 : 1;
    }

    public bool OherPlayerCanChiu(int cardValue, int uIdx = -1)
    {
        if(uIdx == -1)
            uIdx = myIdx;
        for(var i = 0; i < sitCount; i++)
            if(i != uIdx && sdplayers[i].CanChiu(cardValue))
                return true;
        return false;
    }

    public int MyOtherInConnectedPlayers()
    {
        var ret = 0;
        for(var i = 0; i < myIdx; i++)
            if(!sdplayers[i].dis)
                ret++;
        return ret;
    }

    public bool IsAllOtherPlayerBaoOrDis(int uIdxExclude)
    {
        for(var i = 0; i < sitCount; i++)
            if(i != uIdxExclude && !sdplayers[i].dis && !sdplayers[i].bao)
                return false;
        return true;
    }

    public long CurrMaxStake()
    {
        var level = userModel.gVO.level - zoneInfo.level == 0 ? 1 : userModel.gVO.level - zoneInfo.level;
        var stake = zoneInfo.minStake + (long) level * (long) zoneInfo.stakePerLevel;
        return stake > zoneInfo.maxStake ? zoneInfo.maxStake : stake;
    }

    public int TimeSortAndPlayFirst()
    {
        return playTime + (!ScreenManager.Instance.InChallenge() ? (zoneInfo.canManuallySortCards ? 50 : 10) : (IsAutoXep ? 10 : 50));
    }

    public string KhongChoiU()
    {
        return "Không chơi ù " + (minScoreU == 3 ? "Suông" : "dưới" + minScoreU + "điểm");
    }

    public bool xinChoi
    {
        get
        {
            if(!IsReplay)
            {
                return game.ContainsVariable(GameConfig.VAR_GAME_MODE) &&
                        game.GetVariable(GameConfig.VAR_GAME_MODE).GetIntValue() >= 1;
            }

            return false;
        }
    }

    public bool tinhDiem =>
        game != null && game.ContainsVariable(GameConfig.VAR_GAME_MODE)&&
        game.GetVariable(GameConfig.VAR_GAME_MODE).GetIntValue() == 2;

    public int gameNo
    {
        get
        {
            return game != null && game.ContainsVariable(GameConfig.VAR_GAME_NO)
                ? game.GetVariable(GameConfig.VAR_GAME_NO).GetIntValue()
                : 0;
        }
    }

    public ScoreVO GetScoreVO()
    {
        var plScore = game?.GetVariable(GameConfig.VAR_SCORE);
        return plScore != null ? new ScoreVO(plScore.GetSFSObjectValue()) : null;
    }
}
