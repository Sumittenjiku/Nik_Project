using Microsoft.Windows.Controls;
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

namespace NiKu
{
    /// <summary>
    /// Interaction logic for winSetDPcolor.xaml
    /// </summary>
    public partial class winSetDPcolor : Window
    {
        public bool saved;
        public winSetDPcolor()
        {
            InitializeComponent();

            
        }
        public Color[] tlcolor;        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (tlcolor == null)
            {
                tlcolor = new Color[10];

            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    ColorPicker c = (ColorPicker)this.FindName("c" + (i + 1).ToString());
                    c.SelectedColor = tlcolor[i];
                }
            }
        }
        private void btncancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                ColorPicker c = (ColorPicker)this.FindName("c" + (i + 1).ToString());
                tlcolor[i] = c.SelectedColor;
            }
            saved = true;
            this.Close();
            //save to registry
        }
        private void btnreset_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
