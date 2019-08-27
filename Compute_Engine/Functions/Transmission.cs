using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Enums;

namespace Compute_Engine
{
    public static class Transmission
    {
        public static double[] Ceiling(CeiligType ceiligType, CeilingConfiguration ceilingConfiguration)
        {
            double k63, k125, k250, k500, k1000, k2000, k4000, k8000;
            double[] kw = new double[8];
            double[] attn = new double[8];

            if (ceiligType == CeiligType.Gypboard_10mm)
            {
                k63 = 6.0;
                k125 = 11.0;
                k250 = 17.0;
                k500 = 22.0;
                k1000 = 28.0;
                k2000 = 32.0;
                k4000 = 24.0;
                k8000 = 16.0;
            }
            else if (ceiligType == CeiligType.Gypboard_12mm)
            {
                k63 = 9.0;
                k125 = 14.0;
                k250 = 20.0;
                k500 = 24.0;
                k1000 = 30.0;
                k2000 = 31.0;
                k4000 = 27.0;
                k8000 = 23.0;
            }
            else if (ceiligType == CeiligType.Gypboard_15mm)
            {
                k63 = 10.0;
                k125 = 15.0;
                k250 = 22.0;
                k500 = 26.0;
                k1000 = 31.0;
                k2000 = 28.0;
                k4000 = 30.0;
                k8000 = 32.0;
            }
            else if (ceiligType == CeiligType.Gypboard_25mm)
            {
                k63 = 13.0;
                k125 = 18.0;
                k250 = 26.0;
                k500 = 30.0;
                k1000 = 30.0;
                k2000 = 29.0;
                k4000 = 37.0;
                k8000 = 45.0;
            }
            else if (ceiligType == CeiligType.AcousticalCeilingTileExposedTBarGridSuspendedLight_60x120x10mm)
            {
                //sufit podwieszany na kostrukcji metalowej
                k63 = 4.0;
                k125 = 9.0;
                k250 = 9.0;
                k500 = 14.0;
                k1000 = 19.0;
                k2000 = 24.0;
                k4000 = 26.0;
                k8000 = 28.0;
            }
            else if (ceiligType == CeiligType.AcousticalCeilingTileExposedTBarGridSuspendedHeavy_60x120x10mm)
            {
                //sufit podwieszany na kostrukcji metalowej
                k63 = 5.0;
                k125 = 11.0;
                k250 = 13.0;
                k500 = 15.0;
                k1000 = 21.0;
                k2000 = 24.0;
                k4000 = 28.0;
                k8000 = 32.0;
            }
            else if (ceiligType == CeiligType.AcousticalCeilingTileExposedTBarGridSuspended_60x120x15mm)
            //sufit podwieszany na kostrukcji metalowej
            {
                k63 = 4.0;
                k125 = 9.0;
                k250 = 10.0;
                k500 = 11.0;
                k1000 = 15.0;
                k2000 = 20.0;
                k4000 = 25.0;
                k8000 = 30.0;
            }
            else if (ceiligType == CeiligType.AcousticalCeilingTileExposedTBarGridSuspended_60x60x10mm)
            {
                //sufit podwieszany na kostrukcji metalowej
                k63 = 5.0;
                k125 = 10.0;
                k250 = 11.0;
                k500 = 15.0;
                k1000 = 19.0;
                k2000 = 22.0;
                k4000 = 24.0;
                k8000 = 26.0;
            }
            else
            {
                //sufit podwieszany z konstrukcją ukrytą
                k63 = 6;
                k125 = 14;
                k250 = 14;
                k500 = 18;
                k1000 = 22;
                k2000 = 27;
                k4000 = 30;
                k8000 = 33;
            }

            kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };

            for (int i = 0; i < attn.Length; i++)
            {
                if (ceilingConfiguration == CeilingConfiguration.IntegratedLightingAndDiffuserSystem && Convert.ToInt16(ceiligType) <= 4)
                {
                    attn[i] = -10 * Math.Log10((1 - 0.001) * Math.Pow(10, (-kw[i] / 10)) + 0.001);
                }
                else if (ceilingConfiguration == CeilingConfiguration.IntegratedLightingAndDiffuserSystem && Convert.ToInt16(ceiligType) > 4)
                {
                    attn[i] = -10 * Math.Log10((1 - 0.03) * Math.Pow(10, (-kw[i] / 10)) + 0.03);
                }
                else if (ceilingConfiguration == CeilingConfiguration.IntegratedLightingAndDiffuserSystem && Convert.ToInt16(ceiligType) <= 4)
                {
                    attn[i] = -10 * Math.Log10((1 - 0.0001) * Math.Pow(10, (-kw[i] / 10)) + 0.0001);
                }
                else
                {
                    attn[i] = -10 * Math.Log10((1 - 0.001) * Math.Pow(10, (-kw[i] / 10)) + 0.001);
                }
            }
            return attn;
        }

        public static double[] PointCorrection(double t, double rh, double q, double r, double[] rm, double w, double l, double h)
        {
            //q-wpółczynnik kierunkowy,
            //w-szerokość powmieszczenia, m
            //l- długość pomieszczenia, m
            //h-wysokość pomieszczenia, m
            //r-odległość od źródła dźwięku, m
            //rm-chłonność akustyczna pomieszczenia, m2 Sabin
            double m, mfp, s, r1, rv;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] kw = new double[8];

            r1 = UnitConvertion.MToFt(r);
            s = 2 * (UnitConvertion.MToFt(w) * UnitConvertion.MToFt(l)) + 2 * (UnitConvertion.MToFt(l) * UnitConvertion.MToFt(h)) + 2 * (UnitConvertion.MToFt(w) * UnitConvertion.MToFt(h));
            mfp = 4 * (UnitConvertion.MToFt(w) * UnitConvertion.MToFt(l) * UnitConvertion.MToFt(h)) / s;

            for (int i = 0; i < oct.Length; i++)
            {
                m = M_coeff(t, rh)[i] / UnitConvertion.MToFt(1);
                rv = (s * (rm[i] + m * mfp)) / (1 - (rm[i] + m * mfp));
                kw[i] = -(10 * Math.Log10((q * Math.Exp(-m * r1)) / (4 * Math.PI * Math.Pow(r1, 2)) + (mfp / r1) * (4 / rv)) + 10.5);

                if (kw[i] < 0)
                {
                    kw[i] = 0.0;
                }
            }
            return kw;
        }

        public static double[] LinearCorrectionn(double q, double r, double[] rm, double l)
        {
            //q-wpółczynnik kierunkowy
            //r-odległość od źródła dźwięku, m
            //l-długość kanału, m
            //rm-chłonność akustyczna pomieszczenia, m2 Sabin
            double[] tl = new double[8];

            for (int i = 0; i < tl.Length; i++)
            {
                tl[i] = -(10 * Math.Log10(q / (2 * Math.PI * UnitConvertion.MToFt(r) * UnitConvertion.MToFt(l)) + (4 / rm[i])) + 10.5);
            }
            return tl;
        }

        internal static double[] M_coeff(double t, double fi)
        {
            //t-temperatura powietrza (od -20 do 50), st.C
            //fi-wilgotność względna powietrza, %
            double h, tk, frN, frO, twzg;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] kw = new double[8];
            tk = 273.15 + t;
            twzg = tk / 293.15;
            h = fi * Math.Pow(10, (-6.8346 * Math.Pow((273.16 / tk), 1.261) + 4.6151));
            frN = Math.Pow(twzg, (-0.5)) * (9 + 280 * h * Math.Exp(-4.17 * (Math.Pow(twzg, (-1.0 / 3.0)) - 1)));
            frO = 24 + 4.04 * Math.Pow(10, 4) * h * (0.02 + h) / (0.391 + h);

            for (int i = 0; i < oct.Length; i++)
            {
                kw[i] = 3.68 * Math.Pow(10, -11) * Math.Pow(oct[i], 2) * Math.Pow(twzg, 0.5) + Math.Pow(twzg, (-2.5)) * (0.1068 * Math.Exp(-3352 / tk) * 2 * Math.Pow(oct[i], 2) / (frN + Math.Pow(oct[i], 2) / frN)
            + 0.01275 * Math.Exp(-2239.1 / tk) * 2 * Math.Pow(oct[i], 2) / (frO + Math.Pow(oct[i], 2) / frO));
            }
            return kw;
        }

        public static double[] RoomAbsorptionCoeffiecient(RoomType roomType)
        {
            double k63, k125, k250, k500, k1000, k2000, k4000, k8000;
            double[] kw = new double[8];

            if (roomType == RoomType.Dead)
            {
                //Studia radiowe, studia nagrań,studia TV
                k63 = 0.26;
                k125 = 0.3;
                k250 = 0.35;
                k500 = 0.4;
                k1000 = 0.43;
                k2000 = 0.46;
                k4000 = 0.52;
                k8000 = 0.58;
            }
            else if (roomType == RoomType.MediumDead)
            {
                //Domy towarowe, czytelnie
                k63 = 0.24;
                k125 = 0.22;
                k250 = 0.18;
                k500 = 0.25;
                k1000 = 0.3;
                k2000 = 0.36;
                k4000 = 0.42;
                k8000 = 0.48;
            }
            else if (roomType == RoomType.Average)
            {
                //Mieszkania, pokoje hotelowe, biura, sale konferencyjne, teatr
                k63 = 0.25;
                k125 = 0.23;
                k250 = 0.17;
                k500 = 0.2;
                k1000 = 0.24;
                k2000 = 0.29;
                k4000 = 0.34;
                k8000 = 0.39;
            }
            else if (roomType == RoomType.MediumLive)
            {
                //Szkoły, szpitale, małe kościoły
                k63 = 0.25;
                k125 = 0.23;
                k250 = 0.15;
                k500 = 0.15;
                k1000 = 0.17;
                k2000 = 0.2;
                k4000 = 0.23;
                k8000 = 0.26;
            }
            else
            {
                //Hale fabryczne, pływalnie, duże kościoły 
                k63 = 0.26;
                k125 = 0.24;
                k250 = 0.12;
                k500 = 0.1;
                k1000 = 0.09;
                k2000 = 0.11;
                k4000 = 0.13;
                k8000 = 0.15;
            }
            kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
            return kw;
        }
    }
}
