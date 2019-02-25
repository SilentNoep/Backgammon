using Common.Backgammon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = Common.Backgammon.Color;

namespace WPFClient.Converters
{
    public class EllipseConverter : IValueConverter
    {
        private ImageBrush RedEllipse;
        private ImageBrush WhiteEllipse;

        public EllipseConverter()
        {
            RedEllipse = new ImageBrush();
            WhiteEllipse = new ImageBrush();
            RedEllipse.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/redchip.jpg"));
            WhiteEllipse.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/whitechip.jpg"));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Cell Cell = (Cell)value;
            ObservableCollection<Ellipse> col = new ObservableCollection<Ellipse>();
            var ColorStroke = new SolidColorBrush(Colors.Black);
            for (int i = 0; i < Cell.NumOfSoldiers; i++)
            {
                Ellipse e = new Ellipse();


                if(Cell.ID == 50 || Cell.ID == -50)
                {
                    e.Height = 8;
                    e.Width = 31;
                }
                else
                {
                    e.Height = 25;
                    e.Width = 25;
                    e.StrokeThickness = 3;
                }
                if (Cell.ColorOfCell == Color.Red)
                {
                    e.Fill = RedEllipse;
                    if (Cell.IsPicked)
                    {
                        if (Cell.ID <= 11 || Cell.ID == 24)
                        {
                            if (i == Cell.NumOfSoldiers - 1)
                                e.Stroke = ColorStroke;
                        }
                        else
                        {
                            if (i == 0)
                                e.Stroke = ColorStroke;
                        }
                    }
                }
                else
                {
                    e.Fill = WhiteEllipse;
                    if (Cell.IsPicked)
                    {
                        if (Cell.ID <= 11 && Cell.ID != -1 && Cell.ID != 24)
                        {
                            if (i == Cell.NumOfSoldiers - 1)
                                e.Stroke = ColorStroke;
                        }
                        else
                        {
                            if (i == 0)
                                e.Stroke = ColorStroke;
                        }
                    }
                }

                col.Add(e);
            }
            return col;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
