using System;
using static Compute_Engine.Enums;

namespace Compute_Engine
{
    public static class Noise
    {

        #region Public methods

        public static double[] Fan(FanType type, double q, double dp, int rpm, byte blade, byte eff, byte io)
        {
            //hałas generowany przez wentylator
            //dp=Pa, q=m3/h, oct=Hz, rpm=rpm, eff=%
            //io-czy hałas generowany w dwie strony czy tylko strona ssawna/tłoczna
            double c;
            double qw = UnitConvertion.CMHToCFM(q);
            double dpw = UnitConvertion.PaToInH20(dp);
            byte n = new byte();
            byte c1, k63, k125, k250, k500, k1000, k2000, k4000, k8000;
            double[] loc = new double[8];
            double[] kw = new double[8];
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };

            switch (type)
            {
                case FanType.CentrifugalBackwardCurved:
                    //wentylator odśrodkowy z łopatkami odgiętymi do tyłu
                    //liczba łopatek od 10 do 16
                    n = 3;
                    k63 = 35;
                    k125 = 35;
                    k250 = 34;
                    k500 = 32;
                    k1000 = 31;
                    k2000 = 26;
                    k4000 = 18;
                    k8000 = 10;
                    kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
                    break;
                case FanType.CentrifugalRadial:
                    //wentylator odśrodkowy z łopatkami promieniowymi
                    //liczba łopatek od 6 do 10
                    n = 7;
                    k63 = 48;
                    k125 = 45;
                    k250 = 43;
                    k500 = 43;
                    k1000 = 38;
                    k2000 = 33;
                    k4000 = 30;
                    k8000 = 29;
                    kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
                    break;
                case FanType.CentrifugalForwardCurved:
                    //wentylator odśrodkowy z łopatkami odgiętymi do przodu
                    //liczba łopatek od 24 do 64
                    n = 2;
                    k63 = 40;
                    k125 = 38;
                    k250 = 38;
                    k500 = 34;
                    k1000 = 28;
                    k2000 = 24;
                    k4000 = 21;
                    k8000 = 15;
                    kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
                    break;
                case FanType.VaneAxial:
                    //wentylator osiowy kanałowy z kierownicami powietrza
                    //liczba łopatek od 3 do 16
                    n = 7;
                    k63 = 42;
                    k125 = 39;
                    k250 = 41;
                    k500 = 42;
                    k1000 = 40;
                    k2000 = 37;
                    k4000 = 35;
                    k8000 = 25;
                    kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
                    break;
                case FanType.TubeAxial:
                    //wentylator osiowy kanałowy
                    //liczba łopatek od 4 do 8
                    n = 7;
                    k63 = 44;
                    k125 = 42;
                    k250 = 46;
                    k500 = 44;
                    k1000 = 42;
                    k2000 = 40;
                    k4000 = 37;
                    k8000 = 30;
                    kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
                    break;
                case FanType.Propeller:
                    //wentylator osiowy śmigłowy
                    //liczba łopatek od 2 do 8
                    n = 6;
                    k63 = 51;
                    k125 = 48;
                    k250 = 49;
                    k500 = 47;
                    k1000 = 45;
                    k2000 = 45;
                    k4000 = 43;
                    k8000 = 31;
                    kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
                    break;
                case FanType.TubularCentrifugal:
                    //wentylator osiowo-promieniowy
                    //liczba łopatek od 10 do 16
                    n = 5;
                    k63 = 46;
                    k125 = 43;
                    k250 = 43;
                    k500 = 38;
                    k1000 = 37;
                    k2000 = 32;
                    k4000 = 28;
                    k8000 = 25;
                    kw = new double[] { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
                    break;
            }

            if (io == 0)
            { c1 = 3; }
            //tylko strona ssawna lub tłoczna           
            else
            { c1 = 0; }

            if (eff <= 54)
            { c = 15; }
            else if (eff > 54 && eff <= 64)
            { c = 12; }
            else if (eff > 64 && eff <= 74)
            { c = 9; }
            else if (eff > 74 && eff <= 84)
            { c = 6; }
            else if (eff > 84 && eff <= 89)
            { c = 3; }
            else
            { c = 0; }

            for (int i = 0; i < 8; i++)
            {
                loc[i] = kw[i] + 10 * Math.Log10(qw) + 20 * Math.Log10(dpw) + BFI(rpm, oct[i], blade, n) + c - c1;
            }

            return loc;
        }

        internal static double Oct_BFI(double f)
        {
            if (f >= 44.5 && f < 89)
            { return 63; }
            else if (f >= 89 && f < 177)
            { return 125; }
            else if (f >= 177 && f < 354)
            { return 250; }
            else if (f >= 354 && f < 707)
            { return 500; }
            else if (f >= 707 && f < 1414)
            { return 1000; }
            else if (f >= 1414 && f < 2828)
            { return 2000; }
            else if (f >= 2828 && f < 5657)
            { return 4000; }
            else if (f >= 5657 && f < 11314)
            { return 8000; }
            else
            { return 0; }
        }

        public static double[] DamperRectangular(byte blade_number, byte ang, double q, double w, double h)
        {
            //tylko dla przepustnic jednopłaszczznowych i wielopłaszczyznowych współbieżnych
            //dp-całkowita strata ciśnienia na przepustnicy, Pa
            //q-wydajność na przepustnicy, m3/h
            //h-wysokość przepustnicy (wymiar prostopadły do osi obrotu klapy), m
            //w-szerokość przepustnicy, m
            //S-powierzchnia brutto przekroju przepustnicy, m2
            double c, bf, st, uc, s1, kd, dp;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] loc = new double[8];

            s1 = UnitConvertion.MToFt(h) * UnitConvertion.MToFt(w);
            dp = DamperDP(blade_number, q, ang, w, h);
            c = 15.9 * Math.Pow(10, 6) * UnitConvertion.PaToInH20(dp) * Math.Pow(s1 / UnitConvertion.CMHToCFM(q), 2);

            if (blade_number == 0 || blade_number == 1)
                //przepustnica jednopłaszczyznowa
                if (c < 4)
                {
                    bf = (Math.Pow(c, 0.5) - 1) / (c - 1);
                }
                else
                {
                    bf = 0.68 * Math.Pow(c, (-0.15)) - 0.22;
                }
            else
                //płaszczyzna wielopłaszczyznowa
                if (c == 1)
            {
                bf = 0.5;
            }
            else
            {
                bf = (Math.Pow(c, 0.5) - 1) / (c - 1);
            }

            uc = 0.0167 * (UnitConvertion.CMHToCFM(q) / (s1 * bf));

            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * UnitConvertion.MToFt(h) / uc;

                if (st <= 25)
                {
                    kd = -36.3 - 10.7 * Math.Log10(st);
                }
                else
                {
                    kd = -1.1 - 35.9 * Math.Log10(st);
                }

                loc[i] = kd + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(uc) + 10 * Math.Log10(s1) + 10 * Math.Log10(UnitConvertion.MToFt(h));
            }
            return loc;
        }

        public static double[] DamperRound(byte ang, double q, double d)
        {
            //tylko dla przepustnic jednopłaszczznowych i wielopłaszczyznowych współbieżnych
            //dp-całkowita strata ciśnienia na przepustnicy, Pa
            //q-wydajność na przepustnicy, m3/h
            //d-średnica przepustnicy, m
            //s-powierzchnia brutto przekroju przepustnicy, m2
            double c, bf, st, uc, s1, kd, dp;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] loc = new double[8];

            s1 = Math.PI * 0.25 * Math.Pow(UnitConvertion.MToFt(d), 2);
            dp = DamperRadDP(q, ang, d);
            c = 15.9 * Math.Pow(10, 6) * UnitConvertion.PaToInH20(dp) * Math.Pow((s1 / UnitConvertion.CMHToCFM(q)), 2);

            if (c < 4)
            {
                bf = (Math.Pow(c, 0.5) - 1) / (c - 1);
            }
            else
            {
                bf = 0.68 * Math.Pow(c, (-0.15)) - 0.22;
            }

            uc = 0.0167 * (UnitConvertion.CMHToCFM(q) / (s1 * bf));

            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * UnitConvertion.MToFt(d) / uc;

                if (st <= 25)
                {
                    kd = -36.3 - 10.7 * Math.Log10(st);
                }
                else
                {
                    kd = -1.1 - 35.9 * Math.Log10(st);
                }

                loc[i] = kd + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(uc) + 10 * Math.Log10(s1) + 10 * Math.Log10(UnitConvertion.MToFt(d));
            }
            return loc;
        }

        public static double[] Elbow(TurnigVanes trn_van, byte number_of_vanes, double q, double w, double h, double r)
        {
            //q-wydajność, m3/h
            //w-szerokość kolana (wymiar prostopadły do kierumku prowadzenia łopatek), m
            //h-wysokość kolana, m
            //r-promień zaokrąglenia kolana/kierownicy, m
            double c, bf, st, uc, s1, kd, dp, cd;
            double n = new double();
            double lb, dr, kj, ub, db, rd;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = new double[8];
            s1 = UnitConvertion.MToFt(h) * UnitConvertion.MToFt(w);

            if (trn_van == 0)
            {
                //kolano bez kierownic powietrza
                for (int i = 0; i < 8; i++)
                {
                    db = Math.Pow((4 * s1 / Math.PI), 0.5);
                    ub = UnitConvertion.CMHToCFM(q) / (60 * (s1));
                    st = oct[i] * db / ub;
                    kj = -21.61 + 12.388 - 16.482 * Math.Log10(st) - 5.047 * Math.Pow((Math.Log10(st)), 2);
                    lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(s1) + 10 * Math.Log10(db);
                    rd = (r * 100 / 2.54) / (12 * db);
                    dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                    lwf[i] = lb + dr;
                }
            }
            else
            {
                //kolano z kierownicami powietrza
                if (number_of_vanes == 0)
                {
                    n = ElbowTurnVanDP(q, w, h, r)[0];
                }
                else
                {
                    n = number_of_vanes;
                }

                dp = ElbowTurnVanDP(q, w, h, r)[1];
                c = 15.9 * Math.Pow(10, 6) * UnitConvertion.PaToInH20(dp) * Math.Pow((s1 / UnitConvertion.CMHToCFM(q)), 2);

                if (n == 1)
                {
                    cd = 0;
                }
                else
                {
                    cd = Math.Pow(2, 0.5) * 100 * (w + 0.4 * r / w * w) / (n + 1);
                }

                if (c == 1)
                {
                    bf = 0.5;
                }
                else
                {
                    bf = (Math.Pow(c, 0.5) - 1) / (c - 1);
                }

                for (int i = 0; i < 8; i++)
                {
                    uc = 0.0167 * (UnitConvertion.CMHToCFM(q) / (s1 * bf));
                    st = oct[i] * UnitConvertion.MToFt(w) / uc;

                    if (st >= 1)
                    {
                        kd = -47.5 - 7.69 * Math.Pow((Math.Log10(st)), 2.5);
                    }
                    else
                    {
                        double stc = Math.Log(st + 100);
                        kd = -1.7763 * Math.Pow((stc), 5) + 55.08 * Math.Pow((stc), 4) - 682.71 * Math.Pow((stc), 3) + 4227.1 * Math.Pow((stc), 2) - 13115 * (stc) + 16286.4;
                    }
                    lwf[i] = kd + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(uc) + 10 * Math.Log10(s1) + 10 * Math.Log10(cd / 2.54) + 10 * Math.Log10(n);
                }
            }
            return lwf;
        }

        public static double[] BowRectangular(double q, double w, double h, double r)
        {
            //q-wydajność, m3/h
            //w-szerokość kolana, m
            //h-wysokość kolana, m
            //r-promień gięcia łuku, m
            double st, s1, lb, dr, kj, ub, db, rd, r1;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = new double[8];

            s1 = UnitConvertion.MToFt(h) * UnitConvertion.MToFt(w);
            r1 = r - w / 2;
            db = Math.Pow((4 * s1 / Math.PI), 0.5);
            ub = UnitConvertion.CMHToCFM(q) / (60 * (s1));
            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * db / ub;
                kj = -21.61 + 12.388 - 16.482 * Math.Log10(st) - 5.047 * Math.Pow((Math.Log10(st)), 2);
                lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(s1) + 10 * Math.Log10(db);
                rd = (100 * r1 / 2.54) / (12 * db);
                dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                lwf[i] = lb + dr;
            }
            return lwf;
        }

        public static double[] BowRound(double q, double d, double r)
        {
            //q-wydajność, m3/h
            //d-średnica kolana, m
            //r-promień gięcia łuku, m
            double st, s1, r1, lb, dr, kj, ub, rd;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = new double[8];

            r1 = r - d / 2;
            s1 = 0.25 * Math.Pow((UnitConvertion.MToFt(d)), 2) * Math.PI;
            ub = UnitConvertion.CMHToCFM(q) / (60 * (s1));
            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * UnitConvertion.MToFt(d) / ub;
                kj = -21.61 + 12.388 - 16.482 * Math.Log10(st) - 5.047 * Math.Pow((Math.Log10(st)), 2);
                lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(s1) + 10 * Math.Log10(d);
                rd = (100 * r1 / 2.54) / (12 * UnitConvertion.MToFt(d));
                dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                lwf[i] = lb + dr;
            }
            return lwf;
        }

        public static double[] Junction(BranchDirection which_branch, double qb, double qm, double ab, double am, double r, Turbulence turbulence)
        {
            //qm-przepływ powietrza w przewodzie głównym, m3/h
            //qb-przepływ powietrza w odgałęzieniu, m3/h
            //ab-powierzchnia przekroju odgałęzienia, m2
            //am-powierzchnia przekroju przewodu głównego, m2
            //r-promień zaokrąglenia odgałęzienia, m
            double st, lb, dr, dt, kj, ub, um, rd, db, dm, a1, q1;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = new double[8];

            dm = Math.Pow((4 * (am * Math.Pow(UnitConvertion.MToFt(1), 2)) / Math.PI), 0.5);
            um = UnitConvertion.CMHToCFM(qm) / (60 * am * (Math.Pow(UnitConvertion.MToFt(1), 2)));
            a1 = ab * Math.Pow(UnitConvertion.MToFt(1), 2);
            q1 = UnitConvertion.CMHToCFM(qb);
            db = Math.Pow((4 * a1 / Math.PI), 0.5);
            ub = q1 / (60 * (a1));
            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * db / ub;
                kj = -21.61 + 12.388 * Math.Pow((um / ub), 0.673) - 16.482 * Math.Pow((um / ub), (-0.303)) * Math.Log10(st) - 5.047 * Math.Pow((um / ub), (-0.254)) * Math.Pow((Math.Log10(st)), 2);
                lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(a1) + 10 * Math.Log10(db);
                rd = (100 * r / 2.54) / (12 * db);
                dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                dt = -1.667 + 1.8 * (um / ub) - 0.133 * Math.Pow((um / ub), 2);

                if (which_branch == BranchDirection.BranchRight || which_branch == BranchDirection.BranchLeft)
                {
                    if (turbulence == Turbulence.No)
                    {
                        lwf[i] = lb + dr;
                    }
                    else
                    {
                        lwf[i] = lb + dr + dt;
                    }
                }
                else
                {
                    if (turbulence == Turbulence.No)
                    {
                        lwf[i] = lb + dr + 20 * Math.Log10(dm / db);
                    }
                    else
                    {
                        lwf[i] = lb + dr + dt + 20 * Math.Log10(dm / db);
                    }
                }
            }
            return lwf;
        }

        public static double[] TJunction(BranchDirection which_branch, double q1, double q2, double a1, double a2, double am, double r1, double r2, Turbulence turbulence)
        {
            //q1-przepływ powietrza w odgałęzieniu nr 1, m3/h
            //q2-przepływ powietrza w odgałęzieniu nr 2, m3/h
            //a1-powierzchnia przekroju odgałęzienia nr 1, m2
            //a2-powierzchnia przekroju odgałęzienia nr 2, m2
            //am-powierzchnia przekroju przewodu głównego, m2
            //r1-promień zaokrąglenia odgałęzienia nr 1, m
            //r2-promień zaokrąglenia odgałęzienia nr 2, m
            double st, lb, dr, dt, kj, ub, um, rd, db, dm, qm, ab, qb, r;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = { -1000, -1000, -1000, -1000, -1000, -1000, -1000, -1000 };
            qm = UnitConvertion.CMHToCFM(q1 + q2);
            dm = Math.Pow((4 * (am * Math.Pow(UnitConvertion.MToFt(1), 2)) / Math.PI), 0.5);
            um = qm / (60 * (am * Math.Pow(UnitConvertion.MToFt(1), 2)));

            if (which_branch == BranchDirection.BranchRight || which_branch == BranchDirection.BranchLeft)
            {
                if (which_branch == BranchDirection.BranchRight)
                {
                    ab = a1 * Math.Pow(UnitConvertion.MToFt(1), 2);
                    qb = UnitConvertion.CMHToCFM(q1);
                    r = r1;
                }
                else
                {
                    ab = a2 * Math.Pow(UnitConvertion.MToFt(1), 2);
                    qb = UnitConvertion.CMHToCFM(q2);
                    r = r2;
                }

                db = Math.Pow((4 * ab / Math.PI), 0.5);
                ub = qb / (60 * (ab));

                for (int i = 0; i < oct.Length; i++)
                {
                    st = oct[i] * db / ub;
                    kj = -21.61 + 12.388 * Math.Pow((um / ub), 0.673) - 16.482 * Math.Pow((um / ub), (-0.303)) * Math.Log10(st) - 5.047 * Math.Pow((um / ub), (-0.254)) * Math.Pow((Math.Log10(st)), 2);
                    lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(ab) + 10 * Math.Log10(db);
                    rd = (100 * r / 2.54) / (12 * db);
                    dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                    dt = -1.667 + 1.8 * (um / ub) - 0.133 * Math.Pow((um / ub), 2);

                    if (turbulence == Turbulence.No)
                    {
                        lwf[i] = lb + dr;
                    }
                    else
                    {
                        lwf[i] = lb + dr + dt;
                    }
                }
            }
            else
            {
                double[] tab = { a1, q1, r1, a2, q2, r2 };

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < tab.Length; j += 3)
                    {
                        db = Math.Pow((4 * tab[j] * Math.Pow(UnitConvertion.MToFt(1), 2) / Math.PI), 0.5);
                        ub = UnitConvertion.CMHToCFM(tab[j + 1] / (60 * (tab[j] * Math.Pow(UnitConvertion.MToFt(1), 2))));
                        st = oct[i] * db / ub;
                        kj = -21.61 + 12.388 * Math.Pow((um / ub), 0.673) - 16.482 * Math.Pow((um / ub), (-0.303)) * Math.Log10(st) - 5.047 * Math.Pow((um / ub), (-0.254)) * Math.Pow((Math.Log10(st)), 2);
                        lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(tab[j] * Math.Pow(UnitConvertion.MToFt(1), 2)) + 10 * Math.Log10(db);
                        rd = (100 * tab[j + 2] / 2.54) / (12 * db);
                        dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                        dt = -1.667 + 1.8 * (um / ub) - 0.133 * Math.Pow((um / ub), 2);

                        if (turbulence == Turbulence.No)
                        {
                            lwf[i] = MathOperation.DecibelAdd(lwf[i], lb + dr);
                        }
                        else
                        {
                            lwf[i] = MathOperation.DecibelAdd(lwf[i], lb + dr + dr);
                        }
                    }
                }
            }
            return lwf;
        }

        public static double[] DoubleJunction(BranchDirection which_branch, double q1, double q2, double qm, double a1, double a2, double am, double r1, double r2, Turbulence turbulence)
        {
            //q1-przepływ powietrza w odgałęzieniu nr 1, m3/h
            //q2-przepływ powietrza w odgałęzieniu nr 2, m3/h
            //qm-przepływ powietrza w przewodzie głównym, m3/h
            //a1-powierzchnia przekroju odgałęzienia nr 1, m2
            //a2-powierzchnia przekroju odgałęzienia nr 2, m2
            //am-powierzchnia przekroju przewodu głównego, m2
            //r1-promień zaokrąglenia odgałęzienia nr 1, m
            //r2-promień zaokrąglenia odgałęzienia nr 2, m
            double st, lb, dr, dt, kj, ub, um, rd, db, dm, ab, qb, r;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = { -1000, -1000, -1000, -1000, -1000, -1000, -1000, -1000 };
            dm = Math.Pow((4 * (am * Math.Pow(UnitConvertion.MToFt(1), 2)) / Math.PI), 0.5);
            um = UnitConvertion.CMHToCFM(qm) / (60 * (am * Math.Pow(UnitConvertion.MToFt(1), 2)));

            if (which_branch == BranchDirection.BranchRight || which_branch == BranchDirection.BranchLeft)
            {
                if (which_branch == BranchDirection.BranchRight)
                {
                    ab = a1 * Math.Pow(UnitConvertion.MToFt(1), 2);
                    qb = UnitConvertion.CMHToCFM(q1);
                    r = r1;
                }
                else
                {
                    ab = a2 * Math.Pow(UnitConvertion.MToFt(1), 2);
                    qb = UnitConvertion.CMHToCFM(q2);
                    r = r2;
                }

                db = Math.Pow((4 * ab / Math.PI), 0.5);
                ub = qb / (60 * (ab));

                for (int i = 0; i < oct.Length; i++)
                {
                    st = oct[i] * db / ub;
                    kj = -21.61 + 12.388 * Math.Pow((um / ub), 0.673) - 16.482 * Math.Pow((um / ub), (-0.303)) * Math.Log10(st) - 5.047 * Math.Pow((um / ub), (-0.254)) * Math.Pow((Math.Log10(st)), 2);
                    lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(ab) + 10 * Math.Log10(db);
                    rd = (100 * r / 2.54) / (12 * db);
                    dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                    dt = -1.667 + 1.8 * (um / ub) - 0.133 * Math.Pow((um / ub), 2);

                    if (turbulence == Turbulence.No)
                    {
                        lwf[i] = lb + dr;
                    }
                    else
                    {
                        lwf[i] = lb + dr + dt;
                    }
                }
            }
            else
            {
                double[] tab = { a1, q1, r1, a2, q2, r2 };

                for (int i = 0; i < oct.Length; i++)
                {
                    for (int j = 0; j < tab.Length; j += 3)
                    {
                        db = Math.Pow((4 * tab[j] * Math.Pow(UnitConvertion.MToFt(1), 2) / Math.PI), 0.5);
                        ub = UnitConvertion.CMHToCFM(tab[j + 1]) / (60 * (tab[j] * Math.Pow(UnitConvertion.MToFt(1), 2)));
                        st = oct[i] * db / ub;
                        kj = -21.61 + 12.388 * Math.Pow((um / ub), 0.673) - 16.482 * Math.Pow((um / ub), (-0.303)) * Math.Log10(st) - 5.047 * Math.Pow((um / ub), (-0.254)) * Math.Pow((Math.Log10(st)), 2);
                        lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(tab[j] * Math.Pow(UnitConvertion.MToFt(1), 2)) + 10 * Math.Log10(db);
                        rd = (100 * tab[j + 2] / 2.54) / (12 * db);
                        dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                        dt = -1.667 + 1.8 * (um / ub) - 0.133 * Math.Pow((um / ub), 2);

                        if (turbulence == Turbulence.No)
                        {
                            lwf[i] = MathOperation.DecibelAdd(lwf[i], lb + dr + 20 * Math.Log10(dm / db));
                        }
                        else
                        {
                            lwf[i] = MathOperation.DecibelAdd(lwf[i], lb + dr + dt + 20 * Math.Log10(dm / db));
                        }
                    }
                }
            }
            return lwf;
        }

        public static double[] Duct(double q, double a)
        {
            //q-przepływ powietrza, m3/h
            //a-powierzchnia przekroju kanału, m2
            double v;
            double[] kw = { -4.5, -5, -6.5, -7.5, -8.5, -10, -13, -22 };
            double[] lw = new double[8];
            v = (q / 3600) / a;

            for (int i = 0; i < kw.Length; i++)
            {
                lw[i] = kw[i] + 10 + 50 * Math.Log10(v) + 10 * Math.Log10(a);
            }
            return lw;
        }

        public static double[] Silencer(double q, double a, double p)
        {
            //q-przepływ powietrza, m3/h
            //a-powierzchnia przekroju kanału, m2
            //p-procent wolnej przestrzeni w całkowitej powierzchni przekroju poprzecznego kratki (procent powierzchni efektywnej), %
            double v;
            double[] kw = { -4.5, -5, -6.5, -7.5, -8.5, -10, -13, -22 };
            double[] lw = new double[8];
            v = (UnitConvertion.CMHToCFM(q)) / (a * Math.Pow(UnitConvertion.MToFt(1), 2));

            for (int i = 0; i < kw.Length; i++)
            {
                lw[i] = kw[i] - 145 + 55 * Math.Log10(v) + 10 * Math.Log10(a * Math.Pow(UnitConvertion.MToFt(1), 2)) - 45 * Math.Log10(p / 100);
            }

            if (MathOperation.OctaveSum(lw) > MathOperation.OctaveSum(Duct(q, a)))
            {
                return lw;
            }
            else
            {
                return Duct(q, a);
            }
        }

        public static double[] Grill(GrillType grillType, double q, double a, double lOrifice, double wGrill, double hOrifice, double p)
        {
            //p-procent wolnej przestrzeni w całkowitej powierzchni przekroju poprzecznego kratki (procent powierzchni efektywnej), %
            //w-maksymalna szerość kratki, cm
            //l-długość pojedyncznej płyty w kratce, cm
            //h-wysokość pojedynczego otworu kratki, cm
            //dp-spadek ciśnienia na kratce
            //q-przepływ powietrza, m3/h
            //a-powierzchnia przekroju kanału przed kratką, m2
            double d, v, fp, c, m, s, dp;
            double[] l = { 1, 2, 3, 4, 5, 6, 7, 8 };
            double[] lw = new double[8];
            v = UnitConvertion.CMHToCFM(q) / (60 * a * Math.Pow(UnitConvertion.MToFt(1), 2));

            switch (grillType)
            {
                case GrillType.CircularSupplyWire:
                    dp = GrillWireDp(GrillType.CircularSupplyWire, q, a, p);
                    break;
                case GrillType.CircularSupplyPlate:
                    dp = GrillPlateDp(GrillType.CircularSupplyPlate, q, a, p);
                    break;
                case GrillType.CircularSupplySingleLouver:
                    dp = GrillOneblouverOrificeDp(GrillType.CircularSupplySingleLouver, q, a, wGrill, lOrifice, hOrifice, p);
                    break;
                case GrillType.CircularSupplyDoubleLouver:
                    dp = GrillTwoblouverOrificeDp(GrillType.CircularSupplyDoubleLouver, q, a, lOrifice, hOrifice, p);
                    break;
                case GrillType.RectangularSupplyWire:
                    dp = GrillWireDp(GrillType.RectangularSupplyWire, q, a, p);
                    break;
                case GrillType.RectangularSupplyPlate:
                    dp = GrillPlateDp(GrillType.RectangularSupplyPlate, q, a, p);
                    break;
                case GrillType.RectangularSupplySingleLouver:
                    dp = GrillOneblouverOrificeDp(GrillType.RectangularSupplySingleLouver, q, a, wGrill, lOrifice, hOrifice, p);
                    break;
                case GrillType.RectangularSupplyDoubleLouver:
                    dp = GrillTwoblouverOrificeDp(GrillType.RectangularSupplyDoubleLouver, q, a, lOrifice, hOrifice, p);
                    break;
                case GrillType.CircularExtractWire:
                    dp = GrillWireDp(GrillType.CircularExtractWire, q, a, p);
                    break;
                case GrillType.CircularExtractPlate:
                    dp = GrillPlateDp(GrillType.CircularExtractPlate, q, a, p);
                    break;
                case GrillType.CircularExtractSingleLouver:
                    dp = GrillOneblouverOrificeDp(GrillType.CircularExtractSingleLouver, q, a, wGrill, lOrifice, hOrifice, p);
                    break;
                case GrillType.CircularExtractDoubleLouver:
                    dp = GrillTwoblouverOrificeDp(GrillType.CircularExtractDoubleLouver, q, a, lOrifice, hOrifice, p);
                    break;
                case GrillType.RectangularExtractWire:
                    dp = GrillWireDp(GrillType.RectangularExtractWire, q, a, p);
                    break;
                case GrillType.RectangularExtractPlate:
                    dp = GrillPlateDp(GrillType.RectangularExtractPlate, q, a, p);
                    break;
                case GrillType.RectangularExtractSingleLouver:
                    dp = GrillOneblouverOrificeDp(GrillType.RectangularExtractSingleLouver, q, a, wGrill, lOrifice, hOrifice, p);
                    break;
                case GrillType.RectangularExtractDoubleLouver:
                default:
                    dp = GrillTwoblouverOrificeDp(GrillType.RectangularExtractDoubleLouver, q, a, lOrifice, hOrifice, p);
                    break;
            }

            d = 334.9 * UnitConvertion.PaToInH20(dp) / (0.075 * Math.Pow(v, 2));
            fp = 48.8 * v;

            switch (Oct_BFI(fp))
            {
                case 63.0:
                    m = 1;
                    break;
                case 125.0:
                    m = 2;
                    break;
                case 250.0:
                    m = 3;
                    break;
                case 500.0:
                    m = 4;
                    break;
                case 1000.0:
                    m = 5;
                    break;
                case 2000.0:
                    m = 6;
                    break;
                case 4000.0:
                    m = 7;
                    break;
                case 8000.0:
                    m = 8;
                    break;
                default:
                    m = 0;
                    break;
            }

            for (int i = 0; i < l.Length; i++)
            {
                s = l[i] - m;

                switch (grillType)
                {
                    case GrillType.CircularSupplyWire:
                    case GrillType.CircularSupplyPlate:
                    case GrillType.CircularSupplySingleLouver:
                    case GrillType.CircularSupplyDoubleLouver:
                    case GrillType.CircularExtractWire:
                    case GrillType.CircularExtractPlate:
                    case GrillType.CircularExtractSingleLouver:
                    case GrillType.CircularExtractDoubleLouver:
                        c = -5.82 - 0.15 * s - 1.13 * Math.Pow(s, 2);
                        break;
                    default:
                        c = -11.82 - 0.15 * s - 1.13 * Math.Pow(s, 2);
                        break;
                }

                lw[i] = 10 * Math.Log10(a * Math.Pow(UnitConvertion.MToFt(1), 2)) + 30 * Math.Log10(d) + 60 * Math.Log10(v) - 31.3 + c;
            }
            return lw;
        }

        #endregion

        #region Methods

        private static double BFI(double rpm, double oct, double blade, double n)
        {
            double bf;
            bf = (rpm * blade) / 60;

            if (Oct_BFI(bf) == oct)
            { return n; }
            else
            { return 0; }
        }

        private static double DamperDP(double n, double q, double ang, double w, double h)
        {
            //Q-przepływ powietrza, m3/h
            //w-szerokość przepustnicy (wymiar równoległy do osi obrotu łopatek), m
            //h-wysokość przepustnicy, m
            //ang-kąt ustawienia przepustnicy (od 0st. do 70st. - jednopłaszczyznowa/od 0st. do 80st. - wielopłaszczyznowa)
            //h/w=od 0.1 do 5
            //LR=od 0.3 do 1.5
            //n-liczba łopatek przepustnicy
            double dt_kon = new double();
            double ang1, v, dt1, dt2, dt3, lr;
            double dt4, dt5, dt6, dt7, dt8, dt9, dt10;

            ang1 = ang / 25;
            v = (q / 3600) / (w * h);

            if (n == 0 || n == 1)
            {
                //przepustnica jednopłaszczyznowa
                dt1 = Math.Exp(0.1383 * Math.Pow(ang1, 5) - 1.1265 * Math.Pow(ang, 4) + 3.5854 * Math.Pow(ang1, 3) - 5.5999 * Math.Pow(ang1, 2) + 6.8212 * (ang1) - 3.2223);
                dt2 = Math.Exp(-0.0383 * Math.Pow(ang1, 5) + 0.23 * Math.Pow(ang1, 4) - 0.2955 * Math.Pow(ang1, 3) - 0.4917 * Math.Pow(ang1, 2) + 3.8143 * (ang1) - 2.5284);
                dt3 = Math.Exp(-0.169 * Math.Pow(ang1, 5) + 1.2375 * Math.Pow(ang1, 4) - 3.187 * Math.Pow(ang1, 3) + 3.305 * Math.Pow(ang1, 2) + 1.6272 * (ang1) - 2.0425);

                if (h / w < 0.25)
                {
                    dt_kon = dt1 + (dt2 - dt1) / (1 - 0.25) * (h / w - 0.25);
                }
                else if (h / w >= 0.25 && h / w <= 1)
                {
                    dt_kon = dt2;
                }
                else if (h / w > 1 && h / w <= 2)
                {
                    dt_kon = dt2 + (dt3 - dt2) / (2 - 1) * (h / w - 1);
                }
                else
                {
                    dt_kon = dt3 + (dt3 - dt2) / (2 - 1) * (h / w - 2);
                }
            }
            else
            {
                //przepustnica wielopłaszczyznowa
                lr = n * w / (2 * (w + h));
                dt4 = Math.Exp(-0.0075 * Math.Pow(ang1, 6) + 0.1499 * Math.Pow(ang1, 5) - 0.7451 * Math.Pow(ang1, 4) + 1.405 * Math.Pow(ang1, 3) - 0.87188 * Math.Pow(ang1, 2) + 1.3239 * (ang1) - 0.66);
                dt5 = Math.Exp(0.0073 * Math.Pow(ang1, 6) + 0.0161 * Math.Pow(ang1, 5) - 0.3276 * Math.Pow(ang1, 4) + 0.9373 * Math.Pow(ang1, 3) - 0.8558 * Math.Pow(ang1, 2) + 1.5121 * (ang1) - 0.6594);
                dt6 = Math.Exp(0.0362 * Math.Pow(ang1, 6) - 0.2617 * Math.Pow(ang1, 5) + 0.64 * Math.Pow(ang1, 4) - 0.5072 * Math.Pow(ang1, 3) - 0.0287 * Math.Pow(ang1, 2) + 1.4517 * (ang1) - 0.6582);
                dt7 = Math.Exp(0.0639 * Math.Pow(ang1, 6) - 0.511 * Math.Pow(ang1, 5) + 1.4687 * Math.Pow(ang1, 4) - 1.7024 * Math.Pow(ang1, 3) + 0.629 * Math.Pow(ang1, 2) + 1.4155 * (ang1) - 0.6565);
                dt8 = Math.Exp(-0.0008 * Math.Pow(ang1, 6) + 0.0594 * Math.Pow(ang1, 5) - 0.4128 * Math.Pow(ang1, 4) + 1.1876 * Math.Pow(ang1, 3) - 1.4408 * Math.Pow(ang1, 2) + 1.9957 * (ang1) - 0.6567);
                dt9 = Math.Exp(-0.0054 * Math.Pow(ang1, 6) + 0.1068 * Math.Pow(ang1, 5) - 0.5984 * Math.Pow(ang1, 4) + 1.5604 * Math.Pow(ang1, 3) - 1.8496 * Math.Pow(ang1, 2) + 2.2135 * (ang1) - 0.6568);
                dt10 = Math.Exp(-0.0255 * Math.Pow(ang1, 6) + 0.2855 * Math.Pow(ang1, 5) - 1.2281 * Math.Pow(ang1, 4) + 2.7208 * Math.Pow(ang1, 3) - 2.9914 * Math.Pow(ang1, 2) + 2.7019 * (ang1) - 0.6562);

                if (lr == 0.3)
                {
                    dt_kon = dt4;
                }
                else if (lr > 0.3 && lr <= 0.4)
                {
                    dt_kon = dt4 + (dt5 - dt4) / (0.4 - 0.3) * (lr - 0.3);
                }
                else if (lr > 0.4 && lr <= 0.5)
                {
                    dt_kon = dt5 + (dt6 - dt5) / (0.5 - 0.4) * (lr - 0.4);
                }
                else if (lr > 0.5 && lr <= 0.6)
                {
                    dt_kon = dt6 + (dt7 - dt6) / (0.6 - 0.5) * (lr - 0.5);
                }
                else if (lr > 0.6 && lr <= 0.8)
                {
                    dt_kon = dt7 + (dt8 - dt7) / (0.8 - 0.6) * (lr - 0.6);
                }
                else if (lr > 0.8 && lr <= 1.0)
                {
                    dt_kon = dt8 + (dt9 - dt8) / (1.0 - 0.8) * (lr - 0.8);
                }
                else if (lr > 1.0 && lr <= 1.5)
                {
                    dt_kon = dt9 + (dt10 - dt9) / (1.5 - 1.0) * (lr - 1.0);
                }
            }

            return dt_kon * 0.6 * Math.Pow(v, 2);
        }

        private static double DamperRadDP(double q, double ang, double d)
        {
            //q-przepływ powietrza, m3/h
            //w-szerokość przepustnicy (wymiar równoległy do osi obrotu łopatek), m
            //d-średnica przepustnicy, m
            //ang-kąt ustawienia przepustnicy (od 0st. do 70st. - jednopłaszczyznowa/od 0st. do 80st. - wielopłaszczyznowa)
            double ang1, v, dt;
            ang1 = ang / 25;
            v = (q / 3600) / (Math.PI * 0.25 * Math.Pow(d, 2));
            if (ang < 10)
            {
                dt = 0.2 + (0.52 - 0.2) / 10 * ang;
            }
            else
            {
                dt = 9.691 * Math.Pow(ang1, 6) - 52.938 * Math.Pow(ang1, 5) + 111.16 * Math.Pow(ang1, 4) - 107.31 * Math.Pow(ang1, 3) + 49.466 * Math.Pow(ang1, 2) - 7.675 * (ang1) + 0.2;
            }
            return dt * 0.6 * Math.Pow(v, 2);
        }

        private static double[] ElbowTurnVanDP(double q, double w, double h, double r)
        {
            //q-przepływ powietrza, m3/h
            //w-szerokość kolana (wymiar prostopadły do kierumku prowadzenia łopatek), m
            //h-wysokość kolana, m
            //r-promień zaokrąglenia kolana/kierownicy, m
            //od r/w=0 do r/w=0.6
            double v, dt, n, dh;
            double[] eq = new double[2];

            dh = 2 * h * w / (h + w);
            v = (q / 3600) / (w * h);

            if (r == 0)
            {
                dt = 0.45;
            }
            else
            {
                dt = -222.22 * Math.Pow((r / w), 6) + 408.33 * Math.Pow((r / w), 5) - 280.56 * Math.Pow((r / w), 4) + 87.917 * Math.Pow((r / w), 3) - 10.172 * Math.Pow((r / w), 2) - 0.62 * (r / w) + 0.33;
            }

            if (!(r == 0))
            {
                if (!(Math.Round(2.13 * Math.Pow((r / w), -1) - 1) <= 1))
                {
                    n = Math.Round(2.13 * Math.Pow((r / w), -1) - 1);
                }
                else
                {
                    n = 1;
                }
            }
            else
            {
                if (!(Math.Round(2.13 * Math.Pow((0.35 * dh / (w * Math.Pow(2, 0.5))), (-1)) - 1) <= 1))
                {
                    n = Math.Round(2.13 * Math.Pow((0.35 * dh / (w * Math.Pow(2, 0.5))), (-1)) - 1);
                }
                else
                {
                    n = 1;
                }
            }
            eq[0] = n;
            eq[1] = (dt + 0.02 + 0.031 * (r / w)) * 0.6 * Math.Pow(v, 2);
            return eq;
        }

        private static double GrillPlateDp(GrillType grillType, double q, double a, double p)
        {
            //q-przepływ powietrza, m3/h
            //a-powierzchnia brutto przekroju kratki, m2
            //p-procent wolnej przestrzeni w całkowitej powierzchni przekroju poprzecznego kratki (procent powierzchni efektywnej), %
            if (grillType == GrillType.RectangularExtractPlate || grillType == GrillType.CircularExtractPlate)
            {
                return Math.Pow(((q / 3600) / a), 2) * 0.6 * Math.Pow((1.707 - p / 100), 2) * 1 / (Math.Pow((p / 100), 2));
            }
            else
            {
                return Math.Pow(((q / 3600) / a), 2) * 0.6 * Math.Pow((1 + 0.707 * Math.Pow((1 - p / 100), 0.5)), 2) * 1 / (Math.Pow((p / 100), 2));
            }
        }

        private static double GrillWireDp(GrillType grillType, double q, double a, double p)
        {
            //q-przepływ powietrza, m3/h
            //a-powierzchnia brutto przekroju kratki, m2
            //p-procent wolnej przestrzeni w całkowitej powierzchni przekroju poprzecznego kratki (procent powierzchni efektywnej), %
            if (grillType == GrillType.RectangularExtractWire || grillType == GrillType.CircularExtractWire)
            {
                return (0.5 + 1.3 * (1 - p / 100) + Math.Pow((p / 100 - 1), 2)) * Math.Pow(((q / 3600) / a), 2) * 0.6;
            }
            else
            {
                return (1.0 + 1.3 * (1 - p / 100) + Math.Pow((p / 100 - 1), 2)) * Math.Pow(((q / 3600) / a), 2) * 0.6;
            }
        }

        private static double GrillTwoblouverOrificeDp(GrillType grillType, double q, double a, double l, double h, double p)
        {
            //q-przepływ powietrza, m3/h
            //a-powierzchnia brutto przekroju kratki, m2
            //l-długość pojedyncznej płyty w kratce, cm
            //h-wysokość pojedynczego otworu kratki, cm
            //p-procent wolnej przestrzeni w całkowitej powierzchni przekroju poprzecznego kratki (procent powierzchni efektywnej), %
            //l/dh>0.015
            double m, dh;
            dh = h;

            if (l / dh < 2.4)
            {
                m = 0.2484 * Math.Pow((l / dh), 5) - 1.8334 * Math.Pow((l / dh), 4) + 4.8169 * Math.Pow((l / dh), 3) - 4.9355 * Math.Pow((l / dh), 2) + 0.6499 * (l / dh) + 1.3263;
            }
            else
            {
                m = 0;
            }

            if (grillType == GrillType.RectangularExtractDoubleLouver || grillType == GrillType.CircularExtractDoubleLouver)
            {
                return 0.6 * Math.Pow(((q / 3600) / a), 2) * (0.5 + m * (1 - p / 100) + Math.Pow((1 - p / 100), 2) + Lam(20, h / 100, h / 100, ((q / 3600) / (a * p / 100)), 0.0001) * l / dh) * Math.Pow((1 / (p / 100)), 2);
            }
            else
            {
                return 0.6 * Math.Pow(((q / 3600) / a), 2) * ((1 + m * Math.Pow((1 - p / 100), 0.5) + 0.5 * (1 - p / 100)) + Lam(20, h / 100, h / 100, ((q / 3600) / (a * p / 100)), 0.0001) * l / dh) * Math.Pow((1 / (p / 100)), 2);
            }
        }

        private static double GrillOneblouverOrificeDp(GrillType grillType, double q, double a, double w, double l, double h, double p)
        {
            //q-przepływ powietrza, m3/h
            //a-powierzchnia brutto przekroju kratki, m2
            //w-maksymalna szerość kratki, cm
            //l-długość pojedyncznej płyty w kratce, cm
            //h-wysokość pojedynczego otworu kratki, cm
            //p-procent wolnej przestrzeni w całkowitej powierzchni przekroju poprzecznego kratki (procent powierzchni efektywnej), %
            //l/dh>0.015
            double m, dh;

            if (grillType == GrillType.RectangularSupplySingleLouver || grillType == GrillType.RectangularExtractSingleLouver)
            {
                dh = 2 * h * w / (h + w);
            }
            else
            {
                dh = 2 * h * (4 * (w / 2) / (3 * Math.PI)) / (h + (4 * (w / 2) / (3 * Math.PI)));
            }

            if (l / dh < 2.4)
            {
                m = 0.2484 * Math.Pow((l / dh), 5) - 1.8334 * Math.Pow((l / dh), 4) + 4.8169 * Math.Pow((l / dh), 3) - 4.9355 * Math.Pow((l / dh), 2) + 0.6499 * (l / dh) + 1.3263;
            }
            else
            {
                m = 0;
            }

            if (grillType == GrillType.CircularExtractSingleLouver || grillType == GrillType.RectangularExtractSingleLouver)
            {
                return 0.6 * Math.Pow(((q / 3600) / a), 2) * (0.5 + m * (1 - p / 100) + Math.Pow((1 - p / 100), 2) + Lam(20, h / 100, h / 100, ((q / 3600) / (a * p / 100)), 0.0001) * l / dh) * Math.Pow((1 / (p / 100)), 2);
            }
            else
            {
                return 0.6 * Math.Pow(((q / 3600) / a), 2) * ((1 + m * Math.Pow((1 - p / 100), 0.5) + 0.5 * (1 - p / 100)) + Lam(20, h / 100, h / 100, ((q / 3600) / (a * p / 100)), 0.0001) * l / dh) * Math.Pow((1 / (p / 100)), 2);
            }
        }

        private static double Lam(double t, double a, double b, double v, double k)
        {
            double e, f, f1, x0, val;
            e = 0.00001;
            x0 = 0.000000001;
            f = Base(x0, t, a, b, v, k);
            f1 = Derivative(x0, t, a, b, v, k);
            val = x0 - f / f1;

            while (Math.Abs(f) > e)
            {
                x0 = val;
                f = Base(x0, t, a, b, v, k);
                f1 = Derivative(x0, t, a, b, v, k);
                val = x0 - f / f1;
            }
            return val;
        }

        private static double Base(double lm, double t, double a, double b, double v, double k)
        {
            double d, w;
            d = 2 * a * b / (a + b);
            w = v / (3600 * a * b);
            return -0.5 * Math.Pow(lm, (-0.5)) - Math.Log(2.51 / (Math.Pow(lm, 0.5) * Re(t, w, d)) + k / (3.72 * d)) / Math.Log(10);
        }

        private static double Derivative(double lm, double t, double a, double b, double v, double k)
        {
            double d, w;
            d = 2 * a * b / (a + b);
            w = v / (3600 * a * b);
            return 0.25 * Math.Pow(lm, (-1.5)) + 2.027 * Math.Pow(lm, (-1)) / (9.337 + Re(t, w, d) * k / d * Math.Pow(lm, 0.5));
        }

        private static double Re(double t, double w, double d)
        {
            return d * w / Lep(t);
        }

        private static double Lep(double t)
        {
            return (0.00008 * Math.Pow(t, 2) + 0.0905 * t + 13.421) * Math.Pow(10, -6);
        }

        #endregion

    }
}
