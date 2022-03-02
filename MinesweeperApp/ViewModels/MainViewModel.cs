using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using System.Windows.Threading;
using MinesweeperLogic;

namespace MinesweeperApp
{
    internal class MainViewModel : INotifyPropertyChanged
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            soundWin.Open(new Uri(@"Sounds\Win.mp3", UriKind.Relative));
            soundMine.Open(new Uri(@"Sounds\MineHit.mp3", UriKind.Relative));
            soundTick.Open(new Uri(@"Sounds\Tick.mp3", UriKind.Relative));
            
            StartUpGame();

            SetUpGame();
        }

        private void StartUpGame()
        {
            try
            {
                _gameMode = (GameMode)Properties.Settings.Default.Difficulty;
            }
            catch (InvalidCastException)
            {
                _gameMode = GameMode.Beginner;
            }

            if (_gameMode == GameMode.Custom)
            {
                GridSizeRows = Properties.Settings.Default.CustomHeight;
                GridSizeCols = Properties.Settings.Default.CustomWidth;
                _gridSizeMines = Properties.Settings.Default.CustomMines;
            }
            else
            {
                GridSizeRows = GameConstants.RowCounts[_gameMode];
                GridSizeCols = GameConstants.ColCounts[_gameMode];
                _gridSizeMines = GameConstants.MineCounts[_gameMode];
            }

            _marks = Properties.Settings.Default.Marks;
            _sounds = Properties.Settings.Default.Sounds;
            _easyStart = Properties.Settings.Default.EasyStart;
        }

        private int[]? LoadCustomDialog()
        {
            CustomGameDialog customDialog = new CustomGameDialog(_gridSizeRows, _gridSizeCols, _gridSizeMines);
            customDialog.Owner = System.Windows.Application.Current.MainWindow;
            if (customDialog.ShowDialog() == true) //Dialog only returns true if the OK button is pressed
            {
                int rowSize = customDialog.FieldHeight;
                int colSize = customDialog.FieldWidth;
                int mineSize = customDialog.Mines;
                if (rowSize < 9)
                {
                    rowSize = 9;
                }
                else if (rowSize > 24)
                {
                    rowSize = 24;
                }
                if (colSize < 9)
                {
                    colSize = 9;
                }
                else if (colSize > 30)
                {
                    colSize = 30;
                }
                if (mineSize < 10)
                {
                    mineSize = 10;
                }
                else if (mineSize > rowSize * colSize - 25)
                {
                    mineSize = rowSize * colSize;
                }

                return new int[] {rowSize, colSize, mineSize};
            }
            else
            {
                return null;
            }            
        }

        private void LoadNameDialog(string level)
        {
            PlayerNameDialog nameDialog = new PlayerNameDialog(level, _secondElapsed);
            nameDialog.Owner = System.Windows.Application.Current.MainWindow;
            if (nameDialog.ShowDialog() == true)
            {
                HighScoresDialog scoresDialog = new HighScoresDialog();
                scoresDialog.Owner = System.Windows.Application.Current.MainWindow;
                scoresDialog.ShowDialog();
            }
        }

        public void NewGame()
        {
            if (_gameMode == GameMode.Custom)
            {
                var customValues = LoadCustomDialog();
                if (customValues != null)
                {
                    GridSizeRows = customValues[0];
                    GridSizeCols = customValues[1];
                    _gridSizeMines = customValues[2];
                }
            }
            else
            {
                GridSizeRows = GameConstants.RowCounts[_gameMode];
                GridSizeCols = GameConstants.ColCounts[_gameMode];
                _gridSizeMines = GameConstants.MineCounts[_gameMode];
            }

            SetUpGame();
        }

        internal void SetUpGame()
        {
            if (Game != null)
            {
                Game.gameTimer.Dispose();
            }

            Game = new GameModel(_gridSizeRows, _gridSizeCols, _gridSizeMines);

            MinesRemaining = _gridSizeMines;

            Game.FlagsChanged += UpdateMinesEvent;
            Game.EndGameMineHit += MineHitEvent;
            Game.gameTimer.Elapsed += SecondElapsedEvent;

            SmileyStatus = SmileyValue.Happy;
            SecondsElapsed = 0;
            GridClickable = true;
            gameStarted = false;

            //Initialising Tiles
            _tileList.Clear();
            for (int i = 0; i < _gridSizeRows; i++)
            {
                for (int j = 0; j < _gridSizeCols; j++)
                {
                    _tileList.Add(new TileViewModel(i, j));
                }
            }
        }

        public void LeftClick(int rowPos, int colPos)
        {
            if (gameStarted)
            {
                var tile = _tileList.FirstOrDefault(t => t.RowPosition == rowPos && t.ColumnPosition == colPos);
                if (tile != null && tile.IsClickable)
                {
                    List<int[]> tilesToReveal = Game.LeftClick(rowPos, colPos);
                    foreach (var revealTile in tilesToReveal)
                    {
                        TileValue newTileValue = (TileValue)Game.GetSquareValue(revealTile[0], revealTile[1]);
                        var tileToChange = _tileList.FirstOrDefault(t => t.RowPosition == revealTile[0] && t.ColumnPosition == revealTile[1]);
                        if (tileToChange != null)
                        {
                            tileToChange.TileImage = newTileValue;
                            tileToChange.IsClickable = false;
                        }
                    }
                    if (Game.CheckForGameWon())
                    {
                        GameWonEvent();
                    }
                } 
            }
            else
            {
                Game.FirstMove(rowPos, colPos, _easyStart);
                gameStarted = true;
                LeftClick(rowPos, colPos);
            }
        }

        public void RightClick(int rowPos, int colPos)
        {
            var tile = _tileList.FirstOrDefault(t => t.RowPosition == rowPos && t.ColumnPosition == colPos);
            if (tile != null)
            {
                if (tile.TileImage == TileValue.Default)
                {
                    tile.TileImage = TileValue.Flag;
                    tile.IsClickable = false;
                    Game.FlagOnOff(rowPos, colPos, true);
                }
                else if (tile.TileImage == TileValue.Flag)
                {
                    if (_marks)
                    {
                        tile.TileImage = TileValue.Question;
                    }
                    else
                    {
                        tile.TileImage = TileValue.Default;
                    }
                    Game.FlagOnOff(rowPos, colPos, false);
                    tile.IsClickable = true;
                }
                else if (tile.TileImage == TileValue.Question)
                {
                    tile.TileImage = TileValue.Default;
                    tile.IsClickable = true;
                }

                if (Game.CheckForGameWon())
                {
                    GameWonEvent();
                }
            }
        }

        public void DualClick(int rowPos, int colPos)
        {
            var tile = _tileList.FirstOrDefault(t => t.RowPosition == rowPos && t.ColumnPosition == colPos);
            if (tile != null && !tile.IsClickable)
            {
                List<int[]> tilesToReveal = Game.DualClick(rowPos, colPos);
                foreach (var revealTile in tilesToReveal)
                {
                    TileValue newTileValue = (TileValue)Game.GetSquareValue(revealTile[0], revealTile[1]);
                    var tileToChange = _tileList.FirstOrDefault(t => t.RowPosition == revealTile[0] && t.ColumnPosition == revealTile[1]);
                    if (tileToChange != null)
                    {
                        tileToChange.TileImage = newTileValue;
                        tileToChange.IsClickable = false;
                    }
                }

                if (Game.CheckForGameWon())
                {
                    GameWonEvent();
                }
            }
        }

        private MediaPlayer soundWin = new MediaPlayer();
        private MediaPlayer soundMine = new MediaPlayer();
        private MediaPlayer soundTick = new MediaPlayer();

        private GameModel Game;

        private bool gameStarted = false;

        private bool _gridClickable;

        public bool GridClickable
        {
            get { return _gridClickable; }
            set { _gridClickable = value; NotifyPropertyChanged(); }
        }

        private GameMode _gameMode;     

        public GameMode GameMode
        {
            get { return _gameMode; }
            set
            { 
                _gameMode = value;
                Properties.Settings.Default.Difficulty = (int)_gameMode;
                NotifyPropertyChanged();

                NewGame();
            }
        }

        private bool _marks { get; set; }
        public bool Marks { get { return _marks; } set { _marks = value; Properties.Settings.Default.Marks = value; } }

        private bool _sounds { get; set; }
        public bool Sounds { get { return _sounds; } set { _sounds = value; Properties.Settings.Default.Sounds = value; } }

        private bool _easyStart { get; set; }
        public bool EasyStart { get { return _easyStart; } set { _easyStart = value; Properties.Settings.Default.EasyStart = value; } }

        private int _minesRemaining;

        public int MinesRemaining
        {
            get { return _minesRemaining; }
            set { _minesRemaining = value; NotifyPropertyChanged(); }
        }

        private SmileyValue _tempSmiley;
        public SmileyValue TempSmiley
        {
            get { return _tempSmiley; }
            set { _tempSmiley = value; NotifyPropertyChanged(nameof(SmileyStatus)); }
        }

        private SmileyValue _smileyStatus;
        public SmileyValue SmileyStatus
        {
            get
            {
                if (_tempSmiley != SmileyValue.Null)
                {
                    return _tempSmiley;
                }
                else
                {
                    return _smileyStatus;
                }
            }
            set
            {
                _smileyStatus = value;
                NotifyPropertyChanged();
            }
        }

        private int _secondElapsed;

        public int SecondsElapsed
        {
            get { return _secondElapsed; }
            set { _secondElapsed = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<TileViewModel> _tileList = new ObservableCollection<TileViewModel>();

        public ObservableCollection<TileViewModel> TileList
        {
            get { return _tileList; }
            set { _tileList = value; NotifyPropertyChanged(); }
        }

        private int _gridSizeRows;

        public int GridSizeRows
        {
            get { return _gridSizeRows; }
            set { _gridSizeRows = value; NotifyPropertyChanged(); }
        }

        private int _gridSizeCols;

        public int GridSizeCols
        {
            get { return _gridSizeCols; }
            set { _gridSizeCols = value; NotifyPropertyChanged(); }
        }

        private int _gridSizeMines;

        private void SecondElapsedEvent(object? sender, ElapsedEventArgs e)
        {
            SecondsElapsed += 1;
            if (_sounds)
            {
                soundTick.Dispatcher.Invoke(() =>
                {
                    soundTick.Play();
                    soundTick.Position = TimeSpan.Zero;
                });
            }
        }

        private void UpdateMinesEvent(object? sender, int e)
        {
            MinesRemaining = e;
        }

        private void GameWonEvent()
        {
            SmileyStatus = SmileyValue.Shades;
            if (_sounds)
            {
                soundWin.Dispatcher.Invoke(() =>
                {
                    soundWin.Play();
                    soundWin.Position = TimeSpan.Zero;
                });

            }
            GridClickable = false;

            if (_gameMode == GameMode.Beginner && _secondElapsed < Properties.Settings.Default.BTBeginnerTime)
            {
                LoadNameDialog("beginner");
            }
            else if (_gameMode == GameMode.Intermediate && _secondElapsed < Properties.Settings.Default.BTInterTime)
            {
                LoadNameDialog("intermediate");
            }
            else if (_gameMode == GameMode.Expert && _secondElapsed < Properties.Settings.Default.BTExpertTime)
            {
                LoadNameDialog("expert");
            }
        }            

        private void MineHitEvent(object? sender, List<int[]> e)
        {
            SmileyStatus = SmileyValue.Dead;
            foreach (var tile in _tileList)
            {
                int row = tile.RowPosition;
                int col = tile.ColumnPosition;
                TileValue currentTileImage = tile.TileImage;
                GridSquareValue currentSquareValue = Game.GetSquareValue(row, col);

                if (e.Any(t => t[0] == row && t[1] == col))
                {
                    tile.TileImage = TileValue.MineRed;
                }
                else if (currentTileImage == TileValue.Flag && currentSquareValue != GridSquareValue.Mine)
                {
                    tile.TileImage = TileValue.MineCross;
                }
                else if ((currentTileImage == TileValue.Default || currentTileImage == TileValue.Question) && currentSquareValue == GridSquareValue.Mine)
                {
                    tile.TileImage = TileValue.Mine;
                }
            }
            if (_sounds)
            {
                soundMine.Dispatcher.Invoke(() =>
                {
                    soundMine.Play();
                    soundMine.Position = TimeSpan.Zero;
                });
            }
            GridClickable = false;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
