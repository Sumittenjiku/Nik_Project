using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using Market;
using Market.Classes;

namespace NiKu
{   
    class clsATR
    {
        //GetAtr Value ------------------------------------
        //public double getATR(DateTime Date, TimeFrame ChartPeriod, string selScript)
        //{
        //    double ATRVal = 0; double range = 0; double RangeTotal = 0;
        //    int maxvalint = 0; int cnt = 0;
        //    maxvalint = GetMaxVal(ChartPeriod);

        //    clsSql clsc = new clsSql();
        //    clsc.connect();

        //    string qr = clsNiku.getScriptQS(ChartPeriod, selScript);
        //    qr += " where ddate < '" + Date.ToString("yyyy-MM-dd") + "'order by ddate desc limit " + maxvalint + ";";

        //    SqlDataReader dr = clsc.getdr(qr);

        //    while (dr.Read())
        //    {
        //        cnt = cnt + 1;
        //        range = Convert.ToDouble(dr["High"]) - Convert.ToDouble(dr["Low"]);
        //        RangeTotal = RangeTotal + range;
        //        ATRVal = RangeTotal / maxvalint;
        //    }
        //    clsc.disconnect();
        //    return ATRVal;
        //}

        //GetAtr Value ------------------------------------
        public double getATRValue(List<clsNiku.OHLCdp> ohlc, int limit)
        {
            double ATRVal = 0; double range = 0; double RangeTotal = 0;
            int maxvalint = 0; int cnt = 0;
          
            foreach(clsNiku.OHLCdp o in ohlc)
            {
                cnt = cnt + 1;
                range = o.High - o.Low;
                RangeTotal = RangeTotal + range;
                ATRVal = RangeTotal / limit;
            }
            return ATRVal;
        }
        

        ////getVolAvg Value ------------------------------------
        //public int getVolAvg(DateTime Date, TimeFrame ChartPeriod, string selScript)
        //{
        //    int cnt = 0;int turnoverTotal = 0;int turnover = 0;int VolAvg = 0;int maxvalint = 0;
        //    maxvalint = GetMaxVal(ChartPeriod);

        //    clsSql clsc = new clsSql();
        //    clsc.connect();

        //  string qr = clsNiku.getScriptQS(ChartPeriod, selScript);
        //  qr += " where ddate < '" + Date.ToString("yyyy-MM-dd") + "'order by ddate desc limit " + maxvalint + ";";

        //  SqlDataReader dr = clsc.getdr(qr);

        //    while (dr.Read())
        //    {
        //        cnt = cnt + 1;
        //        turnover = Convert.ToInt32(dr["Turnover"]);
        //        turnoverTotal = turnoverTotal + turnover;
        //        VolAvg = turnoverTotal / maxvalint;
        //    }
        //    clsc.disconnect();
        //    return VolAvg;
        //}


        //GetMaxVal Value ------------------------------------
        public int GetMaxVal(TimeFrame TF)
        {
            int maxvalint = 0;

            if (TF == TimeFrame.Daily) { maxvalint = 34; }//Daily-----
            else if (TF == TimeFrame.Weekly) { maxvalint = 21; }//Weekly--
            else { maxvalint = 13; }//monthly Q HY Y

            return maxvalint;
        }

      
        //getVolAvg Value ------------------------------------
        public int getVolAvg(List<clsNiku.OHLCdp> ohlc, int maxvalint)
        {
            int cnt = 0; int turnoverTotal = 0; int turnover = 0; int VolAvg = 0; 

            foreach (clsNiku.OHLCdp o in ohlc)
            {
                cnt = cnt + 1;
                turnover = Convert.ToInt32(o.Turnover);
                turnoverTotal = turnoverTotal + turnover;
                VolAvg = turnoverTotal / maxvalint;
            }
            return VolAvg;
        }               
       

        public string GetPVAnalysis(double cprice, double preclose, int PAVal, int p, int v, int Vol)
        {
            string PVAnalysisStat = "";
            //VolAvgValues --------------------
            if (cprice > preclose) { p = 1; }//1 means price is increasing from previous
            if (Vol > PAVal) { v = 1; }

            //now conditional result-----------------------------------------------------------------------------------
            if ((v == 1) && (p == 1)) { PVAnalysisStat = "Breakout"; }
            else if ((v == 1) && (p == 0)) { PVAnalysisStat = "Accumilation"; }
            else if ((v == 0) && (p == 0)) { PVAnalysisStat = "Bottom-out"; }
            else if ((v == 0) && (p == 1)) { PVAnalysisStat = "Top-out"; }

            return PVAnalysisStat;
        }
      
        //public int GetPrevDPLeg(TimeFrame p, string script, DateTime dt)
        //{
        //    int PrevDPLeg = 0;
        //    clsSql clsc = new clsSql();
        //    clsc.connect();
        //    string qr = clsNiku.getScriptQS(p, script);
        //    qr += " where ddate < '" + dt.ToString("yyyy-MM-dd") + "' order by ddate desc limit 1;";
        //    SqlDataReader dr = clsc.getdr(qr);
        //    while (dr.Read())
        //    {
        //        PrevDPLeg = Convert.ToInt16(dr["DPleg"]);
        //    }
        //    clsc.disconnect();
        //    return PrevDPLeg;
        //}

    }
}