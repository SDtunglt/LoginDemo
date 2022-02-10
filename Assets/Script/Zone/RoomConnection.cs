using Sfs2X.Core;
using Sfs2X.Entities;
using UnityEngine;

public class RoomConnection : ConnectionCpnt
{
    private BoardInfoModel boardInfoModel = BoardInfoModel.Instance;
    private ScreenManager screenManager;

    private void Awake()
    {
        screenManager = ScreenManager.Instance;
        mapListener.Add(SFSEvent.ROOM_REMOVE, RoomRemoved);

        mapListener.Add(SFSEvent.ROOM_ADD, RoomCountChanged);
        mapListener.Add(SFSEvent.USER_COUNT_CHANGE,RoomCountChanged);
        mapListener.Add(SFSEvent.ROOM_VARIABLES_UPDATE,RoomVarsUpdated);

        mapListener.Add(SFSEvent.ROOM_PASSWORD_STATE_CHANGE, RoomVarsUpdated);
    }

    private void RoomVarsUpdated(BaseEvent evt)
    {
        var room = (Room) evt.Params["room"];

        var arr = room.GroupId.Split('_');
        var zoneId = int.Parse(arr[0]);
        if(!GameConfig.IsTourZoneId(zoneId) && room.IsGame)
        {
            boardInfoModel.UpdateBoardInfoByRoom(room);
        }
    }

    private void RoomCountChanged(BaseEvent evt)
    {
        var room = (Room) evt.Params["room"];
        if(screenManager.inRoom) boardInfoModel.UpdateBoardInfoByRoom(room);
    }

    private void RoomRemoved(BaseEvent evt)
    {
        var room = (Room) evt.Params["room"];

        if(room.IsGame)
        {
            boardInfoModel.UpdateBoardInfoByRoom(room, true);
        }
    }
}
