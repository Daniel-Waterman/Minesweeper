using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace MinesweeperLogic
{
    public class GameModel
    {
        public GameModel(int numOfRows, int numOfCols, int numOfMines)
        {
            rowCount = numOfRows;
            colCount = numOfCols;
            mineCount = minesRemaining = numOfMines;

            InitialiseGrid();
        }

        private int rowCount;
        private int colCount;
        private int mineCount;
        private int minesRemaining;
        public Timer gameTimer = new Timer { Interval = 1000, AutoReset = true };

        private GridSquareModel[,] grid;

        public void InitialiseGrid()
        {
            grid = new GridSquareModel[rowCount, colCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    grid[i, j] = new GridSquareModel { RowPosition = i, ColumnPosition = j};
                }
            }
            
            PlaceMines();
        }

        public void FlagOnOff(int row, int col, bool setFlagOn)
        {
            grid[row, col].IsFlagged = setFlagOn;
            if (setFlagOn)
            {
                minesRemaining -= 1;
            }
            else
            {
                minesRemaining += 1;
            }
            OnFlagChanged();
        }

        public List<int[]> LeftClick(int squareRowPos, int squareColPos)
        {
            List<int[]> output = new List<int[]>();
            GridSquareModel currentSquare = grid[squareRowPos, squareColPos];

            if (currentSquare.SquareValue != GridSquareValue.Mine)
            {
                SquaresRevealed(squareRowPos, squareColPos, ref output);

            }
            else
            {
                OnMineHit(new List<int[]> { new int[] {squareRowPos, squareColPos}});
            }

            return output;
        }

        public List<int[]> DualClick(int squareRowPos, int squareColPos)
        {
            List<int[]> output = new List<int[]>();
            List<int[]> minesHit = new List<int[]>();
            GridSquareModel currentSquare = grid[squareRowPos, squareColPos];

            if (currentSquare.IsRevealed && currentSquare.SquareValue != GridSquareValue.Empty)
            {
                int flagCount = 0;
                var surroundingSquares = GetSurroundingSquares(squareRowPos, squareColPos);
                foreach (var square in surroundingSquares)
                {
                    GridSquareModel newCurrentSquare = grid[square[0], square[1]];
                    if (newCurrentSquare.IsFlagged)
                    {
                        flagCount += 1;
                    }
                    else if (newCurrentSquare.SquareValue == GridSquareValue.Mine)
                    {
                        minesHit.Add(new int[] { square[0], square[1] });
                    }
                    else
                    {
                        SquaresRevealed(square[0], square[1], ref output);
                    }
                }


                if (flagCount == (int)currentSquare.SquareValue)
                {
                    if (minesHit.Count > 0)
                    {
                        OnMineHit(minesHit);
                    }
                }
                else
                {
                    foreach (var square in output)
                    {
                        grid[square[0], square[1]].IsRevealed = false;
                    }
                    output.Clear();
                }
            }

            return output;
        }

        //This method is only called if the starting row and column position is not a mine
        private void SquaresRevealed(int startRow, int startCol, ref List<int[]> output)
        {
            GridSquareModel currentSquare = grid[startRow, startCol];

            if (currentSquare.IsRevealable())
            {
                //Add current square
                output.Add(new int[] { startRow, startCol });
                grid[startRow, startCol].IsRevealed = true;

                //If the square is empty, then the surrounding squares need to be checked as well
                if (grid[startRow, startCol].SquareValue == GridSquareValue.Empty)
                {
                    foreach (var square in GetSurroundingSquares(startRow, startCol))
                    {
                        if (!output.Contains(square))
                        {
                            SquaresRevealed(square[0], square[1], ref output);
                        }
                    }
                }
            }        
        }

        public void FirstMove(int row, int col, bool easyStart)
        {
            List<int[]> toCheck = new List<int[]> { new int[] { row, col } };

            if (easyStart)
            {
                toCheck.AddRange(GetSurroundingSquares(row, col));
            }

            foreach (var item in toCheck)
            {
                row = item[0];
                col = item[1];
                if (grid[row, col].SquareValue == GridSquareValue.Mine)
                {
                    grid[row, col].SquareValue = GridSquareValue.Empty;
                    int[] newPosition = GetNewMinePosition(0, 0);
                    grid[newPosition[0], newPosition[1]].SquareValue = GridSquareValue.Mine;
                }
            }

            CalculateGrid();
            gameTimer.Start();
        }

        public GridSquareValue GetSquareValue(int row, int col)
        {
            return grid[row, col].SquareValue;
        }

        public bool CheckForGameWon()
        {
            if (minesRemaining == 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        if (grid[i,j].IsRevealable())
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                return false;
            }

            gameTimer.Stop();
            return true;
        }

        private int[] GetNewMinePosition(int startRow, int startCol)
        {
            int[] output = { startRow, startCol };
            if (grid[startRow, startCol].SquareValue == GridSquareValue.Mine)
            {
                if (startCol == colCount - 1)
                {
                    output = GetNewMinePosition(startRow + 1, 0);
                }
                else
                {
                    output = GetNewMinePosition(startRow, startCol + 1);
                }
            }
            return output;
        }

        private void PlaceMines()
        {
            List<int> randomValues = GetRandomPositions();
            foreach (int value in randomValues)
            {
                int row = value / colCount;
                int column = value % colCount;
                grid[row, column].SquareValue = GridSquareValue.Mine;
            }
        }

        private void CalculateGrid()
        {
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    if (grid[row, col].SquareValue != GridSquareValue.Mine)
                    {
                        int numOfMines = countMines(row, col);
                        grid[row, col].SquareValue = (GridSquareValue)numOfMines;
                    }
                }
            }
        }

        private int countMines(int rowPosition, int colPosition)
        {
            int output = 0;

            List<int[]> toCheck = GetSurroundingSquares(rowPosition, colPosition);
            foreach (var item in toCheck)
            {
                if (grid[item[0], item[1]].SquareValue == GridSquareValue.Mine)
                {
                    output += 1;
                }
            }

            return output;
        }

        private List<int[]> GetSurroundingSquares(int rowPosition, int colPosition)
        {
            List<int[]> squares = new List<int[]>
            {
                 new int[] { rowPosition - 1, colPosition - 1 },
                 new int[] { rowPosition - 1, colPosition     },
                 new int[] { rowPosition - 1, colPosition + 1 },
                 new int[] { rowPosition,     colPosition - 1 },
                 new int[] { rowPosition,     colPosition + 1 },
                 new int[] { rowPosition + 1, colPosition - 1 },
                 new int[] { rowPosition + 1, colPosition     },
                 new int[] { rowPosition + 1, colPosition + 1 }
            };

            for (int i = squares.Count - 1; i >= 0; i--)
            {
                if (squares[i][0] < 0 || squares[i][1] < 0 || squares[i][0] >= rowCount || squares[i][1] >= colCount)
                {
                    squares.RemoveAt(i);
                }
            }

            return squares;
        }

        private List<int> GetRandomPositions()
        {
            Random random = new Random();

            List<int> output = new List<int>();
            int countAdded = 0;

            while (countAdded < mineCount)
            {
                int temp = random.Next(0, rowCount * colCount);
                if (!output.Contains(temp))
                {
                    output.Add(temp);
                    countAdded += 1;
                }
            }

            return output;
        }

        public event EventHandler<int> FlagsChanged;

        protected void OnFlagChanged()
        {
            FlagsChanged?.Invoke(this, minesRemaining);
        }

        public event EventHandler<List<int[]>> EndGameMineHit;

        protected void OnMineHit(List<int[]> minesHit)
        {
            gameTimer.Stop();
            EndGameMineHit?.Invoke(this, minesHit);
        }

        //public event EventHandler EndGameWon;

        //protected void OnGameWon()
        //{
        //    throw new NotImplementedException();
        //}
    }
}