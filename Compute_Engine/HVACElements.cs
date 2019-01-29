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
        int AirFlow { get; set; }
        int Rounding { get; set; }
        BranchType BranchType { get; set; }
    }

    public interface IDimensions
    {
        int Width { get; set; }
        int Height { get; set; }
        int Diameter { get; set; }
        double Velocity { get; }
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
        private int _airflow;
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

        public int AirFlow
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
                    _airflow = value;
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
        private int _airflow_branch;
        private int _width_branch;
        private int _height_branch;
        private int _diameter_branch;
        private int _rnd_branch;
        private DuctType _duct_type_branch;
        private BranchType _branch_type;      

        internal JunctionBranch(DuctType ductTypeMain, int airFlowMain, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchType, int airFlowBranch,
            int widthBranch, int heightBranch, int diameterBranch, int rounding)
        {
            _duct_type_branch = ductTypeBranch;
            _airflow_branch = airFlowBranch;
            _width_branch = widthBranch;
            _height_branch = heightBranch;
            _diameter_branch = diameterBranch;
            _rnd_branch = rounding;
            _branch_type = branchType;
            _in = new DuctConnection(ductTypeMain, airFlowMain, widthMainIn, heightMainIn, diameterMainIn);
            _out = new DuctConnection(ductTypeMain, airFlowMain - airFlowBranch, widthMainOut, heightMainOut, diameterMainOut);
        }

        public int AirFlow
        {
            get
            {
                return _airflow_branch;
            }
            set
            {
                if (value <= In.AirFlow)
                {
                    _airflow_branch = value;

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
                    _airflow_branch = In.AirFlow;
                    Out.AirFlow = 0;
                }
            }
        }

        public int Width
        {
            get
            {
                return _width_branch;
            }
            set
            {
                if (value < 100)
                {
                    _width_branch = 100;
                }
                else if (value < 2000)
                {
                    _width_branch = value;
                }
                else
                {
                    _width_branch = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return _height_branch;
            }
            set
            {
                if (value < 100)
                {
                    _height_branch = 100;
                }
                else if (value < 2000)
                {
                    _height_branch = value;
                }
                else
                {
                    _height_branch = 2000;
                }
            }
        }

        public int Diameter
        {
            get
            {
                return _diameter_branch;
            }
            set
            {
                if (value < 80)
                {
                    _diameter_branch = 80;
                }
                else if (value < 1600)
                {
                    _diameter_branch = value;
                }
                else
                {
                    _diameter_branch = 1600;
                }
            }
        }

        public int Rounding
        {
            get
            {
                return _rnd_branch;
            }
            set
            {
                if (value < 0)
                {
                    _rnd_branch = 0;
                }
                else if (value < Math.Ceiling(0.6 * _width_branch))
                {
                    _rnd_branch = value;
                }
                else
                {
                    _rnd_branch = (int)Math.Ceiling(0.6 * _width_branch);
                }
            }
        }

        public BranchType BranchType
        {
            get
            {
                return _branch_type;
            }
            set
            {
                _branch_type = value;
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _duct_type_branch;
            }
            set
            {
                _duct_type_branch = value;
            }
        }

        public double Velocity
        {
            get
            {
                if (_duct_type_branch == DuctType.Rectangular)
                {
                    return (_airflow_branch / 3600.0) / ((_width_branch / 1000.0) * (_height_branch / 1000.0));
                }
                else
                {
                    return (_airflow_branch / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter_branch / 1000.0, 2));
                }
            }
        }     

        internal DuctConnection In
        {
            get
            {
                return _in;
            }
        }

        internal DuctConnection Out
        {
            get
            {
                return _out;
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

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_in.DuctType == DuctType.Rectangular && _duct_type_branch == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRectangular(Branch.BranchRight, _branch_type, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, _width_branch / 1000.0, _height_branch / 1000.0);
            }
            else if (_in.DuctType == DuctType.Rectangular && _duct_type_branch == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRound(Branch.BranchRight, _branch_type, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, _diameter_branch / 1000.0);

            }
            else if (_in.DuctType == DuctType.Round && _duct_type_branch == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRectangular(Branch.BranchRight, _branch_type, _in.Diameter / 1000.0,
                        _out.Diameter / 1000.0, _width_branch / 1000.0, _height_branch / 1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRound(Branch.BranchRight, _branch_type, _in.Diameter / 1000.0,
                        _out.Diameter / 1000.0, _diameter_branch / 1000.0);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public double[] Noise()
        {
            double[] lw = new double[8];

            if (_in.DuctType == DuctType.Rectangular && _duct_type_branch == DuctType.Rectangular)
            {
                if (_branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
               _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Rectangular && _duct_type_branch == DuctType.Round)
            {
                if (_branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Round && _duct_type_branch == DuctType.Rectangular)
            {
                if (_branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (_branch_type == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }

    [Serializable()]
    internal class DoubleJunctionContaier
    {
        private int airflow_branch_right;
        private int width_branch_right;
        private int height_branch_right;
        private int diameter_branch_right;
        private int rnd_branch_right;
        private DuctType duct_type_branch;
        private BranchType branch_type_right;
        private int airflow_branch_left;
        private int width_branch_left;
        private int height_branch_left;
        private int diameter_branch_left;
        private int rnd_branch_left;
        private BranchType branch_type_left;
        private DuctConnection _in = null;
        private DuctConnection _out = null;

        public DoubleJunctionContaier(DuctType ductTypeMain, int airFlowMain, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft)
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

        public int AirFlowBranchRight
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
                else if (value < 1600)
                {
                    diameter_branch_right = value;
                }
                else
                {
                    diameter_branch_right = 1600;
                }
            }
        }

        public int RoundingBranchRight
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

        public int AirFlowBranchLeft
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
                else if (value < 1600)
                {
                    diameter_branch_left = value;
                }
                else
                {
                    diameter_branch_left = 1600;
                }
            }
        }

        public int RoundingBranchLeft
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
        }

        public DuctConnection Out
        {
            get
            {
                return _out;
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

        public int AirFlow
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

        public int Rounding
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
        private int airflow;

        internal DuctConnection(DuctType ductType, int airFlow, int w, int h, int d)
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
                if (value < 100)
                {
                    width = 100;
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
                if (value < 100)
                {
                    height = 100;
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
                else if (value < 1600)
                {
                    diameter = value;
                }
                else
                {
                    diameter = 1600;
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

        internal int AirFlow
        {
            get
            {
                return airflow;
            }
            set
            {
                if (value < 1)
                {
                    airflow = 1;
                }
                else
                {
                    airflow = value;
                }
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

        public int AirFlow
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

        public int AirFlow
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
        private double _lenght;
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
        public Duct(string name, string comments, DuctType ductType, int airFlow, int width, int height, int diameter, double lenght, int linerThickness, bool linerCheck, bool include)
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
        
        /// <summary>Kanał prosty.</summary>
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

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
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

        /// <summary>Oblicz szum generowany przez element.</summary>
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
                if (value < 100)
                {
                    _width = 100;
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
                if (value < 100)
                {
                    _height = 100;
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
                else if (value < 1600)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 1600;
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
                else
                {
                    _lenght = Math.Round(value,2);
                }
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
        private DuctConnection _in = null;
        private DuctConnection _out = null;
        private double _lenght;
        private DiffuserType _diffuser_type;

        /// <summary>Dyfuzor/konfuzor lub nagłe zwężenie/rozszerzenie.</summary>
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
        public Diffuser(string name, string comments, DiffuserType diffuserType, DuctType diffuserIn, DuctType diffuserOut, int airFlow, int widthIn, int heightIn,
            int widthOut, int heightOut, int diameterIn, int diameterOut, double lenght, bool include)
        {
            _type = "diffuser";
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlow;
            this.Include = include;
            _diffuser_type = diffuserType;
            _lenght = lenght;
            _in = new DuctConnection(diffuserIn, base.AirFlow, widthIn, heightIn, diameterIn);
            _out = new DuctConnection(diffuserOut, base.AirFlow, widthOut, heightOut, diameterOut);
        }
        /// <summary>Dyfuzor/konfuzor lub nagłe zwężenie/rozszerzenie.</summary>
        public Diffuser()
        {
            _type = "diffuser";
            this.Comments = "";
            this.Name = "dfs_1";
            base.AirFlow = 500;
            this.Include = true;
            _diffuser_type = DiffuserType.Sudden;
            _lenght = 0;
            _in = new DuctConnection(DuctType.Rectangular, 500, 200, 200, 250);
            _out = new DuctConnection(DuctType.Rectangular, 500, 200, 200, 250);
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_diffuser_type == DiffuserType.Sudden)
            {
                if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(_in.Width/1000.0 * _in.Height/1000.0, _out.Width/1000.0 * _out.Height/1000.0, 0);
                }
                else if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(_in.Width / 1000.0 * _in.Height / 1000.0, 0.25*Math.PI*Math.Pow(_out.Diameter/1000.0,2), 0);
                }
                else if (_in.DuctType == DuctType.Round && _out.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter/1000.0, 2), _out.Width / 1000.0 * _out.Height / 1000.0, 0);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter / 1000.0, 2), 0.25 * Math.PI * Math.Pow(_out.Diameter / 1000.0, 2), 0);
                }
            }
            else
            {
                if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(_in.Width / 1000.0 * _in.Height / 1000.0, _out.Width / 1000.0 * _out.Height / 1000.0, _lenght);
                }
                else if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(_in.Width / 1000.0 * _in.Height / 1000.0, 0.25 * Math.PI * Math.Pow(_out.Diameter / 1000.0, 2), _lenght);
                }
                else if (_in.DuctType == DuctType.Round && _out.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter / 1000.0, 2), _out.Width / 1000.0 * _out.Height / 1000.0, _lenght);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter / 1000.0, 2), 0.25 * Math.PI * Math.Pow(_out.Diameter / 1000.0, 2), _lenght);
                }
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
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
                if (value < 0.1)
                {
                    _lenght = 0.1;
                }
                else
                {
                    _lenght = Math.Round(value,2);
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

        public new int AirFlow
        {
            get
            {
                return base.AirFlow;
            }
            set
            {
                base.AirFlow = value;
                _in.AirFlow = value;
                _out.AirFlow = value;
            }
        }

        public DuctConnection Inlet
        {
            get { return _in; }
        }

        public DuctConnection Outlet
        {
            get { return _out; }
        }
    }

    [Serializable()]
    public class Bow : ElementsBase, IDimensions
    {
        private int _width;
        private int _height;
        private int _diameter;
        private double _rd;
        private double _rw;
        private int _liner_thickness;
        private bool _liner_check;
        private DuctType _duct_type;

        /// <summary>Łuk.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductType">Typ łuku.</param>
        /// <param name="airFlow">Przepływ powietrza przez element [m3/h].</param>
        /// <param name="width">Szerokość wlotwego króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="rw">Względny promień gięcia łuku (w odniesieniu do łuku o przekroju prostokątnym).</param>
        /// <param name="rd">Względny promień gięcia łuku (w odniesieniu do łuku o przekroju okrągłym).</param>
        /// <param name="linerThickness">Grubość izoloacji akustycznej łuku [mm].</param>
        /// <param name="linerCheck">Czy łuk jest zaizolowany akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Bow(string name, string comments, DuctType ductType, int airFlow, int width, int height, int diameter, double rw, double rd, int linerThckness, bool linerCheck, bool include)
        {
            _type = "bow";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _rw = rw;
            _rd = rd;
            _liner_thickness = linerThckness;
            _liner_check = linerCheck;
        }

        /// <summary>Łuk.</summary>
        public Bow()
        {
            _type = "bow";
            this.Comments = "";
            this.Name = "bow_1";
            this.AirFlow = 500;
            this.Include = true;
            _duct_type = DuctType.Rectangular;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _rw = 1.5;
            _rd = 1.5;
            _liner_thickness = 25;
            _liner_check = false;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

           if (_duct_type == DuctType.Rectangular)
           {
                attn = HVACAcoustic.Attenuation.BowRectangular(_width / 1000.0);
           }
           else
           {
               if (_liner_check == true)
               {
                   attn = HVACAcoustic.Attenuation.BowRound(_liner_thickness / 10.0, _diameter / 1000.0);
               }
               else
               {
                   attn = HVACAcoustic.Attenuation.BowRound(0, _diameter / 1000.0);
               }
           }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                lw = HVACAcoustic.Noise.BowRectangular(this.AirFlow, _width, _height, (_rw * _width - _width  / 2.0) / 1000.0);
            }
            else
            {
                lw = HVACAcoustic.Noise.BowRound(this.AirFlow, _diameter, (_rd * _diameter - _diameter / 2.0) / 1000.0);
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
                if (value < 100)
                {
                    _width = 100;
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
                if (value < 100)
                {
                    _height = 100;
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
                if (_duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow/3600) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow/3600) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
                }
            }
        }

        public double RD
        {
            get
            {
                return _rd;
            }
            set
            {
                if (_rd < 0.5)
                {
                    _rd = 0.5;
                }
                else if (_rd < 5.0)
                {
                    _rd = Math.Round(value,2);
                }
                else
                {
                    _rd = 5.0;
                }
            }
        }

        public double RW
        {
            get
            {
                return _rw;
            }
            set
            {
                if (_rw < 0.5)
                {
                    _rw = 0.5;
                }
                else if (_rw < 5.0)
                {
                    _rw = Math.Round(value, 2);
                }
                else
                {
                    _rw = 5.0;
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
                else if (value < 1600)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 1600;
                }
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
                if (value < 25)
                {
                    _liner_thickness = 25;
                }
                else if (value < 75)
                {
                    _liner_thickness = value;
                }
                else
                {
                    _liner_thickness = 75;
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
                _duct_type = value;
            }
        }

    }

    [Serializable()]
    public class Elbow : ElementsBase
    {
        private int _width;
        private int _height;
        private int _rnd;
        private byte _vanes_number;
        private bool _liner_check;
        private TurnigVanes _tuning_vanes;
        private ElbowType _elbow_type;

        /// <summary>Kolano.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="elbowType">Typ kolana.</param>
        /// <param name="airFlow">Przepływ powietrza przez element [m3/h].</param>
        /// <param name="width">Szerokość wlotwego króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="vanesNumber">Liczba kierownic powietrza.</param>
        /// <param name="turnigVanes">Czy kolano posiada kierownice powietrza.</param>
        /// <param name="rounding">Promień zaokrąglenia kolana [mm].</param>
        /// <param name="linerCheck">Czy kolano jest zaizolowany akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Elbow(string name, string comments, int airFlow, ElbowType elbowType, TurnigVanes turnigVanes, byte vanesNumber,
                 int width, int height, int rounding, bool linerCheck, bool include)
        {
            _type = "elbow";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            _elbow_type = elbowType;
            _tuning_vanes = TurnigVanes;
            _width = width;
            _height = height;
            _vanes_number = vanesNumber;
            _rnd = rounding;
            _liner_check = linerCheck;
        }

        /// <summary>Kolano.</summary>
        public Elbow()
        {
            _type = "elbow";
            this.Comments = "";
            this.Name = "elb_1";
            this.AirFlow = 800;
            this.Include = true;
            _elbow_type = ElbowType.Straight;
            _tuning_vanes = TurnigVanes.No;
            _width = 400;
            _height = 200;
            _vanes_number = 3;
            _rnd = 0;
            _liner_check = false;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

           if (_liner_check == true)
           {
                attn = HVACAcoustic.Attenuation.Elbow(_tuning_vanes, 10.0, _width / 1000.0);
           }
           else
           {
                attn = HVACAcoustic.Attenuation.Elbow(_tuning_vanes, 0, _width / 1000.0);
           }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_elbow_type == ElbowType.Rounded)
            {
                lw = HVACAcoustic.Noise.Elbow(_tuning_vanes, _vanes_number, this.AirFlow, _width / 1000.0, _height / 1000.0, _rnd / 1000.0);
            }
            else
            {
                lw = HVACAcoustic.Noise.Elbow(_tuning_vanes, _vanes_number, this.AirFlow, _width / 1000.0, _height / 1000.0, 0);
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
                if (value < 100)
                {
                    _width = 100;
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
                if (value < 100)
                {
                    _height = 100;
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
                return (this.AirFlow/3600) / ((_width / 1000.0) * (_height / 1000.0));
            }
        }

        public byte VanesNumber
        {
            get
            {
                return _vanes_number;
            }
            set
            {
                if (!(_rnd == 0))
                {
                    if (value < 1)
                    {
                        _rnd = 1;
                    }
                    else if (Math.Round(2.13 * Math.Pow(((double)_rnd / (double)_width), -1) - 1) >= value)
                    {
                        _rnd = value;
                    }
                    else
                    {
                        _rnd = (byte)Math.Round(2.13 * Math.Pow(((double)_rnd / (double)_width), -1) - 1);
                    }
                }
                else
                {
                    double dh = 2 * (double)_height * (double)_width / ((double)_height + (double)_width);

                    if (value < 1)
                    {
                        _rnd = 1;
                    }
                    else if (Math.Round(2.13 * Math.Pow((0.35 * dh / ((double)_width * Math.Pow(2, 0.5))), (-1)) - 1) >= value)
                    {
                        _rnd = value;
                    }
                    else
                    {
                        _rnd = (byte)Math.Round(2.13 * Math.Pow((0.35 * dh / ((double)_width * Math.Pow(2, 0.5))), (-1)) - 1);
                    }
                }
            }
        }

        public int Rouning
        {
            get
            {
                return _rnd;
            }
            set
            {
                if (value < 0)
                {
                    _rnd = 0;
                }
                else if (value < Math.Ceiling(0.6 * _width))
                {
                    _rnd = value;
                }
                else
                {
                    _rnd = (int)Math.Ceiling(0.6 * _width);
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

        public TurnigVanes TurnigVanes
        {
            get
            {
                return _tuning_vanes;
            }
            set
            {
                _tuning_vanes = value;
            }
        }

        public ElbowType ElbowType
        {
            get
            {
                return _elbow_type;
            }
            set
            {
                _elbow_type = value;
            }
        }
    }

    [Serializable()]
    public class Junction : ElementsBase
    {
        private JunctionBranch local = null;
        private JunctionMain main_in = null;
        private JunctionMain main_out = null;

        /// <summary>Trójnik.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductTypeMainIn">Typ głównego króćca podłączeniowego od strony wlotowej.</param>
        /// <param name="ductTypeBranch">Typ króćca odgałęźnego.</param>
        /// <param name="airFlowMainIn">Przepływ powietrza na wlocie do elementu [m3/h].</param>
        /// <param name="airFlowBranch">Przepływ powietrza przez odgałęzienie [m3/h].</param>
        /// <param name="widthMainIn">Szerokość głównego króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="heightMainIn">Wysokość głównego króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="diameterMainIn">Średnica głównego króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="widthMainOut">Szerokość głównego króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="heightMainOut">Wysokość głównego króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="diameterMainOut">Średnica głównego króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="widthBranch">Szerokość króćca odgałęźnego [mm].</param>
        /// <param name="heightBranch">Wysokość króćca odgałęźnego [mm].</param>
        /// <param name="diameterBranch">Średnica króćca odgałęźnego [mm].</param>
        /// <param name="roundingBranch">Promień zaokrąglenia odgałęzienia [mm].</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Junction(string name, string comments, DuctType ductTypeMainIn, DuctType ductTypeBranch, BranchType branchType, int airFlowMainIn, int widthMainIn, int heightMainIn, int widthMainOut,
            int heightMainOut, int diameterMainIn, int diameterMainOut, int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int roundingBranch, bool include)
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

        /// <summary>Trójnik.</summary>
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
        }

        public JunctionMain Inlet
        {
            get
            {
                return main_in;
            }
        }

        public JunctionMain Outlet
        {
            get
            {
                return main_out;
            }
        }

        public new int AirFlow
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

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
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

        /// <summary>Oblicz szum generowany przez element.</summary>
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
        private int _width;
        private int _height;
        private int _lenght;
        private int _dl;
        private int _liner_thickness;
        private bool _liner_check;
        private PlenumType _plenum_type;
        private DuctConnection _in = null;
        private DuctConnection _out = null;

        /// <summary>Skrzynka tłumiąca.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="plenumIn">Typ króćca podłączeniowego od strony wlotowej.</param>
        /// <param name="plenumOut">Typ króćca podłączeniowego od strony wylotowej.</param>
        /// <param name="airFlow">Przepływ powietrza na wlocie do elementu [m3/h].</param>
        /// <param name="widthIn">Szerokość króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="heightIn">Wysokość króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="diameterIn">Średnica króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="widthOut">Szerokość króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="heightOut">Wysokość króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="diameterOut">Średnica króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="plenumWidth">Szerokość skrzynki tłumiącej [mm].</param>
        /// <param name="plenumHeight">Wysokość skrzynki tłumiącej [mm].</param>
        /// <param name="plenumLenght">Długość skrzynki tłumiącej [mm].</param>
        /// <param name="linerThickness">Grubość izoloacji akustycznej skrzynki [mm].</param>
        /// <param name="linerCheck">Czy skrzynka jest zaizolowana akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Plenum(string name, string comments, PlenumType plenumType, DuctType plenumIn, DuctType plenumOut, int airFlow, int widthIn, int heightIn, int widthOut, int heightOut, int diameterIn,
             int diameterOut, int plenumWidth, int plenumHeight, int plenumLenght, int inLocationLenght, int linerThickness, bool linerCheck, bool include)
        {
            _type = "plenum";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            _plenum_type = plenumType;
            _width = plenumWidth;
            _height = plenumHeight;
            _lenght = plenumLenght;
            _dl = inLocationLenght;
            _liner_check = linerCheck;
            _liner_thickness = linerThickness;
            _in = new DuctConnection(plenumIn, base.AirFlow, widthIn, heightIn, diameterIn);
            _out = new DuctConnection(plenumOut, base.AirFlow, widthOut, heightOut, diameterOut);
        }

        /// <summary>Skrzynka tłumiąca.</summary>
        public Plenum()
        {
            _type = "plenum";
            this.Comments = "";
            this.Name = "pln_1";
            this.AirFlow = 800;
            this.Include = true;
            _plenum_type = PlenumType.VerticalConnection;
            _width = 400;
            _height = 250;
            _lenght = 500;
            _dl = 300;
            _liner_check = false;
            _liner_thickness = 25;
            _in = new DuctConnection(DuctType.Round, base.AirFlow, 200, 160, 160);
            _out = new DuctConnection(DuctType.Rectangular, base.AirFlow, 400, 250, 250);
        }     
       
        private void UpdateWidth()
        {
            int temp_in, temp_out;

            if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Width; }
            else { temp_in = _in.Diameter; }

            if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Width; }
            else { temp_out = _out.Diameter; }

            if (_plenum_type == PlenumType.HorizontalConnection)
            {
                if (Math.Max(temp_in, temp_out) > _width)
                {
                    _width = Math.Max(temp_in, temp_out);
                }
            }
            else
            {
                if (temp_in > _width)
                {
                    _width = temp_in;
                }
            }
        }
        
        private void UpdateHeight()
        {
            int temp_in, temp_out;

            if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
            else { temp_in = _in.Diameter; }

            if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
            else { temp_out = _out.Diameter; }

            if (_plenum_type == PlenumType.HorizontalConnection)
            {
                if (Math.Max(temp_in, temp_out) > _height)
                {
                    _height = Math.Max(temp_in, temp_out);
                }
            }
            else
            {
                if (temp_in > _height)
                {
                    _height = temp_in;
                }
            }
        }

        private void UpdateLenght()
        {
            int temp_in, temp_out;

            if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
            else { temp_in = _in.Diameter; }

            if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
            else { temp_out = _out.Diameter; }

            if (_plenum_type == PlenumType.VerticalConnection)
            {
                if (temp_in > _lenght)
                {
                    _lenght = temp_in;
                }
            }
            else
            {
                if (Math.Min(temp_in, temp_out) > _lenght)
                {
                    _lenght = Math.Min(temp_in, temp_out);
                }
            }
        }

        private void UpdateDistance()
        {
            int temp;

            if (_in.DuctType == DuctType.Rectangular) { temp = _in.Height; }
            else { temp = _in.Diameter; }

            if (Math.Floor(0.5 * temp) > _dl)
            {
                _dl = (int)Math.Floor(0.5 * temp);
            }
            else if (!(Math.Ceiling(_lenght - 0.5 * temp) > _dl))
            {
                _dl = (int)Math.Ceiling(_lenght - 0.5 * temp);
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];
            double dq;

            if (_plenum_type == PlenumType.VerticalConnection)
            {
                if ((4 - 4 * ((_lenght - _dl) / _lenght)) >= 2.0 && (4 - 4 * ((_lenght - _dl) / _lenght)) <= 4.0)
                {
                    dq = 4 - 4 * ((_lenght - _dl) / _lenght);
                }
                else if ((4 - 4 * ((_lenght - _dl) / _lenght)) > 4.0)
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
                attn = HVACAcoustic.Attenuation.PlenumInletRectangular(_liner_thickness / 10.0, dq, _in.Width / 1000.0 * _in.Height / 1000.0, _out.Width / 1000.0 * _out.Height / 1000.0,
                    Math.Max(_in.Width, _in.Height), _dl / 1000.0, _lenght / 1000.0, _width / 1000.0, _height / 1000.0);
            }
            else if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.PlenumInletRectangular(_liner_thickness / 10.0, dq, _in.Width / 1000.0 * _in.Height / 1000.0, Math.Pow(_out.Diameter / 1000.0, 2) * 0.25 * Math.PI,
                    Math.Max(_in.Width, _in.Height), _dl / 1000.0, _lenght / 1000.0, _width / 1000.0, _height / 1000.0);
            }
            else if (_in.DuctType == DuctType.Round && _out.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.PlenumInletRound(_liner_thickness / 10.0, dq, _out.Width / 1000.0 * _out.Height / 1000.0, _in.Diameter / 1000.0, _dl / 1000.0, _lenght / 1000.0,
                    _width / 1000.0, _height / 1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.PlenumInletRound(_liner_thickness / 10.0, dq, Math.Pow(_out.Width / 1000.0, 2) * 0.25 * Math.PI, _in.Diameter / 1000.0, _dl / 1000.0, _lenght / 1000.0,
                    _width / 1000.0, _height / 1000.0);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
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
        }

        public DuctConnection Outlet
        {
            get
            {
                return _out;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                int temp_in, temp_out;

                if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Width; }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Width; }
                else { temp_out = _out.Diameter; }

                if (_plenum_type == PlenumType.HorizontalConnection)
                {
                    if (Math.Max(temp_in,temp_out) > value)
                    {
                        _width = Math.Max(temp_in, temp_out);
                    }
                    else
                    {
                        _width = value;
                    }
                }
                else
                {
                    if (temp_in > value)
                    {
                        _width = temp_in;
                    }
                    else
                    {
                        _width = value;
                    }
                }
                UpdateHeight();
                UpdateLenght();
                UpdateDistance();
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
                int temp_in, temp_out;

                if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
                else { temp_out = _out.Diameter; }

                if (_plenum_type == PlenumType.HorizontalConnection)
                {
                    if (Math.Max(temp_in, temp_out) > value)
                    {
                        _height = Math.Max(temp_in, temp_out);
                    }
                    else
                    {
                        _height = value;
                    }
                }
                else
                {
                    if (temp_in > value)
                    {
                        _height = temp_in;
                    }
                    else
                    {
                        _height = value;
                    }
                }
                UpdateWidth();
                UpdateLenght();
                UpdateDistance();
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
                int temp_in, temp_out;

                if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
                else { temp_out = _out.Diameter; }

                if (_plenum_type == PlenumType.VerticalConnection)
                {
                    if (temp_in > value)
                    {
                        _lenght = temp_in;
                    }
                    else
                    {
                        _lenght = value;
                    }
                }
                else
                {
                    if (Math.Min(temp_in,temp_out) > value)
                    {
                        _lenght = Math.Min(temp_in, temp_out);
                    }
                    else
                    {
                        _lenght = value;
                    }
                }
                UpdateHeight();
                UpdateWidth();
                UpdateDistance();
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
                if (value < 25)
                {
                    _liner_thickness = 25;
                }
                else if (value < 100)
                {
                    _liner_thickness = value;
                }
                else
                {
                    _liner_thickness = 100;
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

        public PlenumType PlenumType
        {
            get
            {
                return _plenum_type;
            }
            set
            {
                _plenum_type = value;
                UpdateLenght();
                UpdateWidth();
                UpdateHeight();
                UpdateDistance();
            }
        }

        public int InletDistance
        {
            get
            {
                return _dl;
            }
            set
            {
                int temp;

                if (_in.DuctType == DuctType.Rectangular) { temp = _in.Height; }
                else { temp = _in.Diameter; }

                if (Math.Floor(0.5 * temp) > value)
                {
                    _dl = (int)Math.Floor(0.5 * temp);
                }
                else if (Math.Ceiling(_lenght - 0.5 * temp) > value)
                {
                    _dl = value;
                }
                else
                {
                    _dl = (int)Math.Ceiling(_lenght - 0.5 * temp);
                }
                UpdateHeight();
                UpdateWidth();
                UpdateLenght();
            }
        }
    }

    [Serializable()]
    public class Damper : ElementsBase, IDimensions
    {
        private int _diameter;
        private int _width;
        private int _height;
        private byte _blade_number;
        private byte _blade_angle;
        private DuctType _duct_type;
        private DamperType _damper_type;

        public Damper(string name, string comments, DamperType damperType, DuctType ductType, int airFlow, int width, int height,
             int diameter, bool include)
        {
            _type = "damper";
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.Include = include;
            _damper_type = damperType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _duct_type = ductType;
        }

        public Damper()
        {
            _type = "damper";
            this.Comments = "";
            this.Name = "dmp_1";
            this.AirFlow = 800;
            this.Include = true;
            _damper_type = DamperType.SingleBlade;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _duct_type = DuctType.Rectangular;
        }

        public override double[] Attenuation()
        {
            double[] attn = { 0, 0, 0, 0, 0, 0, 0, 0 };
            return attn;
        }

        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_duct_type==DuctType.Rectangular)
            {
                if (_damper_type == DamperType.SingleBlade)
                {
                    lw = HVACAcoustic.Noise.DamperRectangular(1, _blade_angle, base.AirFlow, _width / 1000.0, _height / 1000.0);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DamperRectangular(_blade_number, _blade_angle, base.AirFlow, _width / 1000.0, _height / 1000.0);
                }
            }
            else
            {
                lw = HVACAcoustic.Noise.DamperRound(_blade_angle, base.AirFlow, _diameter / 1000.0);
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
                if (_duct_type == DuctType.Rectangular)
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
                return _blade_number;
            }
            set
            {
                _blade_number = value;
            }
        }

        public byte BladeAngle
        {
            get
            {
                return _blade_angle;
            }
            set
            {
                _blade_angle = value;
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
                _duct_type = value;
            }
        }

        public DamperType DamperType
        {
            get
            {
                return _damper_type;
            }
            set
            {
                _damper_type = value;
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

        public Grill(string name, string comments, GrillType grillType, GrillLocation grillLocation, int airFlow, int width, int height,
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
                else if (value < 1600)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 1600;
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

        public Fan(string name, string comments, FanType fanType, int airFlow, double pressureDrop, int rpm, byte bladeNumber, byte efficientDeviation,
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

        public DoubleJunction(string name, string comments, DuctType ductTypeMain, int airFlowMainIn, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft, bool include)
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

        public new int AirFlow
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
                    container.AirFlowBranchRight = (int)(temp * value);
                    container.AirFlowBranchLeft = (int)((1 - temp) * value);
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
        public Silencer(string name, string comments, SilencerType silencerType, DuctType ductType, int airFlow, int width, int height,
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
                if (value < 100)
                {
                    _width = 100;
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
                if (value < 100)
                {
                    _height = 100;
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
                else if (value < 1600)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 1600;
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
    public class ElementTreeCollection : List<ElementsBase>
    {

    }
}
