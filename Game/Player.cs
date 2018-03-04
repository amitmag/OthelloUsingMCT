using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    interface Player
    {
        void getPlayers                 // players ids
        (
            ref string player1_1,
            ref string player1_2
        );

        Tuple<int, int> playYourTurn
        (
            Board board,
            TimeSpan timesup,
            char playerChar      // 1 or 2
        );

    }
}
