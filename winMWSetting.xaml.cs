
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
    /// Interaction logic for winMarketWatchSettings.xaml
    /// </summary>
    public partial class winMWSettings : Window
    {
        public winMWSettings()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            closeEvent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveEvent();
        }

        private void mwSetting_Loaded(object sender, RoutedEventArgs e)
        {
            loadCombobox();
            loadProperties();

        }


        private void loadProperties()
        {
        //    cpChangePstv.SelectedColor = clsGenfx.convertStringToColor(Properties.Settings.Default["mwChangePstv"].ToString());
        //    cpChangeNgtv.SelectedColor = clsGenfx.convertStringToColor(Properties.Settings.Default["mwChangeNgtv"].ToString());
        //    cpForeColor.SelectedColor = clsGenfx.convertStringToColor(Properties.Settings.Default["mwForeground"].ToString());
        //    cpBGColor.SelectedColor = clsGenfx.convertStringToColor(Properties.Settings.Default["mwBackground"].ToString());
        //    cpMarketUp.SelectedColor = clsGenfx.convertStringToColor(Properties.Settings.Default["mwMarketUp"].ToString());
        //    cpMarketDown.SelectedColor = clsGen.convertStringToColor(Properties.Settings.Default["mwMarketDown"].ToString());

        //    string font = Properties.Settings.Default["mwFontSize"].ToString();
        //    cmbFontSize.SelectedIndex = Convert.ToInt16(font) - 1;
        }

        private void loadCombobox()
        {
            for (int i = 1; i < 25; i++)
            {
                cmbFontSize.Items.Add(i);
            }

            cmbFontSize.SelectedIndex = 12;
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                saveEvent();
            }
            else if (e.Key == Key.Escape)
            {
                closeEvent();
            }
        }

        private void closeEvent()
        {
            this.Close();
        }

        private void saveEvent()
        {

            Properties.Settings.Default["mwChangePstv"] = cpChangePstv.SelectedColor.ToString();
            Properties.Settings.Default["mwChangeNgtv"] = cpChangeNgtv.SelectedColor.ToString();
            Properties.Settings.Default["mwForeground"] = cpForeColor.SelectedColor.ToString();
            Properties.Settings.Default["mwBackground"] = cpBGColor.SelectedColor.ToString();
            Properties.Settings.Default["mwMarketUp"] = cpMarketUp.SelectedColor.ToString();
            Properties.Settings.Default["mwMarketDown"] = cpMarketDown.SelectedColor.ToString();
            Properties.Settings.Default["mwFontSize"] = cmbFontSize.SelectedItem.ToString();
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            //clsApplicationSetting objappstng = new clsApplicationSetting();
            //objappstng.defaultMWSetting();
            //loadProperties();
        }

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
