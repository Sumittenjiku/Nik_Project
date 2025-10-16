using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Windows.Media;
using Newtonsoft.Json;
using System.IO;
using Market.Models;
using Market.Classes;
using Market.Brokers;
using NiKu.Classes;
using System.Windows.Forms;
using WebSocketSharp;
using Market.Classes;
using Niku___Master_Code;

namespace Market
{
    public class clsNiku
    {


        public double[] PER = new double[12]; // RC FC channel percentage
        public static string[] dbPostFix = new string[] { "AB", "CD", "EF", "GH", "IJ", "KL", "MN", "OP", "QR", "ST", "UV", "WXYZ" };
        public static string[] tblpostfix = { "1", "3", "5", "10", "15", "30", "60", "" };//table names postfix array as per intervals
        public static string dbPreFix = "nik";
        public static TimeFrame dailyCheckCP = TimeFrame.M30;
        //public enum ChartPeriod 
        //{
        //    M1Chart = 0,
        //    M3Chart = 1,
        //    M5Chart = 2,
        //    M10Chart = 3,
        //    M15Chart = 4,
        //    M30Chart = 5,
        //    HourlyChart = 6,
        //    DailyChart = 7,
        //    WeeklyChart = 8,
        //    MonthlyChart = 9,
        //    QuaterlyChart = 10,
        //    HalfYearlyChart = 11,
        //    YearlyChart = 12

        //    //////DailyChart = 0,
        //    //////WeeklyChart = 1,
        //    //////MonthlyChart = 2,
        //    //////QuaterlyChart = 3,
        //    //////HalfYearlyChart = 4,
        //    //////YearlyChart = 5,
        //    //////HourlyChart = 6,
        //    //////M30Chart = 7,
        //    //////M15Chart = 8,
        //    //////M10Chart = 9,
        //    //////M5Chart = 10,
        //    //////M3Chart = 11,
        //    //////M1Chart = 12
        //}
        public enum TRENDINGLEG
        {
            TRB = 0,
            UTC = 1,
            cs = 2,
            CSCOUB = 3,
            CSCOUBC = 4,
            TRS = 5,
            DTC = 6,
            PBB = 7,
            PBBCODB = 8,
            PBBCODBC = 9
        }

        public class OHLCdp : clsOHLC
        {
            //below vals shud be generated live in software when Channels is showing in window
            public string Dpleg { get; set; }   //Director Pattern Leg
            public string Dp { get; set; }
            public string ValidBS { get; set; }//valid Buy/Sell
            public string PCG { get; set; }//PCG
           
        }

        public class OHLCdisp : clsOHLC
        {
            //----------------------------------------------------------
            // Added on 01-05-2025
            //----------------------------------------------------------
            public bool blnUpdate { get; set; }
            public int blnSerLocal { get; set; }
        }



        public class DPVals
        {
            //below vals shud be generated live ? ? ? ?
            public DateTime Date { get; set; }
            public string Dpleg { get; set; }   //Director Pattern Leg
            public string Dp { get; set; }
            public string ValidBS { get; set; }//valid Buy/Sell
            public string PCG { get; set; }//PCG
        }

        public clsNiku(string value = "")
        {  

            // Channels percentage
            PER[1] = 11.8;
            PER[2] = 23.6;
            PER[3] = 38.2;
            PER[4] = 61.8;
            PER[5] = 100;
            PER[6] = 127.2;
            PER[7] = 161.8;
            PER[8] = 261.8;
            PER[9] = 423.6;
            PER[10] = 685.4;
            PER[11] = 785.4;
        }

        public struct HistoryData
        {
            public OHLCdp current;
            public OHLCdp prev;
            public OHLCdp prev2;
        }

        public struct TLResult
        {
            public int TL;
            public int TL2;
            public int ValidBs;  //0: Black, 1: Green(Buy) 2: Red (Sell)
        }

        public struct TVHSTVLS
        {
            public double[] BDPhL;
            public double[] JWDhL;
            public double[] WDPhL;
            public double[] JGDhL;
            public double[] HighHL;
            public double[] LowHL;
        }


        public struct DirectorPattern
        {
            public double JGD;
            public double JWD;
            public double BDP;
            public double WDP;
            public string L1;
            public string L2;
            public string PatternName;
            public double NewBDP;
            public double NewWDP;
        }


        ///// <summary>
        ///// Returns history data of Script for given Date, previous date and previous to previous date
        ///// </summary>
        ///// <param name="Script"></param>
        ///// <param name="date"></param>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //public HistoryData getHistoryData(string Script, DateTime date, ChartPeriod p)
        //{
        //    HistoryData hd=new HistoryData();

        //    //string qr = getScriptQS(p, Script);//query string as per chart  period selected for that script ************MYSQL
        //    //qr += " where ddate<= STR_TO_DATE('" + date.Day + "," + date.Month + "," + date.Year + "'" + ",'%d,%m,%Y') order by ddate desc limit 3";

        //    string qr = clsNiku.getScriptQS(p, Script, 3,"*");//query string as per chart  period selected for that script
        //    qr += " where ddate<= "+clsGen.DateTimeToSqlDateOnly(date)+" order by ddate desc";

        //    //get history data
        //    clsSql cls = new clsSql();
        //    cls.connect();
        //    DataTable dt = cls.getDataTable(qr);
        //    if (dt.Rows.Count == 0)
        //    {
        //      return hd;//return blank
        //    }

        //    setOHLCvals(ref hd.current, dt,0);
        //    setOHLCvals(ref hd.prev, dt,1);
        //    setOHLCvals(ref hd.prev2, dt,2);

        //    //h.Current.Open =Convert.ToDouble( dr["open"].ToString());
        //    //MySqlDataReader dr = new MySqlDataReader();            

        //    cls.disconnect();

        //    return hd;
        //}


        ///// <summary>
        ///// Gives previous EOD data values as per period selected
        ///// </summary>
        ///// <param name="script"></param>
        ///// <param name="date"></param>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //public OHLC getPrevEODdata(string script, DateTime date, ChartPeriod p)
        //{
        //    string qr = getScriptQS(p, script, 1, "*");//query string as per chart  period selected for that script

        //    //string qr = getScriptQS(p, script);//query string as per chart  period selected for that script
        //    //qr += " where ddate< STR_TO_DATE('" + date.Day + "," + date.Month + "," + date.Year + "'" + ",'%d,%m,%Y') order by ddate desc limit 1"; ********MYSQL

        //    qr += " where ddate< " + clsGen.DateTimeToSqlDateOnly(date) + " order by ddate desc ";

        //    OHLC ohlc = new OHLC();
        //    clsSql cls = new clsSql();
        //    cls.connect();
        //    DataTable dt = cls.getDataTable(qr);
        //    cls.disconnect();
        //    if (dt.Rows.Count == 0)
        //    {
        //        Debug.Print(date.ToShortDateString() + " Previous EOD data not available");
        //    }
        //    else
        //    {
        //        setOHLCvals(ref ohlc, dt, 0);
        //    }
        //    return ohlc;
        //}


        /// <summary>
        /// Returns history data of Script for given Date, previous date and previous to previous date
        /// </summary>
        /// <param name="Script"></param>
        /// <param name="date"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public HistoryData getHistoryData(string Script, DateTime date, TimeFrame p)
        {
            HistoryData hd = new HistoryData();
            List<clsNiku.OHLCdp> ohlchistory = getAllOHLC(Script, p);

            List<clsNiku.OHLCdp> ohlc = ohlchistory.Where(x => x.Date <= date).Take(3).ToList();
            if (ohlc.Count > 0)
            {
                if (ohlc.Count >= 0)
                    hd.prev2 = ohlc[0];
                if (ohlc.Count >= 1)
                    hd.prev = ohlc[1];
                if (ohlc.Count >= 2)
                    hd.current = ohlc[2];

                return hd;
            }
            return hd;
        }


        /// <summary>
        /// Gives next EOD data values as per period selected
        /// </summary>
        /// <param name="script"></param>
        /// <param name="date"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public OHLCdp getNextEODdata(string script, DateTime date, TimeFrame p)
        {
            List<clsNiku.OHLCdp> ohlchistory = getAllOHLC(script, p);
            clsNiku.OHLCdp ohlc = new OHLCdp();
            if (p>=TimeFrame.Daily && date.Hour<9) ohlc = ohlchistory.FirstOrDefault(x => x.Date > date.AddHours(9).AddMinutes(15));
            else ohlc = ohlchistory.FirstOrDefault(x => x.Date > date);

            if (ohlc == null)
            {
                Debug.Print(date.ToShortDateString() + " Next EOD data not available");
            }
            return ohlc;
        }

        /// <summary>
        /// Gives previous EOD data values as per period selected
        /// </summary>
        /// <param name="script"></param>
        /// <param name="date"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public OHLCdp getPrevEODdata(string script, DateTime date, TimeFrame p)
        {
            List<clsNiku.OHLCdp> ohlchistory = getAllOHLC(script, p);
            clsNiku.OHLCdp ohlc = ohlchistory.LastOrDefault(x => x.Date < date);

            if (ohlc == null)
            {
                Debug.Print(date.ToShortDateString() + " Next EOD data not available");
            }
            return ohlc;
        }

        public struct channel
        {
            public IList<double> val;
            public IList<double> tvhs;
            public IList<double> tvls;
        }

        public channel[] getChannels(HistoryData hd, TimeFrame p)
        {
            channel[] ret = new channel[2];
            try
            {
                //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
                //to create channels for today.. we need to use previous day's data
                //Let Today's market not open yet.. so for today's applicable channel we have to generate it based on prev data
                //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
                double pclose = hd.prev.Close;
                double range = hd.prev.High - hd.prev.Low;            // prev high-prev low                        

                double compl = pclose * (0.073 / 100); //complement            

                double v, v2;
                channel rc = new channel(), fc = new channel();
                rc.val = new List<double>();
                rc.tvhs = new List<double>();
                rc.tvls = new List<double>();
                fc.val = new List<double>();
                fc.tvhs = new List<double>();
                fc.tvls = new List<double>();

                for (int i = 1; i <= 10; i++) // required till 8.. but 10 is required in sm calculations to chk with nxt level. so 10th is hidden in display
                {
                    // Rising Channel
                    v = getRC(i, range, pclose);
                    rc.val.Add(clsGen.FormatDecPrice(v, 2));

                    double complval = (p == TimeFrame.Daily) ? 0 : compl;
                    // TVHS TVLS
                    if (i == 2)
                    {
                        v2 = v + compl; // tvhs
                        rc.tvhs.Add(clsGen.FormatDecPrice(v2, 2));

                        v2 = v - compl; // tvls
                        rc.tvls.Add(clsGen.FormatDecPrice(v2, 2));
                    }
                    else
                    {
                        // RC TVHS - high
                        v2 = ((getRC(i + 1, range, pclose) - v) * (38.2 / 100.0)) + v + complval;
                        rc.tvhs.Add(clsGen.FormatDecPrice(v2, 2));

                        // RC TVLS - LOW
                        if (i == 1)
                            v2 = v - ((v - getFC(i, range, pclose)) * (38.2 / 100.0)) - complval;
                        else
                            v2 = v - ((v - getRC(i - 1, range, pclose)) * (38.2 / 100.0)) - complval;

                        rc.tvls.Add(clsGen.FormatDecPrice(v2, 2));

                    }
                    // ------------------------------------------------------------
                    // Falling Channel
                    v = getFC(i, range, pclose);
                    fc.val.Add(clsGen.FormatDecPrice(v, 2));
                    // TVHS TVLS
                    if (i == 2)
                    {
                        v2 = v + compl;
                        fc.tvhs.Add(clsGen.FormatDecPrice(v2, 2));
                        v2 = v - compl;
                        fc.tvls.Add(clsGen.FormatDecPrice(v2, 2));
                    }
                    else
                    {
                        // FC TVHS - high
                        if (i == 1)
                            v2 = ((getRC(i, range, pclose) - v) * (38.2 / 100.0)) + v + complval;
                        else
                            v2 = ((getFC(i - 1, range, pclose) - v) * (38.2 / 100.0)) + v + complval;
                        fc.tvhs.Add(clsGen.FormatDecPrice(v2, 2));

                        // FC TVLS - LOW
                        v2 = v - ((v - getFC(i + 1, range, pclose)) * (38.2 / 100.0)) - complval;
                        fc.tvls.Add(clsGen.FormatDecPrice(v2, 2));
                    }
                } //---- end for  

                //return channels -----------

                ret[0] = rc;
                ret[1] = fc;
                return ret;
            }
            catch (Exception e)
            {
                return ret;
            }
            
        }

        public DirectorPattern getDirectorPattern(HistoryData hd)
        {
            // JGD: Junior Good Director      JWD: Junior Weak Director
            double Range, JGD, JWD, Rangeprev, JGDprev, JWDprev;
            string P;

            #region old setting taking current day's DP.. but will be applicable for next day
            //// cur jgd & jwd ---------------------
            //Range = hd.current.High - hd.current.Low;
            //JGD = hd.current.High - (Range * 38.2 / 100.0);
            //JWD = hd.current.Low + (Range * 38.2 / 100.0);

            //// Prev day jgd & jwd-----------------
            //Rangeprev = hd.prev.High - hd.prev.Low;
            //JGDprev = hd.prev.High - (Rangeprev * 38.2 / 100.0);
            //JWDprev = hd.prev.Low + (Rangeprev * 38.2 / 100.0);
            #endregion

            
            //Try catch method to existing method for error handling
            DirectorPattern dpNull = new DirectorPattern();
            if (hd.prev == null) return dpNull;
            try
            {

                
            // cur jgd & jwd --------------------- 
            Range = hd.prev.High - hd.prev.Low;
            JGD = hd.prev.High - (Range * 38.2 / 100.0);
            JWD = hd.prev.Low + (Range * 38.2 / 100.0);

            // Prev day jgd & jwd-----------------
            Rangeprev = hd.prev2.High - hd.prev2.Low;
            JGDprev = hd.prev2.High - (Rangeprev * 38.2 / 100.0);
            JWDprev = hd.prev2.Low + (Rangeprev * 38.2 / 100.0);

            string L1 = "", L2 = "";//line 1 and line 2 to display

            // Pattern condition and arrangement ---------------------------
            string strSpace = "    ";
            double valBDP = 0, valWDP = 0, valJGD = 0, valJWD = 0; // final answer elements
            double valNewBDP, valNewWDP;
            string DPpatternName = "";

            if (JGD > JWDprev & JWD > JWDprev)
            {
                // First LINE -----------'top right is BDP
                if (JGDprev < JGD)
                {
                    L1 = clsGen.FormatDecPrice(JGDprev) + strSpace + clsGen.FormatDecPrice(JGD);
                    valBDP = clsGen.FormatDecPrice(JGD);
                }
                else
                {
                    L1 = clsGen.FormatDecPrice(JGD) + strSpace + clsGen.FormatDecPrice(JGDprev);
                    valBDP = clsGen.FormatDecPrice(JGDprev);
                }

                // Second LINE---------------------------bottom left: wdp   bottom right: jwd
                if (JWD < JWDprev)
                {
                    L2 = clsGen.FormatDecPrice(JWD) + strSpace + clsGen.FormatDecPrice(JWDprev);
                    valWDP = clsGen.FormatDecPrice(JWD);
                    valJWD = clsGen.FormatDecPrice(JWDprev);
                }
                else
                {
                    L2 = clsGen.FormatDecPrice(JWDprev) + strSpace + clsGen.FormatDecPrice(JWD);
                    valWDP = clsGen.FormatDecPrice(JWDprev);
                    valJWD = clsGen.FormatDecPrice(JWD);
                }
                DPpatternName = "22";
            }
            else if (JGD < JWDprev & JWD < JWDprev)
            {
                // First LINE -----------left:jgd   right:bdp
                double[] arVal = new double[3];
                arVal[0] = JGD;
                arVal[1] = JGDprev;
                arVal[2] = JWDprev;
                Array.Sort(arVal); // ascending order
                L1 = clsGen.FormatDecPrice(arVal[0]) + strSpace + clsGen.FormatDecPrice(arVal[1]) + strSpace + clsGen.FormatDecPrice(arVal[2]);
                valJGD = clsGen.FormatDecPrice(arVal[0]);
                valBDP = clsGen.FormatDecPrice(arVal[2]);

                // Second LINE---------------------------wdp
                L2 = clsGen.FormatDecPrice(JWD).ToString();
                valWDP = clsGen.FormatDecPrice(JWD);
                DPpatternName = "31";
            }
            else if (JGD > JWDprev & JWD < JWDprev)
            {
                // First LINE -----------left: jgd     right: bdp
                if (JGD < JGDprev)
                {
                    L1 = clsGen.FormatDecPrice(JGD) + strSpace + clsGen.FormatDecPrice(JGDprev);
                    valJGD = clsGen.FormatDecPrice(JGD);
                    valBDP = clsGen.FormatDecPrice(JGDprev);
                }
                else
                {
                    L1 = clsGen.FormatDecPrice(JGDprev) + strSpace + clsGen.FormatDecPrice(JGD);
                    valJGD = clsGen.FormatDecPrice(JGDprev);
                    valBDP = clsGen.FormatDecPrice(JGD);
                }

                // Second LINE---------------------------wdp
                L2 = clsGen.FormatDecPrice(JWD).ToString();
                valWDP = clsGen.FormatDecPrice(JWD);

                DPpatternName = "21";
            }

            valNewBDP = valBDP + (Range * 14.6 / 100.0);
            valNewWDP = valWDP - (Range * 14.6 / 100.0);

            P = L1 + Environment.NewLine + L2; // final pattern            

            DirectorPattern dp = new DirectorPattern();
            // 0:JGD    1:JWD    2:BDP    3:WDP   4:Pattern Line1     5: Pattern Line2       6: DPpatternName     7:new bdp     8:new wdp
            dp.JGD = valJGD;
            dp.JWD = valJWD;
            dp.BDP = valBDP;
            dp.WDP = valWDP;
            dp.L1 = L1;
            dp.L2 = L2;
            dp.PatternName = DPpatternName;
            dp.NewBDP = valNewBDP;
            dp.NewWDP = valNewWDP;

                return dp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DirectorPattern dp = new DirectorPattern();
                return dp;
            }


            
        }

        public TRENDINGLEG TL, TL2, PrevTL;
        double newBDP, newWDP;
        Color PrevValidBS, Colr;
        double LowestLow = 0, HighestHigh = 0;


        /// <summary>
        /// returns Valid BS and TL 
        /// 0: TL, 1: Valid BS color
        /// </summary>
        /// <param name="hd"></param>
        /// <param name="script"></param>
        /// <param name="dp"></param>
        /// <param name="FRCFC"></param>
        /// <param name="cp"></param>
        /// <param name="highestHigh"></param>
        /// <param name="lowestLow"></param>
        /// <param name="blnFreezeHL"></param>
        /// <returns></returns>
        public TLResult getTrendingLEG(HistoryData hd, TLResult prevResult, string script, DirectorPattern dp, channel[] FRCFC, TimeFrame cp, clsOHLC ohlc, double highestHigh, double lowestLow, ref bool blnFreezeHL)
        {
            //Calculate Director Pattern
            TVHSTVLS tvhstvls = new TVHSTVLS();
            if (lowestLow != 0 || highestHigh != 0)
            {
                LowestLow = lowestLow;
                HighestHigh = highestHigh;
            }

            //clsNiku.OHLCdp ohlc = hd.current;
            clsNiku.OHLCdp ohlcprev = hd.prev;

            DateTime date = ohlc.Date; // Current date
            DateTime DTprev = ohlcprev.Date; //Previous date as per time period selected

            //DP vals from channel ------------------------------------------------------------------------------------------------
            double[] BDPhL, WDPhL, JGDhL, JWDhL, HighHL, LowHL;// '0: TVHS     1:TVLS    2: Next Level TVHS    3:Next Level TVLS            
            tvhstvls.BDPhL = getTvHsTvLs(dp.BDP, FRCFC);
            tvhstvls.WDPhL = getTvHsTvLs(dp.WDP, FRCFC);
            tvhstvls.JGDhL = getTvHsTvLs(dp.JGD, FRCFC);
            tvhstvls.JWDhL = getTvHsTvLs(dp.JWD, FRCFC);

            string TLname = dp.PatternName;  //FRCFC.lblDP.Tag;

            double H, L, C; //  High, Low, Close
            H = ohlc.High;
            L = ohlc.Low;
            C = ohlc.Close;

            // If H > HighestHigh Then HighestHigh = H
            // If L < LowestLow Then LowestLow = L
            // HighHL = fRCFC.getTvHsTvLs(Val(HighestHigh)) '0: TVHS     1:TVLS
            // LowHL = fRCFC.getTvHsTvLs(Val(LowestLow))

            PrevTL = (TRENDINGLEG) prevResult.TL;
            TL = PrevTL;
            //PrevTL = (TRENDINGLEG)Convert.ToInt16(hd.prev.Dpleg);
            //TL = PrevTL;
            //PrevValidBS = ValidBsColorFromStr(ohlcprev.ValidBS);
            PrevValidBS = ValidBsColorFromStr(prevResult.ValidBs);

            // ------------------------------------------------------------------
            bool blnInnerCheck;
            blnInnerCheck = true;

            string S = "";

            tvhstvls.HighHL = getTvHsTvLs(HighestHigh, FRCFC);   // 0: TVHS     1:TVLS    2: Next Level TVHS    3:Next Level TVLS
            tvhstvls.LowHL = getTvHsTvLs(LowestLow, FRCFC);

            #region  R : Not required as we are already getting previous period data in History Data
            //if (blnInnerCheck)
            //{    
            //ADODB.Recordset RS;
            //RS = new ADODB.Recordset();
            //RS.Open;
            //S;
            //cnNSE;
            //adOpenDynamic;
            //adLockOptimistic;
            // reach to the prev date of Period, to start the search process in inner periods Eg: In case of week, get prev week.. then next loop below will chk in days
            //* * * * * * * 
            //while (!RS.EOF)
            //{
            //    if (DateTime.Parse(RS))
            //    {
            //        dDate;
            //        DTprev;
            //        break; //Warning!!! Review that break works as 'Exit Do' as it could be in a nested instruction like switch
            //        if (DateTime.Parse(RS))
            //        {
            //            dDate;
            //            DTprev;
            //            // fri -sat case where 1 day in advance could come
            //            RS.MovePrevious;
            //            if (DateDiff("d", DTprev, DateTime.Parse(RS), dDate))
            //            {
            //                -1;
            //                break; //Warning!!! Review that break works as 'Exit Do' as it could be in a nested instruction like switch
            //            }

            //            RS.MoveNext;
            //        }
            //}

            // If blnFreezeHL Then
            //     'OUTER periodic high low checks
            //     If H > HighestHigh Then HighestHigh = H
            //     If L < LowestLow Then LowestLow = L
            // End If            
            //* * * * *     
            #endregion

            TLResult tl = checkTL(hd, dp, ohlc, tvhstvls, ref blnFreezeHL);
            return tl;
            ////Loop for inner EOD check finding multi legs change ----------------------------------
            //for (int i = 0; i < ohlcdata.Count; i++)    //< < < need to chk for weekly and upper TF to use 1 or 0
            //{
            //}
            //getTrendingLEG = t;
        }

        public TLResult checkTL(HistoryData hd, DirectorPattern dp, clsOHLC dr, TVHSTVLS tvhstvls, ref bool blnFreezeHL)
        {
            string S = "";

            //if (blnInnerCheck) // W M H Y chk using daily eod
            //{
            //RS.MoveNext;
            double H = dr.High;
            double L = dr.Low;
            double C = dr.Close;
            //}

            //     If TL = TRB Or TL = TRS Or TL = CSCOUB Or TL = PBBCODB Then 'Correction added : for Axis 4-6-19 eod
            //         'update highest and lowest vals
            //         If H > HighestHigh Then HighestHigh = H
            //         If L < LowestLow Then LowestLow = L
            //     End If



          
            if (((TL != TRENDINGLEG.TRB) && (TL != TRENDINGLEG.UTC) && (TL != TRENDINGLEG.cs) && (TL != TRENDINGLEG.CSCOUB) && (TL != TRENDINGLEG.CSCOUBC))
                        && (H > tvhstvls.BDPhL[0]))
            {
                TL = TRENDINGLEG.TRB;
                TL2 = TL;
                HighestHigh = H;    // start counting hightest high frm TRB date
                blnFreezeHL = false;
            }

           
            else if ((TL == TRENDINGLEG.TRB) && (H > tvhstvls.HighHL[0]))
            {
                TL = TRENDINGLEG.UTC;
                TL2 = TL;
            }

            // CSCOUB:
            else if ((TL == TRENDINGLEG.cs) && (H > tvhstvls.BDPhL[0]))
            {
                TL = TRENDINGLEG.CSCOUB;
                TL2 = TL;
                HighestHigh = H;    // reset highest high
                blnFreezeHL = false;
            }

            //CSCOUBC:
            else if ((TL == TRENDINGLEG.CSCOUB) && (H > tvhstvls.HighHL[0]))
            {
                TL = TRENDINGLEG.CSCOUBC;
                TL2 = TL;
                // HighestHigh = 9999999999# 'to avoid high check
            }

          
            if (((TL == TRENDINGLEG.TRB) || (TL == TRENDINGLEG.UTC) || (TL == TRENDINGLEG.CSCOUB) || (TL == TRENDINGLEG.CSCOUBC))
                        && (tvhstvls.JWDhL[1] != 0) && (L < tvhstvls.JWDhL[1]) && (L > tvhstvls.WDPhL[1]) && (dp.PatternName == "22"))
            {

                TL2 = TRENDINGLEG.cs; //for chking TL


                //hourly unavailable.. chk with EOD data for TL change.. nearest to close value
                if (((H > tvhstvls.HighHL[0]) || (H > tvhstvls.BDPhL[0])) && (tvhstvls.JWDhL[1] != 0) && (L < tvhstvls.JWDhL[1]))
                {
                    double Diff, Diff1, Diff2;
                    Diff = Math.Abs((C - H));           // diff with high
                    Diff1 = Math.Abs((C - tvhstvls.BDPhL[0]));   // with BDP TVHS
                    Diff2 = Math.Abs((C - tvhstvls.JWDhL[1]));   // with JWD tvls
                    if ((Diff < Diff2) || (Diff1 < Diff2))
                    {
                        // dnt chng TL
                    }

                    if ((Diff2 < Diff) || (Diff2 < Diff1))
                    {
                        TL = TRENDINGLEG.cs;
                        // : LowestLow = L
                    }
                }
                else
                {
                    TL = TRENDINGLEG.cs;
                }

            }

       
            if ((TL != TRENDINGLEG.TRS) && (TL != TRENDINGLEG.DTC) && (TL != TRENDINGLEG.PBB) && (TL != TRENDINGLEG.PBBCODB) && (TL != TRENDINGLEG.PBBCODBC)
                        && (L < tvhstvls.WDPhL[1]))
            {
                TL = TRENDINGLEG.TRS;
                TL2 = TL;
                LowestLow = L;  // start counting lowest low
                blnFreezeHL = false;
            }

            
            else if ((TL == TRENDINGLEG.TRS) && (L < tvhstvls.LowHL[1]))
            {
                TL = TRENDINGLEG.DTC;
                TL2 = TL;
            }

            
            else if ((TL == TRENDINGLEG.PBB) && (L < tvhstvls.WDPhL[1]))
            {
                TL = TRENDINGLEG.PBBCODB;
                TL2 = TL;
                LowestLow = L;
                // start counting lowest low
                blnFreezeHL = false;
            }

            
            else if ((TL == TRENDINGLEG.PBBCODB) && (L < tvhstvls.LowHL[1]))
            {
                TL = TRENDINGLEG.PBBCODBC;
                TL2 = TL;
            }

           
            if (((TL == TRENDINGLEG.TRS) || (TL == TRENDINGLEG.DTC) || (TL == TRENDINGLEG.PBBCODB) || (TL == TRENDINGLEG.PBBCODBC))
                        && (tvhstvls.JGDhL[0] != 0) && (H > tvhstvls.JGDhL[0]) && (H < tvhstvls.BDPhL[0]) && (dp.PatternName == "31"))
            {
                TL2 = TRENDINGLEG.PBB;

                
                if (((L < tvhstvls.LowHL[1]) || (L < tvhstvls.WDPhL[1])) && (tvhstvls.JGDhL[0] != 0) && (H > tvhstvls.JGDhL[0]))
                {
                    double Diff, Diff1, Diff2;

                    Diff = Math.Abs((C - L));           // diff with low
                    Diff1 = Math.Abs((C - tvhstvls.WDPhL[1]));   // with wdp tvls
                    Diff2 = Math.Abs((C - tvhstvls.JGDhL[0]));   // with jgd tvhs
                    if ((Diff < Diff2) || (Diff1 < Diff2))
                    {
                        // dnt chng TL
                    }
                    if ((Diff2 < Diff) || (Diff2 < Diff1))
                    {
                        TL = TRENDINGLEG.PBB;
                        // : HighestHigh = H
                    }
                }
                else
                {
                    TL = TRENDINGLEG.PBB;
                }

            }

            // -++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // chk which is near to Close applies
            // If (H > BDPhL(0) Or H >= HighestHigh) And (l < WDPhL(1) Or l <= LowestLow) Then
            if ((H > tvhstvls.BDPhL[0]) && (L < tvhstvls.WDPhL[1]) && (TL != TRENDINGLEG.UTC) && (TL != TRENDINGLEG.DTC))
            {
                double Diff, Diff1, Diff2;
                Diff = Math.Abs((C - tvhstvls.BDPhL[0]));
                Diff2 = Math.Abs((C - tvhstvls.WDPhL[1]));

                if ((Diff < Diff2))
                {
                    TL = TRENDINGLEG.TRB;
                    HighestHigh = H;
                }

                if ((Diff2 < Diff))
                {
                    TL = TRENDINGLEG.TRS;
                    LowestLow = L;
                }

                TL2 = TL;
            }

            //if ((blnInnerCheck == false))
            //{
            //    break;
            //}

            //if (cp > TimeFrame.Daily)
            //{
            //    if (Convert.ToDateTime(dr.Date) >= date) // reached date
            //        break;
            //}

            if ((blnFreezeHL == false))
            {
                // Internal EOD high low chks
                if (H > HighestHigh) HighestHigh = H;
                if (L < LowestLow) LowestLow = L;

            }

            //--------------------------------------------------------------------------------------------
            // When selected Period is Over nd Leg doesnt change.. Freeze High/Low values for tht period
            //--------------------------------------------------------------------------------------------
            if ((TL == TRENDINGLEG.TRB) || (TL == TRENDINGLEG.TRS) || (TL == TRENDINGLEG.CSCOUB) || (TL == TRENDINGLEG.PBBCODB))
                blnFreezeHL = true;
            else
                blnFreezeHL = false;


            // *******************************************************************************************
            // VALID BUY / SELL
            // *******************************************************************************************
            S = "";
            Colr = PrevValidBS; // continuing prev val
            int validBs = 0;

            // on leg change reset ---------
            if (PrevTL != TL)
            {
                S = "INVALID (leg chng)";
                validBs = 0;
                Colr = Colors.Black;
            }

            H = hd.current.High;// double.Parse(FRCFC.txtHigh.Text);
            L = hd.current.Low; //double.Parse(FRCFC.txtLow.Text);
            if ((TL == TRENDINGLEG.CSCOUBC) || (TL == TRENDINGLEG.UTC))
            {
                validBs = 1;
                S = "VALID BUY";
                Colr = Colors.Green;//vbGreen;
            }

            else if (((TL == TRENDINGLEG.TRB) || (TL == TRENDINGLEG.CSCOUB)) && (Colr != Colors.Green))
            {
                if (H > tvhstvls.BDPhL[2]) // nxt level of bdp traded
                {
                    S = "VALID BDP";
                    validBs = 1;
                    Colr = Colors.Green;
                }
                else
                {
                    S = "INVALID BDP";
                    validBs = 0;
                    Colr = Colors.Black;
                }

            }
            else if (TL == TRENDINGLEG.PBB)
            {
                if ((H > tvhstvls.JGDhL[2]) && (tvhstvls.JGDhL[2] != 0))    //  upper band mae trade hoga
                {
                    S = "VALID PBB";
                    validBs = 1;
                    Colr = Colors.Green;
                }
                else
                {
                    // *added
                    // invalid
                    S = "INVALID PBB";
                    validBs = 0;
                    Colr = Colors.Black;
                }
            }
            // -------------- Selling Leg -------------------------
            else if ((TL == TRENDINGLEG.PBBCODBC) || (TL == TRENDINGLEG.DTC))
            {
                S = "VALID SELL";
                validBs = 2;
                Colr = Colors.Red;
            }
            else if (TL == TRENDINGLEG.cs)
            {
                if ((L < tvhstvls.JWDhL[3]) && (tvhstvls.JWDhL[3] != 0)) // lower band mae trade hoga...
                {
                    validBs = 2;
                    S = "VALID CS";
                    Colr = Colors.Red;
                }
                else
                {
                    // *added
                    // invalid
                    S = "INVALID CS";
                    validBs = 0;
                    Colr = Colors.Black;
                }

            }
            else if ((TL == TRENDINGLEG.TRS) || (TL == TRENDINGLEG.PBBCODB))
            {
                if (L < tvhstvls.WDPhL[3])
                {
                    S = "VALID WDP";
                    validBs = 2;
                    Colr = Colors.Red;
                    // Else '*added
                    //     'invalid
                    //     S = "INVALID WDP"
                    //     Colr = vbBlack
                }

            }

            PrevValidBS = Colr;// storing valid B/S color for checking in next cycle

            TLResult tlresult = new  TLResult();
            tlresult.TL = (int)TL;
            tlresult.TL2 = (int)TL2;
            tlresult.ValidBs = validBs;
            object[] t = new object[3];
            //t[0] = (int)TL;
            //t[1] = validBs; // valid buy/sell colr
            //t[2] = (int)TL2;
            return tlresult;
        }


       

        public List<clsNiku.OHLCdp> getDatesInPeriod(TimeFrame chartP, string Script, DateTime date, DateTime datePrev, bool blnReadDaily = false, bool blnLiveCheck = false)
        {
            //--------------------------------
            //using history data from OHLC data
            //to Calculate DP, DPLEg, validbs
            //--------------------------------

            // select * from `MARUTIw` where ddate< STR_TO_DATE('29,11,2019','%d,%m,%Y') order by ddate desc limit 1;
            // select * from `MARUTI` where ddate between STR_TO_DATE('23,11,2019','%d,%m,%Y')  and STR_TO_DATE('29,11,2019','%d,%m,%Y') ;
            // select * from `MARUTI` where ddate> STR_TO_DATE('25,11,2019','%d,%m,%Y') order by ddate limit 1;
            // select * from `MARUTI60` where ddate between STR_TO_DATE('25,11,2019','%d,%m,%Y')  and STR_TO_DATE('26,11,2019','%d,%m,%Y') ;
            // Now list Daily dates in this period.. ----------------------------------------------
            string[] X;
            List<clsNiku.OHLCdp> ohlc = new List<clsNiku.OHLCdp>();
            // 0:Date   1: O   2:H    3:L    4:C    5:Vol    csv format
            string S;
            string Tbl;
            int CNT = 0;
            DataTable dt;
            List<OHLCdp> ohlcHistory = new List<OHLCdp>();

            if (chartP <= TimeFrame.Daily)
            {
                //hourly data for daily timeframe innercheck 
                ohlcHistory = getAllOHLC(Script, TimeFrame.M5, false);
                ohlc = ohlcHistory.Where(x => x.Date.Date == date.Date).ToList();

                if (ohlc.Count <= 0)
                {
                    ohlcHistory = getAllOHLC(Script, TimeFrame.M30, false);
                    ohlc = ohlcHistory.Where(x => x.Date.Date == date.Date).ToList();

                    if (ohlc.Count <= 0)
                    {
                        ohlcHistory = getAllOHLC(Script, TimeFrame.M60, false);
                        ohlc = ohlcHistory.Where(x => x.Date.Date == date.Date).ToList();

                        //hourly data unavailable...... return daily data
                        if (blnReadDaily)
                        {
                            //use history data from OHLC data
                            //to Calculate DP, DPLEg, validbs
                            ohlcHistory = getAllOHLC(Script, TimeFrame.Daily, false);
                            ohlc = ohlcHistory.Where(x => x.Date > datePrev && x.Date <= date).ToList();
                        }
                        else
                            goto norecord;
                    }
                }
            

                //// use from Hourly chart
                //Tbl = getTableName(ChartPeriod.HourlyChart, Script);
                //// check for availability of 60 min chart data
                //ADODB.Recordset RS;
                //RS = new ADODB.Recordset();
                //RS.Open;
                //("select * from "
                //            + (Tbl + ""));
                //cnNSE;
                //adOpenDynamic;
                //adLockOptimistic;
                //if (RS.EOF)
                //{
                //    goto norecord;
                //}
            }
            else
            {
                if (ohlc.Count <= 0)
                {
                    ohlcHistory = getAllOHLC(Script, TimeFrame.M30, false);
                    ohlc = ohlcHistory.Where(x => x.Date.Date == date.Date).ToList();

                    if (ohlc.Count <= 0)
                    {
                        ohlcHistory = getAllOHLC(Script, TimeFrame.M60, false);
                        ohlc = ohlcHistory.Where(x => x.Date.Date == date.Date).ToList();
                        if (ohlc.Count <= 0)
                        {
                            //EOD data for weekly and above tf inner check
                            ohlcHistory = getAllOHLC(Script, TimeFrame.Daily);
                            ohlc = ohlcHistory.Where(x => x.Date >= datePrev && x.Date < date).ToList();

                            if (ohlc.Count <= 0)
                            {
                                goto norecord;
                            }
                        }
                    }
                }
                //if (blnMDB)
                //{
                //    S = ("select * from `"
                //                + (Script + ("` where ddate between (select top 1 ddate from `"
                //                + (Tbl + ("` where ddate< format(#"
                //                + (DT.Year + ("/"
                //                + (DT.Month + ("/"
                //                + (DT.Day + ("#" + (", \'yyyy/mm/dd\') order by ddate desc,id)  and format(#"
                //                + (DT.Year + ("/"
                //                + (DT.Month + ("/"
                //                + (DT.Day + ("#" + ", \'yyyy/mm/dd\') ;"))))))))))))))))));
                //}
                //else
                //{
                //    S = ("select * from `"
                //                + (Script + ("` where ddate between (select ddate from `"
                //                + (Tbl + ("` where ddate< STR_TO_DATE(\'"
                //                + (DT.Day + (","
                //                + (DT.Month + (","
                //                + (DT.Year + ("\'" + (",\'%d,%m,%Y\') order by ddate desc limit 1)  and STR_TO_DATE(\'"
                //                + (DT.Day + (","
                //                + (DT.Month + (","
                //                + (DT.Year + ("\'" + ",\'%d,%m,%Y\') ;"))))))))))))))))));
                //}

            }

            //ADODB.Recordset RS2;
            //int CNT;
            //RS2 = new ADODB.Recordset();
            //RS2.Open;
            //S;
            //cnNSE;
            //adOpenDynamic;
            //adLockOptimistic;
            //while (!RS2.EOF)
            //{

            // clsdata data = new clsdata();
            CNT = ohlc.Count;
            //    // add in list
            //    object Preserve;
            //    ((string)(X[CNT]));
            //    X[CNT] = RS2;
            //    (dDate + ("," + RS2));
            //    (Open + ("," + RS2));
            //    (High + ("," + RS2));
            //    (Low + ("," + RS2));
            //    (Close + ("," + RS2));
            //    (Turnover + ("," + RS2));
            //    DP;
            //    CNT = (CNT + 1);
            //    RS2.MoveNext;
            //}

            norecord:
            if ((CNT == 0))
            {
                // error / no record found
                ohlc = new List<OHLCdp>();
                //object Preserve;
                //((string)(X[CNT]));
                //X[0] = "err";
            }

            return ohlc;
            //getDatesInPeriod = X;
            // -------------------
            // TODO: On Error Resume Next Warning!!!: The statement is not translatable 
            //RS.Close;
            //RS2.Close;
            //RS = null;
            //RS2 = null;
        }



        //List<clsNiku.OHLC> getDatesInPeriod(ChartPeriod chartP, string Script, DateTime date, DateTime datePrev)
        //{
        //    // select * from `MARUTIw` where ddate< STR_TO_DATE('29,11,2019','%d,%m,%Y') order by ddate desc limit 1;
        //    // select * from `MARUTI` where ddate between STR_TO_DATE('23,11,2019','%d,%m,%Y')  and STR_TO_DATE('29,11,2019','%d,%m,%Y') ;
        //    // select * from `MARUTI` where ddate> STR_TO_DATE('25,11,2019','%d,%m,%Y') order by ddate limit 1;
        //    // select * from `MARUTI60` where ddate between STR_TO_DATE('25,11,2019','%d,%m,%Y')  and STR_TO_DATE('26,11,2019','%d,%m,%Y') ;
        //    // Now list Daily dates in this period.. ----------------------------------------------
        //    string[] X;
        //    List<clsNiku.OHLC> ohlc = new List<clsNiku.OHLC>();
        //    // 0:Date   1: O   2:H    3:L    4:C    5:Vol    csv format
        //    string S;
        //    string Tbl;
        //    int CNT = 0;
        //    clsSql objsql = new clsSql();
        //    DataTable dt;
        //    if ((chartP == ChartPeriod.DailyChart))
        //    {
        //        objsql.connect();
        //        S = getScriptQS(ChartPeriod.HourlyChart, Script);
        //        S += "where Cast(ddate as date) = " + clsGen.DateTimeToSqlDateTime(date);

        //        dt = objsql.getDataTable(S);
        //        objsql.disconnect();
        //        if (dt.Rows.Count <= 0)
        //        {
        //            goto norecord;
        //        }



        //        //// use from Hourly chart
        //        //Tbl = getTableName(ChartPeriod.HourlyChart, Script);
        //        //// check for availability of 60 min chart data
        //        //ADODB.Recordset RS;
        //        //RS = new ADODB.Recordset();
        //        //RS.Open;
        //        //("select * from "
        //        //            + (Tbl + ""));
        //        //cnNSE;
        //        //adOpenDynamic;
        //        //adLockOptimistic;
        //        //if (RS.EOF)
        //        //{
        //        //    goto norecord;
        //        //}
        //    }
        //    else
        //    {
        //        // use from daily data
        //        objsql.connect();
        //        S = getScriptQS(ChartPeriod.DailyChart, Script);
        //        S += " where ddate between " + clsGen.DateTimeToSqlDateTime(datePrev) + " and " + clsGen.DateTimeToSqlDateTime(date);
        //        dt = objsql.getDataTable(S);
        //        objsql.disconnect();
        //        if (dt.Rows.Count <= 0)
        //        {
        //            goto norecord;
        //        }
        //        //if (blnMDB)
        //        //{
        //        //    S = ("select * from `"
        //        //                + (Script + ("` where ddate between (select top 1 ddate from `"
        //        //                + (Tbl + ("` where ddate< format(#"
        //        //                + (DT.Year + ("/"
        //        //                + (DT.Month + ("/"
        //        //                + (DT.Day + ("#" + (", \'yyyy/mm/dd\') order by ddate desc,id)  and format(#"
        //        //                + (DT.Year + ("/"
        //        //                + (DT.Month + ("/"
        //        //                + (DT.Day + ("#" + ", \'yyyy/mm/dd\') ;"))))))))))))))))));
        //        //}
        //        //else
        //        //{
        //        //    S = ("select * from `"
        //        //                + (Script + ("` where ddate between (select ddate from `"
        //        //                + (Tbl + ("` where ddate< STR_TO_DATE(\'"
        //        //                + (DT.Day + (","
        //        //                + (DT.Month + (","
        //        //                + (DT.Year + ("\'" + (",\'%d,%m,%Y\') order by ddate desc limit 1)  and STR_TO_DATE(\'"
        //        //                + (DT.Day + (","
        //        //                + (DT.Month + (","
        //        //                + (DT.Year + ("\'" + ",\'%d,%m,%Y\') ;"))))))))))))))))));
        //        //}

        //    }

        //    //ADODB.Recordset RS2;
        //    //int CNT;
        //    //RS2 = new ADODB.Recordset();
        //    //RS2.Open;
        //    //S;
        //    //cnNSE;
        //    //adOpenDynamic;
        //    //adLockOptimistic;
        //    //while (!RS2.EOF)
        //    //{

        //    clsdata data = new clsdata();
        //    ohlc = (from DataRow dr in dt.Rows
        //            select data.setOHLCvals(dr)).ToList();
        //    CNT = ohlc.Count;
        ////    // add in list
        ////    object Preserve;
        ////    ((string)(X[CNT]));
        ////    X[CNT] = RS2;
        ////    (dDate + ("," + RS2));
        ////    (Open + ("," + RS2));
        ////    (High + ("," + RS2));
        ////    (Low + ("," + RS2));
        ////    (Close + ("," + RS2));
        ////    (Turnover + ("," + RS2));
        ////    DP;
        ////    CNT = (CNT + 1);
        ////    RS2.MoveNext;
        ////}

        //    norecord:
        //    if ((CNT == 0))
        //    {
        //        // error / no record found
        //        ohlc = null;
        //        //object Preserve;
        //        //((string)(X[CNT]));
        //        //X[0] = "err";
        //    }

        //    return ohlc;
        //    //getDatesInPeriod = X;
        //    // -------------------
        //    // TODO: On Error Resume Next Warning!!!: The statement is not translatable 
        //    //RS.Close;
        //    //RS2.Close;
        //    //RS = null;
        //    //RS2 = null;
        //}

        //private void cmdGenerateDP_Click()
        //{
        //    // Generate DP for selected timeframe & script and store in table
        //    blnCancel = false;
        //    ProgSel = DP;
        //    // --- DIRECTOR PATTERN tabular--------------------
        //    // On Error Resume Next
        //    ConnectNse();
        //    ADODB.Recordset RS;
        //    int TimeP;
        //    // time period
        //    for (TimeP = 0; (TimeP <= 5); TimeP++)
        //    {
        //        // Daily to Yearly
        //        opPeriod(TimeP).Value = true;
        //        TL = 99;
        //        // tmp val to reset leg
        //        lblStat.Caption = ("Updating " + opPeriod(TimeP).Caption);
        //        DoEvents;
        //        RS = new ADODB.Recordset();
        //        string S;
        //        S = getScriptQS(getSelectedPeriodOption(opPeriod), cmbScript.Text);
        //        RS.Open;
        //        S;
        //        cnNSE;
        //        adOpenDynamic;
        //        adLockOptimistic;
        //        // ------------------------------------------------------------------------------------------------------
        //        // reach to the record with HIGH LOW OPEN available... sm records just hav close in starting yrs of 1995
        //        // ------------------------------------------------------------------------------------------------------
        //        while (!RS.EOF)
        //        {
        //            if (((Val(RS, (Open + "")) == 0)
        //                        || ((Val(RS, (High + "")) == 0)
        //                        || ((Val(RS, (Low + "")) == 0)
        //                        || (Val(RS, (Close + "")) == 0)))))
        //            {
        //                RS.MoveNext;
        //            }
        //            else
        //            {
        //                break; //Warning!!! Review that break works as 'Exit Do' as it could be in a nested instruction like switch
        //            }

        //        }

        //        if (RS.EOF)
        //        {
        //            goto nxtppp;
        //        }
        //        else
        //        {
        //            RS.MoveNext;
        //        }

        //        if (RS.EOF)
        //        {
        //            goto nxtppp;
        //        }
        //        else
        //        {
        //            RS.MoveNext;
        //        }

        //        // start frm nxt to nxt date.. as prev to prev is required for DP
        //        // now chk if already filled... reach to last updated DP record -----------------------------------------------
        //        if ((chkResume.Value == vbChecked))
        //        {
        //            while (!RS.EOF)
        //            {
        //                if (RS)
        //                {
        //                    (DPleg != "");
        //                    TL = Val(RS, DPleg);
        //                    RS.MoveNext;
        //                }
        //                else
        //                {
        //                    break; //Warning!!! Review that break works as 'Exit Do' as it could be in a nested instruction like switch
        //                }

        //            }

        //        }
        //        else
        //        {
        //            // start from beginning of record..
        //            // skip 2 records... as previous 2 data needed to generate dp
        //            RS.MoveNext;
        //            RS.MoveNext;
        //        }

        //        // Table to multi dimen array
        //        // Dim XX, Cnt As Double
        //        // XX = RS.GetRows() '0: id  1:ddate  2:open 3:high 4:low 5:close 6:shares 7:turnover 8:dp 9:validbs 10:pcg 11:theories
        //        // Dim Max As Double
        //        // Max = RS.RecordCount
        //        // Dim DPvals() As OHLC
        //        // ReDim DPvals(Max) As OHLC
        //        // Now Continue with Filling data in DB while generating Director Pattern '--------------------------------------------------------------------------------------------------------
        //        while (!RS.EOF)
        //        {
        //            // Previous data ------------------------------
        //            RS.MovePrevious;
        //            double PrevHigh;
        //            double PrevLow;
        //            PrevHigh = RS;
        //            High;
        //            PrevLow = RS;
        //            Low;
        //            RS.MoveNext;
        //            // back to normal
        //            // --------------------------------------------
        //            object Range;
        //            Range = (Val(RS, High) - Val(RS, Low));
        //            lblStat.Caption = ("Generating DP: "
        //                        + (opPeriod(TimeP).Caption + (" " + RS)));
        //            (dDate + ("    "
        //                        + (cmbScript.ListIndex + ("/" + cmbScript.ListCount))));
        //            // Do While Not Year(CDate(RS!dDate)) = 2008'for checking
        //            // RS.MoveNext
        //            // Loop
        //            // DoEvents
        //            // Director Pattern-----------------------------
        //            object X;
        //            //  As Double '0:JGD    1:JWD    2:BDP    3:WDP   4:Pattern Line1     5: Pattern Line2       6: DPpatternName
        //            X = getDirectorPattern(Val(RS, High), Val(RS, Low), PrevHigh, PrevLow);
        //            // fDP(cntDP).lblPattern(CNT).Caption = X(4) & vbCrLf & X(5)
        //            RS;
        //            DP = (X[4] + ("\r\n" + X[5]));
        //            // <<<<<  DIRECTOR PATTERN
        //            // ------------- get TVHS nd TVLS of bdp, jwd, jgd, wdp -----------------------------------------
        //            // Load RC FC Form to chk the placement
        //            frmDaily FRCFC;
        //            FRCFC = new frmDaily();
        //            FRCFC.Hide;
        //            FRCFC.selScript = cmbScript.Text;
        //            FRCFC.SelExchange = cmbMain.Text;
        //            int KK;
        //            for (KK = 0; (KK <= 5); KK++)
        //            {
        //                FRCFC.opPeriod(KK).Value = opPeriod(KK).Value;
        //            }

        //            // set D M W Q Y
        //            FRCFC.Caption = cmbScript.Text;
        //            FRCFC.dtDate.Value = DateTime.Parse(RS);
        //            dDate;
        //            FRCFC.LoadHistoryData;
        //            // ********
        //            //  PCG : Post Closing Gap
        //            // ********
        //            string PCG;
        //            PCG = getPCG(FRCFC, getSelectedPeriodOption(opPeriod));
        //            RS;
        //            PCG = PCG;
        //            // ****************************
        //            // Get the Trending LEG, Valid Buy/Sell, PCG
        //            // ****************************
        //            object t;
        //            // 0: Trending Leg    1:Valid Buy Sell Color
        //            t = getTrendingLEG(FRCFC);
        //            TL = t[0];
        //            // *****
        //            // Theories
        //            // *****
        //            string OpenAs;
        //            string Theo;
        //            // OpenAs = fRCFC.getICRCicfc
        //            // Theo = fRCFC.getTheories
        //            // RS!theories = OpenAs & vbCrLf & Theo
        //            RS;
        //            DPleg = TL;
        //            RS;
        //            validBS = t[1];
        //            // Color range as per DP : VALID BUY: Green   VALID SELL: Red    INVALID B/S: Black
        //            RS.Update;
        //            // --------------------------------------------------------------------
        //            // Prev Day DP is applicable for Current Date... so change in TL shud also be reflected in prev date
        //            RS.MovePrevious;
        //            // prev
        //            RS;
        //            DPleg = TL;
        //            RS;
        //            validBS = t[1];
        //            // Color range as per DP : VALID BUY: Green   VALID SELL: Red    INVALID B/S: Black
        //            RS.Update;
        //            RS.MoveNext;
        //            // bk to normal
        //            // --------------------------------------------------------------------
        //            Unload;
        //            FRCFC;
        //            RS.MoveNext;
        //            if (blnCancel)
        //            {
        //                lblStat.Caption = "Cancelled";
        //            }

        //            return;
        //        }

        //    nxtppp:
        //        RS.Close;
        //        RS = null;
        //    }

        //    opPeriod(0).Value = true;
        //    lblStat.Caption = "Done";
        //}


        public static Color ValidBsColorFromStr(int str)
        {
            Color c= Colors.Black;
            if (str == 0) c = Colors.Black;
            if (str == 1) c = Colors.Green;
            if (str == 2) c = Colors.Red;
            return c;
        }

        public double[] getTvHsTvLs(double Vprice, channel[] rcfc)
        {
            double TVHS = 0, TVLS = 0, nxtUpLevel = 0, nxtDwLevel = 0;
            double[] K = new double[4];

            
            if ((Vprice == 0))
            {
                // 0: TVHS     1:TVLS    2: Next Level TVHS    3:Next Level TVLS
                K[0] = TVHS;
                K[1] = TVLS;
                K[2] = nxtUpLevel;
                K[3] = nxtDwLevel;
                return K;
                //goto NF;
            }

           
            int I;
            // ------- RC channels -----------
            // 11-23
            if (Vprice >= rcfc[0].tvls[0] && Vprice <=  rcfc[0].tvhs[1])
            {
                TVHS = rcfc[0].tvhs[2]; //double.Parse(lblRCtvHS(2).Caption);
                TVLS = rcfc[1].tvls[2]; // double.Parse(lblFCtvLs(2).Caption);
                nxtUpLevel = rcfc[0].tvhs[3]; // double.Parse(lblRCtvHS(3).Caption);
                nxtDwLevel = rcfc[1].tvls[3]; // double.Parse(lblFCtvLs(3).Caption);
            }
            // 23 TVHS -38 RC
            else if (Vprice > rcfc[0].tvhs[1] && Vprice <=  rcfc[0].val[2]) 
            {
                TVHS =  rcfc[0].tvhs[2]; //double.Parse(lblRCtvHS(2).Caption);
                TVLS = rcfc[0].tvls[2]; // double.Parse(lblRCtvLs(2).Caption);
                // *changed from fc to rc
                nxtUpLevel = rcfc[0].tvhs[3];  //double.Parse(lblRCtvHS(3).Caption);
                nxtDwLevel = rcfc[1].tvls[3]; // double.Parse(lblFCtvLs(3).Caption);
            }
            // 38-61
            else if (Vprice > rcfc[0].val[2] && Vprice <=  rcfc[0].val[3])
            {
                TVHS =  rcfc[0].tvhs[3]; //double.Parse(lblRCtvHS(3).Caption);
                TVLS = rcfc[0].tvls[2]; // double.Parse(lblRCtvLs(2).Caption);
                nxtUpLevel = rcfc[0].tvhs[4]; //double.Parse(lblRCtvHS(4).Caption);
                nxtDwLevel = rcfc[0].tvls[1]; //double.Parse(lblRCtvLs(1).Caption);
            }
            // 61-100
            else if (Vprice > rcfc[0].val[3] && Vprice <= rcfc[0].val[4] )
            {
                TVHS =  rcfc[0].tvhs[4]; //double.Parse(lblRCtvHS(4).Caption);
                TVLS=  rcfc[0].tvls[3]; //double.Parse(lblRCtvLs(3).Caption);
                nxtUpLevel = rcfc[0].tvhs[5];  //double.Parse(lblRCtvHS(5).Caption);
                nxtDwLevel = rcfc[0].tvls[2]; // double.Parse(lblRCtvLs(2).Caption);
            }
            // 100-127
            else if (Vprice > rcfc[0].val[4] && (Vprice <=  rcfc[0].val[5]))
            {
                TVHS = rcfc[0].tvhs[5];//double.Parse(lblRCtvHS(5).Caption);
                TVLS = rcfc[0].tvls[4]; //double.Parse(lblRCtvLs(4).Caption);
                nxtUpLevel = rcfc[0].tvhs[6]; //double.Parse(lblRCtvHS(6).Caption);
                nxtDwLevel = rcfc[0].tvls[3]; //double.Parse(lblRCtvLs(3).Caption);
            }
            // 127-161
            else if (Vprice > rcfc[0].val[5] && (Vprice <= rcfc[0].val[6]))
            {
                TVHS =  rcfc[0].tvhs[6];//double.Parse(lblRCtvHS(6).Caption);
                TVLS = rcfc[0].tvls[5];//double.Parse(lblRCtvLs(5).Caption);
                nxtUpLevel = rcfc[0].tvhs[7]; //double.Parse(lblRCtvHS(7).Caption);
                nxtDwLevel =rcfc[0].tvls[4]; //double.Parse(lblRCtvLs(4).Caption);
            }
            // 161-261
            else if (Vprice > rcfc[0].val[6] && Vprice <= rcfc[0].val[7])//double.Parse(lblRC(7).Caption))))
            {
                TVHS = rcfc[0].tvhs[7]; //double.Parse(lblRCtvHS(7).Caption);
                TVLS = rcfc[0].tvls[6];//double.Parse(lblRCtvLs(6).Caption);
                nxtUpLevel = rcfc[0].tvhs[8];//double.Parse(lblRCtvHS(8).Caption);
                nxtDwLevel = rcfc[0].tvls[5];//double.Parse(lblRCtvLs(5).Caption);
            }
            // 261-423
            else if (Vprice > rcfc[0].val[7] && Vprice <= rcfc[0].val[8])
            {
                TVHS = rcfc[0].tvhs[8];//double.Parse(lblRCtvHS(8).Caption);
                TVLS = rcfc[0].tvls[7];//double.Parse(lblRCtvLs(7).Caption);
                // '''''''''''''nxtUpLevel = Val(lblRCtvHS(9).Caption)
                nxtDwLevel = rcfc[0].tvls[6]; //double.Parse(lblRCtvLs(6).Caption);
            }
            // Between RC and FC ------------------------------------------------------------------------
            // 11-11
            else if (Vprice > rcfc[1].tvhs[0] && Vprice < rcfc[0].tvls[0])
            {
                TVHS = rcfc[0].tvhs[2];// < gotta confirm this condition
                TVLS = rcfc[1].tvls[2];//double.Parse(lblFCtvLs(2).Caption);
                nxtUpLevel = rcfc[0].tvhs[3]; //double.Parse(lblRCtvHS(3).Caption);
                nxtDwLevel = rcfc[1].tvls[3];//double.Parse(lblFCtvLs(3).Caption);
              
            }
            // --------------
            // FC Channel
            // --------------
            // 11-23
            else if(Vprice <= rcfc[1].tvhs[0]//double.Parse(lblFCtvHS(0).Caption))
                        && Vprice >= rcfc[1].tvls[1])//double.Parse(lblFCtvLs(1).Caption))
            {
                TVHS = rcfc[0].tvhs[2]; //double.Parse(lblRCtvHS(2).Caption);
                TVLS = rcfc[1].tvls[2]; //double.Parse(lblFCtvLs(2).Caption);
                nxtUpLevel = rcfc[0].tvhs[3];//double.Parse(lblRCtvHS(3).Caption);
                nxtDwLevel = rcfc[1].tvls[3]; //double.Parse(lblFCtvLs(3).Caption);
            }
            // 23-38
            else if (Vprice < rcfc[1].tvls[1]//double.Parse(lblFCtvLs(1).Caption))
                        && Vprice >= rcfc[1].val[2])//double.Parse(lblFC(2).Caption))))
            {
                TVHS = rcfc[0].tvhs[2];// < confirm this condition
                TVLS = rcfc[1].tvls[2];//double.Parse(lblFCtvLs(2).Caption);
                nxtUpLevel = rcfc[0].tvhs[3];//double.Parse(lblRCtvHS(3).Caption);
                nxtDwLevel = rcfc[1].tvls[3];//double.Parse(lblFCtvLs(3).Caption);
            }
            // 38-61
            else if (Vprice < rcfc[1].val[2] //double.Parse(lblFC(2).Caption))
                        && Vprice >= rcfc[1].val[3])//double.Parse(lblFC(3).Caption))))
            {
                TVHS = rcfc[1].tvhs[2];//double.Parse(lblFCtvHS(2).Caption);
                TVLS = rcfc[1].tvls[3];//double.Parse(lblFCtvLs(3).Caption);
                nxtUpLevel = rcfc[1].tvhs[1];//double.Parse(lblFCtvHS(1).Caption);
                nxtDwLevel = rcfc[1].tvls[4];//double.Parse(lblFCtvLs(4).Caption);
            }
            // 61-100
            else if (Vprice < rcfc[1].val[3]//double.Parse(lblFC(3).Caption))
                        && Vprice >= rcfc[1].val[4])//double.Parse(lblFC(4).Caption))))
            {
                TVHS = rcfc[1].tvhs[3];//double.Parse(lblFCtvHS(3).Caption);
                TVLS = rcfc[1].tvls[4];//double.Parse(lblFCtvLs(4).Caption);
                nxtUpLevel = rcfc[1].tvhs[2];//double.Parse(lblFCtvHS(2).Caption);
                nxtDwLevel = rcfc[1].tvls[5];//double.Parse(lblFCtvLs(5).Caption);
            }
            // 100-127
            else if (Vprice < rcfc[1].val[4]//double.Parse(lblFC(4).Caption))
                        && Vprice >= rcfc[1].val[5])// double.Parse(lblFC(5).Caption))))
            {
                TVHS = rcfc[1].tvhs[4];//double.Parse(lblFCtvHS(4).Caption);
                TVLS = rcfc[1].tvls[5];//double.Parse(lblFCtvLs(5).Caption);
                nxtUpLevel = rcfc[1].tvhs[3]; //double.Parse(lblFCtvHS(3).Caption);
                nxtDwLevel = rcfc[1].tvls[6];//double.Parse(lblFCtvLs(6).Caption);
            }
            // 127-161
            else if (Vprice < rcfc[1].val[5]//double.Parse(lblFC(5).Caption))
                        && Vprice >= rcfc[1].val[6])//double.Parse(lblFC(6).Caption))))
            {
                TVHS = rcfc[1].tvhs[5];//double.Parse(lblFCtvHS(5).Caption);
                TVLS = rcfc[1].tvls[6];//double.Parse(lblFCtvLs(6).Caption);
                nxtUpLevel = rcfc[1].tvhs[4];//double.Parse(lblFCtvHS(4).Caption);
                nxtDwLevel = rcfc[1].tvls[7];//double.Parse(lblFCtvLs(7).Caption);
            }
            // 161-261
            else if (Vprice < rcfc[1].val[6] //double.Parse(lblFC(6).Caption))
                        && Vprice >= rcfc[1].val[7])//double.Parse(lblFC(7).Caption))))
            {
                TVHS = rcfc[1].tvhs[6];//double.Parse(lblFCtvHS(6).Caption);
                TVLS = rcfc[1].tvls[7];//double.Parse(lblFCtvLs(7).Caption);
                nxtUpLevel = rcfc[1].tvhs[5]; //double.Parse(lblFCtvHS(5).Caption);
                nxtDwLevel = rcfc[1].tvls[8];//double.Parse(lblFCtvLs(8).Caption);
            }
            // 261-423
            else if (Vprice < rcfc[1].val[7]//double.Parse(lblFC(7).Caption))
                        && Vprice >= rcfc[1].val[8])//double.Parse(lblFC(8).Caption))))
            {
                TVHS = rcfc[1].tvhs[7];//double.Parse(lblFCtvHS(7).Caption);
                TVLS = rcfc[1].tvls[8];//double.Parse(lblFCtvLs(8).Caption);
                nxtUpLevel = rcfc[1].tvhs[6];//double.Parse(lblFCtvHS(6).Caption);
                // '''''''''''nxtDwLevel = Val(lblFCtvLs(9).Caption)
            }

     //   NF:
            //double[] K = new double[4];
            // 0: TVHS     1:TVLS    2: Next Level TVHS    3:Next Level TVLS
            K[0] = TVHS;
            K[1] = TVLS;
            K[2] = nxtUpLevel;
            K[3] = nxtDwLevel;
            return K;
        }

        double getKissRC(HistoryData hd, channel rc)
        {
            double range = hd.prev.High - hd.prev.Low;            // prev high-prev low
            double Buf= (hd.current.Close * 0.073 / 100.0) + (range * 7.3 / 100.0);            
            return clsGen.FormatDecPrice(rc.val[0] - Buf);            
        }

        double getKissFC(HistoryData hd, channel fc)
        {
            double range = hd.prev.High - hd.prev.Low;            // prev high-prev low
            double Buf= (hd.current.Close * 0.073 / 100.0) + (range * 7.3 / 100.0);            
            return clsGen.FormatDecPrice(fc.val[0] + Buf);            
        }

        void setOHLCvals(ref OHLCdp ohlc, DataTable dt, int revInd=0)
        {
            int ind=revInd;
            ohlc.Date = Convert.ToDateTime(dt.Rows[ind].Field<DateTime>("ddate"));
            ohlc.Open =Convert.ToDouble( dt.Rows[ind].Field<string>("open"));
            ohlc.High =Convert.ToDouble( dt.Rows[ind].Field<string>("high"));
            ohlc.Low =Convert.ToDouble( dt.Rows[ind].Field<string>("low"));
            ohlc.Close =Convert.ToDouble( dt.Rows[ind].Field<string>("close"));
            ohlc.Turnover =Convert.ToDouble( dt.Rows[ind].Field<string>("turnover"));                                         

            ohlc.PCG = dt.Rows[ind].Field<string>("pcg");
            ohlc.ValidBS = dt.Rows[ind].Field<string>("validbs");
            ohlc.Dpleg = dt.Rows[ind].Field<string>("dpleg");
        }

        public double getRC(int rcNum, double Range, double Pclose)
        {
            // Rising Channel
            double V;
            V = Range * (PER[rcNum] / 100.0) + Pclose;
            return clsGen.FormatDecPrice(V);
        }

        public double getFC(int rcNum, double Range, double Pclose)
        {
            // Falling Channel
            double V;
            V = Pclose - Range * (PER[rcNum] / 100.0);
            return clsGen.FormatDecPrice(V);
        }

        //public List<clsNiku.OHLC> getOHLCRangeData(string script, DateTime sDt, DateTime eDt, ChartPeriod cp, string instr_Id, int limit = 0, bool blnReadKite = true, bool blnEqual = true)
        //{
        //             //get history data from data base
        //    DateTime sdt = getDate(sDt, limit, (ChartPeriod)cp);
        //    getTableName((ChartPeriod)cp, script);
        //    //load DP report as per date selected ----------------------------
        //    string qr = selectionqry(sdt, eDt, script, cp, limit, blnEqual);//generate query as per input


        //    //get history data from data base
        //    clsSql cls = new clsSql();
        //    cls.connect();
        //    DataTable dt = cls.getDataTable(qr);
        //    cls.disconnect();
        //    List<clsNiku.OHLC> ohlc = (from DataRow dr in dt.Rows
        //                               select new clsNiku.OHLC()
        //                               {
        //                                   Close = Convert.ToDouble(dr["close"].ToString()),
        //                                   Date = Convert.ToDateTime(dr["ddate"].ToString()),
        //                                   High = Convert.ToDouble(dr["high"].ToString()),
        //                                   Low = Convert.ToDouble(dr["low"].ToString()),
        //                                   Open = Convert.ToDouble(dr["open"].ToString()),
        //                                   Volume = Convert.ToDouble(dr["turnover"].ToString()),
        //                                   ValidBS = dr["validbs"].ToString(),
        //                                   Dpleg = dr["dpleg"].ToString(),
        //                                   Dp = dr["dp"].ToString(),
        //                                   PCG = dr["pcg"].ToString()
        //                               }).ToList();

        //    if (blnReadKite == false)
        //    {
        //        clsdata data = new clsdata();
        //        //data not found in database............ get data from Kite 
        //        if (ohlc.Count <= 0)
        //        {
        //            ohlc = data.getOhlcData(script, sdt, eDt, instr_Id, cp);
        //        }
        //        else if (ohlc.Any(x => string.IsNullOrEmpty(x.Dpleg)))
        //        {
        //            data.startThreadUpdateDP(script, cp);

        //        }
        //    }

        //    ohlc.Reverse();

        //    return ohlc;
        //}

        public List<clsNiku.OHLCdp> getOHLCRangeData(string script, DateTime sDt, DateTime eDt, TimeFrame cp, string instr_Id, int limit = 0, bool blnReadKite = true, bool blnNikuData = true,bool blnGlobal=false,bool blnAPI=false)
        {
            DateTime sdt = getDate(sDt, limit, (TimeFrame)cp);

            //d history data from data base between the selected dates
            List<clsNiku.OHLCdp> ohlchistory = getAllOHLC(script, cp, blnNikuData,blnAPI:blnAPI);
            
            ohlchistory = ohlchistory.Where(x => x.Date >= sdt && x.Date <= eDt).ToList();
       

            if (blnReadKite == true)
            {
                clsdata data = new clsdata();
                //data not found in database............ get data from Kite 
                if (ohlchistory.Count <= 0)
                {
                    ohlchistory = data.getOhlcData(script,  sdt, eDt, instr_Id, cp);
                }
                else if (ohlchistory.Any(x => string.IsNullOrEmpty(x.Dpleg)))
                {
                   //data.startThreadUpdateDP(script, cp);

                }
            }

            ohlchistory.Reverse();

            //if (blnGlobal == true)
            //{
            //    //For Global Data Feed


            //        Instruments instr = new Instruments();
            //    instr = clsGenfx.liveFeedInstr.Where(x => x.ScriptCode == instr_Id).FirstOrDefault();
            //    if (instr != null)
            //    {
            //       // Calling Global Data Method to Get OHLC Values
            //            List<clsNiku.OHLCdp> ohlcLst = clsBrokerDataSetting.getGlobalHistory(instr_Id, sDt, eDt, cp, 1, FeedAPI.GlobalDatafeed);
            //        if (ohlcLst.Count() > 0)
            //        {
            //            ohlchistory = ohlcLst;
            //            if (ohlchistory.Any(x => string.IsNullOrEmpty(x.Dpleg)))
            //            {
            //               // startThreadUpdateDP(script, cp);
            //            }
            //        }

            //    }

            //}





            return ohlchistory;
        }

        public List<clsNiku.OHLCdp> getOHLCRangeData(string script, DateTime sDt, DateTime eDt, TimeFrame cp, int limit = 0)
        {
            DateTime sdt = getDate(sDt, limit, (TimeFrame)cp);

            //get history data from data base between the selected dates
            List<clsNiku.OHLCdp> ohlchistory = getAllOHLC(script, cp);
            ohlchistory = ohlchistory.Where(x => x.Date >= sdt.AddHours(9).AddMinutes(15) && x.Date <= eDt.AddHours(6)).ToList();


            ohlchistory.Reverse();

            return ohlchistory;
        }

        public List<clsNiku.OHLCdp> getOHLCRangeDataWithoutAddingTime(string script, DateTime sDt, DateTime eDt, TimeFrame cp, int limit = 0)
        {
            DateTime sdt = getDate(sDt, limit, (TimeFrame)cp);

            //get history data from data base between the selected dates
            List<clsNiku.OHLCdp> ohlchistory = getAllOHLC(script, cp);
            ohlchistory = ohlchistory.Where(x => x.Date >= sdt && x.Date <= eDt).ToList();
            ohlchistory.Reverse();
            return ohlchistory;
        }



        public List<clsNiku.OHLCdp> getAllOHLC(string script, TimeFrame cp, bool blnNikuData = true,string InstrID="",bool blnAPI = false)
        {
            //--------------------------
            //Read Data From File to List
            //--------------------------
            List<clsNiku.OHLCdp> ohlcHistory = new List<OHLCdp>();

            try
            {
                if (blnNikuData == true)
                {
                    string dppath = clsGen.getPath(cp, script, blnNikuData,blnAPI);
                    string ohlcpath = clsGen.getPath(cp, script, false,blnAPI);
                    //ohlcHistory = getAllOHLC(ohlcpath,cp,blnAPI:blnAPI);      //mohit
                    ohlcHistory = getAllOHLC(ohlcpath);
                    if (ohlcHistory == null)
                    {
                        ohlcHistory = getAllOHLC(ohlcpath);
                        ohlcHistory =new List<OHLCdp>();

                    }
                    

                    List<DPVals> dpvals = new List<DPVals>();
                    dpvals = getDPVals(dppath);

                    foreach (clsNiku.OHLCdp ohlcdp in ohlcHistory)
                    {
                        DPVals vals = dpvals.FirstOrDefault(x => x.Date == ohlcdp.Date);
                        if (vals == null)
                            continue;

                        ohlcdp.Dp = vals.Dp;
                        ohlcdp.Dpleg = vals.Dpleg;
                        ohlcdp.PCG = vals.PCG;
                        ohlcdp.ValidBS = vals.ValidBS;
                    }



                    return ohlcHistory;
                }
                else
                {
                    string path = clsGen.getPath(cp, script, blnNikuData, blnAPI: blnAPI);
                    ohlcHistory = getAllOHLC(path);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            

            return ohlcHistory;
        }

        public List<clsNiku.OHLCdisp> getAllOHLC2(string script, TimeFrame cp, bool blnNikuData = true, string InstrID = "")
        {
            //--------------------------
            //Read Data From File to List
            //--------------------------
            List<clsNiku.OHLCdisp> ohlcHistory = new List<OHLCdisp>();

            try
            {
                if (blnNikuData == true)
                {
                    string dppath = clsGen.getPath(cp, script, blnNikuData);
                    string ohlcpath = clsGen.getPath(cp, script, false);
                    ohlcHistory = getAllOHLC2(ohlcpath);
                    if (ohlcHistory == null)
                    {
                        ohlcHistory = getAllOHLC2(ohlcpath);
                        ohlcHistory = new List<OHLCdisp>();

                    }





                    return ohlcHistory;
                }
                else
                {
                    string path = clsGen.getPath(cp, script, blnNikuData);
                    ohlcHistory = getAllOHLC2(path);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            return ohlcHistory;
        }

        public void getHighNLow(List<OHLCdp> ohlcHistoryDP, ref double LowestLow, ref double HighestHigh )
        {
            //check for last TRS, CSCOUB, TRS, PBBCOUB  to calculate highest high and lowest low for updation of TL
            clsNiku.OHLCdp lastTL = ohlcHistoryDP.LastOrDefault(x => x.Dpleg == "0" || x.Dpleg == "3" || x.Dpleg == "5" || x.Dpleg == "8"); //TRB, CSCOUB, TRS, PBBCODB
            if (lastTL != null)
            {
                LowestLow = lastTL.Low;
                HighestHigh = lastTL.High;
                int lastindex = ohlcHistoryDP.IndexOf(lastTL);
                for (int j = lastindex; j >= 0; j--)
                {
                    clsNiku.OHLCdp prevTL = ohlcHistoryDP[j];
                    if (prevTL.Dpleg == lastTL.Dpleg)
                    {
                        if (prevTL.Low < LowestLow)
                        {
                            LowestLow = prevTL.Low;
                        }

                        if (prevTL.High >= HighestHigh)
                        {
                            HighestHigh = prevTL.High;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

        }

        public List<clsNiku.OHLCdp> getAllOHLC(string path)
        {
            //--------------------------
            //Read Data From File to List
            //--------------------------
            List<clsNiku.OHLCdp> ohlcHistory = new List<clsNiku.OHLCdp>();
            string readfile = File.ReadAllText(path);
            if(string.IsNullOrEmpty(readfile))
                return new List<clsNiku.OHLCdp>();

            ohlcHistory = JsonConvert.DeserializeObject<List<clsNiku.OHLCdp>>(readfile);
         

            return ohlcHistory;
        }

        public List<clsNiku.OHLCdisp> getAllOHLC2(string path)
        {
            //--------------------------
            //Read Data From File to List
            //--------------------------
            List<clsNiku.OHLCdisp> ohlcHistory = new List<clsNiku.OHLCdisp>();
            string readfile = File.ReadAllText(path);
            if (string.IsNullOrEmpty(readfile))
                return new List<clsNiku.OHLCdisp>();

            ohlcHistory = JsonConvert.DeserializeObject<List<clsNiku.OHLCdisp>>(readfile);


            return ohlcHistory;
        }

        public List<DPVals> getDPVals(string path)
        {
            //--------------------------
            //Read Data From File to List
            //--------------------------
            List<DPVals> ohlcHistory = new List<DPVals>();
            string readfile = File.ReadAllText(path);
            if (string.IsNullOrEmpty(readfile))
                return new List<DPVals>();

            ohlcHistory = JsonConvert.DeserializeObject<List<DPVals>>(readfile);

            return ohlcHistory;
        }


        private DateTime  getDate(DateTime sdt, int limit, TimeFrame p)
        {
            if (limit == 0)
                return sdt;
            limit += 8;
            switch(p)
            {
                case TimeFrame.Daily:
                    return sdt.AddDays(-limit);
                case TimeFrame.Weekly:
                    return sdt.AddDays(-limit * 7);
                case TimeFrame.Monthly:
                    return sdt.AddMonths(-limit);
                case TimeFrame.Quarterly:
                    return sdt.AddMonths(-limit*3);
                case TimeFrame.HalfYearly:
                return sdt.AddMonths(-limit *6) ;
                case TimeFrame.Yearly:
                    return sdt.AddYears(-limit);
                default:
                    return sdt;
            }
        }

        //public string selectionqry(DateTime sDt, DateTime eDt, string script, ChartPeriod cp, int limit=0, bool blnMatchEqual =true)
        //{
        //    string qr = "";
        //    qr =  clsNiku.getScriptQS(cp, script);//query string as per chart  period selected for that script

        //    if (blnMatchEqual)
        //    {
        //        qr += " where ddate<= " + clsGen.DateTimeToSqlDateTime(eDt) + "" +
        //              " and ddate>=" + clsGen.DateTimeToSqlDateTime(sDt) + " order by ddate";
        //    }
        //    else
        //    {
        //        qr += " where ddate< " + clsGen.DateTimeToSqlDateTime(eDt) + "" +
        //             " and ddate>" + clsGen.DateTimeToSqlDateTime(sDt) + " order by ddate";
        //    }
            
        //    return qr;
        //}

        /// <summary>
        /// Returns history data of Script for given Date, previous date and previous to previous date
        /// </summary>
        /// <param name="Script"></param>
        /// <param name="date"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public HistoryData getPrevHistoryData(string Script, DateTime date, TimeFrame p, string instr_id)
        {
            if(p > TimeFrame.Daily)
            {
                clsdata cls = new clsdata();
                date = cls.getNxtDate(date, p);
            }
            HistoryData hd = new HistoryData();
            List<clsNiku.OHLCdp> ohlchistory = getAllOHLC(Script, p);
            try
            {
                ohlchistory.Reverse();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            
            //List<clsNiku.OHLCdp> ohlc = ohlchistory.Where(x => x.Date <= date).Take(3).ToList();
            List<clsNiku.OHLCdp> ohlc = ohlchistory.Where(x => x.Date.Date <= date.Date).Take(3).ToList();

            //if (clsGenfx.livefeed == FeedAPI.GlobalDatafeed)
            //{
            //    List<clsNiku.OHLCdp> ohlcGlobal = new List<OHLCdp>();
            //    Instruments instr = new Instruments();
            //    instr = clsGenfx.datafeedInstr.Where(x => x.ScriptCode == instr_id && x.Symbol== Script).FirstOrDefault();
            //    if(instr != null) 
            //    {
            //        ohlcGlobal = clsBrokerDataSetting.getGlobalPreviousHistoryData(instr.ScriptCode, date, p, instr.ExchangeCode);
            //        if (ohlcGlobal.Count() > 0) { ohlc = ohlcGlobal; }
            //    }
                
            //}


            if (ohlc.Count>0)
            { 
                if (ohlc.Count > 0)
                    hd.current = ohlc[0];
                if (ohlc.Count > 1)
                    hd.prev = ohlc[1];
                if (ohlc.Count > 2)
                    hd.prev2 = ohlc[2];

                return hd;
            }

            //setOHLCvals(ref hd.current, dt, 0);
            //setOHLCvals(ref hd.prev, dt, 1);
            //setOHLCvals(ref hd.prev2, dt, 2);

            //h.Current.Open =Convert.ToDouble( dr["open"].ToString());
            //MySqlDataReader dr = new MySqlDataReader();            

            //cls.disconnect();

            return hd;
        }

        ///// <summary>
        ///// Returns history data of Script for given Date, previous date and previous to previous date
        ///// </summary>
        ///// <param name="Script"></param>
        ///// <param name="date"></param>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //public HistoryData getPrevHistoryData(string Script, DateTime date, ChartPeriod p, string instr_id)
        //{
        //    HistoryData hd = new HistoryData();
        //    //SQL:- select * from [axisbank] where ddate<= Cast('" + string.Format(dt.ToString("2017-08-29")) + "' as Date)";
        //     //MYSQL: - select * from AXISBANK where ddate<= STR_TO_DATE('29,08,2017','%d,%m,%Y');
        //    string qr = getScriptQS(p, Script, 3, "*");
        //    qr += " where ddate<= "+clsGen.DateTimeToSqlDateOnly(date)+" order by ddate desc";

        //    //qr += " where ddate<= STR_TO_DATE('" + date.Day + "," + date.Month + "," + date.Year + "'" + ",'%d,%m,%Y') order by ddate desc limit 3"; //*********MYSQL


        //    //get history data
        //    clsSql cls = new clsSql();
        //    cls.connect();
        //    DataTable dt = cls.getDataTable(qr);
        //    cls.disconnect();
        //    if (dt.Rows.Count == 0)
        //    {  

        //        clsdata data = new clsdata();

        //        List<clsNiku.OHLC> ohlc = data.getOhlcData(Script, date.AddDays(-6), date, instr_id, p);

        //        if(ohlc.Count>=0)
        //            hd.prev2 = ohlc[ohlc.Count - 3];
        //        if (ohlc.Count >= 1)
        //            hd.prev = ohlc[ohlc.Count - 2];
        //        if(ohlc.Count>=2)
        //            hd.current = ohlc[ohlc.Count-1];


        //        return hd;//return blank
        //    }

        //    setOHLCvals(ref hd.current, dt, 0);
        //    setOHLCvals(ref hd.prev, dt, 1);
        //    setOHLCvals(ref hd.prev2, dt, 2);

        //    //h.Current.Open =Convert.ToDouble( dr["open"].ToString());
        //    //MySqlDataReader dr = new MySqlDataReader();            

        //    cls.disconnect();

        //    return hd;
        //}

     
    }
}
