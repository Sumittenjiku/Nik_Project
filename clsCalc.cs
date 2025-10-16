using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niku___Master_Code
{
    public class clsCalc
    {
        public static double calcPercentageChange(double close, double prevclose)
        {
            double p1 = close - prevclose;
            p1 = p1 / close * 100.0;
            return p1;
        }
        public static decimal calcPercentageChange(decimal close, decimal prevclose)
        {
            decimal p1 = close - prevclose;
            try { p1 = p1 / close * 100m; } catch(Exception ex) { Debug.WriteLine(ex.Message); }
            
            return p1;
        }

        public static double calcRange(double high, double low)
        {
            return high - low;
        }
    }
}
