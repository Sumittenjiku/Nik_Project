using Market;
using Market.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Niku___Master_Code
{
    public static class clsGen
    {
        public const int nseHourLimit = 15, mcsHourLimit = 23;
        public static string getAppPath(bool blnData = false)
        {
            //string p = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (blnData)
            {
                string p = AppDomain.CurrentDomain.BaseDirectory;
                string[] arr = p.Split(new char[] { '\\' });
                if (p.Contains("bin"))
                {
                    int index = Array.IndexOf(arr, "bin");
                    Array.Resize(ref arr, index);
                    p = string.Join("\\", arr);
                    p += "\\";
                }
                if (p.Contains("release"))
                {
                    int index = Array.IndexOf(arr, "release");
                    Array.Resize(ref arr, index);
                    p = string.Join("\\", arr);
                    p += "\\";
                }
                return p;
            }
            else
            {
                string p = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (p.Contains("Release"))
                {
                    if (p.Contains("bin"))
                    {
                        p = p.Substring(0, p.Length - 11);
                    }
                }
                else
                {

                    string[] arr = p.Split(new char[] { '\\' });
                    int index = Array.IndexOf(arr, "bin");
                    if (p.Contains("bin"))
                    {
                        Array.Resize(ref arr, index);
                        p = string.Join("\\", arr);
                        p += "\\";
                    }
                }
                return p;
            }

        }


        public static string removeSpclChars(string str)
        {
            return Regex.Replace(str, @"[^a-zA-Z\.]", string.Empty);
        }


        public static void AddGroups(ref System.Windows.Controls.ComboBox cmb)
        {
            string[] Groups = clsGen.getGroups();

            foreach (string group in Groups)
            {
                cmb.Items.Add(group);
            }
        }

        /// <summary>
        /// Loads all scripts name from groupname external text file and returns string array.        
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public static string[,] getScriptsOfGroup(string groupname)
        {
            string p = clsGen.getScriptGroupPath() + groupname;//group to load from Text File

            //Read text file with all script names
            string[] lines = System.IO.File.ReadAllLines(p);
            string[,] scripts = new string[lines.Count(), 2];

            int cnt = 0;
            foreach (string line in lines)
            {
                string[] s = line.Split(',');   //0: scriptname   1:key name                
                scripts[cnt, 0] = s[0];
                scripts[cnt, 1] = s[1];
                cnt++;
            }
            return scripts;
        }

        public static void AddScriptsOfGroup(string groupname, ref System.Windows.Controls.ComboBox cmb)
        {
            string[,] s = getScriptsOfGroup(groupname);

            for (int i = 0; i < s.GetLength(0); i++)
            {
                cmb.SelectedValuePath = "Value";
                cmb.DisplayMemberPath = "Key";
                cmb.Items.Add(new KeyValuePair<string, string>(s[i, 0], s[i, 1]));
            }
        }





        public static void LoadPeriods(ref System.Windows.Controls.ComboBox cmb, bool blnLoadAll = false)
        {
            cmb.SelectedValuePath = "Value";
            cmb.DisplayMemberPath = "Key";
            int i = 0;
            if (blnLoadAll)
            {
                cmb.Items.Add(new KeyValuePair<string, int>("1", i++));
                cmb.Items.Add(new KeyValuePair<string, int>("3", i++));
                cmb.Items.Add(new KeyValuePair<string, int>("5", i++));
                cmb.Items.Add(new KeyValuePair<string, int>("10", i++));
                cmb.Items.Add(new KeyValuePair<string, int>("15", i++));
                cmb.Items.Add(new KeyValuePair<string, int>("30", i++));
            }

            i = 6;
            cmb.Items.Add(new KeyValuePair<string, int>("60", i++));
            cmb.Items.Add(new KeyValuePair<string, int>("D", i++));//0 or 7
            cmb.Items.Add(new KeyValuePair<string, int>("W", i++));
            cmb.Items.Add(new KeyValuePair<string, int>("M", i++));
            cmb.Items.Add(new KeyValuePair<string, int>("Q", i++));
            cmb.Items.Add(new KeyValuePair<string, int>("H", i++));
            cmb.Items.Add(new KeyValuePair<string, int>("Y", i++));
        }


        public static Dictionary<TimeFrame, string> getTimeFramedata(bool blnLoadAll = false)
        {
            Dictionary<TimeFrame, string> data = new Dictionary<TimeFrame, string>();
            int i = 0;

            if (blnLoadAll)
            {
                data.Add((TimeFrame)i++, "1 min");
                data.Add((TimeFrame)i++, "3 min");
                data.Add((TimeFrame)i++, "5 min");
                data.Add((TimeFrame)i++, "10 min");
                data.Add((TimeFrame)i++, "15 min");
                data.Add((TimeFrame)i++, "30 min");
            }

            i = 6;
            data.Add((TimeFrame)i++, "1 Hr");
            data.Add((TimeFrame)i++, "Daily");//0 or 7
            data.Add((TimeFrame)i++, "Weekly");
            data.Add((TimeFrame)i++, "Montly");
            data.Add((TimeFrame)i++, "Quaterly");
            data.Add((TimeFrame)i++, "Half-yearly");
            data.Add((TimeFrame)i++, "Yearly");

            return data;
        }

        public static DateTime UnixToDateTime(int exchangeTimeStamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 5, 30, 0, 0, DateTimeKind.Unspecified);
            dt = dt.AddSeconds(exchangeTimeStamp);
            return dt;

        }

        public static double FormatDecPrice(object X, int L = 0)
        {
            return Convert.ToDouble(String.Format("{0:0.00}", X));
        }

        public static double RoundOffPrice(double value)
        {
            value = Convert.ToDouble(String.Format("{0:0.000}", value));
            double dec = value - Math.Floor(value);

            double X = dec % 10;
            double Y = (int)dec / 10;

            if (X >= 0 && X <= 2)
                X = 0;
            else if (X >= 3 && X <= 7)
                X = 5;
            else if (X >= 8 && X <= 10)
                X = 0;


            return dec;

        }

        public static string formatdate(DateTime dt, bool withTime = false)
        {
            return dt.ToString(getDateFormats(withTime));
        }

        public static string formatdateInMonth(DateTime dt)
        {
            return dt.ToString("dd-MMM-yyyy");
        }

        /// <summary>
        /// Returns the string format for date
        /// </summary>
        /// <param name="blnWithTime">True: Date format Date with Time   False: Date Format only Date</param>
        /// <returns></returns>
        public static string getDateFormats(bool blnWithTime = false)
        {
            if (blnWithTime == true)
                return "dd-MM-yyyy HH:mm";
            else
                return "dd-MM-yyyy";

        }


        public static System.Windows.Media.Color getTLcolor(clsNiku.TRENDINGLEG TL)
        {
            Color[] tlcolr = getTLcolor();
            if ((int)TL == 99)
                return Colors.Silver;
            else
                return tlcolr[(int)TL];

        }
        //Getting Array of Color 
        public static Color[] getTLcolor()
        {
            System.Windows.Media.Color[] tlcolr = new System.Windows.Media.Color[10];
            tlcolr[0] = Color.FromRgb(0, 255, 64);
            tlcolr[1] = Color.FromRgb(0, 128, 64);
            tlcolr[2] = Color.FromRgb(0, 0, 255);
            tlcolr[3] = Color.FromRgb(0, 255, 64);
            tlcolr[4] = Color.FromRgb(0, 128, 64);
            tlcolr[5] = Color.FromRgb(255, 91, 160);
            tlcolr[6] = Color.FromRgb(232, 0, 5);
            tlcolr[7] = Color.FromRgb(128, 0, 0);
            tlcolr[8] = Color.FromRgb(255, 91, 160);
            tlcolr[9] = Color.FromRgb(232, 0, 5);

            return tlcolr;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static void saveGroup(ref ListBox lst, string group)
        {
            string groupPath = clsGen.getScriptGroupPath() + group;
            string scripts = "";

            int count = 0;
            foreach (ListBoxItem item in lst.Items)
            {
                scripts += item.Content.ToString() + "," + item.Tag.ToString() + "\n";
            }


            File.WriteAllText(groupPath, scripts);

        }

        /// <summary>
        /// convert DateTime to sql format
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateTimeToSqlDateTime(DateTime dt)
        {
            return " Cast('" + string.Format(dt.ToString("yyyy-MM-dd HH:mm:ss")) + "' as Datetime)";
        }

        /// <summary>
        /// convert DateTime to sql format
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateTimeToSqlDateOnly(DateTime dt)
        {
            return " Cast('" + string.Format(dt.ToString("yyyy-MM-dd")) + "' as Date)";
        }

        public static DateTime getLastThirdWorkingDays(DateTime start, int daysDif)
        {
            DateTime prev = start;

            start = start.AddDays(-1);
            int i = 1;
            while (i < daysDif)//shud b next saturday.. date subtract is used to see min 5 days diff.. to prevent nearby date catch Ex: old db is having fri.. nd it would catch next day's sat as next weekend
            {
                start = start.AddDays(-1);

                if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                i++;

            }
            return start;
        }

        public static DataTable CsvToDataTable(string csv)
        {
            string[] Lines = csv.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (Lines.Length <= 1)
                return new DataTable();



            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
            {
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            }
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

            Fields = Lines[1].Split(new char[] { ',' });
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                try
                {
                    if (Double.TryParse(Fields[i], out double val) == true)
                    {

                    }
                    else
                    {
                        DateTime date = DateTime.Parse(Fields[i]);

                        if (DateTime.TryParse(date.ToString("dd-MM-yyyy hh:mm:ss tt"), culture, DateTimeStyles.None, out DateTime Temp) == true)
                        {
                            dt.Columns[i].DataType = typeof(DateTime);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }


            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                {
                    try
                    {
                        Row[f] = Fields[f];
                        dt.Rows.Add(Row);
                    }
                    catch (Exception e) { }
                }
            }
            return dt;
        }


        public static string chkFolder(string folder)
        {
            string folderpath = Path.Combine(clsGen.getAppPath(), folder);

            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            return folderpath;

        }

        public static bool chkOrCreateFile(string file)
        {
            if (!File.Exists(file))
            {
                try
                {
                    File.Create(file).Close();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
            // getProfiles(profilefolder);
        }



        public static DateTime chkExchangeTime(DateTime startDate, string exchange, TimeFrame tf)
        {
            //-----------------------------
            //check exchange time limit or market updation
            //------------------------------

            if (exchange.Contains("NSE") || exchange.Contains("CDS"))
            {
                if (tf == TimeFrame.Daily)
                {
                    if (startDate >= new DateTime(startDate.Year, startDate.Month, startDate.Day, 15, 35, 0))
                    {
                        DateTime dt = new DateTime(startDate.Year, startDate.Month, startDate.Day, nseHourLimit, 30, 0);

                        return dt;
                    }
                    else
                    {
                        DateTime dt = startDate.AddDays(-1);
                        return dt;
                    }
                }
                else //if time interval below than day
                {
                    DateTime dt = setLastDate(startDate, tf, nseHourLimit);
                    return dt;

                }
            }
            else
            {
                if (tf == TimeFrame.Daily)
                {
                    if (startDate.Hour > 24)
                    {
                        return startDate.AddDays(1);
                    }
                    else
                    {
                        DateTime dt = startDate.AddDays(-1);
                        return dt;
                    }
                }
                else
                {
                    return setLastDate(startDate, tf, mcsHourLimit);
                }
            }
        }

        /// <summary>
        /// get the limit of the time according to the Limit
        /// </summary>
        /// <param name = "startDate" ></ param >
        /// < param name="interval"></param>
        /// <param name = "addTime" ></ param >
        /// < returns ></ returns >
        static DateTime setLastDate(DateTime lastdate, TimeFrame tf, int hourLimit)
        {
            switch (tf)
            {
                case TimeFrame.M60:
                    return new DateTime(lastdate.Year, lastdate.Month, lastdate.Day, hourLimit, 15, 0);

                case TimeFrame.M30:
                    return new DateTime(lastdate.Year, lastdate.Month, lastdate.Day, hourLimit, 30, 0);

                case TimeFrame.M15:
                    return new DateTime(lastdate.Year, lastdate.Month, lastdate.Day, hourLimit, 30, 0);

                case TimeFrame.M10:
                    return new DateTime(lastdate.Year, lastdate.Month, lastdate.Day, hourLimit, 25, 0);

                case TimeFrame.M5:
                    return new DateTime(lastdate.Year, lastdate.Month, lastdate.Day, hourLimit, 25, 0);

                case TimeFrame.M3:
                    return new DateTime(lastdate.Year, lastdate.Month, lastdate.Day, hourLimit, 27, 0);

                case TimeFrame.M1:
                    return new DateTime(lastdate.Year, lastdate.Month, lastdate.Day, hourLimit, 29, 0);

                default:
                    return lastdate;
            }
        }


        public static void getBlackOutDates(DatePicker dtPicker, TimeFrame tf)
        {
            if (dtPicker != null && tf >= TimeFrame.Daily)
            {
                //Disables Weekends
                DateTime minDate = dtPicker.DisplayDateStart ?? DateTime.MinValue;
                DateTime maxDate = DateTime.Today;
                //Daily
                if (tf == TimeFrame.Daily)
                {

                    // In the following for loop, I'm checking every date in given range and if the day is Saturday or Sunday I'm adding them to blackoutdates to remove them form the calender.
                    for (DateTime day = minDate; day <= maxDate; day = day.AddDays(1))
                    {
                        if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                        {
                            dtPicker.BlackoutDates.Add(new CalendarDateRange(day));
                        }
                    }

                }
                //Weekly
                if (tf == TimeFrame.Weekly)
                {
                    //enable only Friday
                    // In the following for loop, I'm checking every date in given range and if the day is Saturday or Sunday I'm adding them to blackoutdates to remove them form the calender.
                    for (DateTime day = minDate; day <= maxDate; day = day.AddDays(1))
                    {
                        if (day.DayOfWeek != DayOfWeek.Friday)
                        {
                            dtPicker.BlackoutDates.Add(new CalendarDateRange(day));
                        }
                    }
                }
                //Monthly
                if (tf == TimeFrame.Monthly)
                {
                    //enable only 1 Date of Month
                    // In the following for loop, I'm checking every date in given range and if the day is Saturday or Sunday I'm adding them to blackoutdates to remove them form the calender.
                    for (DateTime day = minDate; day <= maxDate; day = day.AddDays(1))
                    {
                        if (day.Day != 1)
                        {
                            dtPicker.BlackoutDates.Add(new CalendarDateRange(day));
                        }
                    }
                }


            }
        }


        /// <summary>
        /// concat db and table to use in query
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string getTable(string script, TimeFrame tf)
        {
            string[] tblpostfix = { "1", "3", "5", "10", "15", "30", "60", "" };//table names postfix array as per intervals
            string tbl = script + tblpostfix[(int)tf];
            return tbl;

        }

    }


}
