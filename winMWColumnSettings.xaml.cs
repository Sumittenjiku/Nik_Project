using Newtonsoft.Json;
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
    /// Interaction logic for winMWColumnSettings.xaml
    /// </summary>
    public partial class winMWColumnSettings : Window
    {
        public winMWColumnSettings()
        {
            InitializeComponent();
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            List<string> uncheckedColumns = new List<string>();
            for (int i = 0; i < 9; i++)
            {
                CheckBox chk = (CheckBox)FindName("chk"+i);
                
                if(chk.IsChecked == false)
                    uncheckedColumns.Add(chk.Content.ToString());
            }

            Properties.Settings.Default["MWColumnsHide"] = JsonConvert.SerializeObject(uncheckedColumns);
            Properties.Settings.Default.Save();
            this.Close();

        }

        private void btnClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void winMWColumnSettings_Loaded(object sender, RoutedEventArgs e)
        {
            //clsApplicationSetting objappstng = new clsApplicationSetting();

            //string value = Properties.Settings.Default["MWColumnsHide"].ToString();
            //if(string.IsNullOrEmpty(value))
            //     objappstng.MWColumnvisibilitySetting();

            //List<String> uncheckedColumn = JsonConvert.DeserializeObject<List<String>>(Properties.Settings.Default["MWColumnsHide"].ToString());

            //for (int i = 0; i < 9; i++)
            //{
            //    CheckBox chk = (CheckBox)FindName("chk" + i);
            //    string content = uncheckedColumn.FirstOrDefault(x=> x == chk.Content.ToString());
                
            //    if(string.IsNullOrEmpty(content))
            //    {               
            //        chk.IsChecked = true;
            //    }
            //    else
            //    {
            //     chk.IsChecked = false;
            //    }
            //}

        }
    }
}
