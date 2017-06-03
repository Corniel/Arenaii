using Arenaii.AIGames.Data;
using System;

namespace Arenaii.AIGames.LightRiders.Data
{
    [Serializable]
    public class LightRidersSettings : AIGamesSettings
    {
        public LightRidersSettings()
        {
            IsSymetric = true;
            PlayerNames = new[] { PlayerName.player0, PlayerName.player1 };
            TimePerMove = 200;
        }
    }
}
