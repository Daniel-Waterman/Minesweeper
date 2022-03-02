using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperLogic
{
    public class GridSquareModel
    {
        public int RowPosition { get; set; }
        public int ColumnPosition { get; set; }
        public bool IsFlagged { get; set; } = false;
        public bool IsRevealed { get; set; } = false;
        public GridSquareValue SquareValue { get; set; } = GridSquareValue.Empty;
        public bool IsRevealable()
        {
            return !IsFlagged && !IsRevealed;
        }
    }
}
