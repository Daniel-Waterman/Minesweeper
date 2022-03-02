using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MinesweeperApp
{
    /// <summary>
    /// Interaction logic for CustomGameDialog.xaml
    /// </summary>
    public partial class CustomGameDialog : Window
    {
        public CustomGameDialog(int height, int width, int mines)
        {
            FieldHeight = height;
            FieldWidth = width;
            Mines = mines;
            DataContext = this;
            InitializeComponent();
        }

        public int FieldHeight { get; set; }
        public int FieldWidth { get; set; }
        public int Mines { get; set; }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CustomHeight = FieldHeight;
            Properties.Settings.Default.CustomWidth = FieldWidth;
            Properties.Settings.Default.CustomMines = Mines;
            DialogResult = true;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
