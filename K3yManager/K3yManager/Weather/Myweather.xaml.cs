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

namespace K3yManager.Weather
{
    /// <summary>
    /// Myweather.xaml 的交互逻辑
    /// </summary>
    public partial class Myweather : Window
    {
        public Myweather()
        {
            InitializeComponent();
        }

        private void weaquery_Click(object sender, RoutedEventArgs e)
        {
            Weather.WeatherWebServiceSoapClient w = new WeatherWebServiceSoapClient("WeatherWebServiceSoap");
            string[] s = new string[23];
            string strcity = city.Text.Trim();

            try
            {
                s = w.getWeatherbyCityName(strcity);
            }
            catch
            {
                MessageBox.Show("can't use now because it's will break sometimes");
                return; 
            }

            if(s[8] =="")
            {
                return; 
            }
            else
            {
                string aa = "../pic/" + s[8];
                image1.Source = new BitmapImage(new Uri(aa, UriKind.Relative));
                label3.Content = s[1] + " " + s[6];
                moreinfo.Text = s[10]; 
            }


            if (s[14] == "")
            {
                return;
            }
            else
            {
                string aa = "../pic/" + s[14];
                image1.Source = new BitmapImage(new Uri(aa, UriKind.Relative));
                label3.Content = s[12] + " " + s[11]+ " "+s[13];
              
            }

            if (s[16] == "")
            {
                return;
            }
            else
            {
                string aa = "../pic/" + s[19];
                image1.Source = new BitmapImage(new Uri(aa, UriKind.Relative));
                label3.Content = s[17] + " " + s[16]+" "+s[18];
         
            }

        }
    }
}
