using System.Collections.Generic;

public interface IPlayersContainer
{
    List<PlayerMediator> GetPlayers();
    PlayerMediator GetPlayer(int idx);
}