using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MinesweeperLogic;

namespace MinesweeperApp
{
    internal class TileViewModel : INotifyPropertyChanged
    {
        public TileViewModel(int row, int col)
        {
            RowPosition = row;
            ColumnPosition = col;
        }

        public void ResetTile(int row, int col)
        {
            IsClickable = true;
            TileImage = TileValue.Default;
            RowPosition = row;
            ColumnPosition = col;
        }

        private bool _isClickable = true;

        public bool IsClickable
        {
            get { return _isClickable; }
            set { _isClickable = value; NotifyPropertyChanged(); }
        }

        private TileValue _tileImage = TileValue.Default;

        public TileValue TileImage
        {
            get { return _tileImage; }
            set { _tileImage = value; NotifyPropertyChanged(); }
        }

        public int RowPosition { get; set; }
        public int ColumnPosition { get; set; }



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
