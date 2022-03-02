using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperLogic
{
    public enum GridSquareValue
    {
        Mine = -1,
        Empty,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight
    }
    public enum GameMode
    {
        Beginner,
        Intermediate,
        Expert,
        Custom
    }

    public enum SmileyValue
    {
        Null,
        Happy,
        Shock,
        Dead,
        Shades
    }

    public enum TileValue
    {
        Mine = GridSquareValue.Mine,
        Empty = GridSquareValue.Empty,
        One = GridSquareValue.One,
        Two = GridSquareValue.Two,
        Three = GridSquareValue.Three,
        Four = GridSquareValue.Four,
        Five = GridSquareValue.Five,
        Six = GridSquareValue.Six,
        Seven = GridSquareValue.Seven,
        Eight = GridSquareValue.Eight,
        Default,
        Flag,
        Question,
        MineRed,
        MineCross
    }
}
