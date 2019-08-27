using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Enums;

namespace Compute_Engine
{
    public static class Attenuation
    {
        public static double[] Diffuser(double a1, double a2, double l)
        {
            //a1-powierzchnia netto przewodu przed zmianą przekroju, m2
            //a2-powierzchnia netto przewodu za zmianą przekroju, m2
            //l-długość kształtki, m
            double co = 340.3;
            double k, la, i, n;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];

            for (int j = 0; j < oct.Length; j++)
            {
                la = co / oct[j];
                k = -81.096 * Math.Pow((l / la), 4) + 101.97 * Math.Pow((l / la), 3) - 39.077 * Math.Pow((l / la), 2) + 2.3702 * (l / la) + 0.999;
                i = 60;
                n = 10 * Math.Log10(Math.Pow((i * k + 1), 2) / (4 * i * k));

                do
                {
                    i = i - 0.05;
                    n = 10 * Math.Log10(Math.Pow((i * k + 1), 2) / (4 * i * k));
                }
                while (n >= 1.0);

                if (a1 / a2 >= i)
                {
                    attn[j] = 10 * Math.Log10(Math.Pow((a1 / a2 * k + 1), 2) / (4 * a1 / a2 * k));
                }
                else
                {
                    attn[j] = 0;
                }
            }
            return attn;
        }

        public static double[] JunctionMainRoundBranchRound(Branch which_branch, BranchType branchType, double d_start, double d_end, double d_branch)
        {
            double[] loc = JunctionMainRoundEq(which_branch, d_start, d_end, Math.Pow(d_branch, 2) * 0.25 * Math.PI);
            double[] attn = new double[8];

            if (which_branch != Branch.Main)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRound(0, d_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, d_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        public static double[] JunctionMainRoundBranchRectangular(Branch which_branch, BranchType branchType, double d_start, double d_end, double w_branch, double h_branch)
        {
            double[] loc = JunctionMainRoundEq(which_branch, d_start, d_end, w_branch * h_branch);
            double[] attn = new double[8];

            if (which_branch != Branch.Main)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRectangular(w_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, w_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        public static double[] JunctionMainRectangularBranchRound(Branch which_branch, BranchType branchType, double w_start, double h_start, double w_end, double h_end, double d_branch)
        {
            double[] loc = JunctionMainRectangularEq(which_branch, w_start * h_start, w_end * h_end, Math.Pow(d_branch, 2) * 0.25 * Math.PI, Math.Max(w_start, h_start));
            double[] attn = new double[8];

            if (which_branch != Branch.Main)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRound(0, d_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, d_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        public static double[] JunctionMainRectangularBranchRectangular(Branch which_branch, BranchType branchType, double w_start, double h_start, double w_end, double h_end, double w_branch, double h_branch)
        {
            double[] loc = JunctionMainRectangularEq(which_branch, w_start * h_start, w_end * w_end, w_branch * h_branch, Math.Max(w_start, h_start));
            double[] attn = new double[8];

            if (which_branch != Branch.Main)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRectangular(w_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, w_branch);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        private static double[] JunctionMainRoundEq(Branch which_branch, double d_poc, double d_kon, double a_odg)
        {
            //d_poc-średnica netto wlotu do trójnika, m
            //d_kon-średnica netto wylotu z trójnika, m
            //a_odg-powierzchnia netto odgałęzienia, m2
            double fco, a_kon, a_poc;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            a_kon = 0.25 * Math.PI * Math.Pow(d_kon, 2);
            a_poc = 0.25 * Math.PI * Math.Pow(d_poc, 2);
            fco = 0.586 * 1125 / UnitConvertion.MToFt(d_poc);

            for (int i = 0; i < oct.Length; i++)
            {
                if (which_branch == Branch.Main)
                {
                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(1 - Math.Pow((((a_odg + a_kon) / a_poc - 1) / ((a_odg + a_kon) / a_poc + 1)), 2)))) + Math.Abs(10 * Math.Log10(a_kon / (a_odg + a_kon)));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(a_kon / (a_odg + a_kon)));
                    }
                }
                else
                {
                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(1 - Math.Pow((((a_odg + a_kon) / a_poc - 1) / ((a_odg + a_kon) / a_poc + 1)), 2)))) + Math.Abs(10 * Math.Log10(a_odg / (a_odg + a_kon)));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(a_odg / (a_odg + a_kon)));
                    }
                }
            }
            return attn;
        }

        private static double[] JunctionMainRectangularEq(Branch which_branch, double a_poc, double a_kon, double a_odg, double a_in)
        {
            //a_poc-powierzchnia netto wlotu do trójnika, m2
            //a_kon-powierzchnia netto wylotu z trójnika, m2
            //a_odg-powierzchnia netto odgałęzienia, m2
            //a_in-większy wymiar wlotowego przekroju poprzecznego, m
            double fco; double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            fco = 1125 / UnitConvertion.MToFt(a_in);

            for (int i = 0; i < oct.Length; i++)
            {
                if (which_branch == Branch.Main)
                {
                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(1 - Math.Pow((((a_odg + a_kon) / a_poc - 1) / ((a_odg + a_kon) / a_poc + 1)), 2)))) + Math.Abs(10 * Math.Log10(a_kon / (a_odg + a_kon)));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(a_kon / (a_odg + a_kon)));
                    }
                }
                else
                {
                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(1 - Math.Pow((((a_odg + a_kon) / a_poc - 1) / ((a_odg + a_kon) / a_poc + 1)), 2)))) + Math.Abs(10 * Math.Log10(a_odg / (a_odg + a_kon)));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(a_odg / (a_odg + a_kon)));
                    }
                }
            }
            return attn;
        }

        public static double[] DoubleJunctionMainRoundBranchRound(Branch which_branch, BranchType branchType, double d_start, double d_end, double d_branch_right, double d_branch_left)
        {
            double[] loc = DoubleJunctionMainRoundEq(which_branch, d_start, d_end, Math.Pow(d_branch_right, 2) * 0.25 * Math.PI, Math.Pow(d_branch_left, 2) * 0.25 * Math.PI);
            double[] attn = new double[8];

            if (which_branch == Branch.BranchRight)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRound(0, d_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, d_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else if (which_branch == Branch.BranchLeft)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRound(0, d_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, d_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        public static double[] DoubleJunctionMainRoundBranchRectangular(Branch which_branch, BranchType branchType, double d_start, double d_end, double w_branch_right, double h_branch_right, double w_branch_left, double h_branch_left)
        {
            double[] loc = DoubleJunctionMainRoundEq(which_branch, d_start, d_end, w_branch_right * h_branch_right, w_branch_left * h_branch_left);
            double[] attn = new double[8];

            if (which_branch == Branch.BranchRight)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRectangular(w_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, w_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else if (which_branch == Branch.BranchLeft)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRectangular(w_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, w_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        public static double[] DoubleJunctionMainRectangularBranchRound(Branch which_branch, BranchType branchType, double w_start, double h_start, double w_end, double h_end, double d_branch_right, double d_branch_left)
        {
            double[] loc = DoubleJunctionMainRectangularEq(which_branch, w_start * h_start, w_end * h_end, Math.Pow(d_branch_right, 2) * 0.25 * Math.PI, Math.Pow(d_branch_left, 2) * 0.25 * Math.PI, Math.Max(w_start, h_start));
            double[] attn = new double[8];

            if (which_branch == Branch.BranchRight)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRound(0, d_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, d_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else if (which_branch == Branch.BranchLeft)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRound(0, d_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, d_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        public static double[] DoubleJunctionMainRectangularBranchRectangular(Branch which_branch, BranchType branchType, double w_start, double h_start, double w_end, double h_end, double w_branch_right, double h_branch_right, double w_branch_left, double h_branch_left)
        {
            double[] loc = DoubleJunctionMainRectangularEq(which_branch, w_start * h_start, w_end * w_end, w_branch_right * h_branch_right, w_branch_left * h_branch_left, Math.Max(w_start, h_start));
            double[] attn = new double[8];

            if (which_branch == Branch.BranchRight)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRectangular(w_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, w_branch_right);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else if (which_branch == Branch.BranchLeft)
            {
                if (branchType == BranchType.Rounded)
                {
                    double[] brch = BowRectangular(w_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
                else
                {
                    double[] brch = Elbow(TurnigVanes.No, 0, w_branch_left);

                    for (int i = 0; i < attn.Length; i++)
                    {
                        attn[i] = loc[i] + brch[i];
                    }
                }
            }
            else
            {
                attn = loc;
            }
            return attn;
        }

        private static double[] DoubleJunctionMainRoundEq(Branch which_branch, double d_poc, double d_kon, double a_odg1, double a_odg2)
        {
            //d_kon-średnica netto wylotu z czwórnika, m
            //d_poc-średnica netto wlotu do czwórnika, m
            //a_odg1-powierzchnia netto pierwszego odgałęzienia, m2
            //a_odg2-powierzchnia netto drugiego odgałęzienia, m2
            double a, fco, a_kon, a_poc;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            a_kon = 0.25 * Math.PI * Math.Pow(d_kon, 2);
            a_poc = 0.25 * Math.PI * Math.Pow(d_poc, 2);
            fco = 0.586 * 1125 / UnitConvertion.MToFt(d_poc);

            for (int i = 0; i < oct.Length; i++)
            {
                if (which_branch == Branch.BranchRight || which_branch == Branch.BranchLeft)
                {
                    if (which_branch == Branch.BranchRight)
                    {
                        a = a_odg1;
                    }
                    else
                    {
                        a = a_odg2;
                    }

                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(1 - Math.Pow((((a_odg1 + a_odg2 + a_kon) / a_poc - 1) / ((a_odg1 + a_odg2 + a_kon) / a_poc + 1)), 2)))
                            + Math.Abs(10 * Math.Log10(Math.Abs(a / (a_odg1 + a_odg2 + a_kon))));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(a / (a_odg1 + a_odg2 + a_kon))));
                    }
                }
                else
                {
                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(1 - Math.Pow((((a_odg1 + a_odg2 + a_kon) / a_poc - 1) / ((a_odg1 + a_odg2 + a_kon) / a_poc + 1)), 2)))
                          + Math.Abs(10 * Math.Log10(Math.Abs(a_kon / (a_odg1 + a_odg2 + a_kon))));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(a_kon / (a_odg1 + a_odg2 + a_kon))));
                    }
                }
            }
            return attn;
        }

        private static double[] DoubleJunctionMainRectangularEq(Branch which_branch, double a_poc, double a_kon, double a_odg1, double a_odg2, double a_in)
        {
            //a_kon-powierzchnia netto wylotu z czwórnika, m2
            //a_poc-powierzchnia netto wlotu do czwórnika, m2
            //a_odg1-powierzchnia netto pierwszego odgałęzienia, m2
            //a_odg2-powierzchnia netto drugiego odgałęzienia, m2
            //a-większy wymiar wlotowego przekroju poprzecznego, m
            double f, fco;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            fco = 1125 / UnitConvertion.MToFt(a_in);

            for (int i = 0; i < oct.Length; i++)
            {
                if (which_branch == Branch.BranchRight || which_branch == Branch.BranchRight)
                {
                    if (which_branch == Branch.BranchRight)
                    {
                        f = a_odg1;
                    }
                    else
                    {
                        f = a_odg2;
                    }

                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(1 - Math.Pow((((a_odg1 + a_odg2 + a_kon) / a_poc - 1) / ((a_odg1 + a_odg2 + a_kon) / a_poc + 1)), 2)))
                        + Math.Abs(10 * Math.Log10(Math.Abs(f / (a_odg1 + a_odg2 + a_kon))));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(f / (a_odg1 + a_odg2 + a_kon))));
                    }
                }
                else
                {
                    if (oct[i] < Noise.Oct_BFI(fco))
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(1 - Math.Pow((((a_odg1 + a_odg2 + a_kon) / a_poc - 1) / ((a_odg1 + a_odg2 + a_kon) / a_poc + 1)), 2)))
                        + Math.Abs(10 * Math.Log10(Math.Abs(a_kon / (a_odg1 + a_odg2 + a_kon))));
                    }
                    else
                    {
                        attn[i] = Math.Abs(10 * Math.Log10(Math.Abs(a_kon / (a_odg1 + a_odg2 + a_kon))));
                    }
                }
            }
            return attn;
        }

        public static double[] PlenumInletRectangular(double liningThickness, double q, double sin, double sout, double b, double ld, double l, double w, double h)
        {
            //sin-powierzchnia otworu wlotowego, m2
            //sout-powierzchnia otworu wylotowego, m2
            //a-współczynnik pochłaniania dźwięku w danej częstotliwości,
            //l-głębokość skrzynki rozprężnej, m
            //ld-pozioma odległość między środkiem otworu wlotowego a środkiem otworu wylotowego, m
            //w-szerokość skrzynki rozprężnej (wymiar x), m
            //h-wysokość skrzynki rozprężnej (wymiar y), m
            //b-większy wymiar otworu wlotowego, m
            //liningThickness-grubość wewnętrznej izolacji akustycznej (wata szklana), cm
            double fco, s, m, gl, w1, h1, l1;
            double[] a = new double[8];
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];

            if (!(liningThickness == 0))
            {
                a = AbsMaterial(LiningType.Fiberglass, liningThickness);
            }
            else
            {
                a = AbsMaterial(LiningType.Steel, liningThickness);
            }

            fco = 1125 / (2 * UnitConvertion.MToFt(b));
            w1 = w;
            h1 = h;
            l1 = l;

            for (int i = 0; i < oct.Length; i++)
            {
                if (oct[i] < Noise.Oct_BFI(fco))
                {
                    m = (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1)) / (sin * Math.Pow(UnitConvertion.MToFt(1), 2));

                    if (oct[i] == 63)
                    {
                        gl = (0.00306 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (1.959)) * Math.Pow((liningThickness / 2.5), (0.917))) * UnitConvertion.MToFt(l1);
                    }
                    else if (oct[i] == 125)
                    {
                        gl = (0.01323 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (1.41)) * Math.Pow((liningThickness / 2.5), (0.941))) * UnitConvertion.MToFt(l1);
                    }
                    else if (oct[i] == 250)
                    {
                        gl = (0.06244 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (0.824)) * Math.Pow((liningThickness / 2.5), (1.079))) * UnitConvertion.MToFt(l1);
                    }
                    else
                    {
                        gl = (0.2338 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (0.5)) * Math.Pow((liningThickness / 2.5), (1.087))) * UnitConvertion.MToFt(l1);
                    }

                    attn[i] = 10 * Math.Log10(Math.Pow((Math.Cosh(gl / 2) + 0.5 * (m + 1 / m) * Math.Sinh(gl / 2)), 2) * Math.Pow((Math.Cos(2 * Math.PI * oct[i] * UnitConvertion.MToFt(l1) / 1125)), 2) + Math.Pow((Math.Sinh(gl / 2) + 0.5 * (m + 1 / m) * Math.Cosh(gl / 2)), 2)
                        * Math.Pow((Math.Sin(2 * Math.PI * oct[i] * UnitConvertion.MToFt(l1) / 1125)), 2));
                }
                else
                {
                    s = 2 * (UnitConvertion.MToFt(l1) * UnitConvertion.MToFt(w1)) + 2 * (UnitConvertion.MToFt(l1) * UnitConvertion.MToFt(h1)) + 2 * (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1)) - (sin + sout) * Math.Pow(UnitConvertion.MToFt(1), 2);

                    if (-10 * Math.Log10(sout * (q / (4 * Math.PI * Math.Pow(ld, 2)) + (1 - a[i]) / (s * a[i]))) > 1)
                    {
                        attn[i] = -10 * Math.Log10(sout * (q / (4 * Math.PI * Math.Pow(ld, 2)) + (1 - a[i]) / (s * a[i])));
                    }
                    else
                    {
                        attn[i] = 0;
                    }
                }
            }
            return attn;
        }

        private static double[] AbsMaterial(LiningType liningType, double t)
        {
            double k63, k125, k250, k500, k1000, k2000, k4000, k8000;
            if (liningType == LiningType.Concrete)
            {
                //beton
                k63 = 0.01;
                k125 = 0.01;
                k250 = 0.01;
                k500 = 0.02;
                k1000 = 0.02;
                k2000 = 0.02;
                k4000 = 0.03;
                k8000 = 0.04;
            }
            else if (liningType == LiningType.Steel)
            {
                //blacha stalowa
                k63 = 0.04;
                k125 = 0.04;
                k250 = 0.04;
                k500 = 0.05;
                k1000 = 0.05;
                k2000 = 0.05;
                k4000 = 0.07;
                k8000 = 0.09;
            }
            else
            {
                //wata szklana
                //grubość od 2,5 do 10 cm
                k63 = 0.03 * Math.Pow((t / 2.5), 2) + 0.102 * (t / 2.5) - 0.12;
                k125 = 0.03 * Math.Pow((t / 2.5), 2) + 0.124 * (t / 2.5) - 0.13;
                if (t < 7.5)
                {
                    k250 = -0.21 * Math.Pow((t / 2.5), 2) + 1.23 * (t / 2.5) - 0.8;
                }
                else
                {
                    k250 = 1;
                }

                if ((0.31 * (t / 2.5) + 0.38) <= 1)
                {
                    k500 = 0.31 * (t / 2.5) + 0.38;
                }
                else
                {
                    k500 = 1;
                }

                if ((0.09 * (t / 2.5) + 0.82) <= 1)
                {
                    k1000 = 0.09 * (t / 2.5) + 0.82;
                }
                else
                {
                    k1000 = 1;
                }

                k2000 = 1;
                k4000 = 1;
                k8000 = 1;
            }
            double[] val = { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
            return val;
        }

        public static double[] PlenumInletRound(double liningThickness, double q, double sout, double d, double ld, double l, double w, double h)
        {
            //sin-powierzchnia otworu wlotowego, m2
            //sout-powierzchnia otworu wylotowego, m2
            //a-współczynnik pochłaniania dźwięku w danej częstotliwości,
            //l-głębokość skrzynki rozprężnej, m
            //ld-pozioma odległość między środkiem otworu wlotowego a środkiem otworu wylotowego, m
            //w-szerokość skrzynki rozprężnej (wymiar x), m
            //h-wysokość skrzynki rozpręż nej (wymiar y), m
            //d-średnica otworu wlotowego, m
            //liningThickness-grubość wewnętrznej izolacji akustycznej (wata szkalana), cm
            double fco, s, sin, m, gl, w1, h1, l1;
            double[] a = new double[8];
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];

            if (!(liningThickness == 0))
            {
                a = AbsMaterial(LiningType.Fiberglass, liningThickness);
            }
            else
            {
                a = AbsMaterial(LiningType.Steel, liningThickness);
            }

            sin = Math.PI * 0.25 * Math.Pow(d, 2);
            fco = 0.586 * 1125 / UnitConvertion.MToFt(d);
            w1 = w;
            h1 = h;
            l1 = l;

            for (int i = 0; i < oct.Length; i++)
            {
                if (oct[i] < Noise.Oct_BFI(fco))
                {
                    m = (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1)) / (sin * Math.Pow(UnitConvertion.MToFt(1), 2));

                    if (oct[i] == 63)
                    {
                        gl = (0.00306 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (1.959)) * Math.Pow((liningThickness / 2.5), (0.917))) * UnitConvertion.MToFt(l1);
                    }
                    else if (oct[i] == 125)
                    {
                        gl = (0.01323 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (1.41)) * Math.Pow((liningThickness / 2.5), (0.941))) * UnitConvertion.MToFt(l1);
                    }
                    else if (oct[i] == 250)
                    {
                        gl = (0.06244 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (0.824)) * Math.Pow((liningThickness / 2.5), (1.079))) * UnitConvertion.MToFt(l1);
                    }
                    else
                    {
                        gl = (0.2338 * Math.Pow(((2 * UnitConvertion.MToFt(h1) + 2 * UnitConvertion.MToFt(w1)) / (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1))), (0.5)) * Math.Pow((liningThickness / 2.5), (1.087))) * UnitConvertion.MToFt(l1);
                    }

                    attn[i] = 10 * Math.Log10(Math.Pow((Math.Cosh(gl / 2) + 0.5 * (m + 1 / m) * Math.Sinh(gl / 2)), 2) * Math.Pow((Math.Cos(2 * Math.PI * oct[i] * UnitConvertion.MToFt(l1) / 1125)), 2) + Math.Pow((Math.Sinh(gl / 2) +
                        0.5 * (m + 1 / m) * Math.Cosh(gl / 2)), 2) * Math.Pow((Math.Sin(2 * Math.PI * oct[i] * UnitConvertion.MToFt(l1) / 1125)), 2));
                }
                else
                {
                    s = 2 * (UnitConvertion.MToFt(l1) * UnitConvertion.MToFt(w1)) + 2 * (UnitConvertion.MToFt(l1) * UnitConvertion.MToFt(h1)) + 2 * (UnitConvertion.MToFt(h1) * UnitConvertion.MToFt(w1)) - (sin + sout) * Math.Pow(UnitConvertion.MToFt(1), 2);

                    if (-10 * Math.Log10(sout * (q / (4 * Math.PI * Math.Pow(ld, 2)) + (1 - a[i]) / (s * a[i]))) > 1)
                    {
                        attn[i] = -10 * Math.Log10(sout * (q / (4 * Math.PI * Math.Pow(ld, 2)) + (1 - a[i]) / (s * a[i])));
                    }
                    else
                    {
                        attn[i] = 0;
                    }
                }
            }
            return attn;
        }

        public static double[] Grill(GrillLocation grillLocation, double a)
        {
            //a-pole powierzchni przewodu do którego jest przyłączona jest kratka, m2
            double co = 1125.0;
            double a1, d;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            a1 = a * Math.Pow(UnitConvertion.MToFt(1), 2);
            d = Math.Pow((4 * a1 / Math.PI), 0.5);

            for (int i = 0; i < oct.Length; i++)
            {
                if (grillLocation == GrillLocation.FreeSpace)
                {
                    attn[i] = 10 * Math.Log10(1 + Math.Pow((co / (Math.PI * oct[i] * d)), 1.88));
                }
                else
                {
                    attn[i] = 10 * Math.Log10(1 + Math.Pow((0.8 * co / (Math.PI * oct[i] * d)), 1.88));
                }
            }
            return attn;
        }

        public static double[] DuctOther(double circuit, double p, double[] a, double l)
        {
            //p-pole powierzchni przewodu, m2
            //circuit-obwód kanału, m
            //a-współczynnik pochłaniania dźwięku w danej częstotliwości.
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];

            for (int i = 0; i < oct.Length; i++)
            {
                if (1.09 * a[i] / (p / circuit) * l < 1)
                {
                    attn[i] = 0;
                }
                else
                {
                    attn[i] = 1.09 * a[i] / (p / circuit) * l;
                }
            }
            return attn;
        }

        public static double[] BowRound(double liningThickness, double d)
        {
            //d=m,f=Hz
            //liningThickness-grubość wewnętrznej izolacji akustycznej (wata szklana od 2,5 do 7,5cm), cm
            double d1, r, f;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            d1 = 100 * (d) / 2.54;
            r = 100 * (1.5 * d + 3 * liningThickness / 100) / 2.54;

            for (int i = 0; i < oct.Length; i++)
            {
                if (liningThickness == 0)
                {
                    if (oct[i] * d < 48)
                        attn[i] = 0.0;
                    else if (48 <= oct[i] * d && oct[i] * d < 96)
                    {
                        attn[i] = 1.0;
                    }
                    else if (96 <= oct[i] * d && oct[i] * d < 190)
                    {
                        attn[i] = 2.0;
                    }
                    else
                    {
                        attn[i] = 3.0;
                    }
                }
                else
                {
                    f = oct[i] / 1000;

                    if (d1 <= 18)
                    {
                        attn[i] = (0.485 + 2.094 * Math.Log10(f * d1) + 3.172 * Math.Pow((Math.Log10(f * d1)), 2) - 1.578 * Math.Pow((Math.Log10(f * d1)), 4) + 0.085 * Math.Pow((Math.Log10(f * d1)), 7)) / Math.Pow((d1 / r), 2);
                    }
                    else
                    {
                        attn[i] = (-1.493 + 0.538 * (liningThickness / 2.5) + 1.406 * Math.Log10(f * d1) + 2.779 * Math.Pow((Math.Log10(f * d1)), 2) - 0.662 * Math.Pow((Math.Log10(f * d1)), 4) + 0.016 * Math.Pow((Math.Log10(f * d1)), 7)) / Math.Pow((d1 / r), 2);
                    }

                    if (attn[i] < 0)
                    {
                        attn[i] = 0.0;
                    }
                }
            }
            return attn;
        }

        public static double[] BowRectangular(double w)
        {
            //w=szerekość kolana, m
            //w=m,f=Hz
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];

            for (int i = 0; i < oct.Length; i++)
            {
                if (oct[i] * w < 48)
                {
                    attn[i] = 0.0;
                }
                else if (48 <= oct[i] * w && oct[i] * w < 96)
                {
                    attn[i] = 1.0;
                }
                else if (96 <= oct[i] * w && oct[i] * w < 190)
                {
                    attn[i] = 2.0;
                }
                else
                {
                    attn[i] = 3.0;
                }
            }
            return attn;
        }

        public static double[] Elbow(TurnigVanes turnigVanes, double liningThickness, double w)
        {
            //w-szerokość kolana
            //w=m,f=Hz
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];

            for (int i = 0; i < oct.Length; i++)
            {
                if (liningThickness == 0)
                {
                    //kolano nieizolowane akustycznie

                    if (turnigVanes == TurnigVanes.Yes)
                    {
                        //kolano z kierownicami powietrza
                        if (oct[i] * w < 48)
                        {
                            attn[i] = 0.0;
                        }
                        else if (48 <= oct[i] * w && oct[i] * w < 96)
                        {
                            attn[i] = 1.0;
                        }
                        else if (96 <= oct[i] * w && oct[i] * w < 190)
                        {
                            attn[i] = 4.0;
                        }
                        else if (190 <= oct[i] * w && oct[i] * w < 380)
                        {
                            attn[i] = 6.0;
                        }
                        else
                        {
                            attn[i] = 4.0;
                        }
                    }
                    else
                    {
                        //kolano bez kierownic powietrza
                        if (oct[i] * w < 48)
                        {
                            attn[i] = 0;
                        }
                        else if (48 <= oct[i] * w && oct[i] * w < 96)
                        {
                            attn[i] = 1.0;
                        }
                        else if (96 <= oct[i] * w && oct[i] * w < 190)
                        {
                            attn[i] = 5.0;
                        }
                        else if (190 <= oct[i] * w && oct[i] * w < 380)
                        {
                            attn[i] = 8.0;
                        }
                        else if (380 <= oct[i] * w && oct[i] * w < 760)
                        {
                            attn[i] = 4.0;
                        }
                        else
                        {
                            attn[i] = 3.0;
                        }
                    }
                }
                else
                {
                    //kolano izolowane akustycznie
                    if (turnigVanes == TurnigVanes.Yes)
                    {
                        if (oct[i] * w < 48)
                        {
                            attn[i] = 0.0;
                        }
                        else if (48 <= oct[i] * w && oct[i] * w < 96)
                        {
                            attn[i] = 1.0;
                        }
                        else if (96 <= oct[i] * w && oct[i] * w < 190)
                        {
                            attn[i] = 4.0;
                        }
                        else if (190 <= oct[i] * w && oct[i] * w < 380)
                        {
                            attn[i] = 7.0;
                        }
                        else
                        {
                            attn[i] = 7.0;
                        }
                    }
                    else
                    {
                        if (oct[i] * w < 48)
                        {
                            attn[i] = 0.0;
                        }
                        else if (48 <= oct[i] * w && oct[i] * w < 96)
                        {
                            attn[i] = 1.0;
                        }
                        else if (96 <= oct[i] * w && oct[i] * w < 190)
                        {
                            attn[i] = 6.0;
                        }
                        else if (190 <= oct[i] * w && oct[i] * w < 380)
                        {
                            attn[i] = 11.0;
                        }
                        else if (380 <= oct[i] * w && oct[i] * w < 760)
                        {
                            attn[i] = 10.0;
                        }
                        else
                        {
                            attn[i] = 10.0;
                        }
                    }
                }
            }
            return attn;
        }

        public static double[] DuctRound(double liningThicknesss, double d, double l)
        {
            //d=m, l=m, oct=Hz
            //turnigVanes-grubość wewnętrznej izolacji akustycznej (wata szklana od 2,5 do 7,5cm), cm
            byte n = 0;
            double[] loc = DuctRadEq(liningThicknesss, d, l);
            double[] attn = new double[8];

            for (int i = 0; i < loc.Length; i++)
            {
                if (loc[i] > 1)
                {
                    n++;
                }

                if (n < 1)
                {
                    attn = new double[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                }
                else
                {
                    attn = loc;
                }
            }
            return attn;
        }

        private static double[] DuctRadEq(double liningThickness, double d, double l)
        {
            //d=m, l=m, Oct=Hz
            //liningThickness-grubość wewnętrznej izolacji akustycznej (wata szklana od 2,5 do 7,5cm), cm
            double a, b, c, d2, e, f, d1;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            d1 = 100 * (d) / 2.54;

            for (int i = 0; i < oct.Length; i++)
            {
                if (liningThickness == 0)
                {
                    if (d < 0.2)
                    {
                        if (oct[i] <= 125)
                        {
                            attn[i] = 0.1 * l;
                        }
                        else if (oct[i] > 125 && oct[i] <= 500)
                        {
                            attn[i] = 0.15 * l;
                        }
                        else
                        {
                            attn[i] = 0.3 * l;
                        }
                    }
                    else if (d >= 0.2 && d < 0.4)
                    {
                        if (oct[i] < 125)
                        {
                            attn[i] = 0.05 * l;
                        }
                        else if (oct[i] >= 125 && oct[i] <= 250)
                        {
                            attn[i] = 0.1 * l;
                        }
                        else if (oct[i] > 250 && oct[i] <= 500)
                        {
                            attn[i] = 0.15 * l;
                        }
                        else
                        {
                            attn[i] = 0.2 * l;
                        }
                    }
                    else if (d >= 0.4 && d < 0.8)
                    {
                        if (oct[i] < 125)
                        {
                            attn[i] = 0.0 * l;
                        }
                        else if (oct[i] >= 125 && oct[i] <= 250)
                        {
                            attn[i] = 0.05 * l;
                        }
                        else if (oct[i] > 250 && oct[i] <= 500)
                        {
                            attn[i] = 0.1 * l;
                        }
                        else
                        {
                            attn[i] = 0.15 * l;
                        }
                    }
                    else if (d >= 0.8 && d < 1.0)
                    {
                        if (oct[i] <= 250)
                        {
                            attn[i] = 0 * l;
                        }
                        else
                        {
                            attn[i] = 0.05 * l;
                        }
                    }
                    else
                    {
                        attn[i] = 0 * l;
                    }
                }
                else
                {
                    if (oct[i] == 63)
                    {
                        a = 0.2825;
                        b = 0.3447;
                        c = -0.05251;
                        d2 = -0.03837;
                        e = 0.0009132;
                        f = -0.000008294;
                    }
                    else if (oct[i] == 125)
                    {
                        a = 0.5237;
                        b = 0.2234;
                        c = -0.004936;
                        d2 = -0.02724;
                        e = 0.0003377;
                        f = -0.00000249;
                    }
                    else if (oct[i] == 250)
                    {
                        a = 0.3652;
                        b = 0.79;
                        c = -0.1157;
                        d2 = -0.01834;
                        e = -0.0001211;
                        f = 0.000002681;
                    }
                    else if (oct[i] == 500)
                    {
                        a = 0.1333;
                        b = 1.845;
                        c = -0.3735;
                        d2 = -0.01293;
                        e = 0.00008624;
                        f = -0.000004986;
                    }
                    else if (oct[i] == 1000)
                    {
                        a = 1.933;
                        b = 0.0;
                        c = 0.0;
                        d2 = 0.06135;
                        e = -0.003891;
                        f = 0.00003934;
                    }
                    else if (oct[i] == 2000)
                    {
                        a = 2.73;
                        b = 0.0;
                        c = 0.0;
                        d2 = -0.07341;
                        e = 0.0004428;
                        f = 0.000001006;
                    }
                    else if (oct[i] == 4000)
                    {
                        a = 2.8;
                        b = 0.0;
                        c = 0.0;
                        d2 = -0.1467;
                        e = 0.003404;
                        f = -0.00002851;
                    }
                    else
                    {
                        a = 1.545;
                        b = 0.0;
                        c = 0.0;
                        d2 = -0.05452;
                        e = 0.00129;
                        f = -0.00001318;
                    }

                    attn[i] = (a + (b * liningThickness / 2.5) + (c * Math.Pow((liningThickness / 2.5), 2))
                        + (d2 * (d1)) + (e * Math.Pow((d1), 2)) + (f * Math.Pow((d1), 3))) * UnitConvertion.MToFt(l);

                    if (attn[i] > 40)
                    {
                        attn[i] = 40;
                    }
                }
            }
            return attn;
        }

        public static double[] DuctRectanguar(double liningThickness, double w, double h, double l)
        {
            //w-szerokość kanału, m
            //h-wysokość kanału, m
            //l-długość kanału, m
            //liningThickness-grubość wewnętrznej izolacji akustycznej (wata szklana od 2,5 do 5cm), cm
            //oct=Hz
            byte n = 0;
            double[] loc = DuctRectEq(liningThickness, w, h, l);
            double[] attn = new double[8];

            for (int i = 0; i < loc.Length; i++)
            {
                if (loc[i] > 1)
                {
                    n++;
                }

                if (n < 1)
                {
                    attn = new double[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                }
                else
                {
                    attn = loc;
                }
            }
            return attn;
        }

        private static double[] DuctRectEq(double liningThickness, double w, double h, double l)
        {
            //w-szerokość kanału, m
            //h-wysokość kanału, m
            //l-długość kanału, m
            //liningThickness-grubość wewnętrznej izolacji akustycznej (wata szklana od 2,5 do 5cm), cm
            double p, a, b, c, d, p1, a1;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] attn = new double[8];
            double[] il = { 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] val = new double[8];

            p = 2 * (UnitConvertion.MToFt(h) + UnitConvertion.MToFt(w));
            a = UnitConvertion.MToFt(h) * UnitConvertion.MToFt(w);

            for (int i = 0; i < oct.Length; i++)
            {
                if (oct[i] <= 250)
                    if (p / a >= 3)
                    {
                        attn[i] = 17 * Math.Pow((p / a), (-0.25)) * Math.Pow(oct[i], (-0.85)) * UnitConvertion.MToFt(l);
                    }
                    else
                    {
                        attn[i] = 1.64 * Math.Pow((p / a), 0.73) * Math.Pow(oct[i], (-0.58)) * UnitConvertion.MToFt(l);
                    }
                else
                {
                    attn[i] = 0.02 * Math.Pow((p / a), 0.8) * UnitConvertion.MToFt(l);
                }

                if (liningThickness > 0)
                {
                    if (oct[i] == 63)
                    {
                        b = 0.0133;
                        c = 1.959;
                        d = 0.917;
                    }
                    else if (oct[i] == 125)
                    {
                        b = 0.0574;
                        c = 1.41;
                        d = 0.941;
                    }
                    else if (oct[i] == 250)
                    {
                        b = 0.271;
                        c = 0.824;
                        d = 1.079;
                    }
                    else if (oct[i] == 500)
                    {
                        b = 1.0147;
                        c = 0.5;
                        d = 1.087;
                    }
                    else if (oct[i] == 1000)
                    {
                        b = 1.77;
                        c = 0.695;
                        d = 0.0;
                    }
                    else if (oct[i] == 2000)
                    {
                        b = 1.392;
                        c = 0.802;
                        d = 0.0;
                    }
                    else if (oct[i] == 4000)
                    {
                        b = 1.518;
                        c = 0.451;
                        d = 0.0;
                    }
                    else
                    {
                        b = 1.581;
                        c = 0.219;
                        d = 0.0;
                    }

                    p1 = 2 * (UnitConvertion.MToFt(h) + UnitConvertion.MToFt(w));
                    a1 = UnitConvertion.MToFt(h) * UnitConvertion.MToFt(w);
                    il[i] = b * Math.Pow((p1 / a1), (c)) * Math.Pow((liningThickness / 2.5), (d)) * UnitConvertion.MToFt(l);
                }

                if ((attn[i] + il[i]) > 40)
                {
                    val[i] = 40;
                }
                else
                {
                    val[i] = attn[i] + il[i];
                }
            }
            return val;
        }
    }
}
