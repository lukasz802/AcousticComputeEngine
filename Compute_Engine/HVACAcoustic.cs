using System;

namespace HVACAcoustic
{
    public enum FanType
    {
        CentrifugalBackwardCurved = 1,
        CentrifugalRadial = 2,
        CentrifugalForwardCurved = 3,
        VaneAxial = 4,
        TubeAxial = 5,
        Propeller = 6,
        TubularCentrifugal = 7
    }

    public enum TurnigVanes
    {
        No = 0,
        Yes = 1
    }

    public enum Turbulence
    {
        No = 0,
        Yes = 1
    }

    public enum Branch
    {
        BranchRight = 1,
        BranchLeft = 2,
        Main = 0
    }

    public enum BranchType
    {
        Straight = 0,
        Rounded = 1
    }

    public enum GrillType
    {
        CircularSupplyWire = 0,
        CircularSupplyPlate = 1,
        CircularSupplySingleLouver = 2,
        CircularSupplyDoubleLouver = 3,
        RectangularSupplyWire = 4,
        RectangularSupplyPlate = 5,
        RectangularSupplySingleLouver = 6,
        RectangularSupplyDoubleLouver = 7,
        CircularExtractWire = 8,
        CircularExtractPlate = 9,
        CircularExtractSingleLouver = 10,
        CircularExtractDoubleLouver = 11,
        RectangularExtractWire = 12,
        RectangularExtractPlate = 13,
        RectangularExtractSingleLouver = 14,
        RectangularExtractDoubleLouver = 15,
    }

    public enum GrillLocation
    {
        FreeSpace = 0,
        FlushWall = 1,
    }

    public enum Lining
    {
        No = 0,
        Yes = 1,
    }

    public enum LiningType
    {
        Concrete = 0,
        Steel = 1,
        Fiberglass = 2,
    }

    public enum CeiligType
    {
        Gypboard_10mm = 0,
        Gypboard_12mm = 1,
        Gypboard_15mm = 2,
        Gypboard_25mm = 3,
        AcousticalCeilingTileExposedTBarGridSuspendedLight_60x120x10mm = 4,
        AcousticalCeilingTileExposedTBarGridSuspendedHeavy_60x120x10mm = 5,
        AcousticalCeilingTileExposedTBarGridSuspended_60x120x15mm = 6,
        AcousticalCeilingTileExposedTBarGridSuspended_60x60x10mm = 7,
        AcousticalCeilingTileConcealedSplineSuspended = 8,
    }

    public enum CeilingConfiguration
    {
        IntegratedLightingAndDiffuserSystem = 0,
        NoIntegratedLightingAndDiffuserSystem = 1,
    }

    public enum RoomType
    {
        Dead = 0,
        MediumDead = 1,
        Average = 2,
        MediumLive = 3,
        Live = 4,
    }

    public static class MathOperations
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

            for (int i=0; i < val.Length; i++)
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

    public static class Unit
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

    public static class Criteria
    {
        public static double[] FilterA()
        {
            double[] loc = { -26.2, -16.1, -8.6, -3.2, 0, 1.2, 1.0, -1.1 };
            return loc;
        }

        public static double[] FilterB()
        {
            double[] loc = { -9.3, -4.2, -1.3, -0.3, 0, -0.1, -0.7, -2.9 };
            return loc;
        }

        public static double[] FilterC()
        {
            double[] loc = { -0.8, -0.2, 0, 0, 0, -0.2, -0.8, -3 };
            return loc;
        }

        public static double[] NC(double n)
        {
            double k63, k125, k250, k500, k1000, k2000, k4000, k8000;
            switch (n)
            {
                case 15.0:
                    k63 = 47;
                    k125 = 36;
                    k250 = 29;
                    k500 = 22;
                    k1000 = 17;
                    k2000 = 14;
                    k4000 = 12;
                    k8000 = 11;
                    break;
                case 20.0:
                    k63 = 51;
                    k125 = 40;
                    k250 = 33;
                    k500 = 26;
                    k1000 = 22;
                    k2000 = 19;
                    k4000 = 17;
                    k8000 = 16;
                    break;
                case 25.0:
                    k63 = 54;
                    k125 = 44;
                    k250 = 37;
                    k500 = 31;
                    k1000 = 27;
                    k2000 = 24;
                    k4000 = 22;
                    k8000 = 21;
                    break;
                case 30.0:
                    k63 = 57;
                    k125 = 48;
                    k250 = 41;
                    k500 = 35;
                    k1000 = 31;
                    k2000 = 29;
                    k4000 = 28;
                    k8000 = 27;
                    break;
                case 35.0:
                    k63 = 60;
                    k125 = 52;
                    k250 = 45;
                    k500 = 40;
                    k1000 = 36;
                    k2000 = 34;
                    k4000 = 33;
                    k8000 = 32;
                    break;
                case 40.0:
                    k63 = 64;
                    k125 = 56;
                    k250 = 50;
                    k500 = 45;
                    k1000 = 41;
                    k2000 = 39;
                    k4000 = 38;
                    k8000 = 37;
                    break;
                case 45.0:
                    k63 = 67;
                    k125 = 60;
                    k250 = 54;
                    k500 = 49;
                    k1000 = 46;
                    k2000 = 44;
                    k4000 = 43;
                    k8000 = 42;
                    break;
                case 50.0:
                    k63 = 71;
                    k125 = 64;
                    k250 = 58;
                    k500 = 54;
                    k1000 = 51;
                    k2000 = 49;
                    k4000 = 48;
                    k8000 = 47;
                    break;
                case 55.0:
                    k63 = 74;
                    k125 = 67;
                    k250 = 62;
                    k500 = 58;
                    k1000 = 56;
                    k2000 = 54;
                    k4000 = 53;
                    k8000 = 52;
                    break;
                case 60.0:
                    k63 = 77;
                    k125 = 71;
                    k250 = 67;
                    k500 = 63;
                    k1000 = 61;
                    k2000 = 59;
                    k4000 = 58;
                    k8000 = 57;
                    break;
                case 65.0:
                    k63 = 80;
                    k125 = 75;
                    k250 = 71;
                    k500 = 68;
                    k1000 = 66;
                    k2000 = 64;
                    k4000 = 63;
                    k8000 = 62;
                    break;
                case 70:
                default:
                    k63 = 83;
                    k125 = 79;
                    k250 = 75;
                    k500 = 72;
                    k1000 = 71;
                    k2000 = 70;
                    k4000 = 69;
                    k8000 = 68;
                    break;
            }
            double[] lw = { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
            return lw;
        }

        public static double[] NR(double n)
        {
            double k63, k125, k250, k500, k1000, k2000, k4000, k8000;
            switch (n)
            {
                case 0.0:
                    k63 = 36;
                    k125 = 22;
                    k250 = 12;
                    k500 = 5;
                    k1000 = 0;
                    k2000 = -4;
                    k4000 = -6;
                    k8000 = -8;
                    break;
                case 10.0:
                    k63 = 43;
                    k125 = 31;
                    k250 = 21;
                    k500 = 15;
                    k1000 = 10;
                    k2000 = 7;
                    k4000 = 4;
                    k8000 = 2;
                    break;
                case 20.0:
                    k63 = 51;
                    k125 = 39;
                    k250 = 31;
                    k500 = 24;
                    k1000 = 20;
                    k2000 = 17;
                    k4000 = 14;
                    k8000 = 13;
                    break;
                case 30.0:
                    k63 = 59;
                    k125 = 48;
                    k250 = 40;
                    k500 = 34;
                    k1000 = 30;
                    k2000 = 27;
                    k4000 = 25;
                    k8000 = 23;
                    break;
                case 40.0:
                    k63 = 67;
                    k125 = 57;
                    k250 = 49;
                    k500 = 44;
                    k1000 = 40;
                    k2000 = 37;
                    k4000 = 35;
                    k8000 = 33;
                    break;
                case 50.0:
                    k63 = 75;
                    k125 = 66;
                    k250 = 59;
                    k500 = 54;
                    k1000 = 50;
                    k2000 = 47;
                    k4000 = 45;
                    k8000 = 44;
                    break;
                case 60.0:
                    k63 = 83;
                    k125 = 74;
                    k250 = 68;
                    k500 = 63;
                    k1000 = 60;
                    k2000 = 57;
                    k4000 = 55;
                    k8000 = 54;
                    break;
                case 70.0:
                    k63 = 91;
                    k125 = 83;
                    k250 = 77;
                    k500 = 73;
                    k1000 = 70;
                    k2000 = 68;
                    k4000 = 66;
                    k8000 = 64;
                    break;
                case 80.0:
                    k63 = 99;
                    k125 = 92;
                    k250 = 86;
                    k500 = 83;
                    k1000 = 80;
                    k2000 = 78;
                    k4000 = 76;
                    k8000 = 74;
                    break;
                case 90.0:
                    k63 = 107;
                    k125 = 100;
                    k250 = 96;
                    k500 = 93;
                    k1000 = 90;
                    k2000 = 88;
                    k4000 = 86;
                    k8000 = 85;
                    break;
                case 100.0:
                    k63 = 115;
                    k125 = 109;
                    k250 = 105;
                    k500 = 102;
                    k1000 = 100;
                    k2000 = 98;
                    k4000 = 96;
                    k8000 = 95;
                    break;
                case 110.0:
                    k63 = 122;
                    k125 = 118;
                    k250 = 114;
                    k500 = 112;
                    k1000 = 110;
                    k2000 = 108;
                    k4000 = 107;
                    k8000 = 105;
                    break;
                case 120.0:
                    k63 = 130;
                    k125 = 126;
                    k250 = 124;
                    k500 = 122;
                    k1000 = 120;
                    k2000 = 118;
                    k4000 = 117;
                    k8000 = 116;
                    break;
                case 130.0:
                default:
                    k63 = 138;
                    k125 = 135;
                    k250 = 133;
                    k500 = 131;
                    k1000 = 130;
                    k2000 = 128;
                    k4000 = 127;
                    k8000 = 126;
                    break;
            }
            double[] lw = { k63, k125, k250, k500, k1000, k2000, k4000, k8000 };
            return lw;
        }
    }

    public static class Noise
    {
        public static double[] Fan(FanType type, double q, double dp, int rpm, byte blade, byte eff, byte io)
        {
            //hałas generowany przez wentylator
            //dp=Pa, q=m3/h, oct=Hz, rpm=rpm, eff=%
            //io-czy hałas generowany w dwie strony czy tylko strona ssawna/tłoczna
            double c;
            double qw = Unit.CMHToCFM(q);
            double dpw = Unit.PaToInH20(dp);
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

        private static double BFI(double rpm, double oct, double blade, double n)
        {
            double bf;
            bf = (rpm * blade) / 60;

            if (Oct_BFI(bf) == oct)
            { return n; }
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

            s1 = Unit.MToFt(h) * Unit.MToFt(w);
            dp = DamperDP(blade_number, q, ang, w, h);
            c = 15.9 * Math.Pow(10, 6) * Unit.PaToInH20(dp) * Math.Pow(s1 / Unit.CMHToCFM(q), 2);

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

            uc = 0.0167 * (Unit.CMHToCFM(q) / (s1 * bf));

            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * Unit.MToFt(h) / uc;

                if (st <= 25)
                {
                    kd = -36.3 - 10.7 * Math.Log10(st);
                }
                else
                {
                    kd = -1.1 - 35.9 * Math.Log10(st);
                }

                loc[i] = kd + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(uc) + 10 * Math.Log10(s1) + 10 * Math.Log10(Unit.MToFt(h));
            }
            return loc;
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

            s1 = Math.PI * 0.25 * Math.Pow(Unit.MToFt(d), 2);
            dp = DamperRadDP(q, ang, d);
            c = 15.9 * Math.Pow(10, 6) * Unit.PaToInH20(dp) * Math.Pow((s1 / Unit.CMHToCFM(q)), 2);

            if (c < 4)
            {
                bf = (Math.Pow(c, 0.5) - 1) / (c - 1);
            }
            else
            {
                bf = 0.68 * Math.Pow(c, (-0.15)) - 0.22;
            }

            uc = 0.0167 * (Unit.CMHToCFM(q) / (s1 * bf));

            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * Unit.MToFt(d) / uc;

                if (st <= 25)
                {
                    kd = -36.3 - 10.7 * Math.Log10(st);
                }
                else
                {
                    kd = -1.1 - 35.9 * Math.Log10(st);
                }

                loc[i] = kd + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(uc) + 10 * Math.Log10(s1) + 10 * Math.Log10(Unit.MToFt(d));
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
            s1 = Unit.MToFt(h) * Unit.MToFt(w);

            if (trn_van == 0)
            {
                //kolano bez kierownic powietrza
                for (int i = 0; i < 8; i++)
                {
                    db = Math.Pow((4 * s1 / Math.PI), 0.5);
                    ub = Unit.CMHToCFM(q) / (60 * (s1));
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
                c = 15.9 * Math.Pow(10, 6) * Unit.PaToInH20(dp) * Math.Pow((s1 / Unit.CMHToCFM(q)), 2);

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
                    uc = 0.0167 * (Unit.CMHToCFM(q) / (s1 * bf));
                    st = oct[i] * Unit.MToFt(w) / uc;

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

        public static double[] BowRectangular(double q, double w, double h, double r)
        {
            //q-wydajność, m3/h
            //w-szerokość kolana, m
            //h-wysokość kolana, m
            //r-promień gięcia łuku, m
            double st, s1, lb, dr, kj, ub, db, rd, r1;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = new double[8];

            s1 = Unit.MToFt(h) * Unit.MToFt(w);
            r1 = r - w / 2;
            db = Math.Pow((4 * s1 / Math.PI), 0.5);
            ub = Unit.CMHToCFM(q) / (60 * (s1));
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
            s1 = 0.25 * Math.Pow((Unit.MToFt(d)), 2) * Math.PI;
            ub = Unit.CMHToCFM(q) / (60 * (s1));
            for (int i = 0; i < 8; i++)
            {
                st = oct[i] * Unit.MToFt(d) / ub;
                kj = -21.61 + 12.388 - 16.482 * Math.Log10(st) - 5.047 * Math.Pow((Math.Log10(st)), 2);
                lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(s1) + 10 * Math.Log10(d);
                rd = (100 * r1 / 2.54) / (12 * Unit.MToFt(d));
                dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                lwf[i] = lb + dr;
            }
            return lwf;
        }

        public static double[] Junction(Branch which_branch, double qb, double qm, double ab, double am, double r, Turbulence turbulence)
        {
            //qm-przepływ powietrza w przewodzie głównym, m3/h
            //qb-przepływ powietrza w odgałęzieniu, m3/h
            //ab-powierzchnia przekroju odgałęzienia, m2
            //am-powierzchnia przekroju przewodu głównego, m2
            //r-promień zaokrąglenia odgałęzienia, m
            double st, lb, dr, dt, kj, ub, um, rd, db, dm, a1, q1;
            double[] oct = { 63, 125, 250, 500, 1000, 2000, 4000, 8000 };
            double[] lwf = new double[8];

            dm = Math.Pow((4 * (am * Math.Pow(Unit.MToFt(1), 2)) / Math.PI), 0.5);
            um = Unit.CMHToCFM(qm) / (60 * am * (Math.Pow(Unit.MToFt(1), 2)));
            a1 = ab * Math.Pow(Unit.MToFt(1), 2);
            q1 = Unit.CMHToCFM(qb);
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

                if (which_branch == Branch.BranchRight || which_branch == Branch.BranchLeft)
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

        public static double[] TJunction(Branch which_branch, double q1, double q2, double a1, double a2, double am, double r1, double r2, Turbulence turbulence)
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
            qm = Unit.CMHToCFM(q1 + q2);
            dm = Math.Pow((4 * (am * Math.Pow(Unit.MToFt(1), 2)) / Math.PI), 0.5);
            um = qm / (60 * (am * Math.Pow(Unit.MToFt(1), 2)));

            if (which_branch == Branch.BranchRight || which_branch == Branch.BranchLeft)
            {
                if (which_branch == Branch.BranchRight)
                {
                    ab = a1 * Math.Pow(Unit.MToFt(1), 2);
                    qb = Unit.CMHToCFM(q1);
                    r = r1;
                }
                else
                {
                    ab = a2 * Math.Pow(Unit.MToFt(1), 2);
                    qb = Unit.CMHToCFM(q2);
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
                        db = Math.Pow((4 * tab[j] * Math.Pow(Unit.MToFt(1), 2) / Math.PI), 0.5);
                        ub = Unit.CMHToCFM(tab[j + 1] / (60 * (tab[j] * Math.Pow(Unit.MToFt(1), 2))));
                        st = oct[i] * db / ub;
                        kj = -21.61 + 12.388 * Math.Pow((um / ub), 0.673) - 16.482 * Math.Pow((um / ub), (-0.303)) * Math.Log10(st) - 5.047 * Math.Pow((um / ub), (-0.254)) * Math.Pow((Math.Log10(st)), 2);
                        lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(tab[j] * Math.Pow(Unit.MToFt(1), 2)) + 10 * Math.Log10(db);
                        rd = (100 * tab[j + 2] / 2.54) / (12 * db);
                        dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                        dt = -1.667 + 1.8 * (um / ub) - 0.133 * Math.Pow((um / ub), 2);

                        if (turbulence == Turbulence.No)
                        {
                            lwf[i] = MathOperations.DecibelAdd(lwf[i], lb + dr);
                        }
                        else
                        {
                            lwf[i] = MathOperations.DecibelAdd(lwf[i], lb + dr + dr);
                        }
                    }
                }
            }
            return lwf;
        }

        public static double[] DoubleJunction(Branch which_branch, double q1, double q2, double qm, double a1, double a2, double am, double r1, double r2, Turbulence turbulence)
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
            dm = Math.Pow((4 * (am * Math.Pow(Unit.MToFt(1), 2)) / Math.PI), 0.5);
            um = Unit.CMHToCFM(qm) / (60 * (am * Math.Pow(Unit.MToFt(1), 2)));

            if (which_branch == Branch.BranchRight || which_branch == Branch.BranchLeft)
            {
                if (which_branch == Branch.BranchRight)
                {
                    ab = a1 * Math.Pow(Unit.MToFt(1), 2);
                    qb = Unit.CMHToCFM(q1);
                    r = r1;
                }
                else
                {
                    ab = a2 * Math.Pow(Unit.MToFt(1), 2);
                    qb = Unit.CMHToCFM(q2);
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
                        db = Math.Pow((4 * tab[j] * Math.Pow(Unit.MToFt(1), 2) / Math.PI), 0.5);
                        ub = Unit.CMHToCFM(tab[j + 1]) / (60 * (tab[j] * Math.Pow(Unit.MToFt(1), 2)));
                        st = oct[i] * db / ub;
                        kj = -21.61 + 12.388 * Math.Pow((um / ub), 0.673) - 16.482 * Math.Pow((um / ub), (-0.303)) * Math.Log10(st) - 5.047 * Math.Pow((um / ub), (-0.254)) * Math.Pow((Math.Log10(st)), 2);
                        lb = kj + 10 * Math.Log10(oct[i] / 63) + 50 * Math.Log10(ub) + 10 * Math.Log10(tab[j] * Math.Pow(Unit.MToFt(1), 2)) + 10 * Math.Log10(db);
                        rd = (100 * tab[j + 2] / 2.54) / (12 * db);
                        dr = (1 - rd / 0.15) * (6.793 - 1.86 * Math.Log10(st));
                        dt = -1.667 + 1.8 * (um / ub) - 0.133 * Math.Pow((um / ub), 2);

                        if (turbulence == Turbulence.No)
                        {
                            lwf[i] = MathOperations.DecibelAdd(lwf[i], lb + dr + 20 * Math.Log10(dm / db));
                        }
                        else
                        {
                            lwf[i] = MathOperations.DecibelAdd(lwf[i], lb + dr + dt + 20 * Math.Log10(dm / db));
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
            v = (Unit.CMHToCFM(q)) / (a * Math.Pow(Unit.MToFt(1), 2));

            for (int i = 0; i < kw.Length; i++)
            {
                lw[i] = kw[i] - 145 + 55 * Math.Log10(v) + 10 * Math.Log10(a * Math.Pow(Unit.MToFt(1), 2)) - 45 * Math.Log10(p / 100);
            }
            return lw;
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
            v = Unit.CMHToCFM(q) / (60 * a * Math.Pow(Unit.MToFt(1), 2));

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

            d = 334.9 * Unit.PaToInH20(dp) / (0.075 * Math.Pow(v, 2));
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

                lw[i] = 10 * Math.Log10(a * Math.Pow(Unit.MToFt(1), 2)) + 30 * Math.Log10(d) + 60 * Math.Log10(v) - 31.3 + c;
            }
            return lw;
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
    }

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
            double[] loc = JunctionMainRoundEq(which_branch, d_start, d_end, w_branch*h_branch);
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
            double[] loc = JunctionMainRectangularEq(which_branch, w_start*h_start, w_end*h_end, Math.Pow(d_branch, 2) * 0.25 * Math.PI,Math.Max(w_start,h_start));
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
            double[] loc = JunctionMainRectangularEq(which_branch, w_start*h_start, w_end*w_end, w_branch * h_branch,Math.Max(w_start,h_start));
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
            fco = 0.586 * 1125 / Unit.MToFt(d_poc);

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
            fco = 1125 / Unit.MToFt(a_in);

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
            fco = 0.586 * 1125 / Unit.MToFt(d_poc);

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
            fco = 1125 / Unit.MToFt(a_in);

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

            fco = 1125 / (2 * Unit.MToFt(b));
            w1 = w;
            h1 = h;
            l1 = l;

            for (int i = 0; i < oct.Length; i++)
            {
                if (oct[i] < Noise.Oct_BFI(fco))
                {
                    m = (Unit.MToFt(h1) * Unit.MToFt(w1)) / (sin * Math.Pow(Unit.MToFt(1), 2));

                    if (oct[i] == 63)
                    {
                        gl = (0.00306 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (1.959)) * Math.Pow((liningThickness / 2.5), (0.917))) * Unit.MToFt(l1);
                    }
                    else if (oct[i] == 125)
                    {
                        gl = (0.01323 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (1.41)) * Math.Pow((liningThickness / 2.5), (0.941))) * Unit.MToFt(l1);
                    }
                    else if (oct[i] == 250)
                    {
                        gl = (0.06244 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (0.824)) * Math.Pow((liningThickness / 2.5), (1.079))) * Unit.MToFt(l1);
                    }
                    else
                    {
                        gl = (0.2338 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (0.5)) * Math.Pow((liningThickness / 2.5), (1.087))) * Unit.MToFt(l1);
                    }

                    attn[i] = 10 * Math.Log10(Math.Pow((Math.Cosh(gl / 2) + 0.5 * (m + 1 / m) * Math.Sinh(gl / 2)), 2) * Math.Pow((Math.Cos(2 * Math.PI * oct[i] * Unit.MToFt(l1) / 1125)), 2) + Math.Pow((Math.Sinh(gl / 2) + 0.5 * (m + 1 / m) * Math.Cosh(gl / 2)), 2)
                        * Math.Pow((Math.Sin(2 * Math.PI * oct[i] * Unit.MToFt(l1) / 1125)), 2));
                }
                else
                {
                    s = 2 * (Unit.MToFt(l1) * Unit.MToFt(w1)) + 2 * (Unit.MToFt(l1) * Unit.MToFt(h1)) + 2 * (Unit.MToFt(h1) * Unit.MToFt(w1)) - (sin + sout) * Math.Pow(Unit.MToFt(1), 2);

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
            fco = 0.586 * 1125 / Unit.MToFt(d);
            w1 = w;
            h1 = h;
            l1 = l;

            for (int i = 0; i < oct.Length; i++)
            {
                if (oct[i] < Noise.Oct_BFI(fco))
                {
                    m = (Unit.MToFt(h1) * Unit.MToFt(w1)) / (sin * Math.Pow(Unit.MToFt(1), 2));

                    if (oct[i] == 63)
                    {
                        gl = (0.00306 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (1.959)) * Math.Pow((liningThickness / 2.5), (0.917))) * Unit.MToFt(l1);
                    }
                    else if (oct[i] == 125)
                    {
                        gl = (0.01323 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (1.41)) * Math.Pow((liningThickness / 2.5), (0.941))) * Unit.MToFt(l1);
                    }
                    else if (oct[i] == 250)
                    {
                        gl = (0.06244 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (0.824)) * Math.Pow((liningThickness / 2.5), (1.079))) * Unit.MToFt(l1);
                    }
                    else
                    {
                        gl = (0.2338 * Math.Pow(((2 * Unit.MToFt(h1) + 2 * Unit.MToFt(w1)) / (Unit.MToFt(h1) * Unit.MToFt(w1))), (0.5)) * Math.Pow((liningThickness / 2.5), (1.087))) * Unit.MToFt(l1);
                    }

                    attn[i] = 10 * Math.Log10(Math.Pow((Math.Cosh(gl / 2) + 0.5 * (m + 1 / m) * Math.Sinh(gl / 2)), 2) * Math.Pow((Math.Cos(2 * Math.PI * oct[i] * Unit.MToFt(l1) / 1125)), 2) + Math.Pow((Math.Sinh(gl / 2) +
                        0.5 * (m + 1 / m) * Math.Cosh(gl / 2)), 2) * Math.Pow((Math.Sin(2 * Math.PI * oct[i] * Unit.MToFt(l1) / 1125)), 2));
                }
                else
                {
                    s = 2 * (Unit.MToFt(l1) * Unit.MToFt(w1)) + 2 * (Unit.MToFt(l1) * Unit.MToFt(h1)) + 2 * (Unit.MToFt(h1) * Unit.MToFt(w1)) - (sin + sout) * Math.Pow(Unit.MToFt(1), 2);

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
            a1 = a * Math.Pow(Unit.MToFt(1), 2);
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
                        + (d2 * (d1)) + (e * Math.Pow((d1), 2)) + (f * Math.Pow((d1), 3))) * Unit.MToFt(l);

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

            p = 2 * (Unit.MToFt(h) + Unit.MToFt(w));
            a = Unit.MToFt(h) * Unit.MToFt(w);

            for (int i = 0; i < oct.Length; i++)
            {
                if (oct[i] <= 250)
                    if (p / a >= 3)
                    {
                        attn[i] = 17 * Math.Pow((p / a), (-0.25)) * Math.Pow(oct[i], (-0.85)) * Unit.MToFt(l);
                    }
                    else
                    {
                        attn[i] = 1.64 * Math.Pow((p / a), 0.73) * Math.Pow(oct[i], (-0.58)) * Unit.MToFt(l);
                    }
                else
                {
                    attn[i] = 0.02 * Math.Pow((p / a), 0.8) * Unit.MToFt(l);
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

                    p1 = 2 * (Unit.MToFt(h) + Unit.MToFt(w));
                    a1 = Unit.MToFt(h) * Unit.MToFt(w);
                    il[i] = b * Math.Pow((p1 / a1), (c)) * Math.Pow((liningThickness / 2.5), (d)) * Unit.MToFt(l);
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
            q1 = Unit.KgTolb(q) / (Unit.MToFt(1) * Unit.MToFt(1));
            a = (100 * h / 2.54) * (100 * w / 2.54);
            ao = 24 * Unit.MToFt(l) * ((100 * h / 2.54) + (100 * w / 2.54));
            fl = 24134 / Math.Pow(((100 * h / 2.54) * (100 * w / 2.54)), 0.5);
            tlmin = 10 * Math.Log10(24 * Unit.MToFt(l) * (1 / (100 * h / 2.54) + 1 / (100 * w / 2.54)));

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
                        attn[i] = tl1 -10 * Math.Log10(ao / a);
                    }
                    else
                    {
                        attn[i] = tlmin -10 * Math.Log10(ao / a);
                    }
                }
                else
                {
                    if (tlmin < tl2)
                    {
                        attn[i] = tl2 -10 * Math.Log10(ao / a);
                    }
                    else
                    {
                        attn[i] = tlmin -10 * Math.Log10(ao / a);
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
            q1 = Unit.KgTolb(q) / (Unit.MToFt(1) * Unit.MToFt(1));
            a = Math.PI * Math.Pow((100 * d / 2.54), 2) * 0.25;
            ao = 12 * Math.PI * (100 * d / 2.54) * Unit.MToFt(l);

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

            r1 = Unit.MToFt(r);
            s = 2 * (Unit.MToFt(w) * Unit.MToFt(l)) + 2 * (Unit.MToFt(l) * Unit.MToFt(h)) + 2 * (Unit.MToFt(w) * Unit.MToFt(h));
            mfp = 4 * (Unit.MToFt(w) * Unit.MToFt(l) * Unit.MToFt(h)) / s;

            for (int i = 0; i < oct.Length; i++)
            {
                m = M_coeff(t, rh)[i] / Unit.MToFt(1);
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
                tl[i] = -(10 * Math.Log10(q / (2 * Math.PI * Unit.MToFt(r) * Unit.MToFt(l)) + (4 / rm[i])) + 10.5);
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