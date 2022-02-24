using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using UnityEngine;

public class GamePlayLogic : Singleton<GamePlayLogic>
{
    private PlayerJoinedSignal playerJoinedSignal = Signals.Get<PlayerJoinedSignal>();
    private PlayerLeavedSignal playerLeavedSignal = Signals.Get<PlayerLeavedSignal>();

    private GamePlayModel gamePlayModel = GamePlayModel.Instance;

    public void updatePlayers()
    {
        if (gamePlayModel.sdplayers.Count > 0) removeAllPlayers();
        addPlayers();
        gamePlayModel.UpdateSeats();
        playerJoinedSignal.Dispatch();
    }

    public void UpdateReplayPlayers()
    {
        if (gamePlayModel.sdplayers.Count > 0) removeAllPlayers();
        AddReplayPlayers();
        gamePlayModel.UpdateSeats();
        playerJoinedSignal.Dispatch();
    }

    public void addPlayer(User u)
    {
        if (gamePlayModel.GETPlayerIdx(u) != -1) return;
        gamePlayModel.sitCount = gamePlayModel.sdplayers.Count + 1;
        var p = new SDPlayer {u = u};
        gamePlayModel.sdplayers.Add(p);
        sortPlayer(gamePlayModel.sdplayers);
        gamePlayModel.UpdateSeats();

        playerJoinedSignal.Dispatch();
    }

    private void addPlayers()
    {
        List<User> pl, disPlayers;
        if (gamePlayModel.isPlaying)
        {
            disPlayers = getDisPlayers();
            Debug.Log("Player DisConnect: " + disPlayers.Count);
            pl = gamePlayModel.game.PlayerList;
            // remove duplicate user in disPlayers and players
            bool duplicate;
            foreach (var dp in disPlayers)
            {
                duplicate = false;
                foreach (var u in pl)
                {
                    if (u.Name == dp.Name) duplicate = true;
                }

                if (!duplicate) pl.Add(dp);
            }
        }
        else
        {
            pl = gamePlayModel.game.PlayerList;
            disPlayers = new List<User>();
        }

        Debug.Log("Player List Cont: " + pl.Count);
        foreach (var p in pl.Select(t => new SDPlayer
        {
            u = t,
            dis = gamePlayModel.isPlaying && disPlayers.IndexOf(t) != -1
        }))
        {
            gamePlayModel.sdplayers.Add(p);
        }

        //+sortUserList
        sortPlayer(gamePlayModel.sdplayers);
        //-sortUserList
        gamePlayModel.sitCount = gamePlayModel.sdplayers.Count;
    }

    private void AddReplayPlayers()
    {
        var x = 0;
        var uid = UserModel.Instance.uid;
        var i = 0;
        foreach (var p in ReplayModel.users.Select(t => new SDPlayer
        {
            u = new SFSUser(int.Parse(t.uid), t.uid, uid == t.uid),
            dis = false,
            idx = i++
        }))
        {
            p.u.SetVariable(new SFSUserVariable(GameConfig.VAR_COIN, (double)ReplayModel.users[x].coin));
            p.u.SetVariable(new SFSUserVariable(GameConfig.VAR_UNAME, ReplayModel.users[x].uName));
            gamePlayModel.sdplayers.Add(p);
            x++;
        }

        //-sortUserList
        gamePlayModel.sitCount = gamePlayModel.sdplayers.Count;
    }

    public void removePlayer(User u)
    {
        var i = gamePlayModel.GETPlayerIdx(u);
        Signals.Get<OnPlayerLeaveBoard>().Dispatch();
        if (i == -1) return;
        var pl = gamePlayModel.sdplayers[i];
        gamePlayModel.sdplayers.Splice(i, 1);
        gamePlayModel.sitCount = gamePlayModel.sdplayers.Count;
        updatePos();
        playerLeavedSignal.Dispatch(pl);
    }

    public void updatePos()
    {
        sortPlayer(gamePlayModel.sdplayers);
        gamePlayModel.UpdateSeats();
    }

    private void sortPlayer(List<SDPlayer> pl)
    {
        int i, j;
        SDPlayer tmp;
        var n = pl.Count;
        if (isAllPlayerHasIndex(pl))
        {
            var positions = gamePlayModel.GETPositions();
            //+sortUserList
            for (i = 0; i < n; i++)
            for (j = n - 1; j > i; j--)
            {
                var uid1 = int.Parse(pl[j - 1].uid);
                var uid2 = int.Parse(pl[j].uid);
                if (Array.IndexOf(positions, uid1) > Array.IndexOf(positions, uid2))
                {
                    tmp = pl[j - 1];
                    pl[j - 1] = pl[j];
                    pl[j] = tmp;
                }
            }
        }
        else
        {
            //+sortUserList
            for (i = 0; i < n; i++)
            for (j = n - 1; j > i; j--)
                if (pl[j - 1].u.PlayerId > pl[j].u.PlayerId)
                {
                    tmp = pl[j - 1];
                    pl[j - 1] = pl[j];
                    pl[j] = tmp;
                }
        }
    }

    private void SortReplayPlayer(List<SDPlayer> pl)
    {
        int i, j;
        SDPlayer tmp;
        var n = pl.Count;

        //+sortUserList
        for (i = 0; i < n; i++)
        for (j = n - 1; j > i; j--)
            if (pl[j - 1].idx > pl[j].idx)
            {
                tmp = pl[j - 1];
                pl[j - 1] = pl[j];
                pl[j] = tmp;
            }

    }


    private bool isAllPlayerHasIndex(List<SDPlayer> pl)
    {
        var positions = gamePlayModel.GETPositions();
        return pl.All(t => Array.IndexOf(positions, int.Parse(t.uid)) != -1);
    }

    public void removeAllPlayers()
    {
        //sitCount will be update in playerLeave, playerJoin
        // var listPlayers = gamePlayModel.sdplayers;
        // while(gamePlayModel.sdplayers.Count > 1)
        //     for (var i= listPlayers.Count - 1; i >= 0; i--)
        //     {
        //         if (!listPlayers[i].u.IsItMe) removePlayer(listPlayers[i].u);
        //     }
        //
        // //xóa nốt myself:
        // if (gamePlayModel.sdplayers.Count > 0)
        // {
        //     var pl = gamePlayModel.sdplayers[0];
        //     gamePlayModel.sdplayers.Pop();
        //     playerLeavedSignal.Dispatch(pl);
        // }

        gamePlayModel.sdplayers.Clear();
        Signals.Get<UpdatePlayerInBoardSignal>().Dispatch();

        gamePlayModel.sitCount = 0;
        gamePlayModel.myIdx = -1;
    }

    private List<User> getDisPlayers()
    {
        var dis = gamePlayModel.game.GetVariable(GameConfig.VAR_DIS).GetSFSArrayValue();
        var disPlayers = new List<User>();
        for (var i = 0; i < dis.Size(); i++)
        {
            var a = dis.GetSFSArray(i);
            //@see GameRequestHander.java#joinBoard
            //Note: SFSUser.fromSFSArray(mySelf.toSFSArray()).isItMe == false!
            //Note: SFSUser.fromSFSArray(uDump).playerId => throw exception!
            if (a.GetInt(1) == SmartFoxConnection.Instance.mySelf.PlayerId)
                continue;
            var u = toSFSUser(a);
            disPlayers.Add(u);
        }

        return disPlayers;
    }

    private User toSFSUser(ISFSArray a)
    {
        var u = new SFSUser(-1, a.GetInt(0).ToString());
        u.UserManager = SmartFoxConnection.Instance.mySelf.UserManager;
        u.SetPlayerId(a.GetInt(1), gamePlayModel.game);

        var a1 = new SFSUserVariable(GameConfig.VAR_UNAME, a.GetUtfString(2));
        var a2 = new SFSUserVariable(GameConfig.VAR_COIN, double.Parse(a.GetLong(3).ToString()));
        var a4 = new SFSUserVariable(GameConfig.VAR_EXP, a.GetInt(4));
        var a3 = new SFSUserVariable(GameConfig.VAR_IP, double.Parse(a.GetLong(5).ToString()));
        // SDLogger.Log("CHECK " + a1 + " - " + a2 + " - " + a3 + " - " + a4);

        u.SetVariables(new[]
        {
            new SFSUserVariable(GameConfig.VAR_UNAME, a.GetUtfString(2)),
            new SFSUserVariable(GameConfig.VAR_COIN, double.Parse(a.GetLong(3).ToString())),
            new SFSUserVariable(GameConfig.VAR_EXP, a.GetInt(4)),
            new SFSUserVariable(GameConfig.VAR_IP, double.Parse(a.GetLong(5).ToString()))
        });
        return u;
    }

    public static void HandleChiaBai(SFSObject data)
    {
        var vo = new ChiaBaiVO();
        vo.fromSFSObject(data);

        var chiaBaiHandle = new ChiaBaiHandle();
        chiaBaiHandle.Execute(vo);
    }
    
    public static void HandleChiaBai(ISFSObjVO vo)
    {
        var chiaBaiHandle = new ChiaBaiHandle();
        chiaBaiHandle.Execute(vo as ChiaBaiVO);
    }

    public static void HandleChonNoc(SFSObject data)
    {
        var vo = new ChonNocVO();
        vo.fromSFSObject(data);
        ChiaBaiModel.Instance.nocIdx = vo.nocIdx;
        Signals.Get<ChonNocSignal>().Dispatch();
    }
    
    public static void HandleChonNoc(ISFSObjVO data)
    {
      
        ChiaBaiModel.Instance.nocIdx = (data as ChonNocVO).nocIdx;
        Signals.Get<ChonNocSignal>().Dispatch();
    }

    public static void HandleBocCai(SFSObject data)
    {
        var vo = new BocCaiVO();
        vo.fromSFSObject(data);
        ChiaBaiModel.Instance.baiCaiIdx = vo.baiCaiIdx;
        if (GameObject.FindObjectOfType<DealCards>())
        {
            GamePlayModel.Instance.status = BoardStatus.BOC_CAI;
            Signals.Get<BocCaiSignal>().Dispatch();
            if (vo.scoreVaoGa > 0)
            {
                Signals.Get<ShowTimeOutMsgSignal>().Dispatch(SDMsg.Join(AppMsg.VAOGAALL, vo.scoreVaoGa));
            }
        }
        else
        {
            GamePlayModel.Instance.status = BoardStatus.PLAYING;
            Clock.Instance.StartClock(Clock.COUNT_FIRST, GamePlayModel.Instance.TimeSortAndPlayFirst(),
                GamePlayModel.Instance.sdplayers[PlayModel.Instance.curTurn]);
        }
    }
    
    public static void HandleBocCai(ISFSObjVO data)
    {
        var vo = data as BocCaiVO;
        ChiaBaiModel.Instance.baiCaiIdx = vo.baiCaiIdx;
        if (GameObject.FindObjectOfType<DealCards>())
        {
            GamePlayModel.Instance.status = BoardStatus.BOC_CAI;
            Signals.Get<BocCaiSignal>().Dispatch();
            if (vo.scoreVaoGa > 0)
            {
                Signals.Get<ShowTimeOutMsgSignal>().Dispatch(SDMsg.Join(AppMsg.VAOGAALL, vo.scoreVaoGa));
            }
        }
        else
        {
            GamePlayModel.Instance.status = BoardStatus.PLAYING;
            Clock.Instance.StartClock(Clock.COUNT_FIRST, GamePlayModel.Instance.TimeSortAndPlayFirst(),
                GamePlayModel.Instance.sdplayers[PlayModel.Instance.curTurn]);
        }
    }

    public static IEnumerator HandleResume(ResumeVO vo)
    {
        if (vo == null)
            yield break;
        var resumeHandle = new ResumeHandle();
        yield return resumeHandle.Execute(vo);
    }

    public static void HandlePlay(SFSObject data)
    {
        var vo = new PlayVO();
        vo.fromSFSObject(data);

        var playHandle = new PlayHandle();
        playHandle.Execute(vo);
    }
    
    public static void HandlePlay(ISFSObjVO data)
    {
        var vo = data as PlayVO;
        var playHandle = new PlayHandle();
        playHandle.Execute(vo);
    }


    public static void HandleU(SFSObject data)
    {
        var vo = new UVO();
        vo.fromSFSObject(data);

        var uHandle = new UHandle();
        uHandle.Execute(vo);
    }
    public static void HandleU(ISFSObjVO data)
    {
        var vo = data as UVO;
        var uHandle = new UHandle();
        uHandle.Execute(vo);
    }
    

    public static void HandleSumUp(SFSObject data)
    {
        var vo = new SumUpVO();
        vo.fromSFSObject(data);

        var sumUpHandle = new SumUpReceiveHandle();
        sumUpHandle.Execute(vo);
    }
    
    public static void HandleSumUp(ISFSObjVO data)
    {
        var vo = data as SumUpVO;
        var sumUpHandle = new SumUpReceiveHandle();
        sumUpHandle.Execute(vo);
    }
}