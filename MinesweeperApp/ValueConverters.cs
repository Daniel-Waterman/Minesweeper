using MinesweeperLogic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MinesweeperApp
{
    public class IntToSegment : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //It is assumed that a value that can be displayed is passed to the converter
            int valueTemp = (int)value;
            string valueSplit;
            if (valueTemp < 0)
            {
                valueTemp = Math.Abs(valueTemp) % 100;
                valueSplit = valueTemp.ToString().PadLeft(2, '0');
                valueSplit = "-" + valueSplit;
            }
            else
            {
                valueTemp %= 1000;
                valueSplit = valueTemp.ToString().PadLeft(3, '0');
            }

            return valueSplit[int.Parse((string)parameter)];

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GameModeToView : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GameMode gameMode = (GameMode)value;
            if ((int)gameMode == int.Parse((string)parameter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)parameter)
            {
                case "0":
                    return GameMode.Beginner;
                case "1":
                    return GameMode.Intermediate;
                case "2":
                    return GameMode.Expert;
                case "3":
                    return GameMode.Custom;
                default:
                    return GameMode.Beginner;
            }
        }
    }
}
