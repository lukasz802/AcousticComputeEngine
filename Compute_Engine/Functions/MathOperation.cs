using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compute_Engine
{
    public static class MathOperation
    {
        public static double DecibelAdd(double db1, double db2)
        {
            return 10 * Math.Log10(Math.Pow(10, (db1 / 10)) + Math.Pow(10, (db2 / 10)));
        }

        public static double[] OctaveAdd(double[] db1, double[] db2)
        {
            double[] val = new double[8];

            for (int i = 0; i < val.Length; i++)
            {
                val[i] = db1[i] + db2[i];
            }
            return val;
        }

        public static double[] OctaveSubstract(double[] db1, double[] db2)
        {
            double[] val = new double[8];

            for (int i = 0; i < val.Length; i++)
            {
                val[i] = db1[i] - db2[i];
            }
            return val;
        }

        public static double[] OctaveDecibelAdd(double[] db1, double[] db2)
        {
            double[] val = new double[8];

            for (int i = 0; i < val.Length; i++)
            {
                val[i] = 10 * Math.Log10(Math.Pow(10, (db1[i] / 10)) + Math.Pow(10, (db2[i] / 10)));
            }
            return val;
        }

        public static double OctaveSum(params double[] oct)
        {
            double val = -1000;

            for (int i = 0; i < oct.Length; i++)
            {
                val = 10 * Math.Log10(Math.Pow(10, (val / 10)) + Math.Pow(10, (oct[i] / 10)));
            }
            return val;
        }
    }
}
