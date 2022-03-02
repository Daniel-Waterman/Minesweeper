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
using System.Windows.Shapes;

namespace MinesweeperApp
{
    /// <summary>
    /// Interaction logic for PlayerNameDialog.xaml
    /// </summary>
    public partial class PlayerNameDialog : Window
    {
        public PlayerNameDialog(string level, int secondsElapsed)
        {
            if (level == "beginner")
            {
                PlayerName = Properties.Settings.Default.BTBeginnerName;
            }
            else if (level == "intermediate")
            {
                PlayerName = Properties.Settings.Default.BTInterName;
            }
            else if (level == "expert")
            {
                PlayerName = Properties.Settings.Default.BTExpertName;
            }
            else
            {
                throw new NotImplementedException();
            }

            PlayerLevel = level;
            time = secondsElapsed;
            DataContext = this;
            InitializeComponent();
        }

        public string PlayerName { get; set; }
        public string PlayerLevel { get; set; }

        private int time;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (PlayerLevel == "beginner")
            {
                Properties.Settings.Default.BTBeginnerName = PlayerName;
                Properties.Settings.Default.BTBeginnerTime = time;
            }
            else if (PlayerLevel == "intermediate")
            {
                Properties.Settings.Default.BTInterName = PlayerName;
                Properties.Settings.Default.BTInterTime = time;
            }
            else if (PlayerLevel == "expert")
            {
                Properties.Settings.Default.BTExpertName = PlayerName;
                Properties.Settings.Default.BTExpertTime = time;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
