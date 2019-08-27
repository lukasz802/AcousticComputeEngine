using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compute_Engine
{
    public static class UnitConvertion
    {
        public static double CMHToCFM(double x)
        {
            //m3/h na ft3/min
            return x * 0.588578;
        }

        public static double PaToInH20(double x)
        {
            //Pa na cale H2O
            return x / (2.54 * 98.1);
        }

        public static double MToFt(double x)
        {
            //m na ft
            return x * 3.2808399;
        }

        public static double KgTolb(double x)
        //kg na lb (funty)
        {
            return x / 0.45359237;
        }
    }
}
