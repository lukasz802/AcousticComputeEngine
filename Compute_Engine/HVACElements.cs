using System;
using System.Collections;
using System.Collections.Generic;
using HVACAcoustic;

namespace HVACElements
{
    public interface IBranch: IDimensions
    {
        double[] Attenuation();
        double[] Noise();
        double AirFlow { get; set; }
        double Rounding { get; set; }
        BranchType BranchType { get; set; }
    }

    public interface IDimensions
    {
        int Width { get; set; }
        int Height { get; set; }
        int Diameter { get; set; }
        double Velocity { get; }
    }

    public interface IDuctConnection
    {
        DuctType DuctType { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int Diameter { get; set; }
        double AirFlow { get; set; }
    }

    public enum DuctType
    {
        Round = 0,
        Rectangular = 1
    }

    public enum DiffuserType
    {
        Sudden = 0,
        Standard = 1
    }

    public enum ElbowType
    {
        Straight = 0,
        Rounded = 1
    }

    public enum PlenumType
    {
        HorizontalConnection = 0,
        VerticalConnection = 1
    }

    public enum DamperType
    {
        SingleBlade = 0,
        MultiBlade = 1
    }

    public enum SilencerType
    {
        ParallelBaffles = 0,
        Absorptive = 1
    }

    public enum NoiseEmission
    {
        OneDirection = 0,
        TwoDirection = 1,
    }

    public enum WorkArea
    {
        MaximumEfficiencyArea = 0,
        OutOffMaximumEfficiencyArea = 1
    }

    internal enum JunctionConnectionSide
    {
        Inlet = 0,
        Outlet = 1
    }

    [Serializable()]
    public abstract class ElementsBase
    {
        internal string _type;
        private double _airflow;
        private string _name;
        private string _comments;
        private bool _include;
        public abstract double[] Attenuation();
        public abstract double[] Noise();

        public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public double AirFlow
        {
            get
            {
                return _airflow;
            }
            set
            {
                if (value < 1)
                {
                    _airflow = 1;
                }
                else
                {
                    _airflow = value; ;
                }
            }
        }

        public bool Include
        {
            get
            {
                return _include;
            }
            set
            {
                _include = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }
    }

    [Serializable()]
    public class JunctionBranch : IBranch
    {
        private JunctionConnectionSide connectionSide;
        private DuctConnection _in = null ;
        private DuctConnection _out = null;
        private double airflow_branch;
        private int width_branch;
        private int height_branch;
        private int diameter_branch;
        private double rnd_branch;
        private DuctType duct_type_branch;
        private BranchType branch_type;      

        internal JunctionBranch(DuctType ductTypeMain, double airFlowMain, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchType, double airFlowBranch,
            int widthBranch, int heightBranch, int diameterBranch, double rounding)
        {
            duct_type_branch = ductTypeBranch;
            airflow_branch = airFlowBranch;
            width_branch = widthBranch;
            height_branch = heightBranch;
            diameter_branch = diameterBranch;
            rnd_branch = rounding;
            branch_type = branchType;
            _in = new DuctConnection(ductTypeMain, airFlowMain, widthMainIn, heightMainIn, diameterMainIn);
            _out = new DuctConnection(ductTypeMain, airFlowMain - airFlowBranch, widthMainOut, heightMainOut, diameterMainOut);
        }

        public double AirFlow
        {
            get
            {
                return airflow_branch;
            }
            set
            {
                if (value <= In.AirFlow)
                {
                    airflow_branch = value;

                    if (connectionSide == JunctionConnectionSide.Inlet)

                    {
                        Out.AirFlow = In.AirFlow - value;
                    }
                    else
                    {
                        In.AirFlow = Out.AirFlow + value;                       
                    }
                }
                else
                {
                    airflow_branch = In.AirFlow;
                    Out.AirFlow = 0;
                }
            }
        }

        public int Width
        {
            get
            {
                return width_branch;
            }
            set
            {
                if (value < 80)
                {
                    width_branch = 80;
                }
                else if (value < 2000)
                {
                    width_branch = value;
                }
                else
                {
                    width_branch = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return height_branch;
            }
            set
            {
                if (value < 80)
                {
                    height_branch = 80;
                }
                else if (value < 2000)
                {
                    height_branch = value;
                }
                else
                {
                    height_branch = 2000;
                }
            }
        }

        public int Diameter
        {
            get
            {
                return diameter_branch;
            }
            set
            {
                if (value < 80)
                {
                    diameter_branch = 80;
                }
                else if (value < 2000)
                {
                    diameter_branch = value;
                }
                else
                {
                    diameter_branch = 2000;
                }
            }
        }

        public double Rounding
        {
            get
            {
                return rnd_branch;
            }
            set
            {
                rnd_branch = value;
            }
        }

        public BranchType BranchType
        {
            get
            {
                return branch_type;
            }
            set
            {
                branch_type = value;
            }
        }

        public DuctType DuctType
        {
            get
            {
                return duct_type_branch;
            }
            set
            {
                duct_type_branch = value;
            }
        }

        public double Velocity
        {
            get
            {
                if (duct_type_branch == DuctType.Rectangular)
                {
                    return (airflow_branch / 3600.0) / ((width_branch / 1000.0) * (height_branch / 1000.0));
                }
                else
                {
                    return (airflow_branch / 3600.0) / (0.25 * Math.PI * Math.Pow(diameter_branch / 1000.0, 2));
                }
            }
        }     

        internal DuctConnection In
        {
            get
            {
                return _in;
            }
            set
            {
                _in =  value;
            }
        }

        internal DuctConnection Out
        {
            get
            {
                return _out;
            }
            set
            {
                _out = value;
            }
        }

        internal JunctionConnectionSide JunctionConnectionSide
        {
            get
            {
                return connectionSide;
            }
            set
            {
                connectionSide = value;
            }
        }

        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_in.DuctType == DuctType.Rectangular && duct_type_branch == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRectangular(Branch.BranchRight, branch_type, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, width_branch / 1000.0, height_branch / 1000.0);
            }
            else if (_in.DuctType == DuctType.Rectangular && duct_type_branch == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRound(Branch.BranchRight, branch_type, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, diameter_branch / 1000.0);

            }
            else if (_in.DuctType == DuctType.Round && duct_type_branch == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRectangular(Branch.BranchRight, branch_type, _in.Diameter / 1000.0,
                        _out.Diameter / 1000.0, width_branch / 1000.0, height_branch / 1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRound(Branch.BranchRight, branch_type, _in.Diameter / 1000.0,
                        _out.Diameter / 1000.0, diameter_branch / 1000.0);
            }
            return attn;
        }

        public double[] Noise()
        {
            double[] lw = new double[8];

            if (_in.DuctType == DuctType.Rectangular && duct_type_branch == DuctType.Rectangular)
            {
                if (branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, width_branch / 1000.0 * height_branch / 1000.0,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, width_branch / 1000.0 * height_branch / 1000.0,
               _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Rectangular && duct_type_branch == DuctType.Round)
            {
                if (branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, Math.Pow(diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, Math.Pow(diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Round && duct_type_branch == DuctType.Rectangular)
            {
                if (branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, width_branch / 1000.0 * height_branch / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, width_branch / 1000.0 * height_branch / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, Math.Pow(diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, airflow_branch, _in.AirFlow, Math.Pow(diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }

    [Serializable()]
    internal class DoubleJunctionContaier
    {
        private double airflow_branch_right;
        private int width_branch_right;
        private int height_branch_right;
        private int diameter_branch_right;
        private double rnd_branch_right;
        private DuctType duct_type_branch;
        private BranchType branch_type_right;
        private double airflow_branch_left;
        private int width_branch_left;
        private int height_branch_left;
        private int diameter_branch_left;
        private double rnd_branch_left;
        private BranchType branch_type_left;
        private DuctConnection _in = null;
        private DuctConnection _out = null;

        public DoubleJunctionContaier(DuctType ductTypeMain, double airFlowMain, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchTypeRight, double airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, double roundingRight, BranchType branchTypeLeft,
            double airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, double roundingLeft)
        {
            duct_type_branch = ductTypeBranch;
            airflow_branch_right = airFlowBranchRight;
            width_branch_right = widthBranchRight;
            height_branch_right = heightBranchRight;
            diameter_branch_right = diameterBranchRight;
            rnd_branch_right = roundingRight;
            branch_type_right = branchTypeRight;
            airflow_branch_left= airFlowBranchLeft;
            width_branch_left = widthBranchLeft;
            height_branch_left = heightBranchLeft;
            diameter_branch_left = diameterBranchLeft;
            rnd_branch_left = roundingLeft;
            branch_type_left = branchTypeLeft;
            _in = new DuctConnection(ductTypeMain, airFlowMain, widthMainIn, heightMainIn, diameterMainIn);
            _out = new DuctConnection(ductTypeMain, airFlowMain - airFlowBranchRight - airFlowBranchLeft, widthMainOut, heightMainOut, diameterMainOut);
        }

        public double AirFlowBranchRight
        {
            get
            {
                return airflow_branch_right;
            }
            set
            {
                airflow_branch_right = value;
            }
        }

        public int WidthBranchRight
        {
            get
            {
                return width_branch_right;
            }
            set
            {
                if (value < 80)
                {
                    width_branch_right = 80;
                }
                else if (value < 2000)
                {
                    width_branch_right = value;
                }
                else
                {
                    width_branch_right = 2000;
                }
            }
        }

        public int HeightBranchRight
        {
            get
            {
                return height_branch_right;
            }
            set
            {
                if (value < 80)
                {
                    height_branch_right = 80;
                }
                else if (value < 2000)
                {
                    height_branch_right = value;
                }
                else
                {
                    height_branch_right = 2000;
                }
            }
        }

        public int DiameterBranchRight
        {
            get
            {
                return diameter_branch_right;
            }
            set
            {
                if (value < 80)
                {
                    diameter_branch_right = 80;
                }
                else if (value < 2000)
                {
                    diameter_branch_right = value;
                }
                else
                {
                    diameter_branch_right = 2000;
                }
            }
        }

        public double RoundingBranchRight
        {
            get
            {
                return rnd_branch_right;
            }
            set
            {
                rnd_branch_right = value;
            }
        }

        public BranchType BranchTypeRight
        {
            get
            {
                return branch_type_right;
            }
            set
            {
                branch_type_right = value;
            }
        }

        public double AirFlowBranchLeft
        {
            get
            {
                return airflow_branch_left;
            }
            set
            {
                airflow_branch_left = value;
            }
        }

        public int WidthBranchLeft
        {
            get
            {
                return width_branch_left;
            }
            set
            {
                if (value < 80)
                {
                    width_branch_left = 80;
                }
                else if (value < 2000)
                {
                    width_branch_left = value;
                }
                else
                {
                    width_branch_left = 2000;
                }
            }
        }

        public int HeightBranchLeft
        {
            get
            {
                return height_branch_left;
            }
            set
            {
                if (value < 80)
                {
                    height_branch_left = 80;
                }
                else if (value < 2000)
                {
                    height_branch_left = value;
                }
                else
                {
                    height_branch_left = 2000;
                }
            }
        }

        public int DiameterBranchLeft
        {
            get
            {
                return diameter_branch_left;
            }
            set
            {
                if (value < 80)
                {
                    diameter_branch_left = 80;
                }
                else if (value < 2000)
                {
                    diameter_branch_left = value;
                }
                else
                {
                    diameter_branch_left = 2000;
                }
            }
        }

        public double RoundingBranchLeft
        {
            get
            {
                return rnd_branch_left;
            }
            set
            {
                rnd_branch_left = value;
            }
        }

        public DuctConnection In
        {
            get
            {
                return _in;
            }
            set
            {
                _in = value;
            }
        }

        public DuctConnection Out
        {
            get
            {
                return _out;
            }
            set
            {
                _out = value;
            }
        }

        public BranchType BranchTypeLeft
        {
            get
            {
                return branch_type_left;
            }
            set
            {
                branch_type_left = value;
            }
        }

        public DuctType DuctType
        {
            get
            {
                return duct_type_branch;
            }
            set
            {
                duct_type_branch = value;
            }
        }
    }

    [Serializable()]
    public class SilencerAttenuation
    {
        public double OctaveBand63Hz { get; set; }
        public double OctaveBand125Hz { get; set; }
        public double OctaveBand250Hz { get; set; }
        public double OctaveBand500Hz { get; set; }
        public double OctaveBand1000Hz { get; set; }
        public double OctaveBand2000Hz { get; set; }
        public double OctaveBand4000Hz { get; set; }
        public double OctaveBand8000Hz { get; set; }

        internal SilencerAttenuation(double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz)
        {
            this.OctaveBand63Hz = octaveBand63Hz;
            this.OctaveBand125Hz = octaveBand125Hz;
            this.OctaveBand250Hz = octaveBand250Hz;
            this.OctaveBand500Hz = octaveBand500Hz;
            this.OctaveBand1000Hz = octaveBand1000Hz;
            this.OctaveBand2000Hz = octaveBand2000Hz;
            this.OctaveBand4000Hz = octaveBand4000Hz;
            this.OctaveBand8000Hz = octaveBand8000Hz;
        }
    }

    [Serializable()]
    public class DoubleJunctionBranch : IBranch
    {
        private DoubleJunctionContaier container = null;
        private Branch branch;
        private JunctionConnectionSide connectionSide;

        internal DoubleJunctionBranch(DoubleJunctionContaier doubleJunctionContaier, Branch doubleJunctionBranch)
        {
            container = doubleJunctionContaier;
            branch = doubleJunctionBranch;
        }

        public double AirFlow
        {
            get
            {
                if (branch == Branch.BranchRight)
                {
                    return container.AirFlowBranchRight;
                }
                else
                {
                    return container.AirFlowBranchLeft;
                }
            }
            set
            {
                if (branch == Branch.BranchRight)
                {
                    if (value <= (container.In.AirFlow - container.AirFlowBranchLeft))
                    {
                        container.AirFlowBranchRight = value;

                        if (connectionSide == JunctionConnectionSide.Inlet)

                        {
                            container.Out.AirFlow = container.In.AirFlow - container.AirFlowBranchLeft - value;
                        }
                        else
                        {
                            container.In.AirFlow = container.Out.AirFlow + container.AirFlowBranchLeft + value;
                        }
                    }
                    else
                    {
                        container.AirFlowBranchRight = container.In.AirFlow - container.AirFlowBranchLeft;
                        container.Out.AirFlow = 0;
                    }
                }
                else
                {
                    if (value <= (container.In.AirFlow - container.AirFlowBranchRight))
                    {
                        container.AirFlowBranchLeft = value;

                        if (connectionSide == JunctionConnectionSide.Inlet)

                        {
                            container.Out.AirFlow = container.In.AirFlow - container.AirFlowBranchRight - value;
                        }
                        else
                        {
                            container.In.AirFlow = container.Out.AirFlow + container.AirFlowBranchRight + value;
                        }
                    }
                    else
                    {
                        container.AirFlowBranchRight = container.In.AirFlow - container.AirFlowBranchRight;
                        container.Out.AirFlow = 0;
                    }
                }
            }
        }

        internal JunctionConnectionSide JunctionConnectionSide
        {
            get
            {
                return connectionSide;
            }
            set
            {
                connectionSide = value;
            }
        }

        public double Rounding
        {
            get
            {
                if (branch == Branch.BranchRight)
                {
                    return container.RoundingBranchRight;
                }
                else
                {
                    return container.RoundingBranchLeft;
                }
            }
            set
            {
                if (branch == Branch.BranchRight)
                {
                    container.RoundingBranchRight = value;
                }
                else
                {
                    container.RoundingBranchLeft = value;
                }
            }
        }

        public BranchType BranchType
        {
            get
            {
                if (branch == Branch.BranchRight)
                {
                    return container.BranchTypeRight;
                }
                else
                {
                    return container.BranchTypeLeft;
                }
            }
            set
            {
                if (branch == Branch.BranchRight)
                {
                    container.BranchTypeRight = value;
                }
                else
                {
                    container.BranchTypeLeft = value;
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return container.DuctType;
            }
            set
            {
                container.DuctType = value;
            }
        }

        public int Width
        {
            get
            {
                if (branch == Branch.BranchRight)
                {
                    return container.WidthBranchRight;
                }
                else
                {
                    return container.WidthBranchLeft;
                }
            }
            set
            {
                if (branch == Branch.BranchRight)
                {
                    container.WidthBranchRight = value;
                }
                else
                {
                    container.WidthBranchLeft = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (branch == Branch.BranchRight)
                {
                    return container.HeightBranchRight;
                }
                else
                {
                    return container.HeightBranchLeft;
                }
            }
            set
            {
                if (branch == Branch.BranchRight)
                {
                    container.HeightBranchRight = value;
                }
                else
                {
                    container.HeightBranchLeft = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (branch == Branch.BranchRight)
                {
                    return container.DiameterBranchRight;
                }
                else
                {
                    return container.DiameterBranchLeft;
                }
            }
            set
            {
                if (branch == Branch.BranchRight)
                {
                    container.DiameterBranchRight = value;
                }
                else
                {
                    container.DiameterBranchLeft = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (branch == Branch.BranchRight)
                {
                    if (container.DuctType == DuctType.Rectangular)
                    {
                        return (container.AirFlowBranchRight / 3600.0) / ((container.WidthBranchRight / 1000.0) * (container.HeightBranchRight / 1000.0));
                    }
                    else
                    {
                        return (container.AirFlowBranchRight / 3600.0) / (0.25 * Math.PI * Math.Pow(container.DiameterBranchRight / 1000.0, 2));
                    }
                }
                else
                {
                    if (container.DuctType == DuctType.Rectangular)
                    {
                        return (container.AirFlowBranchLeft / 3600.0) / ((container.WidthBranchLeft / 1000.0) * (container.HeightBranchLeft / 1000.0));
                    }
                    else
                    {
                        return (container.AirFlowBranchLeft/ 3600.0) / (0.25 * Math.PI * Math.Pow(container.DiameterBranchLeft / 1000.0, 2));
                    }
                }
            }
        }

        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (branch == Branch.BranchRight)
            {
                if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.BranchRight, container.BranchTypeRight,
                        container.In.Width, container.In.Height, container.Out.Width, container.Out.Height, container.WidthBranchRight, container.HeightBranchRight,
                        container.WidthBranchLeft, container.HeightBranchLeft);
                }
                else if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.BranchRight, container.BranchTypeRight,
                        container.In.Width, container.In.Height, container.Out.Width, container.Out.Height,
                        container.DiameterBranchRight, container.DiameterBranchLeft);
                }
                else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.BranchRight, container.BranchTypeRight,
                        container.In.Diameter, container.Out.Diameter, container.WidthBranchRight, container.HeightBranchRight,
                        container.WidthBranchLeft, container.HeightBranchLeft);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.BranchRight, container.BranchTypeRight,
                        container.In.Diameter, container.Out.Diameter, container.DiameterBranchRight, container.DiameterBranchLeft);
                }
            }
            else
            {
                if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.BranchLeft, container.BranchTypeRight,
                        container.In.Width, container.In.Height, container.Out.Width, container.Out.Height, container.WidthBranchRight, container.HeightBranchRight,
                        container.WidthBranchLeft, container.HeightBranchLeft);
                }
                else if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.BranchLeft, container.BranchTypeRight,
                        container.In.Width, container.In.Height, container.Out.Width, container.Out.Height,
                        container.DiameterBranchRight, container.DiameterBranchLeft);
                }
                else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.BranchLeft, container.BranchTypeRight,
                        container.In.Diameter, container.Out.Diameter, container.WidthBranchRight, container.HeightBranchRight,
                        container.WidthBranchLeft, container.HeightBranchLeft);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.BranchLeft, container.BranchTypeRight,
                        container.In.Diameter, container.Out.Diameter, container.DiameterBranchRight, container.DiameterBranchLeft);
                }
            }
            return attn;
        }

        public double[] Noise()
        {
            double[] lw = new double[8];

            if (branch == Branch.BranchRight)
            {
                if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Rectangular)
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Rectangular)
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(container.In.Diameter/1000.0,2),
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                            0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Round)
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight/1000.0,2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                            0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight/1000.0,2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
            }
            else
            {
                if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Rectangular)
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Rectangular)
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                            container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                            container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Round)
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                            container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else
                {
                    if (container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                            container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), container.In.Width / 1000.0 * container.In.Height / 1000.0,
                            container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
            }
            return lw;
        }
    }

    [Serializable()]
    public class DuctConnection: IDimensions
    {
        private DuctType duct_type;
        private int width;
        private int height;
        private int diameter;
        private double airflow;

        internal DuctConnection(DuctType ductType, double airFlow, int w, int h, int d)
        {
            duct_type = ductType;
            airflow = airFlow;
            width = w;
            height = h;
            diameter = d;
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value < 80)
                {
                    width = 80;
                }
                else if (value < 2000)
                {
                    width = value;
                }
                else
                {
                    width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value < 80)
                {
                    height = 80;
                }
                else if (value < 2000)
                {
                    height = value;
                }
                else
                {
                    height = 2000;
                }
            }
        }

        public int Diameter
        {
            get
            {
                return diameter;
            }
            set
            {
                if (value < 80)
                {
                    diameter = 80;
                }
                else if (value < 2000)
                {
                    diameter = value;
                }
                else
                {
                    diameter = 2000;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (duct_type == DuctType.Rectangular)
                {
                    return (airflow / 3600.0) / ((width / 1000.0) * (height / 1000.0));
                }
                else
                {
                    return (airflow / 3600.0) / (0.25 * Math.PI * Math.Pow(diameter / 1000.0, 2));
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return duct_type;
            }
            set
            {
                duct_type = value;
            }
        }

        internal double AirFlow
        {
            get
            {
                return airflow;
            }
            set
            {
                airflow = value;
            }
        }
    }

    [Serializable()]
    public class JunctionMain: IDimensions
    {
        private Junction local_junction = null;
        private JunctionConnectionSide? junction_connection_side = null;

        internal JunctionMain(Junction baseJunction, JunctionConnectionSide junctionConnectionSide)
        {
            junction_connection_side = junctionConnectionSide;
            local_junction = baseJunction;
        }

        public int Width
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_junction.Branch.In.Width;
                }
                else
                {
                    return local_junction.Branch.Out.Width;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_junction.Branch.In.Width = value;
                }
                else
                {
                    local_junction.Branch.Out.Width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_junction.Branch.In.Height;
                }
                else
                {
                    return local_junction.Branch.Out.Height;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_junction.Branch.In.Height = value;
                }
                else
                {
                    local_junction.Branch.Out.Height = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_junction.Branch.In.Diameter;
                }
                else
                {
                    return local_junction.Branch.Out.Diameter;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_junction.Branch.In.Diameter = value;
                }
                else
                {
                    local_junction.Branch.Out.Diameter = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    if (local_junction.Branch.In.DuctType == DuctType.Rectangular)
                    {
                        return (local_junction.Branch.In.AirFlow / 3600.0) / ((local_junction.Branch.In.Width / 1000.0) * (local_junction.Branch.In.Height / 1000.0));
                    }
                    else
                    {
                        return (local_junction.Branch.In.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(local_junction.Branch.In.Diameter / 1000.0, 2));
                    }
                }
                else
                {
                    if (local_junction.Branch.Out.DuctType == DuctType.Rectangular)
                    {
                        return (local_junction.Branch.Out.AirFlow / 3600.0) / ((local_junction.Branch.Out.Width / 1000.0) * (local_junction.Branch.Out.Height / 1000.0));
                    }
                    else
                    {
                        return (local_junction.Branch.Out.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(local_junction.Branch.Out.Diameter / 1000.0, 2));
                    }
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_junction.Branch.In.DuctType;
                }
                else
                {
                    return local_junction.Branch.Out.DuctType;
                }
            }
            set
            {
                local_junction.Branch.In.DuctType = value;
                local_junction.Branch.Out.DuctType = value;
            }
        }

        public double AirFlow
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_junction.Branch.In.AirFlow;
                }
                else
                {
                    return local_junction.Branch.Out.AirFlow;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_junction.Branch.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    local_junction.AirFlow = value;
                }
                else
                {
                    local_junction.Branch.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    local_junction.AirFlow = value + local_junction.Branch.AirFlow;
                }
            }
        }
    }

    [Serializable()]
    public class DoubleJunctionMain : IDimensions
    {
        private DoubleJunction local_djunction = null;
        private JunctionConnectionSide? junction_connection_side = null;

        internal DoubleJunctionMain(DoubleJunction baseDoubleJunction, JunctionConnectionSide junctionConnectionSide)
        {
            junction_connection_side = junctionConnectionSide;
            local_djunction = baseDoubleJunction;
        }

        public int Width
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_djunction.Container.In.Width;
                }
                else
                {
                    return local_djunction.Container.Out.Width;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_djunction.Container.In.Width = value;
                }
                else
                {
                    local_djunction.Container.Out.Width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_djunction.Container.In.Height;
                }
                else
                {
                    return local_djunction.Container.Out.Height;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_djunction.Container.In.Height = value;
                }
                else
                {
                    local_djunction.Container.Out.Height = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_djunction.Container.In.Diameter;
                }
                else
                {
                    return local_djunction.Container.Out.Diameter;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_djunction.Container.In.Diameter = value;
                }
                else
                {
                    local_djunction.Container.Out.Diameter = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    if (local_djunction.Container.In.DuctType == DuctType.Rectangular)
                    {
                        return (local_djunction.Container.In.AirFlow / 3600.0) / ((local_djunction.Container.In.Width / 1000.0)
                            * (local_djunction.Container.In.Height / 1000.0));
                    }
                    else
                    {
                        return (local_djunction.Container.In.AirFlow / 3600.0) /
                            (0.25 * Math.PI * Math.Pow(local_djunction.Container.In.Diameter / 1000.0, 2));
                    }
                }
                else
                {
                    if (local_djunction.Container.Out.DuctType == DuctType.Rectangular)
                    {
                        return (local_djunction.Container.Out.AirFlow / 3600.0) /
                            ((local_djunction.Container.Out.Width / 1000.0) * (local_djunction.Container.Out.Height / 1000.0));
                    }
                    else
                    {
                        return (local_djunction.Container.Out.AirFlow / 3600.0) /
                            (0.25 * Math.PI * Math.Pow(local_djunction.Container.Out.Diameter / 1000.0, 2));
                    }
                }
            }
        }

        public double AirFlow
        {
            get
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return local_djunction.Container.In.AirFlow;
                }
                else
                {
                    return local_djunction.Container.Out.AirFlow;
                }
            }
            set
            {
                if (junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    local_djunction.BranchRight.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    local_djunction.BranchLeft.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    local_djunction.AirFlow = value;
                }
                else
                {
                    local_djunction.BranchRight.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    local_djunction.BranchLeft.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    local_djunction.AirFlow = value + local_djunction.BranchRight.AirFlow + local_djunction.BranchLeft.AirFlow;
                }
            }
        }
    }

    [Serializable()]
    public class GrillOrifice
    {
        public int Height { get; set; }
        public int Depth { get; set; }

        internal GrillOrifice(int height, int depth)
        {
            this.Height = height;
            this.Depth = depth;
        }
    }

    [Serializable()]
    public class Duct : ElementsBase, IDimensions
    {
        private int _width;
        private int _height;
        private int _diameter;
        private int _lenght;
        private int _liner_thickness;
        private bool _liner_check;
        private DuctType _duct_type;

        /// <summary>Kanał prosty.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductType">Typ króćca podłączeniowego.</param>
        /// <param name="airFlow">Przepływ powietrza przez kanał [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="lenght">Długość kanału [m].</param>
        /// <param name="linerThickness">Grubość izoloacji akustycznej kanału [mm].</param>
        /// <param name="linerCheck">Czy kanał jest zaizolowany akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Duct(string name, string comments, DuctType ductType, double airFlow, int width, int height, int diameter, int lenght, int linerThickness, bool linerCheck, bool include)
        {
            _type = "duct";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _lenght = lenght;
            _liner_thickness = linerThickness;
            _liner_check = linerCheck;
        }

        public Duct()
        {
            _type = "duct";
            this.Comments = "";
            this.Name = "duct_1";
            this.AirFlow = 500;
            this.Include = true;
            _duct_type = DuctType.Rectangular;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _lenght = 1;
            _liner_thickness = 25;
            _liner_check = false;
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                if (_liner_check == true)
                {
                    attn = HVACAcoustic.Attenuation.DuctRectanguar(_liner_thickness / 10.0, _width / 1000.0, _height / 1000.0, _lenght);
                }
                else
                {
                    attn=HVACAcoustic.Attenuation.DuctRectanguar(0, _width / 1000.0, _height / 1000.0, _lenght);
                }
            }
            else
            {
                if (_liner_check == true)
                {
                    attn = HVACAcoustic.Attenuation.DuctRound(_liner_thickness / 10, _diameter / 1000, _lenght);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.DuctRound(0, _diameter / 1000, _lenght);
                }
            }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                lw = HVACAcoustic.Noise.Duct(this.AirFlow, _width/1000.0 * _height/1000.0);
            }
            else
            {
                lw = HVACAcoustic.Noise.Duct(this.AirFlow, Math.Pow(_diameter/1000.0, 2) * 0.25 * Math.PI);
            }
            return lw;
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 80)
                {
                    _width = 80;
                }
                else if (value < 2000)
                {
                    _width = value;
                }
                else
                {
                    _width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 80)
                {
                    _height = 80;
                }
                else if (value < 2000)
                {
                    _height = value;
                }
                else
                {
                    _height = 2000;
                }
            }
        }

        public int Diameter
        {
            get
            {
                return _diameter;
            }
            set
            {
                if (value < 80)
                {
                    _diameter = 80;
                }
                else if (value < 2000)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 2000;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (_duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow/3600) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow/3600) / (0.25*Math.PI*Math.Pow(_diameter/1000.0,2));
                }
            }
        }

        public int Lenght
        {
            get
            {
                return _lenght;
            }
            set
            {
                _lenght = value;
            }
        }

        public int LinerDepth
        {
            get
            {
                return _liner_thickness;
            }
            set
            {
                if (this.DuctType == DuctType.Round)
                {
                    if (value < 25)
                    {
                        _width = 25;
                    }
                    else if (value < 75)
                    {
                        _width = value;
                    }
                    else
                    {
                        _width = 75;
                    }
                }
                else
                {
                    if (value < 25)
                    {
                        _width = 25;
                    }
                    else if (value < 50)
                    {
                        _width = value;
                    }
                    else
                    {
                        _width = 50;
                    }
                }
            }
        }

        public bool LinerCheck
        {
            get
            {
                return _liner_check;
            }
            set
            {
                _liner_check = value;
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _duct_type;
            }
            set
            {
                if (value == DuctType.Rectangular && this.LinerDepth > 50)
                {
                    this.LinerDepth = 50;
                }

                _duct_type = value;
            }
        }
    }

    [Serializable()]
    public class Diffuser : ElementsBase
    {
        private IDuctConnection Inlet { get; set; }
        private IDuctConnection Outlet { get; set; }
        private double _lenght;
        private DiffuserType _diffuser_type;

        /// <summary>Kanał prosty.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="diffuserType">Typ redukcji.</param>
        /// <param name="diffuserIn">Typ wlotowego króćca podłączeniowego.</param>
        /// <param name="diffuserOut">Typ wylotowego króćca podłączeniowego.</param>
        /// <param name="airFlow">Przepływ powietrza przez element [m3/h].</param>
        /// <param name="widthIn">Szerokość wlotwego króćca przyłączeniowego [mm].</param>
        /// <param name="heightIn">Wysokość wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="diameterIn">Średnica wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="widthOut">Szerokość wylotowego króćca przyłączeniowego [mm].</param>
        /// <param name="heightOut">Wysokość wylotowego króćca przyłączeniowego [mm].</param>
        /// <param name="diameterOut">Średnica wylotowego króćca przyłączeniowego [mm].</param>
        /// <param name="lenght">Długość kształtki [m].</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Diffuser(string name, string comments, DiffuserType diffuserType, DuctType diffuserIn, DuctType diffuserOut, double airFlow, int widthIn, int heightIn,
            int widthOut, int heightOut, int diameterIn, int diameterOut, int lenght, bool include)
        {
            _type = "diffuser";
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlow;
            this.Include = include;
            _diffuser_type = diffuserType;
            _lenght = lenght;
            Inlet.DuctType = diffuserIn;
            Inlet.AirFlow = airFlow;
            Inlet.Width = widthIn;
            Inlet.Height = heightIn;
            Inlet.Diameter = diameterIn;
            _diffuser_type = diffuserType;
            _lenght = lenght;
            Outlet.DuctType = diffuserOut;
            Outlet.AirFlow = airFlow;
            Outlet.Width = widthOut;
            Outlet.Height = heightOut;
            Outlet.Diameter = diameterOut;
        }

        public Diffuser()
        {
            _type = "diffuser";
            this.Comments = "";
            this.Name = "dfs_1";
            base.AirFlow = 500;
            this.Include = true;
            _diffuser_type = DiffuserType.Sudden;
            _lenght = 0;
            Inlet.DuctType = DuctType.Rectangular;
            Inlet.AirFlow = base.AirFlow;
            Inlet.Height = 200;
            Inlet.Width = 200;
            Inlet.Diameter = 250;
            Outlet.DuctType = DuctType.Rectangular;
            Outlet.AirFlow = base.AirFlow;
            Outlet.Height = 200;
            Outlet.Width = 200;
            Outlet.Diameter = 250;
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_diffuser_type == DiffuserType.Sudden)
            {
                if (Inlet.DuctType == DuctType.Rectangular && Inlet.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(Inlet.Width/1000.0 * Inlet.Height/1000.0, Outlet.Width/1000.0 * Outlet.Height/1000.0, 0);
                }
                else if (Inlet.DuctType == DuctType.Rectangular && Inlet.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(Inlet.Width / 1000.0 * Inlet.Height / 1000.0, 0.25*Math.PI*Math.Pow(Outlet.Diameter/1000.0,2), 0);
                }
                else if (Inlet.DuctType == DuctType.Round && Inlet.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(Inlet.Diameter/1000.0, 2), Outlet.Width/1000.0 * Outlet.Height/1000.0, 0);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(Inlet.Diameter/1000.0, 2), 0.25 * Math.PI * Math.Pow(Outlet.Diameter/1000, 2), 0);
                }
            }
            else
            {
                if (Inlet.DuctType == DuctType.Rectangular && Inlet.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(Inlet.Width/1000.0 * Inlet.Height/1000.0, Outlet.Width/1000.0 * Outlet.Height/1000.0, _lenght);
                }
                else if (Inlet.DuctType == DuctType.Rectangular && Inlet.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(Inlet.Width/1000.0 * Inlet.Height/1000.0, 0.25 * Math.PI * Math.Pow(Outlet.Diameter/1000.0, 2), _lenght);
                }
                else if (Inlet.DuctType == DuctType.Round && Inlet.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(Inlet.Diameter/1000.0, 2), Outlet.Width/1000.0 * Outlet.Height/1000.0, _lenght);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(Inlet.Diameter/1000.0, 2), 0.25 * Math.PI * Math.Pow(Outlet.Diameter/1000.0, 2), _lenght);
                }
            }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            return lw;
        }

        public double Lenght
        {
            get
            {
                return _lenght;
            }
            set
            {
                if (value < 0)
                {
                    _lenght = value;
                }
            }
        }

        public DiffuserType DiffuserType
        {
            get
            {
                return _diffuser_type;
            }
            set
            {
                _diffuser_type = value;
            }
        }

        public new double AirFlow
        {
            get
            {
                return base.AirFlow;
            }
            set
            {
                base.AirFlow = value;
                Inlet.AirFlow = value;
                Outlet.AirFlow = value;
            }
        }
    }

    [Serializable()]
    public class Bow : ElementsBase, IDimensions
    {
        private int width;
        private int height;
        private int diameter;
        private double rd;
        private double rw;
        private int liner_thickness;
        private bool liner_check;
        private DuctType duct_type;

        public Bow(string name, string comments, DuctType ductType, double airFlow, int w, int h, int d, double r_w, double r_d, int linerThckness, bool linerCheck, bool include)
        {
            _type = "bow";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            duct_type = ductType;
            width = w;
            height = h;
            diameter = d;
            rw = r_w;
            rd = r_d;
            liner_thickness = linerThckness;
            liner_check = linerCheck;
        }

        public Bow()
        {
            _type = "bow";
            this.Comments = "";
            this.Name = "bow_1";
            this.AirFlow = 500;
            this.Include = true;
            duct_type = DuctType.Rectangular;
            width = 200;
            height = 200;
            diameter = 250;
            rw = 1.5;
            rd = 1.5;
            liner_thickness = 25;
            liner_check = false;
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];

           if (duct_type == DuctType.Rectangular)
           {
                attn = HVACAcoustic.Attenuation.BowRectangular(width / 1000.0);
           }
           else
           {
               if (liner_check == true)
               {
                   attn = HVACAcoustic.Attenuation.BowRound(liner_thickness / 10.0, diameter / 1000.0);
               }
               else
               {
                   attn = HVACAcoustic.Attenuation.BowRound(0, diameter / 1000.0);
               }
           }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (duct_type == DuctType.Rectangular)
            {
                lw = HVACAcoustic.Noise.BowRectangular(this.AirFlow, width, height, (rw * width - width  / 2.0) / 1000.0);
            }
            else
            {
                lw = HVACAcoustic.Noise.BowRound(this.AirFlow, diameter, (rd * diameter - diameter / 2.0) / 1000.0);
            }

            return lw;
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value < 80)
                {
                    width = 80;
                }
                else if (value < 2000)
                {
                    width = value;
                }
                else
                {
                    width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value < 80)
                {
                    height = 80;
                }
                else if (value < 2000)
                {
                    height = value;
                }
                else
                {
                    height = 2000;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow/3600) / ((width / 1000.0) * (height / 1000.0));
                }
                else
                {
                    return (this.AirFlow/3600) / (0.25 * Math.PI * Math.Pow(diameter / 1000.0, 2));
                }
            }
        }

        public double RD
        {
            get
            {
                return rd;
            }
            set
            {
                rd = value;
            }
        }

        public double RW
        {
            get
            {
                return rw;
            }
            set
            {
                rw = value;
            }
        }

        public int Diameter
        {
            get
            {
                return diameter;
            }
            set
            {
                if (value < 80)
                {
                    diameter = 80;
                }
                else if (value < 2000)
                {
                    diameter = value;
                }
                else
                {
                    diameter = 2000;
                }
            }
        }

        public int LinerDepth
        {
            get
            {
                return liner_thickness;
            }
            set
            {
                liner_thickness = value;
            }
        }

        public bool LinerCheck
        {
            get
            {
                return liner_check;
            }
            set
            {
                liner_check = value;
            }
        }

        public DuctType DuctType
        {
            get
            {
                return duct_type;
            }
            set
            {
                duct_type = value;
            }
        }

    }

    [Serializable()]
    public class Elbow : ElementsBase
    {
        private int width;
        private int height;
        private double rnd;
        private byte vanes_number;
        private int liner_thickness;
        private bool liner_check;
        private TurnigVanes tuning_vanes;
        private ElbowType elbow_type;

        public Elbow(string name, string comments, double airFlow, ElbowType elbowType, TurnigVanes turnigVanes, byte vanesNumber,
                 int w, int h,  double rounding, int linerThickness, bool linerCheck, bool include)
        {
            _type = "elbow";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            elbow_type = elbowType;
            tuning_vanes = TurnigVanes;
            width = w;
            height = h;
            vanes_number = vanesNumber;
            rnd = rounding;
            liner_thickness = linerThickness;
            liner_check = linerCheck;
        }

        public Elbow()
        {
            _type = "elbow";
            this.Comments = "";
            this.Name = "elb_1";
            this.AirFlow = 800;
            this.Include = true;
            elbow_type = ElbowType.Straight;
            tuning_vanes = TurnigVanes.No;
            width = 400;
            height = 200;
            vanes_number = 3;
            rnd = 0;
            liner_thickness = 25;
            liner_check = false;
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];

           if (liner_check == true)
           {
                attn = HVACAcoustic.Attenuation.Elbow(tuning_vanes, liner_thickness / 10.0, width / 1000.0);
           }
           else
           {
                attn = HVACAcoustic.Attenuation.Elbow(tuning_vanes, 0, width / 1000.0);
           }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (elbow_type == ElbowType.Rounded)
            {
                lw = HVACAcoustic.Noise.Elbow(tuning_vanes, vanes_number, this.AirFlow, width / 1000.0, height / 1000.0, rnd / 1000);
            }
            else
            {
                lw = HVACAcoustic.Noise.Elbow(tuning_vanes, vanes_number, this.AirFlow, width / 1000.0, height / 1000.0, 0);
            }
            return lw;
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value < 80)
                {
                    width = 80;
                }
                else if (value < 2000)
                {
                    width = value;
                }
                else
                {
                    width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value < 80)
                {
                    height = 80;
                }
                else if (value < 2000)
                {
                    height = value;
                }
                else
                {
                    height = 2000;
                }
            }
        }

        public double Velocity
        {
            get
            {
                return (this.AirFlow/3600) / ((width / 1000.0) * (height / 1000.0));
            }
        }

        public byte VanesNumber
        {
            get
            {
                return vanes_number;
            }
            set
            {
                vanes_number = value;
            }
        }

        public double Rouning
        {
            get
            {
                return rnd;
            }
            set
            {
                rnd = value;
            }
        }

        public int LinerDepth
        {
            get
            {
                return liner_thickness;
            }
            set
            {
                liner_thickness = value;
            }
        }

        public bool LinerCheck
        {
            get
            {
                return liner_check;
            }
            set
            {
                liner_check = value;
            }
        }

        public TurnigVanes TurnigVanes
        {
            get
            {
                return tuning_vanes;
            }
            set
            {
                tuning_vanes = value;
            }
        }

        public ElbowType ElbowType
        {
            get
            {
                return elbow_type;
            }
            set
            {
                elbow_type = value;
            }
        }
    }

    [Serializable()]
    public class Junction : ElementsBase
    {
        private JunctionBranch local = null;
        private JunctionMain main_in = null;
        private JunctionMain main_out = null;

        public Junction(string name, string comments, DuctType ductTypeMainIn, DuctType ductTypeBranch, BranchType branchType, double airFlowMainIn, int widthMainIn, int heightMainIn, int widthMainOut,
            int heightMainOut, int diameterMainIn, int diameterMainOut, double airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, double roundingBranch, bool include)
        {
            _type = "junction";
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowMainIn;
            this.Include = include;

            if (airFlowBranch >= airFlowMainIn)
            {
                airFlowBranch = airFlowMainIn;
            }

            local = new JunctionBranch(ductTypeMainIn, base.AirFlow, widthMainIn, widthMainOut, heightMainIn, heightMainOut, diameterMainIn, diameterMainOut,
                ductTypeBranch, branchType, airFlowBranch, widthBranch, heightBranch, diameterBranch, roundingBranch);
            main_in = new JunctionMain(this, JunctionConnectionSide.Inlet);
            main_out = new JunctionMain(this, JunctionConnectionSide.Outlet);
        }

        public Junction()
        {
            _type = "junction";
            this.Comments = "";
            this.Name = "jnt_1";
            base.AirFlow = 2400;
            this.Include = true;
            local = new JunctionBranch(DuctType.Rectangular, base.AirFlow, 400, 200, 400, 200, 450, 250, DuctType.Rectangular,
                BranchType.Straight, 400, 160, 160, 200, 0);
            main_in = new JunctionMain(this, JunctionConnectionSide.Inlet);
            main_out = new JunctionMain(this, JunctionConnectionSide.Outlet);
        }

        public JunctionBranch Branch
        {
            get
            {
                return local;
            }
            set
            {
                local = value;
            }
        }

        public JunctionMain Inlet
        {
            get
            {
                return main_in;
            }
            set
            {
                main_in = value;
            }
        }

        public JunctionMain Outlet
        {
            get
            {
                return main_out;
            }
            set
            {
                main_out = value;
            }
        }

        public new double AirFlow
        {
            get
            {
                return base.AirFlow;
            }
            set
            {
                base.AirFlow = value;
                local.In.AirFlow = value;

                if (local.AirFlow >= value)
                {
                    local.AirFlow = value;
                }

                local.Out.AirFlow = value - local.AirFlow;
            }
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (main_in.DuctType == DuctType.Rectangular && local.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRectangular(HVACAcoustic.Branch.Main, local.BranchType, main_in.Width / 1000.0,main_in.Height / 1000.0,
                        main_out.Width / 1000.0, main_out.Height / 1000.0, local.Width / 1000.0, local.Height / 1000.0);
            }
            else if (main_in.DuctType == DuctType.Rectangular && local.DuctType == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRound(HVACAcoustic.Branch.Main, local.BranchType, main_in.Width / 1000.0, main_in.Height / 1000.0,
                        main_out.Width / 1000.0, main_out.Height / 1000.0, local.Diameter / 1000.0);

            }
            else if (main_in.DuctType == DuctType.Round && local.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRectangular(HVACAcoustic.Branch.Main, local.BranchType, main_in.Diameter / 1000.0,
                        main_out.Diameter / 1000.0, local.Width / 1000.0, local.Height / 1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRound(HVACAcoustic.Branch.Main, local.BranchType, main_in.Diameter / 1000.0,
                        main_out.Diameter / 1000.0, local.Diameter / 1000.0);
            }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (main_in.DuctType == DuctType.Rectangular && local.DuctType == DuctType.Rectangular)
            {
                if (local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, local.Width / 1000.0 * local.Height / 1000.0,
                                   main_in.Width / 1000.0 * main_in.Height / 1000.0, local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, local.Width / 1000.0 * local.Height / 1000.0,
               main_in.Width / 1000.0 * main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (main_in.DuctType == DuctType.Rectangular && local.DuctType == DuctType.Round)
            {
                if (local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, Math.Pow(local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   main_in.Width / 1000.0 * main_in.Height / 1000.0, local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, Math.Pow(local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   main_in.Width / 1000.0 * main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (main_in.DuctType == DuctType.Round && local.DuctType == DuctType.Rectangular)
            {
                if (local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, local.Width / 1000.0 * local.Height / 1000.0,
                   Math.Pow(main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, local.Width / 1000.0 * local.Height / 1000.0,
                   Math.Pow(main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, Math.Pow(local.Diameter/ 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, local.AirFlow, main_in.AirFlow, Math.Pow(local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }

    [Serializable()]
    public class Plenum : ElementsBase
    {
        private int width;
        private int height;
        private int lenght;
        private int dl;
        private int liner_thickness;
        private bool liner_check;
        private PlenumType plenum_type;
        private DuctConnection _in = null;
        private DuctConnection _out = null;

        public Plenum(string name, string comments, PlenumType plenumType, DuctType plenumIn, DuctType plenumOut, double airFlow, int widthIn, int heightIn, int widthOut, int heightOut, int diameterIn,
             int diameterOut, int plenumWidth, int plenumHeight, int plenumLenght, int inLocationLenght, int linerThickness, bool linerCheck, bool include)
        {
            _type = "plenum";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            plenum_type = plenumType;
            width = plenumWidth;
            height = plenumHeight;
            lenght = plenumLenght;
            dl = inLocationLenght;
            liner_check = linerCheck;
            liner_thickness = linerThickness;
            _in = new DuctConnection(plenumIn, base.AirFlow, widthIn, heightIn, diameterIn);
            _out = new DuctConnection(plenumOut, base.AirFlow, widthOut, heightOut, diameterOut);
        }

        public Plenum()
        {
            _type = "plenum";
            this.Comments = "";
            this.Name = "pln_1";
            this.AirFlow = 800;
            this.Include = true;
            plenum_type = PlenumType.VerticalConnection;
            width = 400;
            height = 250;
            lenght = 500;
            dl = 300;
            liner_check = false;
            liner_thickness = 25;
            _in = new DuctConnection(DuctType.Round, base.AirFlow, 200, 160, 160);
            _out = new DuctConnection(DuctType.Rectangular, base.AirFlow, 400, 250, 250);
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];
            double dq;

            if (plenum_type == PlenumType.VerticalConnection)
            {
                if ((4 - 4 * ((lenght - dl) / lenght)) >= 2.0 && (4 - 4 * ((lenght - dl) / lenght)) <= 4.0)
                {
                    dq = 4 - 4 * ((lenght - dl) / lenght);
                }
                else if ((4 - 4 * ((lenght - dl) / lenght)) > 4.0)
                {
                    dq = 4.0;
                }
                else
                {
                    dq = 2.0;
                }
            }
            else
            {
                dq = 2.0;
            }

            if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.PlenumInletRectangular(liner_thickness / 10.0, dq, _in.Width / 1000.0 * _in.Height / 1000.0, _out.Width / 1000.0 * _out.Height / 1000.0,
                    Math.Max(_in.Width, _in.Height), dl / 1000.0, lenght / 1000.0, width / 1000.0, height / 1000.0);
            }
            else if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.PlenumInletRectangular(liner_thickness / 10.0, dq, _in.Width / 1000.0 * _in.Height / 1000.0, Math.Pow(_out.Diameter / 1000.0, 2) * 0.25 * Math.PI,
                    Math.Max(_in.Width, _in.Height), dl / 1000.0, lenght / 1000.0, width / 1000.0, height / 1000.0);
            }
            else if (_in.DuctType == DuctType.Round && _out.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.PlenumInletRound(liner_thickness / 10.0, dq, _out.Width / 1000.0 * _out.Height / 1000.0, _in.Diameter / 1000.0, dl / 1000.0, lenght / 1000.0,
                    width / 1000.0, height / 1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.PlenumInletRound(liner_thickness / 10.0, dq, Math.Pow(_out.Width / 1000.0, 2) * 0.25 * Math.PI, _in.Diameter / 1000.0, dl / 1000.0, lenght / 1000.0,
                    width / 1000.0, height / 1000.0);
            }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            return lw;
        }

        public DuctConnection Inlet
        {
            get
            {
                return _in;
            }
            set
            {
                _in = value;
            }
        }

        public DuctConnection Outlet
        {
            get
            {
                return _out;
            }
            set
            {
                _out = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value < 80)
                {
                    width = 80;
                }
                else if (value < 2000)
                {
                    width = value;
                }
                else
                {
                    width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value < 80)
                {
                    height = 80;
                }
                else if (value < 2000)
                {
                    height = value;
                }
                else
                {
                    height = 2000;
                }
            }
        }

        public int Lenght
        {
            get
            {
                return lenght;
            }
            set
            {
                lenght = value;
            }
        }

        public int LinerDepth
        {
            get
            {
                return liner_thickness;
            }
            set
            {
                liner_thickness = value;
            }
        }

        public bool LinerCheck
        {
            get
            {
                return liner_check;
            }
            set
            {
                liner_check = value;
            }
        }

        public PlenumType PlenumType
        {
            get
            {
                return plenum_type;
            }
            set
            {
                plenum_type = value;
            }
        }

        public int InletDistance
        {
            get
            {
                return dl;
            }
            set
            {
                dl = value;
            }
        }
    }

    [Serializable()]
    public class Damper : ElementsBase, IDimensions
    {
        private int _diameter;
        private int _width;
        private int _height;
        private byte blade_number;
        private byte blade_angle;
        private DuctType duct_type;
        private DamperType damper_type;

        public Damper(string name, string comments, DamperType damperType, DuctType ductType, double airFlow, int width, int height,
             int diameter, bool include)
        {
            _type = "damper";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            damper_type = damperType;
            _width = width;
            _height = height;
            _diameter = diameter;
            duct_type = ductType;
        }

        public Damper()
        {
            _type = "damper";
            this.Comments = "";
            this.Name = "dmp_1";
            this.AirFlow = 800;
            this.Include = true;
            damper_type = DamperType.SingleBlade;
            _width = 200;
            _height = 200;
            _diameter = 250;
            duct_type = DuctType.Rectangular;
        }

        public override double[] Attenuation()
        {
            double[] attn = { 0, 0, 0, 0, 0, 0, 0, 0 };
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (duct_type==DuctType.Rectangular)
            {
                if (damper_type == DamperType.SingleBlade)
                {
                    lw = HVACAcoustic.Noise.DamperRectangular(1, blade_angle, base.AirFlow, _width / 1000.0, _height / 1000.0);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DamperRectangular(blade_number, blade_angle, base.AirFlow, _width / 1000.0, _height / 1000.0);
                }
            }
            else
            {
                lw = HVACAcoustic.Noise.DamperRound(blade_angle, base.AirFlow, _diameter / 1000.0);
            }
            return lw;
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 80)
                {
                    _width = 80;
                }
                else if (value < 2000)
                {
                    _width = value;
                }
                else
                {
                    _width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 80)
                {
                    _height = 80;
                }
                else if (value < 2000)
                {
                    _height = value;
                }
                else
                {
                    _height = 2000;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow / 3600) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow / 3600) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
                }
            }
        }

        public int Diameter
        {
            get
            {
                return _diameter;
            }
            set
            {
                if (value < 80)
                {
                    _diameter = 80;
                }
                else if (value < 2000)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 2000;
                }
            }
        }

        public byte BladeNumber
        {
            get
            {
                return blade_number;
            }
            set
            {
                blade_number = value;
            }
        }

        public byte BladeAngle
        {
            get
            {
                return blade_angle;
            }
            set
            {
                blade_angle = value;
            }
        }

        public DuctType DuctType
        {
            get
            {
                return duct_type;
            }
            set
            {
                duct_type = value;
            }
        }

        public DamperType DamperType
        {
            get
            {
                return damper_type;
            }
            set
            {
                damper_type = value;
            }
        }
    }

    [Serializable()]
    public class Grill: ElementsBase, IDimensions
    {
        private int _diameter;
        private int _width;
        private int _height;
        private int eff_area;
        private GrillType grill_type;
        private GrillLocation grill_location;
        private GrillOrifice local = null;

        public Grill(string name, string comments, GrillType grillType, GrillLocation grillLocation, double airFlow, int width, int height,
             int diameter, int orificeDepth, int orificeHeight, int percEffectiveArea, bool include)
        {
            _type = "grill";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            grill_type = grillType;
            grill_location = grillLocation;
            _width = width;
            _height = height;
            _diameter = diameter;
            eff_area = percEffectiveArea;
            local = new GrillOrifice( orificeHeight, orificeDepth);
        }

        public Grill()
        {
            _type = "grill";
            this.Comments = "";
            this.Name = "grill_1";
            this.AirFlow = 400;
            this.Include = true;
            grill_type = GrillType.RectangularSupplyWire;
            grill_location = GrillLocation.FlushWall;
            _width = 250;
            _height = 150;
            _diameter = 200;
            eff_area = 70;
            local = new GrillOrifice(20, 20);
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];
            DuctType duct_type;

            if ((Convert.ToInt16(grill_type)<=3) || ((Convert.ToInt16(grill_type) >= 8) && (Convert.ToInt16(grill_type) <= 11)))
            {
                duct_type = DuctType.Round;
            }
            else
            {
                duct_type = DuctType.Rectangular;
            }

            if (duct_type==DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.Grill(grill_location, _width/1000.0 * _height/1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.Grill(grill_location, Math.PI * 0.25 * Math.Pow(_diameter/1000.0, 2));
            }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];
            DuctType duct_type;

            if ((Convert.ToInt16(grill_type) <= 3) || ((Convert.ToInt16(grill_type) >= 8) && (Convert.ToInt16(grill_type) <= 11)))
            {
                duct_type = DuctType.Round;
            }
            else
            {
                duct_type = DuctType.Rectangular;
            }

            if (duct_type==DuctType.Rectangular)
            {
                lw = HVACAcoustic.Noise.Grill(grill_type, base.AirFlow, _width / 1000.0 * _height / 1000.0, local.Depth / 10.0,
                    _width / 1000.0, local.Height / 10.0, eff_area);
            }
            else
            {
                lw = HVACAcoustic.Noise.Grill(grill_type, base.AirFlow, Math.PI * 0.25 * Math.Pow(_diameter/1000.0, 2), local.Depth / 10.0,
                    _width / 1000.0, local.Height / 10.0, eff_area);
            }
            return lw;
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 80)
                {
                    _width = 80;
                }
                else if (value < 2000)
                {
                    _width = value;
                }
                else
                {
                    _width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 80)
                {
                    _height = 80;
                }
                else if (value < 2000)
                {
                    _height = value;
                }
                else
                {
                    _height = 2000;
                }
            }
        }

        public double Velocity
        {
            get
            {
                DuctType duct_type;

                if ((Convert.ToInt16(grill_type) <= 3) || ((Convert.ToInt16(grill_type) >= 8) && (Convert.ToInt16(grill_type) <= 11)))
                {
                    duct_type = DuctType.Round;
                }
                else
                {
                    duct_type = DuctType.Rectangular;
                }

                if (duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow / 3600) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow / 3600) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
                }
            }
        }

        public int Diameter
        {
            get
            {
                return _diameter;
            }
            set
            {
                if (value < 80)
                {
                    _diameter = 80;
                }
                else if (value < 2000)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 2000;
                }
            }
        }

        public int EffectiveArea
        {
            get
            {
                return eff_area;
            }
            set
            {
                if (value < 10)
                {
                    eff_area = 10;
                }
                else if (value < 100)
                {
                    eff_area = value;
                }
                else
                {
                    eff_area = 100;
                }
            }
        }

        public GrillType GrillType
        {
            get
            {
                return grill_type;
            }
            set
            {
                grill_type = value;
            }
        }

        public GrillLocation GrillLocation
        {
            get
            {
                return grill_location;
            }
            set
            {
                grill_location = value;
            }
        }

        public GrillOrifice Orifice
        {
            get
            {
                return local;
            }
            set
            {
                local = value;
            }
        }
    }

    [Serializable()]
    public class Fan: ElementsBase
    {
        public FanType FanType { get; set; }
        public NoiseEmission NoiseEmission { get; set; }
        public WorkArea WorkArea { get; set; }
        private double pressure_drop;
        private byte efficient;
        private byte blade_number;
        private int _rpm;       

        public Fan(string name, string comments, FanType fanType, double airFlow, double pressureDrop, int rpm, byte bladeNumber, byte efficientDeviation,
             WorkArea workArea, NoiseEmission noiseEmissionDirection, bool include)
        {
            _type = "fan";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            this.FanType = fanType;
            pressure_drop = pressureDrop;
            _rpm = rpm;
            blade_number = bladeNumber;
            efficient = efficientDeviation;
            this.NoiseEmission = noiseEmissionDirection;
            this.WorkArea = workArea;
        }

        public Fan()
        {
            _type = "fan";
            this.Comments = "";
            this.Name = "fan_1";
            this.AirFlow = 5000;
            this.Include = true;
            this.FanType = FanType.CentrifugalBackwardCurved;
            pressure_drop = 250;
            _rpm = 1500;
            blade_number = 12;
            efficient = 0;
            this.NoiseEmission = NoiseEmission.OneDirection;
            this.WorkArea = WorkArea.MaximumEfficiencyArea;
        }

        public override double[] Attenuation()
        {
            double[] attn = { 0, 0, 0, 0, 0, 0, 0, 0 };
            return attn;
        }

        public override double[] Noise()
        {
            byte loc;

            if (this.NoiseEmission == NoiseEmission.OneDirection)
            {
                loc = 0;
            }
            else
            {
                loc = 1;
            }

            return HVACAcoustic.Noise.Fan(this.FanType, base.AirFlow, pressure_drop, _rpm, blade_number, efficient, loc);
        }

        public double PressureDrop
        {
            get
            {
                return pressure_drop;
            }
            set
            {
                pressure_drop = value;
            }
        }

        public byte Efficient
        {
            get
            {
                return efficient;
            }
            set
            {
                if (value < 0)
                {
                    efficient = 0;
                }
                else if (value < 99)
                {
                    efficient = value;
                }
                else
                {
                    efficient = 99;
                }
            }
        }

        public byte BladeNumber
        {
            get
            {
                return blade_number;
            }
            set
            {
                blade_number = value;
            }
        }

        public int RPM
        {
            get
            {
                return _rpm;
            }
            set
            {
                _rpm = value;
            }
        }
    }

    [Serializable()]
    public class DoubleJunction : ElementsBase
    {
        private DoubleJunctionBranch local_right = null;
        private DoubleJunctionBranch local_left = null;
        private DoubleJunctionContaier container = null;
        private DoubleJunctionMain main_in = null;
        private DoubleJunctionMain main_out = null;

        public DoubleJunction(string name, string comments, DuctType ductTypeMain, double airFlowMainIn, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchTypeRight, double airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, double roundingRight, BranchType branchTypeLeft,
            double airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, double roundingLeft, bool include)
        {
            _type = "doublejunction";
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowMainIn;
            this.Include = include;

            if (airFlowMainIn < (airFlowBranchRight + airFlowBranchLeft))
            {
                if (airFlowBranchRight >= airFlowMainIn)
                {
                    airFlowBranchRight = airFlowMainIn;
                    airFlowBranchLeft = 0;
                }
                else
                {
                    airFlowBranchRight = airFlowMainIn - airFlowBranchRight;
                    airFlowBranchLeft = airFlowMainIn - airFlowBranchRight;
                }
            }

            container = new DoubleJunctionContaier(ductTypeMain, airFlowMainIn, widthMainIn, widthMainOut, heightMainIn, heightMainOut,
                diameterMainIn, diameterMainOut, ductTypeBranch, branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight,
                diameterBranchRight, roundingRight, branchTypeLeft, airFlowBranchLeft, widthBranchLeft, heightBranchLeft, diameterBranchLeft,
                roundingLeft);
            local_right = new DoubleJunctionBranch(container, Branch.BranchRight);
            local_left = new DoubleJunctionBranch(container, Branch.BranchLeft);
            main_in = new DoubleJunctionMain(this, JunctionConnectionSide.Inlet);
            main_out = new DoubleJunctionMain(this, JunctionConnectionSide.Outlet);
        }

        public DoubleJunction()
        {
            _type = "doublejunction";
            this.Comments = "";
            this.Name = "djnt_1";
            base.AirFlow = 2600;
            this.Include = true;

            container = new DoubleJunctionContaier(DuctType.Rectangular, base.AirFlow, 400, 200, 400, 200, 450, 250,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0, BranchType.Straight, 400, 160, 160, 200, 0);
            local_right = new DoubleJunctionBranch(container, Branch.BranchRight);
            local_left = new DoubleJunctionBranch(container, Branch.BranchLeft); main_in = new DoubleJunctionMain(this, JunctionConnectionSide.Inlet);
            main_in = new DoubleJunctionMain(this, JunctionConnectionSide.Inlet);
            main_out = new DoubleJunctionMain(this, JunctionConnectionSide.Outlet);
        }

        public DoubleJunctionBranch BranchRight
        {
            get
            {
                return local_right;
            }
            set
            {
                local_right = value;
            }
        }

        public DoubleJunctionBranch BranchLeft
        {
            get
            {
                return local_left;
            }
            set
            {
                local_left = value;
            }
        }

        public DoubleJunctionMain Inlet
        {
            get
            {
                return main_in;
            }
            set
            {
                main_in = value;
            }
        }

        public DoubleJunctionMain Outlet
        {
            get
            {
                return main_out;
            }
            set
            {
                main_out = value;
            }
        }

        internal DoubleJunctionContaier Container
        {
            get
            {
                return container;
            }
            set
            {
                container = value;
            }
        }

        public new double AirFlow
        {
            get
            {
                return base.AirFlow;
            }
            set
            {
                base.AirFlow = value;
                container.In.AirFlow = value;

                if ((container.AirFlowBranchRight + container.AirFlowBranchLeft) >= value)
                {
                    double temp = container.AirFlowBranchRight / (container.AirFlowBranchRight + container.AirFlowBranchLeft);
                    container.AirFlowBranchRight = temp * value;
                    container.AirFlowBranchLeft = (1 - temp) * value;
                }

                container.Out.AirFlow = value - container.AirFlowBranchRight - container.AirFlowBranchLeft;
            }
        }

        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.Main, container.BranchTypeRight,
                    container.In.Width, container.In.Height, container.Out.Width, container.Out.Height, container.WidthBranchRight, container.HeightBranchRight,
                    container.WidthBranchLeft, container.HeightBranchLeft);
            }
            else if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.Main, container.BranchTypeRight,
                    container.In.Width, container.In.Height, container.Out.Width, container.Out.Height,
                    container.DiameterBranchRight, container.DiameterBranchLeft);
            }
            else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.Main, container.BranchTypeRight,
                    container.In.Diameter, container.Out.Diameter, container.WidthBranchRight, container.HeightBranchRight,
                    container.WidthBranchLeft, container.HeightBranchLeft);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.Main, container.BranchTypeRight,
                    container.In.Diameter, container.Out.Diameter, container.DiameterBranchRight, container.DiameterBranchLeft);
            }
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (container.In.DuctType == DuctType.Rectangular && container.DuctType == DuctType.Rectangular)
            {
                if (container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                        container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, container.In.Width / 1000.0 * container.In.Height / 1000.0,
                        container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                        container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, container.In.Width / 1000.0 * container.In.Height / 1000.0,
                        container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Rectangular)
            {
                if (container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                        container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                        container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, container.WidthBranchRight / 1000.0 * container.HeightBranchRight / 1000.0,
                        container.WidthBranchLeft / 1000.0 * container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                        container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (container.In.DuctType == DuctType.Round && container.DuctType == DuctType.Round)
            {
                if (container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                        container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(container.In.Diameter / 1000.0, 2),
                        container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else
            {
                if (container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), container.In.Width / 1000.0 * container.In.Height / 1000.0,
                        container.RoundingBranchRight / 1000.0, container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, container.AirFlowBranchRight, container.AirFlowBranchLeft,
                        container.In.AirFlow, Math.PI * 0.25 * Math.Pow(container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(container.DiameterBranchLeft / 1000.0, 2), container.In.Width / 1000.0 * container.In.Height / 1000.0,
                        container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }

    [Serializable()]
    public class Silencer : ElementsBase, IDimensions
    {
        private SilencerAttenuation local = null;
        private int _width;
        private int _height;
        private int _diameter;
        private int eff_area;
        private DuctType duct_type;
        private SilencerType silencer_type;
        private double _lenght;

        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="silencerType">Typ tłumika.</param>
        /// <param name="ductType">Typ króćca podłączeniowego.</param>
        /// <param name="airFlow">Przepływ powietrza przez tłumik [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="lenght">Długość tłumika [m].</param>
        /// <param name="octaveBand63Hz">Tłumienie akustyczne w paśmie 63Hz [dB].</param>
        /// <param name="octaveBand125Hz">Tłumienie akustyczne w paśmie 125Hz [dB].</param>
        /// <param name="octaveBand250Hz">Tłumienie akustyczne w paśmie 250Hz [dB].</param>
        /// <param name="octaveBand500Hz">Tłumienie akustyczne w paśmie 500Hz [dB].</param>
        /// <param name="octaveBand1000Hz">Tłumienie akustyczne w paśmie 1000Hz [dB].</param>
        /// <param name="octaveBand2000Hz">Tłumienie akustyczne w paśmie 2000Hz [dB].</param>
        /// <param name="octaveBand4000Hz">Tłumienie akustyczne w paśmie 4000Hz [dB].</param>
        /// <param name="octaveBand8000Hz">Tłumienie akustyczne w paśmie 8000Hz [dB].</param>
        /// <param name="percEffectiveArea">Procentowy udział efektywnej powierzchnia netto króćca przyłączeniowego [%].</param>
        /// <param name="include">Uwzględnienie elementu podczas obliczeń.</param>
        /// <returns></returns>
        public Silencer(string name, string comments, SilencerType silencerType, DuctType ductType, double airFlow, int width, int height,
             int diameter, double lenght, double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
             double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz, int percEffectiveArea,
             bool include)
        {
            _type = "silencer";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            silencer_type = silencerType;
            duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _lenght = lenght;
            eff_area = percEffectiveArea;
            local = new SilencerAttenuation(octaveBand63Hz, octaveBand125Hz, octaveBand250Hz, octaveBand500Hz,
             octaveBand1000Hz, octaveBand2000Hz, octaveBand4000Hz, octaveBand8000Hz);
        }

        public Silencer()
        {
            _type = "silencer";
            this.Comments = "";
            this.Name = "sln_1";
            this.AirFlow = 500;
            this.Include = true;
            silencer_type = SilencerType.Absorptive;
            duct_type = DuctType.Round;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _lenght = 0.6;
            eff_area = 100;
            local = new SilencerAttenuation(1, 2, 5, 9, 16, 13, 5, 6);
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 80)
                {
                    _width = 80;
                }
                else if (value < 2000)
                {
                    _width = value;
                }
                else
                {
                    _width = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 80)
                {
                    _height = 80;
                }
                else if (value < 2000)
                {
                    _height = value;
                }
                else
                {
                    _height = 2000;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow / 3600) / ((_width / 1000.0) * (_height / 1000.0) * eff_area/100.0);
                }
                else
                {
                    return (this.AirFlow / 3600) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2) * eff_area / 100.0);
                }
            }
        }

        public int Diameter
        {
            get
            {
                return _diameter;
            }
            set
            {
                if (value < 80)
                {
                    _diameter = 80;
                }
                else if (value < 2000)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 2000;
                }
            }
        }

        public double Lenght
        {
            get
            {
                return _lenght;
            }
            set
            {
                if (value < 0.1)
                {
                    _lenght = 0.1;
                }
                else if (value < 3.0)
                {
                    _lenght = value;
                }
                else
                {
                    _lenght = 3.0;
                }
            }
        }

        public int EffectiveArea
        {
            get
            {
                return eff_area;
            }
            set
            {
                if (value < 10)
                {
                    eff_area = 10;
                }
                else if (value < 100)
                {
                    eff_area = value;
                }
                else
                {
                    eff_area = 100;
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return duct_type;
            }
            set
            {
                duct_type = value;
            }
        }

        public SilencerType SilencerType
        {
            get
            {
                return silencer_type;
            }
            set
            {
                silencer_type = value;
            }
        }

        public SilencerAttenuation OctaveBandAttenuation
        {
            get
            {
                return local;
            }
            set
            {
                local = value;
            }
        }

        public override double[] Attenuation()
        {
            double[] attn = { local.OctaveBand63Hz, local.OctaveBand125Hz, local.OctaveBand250Hz, local.OctaveBand500Hz, local.OctaveBand1000Hz,
                local.OctaveBand2000Hz, local.OctaveBand4000Hz, local.OctaveBand8000Hz };
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (duct_type == DuctType.Rectangular)
            {
                lw = HVACAcoustic.Noise.Silencer(base.AirFlow, _height / 1000.0 * _width / 1000.0, eff_area);
            }
            else
            {
                if (silencer_type == SilencerType.ParallelBaffles)
                {
                    lw = HVACAcoustic.Noise.Silencer(base.AirFlow, 0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2), eff_area);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Silencer(base.AirFlow, 0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2), 0);
                }
            }
            return lw;
        }
    }

    [Serializable()]
    public class ElementTreeCollection
    {

    }
}
