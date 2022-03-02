using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperLogic
{
    public static class GameConstants
    {
        public static readonly Dictionary<GameMode, int> RowCounts = new Dictionary<GameMode, int>()
        {
            { GameMode.Beginner, 9 },
            { GameMode.Intermediate, 16 },
            { GameMode.Expert, 16 }
        };

        public static readonly Dictionary<GameMode, int> ColCounts = new Dictionary<GameMode, int>()
        {
            { GameMode.Beginner, 9 },
            { GameMode.Intermediate, 16 },
            { GameMode.Expert, 30 }
        };

        public static readonly Dictionary<GameMode, int> MineCounts = new Dictionary<GameMode, int>()
        {
            { GameMode.Beginner, 10 },
            { GameMode.Intermediate, 40 },
            { GameMode.Expert, 99 }
        };
    }
}
