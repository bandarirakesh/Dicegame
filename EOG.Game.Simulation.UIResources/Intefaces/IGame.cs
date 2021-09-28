using System;
using System.Collections.Generic;

namespace EOG.Game.Simulation.UIResources.Intefaces
{
    public interface IGame
    {
        int GameId { get; }
        string GameName { get; }
        IPlayer Winner { get; }
        List<IPlayer> Players { get; }
        int NoOfPlayers { get; }
        void PlayGame(Random random);
    }
}
