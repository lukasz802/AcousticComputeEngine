using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compute_Engine
{
    public static class Breakout
    {
        public static double[] DuctRectangular(double w, double h, double l, double q)
        {
            //w-szerokość kanału, m
            //h-wysokość kanału, m
            //l-długość kanału, m
            //q-gęstość powierzchniowa materiału z którego wykonano kanał, kg/m2
            double a, ao, fl, tlmin, q1, tl1, tl2;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            q1 = UnitConvertion.KgTolb(q) / (UnitConvertion.MToFt(1) * UnitConvertion.MToFt(1));
            a = (100 * h / 2.54) * (100 * w / 2.54);
            ao = 24 * UnitConvertion.MToFt(l) * ((100 * h / 2.54) + (100 * w / 2.54));
            fl = 24134 / Math.Pow(((100 * h / 2.54) * (100 * w / 2.54)), 0.5);
            tlmin = 10 * Math.Log10(24 * UnitConvertion.MToFt(l) * (1 / (100 * h / 2.54) + 1 / (100 * w / 2.54)));

            for (int i = 0; i < oct.Length; i++)
            {
                if ((10 * Math.Log10(oct[i] * Math.Pow(q1, 2) / ((100 * h / 2.54) + (100 * w / 2.54))) + 17) <= 45)
                {
                    tl1 = 10 * Math.Log10(oct[i] * Math.Pow(q1, 2) / ((100 * h / 2.54) + (100 * w / 2.54))) + 17;
                }
                else
                {
                    tl1 = 45.0;
                }

                if ((20 * Math.Log10(q1 * oct[i]) - 31) <= 45)
                {
                    tl2 = 20 * Math.Log10(q1 * oct[i]) - 31;
                }
                else
                {
                    tl2 = 45.0;
                }

                if (oct[i] < Noise.Oct_BFI(fl))
                {
                    if (tlmin < tl1)
                    {
                        attn[i] = tl1 - 10 * Math.Log10(ao / a);
                    }
                    else
                    {
                        attn[i] = tlmin - 10 * Math.Log10(ao / a);
                    }
                }
                else
                {
                    if (tlmin < tl2)
                    {
                        attn[i] = tl2 - 10 * Math.Log10(ao / a);
                    }
                    else
                    {
                        attn[i] = tlmin - 10 * Math.Log10(ao / a);
                    }
                }
            }
            return attn;
        }

        public static double[] DuctRound(double d, double l, double q)
        {
            //d-średnica kanału, m
            //l-długość kanału, m
            //q-gęstość powierzchniowa materiału z którego wykonano kanał, kg/m2
            double a, ao, tlout, q1, tl1, tl2;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            q1 = UnitConvertion.KgTolb(q) / (UnitConvertion.MToFt(1) * UnitConvertion.MToFt(1));
            a = Math.PI * Math.Pow((100 * d / 2.54), 2) * 0.25;
            ao = 12 * Math.PI * (100 * d / 2.54) * UnitConvertion.MToFt(l);

            for (int i = 0; i < oct.Length; i++)
            {
                tl1 = 17.6 * Math.Log10(q1) - 49.8 * Math.Log10(oct[i]) - 55.3 * Math.Log10(100 * d / 2.54) + 232.9;
                tl2 = 17.6 * Math.Log10(q1) - 6.6 * Math.Log10(oct[i]) - 36.9 * Math.Log10(100 * d / 2.54) + 97.4;

                if ((d * 100 / 2.54) < 26)
                {
                    tlout = Math.Min(Math.Max(tl1, tl2), 50);
                }
                else
                {
                    if (oct[i] >= 4000)
                    {
                        tlout = Math.Min(17.6 * Math.Log10(q1) - 6.6 * Math.Log10(oct[i]) - 36.9 * Math.Log10(100 * d / 2.54) + 114.4, 50);
                    }
                    else
                    {
                        tlout = Math.Min(Math.Max(tl1, tl2), 50);
                    }
                }

                attn[i] = tlout - 10 * Math.Log10(ao / a);
            }
            return attn;
        }
    }
}
