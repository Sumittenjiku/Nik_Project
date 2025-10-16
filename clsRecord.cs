using Market;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using System.Security.Policy;
using Market.Models;
using DocumentFormat.OpenXml.Math;
using WebSocketSharp;
using System.Globalization;
using System.Threading;
using NiKu.Classes;
using Market.Classes;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace NiKu
{
    internal class clsRecord
    {
        public double UB = 0.0, UBtvls = 0.0, LB = 0.0, LBTVHS = 0.0;//upperband, lowerband 
        public string GetTheories(double WDP, double BDP, double JGD, double JWD, TimeFrame p, clsNiku.HistoryData hd)
        {
            //-------------------------------------------
            //stores theories as string with line breaks
            //Upperband and lowerband is also calculated
            //-------------------------------------------

            string TheoriesVal = "";


            clsNiku clsn = new clsNiku();
            ArrayList theoriesList = new ArrayList();
            clsNiku.channel[] channel = clsn.getChannels(hd, p); //Channel Array--------------------- 0:RC 1:FC
            

            bool blnSkipJGDjwd = false; //when cut cut , jgd/jwd condition is not considered
            //----
            //BDP
            //----            
            if (BDP > channel[0].val[4]) theoriesList.Add(TheoriesVal += "BDP > RC100% 4 Options" + Environment.NewLine);            
            if ((BDP < channel[0].val[4]) && (BDP >= channel[0].val[3])) theoriesList.Add(TheoriesVal += "Cutting the Flying Kite in RC" + Environment.NewLine);
            
            if ((BDP < channel[0].val[3]) && (BDP >= channel[0].val[2]))
            {
                theoriesList.Add(TheoriesVal += "BDP Cut-Cut" + Environment.NewLine);
                blnSkipJGDjwd = true;
                //UB = channel[0].tvhs[3];//rc61tvhs
                //UBtvls = channel[0].tvls[3];
            }

            
            if ((BDP < channel[0].val[2]) && (BDP >= channel[0].val[0])) theoriesList.Add(TheoriesVal = "BDP Knock-Knock" + Environment.NewLine);
            
            if ((BDP < channel[0].val[0]) && (BDP >= channel[1].val[0])) theoriesList.Add(TheoriesVal += "BDP in ICRR" + Environment.NewLine);
            
            if ((BDP <= channel[1].val[0]) && (BDP > channel[1].val[2]))
            {
                theoriesList.Add(TheoriesVal += "BDP Higher Gap/Sleeping Beauty" + Environment.NewLine);
                //LB = channel[1].tvls[3];//fc61tvls
                //LBtvhs = channel[1].tvhs[3];
            }

            //------------------
            //JGD/JWD Conditions
            //------------------
            if (blnSkipJGDjwd == false)
            {
                
                if ((WDP >= channel[0].val[0]) && (WDP < channel[0].val[2]) && ((JGD < channel[0].val[2]) || (JWD < channel[0].val[2])))
                {
                    theoriesList.Add(TheoriesVal += "Upperband RC38TVHS" + Environment.NewLine);
                    //UB = channel[0].tvhs[2];//rc38tvhs
                    //UBtvls = channel[0].tvls[2];
                }
                
                if (((BDP <= channel[1].val[0]) && (BDP > channel[1].val[2])) && ((JGD > channel[1].val[2]) || (JWD > channel[1].val[2])))
                {
                    theoriesList.Add(TheoriesVal += "Lowerband FC38TVLS" + Environment.NewLine);
                    //LB = channel[1].tvls[2];//fc38tvls
                    //LBtvhs = channel[1].tvhs[2];
                }
            }

            //Other
            if (BDP > channel[0].val[8]) theoriesList.Add(TheoriesVal += "BDP > RC423.6%" + Environment.NewLine);

            //old
            //if ((BDP < channel[0].tvls[0]) && (BDP >= channel[1].tvhs[0]) && (WDP < channel[1].tvls[2])) { theoriesList.Add(TheoriesVal += "BDP in ICRR WDP<FC38.2%" + Environment.NewLine); }
            //if ((BDP < channel[0].tvls[0]) && (BDP >= channel[1].tvhs[0]) && (WDP > channel[1].tvls[2])) { theoriesList.Add(TheoriesVal += "BDP in ICRR and WDP > FC38.2%" + Environment.NewLine); }
            //if ((BDP >= channel[0].tvhs[2]) && (BDP < channel[0].tvls[3])) { theoriesList.Add(TheoriesVal += "Cut Cut Pack up in RC" + Environment.NewLine); }
            //if ((BDP >= channel[0].tvls[0]) && (BDP < channel[0].tvhs[2]) && (WDP < channel[1].tvls[2])) { theoriesList.Add(TheoriesVal += "Knock Knock in RC and WDP < FC38.2%" + Environment.NewLine); }
            //if ((BDP >= channel[0].tvls[0]) && (BDP < channel[0].tvhs[2]) && (WDP > channel[1].tvls[2])) { theoriesList.Add(TheoriesVal += "Knock Knock in RC and WDP > FC38.2%" + Environment.NewLine); }
            //if ((BDP >= channel[1].tvls[2]) && (BDP < channel[1].tvhs[0])) { theoriesList.Add(TheoriesVal += "Higher Gap in RC" + Environment.NewLine); }

            //----
            //WDP
            //----
            
            if ((WDP >= channel[0].val[0]) && (WDP < channel[0].val[2]))
            {
                theoriesList.Add(TheoriesVal += "Lower Gap/Reverse Sleeping Beauty" + Environment.NewLine);
                //UB = channel[0].tvhs[3];//rc61tvhs
                //UBtvls = channel[0].tvls[3];
            }
            
            if ((WDP < channel[0].val[0]) && (WDP >= channel[1].val[0])) theoriesList.Add(TheoriesVal += "WDP in ICRR" + Environment.NewLine);
            
            if ((WDP < channel[1].val[0]) && (WDP >= channel[1].val[2])) theoriesList.Add(TheoriesVal += "Knock-Knock in FC" + Environment.NewLine);
            
            if ((WDP < channel[1].val[2]) && (WDP >= channel[1].val[3]))
            {
                theoriesList.Add(TheoriesVal += "Cut-Cut in FC" + Environment.NewLine);
                //LB = channel[1].tvls[3];//fc61tvls
                //LBtvhs = channel[1].tvhs[3];
            }
            
            if ((WDP < channel[1].val[3]) && (WDP >= channel[1].val[4])) theoriesList.Add(TheoriesVal += "Catching the falling Knife" + Environment.NewLine);
            
            if (WDP < channel[1].val[4]) theoriesList.Add(TheoriesVal += "WDP 4 Options" + Environment.NewLine);

            //if (WDP < channel[1].tvls[8]) { theoriesList.Add(TheoriesVal += "WDP < FC423.6%" + Environment.NewLine); }                        
            //if (WDP < channel[1].tvls[4]) { theoriesList.Add(TheoriesVal += "WDP < FC100% 4 Options" + Environment.NewLine); }
            //if ((WDP < channel[0].tvls[0]) && (WDP >= channel[1].tvhs[0]) && (BDP > channel[0].tvhs[2])) { theoriesList.Add(TheoriesVal += "WDP in ICRR and BDP>RC38.2%" + Environment.NewLine); }            
            //if ((WDP < channel[0].tvls[0]) && (WDP >= channel[1].tvhs[0]) && (BDP < channel[0].tvhs[2])) { theoriesList.Add(TheoriesVal += "WDP in ICRR and BDP < RC38.2%" + Environment.NewLine); }            
            //if ((WDP >= channel[1].tvhs[3]) && (WDP < channel[1].tvls[2])) { theoriesList.Add(TheoriesVal += "Cut Cut Pack up in FC" + Environment.NewLine); }            
            //if ((WDP >= channel[1].tvls[2]) && (WDP < channel[1].tvhs[0]) && (BDP > channel[0].tvhs[2])) { theoriesList.Add(TheoriesVal += "Knock Knock in FC and BDP > RC38.2%" + Environment.NewLine); }            
            //if ((WDP >= channel[1].tvls[2]) && (WDP < channel[1].tvhs[0]) && (BDP < channel[0].tvhs[2])) { theoriesList.Add(TheoriesVal += "Knock Knock in FC and BDP < RC38.2%" + Environment.NewLine); }            
            //if ((WDP >= channel[0].tvls[0]) && (BDP < channel[0].tvhs[2])) { theoriesList.Add(TheoriesVal += "Lower Gap in FC" + Environment.NewLine); }
            //if ((WDP >= channel[1].tvls[4]) && (WDP < channel[1].tvhs[3])) { theoriesList.Add(TheoriesVal += "Catching the Falling Knife in FC" + Environment.NewLine); }

            //adding to arraylist for individual disp/calc
            foreach (string th in theoriesList)
            {
                TheoriesVal = th;
            }
            return TheoriesVal;
        }

        ////GetTheories Value ------------------------------------
        //public string GetTheories(double WDP, double BDP, clsNiku.channel[] channel)
        //{
        //    string TheoriesVal = "";
        //    clsNiku clsn = new clsNiku();
        //    List<string> theoriesList = new List<string>();

        //    if ((BDP >= channel[1].tvls[2]) && (BDP < channel[1].tvhs[0])) { theoriesList.Add("Sleeping Beauty in FC" + Environment.NewLine); }
        //    if ((WDP >= channel[0].tvls[0]) && (WDP < channel[0].tvhs[2])) { theoriesList.Add("Reverse Sleeping Beauty in RC" + Environment.NewLine); }
        //    //--------------------------------------------------------------------------------------------------------
        //    if (WDP < channel[1].tvls[8]) { theoriesList.Add("WDP < FC423.6%" + Environment.NewLine); }
        //    if (BDP > channel[0].tvhs[8]) { theoriesList.Add("BDP > RC423.6%" + Environment.NewLine); }
        //    //--------------------------------------------------------------------------------------------------------
        //    if (WDP < channel[1].tvls[4]) { theoriesList.Add("WDP < FC100% 4 Options" + Environment.NewLine); }
        //    if ((BDP >= channel[0].tvhs[4]) && (BDP < channel[0].tvhs[8])) { theoriesList.Add("BDP > RC100% 4 Options" + Environment.NewLine); }
        //    //-----------------------------------------------------------------------------------------------------------
        //    if ((BDP < channel[0].tvls[0]) && (BDP >= channel[1].tvhs[0]) && (WDP < channel[1].tvls[2])) { theoriesList.Add("BDP in ICRR WDP < FC38.2%" + Environment.NewLine); }
        //    if ((WDP < channel[0].tvls[0]) && (WDP >= channel[1].tvhs[0]) && (BDP > channel[0].tvhs[2])) { theoriesList.Add("WDP in ICRR and BDP > RC38.2%" + Environment.NewLine); }
        //    //---------------------------------------------------------------------------------------------------------------------------------------------
        //    if ((BDP < channel[0].tvls[0]) && (BDP >= channel[1].tvhs[0]) && (WDP > channel[1].tvls[2])) { theoriesList.Add("BDP in ICRR and WDP > FC38.2%" + Environment.NewLine); }
        //    if ((WDP < channel[0].tvls[0]) && (WDP >= channel[1].tvhs[0]) && (BDP < channel[0].tvhs[2])) { theoriesList.Add("WDP in ICRR and BDP < RC38.2%" + Environment.NewLine); }
        //    //-----------------------------------------------------------------------------------------------------------------------------------------------
        //    if ((WDP >= channel[1].tvhs[3]) && (WDP < channel[1].tvls[2])) { theoriesList.Add("Cut Cut Pack up in FC" + Environment.NewLine); }
        //    if ((BDP >= channel[0].tvhs[2]) && (BDP < channel[0].tvls[3])) { theoriesList.Add("Cut Cut Pack up in RC" + Environment.NewLine); }
        //    //-----------------------------------------------------------------------------------------------------------
        //    if ((BDP >= channel[0].tvls[0]) && (BDP < channel[0].tvhs[2]) && (WDP < channel[1].tvls[2])) { theoriesList.Add("Knock Knock in RC and WDP < FC38.2%" + Environment.NewLine); }
        //    if ((WDP >= channel[1].tvls[2]) && (WDP < channel[1].tvhs[0]) && (BDP > channel[0].tvhs[2])) { theoriesList.Add("Knock Knock in FC and BDP > RC38.2%" + Environment.NewLine); }
        //    //-----------------------------------------------------------------------------------------------------------------------------------------------
        //    if ((BDP >= channel[0].tvls[0]) && (BDP < channel[0].tvhs[2]) && (WDP > channel[1].tvls[2])) { theoriesList.Add("Knock Knock in RC and WDP > FC38.2%" + Environment.NewLine); }
        //    if ((WDP >= channel[1].tvls[2]) && (WDP < channel[1].tvhs[0]) && (BDP < channel[0].tvhs[2])) { theoriesList.Add("Knock Knock in FC and BDP < RC38.2%" + Environment.NewLine); }
        //    //-----------------------------------------------------------------------------------------------------------------------------------------------
        //    if ((BDP >= channel[1].tvls[2]) && (BDP < channel[1].tvhs[0])) { theoriesList.Add("Higher Gap in RC" + Environment.NewLine); }
        //    if ((WDP >= channel[0].tvls[0]) && (BDP < channel[0].tvhs[2])) { theoriesList.Add("Lower Gap in FC" + Environment.NewLine); }
        //    //-----------------------------------------------------------------------------------------------------------
        //    if ((WDP >= channel[1].tvls[4]) && (WDP < channel[1].tvhs[3])) { theoriesList.Add("Catching the Falling Knife in FC" + Environment.NewLine); }
        //    if ((BDP >= channel[0].tvls[3]) && (BDP < channel[0].tvhs[4])) { theoriesList.Add("Cutting the Flying Kite in RC" + Environment.NewLine); }
        //    //-----------------------------------------------------------------------------------------------------------

        //    foreach (string th in theoriesList)
        //    {
        //        TheoriesVal += th;
        //    }
        //    return TheoriesVal;

        //}


        //-------------------------------------------------------------------------------------------
        // Get UB And LB Values
        //-------------------------------------------------------------------------------------------
        public string strChannelUB = string.Empty;
        public string strChannelLB = string.Empty;
        public double[] getUBLB(clsNiku.channel[] channel, clsNiku.DirectorPattern dp)
        {
            UB = 0.0; LB = 0.0; LBTVHS = 0.0; UBtvls = 0.0;
            strChannelUB = string.Empty;
            strChannelLB = string.Empty;
            string LBtxt = string.Empty;
            string UBtxt = string.Empty;
            //clsNiku clsn = new clsNiku();
            //clsNiku.channel[] channel = clsn.getChannels(hd, p); //Channel Array--------------------- 0:RC 1:FC
            double[] UBLB = new double[4];
            ////Default vals for Upperband and Lowerband
            UB = channel[0].tvhs[2];
            UBtxt = "UB: rc38tvhs ";
            UBtvls = channel[0].tvls[2];//upperband ka lower side
            LB = channel[1].tvls[2];
            LBtxt = "LB: fc38tvls";
            LBTVHS = channel[1].tvhs[2];//lowerband ka higher side



            //-------------------------------------------------------
            // 1st Check
            //-------------------------------------------------------

            if (((dp.WDP <= channel[1].val[0]) && (dp.WDP > channel[1].val[2])) 
                || ((dp.BDP < channel[0].val[0]) && (dp.BDP >= channel[1].val[0]))
                || ((dp.BDP < channel[0].val[2]) && (dp.BDP >= channel[0].val[0])))
            {
                UB = channel[0].tvhs[2]; // UB is RC38 TVHS
                UBtxt = "UB: rc38tvhs";
                UBtvls = channel[0].tvls[2];
            }

            if (((dp.WDP >= channel[0].val[0]) && (dp.WDP < channel[0].val[2]))// WDP lower 
                || ((dp.WDP < channel[0].val[0]) && (dp.WDP >= channel[1].val[0]))//WDP ICRR
                || ((dp.WDP < channel[1].val[0]) && (dp.WDP >= channel[1].val[2])))// WDP Knock-2 
            {
                LB = channel[1].tvls[2];//LB is always FC38 TVLS
                LBtxt = "LB: fc38tvls";
                LBTVHS = channel[1].tvhs[2];
            }
            //-------------------------------------------------------
            // 1st Check
            //-------------------------------------------------------

            //-------------------------------------------------------
            //In the following conditions UB is always RC38 TVHS
            //- if BDP higher gap i.e  BDP btw FC14 and FC38
            //- if BDP ICRR  i.e  BDP btw RC14 and FC14
            //- if BDP Knock - 2 i.e BDP RC38 and RC14 
            //-------------------------------------------------------
            if (((dp.WDP <= channel[1].val[0]) && (dp.WDP > channel[1].val[2])) 
                || ((dp.BDP < channel[0].val[0]) && (dp.BDP >= channel[1].val[0]))// BDP ICRR  i.e  BDP btw RC14 and FC14
                || ((dp.BDP < channel[0].val[2]) && (dp.BDP >= channel[0].val[0])))//BDP Knock - 2
            {
                UB = channel[0].tvhs[2]; // UB is RC38 TVHS
                UBtxt = "UB: rc38tvhs";
                UBtvls = channel[0].tvls[2];
            }

           
            if (((dp.WDP >= channel[0].val[0]) && (dp.WDP < channel[0].val[2]))// WDP lower gap
                || ((dp.WDP < channel[0].val[0]) && (dp.WDP >= channel[1].val[0]))//WDP ICRR 
                || ((dp.WDP < channel[1].val[0]) && (dp.WDP >= channel[1].val[2])))// WDP Knock-
            {
                LB = channel[1].tvls[2];//LB is always FC38 TVLS
                LBtxt = "LB: fc38tvls";
                LBTVHS = channel[1].tvhs[2];
            }

            //-----------------------------------------------------------------
            // BDP
            //-----------------------------------------------------------------
            //UB = channel[0].tvhs[3];//rc61tvhs   
            //BDP cut cut = UB RC61TVHS
            if ((dp.BDP < channel[0].val[3]) && (dp.BDP >= channel[0].val[2]))
            {
                UB = channel[0].tvhs[3];//rc61tvhs   
                //Text
                UBtxt = "UB: rc61tvhs";
                UBtvls = channel[0].tvls[3];
            }

            
            if (((dp.BDP < channel[0].val[4]) && (dp.BDP >= channel[0].val[3])) //BDP Cutting the flying kite
                || (dp.BDP > channel[0].val[4]))//BDP > RC100 abs 
            {
                
                if (dp.PatternName == "22")
                {
                    if ((dp.JWD >= channel[0].val[2]) && (dp.JWD <= channel[0].tvhs[3]))
                    {
                        UB = channel[0].tvhs[3];//then UB is RC61TVHS
                        UBtxt = "UB: rc61tvhs";
                        UBtvls = channel[0].tvls[3];
                    }
                }

                
                if (dp.PatternName == "31")
                {
                    if ((dp.JGD >= channel[0].val[2]) && (dp.JGD <= channel[0].tvhs[3]))
                    {
                        UB = channel[0].tvhs[3];//then UB is RC61TVHS
                        UBtxt = "UB: rc61tvhs";
                        UBtvls = channel[0].tvls[3];
                    }
                }

                
                if (dp.PatternName == "21")
                {
                    //double Value = Convert.ToDouble(dp.L1.Split(' ')[0]);
                    //Get String from L
                    string line = dp.L1;
                    if (!string.IsNullOrEmpty(line) && line.Contains(" "))
                    {
                        string[] values = line.Split(' ');
                        double Value1 = Convert.ToDouble(values[0]);
                        if ((Value1 >= channel[0].val[2]) && (Value1 <= channel[0].tvhs[3]))
                        {
                            UB = channel[0].tvhs[3];//then UB is RC61TVHS
                            UBtxt = "UB: rc61tvhs";
                            UBtvls = channel[0].tvls[3];
                        }
                    }


                }
            }


            //-----------------------------------------------------------------
            // WDP
            //-----------------------------------------------------------------

           
            //apply same 3 conditions as above
            if (((dp.WDP < channel[1].val[3]) && (dp.WDP >= channel[1].val[4])) //Catching the falling knife
                || (dp.WDP > channel[1].val[4]) //WDP > FC100 abs
                || (((dp.BDP <= channel[1].val[0]) && (dp.BDP > channel[1].val[2])) || ((dp.BDP < channel[0].val[0]) && (dp.BDP >= channel[1].val[0]))))// BDP is higher gap or BDP in ICRR for LB we need to check the following conditions
            {
                
                if (dp.PatternName == "22")
                {
                    if ((dp.JWD <= channel[1].val[2]) && (dp.JWD >= channel[1].tvls[3]))
                    {
                        LB = channel[1].tvls[3];//then LB is FC61TVHS} 
                        LBtxt = " LB: fc61tvls";
                        LBTVHS = channel[1].tvhs[3];
                    }
                }
                
                if (dp.PatternName == "31")
                {
                    if ((dp.JGD <= channel[1].val[2]) && (dp.JGD >= channel[1].tvls[3]))
                    {
                        LB = channel[1].tvls[3];//then LB is fC61TVHS}
                        LBtxt = " LB: fc61tvls";
                        LBTVHS = channel[1].tvhs[3];
                    }
                }
                
                if (dp.PatternName == "21")
                {
                    string line = dp.L1;
                    if (!string.IsNullOrEmpty(line) || line.Contains("  "))
                    {
                        string[] values = line.Split(' ');
                        double Value1 = Convert.ToDouble(values[0]);
                        if ((Value1 <= channel[1].val[2]) && (Value1 >= channel[1].tvls[3]))
                        {
                            LB = channel[1].tvls[3];//then LB is FC61TVHS
                            LBtxt = " LB: fc61tvls";
                            LBTVHS = channel[1].tvhs[3];
                        }
                    }



                }


            }

            //IF WDP cut cut then LB FC61TVLS
            if ((dp.WDP >= channel[1].tvhs[3]) && (dp.WDP < channel[1].tvls[2]))
            {
                LB = channel[1].tvls[3];
                LBtxt = " LB: fc61tvls";
                LBTVHS = channel[1].tvhs[3];
            }


            

            UBLB[0] = UB;
            UBLB[1] = UBtvls;
            UBLB[2] = LB;
            UBLB[3] = LBTVHS;
            strChannelUB = UBtxt ;
            strChannelLB = LBtxt ;


            return UBLB;
        }



        public int GetPrevDPLeg(clsNiku.HistoryData hd)
        {
            int PrevDPLeg = 0;
            PrevDPLeg = Convert.ToInt16(hd.prev.Dpleg);
            return PrevDPLeg;
        }

        //public clsNiku.DirectorPattern GetPrevDP(TimeFrame p, string script, DateTime dt)
        //{
        //    clsNiku.DirectorPattern PrevDP = new clsNiku.DirectorPattern();
        //    clsNiku ck = new clsNiku();
        //    clsSql clsc = new clsSql();
        //    clsc.connect();
        //    string qr = clsNiku.getScriptQS(p, script);
        //    qr += " where ddate < '" + dt.ToString("yyyy-MM-dd") + "' order by ddate desc limit 1;";
        //    SqlDataReader dr = clsc.getdr(qr);
        //    while (dr.Read())
        //    {
        //        clsNiku.HistoryData hd = ck.getHistoryData(script, Convert.ToDateTime(dr["ddate"]), p);
        //        PrevDP = ck.getDirectorPattern(hd);
        //    }
        //    clsc.disconnect();
        //    return PrevDP;
        //}

        public clsNiku.DirectorPattern GetPrevDP(clsNiku.HistoryData hd)
        {
            clsNiku.DirectorPattern PrevDP = new clsNiku.DirectorPattern();
            clsNiku ck = new clsNiku();

            PrevDP = ck.getDirectorPattern(hd);

            return PrevDP;
        }

        //GetOpenAs Value ------------------------------------
        public string GetOpenAs(double H, double L, double C, clsNiku.channel[] channel)
        {

            string OpenAsValue = "";
            clsNiku clsn = new clsNiku();

            //--------------------
            //ICRC ICFC
            //--------------------
            if ((H > channel[0].tvhs[0]) && (H < UB)) OpenAsValue = "ICRC";  
            if ((L < channel[1].tvls[0]) && (L > LB)) OpenAsValue = "ICFC"; 

            //if both breaks... take first breaker
            if ((H > channel[0].tvhs[0]) && (L < channel[1].tvls[0]))
            {
                //take which breaks first
                //condition to be applied for LIVE
            }

            //--------------------
            //RMSL HIGH/LOW
            //--------------------
            if ((H > channel[0].tvhs[0]) && (H > UB)) OpenAsValue = "RMSL HIGH";
            if ((L < channel[1].tvls[0]) && (L < LB)) OpenAsValue = "RMSL LOW";

            //--------------------------
            //If both RMSL HIGH & LOW... 
            //--------------------------
            if ((H > channel[0].tvhs[0]) && (H > UB) && (L < channel[1].tvls[0]) && (L < LB))
            {
               
                double d1 = Math.Abs(H - C);
                double d2 = Math.Abs(L - C);
                if (d1 < d2) OpenAsValue = "RMSL HIGH";
                else
                    OpenAsValue = "RMSL LOW";
                //--------------------------------------------------------                
            }

            //-----------------------
            //RMSL high close below
            //-----------------------
            if ((H > channel[0].tvhs[0]) && (H > UB) && (C < UBtvls))
                OpenAsValue = "RMSL High Close Below";

            //-----------------------
            //RMSL Low close Above
            //-----------------------
            if ((L < channel[1].tvls[0]) && (L < LB) && (C > LBTVHS))
                OpenAsValue = "RMSL LOW Close Above";


            //if ((H < channel[0].tvhs[0]) && (H > channel[1].tvls[9]) && (L < channel[0].tvhs[0]) && (L > channel[1].tvls[9])) OpenAsValue = "ICRR"; 

            return OpenAsValue;
        }

        //public string GetOpenAs(double H, double L, TimeFrame p, clsNiku.HistoryData hd)
        //{
        //    string OpenAsValue = "";
        //    clsNiku clsn = new clsNiku();
        //    clsNiku.channel[] channel = clsn.getChannels(hd, p);//Channel Array---------------------

        //    //When prices are rising (Open As)------------
        //    if ((H > channel[0].tvhs[0]) && (H < channel[0].tvhs[2])) { OpenAsValue = "ICRC"; }
        //    else if (H > channel[0].tvhs[2]) { OpenAsValue = "RMSL HIGH"; }
        //    else if ((L > channel[1].tvls[9]) && (L < channel[1].tvls[7])) { OpenAsValue = "ICFC"; }
        //    else if (L > channel[1].tvls[7]) { OpenAsValue = "RMSL LOW"; }
        //    else if ((H < channel[0].tvhs[0]) && (H > channel[1].tvls[9]) && (L < channel[0].tvhs[0]) && (L > channel[1].tvls[9])) { OpenAsValue = "ICRR"; }

        //    return OpenAsValue;
        //}



        public string GetTrend(double WDP, double BDP, clsNiku.channel[] channel)
        {
            string TrendAsValue = "";
            clsNiku clsn = new clsNiku();
            List<string> theoriesList = new List<string>();

            if ((BDP >= channel[1].val[2]) && (BDP < channel[0].val[2]) && (WDP >= channel[1].val[2]) && (WDP < channel[0].val[2])) { TrendAsValue = "Both"; }
            else if ((BDP < channel[1].val[2] || BDP > channel[0].val[2]) && (WDP < channel[1].val[2] || WDP > channel[0].val[2])) { TrendAsValue = "Both"; }
            else if ((BDP >= channel[1].val[2]) && (BDP < channel[0].val[2]) && (WDP > channel[0].val[2] || WDP < channel[1].val[2])) { TrendAsValue = "Up"; }
            else if ((WDP >= channel[1].val[2]) && (WDP < channel[0].val[2]) && (BDP > channel[0].val[2] || BDP < channel[1].val[2])) { TrendAsValue = "Down"; }

            return TrendAsValue;
        }



        ////-------------------------------------------------------------------------
        //// Get Opening Periods OF Date
        ////-------------------------------------------------------------------------
        //public DateTime getOpening(string script, DateTime sDt, DateTime eDt, string instr_id, clsNiku.OHLCdp o, TimeFrame tf, clsNiku.channel[] channel)
        //{
        //    //Getting All ohlc Data
        //    clsNiku cls = new clsNiku();
        //    List<clsNiku.OHLCdp> ohlcAllData = cls.getOHLCRangeData(script, sDt, eDt, TimeFrame.M15, instr_id, 3, true, true);
        //    //Getting OHLC Data
        //    DateTime DtOpening = new DateTime();
        //    //If History Data is Not Nulll
        //    if (o != null && channel != null)
        //    {
        //        //-------------------------------------------------------------
        //        // TimeFrame is 60
        //        //-------------------------------------------------------------
        //        if (tf == TimeFrame.M60)
        //        {
        //            ohlcAllData = ohlcAllData.Where(x => x.Date.Date == o.Date.Date).OrderBy(x=>x.Date).ToList();
        //            //D	9:15:00 AM - 9:30 (RC14TVHS on upside shud break / FC14TVLS shud break on downside)
        //            //if((o.Date.Hour==9 && o.Date.Minute>=15) && (o.Date.Hour == 9 && o.Date.Minute <= 30))
        //            //{
        //            for (int i = 0; i < ohlcAllData.Count(); i++)
        //            {
        //                clsNiku.HistoryData hd = new clsNiku.HistoryData();
        //                hd.current = o;
        //                if ((i + 2) >= ohlcAllData.Count) continue;
        //                hd.prev = ohlcAllData[i + 1];
        //                hd.prev2 = ohlcAllData[i + 2];
        //                clsNiku.channel[] channels = cls.getChannels(hd, tf);
        //                if ((ohlcAllData[i].High > channels[0].tvhs[0]) && (ohlcAllData[i].Low < channels[1].tvls[0]))
        //                {
        //                    DtOpening = ohlcAllData[i].Date;
        //                    break;

        //                }

        //            }
        //        }

        //        //-------------------------------------------------------------
        //        //TimeFrame is  Daily 
        //        //-------------------------------------------------------------
        //        if (tf == TimeFrame.Daily)
        //        {

        //            ohlcAllData = ohlcAllData.Where(x => x.Date.Date == o.Date.Date).OrderBy(x=>x.Date).ToList();
        //            //D	9:15:00 AM - 9:30 (RC14TVHS on upside shud break / FC14TVLS shud break on downside)
        //            //if((o.Date.Hour==9 && o.Date.Minute>=15) && (o.Date.Hour == 9 && o.Date.Minute <= 30))
        //            //{
        //            for (int i = 0; i < ohlcAllData.Count(); i++)
        //            {
        //                clsNiku.HistoryData hd = new clsNiku.HistoryData();
        //                hd.current = o;
        //                if ((i + 2) >= ohlcAllData.Count) continue;
        //                hd.prev = ohlcAllData[i + 1];
        //                hd.prev2 = ohlcAllData[i + 2];
        //                clsNiku.channel[] channels = cls.getChannels(hd, tf);
        //                if ((ohlcAllData[i].High > channels[0].tvhs[0]) && (ohlcAllData[i].Low < channels[1].tvls[0]))
        //                {
        //                    //if (ohlcAllData[i].Date.Hour==9 && (ohlcAllData[i].Date.Minute>=15 || ohlcAllData[i].Date.Minute <= 30))
        //                    //{
        //                    DtOpening = ohlcAllData[i].Date;
        //                    break;
        //                    //}
        //                }
        //            }

        //        }

        //        //---------------------------------------------------------------
        //        //Weekly
        //        //---------------------------------------------------------------
        //        if (tf == TimeFrame.Weekly)
        //        {

        //            DateTime dtOhlc = o.Date;
        //            //get Difference from date to Monday
        //            int diff = (7 + (dtOhlc.DayOfWeek - DayOfWeek.Monday)) % 7;
        //            DateTime dtMonday = dtOhlc.AddDays(-1 * diff);
        //            ohlcAllData = ohlcAllData.Where(x => x.Date.Date <= o.Date.Date && x.Date.Date >= dtMonday.Date).OrderBy(x => x.Date).ToList();
        //            int i = 0;
        //            foreach (clsOHLC oD in ohlcAllData)
        //            {
        //                clsNiku.HistoryData hd = new clsNiku.HistoryData();
        //                hd.current = o;
        //                if ((i + 2) >= ohlcAllData.Count) continue;
        //                hd.prev = ohlcAllData[i + 1];
        //                hd.prev2 = ohlcAllData[i + 2];
        //                clsNiku.channel[] channels = cls.getChannels(hd, tf);
        //                if ((oD.High > channels[0].tvhs[0]) && (oD.Low < channels[1].tvls[0]))
        //                {
        //                    DtOpening = oD.Date;
        //                    break;
        //                }
        //                i++;
        //            }

        //            //W	Monday	 (1st working day of the week)	

        //        }

        //        //-----------------------------------------------------------
        //        // Monthly
        //        //-----------------------------------------------------------
        //        if (tf == TimeFrame.Monthly)
        //        {
        //            //M	Check opening breaks till Friday in which minimum 3 days working shud be there upto Friday else will consider next week till Friday		
        //            DateTime dtmonth = new DateTime(o.Date.Year, o.Date.Month, 1);
        //            DateTime dtmonthEnd = dtmonth.AddMonths(1).AddDays(-1);
        //            ohlcAllData = ohlcAllData = ohlcAllData.Where(x => x.Date.Date <= dtmonthEnd.Date && x.Date.Date >= dtmonth.Date).OrderBy(x => x.Date).ToList();
        //            int i = 0;
        //            foreach (clsOHLC oD in ohlcAllData)
        //            {
        //                clsNiku.HistoryData hd = new clsNiku.HistoryData();
        //                hd.current = o;
        //                if ((i + 2) >= ohlcAllData.Count) continue;
        //                hd.prev = ohlcAllData[i + 1];
        //                hd.prev2 = ohlcAllData[i + 2];
        //                clsNiku.channel[] channels = cls.getChannels(hd, tf);
        //                if ((oD.High > channels[0].tvhs[0]) && (oD.Low < channels[1].tvls[0]))
        //                {
        //                    DtOpening = oD.Date;
        //                    break;
        //                }
        //            }
        //        }
        //        //-----------------------------------------------------------
        //        // Quaterly
        //        //-----------------------------------------------------------
        //        if (tf == TimeFrame.Quarterly)
        //        {
        //            //Quaterly	minimum count working 13 days upto Friday		
        //            //M	Check opening breaks till Friday in which minimum 3 days working shud be there upto Friday else will consider next week till Friday		
        //            //Getting Range of Quater
        //            DateTime dtQtrStart = new DateTime(o.Date.Year, o.Date.Month, 1);
        //            DateTime dtQtrEnd = dtQtrStart.AddMonths(4).AddDays(-1);
        //            ohlcAllData = ohlcAllData = ohlcAllData.Where(x => x.Date.Date <= dtQtrEnd.Date && x.Date.Date >= dtQtrStart.Date).OrderBy(x => x.Date).ToList();
        //            int i = 0;
        //            foreach (clsOHLC oD in ohlcAllData)
        //            {
        //                clsNiku.HistoryData hd = new clsNiku.HistoryData();
        //                hd.current = o;
        //                if ((i + 2) >= ohlcAllData.Count) continue;
        //                hd.prev = ohlcAllData[i + 1];
        //                hd.prev2 = ohlcAllData[i + 2];
        //                clsNiku.channel[] channels = cls.getChannels(hd, tf);
        //                if ((oD.High > channels[0].tvhs[0]) && (oD.Low < channels[1].tvls[0]))
        //                {
        //                    DtOpening = oD.Date;
        //                    break;
        //                }
        //            }
        //        }


        //        //-----------------------------------------------------------
        //        // Half-Yearly
        //        //-----------------------------------------------------------
        //        if (tf == TimeFrame.HalfYearly)
        //        {
        //            //HY	minimum working 21 days upto Friday		
        //            //Getting Range of Half year
        //            DateTime dtQtrStart = new DateTime(o.Date.Year, o.Date.Month, 1);
        //            DateTime dtQtrEnd = dtQtrStart.AddMonths(6).AddDays(-1);
        //            ohlcAllData = ohlcAllData = ohlcAllData.Where(x => x.Date.Date <= dtQtrEnd.Date && x.Date.Date >= dtQtrStart.Date).OrderBy(x => x.Date).ToList();
        //            int i = 0;
        //            foreach (clsOHLC oD in ohlcAllData)
        //            {
        //                clsNiku.HistoryData hd = new clsNiku.HistoryData();
        //                hd.current = o;
        //                if ((i + 2) >= ohlcAllData.Count) continue;
        //                hd.prev = ohlcAllData[i + 1];
        //                hd.prev2 = ohlcAllData[i + 2];

        //                clsNiku.channel[] channels = cls.getChannels(hd, tf);
        //                if ((oD.High > channels[0].tvhs[0]) && (oD.Low < channels[1].tvls[0]))
        //                {
        //                    DtOpening = oD.Date;
        //                    break;
        //                }
        //            }

        //        }

        //        //-----------------------------------------------------------
        //        // Yearly
        //        //-----------------------------------------------------------
        //        if (tf == TimeFrame.Yearly)
        //        {
        //            //Get list acc to Daily Time Frame

        //            //Getting Range of year
        //            DateTime dtQtrStart = new DateTime(o.Date.Year, o.Date.Month, 1);
        //            DateTime dtQtrEnd = dtQtrStart.AddMonths(12).AddDays(-1);
        //            ohlcAllData = ohlcAllData = ohlcAllData.Where(x => x.Date.Date <= dtQtrEnd.Date && x.Date.Date >= dtQtrStart.Date).OrderBy(x => x.Date).ToList();
        //            int i = 0;
        //            //Y	minimum working 21 days upto Friday		
        //            foreach (clsOHLC oD in ohlcAllData)
        //            {
        //                clsNiku.HistoryData hd = new clsNiku.HistoryData();
        //                hd.current = o;
        //                if ((i + 2) >= ohlcAllData.Count) continue;
        //                hd.prev = ohlcAllData[i + 1];
        //                hd.prev2 = ohlcAllData[i + 2];
        //                clsNiku.channel[] channels = cls.getChannels(hd, tf);
        //                if ((oD.High > channels[0].tvhs[0]) && (oD.Low < channels[1].tvls[0]))
        //                {
        //                    DtOpening = oD.Date;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return DtOpening;
        //}

        
        string OPens = string.Empty;
        DateTime dtOPenResult = new DateTime();
        public string[] getOpeningDateAndOpenAS(clsNiku.channel[] channel, string instrID, double UB, double LB, DateTime dtOpening, TimeFrame tf, bool blnNextDataSegment = false)
        {
            OPens = string.Empty;
            dtOPenResult = new DateTime();
            //Resulting Array
            string[] strArrOpenRes=new string[2]; 
            clsdata data = new clsdata();
            
            
            //------------------------------------------------------
            // For Setting Date Time According to Given DateTime
            //------------------------------------------------------
            DateTime dtOPenS = new DateTime();
            DateTime dtOPenEnd = new DateTime();
            //--------------------------------------------------------
            // For Gettting Monday Time Period
            //--------------------------------------------------------
            clsNiku cls = new clsNiku();
            List<clsNiku.OHLCdp> ohlclst = new List<clsNiku.OHLCdp>();

            //Getting Instrument by Symbol or ScriptCode
            Instruments instr = clsGenfx.liveFeedInstr.Where(x => x.Symbol == instrID || x.ScriptCode == instrID).FirstOrDefault();

            if (instr != null)
            {
                //--------------------------------------------------------------
                // Date Range For Time Frame Daily
                //--------------------------------------------------------------
                if (tf == TimeFrame.Daily)
                {
                    dtOPenS = new DateTime(dtOpening.Year, dtOpening.Month, dtOpening.Day, 9, 00, 0);
                    dtOPenEnd = new DateTime(dtOpening.Year, dtOpening.Month, dtOpening.Day, 9, 31, 00);

                    if(blnNextDataSegment==true)
                    {
                        dtOPenS=dtOpening;
                        dtOPenEnd=dtOpening.AddMinutes(30);
                    }

                    //---------------------------------------
                    // Calling OHLC data According to Brokers
                    //---------------------------------------
                    //if (clsGenfx.livefeed == FeedAPI.GlobalDatafeed)
                    //{
                    //    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3, false, false, true);
                    //    //IF GlobalDataFeed Returns zero List
                    //    if (ohlclst.Count() <= 0)
                    //    {
                    //        //Calling Data from Other Brokers
                          ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode);
                    //    }
                    //}
                    //else ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3);
                }
                //--------------------------------------------------------------
                // Date Range For Time Frame Weekly
                //--------------------------------------------------------------
                else if (tf == TimeFrame.Weekly)
                {
                    dtOPenEnd = new DateTime(dtOpening.Year, dtOpening.Month, dtOpening.Day, 23, 59, 59);
                    dtOPenS = data.getLastMonday(dtOpening);
                    //---------------------------------------
                    // Calling OHLC data According to Brokers
                    //---------------------------------------
                    //if (clsGenfx.livefeed == FeedAPI.GlobalDatafeed)
                    //{
                    //    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3, false, false, true);
                    //    //IF GlobalDataFeed Returns zero List
                    //    if (ohlclst.Count() <= 0)
                    //    {
                            //Calling Data from Other Brokers
                            ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode);
                    //    }
                    //}
                    //else ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3);
                }
                //--------------------------------------------------------------
                // Date Range For Time Frame Weekly
                //--------------------------------------------------------------
                else if (tf == TimeFrame.Monthly)
                {
                    dtOPenS = new DateTime(dtOpening.Year, dtOpening.Month, 1, 9, 15, 0);
                    dtOPenEnd = dtOPenS.AddMonths(1).AddDays(-1);
                    //---------------------------------------
                    // Calling OHLC data According to Brokers
                    //---------------------------------------
                    //if (clsGenfx.livefeed == FeedAPI.GlobalDatafeed)
                    //{
                    //    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.Daily, instr.ScriptCode, 3, false, false, true);
                    //    //IF GlobalDataFeed Returns zero List
                    //    if (ohlclst.Count() <= 0)
                    //    {
                            //Calling Data from Other Brokers
                            ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.Daily, instr.ScriptCode);
                    //    }
                    //}
                    //else ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.Daily, instr.ScriptCode, 3);
                }

                //Sorting OHLC According to Date
                ohlclst=ohlclst.OrderBy(x=>x.Date).ToList();

                //Variable to Store High and Low from OHLC List
                double High = 0.0, Low = 0.0;


                if (ohlclst.Count() > 0)
                {
                    //Getting Highest High from Ohlc List
                    High = ohlclst.OrderBy(x=>x.Date).Max(x => x.High);

                    //Getting Lowwest Low from OHLC list
                    Low = ohlclst.OrderBy(x=>x.Date).Min(x => x.Low);

                    //--------------------------------------------------------------
                    // Getting Opens Value Based on High, Low, UB ,LB and Channels
                    //--------------------------------------------------------------
                    //---------------------
                    // ICRC
                    //---------------------
                    if (High > channel[0].tvhs[0]) //(ohlclst.Any(x=>x.High > channel[0].tvhs[0])) 
                    {
                        OPens = "ICRC";
                        dtOPenResult=ohlclst.Where(x=>x.High> channel[0].tvhs[0]).Select(x=>x.Date).FirstOrDefault();
                    }

                    //----------------------
                    // RMSL HIGHER
                    //----------------------
                    if(High > UB && OPens == "ICRC") //(ohlclst.Any(x=>x.High>UB)&& OPens == "ICRC") 
                    {
                        OPens = "RMSL HIGHER";
                        dtOPenResult = ohlclst.Where(x => x.High > UB).Select(x => x.Date).FirstOrDefault();
                    }


                    //--------------------------
                    // Condition for ICFC 
                    //--------------------------

                    if (Low < channel[1].tvls[0] && string.IsNullOrEmpty(OPens)/*&&(OPens=="ICRC"|| OPens == "RMSL HIGHER")*/) //(ohlclst.Any(x=>x.Low < channel[1].tvls[0]))// 
                    {
                        OPens = "ICFC";
                        dtOPenResult = ohlclst.Where(x => x.Low < channel[1].tvls[0]).Select(x => x.Date).FirstOrDefault();
                    }

                    //-----------------------------
                    // Condition for RMSL LOWWER
                    //-----------------------------
                    if (Low < LB && OPens == "ICFC") //(ohlclst.Any(x=>x.Low<LB) && OPens == "ICFC") //
                    {
                        OPens = "RMSL LOWER";
                        dtOPenResult = ohlclst.Where(x => x.Low < LB).Select(x => x.Date).FirstOrDefault();
                    }

                    //------------------------------
                    // Condition for ICRR
                    //------------------------------
                    //
                    if (High < channel[0].tvls[0] && Low > channel[1].tvls[0] && string.IsNullOrEmpty(OPens) /*&& string.IsNullOrEmpty(OPens)*/) //(ohlclst.Any(x=>x.High < channel[0].tvhs[0] && x.Low > channel[1].tvls[0])) // 
                    {
                        OPens = "ICRR";
                        dtOPenResult = ohlclst.Where(x => x.Low == Low || x.High==High).Select(x => x.Date).FirstOrDefault();
                    } 

                    // any Condition not match 
                    // recursively calling function again to get next set of ohlc 
                    // Till we get Answer
                    if(string.IsNullOrEmpty(OPens))
                    {
                        //Next Date Time
                        DateTime dtNextDate = new DateTime(dtOPenEnd.Year, dtOPenEnd.Month, dtOPenEnd.Day, dtOPenEnd.Hour, dtOPenEnd.Minute, 0);
                        getOpeningDateAndOpenAS(channel, instrID, UB, LB, dtNextDate, tf, true);
                    }
                }
            }

            strArrOpenRes[0] = OPens;//Opening Result
            strArrOpenRes[1]=dtOPenResult.ToString();

            //-------------------------------------------
            //Returning Results
            //-------------------------------------------
            return strArrOpenRes;
        }



        //----------------------------------------------------------------
        //Open As Conditions(New)
        //----------------------------------------------------------------
        /// <summary>
        /// Confilicting Dates
        /// Nifty 50 : 9-jan-2023,24-feb-23,28-feb-23,15-mar-23,17-mar-23,17-apr-23,20-apr-23,25-apr-23,27-apr-23,2-may-23,4-may-23,15-may-23,18-may-23,6-jun-23, 22-jun-23,5-jul-23,6-jul-23                
        /// </summary>
        public bool blnCheckICRC=false;
        public bool blnCheckICFC=false;
        public string openingFlow = string.Empty;
        public string[] OpeningPeriod(clsNiku.channel[] channel, string instrID, double UB, double LB, DateTime dtOpening, TimeFrame tf, bool blnICRC = false, bool blnICFC = false, bool blnNextDataSegment = false)
        {
            string[] OpenValues=new string[2];
            string OPens = string.Empty;
            //for flow condition
            string OpensFlow = string.Empty;
            //For Getting Dates Related Methods
            clsdata data = new clsdata();
            //--------------------------------------------------------------------
            // Opens Condition
            //--------------------------------------------------------------------
            //Steps:Checking Opening Time According to Provided time frame
            //cond: tf is Weekly (Opening should be from  Monday to Friday
            //********************if  it is not open from monday to friday it will be ICRRR*************************

            //------------------------------------------------------
            // For Setting Date Time According to Given DateTime
            //------------------------------------------------------
            DateTime dtOPenS = new DateTime();
            DateTime dtOPenEnd = new DateTime();
            //--------------------------------------------------------
            // For Gettting Monday Time Period
            //--------------------------------------------------------
            clsNiku cls = new clsNiku();
            List<clsNiku.OHLCdp> ohlclst = new List<clsNiku.OHLCdp>();

            //Getting Instrument by Symbol or ScriptCode
            Instruments instr = clsGenfx.liveFeedInstr.Where(x => x.Symbol == instrID || x.ScriptCode==instrID).FirstOrDefault();
            if (instr != null)
            {
                //--------------------------------------------------------------
                // Date Range For Time Frame Daily
                //--------------------------------------------------------------
                if (tf == TimeFrame.Daily)
                {

                    if (blnNextDataSegment == true)
                    {
                        dtOPenS = dtOpening;
                        dtOPenEnd = dtOpening.AddMinutes(30);
                    }
                    else
                    {
                        //dtOPenS = new DateTime(dtOpening.Year, dtOpening.Month, dtOpening.Day, 9, 15, 0);
                        dtOPenS = new DateTime(dtOpening.Year, dtOpening.Month, dtOpening.Day, 9, 0, 0);
                        dtOPenEnd = new DateTime(dtOpening.Year, dtOpening.Month, dtOpening.Day, 9, 30, 59);
                    }


                    //---------------------------------------
                    // Calling OHLC data According to Brokers
                    //---------------------------------------
                    //if (clsGenfx.livefeed == FeedAPI.GlobalDatafeed)
                    //{
                    //    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3, false, false, true);
                    //    //IF GlobalDataFeed Returns zero List
                    //    if (ohlclst.Count() <= 0)
                    //    {
                    //        //Calling Data from Other Brokers
                    //        ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3);
                    //    }
                    //}
                    //else 
                    //ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode);
                    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M15, instr.ScriptCode);
                }
                //--------------------------------------------------------------
                // Date Range For Time Frame Weekly
                //--------------------------------------------------------------
                else if (tf == TimeFrame.Weekly)
                {
                    dtOPenEnd = new DateTime(dtOpening.Year, dtOpening.Month, dtOpening.Day, 23, 59, 59);
                    dtOPenS = data.getLastMonday(dtOpening);
                    //---------------------------------------
                    // Calling OHLC data According to Brokers
                    //---------------------------------------
                    //if (clsGenfx.livefeed == FeedAPI.GlobalDatafeed)
                    //{
                    //    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3, false, false, true);
                    //    //IF GlobalDataFeed Returns zero List
                    //    if (ohlclst.Count() <= 0)
                    //    {
                    //        //Calling Data from Other Brokers
                    //        ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode, 3);
                    //    }
                    //}
                    //else
                    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.M1, instr.ScriptCode);
                }
                //--------------------------------------------------------------
                // Date Range For Time Frame Weekly
                //--------------------------------------------------------------
                else if (tf == TimeFrame.Monthly)
                {
                    dtOPenS = new DateTime(dtOpening.Year, dtOpening.Month, 1, 9, 15, 0);
                    dtOPenEnd = dtOPenS.AddMonths(1).AddDays(-1);
                    //---------------------------------------
                    // Calling OHLC data According to Brokers
                    //---------------------------------------
                    //if (clsGenfx.livefeed == FeedAPI.GlobalDatafeed)
                    //{
                    //    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.Daily, instr.ScriptCode, 3, false, false, true);
                    //    //IF GlobalDataFeed Returns zero List
                    //    if (ohlclst.Count() <= 0)
                    //    {
                    //        //Calling Data from Other Brokers
                    //        ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.Daily, instr.ScriptCode, 3);
                    //    }
                    //}
                    //else
                    ohlclst = cls.getOHLCRangeData(instr.Symbol, dtOPenS, dtOPenEnd, TimeFrame.Daily, instr.ScriptCode);
                }

                //Variable to Store High and Low from OHLC List
                double High = 0.0, Low = 0.0;
                if (ohlclst.Count() > 0)
                {
                    //Getting Highest High from Ohlc List
                     High = ohlclst.Max(x => x.High);

                    //Getting Lowwest Low from OHLC list
                     Low = ohlclst.Min(x => x.Low);

                    //--------------------------------------------------------------
                    // Getting Opens Value Based on High, Low, UB ,LB and Channels
                    //--------------------------------------------------------------
                  
                    if (High > channel[0].tvhs[0]) { OPens = "ICRC"; blnCheckICRC = true; blnCheckICFC = false; dtOPenResult = ohlclst.Where(x => x.High > channel[0].tvhs[0]).Select(x => x.Date).FirstOrDefault(); }

                  
                    if (High > UB && OPens == "ICRC") { OPens = "RMSL HIGHER"; blnCheckICRC = true; blnCheckICFC = false; dtOPenResult = ohlclst.Where(x => x.High > UB).Select(x => x.Date).FirstOrDefault(); }

                 
                    if (Low < channel[1].tvls[0] && string.IsNullOrEmpty(OPens)) { OPens = "ICFC"; blnCheckICRC = false; blnCheckICFC = true; dtOPenResult = ohlclst.Where(x => x.Low < channel[1].tvls[0]).Select(x => x.Date).FirstOrDefault(); }

                  
                    if (Low < LB && OPens == "ICFC") { OPens = "RMSL LOWER"; blnCheckICRC = false; blnCheckICFC = true; dtOPenResult = ohlclst.Where(x => x.Low < LB).Select(x => x.Date).FirstOrDefault(); }

                   
                    if (High < channel[0].tvhs[0] && Low > channel[1].tvls[0] && string.IsNullOrEmpty(OPens)) 
                    { 
                        OPens = "ICRR";
                        dtOPenResult = ohlclst.Where(x => x.Low == Low || x.High == High).Select(x => x.Date).FirstOrDefault();
                        if (blnCheckICFC) OPens += " to ICFC";
                        if (blnCheckICRC) OPens += " to ICRC";

                        blnCheckICRC = false;
                        blnCheckICFC = false;
                    }

                    if (string.IsNullOrEmpty(OPens))
                    {
                        //Next Date Time
                        DateTime dtNextDate = new DateTime(dtOPenEnd.Year, dtOPenEnd.Month, dtOPenEnd.Day, dtOPenEnd.Hour, dtOPenEnd.Minute, 0);
                        OpeningPeriod(channel, instrID, UB, LB, dtNextDate, tf, blnICRC, blnICFC, blnNextDataSegment: true);
                    }


                }
            }
            //-------------------------------------------
            //Returning Results
            //-------------------------------------------
            OpenValues[0] = OPens;
            OpenValues[1] = dtOpening.ToString();

            return OpenValues;
        }
    }
}
