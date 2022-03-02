using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MinesweeperLogic;

namespace MinesweeperApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel viewModel;

        public MainWindow()
        {
            viewModel = new MainViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void SmileyClick(object sender, RoutedEventArgs e)
        {
            viewModel.SetUpGame();
        }

        private void BestTimesClick(object sender, RoutedEventArgs e)
        {
            HighScoresDialog bestTimesDialog = new HighScoresDialog();
            bestTimesDialog.Owner = this;
            bestTimesDialog.ShowDialog();
        }

        private bool mouseLeftDown = false;
        private bool mouseRightDown = false;

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var data = getData(sender);

            if (e.ChangedButton == MouseButton.Left)
            {
                mouseLeftDown = true;
            }
            else if(e.ChangedButton == MouseButton.Right)
            {
                mouseRightDown = true;
            }

            if (mouseRightDown && mouseLeftDown)
            {
                //Implement tiles that need to preview when both buttons are clicked
            }
            else if (mouseLeftDown)
            {
                viewModel.TempSmiley = SmileyValue.Shock;
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var data = getData(sender);
            int row = data.RowPosition;
            int col = data.ColumnPosition;

            if (mouseLeftDown && mouseRightDown)
            {
                viewModel.DualClick(row, col);
                mouseLeftDown = false;
                mouseRightDown = false;
            }
            else if (mouseLeftDown)
            {
                viewModel.LeftClick(row, col);
            }
            else if (mouseRightDown)
            {
                viewModel.RightClick(row, col);
            }

            viewModel.TempSmiley = SmileyValue.Null;

            if (e.ChangedButton == MouseButton.Left)
            {
                mouseLeftDown = false;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                mouseRightDown = false;
            }
        }

        private TileViewModel getData(object sender)
        {
            Button btn = (Button)sender;
            return (TileViewModel)btn.DataContext;
        }

        protected override void OnClosed(EventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosed(e);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                viewModel.SetUpGame();
            }
        }
    }
}
