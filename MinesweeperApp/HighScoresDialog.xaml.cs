using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class HighScoresDialog : Window
    {
        public HighScoresDialog()
        {
            BeginnerTime = Properties.Settings.Default.BTBeginnerTime;
            InterTime = Properties.Settings.Default.BTInterTime;
            ExpertTime = Properties.Settings.Default.BTExpertTime;
            BeginnerName = Properties.Settings.Default.BTBeginnerName;
            InterName = Properties.Settings.Default.BTInterName;
            ExpertName = Properties.Settings.Default.BTExpertName;

            DataContext = this;
            InitializeComponent();
        }

        public int BeginnerTime { get; private set; }
        public int InterTime { get; private set; }
        public int ExpertTime { get; private set; }

        public string BeginnerName { get; private set; }
        public string InterName { get; private set; }
        public string ExpertName { get; private set; }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            BeginnerTime = InterTime = ExpertTime = 999;
            BeginnerName = InterName = ExpertName = "Anonymous";
            Properties.Settings.Default.BTBeginnerTime = BeginnerTime;
            Properties.Settings.Default.BTInterTime = InterTime;
            Properties.Settings.Default.BTExpertTime = ExpertTime;
            Properties.Settings.Default.BTBeginnerName = BeginnerName;
            Properties.Settings.Default.BTInterName = InterName;
            Properties.Settings.Default.BTExpertName = ExpertName;

            DataContext = null;
            DataContext = this;
        }
    }
}
