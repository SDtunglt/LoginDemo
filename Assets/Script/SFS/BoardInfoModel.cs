using System.Collections.Generic;
using Sfs2X.Entities;
using UnityEngine;

public class BoardInfoModel : Singleton<BoardInfoModel>
{
    private List<BoardInfo> boardInfos;

    private UpdateBoardInfoSignal updateBoardInfoSignal = Signals.Get<UpdateBoardInfoSignal>();
    public void InitBoardInfos()
    {
        boardInfos = new List<BoardInfo>();
        boardInfos.SetCount(GameConfig.MaxBoardInRoom, new BoardInfo());
        for (var i = 0; i < boardInfos.Count; i++)
        {
            boardInfos[i] = new BoardInfo();
        }
    }

    public void ResetBoardInfo(Room boardRoom)
    {
        var boardData = boardRoom.Name.Split('_');
        var z = int.Parse(boardData[0]);
        var group = int.Parse(boardData[1]);
        var b = int.Parse(boardData[2]);
        if (z == GameConfig.IdRoomVuongPhu) {
            b += GetStartPosition(group);
        }

        if (boardInfos != null)
        {
            boardInfos[b].Reset(z);
            updateBoardInfoSignal.Dispatch(b);
        }
        
    }

    public BoardInfo GetInfo(int boardNumber)
    {
        return boardInfos[boardNumber];
    }

    public bool IsInited()
    {
        return boardInfos!=null;
    }

    private static int GetStartPosition(int group)
    {
        var startPos = 0;
        for (var i = group + 1; i <= GameConfig.PhuDeGroupId; i++)
        {
            startPos += GameConfig.VuongCfg[i].numBoard;
        }

        return startPos;
    }

    public void UpdateBoardInfoByRoom(Room room, bool isRemoved = false)
    {
        var arr = room.Name.Split('_');
        var zoneId = int.Parse(arr[0]);
        
        var group = int.Parse(arr[1]);
        var id = int.Parse(arr[2]);
        if (zoneId == GameConfig.IdRoomVuongPhu) {
            id += GetStartPosition(group);
        }
        boardInfos[id].Update(room);
        
        if (isRemoved)
        {
            boardInfos[id].Reset(zoneId);
        }
        else
        {
            boardInfos[id].Update(room);
        }

        updateBoardInfoSignal.Dispatch(id);
    }

    public void UpdateBoardInfoByGroup(List<Room> newRooms)
    {
        var groupData = newRooms[0].GroupId.Split('_');
        var z = int.Parse(groupData[0]);
        var group = int.Parse(groupData[1]);
        int i, b;
        if (z == GameConfig.IdRoomVuongPhu)
        {
            var startPos = GetStartPosition(group);
            var numBoard = GameConfig.VuongCfg[group].numBoard;
            for (i = startPos; i < startPos + numBoard; i++)
                boardInfos[i].Reset(z);

            foreach (var r in newRooms)
            {
                if (!r.IsGame) continue;
                b = int.Parse(r.Name.Split('_')[2]);
                if (b < numBoard)
                {
                    boardInfos[startPos + b].Update(r);
                }
            }

            for (i = startPos; i < startPos + numBoard; i++)
            {
                updateBoardInfoSignal.Dispatch(i);
            }
        }
        else
        {
            for (i = 0; i < GameConfig.MaxBoardInRoom; i++)
            {
                boardInfos[i].Reset(z);   
            }
            foreach (var r in newRooms)
            {
                if (!r.IsGame) continue;
                b = int.Parse(r.Name.Split('_')[2]);
                boardInfos[b].Update(r);
            }

            for (i = 0; i < GameConfig.MaxBoardInRoom; i++)
                updateBoardInfoSignal.Dispatch(i);
        }
    }
}