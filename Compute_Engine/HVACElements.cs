using HVACAcoustic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace HVACElements
{
    public interface IBranch: IRectangular, IRound, IVelocity, IElementsContainer
    {
        double[] Attenuation();
        double[] Noise();
        int AirFlow { get; set; }
        int Rounding { get; set; }
        BranchType BranchType { get; set; }
    }

    public interface IRectangular
    {
        int Width { get; set; }
        int Height { get; set; }
    }

    public interface IRound
    {
        int Diameter { get; set; }
    }

    public interface IVelocity
    {
        double Velocity { get; }
    }

    public interface IChangeableDimensions<T> where T: IRectangular, IRound, IVelocity
    {
        T Inlet { get; }
        T Outlet { get; }
    }

    public interface IDoubleBranchingElement<T> where T: IBranch
    {
        T BranchRight { get; }
        T BranchLeft { get; }
    }

    public interface ISingleBranchingElement<T> where T : IBranch
    {
        T Branch { get; }
    }

    public interface IOctaveBandAttenuation
    {
        int OctaveBand63Hz { get; set; }
        int OctaveBand125Hz { get; set; }
        int OctaveBand250Hz { get; set; }
        int OctaveBand500Hz { get; set; }
        int OctaveBand1000Hz { get; set; }
        int OctaveBand2000Hz { get; set; }
        int OctaveBand4000Hz { get; set; }
        int OctaveBand8000Hz { get; set; }
    }

    public interface IOctaveBandAbsorption
    {
        double OctaveBand63Hz { get; set; }
        double OctaveBand125Hz { get; set; }
        double OctaveBand250Hz { get; set; }
        double OctaveBand500Hz { get; set; }
        double OctaveBand1000Hz { get; set; }
        double OctaveBand2000Hz { get; set; }
        double OctaveBand4000Hz { get; set; }
        double OctaveBand8000Hz { get; set; }
    }

    public interface IElementsContainer
    {
        ElementsCollection Elements { get; }
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

    public enum NoiseLocation
    {
        RoomCenter = 1,
        SurfaceCenter = 2,
        SurfaceCorner = 3,
        SurfaceEdge = 4,
    }

    public enum ElementType
    {
        Duct = 0,
        Diffuser = 1,
        Bow = 2,
        Elbow = 3,
        Junction = 4,
        DoubleJunction = 5,
        TJunction = 6,
        Plenum = 7,
        Damper = 8,
        Grill = 9,
        Fan = 10,
        Silencer = 11,
        Room = 12
    }

    internal enum JunctionConnectionSide
    {
        Inlet = 0,
        Outlet = 1
    }

    [Serializable]
    public abstract class ElementsBase: ICloneable
    {
        public ElementsBase Parent { get; internal set; }
        protected ElementType _type;
        private int _airflow;
        private string _name;
        private string _comments;
        private bool _include;

        public abstract double[] Attenuation();

        public abstract double[] Noise();

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, this);
                stream.Position = 0;
                return bf.Deserialize(stream);
            }
        }

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

        public bool IsIncluded
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

        public ElementType Type
        {
            get
            {
                return _type;
            }
        }
    }

    [Serializable]
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
        private readonly ElementsCollection _elements = null;

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
            _elements = new ElementsCollection();
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

        public ElementsCollection Elements
        {
            get { return _elements; }
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

    [Serializable]
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
        private readonly DuctConnection _in = null;
        private readonly DuctConnection _out = null;

        internal DoubleJunctionContaier(DuctType ductTypeMain, int airFlowMain, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
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

        internal int AirFlowBranchRight
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

        internal int WidthBranchRight
        {
            get
            {
                return width_branch_right;
            }
            set
            {
                if (value < 100)
                {
                    width_branch_right = 100;
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

        internal int HeightBranchRight
        {
            get
            {
                return height_branch_right;
            }
            set
            {
                if (value < 100)
                {
                    height_branch_right = 100;
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

        internal int DiameterBranchRight
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

        internal int RoundingBranchRight
        {
            get
            {
                return rnd_branch_right;
            }
            set
            {
                if (value < 0)
                {
                    rnd_branch_right = 0;
                }
                else if (value < Math.Ceiling(0.6 * width_branch_right))
                {
                    rnd_branch_right = value;
                }
                else
                {
                    rnd_branch_right = (int)Math.Ceiling(0.6 * width_branch_right);
                }
            }
        }

        internal BranchType BranchTypeRight
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

        internal int AirFlowBranchLeft
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

        internal int WidthBranchLeft
        {
            get
            {
                return width_branch_left;
            }
            set
            {
                if (value < 100)
                {
                    width_branch_left = 100;
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

        internal int HeightBranchLeft
        {
            get
            {
                return height_branch_left;
            }
            set
            {
                if (value < 100)
                {
                    height_branch_left = 100;
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

        internal int DiameterBranchLeft
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

        internal int RoundingBranchLeft
        {
            get
            {
                return rnd_branch_left;
            }
            set
            {
                if (value < 0)
                {
                    rnd_branch_left = 0;
                }
                else if (value < Math.Ceiling(0.6 * width_branch_left))
                {
                    rnd_branch_left = value;
                }
                else
                {
                    rnd_branch_left = (int)Math.Ceiling(0.6 * width_branch_left);
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

        internal BranchType BranchTypeLeft
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

        internal DuctType DuctType
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

    [Serializable]
    internal class TJunctionContaier
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
        private int width_main;
        private int height_main;
        private int diameter_main;
        private DuctType duct_type_main;
        private TJunction _local_TJunction = null;

        internal TJunctionContaier(TJunction baseTJunction, DuctType ductTypeMain, int widthMainIn, int heightMainIn,
            int diameterMainIn, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft)
        {
            _local_TJunction = baseTJunction;
            width_main =  widthMainIn;
            height_main = heightMainIn;
            diameter_main = diameterMainIn;
            duct_type_main = ductTypeMain;
            duct_type_branch = ductTypeBranch;
            airflow_branch_right = airFlowBranchRight;
            width_branch_right = widthBranchRight;
            height_branch_right = heightBranchRight;
            diameter_branch_right = diameterBranchRight;
            rnd_branch_right = roundingRight;
            branch_type_right = branchTypeRight;
            airflow_branch_left = airFlowBranchLeft;
            width_branch_left = widthBranchLeft;
            height_branch_left = heightBranchLeft;
            diameter_branch_left = diameterBranchLeft;
            rnd_branch_left = roundingLeft;
            branch_type_left = branchTypeLeft;
        }

        internal int AirFlowMain
        {
            get
            {
                return airflow_branch_right + airflow_branch_left;
            }
            set
            {
                double temp = Convert.ToDouble(airflow_branch_right) / Convert.ToDouble(airflow_branch_right + airflow_branch_left);
                airflow_branch_right = (int)Math.Round(temp * value);
                airflow_branch_left = (int)Math.Round((1 - temp) * value);
                _local_TJunction.AirFlow = airflow_branch_right + airflow_branch_left;
            }
        }

        internal int WidthMain
        {
            get
            {
                return width_main;
            }
            set
            {
                if (value < 100)
                {
                    width_main = 100;
                }
                else if (value < 2000)
                {
                    width_main = value;
                }
                else
                {
                    width_main = 2000;
                }
            }
        }

        internal int HeightMain
        {
            get
            {
                return height_main;
            }
            set
            {
                if (value < 100)
                {
                    height_main = 100;
                }
                else if (value < 2000)
                {
                    height_main = value;
                }
                else
                {
                    height_main = 2000;
                }
            }
        }

        internal int DiameterMain
        {
            get
            {
                return diameter_main;
            }
            set
            {
                if (value < 80)
                {
                    diameter_main = 80;
                }
                else if (value < 1600)
                {
                    diameter_main = value;
                }
                else
                {
                    diameter_main = 1600;
                }
            }
        }

        internal DuctType DuctTypeMain
        {
            get
            {
                return duct_type_main;
            }
            set
            {
                duct_type_main = value;
            }
        }

        internal int AirFlowBranchRight
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

        internal int WidthBranchRight
        {
            get
            {
                return width_branch_right;
            }
            set
            {
                if (value < 100)
                {
                    width_branch_right = 100;
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

        internal int HeightBranchRight
        {
            get
            {
                return height_branch_right;
            }
            set
            {
                if (value < 100)
                {
                    height_branch_right = 100;
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

        internal int DiameterBranchRight
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

        internal int RoundingBranchRight
        {
            get
            {
                return rnd_branch_right;
            }
            set
            {
                if (value < 0)
                {
                    rnd_branch_right = 0;
                }
                else if (value < Math.Ceiling(0.6 * width_branch_right))
                {
                    rnd_branch_right = value;
                }
                else
                {
                    rnd_branch_right = (int)Math.Ceiling(0.6 * width_branch_right);
                }
            }
        }

        internal BranchType BranchTypeRight
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

        internal int AirFlowBranchLeft
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

        internal int WidthBranchLeft
        {
            get
            {
                return width_branch_left;
            }
            set
            {
                if (value < 100)
                {
                    width_branch_left = 100;
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

        internal int HeightBranchLeft
        {
            get
            {
                return height_branch_left;
            }
            set
            {
                if (value < 100)
                {
                    height_branch_left = 100;
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

        internal int DiameterBranchLeft
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

        internal int RoundingBranchLeft
        {
            get
            {
                return rnd_branch_left;
            }
            set
            {
                if (value < 0)
                {
                    rnd_branch_left = 0;
                }
                else if (value < Math.Ceiling(0.6 * width_branch_left))
                {
                    rnd_branch_left = value;
                }
                else
                {
                    rnd_branch_left = (int)Math.Ceiling(0.6 * width_branch_left);
                }
            }
        }

        internal BranchType BranchTypeLeft
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

        internal DuctType DuctTypeBranch
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

    [Serializable]
    public class SoundAttenuation: IOctaveBandAttenuation
    {
        private int _octaveBand63Hz;
        private int _octaveBand125Hz;
        private int _octaveBand250Hz;
        private int _octaveBand500Hz;
        private int _octaveBand1000Hz;
        private int _octaveBand2000Hz;
        private int _octaveBand4000Hz;
        private int _octaveBand8000Hz;

        internal SoundAttenuation(int octaveBand63Hz, int octaveBand125Hz, int octaveBand250Hz, int octaveBand500Hz,
            int octaveBand1000Hz, int octaveBand2000Hz, int octaveBand4000Hz, int octaveBand8000Hz)
        {
            _octaveBand63Hz = octaveBand63Hz;
            _octaveBand125Hz = octaveBand125Hz;
            _octaveBand250Hz = octaveBand250Hz;
            _octaveBand500Hz = octaveBand500Hz;
            _octaveBand1000Hz = octaveBand1000Hz;
            _octaveBand2000Hz = octaveBand2000Hz;
            _octaveBand4000Hz = octaveBand4000Hz;
            _octaveBand8000Hz = octaveBand8000Hz;
        }

        public double TotalAttenution()
        {
            double result;
            result = HVACAcoustic.MathOperations.OctaveSum(_octaveBand63Hz, _octaveBand125Hz, _octaveBand250Hz, _octaveBand500Hz, _octaveBand1000Hz, _octaveBand2000Hz,
                _octaveBand4000Hz, OctaveBand8000Hz);

            return result;
        }

        public int OctaveBand63Hz
        {
            get { return _octaveBand63Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand63Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand63Hz = value;
                }
                else
                {
                    _octaveBand63Hz = 99;
                }
            }
        }

        public int OctaveBand125Hz
        {
            get { return _octaveBand125Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand125Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand125Hz = value;
                }
                else
                {
                    _octaveBand125Hz = 99;
                }
            }
        }

        public int OctaveBand250Hz
        {
            get { return _octaveBand250Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand250Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand250Hz = value;
                }
                else
                {
                    _octaveBand250Hz = 99;
                }
            }
        }

        public int OctaveBand500Hz
        {
            get { return _octaveBand500Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand500Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand500Hz = value;
                }
                else
                {
                    _octaveBand500Hz = 99;
                }
            }
        }

        public int OctaveBand1000Hz
        {
            get { return _octaveBand1000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand1000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand1000Hz = value;
                }
                else
                {
                    _octaveBand1000Hz = 99;
                }
            }
        }

        public int OctaveBand2000Hz
        {
            get { return _octaveBand2000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand2000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand2000Hz = value;
                }
                else
                {
                    _octaveBand2000Hz = 99;
                }
            }
        }

        public int OctaveBand4000Hz
        {
            get { return _octaveBand4000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand4000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand4000Hz = value;
                }
                else
                {
                    _octaveBand4000Hz = 99;
                }
            }
        }

        public int OctaveBand8000Hz
        {
            get { return _octaveBand8000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand8000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand8000Hz = value;
                }
                else
                {
                    _octaveBand8000Hz = 99;
                }
            }
        }
    }

    [Serializable]
    public class RoomConstant: IOctaveBandAbsorption
    {
        private double _octaveBand63Hz;
        private double _octaveBand125Hz;
        private double _octaveBand250Hz;
        private double _octaveBand500Hz;
        private double _octaveBand1000Hz;
        private double _octaveBand2000Hz;
        private double _octaveBand4000Hz;
        private double _octaveBand8000Hz;
        private readonly Room _room = null;

        internal RoomConstant(Room room, double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz)
        {
            _room = room;
            _octaveBand63Hz = octaveBand63Hz;
            _octaveBand125Hz = octaveBand125Hz;
            _octaveBand250Hz = octaveBand250Hz;
            _octaveBand500Hz = octaveBand500Hz;
            _octaveBand1000Hz = octaveBand1000Hz;
            _octaveBand2000Hz = octaveBand2000Hz;
            _octaveBand4000Hz = octaveBand4000Hz;
            _octaveBand8000Hz = octaveBand8000Hz;
        }

        internal double[] MaxAbsorption(Room room)
        {
            double s, mfp;
            double[] m = new double[8];

            s = 2 * (Unit.MToFt(room.Width) * Unit.MToFt(room.Lenght)) + 2 * (Unit.MToFt(room.Lenght) * Unit.MToFt(room.Height))
                + 2 * (Unit.MToFt(room.Width) * Unit.MToFt(room.Height));
            mfp = 4 * (Unit.MToFt(room.Width) * Unit.MToFt(room.Height) * Unit.MToFt(room.Lenght)) / s;

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = Transmission.M_coeff(room.Temperature, room.RelativeHumidity)[i] / Unit.MToFt(1) * mfp;
            }

            return m;
        }

        public double OctaveBand63Hz
        {
            get { return _octaveBand63Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand63Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[0],2)))
                {
                    _octaveBand63Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand63Hz = 0.99 - Math.Round(MaxAbsorption(_room)[0], 2);
                }
            }
        }

        public double OctaveBand125Hz
        {
            get { return _octaveBand125Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand125Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[1], 2)))
                {
                    _octaveBand125Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand125Hz = 0.99 - Math.Round(MaxAbsorption(_room)[1], 2);
                }
            }
        }

        public double OctaveBand250Hz
        {
            get { return _octaveBand250Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand250Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[2], 2)))
                {
                    _octaveBand250Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand250Hz = 0.99 - Math.Round(MaxAbsorption(_room)[2], 2);
                }
            }
        }

        public double OctaveBand500Hz
        {
            get { return _octaveBand500Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand500Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[3], 2)))
                {
                    _octaveBand500Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand500Hz = 0.99 - Math.Round(MaxAbsorption(_room)[3], 2);
                }
            }
        }

        public double OctaveBand1000Hz
        {
            get { return _octaveBand1000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand1000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[4], 2)))
                {
                    _octaveBand1000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand1000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[4], 2);
                }
            }
        }

        public double OctaveBand2000Hz
        {
            get { return _octaveBand2000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand2000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[5], 2)))
                {
                    _octaveBand2000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand2000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[5], 2);
                }
            }
        }

        public double OctaveBand4000Hz
        {
            get { return _octaveBand4000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand4000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[6], 2)))
                {
                    _octaveBand4000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand4000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[6], 2);
                }
            }
        }

        public double OctaveBand8000Hz
        {
            get { return _octaveBand8000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand8000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[7], 2)))
                {
                    _octaveBand8000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand8000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[7], 2);
                }
            }
        }
    }

    [Serializable]
    public class DoubleJunctionBranch : IBranch
    {
        private DoubleJunctionContaier _container = null;
        private readonly Branch _branch;
        private JunctionConnectionSide _connectionSide;
        private readonly ElementsCollection _elements = null;

        internal DoubleJunctionBranch(DoubleJunctionContaier doubleJunctionContaier, Branch doubleJunctionBranch)
        {
            _container = doubleJunctionContaier;
            _branch = doubleJunctionBranch;
            _elements = new ElementsCollection();
        }

        public int AirFlow
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.AirFlowBranchRight;
                }
                else
                {
                    return _container.AirFlowBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    if (value <= (_container.In.AirFlow - _container.AirFlowBranchLeft))
                    {
                        _container.AirFlowBranchRight = value;

                        if (_connectionSide == JunctionConnectionSide.Inlet)
                        {
                            _container.Out.AirFlow = _container.In.AirFlow - _container.AirFlowBranchLeft - value;
                        }
                        else
                        {
                            _container.In.AirFlow = _container.Out.AirFlow + _container.AirFlowBranchLeft + value;
                        }
                    }
                    else
                    {
                        _container.AirFlowBranchRight = _container.In.AirFlow - _container.AirFlowBranchLeft;
                        _container.Out.AirFlow = 0;
                    }
                }
                else
                {
                    if (value <= (_container.In.AirFlow - _container.AirFlowBranchRight))
                    {
                        _container.AirFlowBranchLeft = value;

                        if (_connectionSide == JunctionConnectionSide.Inlet)
                        {
                            _container.Out.AirFlow = _container.In.AirFlow - _container.AirFlowBranchRight - value;
                        }
                        else
                        {
                            _container.In.AirFlow = _container.Out.AirFlow + _container.AirFlowBranchRight + value;
                        }
                    }
                    else
                    {
                        _container.AirFlowBranchRight = _container.In.AirFlow - _container.AirFlowBranchRight;
                        _container.Out.AirFlow = 0;
                    }
                }
            }
        }

        internal JunctionConnectionSide JunctionConnectionSide
        {
            get
            {
                return _connectionSide;
            }
            set
            {
                _connectionSide = value;
            }
        }

        public int Rounding
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.RoundingBranchRight;
                }
                else
                {
                    return _container.RoundingBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.RoundingBranchRight = value;
                }
                else
                {
                    _container.RoundingBranchLeft = value;
                }
            }
        }

        public BranchType BranchType
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.BranchTypeRight;
                }
                else
                {
                    return _container.BranchTypeLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.BranchTypeRight = value;
                }
                else
                {
                    _container.BranchTypeLeft = value;
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _container.DuctType;
            }
            set
            {
                _container.DuctType = value;
            }
        }

        public int Width
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.WidthBranchRight;
                }
                else
                {
                    return _container.WidthBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.WidthBranchRight = value;
                }
                else
                {
                    _container.WidthBranchLeft = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.HeightBranchRight;
                }
                else
                {
                    return _container.HeightBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.HeightBranchRight = value;
                }
                else
                {
                    _container.HeightBranchLeft = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.DiameterBranchRight;
                }
                else
                {
                    return _container.DiameterBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.DiameterBranchRight = value;
                }
                else
                {
                    _container.DiameterBranchLeft = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    if (_container.DuctType == DuctType.Rectangular)
                    {
                        return (_container.AirFlowBranchRight / 3600.0) / ((_container.WidthBranchRight / 1000.0) * (_container.HeightBranchRight / 1000.0));
                    }
                    else
                    {
                        return (_container.AirFlowBranchRight / 3600.0) / (0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight / 1000.0, 2));
                    }
                }
                else
                {
                    if (_container.DuctType == DuctType.Rectangular)
                    {
                        return (_container.AirFlowBranchLeft / 3600.0) / ((_container.WidthBranchLeft / 1000.0) * (_container.HeightBranchLeft / 1000.0));
                    }
                    else
                    {
                        return (_container.AirFlowBranchLeft/ 3600.0) / (0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2));
                    }
                }
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_branch == Branch.BranchRight)
            {
                if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height,
                        _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Diameter, _container.Out.Diameter, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Diameter, _container.Out.Diameter, _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
            }
            else
            {
                if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.BranchLeft, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.BranchLeft, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height,
                        _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.BranchLeft, _container.BranchTypeRight,
                        _container.In.Diameter, _container.Out.Diameter, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.BranchLeft, _container.BranchTypeRight,
                        _container.In.Diameter, _container.Out.Diameter, _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public double[] Noise()
        {
            double[] lw = new double[8];

            if (_branch == Branch.BranchRight)
            {
                if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter/1000.0,2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight/1000.0,2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight/1000.0,2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
            }
            else
            {
                if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
            }
            return lw;
        }

        public ElementsCollection Elements
        {
            get { return _elements; }
        }
    }

    [Serializable]
    public class TJunctionBranch : IBranch
    {
        private TJunctionContaier _container = null;
        private readonly Branch _branch;
        private readonly ElementsCollection _elements = null;

        internal TJunctionBranch(TJunctionContaier TJunctionContaier, Branch doubleJunctionBranch)
        {
            _container = TJunctionContaier;
            _branch = doubleJunctionBranch;
            _elements = new ElementsCollection();
        }

        public int AirFlow
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.AirFlowBranchRight;
                }
                else
                {
                    return _container.AirFlowBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    if (value < _container.AirFlowMain)
                    {
                        _container.AirFlowBranchLeft = _container.AirFlowMain - value;
                        _container.AirFlowBranchRight = value;
                    }
                    else
                    {
                        _container.AirFlowBranchRight = _container.AirFlowMain;
                        _container.AirFlowBranchLeft= 0;
                    }
                }
                else
                {
                    if (value < _container.AirFlowMain)
                    {
                        _container.AirFlowBranchRight = _container.AirFlowMain - value;
                        _container.AirFlowBranchLeft = value;
                    }
                    else
                    {
                        _container.AirFlowBranchLeft = _container.AirFlowMain;
                        _container.AirFlowBranchRight = 0;
                    }
                }
            }
        }

        public int Rounding
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.RoundingBranchRight;
                }
                else
                {
                    return _container.RoundingBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.RoundingBranchRight = value;
                }
                else
                {
                    _container.RoundingBranchLeft = value;
                }
            }
        }

        public BranchType BranchType
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.BranchTypeRight;
                }
                else
                {
                    return _container.BranchTypeLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.BranchTypeRight = value;
                }
                else
                {
                    _container.BranchTypeLeft = value;
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _container.DuctTypeBranch;
            }
            set
            {
                _container.DuctTypeBranch = value;
            }
        }

        public int Width
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.WidthBranchRight;
                }
                else
                {
                    return _container.WidthBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.WidthBranchRight = value;
                }
                else
                {
                    _container.WidthBranchLeft = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.HeightBranchRight;
                }
                else
                {
                    return _container.HeightBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.HeightBranchRight = value;
                }
                else
                {
                    _container.HeightBranchLeft = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    return _container.DiameterBranchRight;
                }
                else
                {
                    return _container.DiameterBranchLeft;
                }
            }
            set
            {
                if (_branch == Branch.BranchRight)
                {
                    _container.DiameterBranchRight = value;
                }
                else
                {
                    _container.DiameterBranchLeft = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (_branch == Branch.BranchRight)
                {
                    if (_container.DuctTypeBranch == DuctType.Rectangular)
                    {
                        return (_container.AirFlowBranchRight / 3600.0) / ((_container.WidthBranchRight / 1000.0) * (_container.HeightBranchRight / 1000.0));
                    }
                    else
                    {
                        return (_container.AirFlowBranchRight / 3600.0) / (0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight / 1000.0, 2));
                    }
                }
                else
                {
                    if (_container.DuctTypeBranch == DuctType.Rectangular)
                    {
                        return (_container.AirFlowBranchLeft / 3600.0) / ((_container.WidthBranchLeft / 1000.0) * (_container.HeightBranchLeft / 1000.0));
                    }
                    else
                    {
                        return (_container.AirFlowBranchLeft / 3600.0) / (0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2));
                    }
                }
            }
        }

        public ElementsCollection Elements
        {
            get { return _elements; }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_branch == Branch.BranchRight)
            {
                if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRectangular(Branch.BranchRight, _container.BranchTypeRight, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5),
                         Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5), _container.WidthBranchRight, _container.HeightBranchRight);
                }
                else if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRound(Branch.BranchRight, _container.BranchTypeRight, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5),
                        Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5), _container.DiameterBranchRight);
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRectangular(Branch.BranchRight, _container.BranchTypeRight, _container.DiameterMain,
                        Math.Pow((4 * _container.WidthBranchLeft * _container.HeightBranchLeft) / Math.PI, 0.5), _container.WidthBranchRight, _container.HeightBranchRight);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRound(Branch.BranchRight, _container.BranchTypeRight, _container.DiameterMain,
                        _container.DiameterBranchLeft, _container.DiameterBranchRight);
                }
            }
            else
            {
                if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRectangular(Branch.BranchLeft, _container.BranchTypeLeft, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5),
                         Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5), _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Round)
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRound(Branch.BranchLeft, _container.BranchTypeLeft, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5),
                        Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5), _container.DiameterBranchLeft);
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRectangular(Branch.BranchLeft, _container.BranchTypeLeft, _container.DiameterMain,
                        Math.Pow((4 * _container.WidthBranchRight * _container.HeightBranchRight) / Math.PI, 0.5), _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else
                {
                    attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRound(Branch.BranchLeft, _container.BranchTypeLeft, _container.DiameterMain,
                        _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public double[] Noise()
        {
            double[] lw = new double[8];

            if (_branch == Branch.BranchRight)
            {
                if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                             0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                             0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
            }
            else
            {
                if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = HVACAcoustic.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
            }
            return lw;
        }
    }

    [Serializable]
    public class DuctConnection: IRectangular, IRound, IVelocity
    {
        private DuctType _duct_type;
        private int _width;
        private int _height;
        private int _diameter;
        private int _airflow;

        internal DuctConnection(DuctType ductType, int airFlow, int w, int h, int d)
        {
            _duct_type = ductType;
            _airflow = airFlow;
            _width = w;
            _height = h;
            _diameter = d;
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
                OnDimensionsChanged();
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
                OnDimensionsChanged();
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
                OnDimensionsChanged();
            }
        }

        public double Velocity
        {
            get
            {
                if (_duct_type == DuctType.Rectangular)
                {
                    return (_airflow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (_airflow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
                }
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

        internal int AirFlow
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

        internal event EventHandler DimensionsChanged;

        protected void OnDimensionsChanged()
        {
            DimensionsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [Serializable]
    public class JunctionMain: IRectangular, IRound, IVelocity
    {
        private Junction _local_junction = null;
        private JunctionConnectionSide? _junction_connection_side = null;

        internal JunctionMain(Junction baseJunction, JunctionConnectionSide junctionConnectionSide)
        {
            _junction_connection_side = junctionConnectionSide;
            _local_junction = baseJunction;
        }

        public int Width
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.Width;
                }
                else
                {
                    return _local_junction.Branch.Out.Width;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.In.Width = value;
                }
                else
                {
                    _local_junction.Branch.Out.Width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.Height;
                }
                else
                {
                    return _local_junction.Branch.Out.Height;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.In.Height = value;
                }
                else
                {
                    _local_junction.Branch.Out.Height = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.Diameter;
                }
                else
                {
                    return _local_junction.Branch.Out.Diameter;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.In.Diameter = value;
                }
                else
                {
                    _local_junction.Branch.Out.Diameter = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    if (_local_junction.Branch.In.DuctType == DuctType.Rectangular)
                    {
                        return (_local_junction.Branch.In.AirFlow / 3600.0) / ((_local_junction.Branch.In.Width / 1000.0) * (_local_junction.Branch.In.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_junction.Branch.In.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_local_junction.Branch.In.Diameter / 1000.0, 2));
                    }
                }
                else
                {
                    if (_local_junction.Branch.Out.DuctType == DuctType.Rectangular)
                    {
                        return (_local_junction.Branch.Out.AirFlow / 3600.0) / ((_local_junction.Branch.Out.Width / 1000.0) * (_local_junction.Branch.Out.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_junction.Branch.Out.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_local_junction.Branch.Out.Diameter / 1000.0, 2));
                    }
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.DuctType;
                }
                else
                {
                    return _local_junction.Branch.Out.DuctType;
                }
            }
            set
            {
                _local_junction.Branch.In.DuctType = value;
                _local_junction.Branch.Out.DuctType = value;
            }
        }

        public int AirFlow
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.AirFlow;
                }
                else
                {
                    return _local_junction.Branch.Out.AirFlow;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    _local_junction.AirFlow = value;
                }
                else
                {
                    _local_junction.Branch.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    _local_junction.AirFlow = value + _local_junction.Branch.AirFlow;
                }
            }
        }
    }

    [Serializable]
    public class DoubleJunctionMain : IRectangular, IRound, IVelocity
    {
        private DoubleJunction _local_djunction = null;
        private JunctionConnectionSide? _junction_connection_side = null;

        internal DoubleJunctionMain(DoubleJunction baseDoubleJunction, JunctionConnectionSide junctionConnectionSide)
        {
            _junction_connection_side = junctionConnectionSide;
            _local_djunction = baseDoubleJunction;
        }

        public int Width
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.Width;
                }
                else
                {
                    return _local_djunction.Container.Out.Width;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.Container.In.Width = value;
                }
                else
                {
                    _local_djunction.Container.Out.Width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.Height;
                }
                else
                {
                    return _local_djunction.Container.Out.Height;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.Container.In.Height = value;
                }
                else
                {
                    _local_djunction.Container.Out.Height = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.Diameter;
                }
                else
                {
                    return _local_djunction.Container.Out.Diameter;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.Container.In.Diameter = value;
                }
                else
                {
                    _local_djunction.Container.Out.Diameter = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    if (_local_djunction.Container.In.DuctType == DuctType.Rectangular)
                    {
                        return (_local_djunction.Container.In.AirFlow / 3600.0) / ((_local_djunction.Container.In.Width / 1000.0)
                            * (_local_djunction.Container.In.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_djunction.Container.In.AirFlow / 3600.0) /
                            (0.25 * Math.PI * Math.Pow(_local_djunction.Container.In.Diameter / 1000.0, 2));
                    }
                }
                else
                {
                    if (_local_djunction.Container.Out.DuctType == DuctType.Rectangular)
                    {
                        return (_local_djunction.Container.Out.AirFlow / 3600.0) /
                            ((_local_djunction.Container.Out.Width / 1000.0) * (_local_djunction.Container.Out.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_djunction.Container.Out.AirFlow / 3600.0) /
                            (0.25 * Math.PI * Math.Pow(_local_djunction.Container.Out.Diameter / 1000.0, 2));
                    }
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.DuctType;
                }
                else
                {
                    return _local_djunction.Container.Out.DuctType;
                }
            }
            set
            {
                _local_djunction.Container.In.DuctType = value;
                _local_djunction.Container.Out.DuctType = value;
            }
        }

        public int AirFlow
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.AirFlow;
                }
                else
                {
                    return _local_djunction.Container.Out.AirFlow;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.BranchRight.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    _local_djunction.BranchLeft.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    _local_djunction.AirFlow = value;
                }
                else
                {
                    _local_djunction.BranchRight.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    _local_djunction.BranchLeft.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    _local_djunction.AirFlow = value + _local_djunction.BranchRight.AirFlow + _local_djunction.BranchLeft.AirFlow;
                }
            }
        }
    }

    [Serializable]
    public class GrillOrifice
    {
        private int _height;
        private int _depth;

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 5)
                {
                    _height = 5;
                }
                else if (value < 30)
                {
                    _height = value;
                }
                else
                {
                    _height = 30;
                }
            }
        }

        public int Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                if (value < 10)
                {
                    _depth = 10;
                }
                else if (value < 99)
                {
                    _depth = value;
                }
                else
                {
                    _depth = 99;
                }
            }
        }

        internal GrillOrifice(int height, int depth)
        {
            this.Height = height;
            this.Depth = depth;
        }
    }

    [Serializable]
    public class Duct : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "dct_";
        private int _width;
        private int _height;
        private int _diameter;
        private double _lenght;
        private readonly int _liner_thickness;
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
            _type = ElementType.Duct;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _lenght = lenght;
            _liner_thickness = linerThickness;
            _liner_check = linerCheck;
            _counter = 1;
        }
        
        /// <summary>Kanał prosty.</summary>
        public Duct()
        {
            _type = ElementType.Duct;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 500;
            this.IsIncluded = true;
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
                    return (this.AirFlow/3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow/3600.0) / (0.25*Math.PI*Math.Pow(_diameter/1000.0,2));
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
                    _lenght = Math.Round(value, 2);
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

    [Serializable]
    public class Diffuser : ElementsBase, IChangeableDimensions<DuctConnection>
    {
        private static int _counter = 1;
        private static string _name = "dfs_";
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
            _type = ElementType.Diffuser;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlow;
            this.IsIncluded = include;
            _diffuser_type = diffuserType;
            _lenght = lenght;
            _in = new DuctConnection(diffuserIn, base.AirFlow, widthIn, heightIn, diameterIn);
            _out = new DuctConnection(diffuserOut, base.AirFlow, widthOut, heightOut, diameterOut);
            _counter = 1;
        }
        /// <summary>Dyfuzor/konfuzor lub nagłe zwężenie/rozszerzenie.</summary>
        public Diffuser()
        {
            _type = ElementType.Diffuser;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            base.AirFlow = 500;
            this.IsIncluded = true;
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

    [Serializable]
    public class Bow : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "bow_";
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
            _type = ElementType.Bow;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _rw = rw;
            _rd = rd;
            _liner_thickness = linerThckness;
            _liner_check = linerCheck;
            _counter = 1;
        }

        /// <summary>Łuk.</summary>
        public Bow()
        {
            _type = ElementType.Bow;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 500;
            this.IsIncluded = true;
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
                    return (this.AirFlow/3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow/3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
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

    [Serializable]
    public class Elbow : ElementsBase, IRectangular, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "elb_";
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
            _type = ElementType.Elbow;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _elbow_type = elbowType;
            _tuning_vanes = TurnigVanes;
            _width = width;
            _height = height;
            _vanes_number = vanesNumber;
            _rnd = rounding;
            _liner_check = linerCheck;
            _counter = 1;
        }

        /// <summary>Kolano.</summary>
        public Elbow()
        {
            _type = ElementType.Elbow;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 800;
            this.IsIncluded = true;
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
                this.Rouning = _rnd;
                this.VanesNumber = _vanes_number;
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
                this.Rouning = _rnd;
                this.VanesNumber = _vanes_number;
            }
        }

        public double Velocity
        {
            get
            {
                return (this.AirFlow/3600.0) / ((_width / 1000.0) * (_height / 1000.0));
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
                        _vanes_number = 1;
                    }
                    else if (Math.Round(2.13 * Math.Pow((_rnd/1000.0 / _width/1000.0), -1) - 1) >= value)
                    {
                        _vanes_number = value;
                    }
                    else
                    {
                        _vanes_number = (byte)Math.Round(2.13 * Math.Pow((_rnd/1000.0 / _width/1000.0), -1) - 1);
                    }
                }
                else
                {
                    double dh = 2 * _height/1000.0 * _width/1000.0 / (_height/1000.0 + _width/1000.0);

                    if (value < 1)
                    {
                        _vanes_number = 1;
                    }
                    else if (Math.Round(2.13 * Math.Pow((0.35 * dh / (_width/1000.0 * Math.Pow(2, 0.5))), (-1)) - 1) >= value)
                    {
                        _vanes_number = value;
                    }
                    else
                    {
                        _vanes_number = (byte)Math.Round(2.13 * Math.Pow((0.35 * dh / (_width/1000.0 * Math.Pow(2, 0.5))), (-1)) - 1);
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
                this.VanesNumber = _vanes_number;
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

    [Serializable]
    public class Junction : ElementsBase, IChangeableDimensions<JunctionMain>, ISingleBranchingElement<JunctionBranch>
    {
        private static int _counter = 1;
        private static string _name = "jnt_";
        private JunctionBranch _local = null;
        private JunctionMain _main_in = null;
        private JunctionMain _main_out = null;

        /// <summary>Trójnik.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductTypeMainIn">Typ głównego króćca podłączeniowego od strony wlotowej.</param>
        /// <param name="ductTypeBranch">Typ króćca odgałęźnego.</param>
        /// <param name="airFlowMainIn">Przepływ powietrza na wlocie do elementu [m3/h].</param>
        /// <param name="airFlowBranch">Przepływ powietrza przez odgałęzienie [m3/h].</param>
        /// <param name="branchType">Typ odgałęzienia.</param>
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
            _type = ElementType.Junction;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowMainIn;
            this.IsIncluded = include;
            _counter = 1;

            if (airFlowBranch >= airFlowMainIn)
            {
                airFlowBranch = airFlowMainIn;
            }

            _local = new JunctionBranch(ductTypeMainIn, base.AirFlow, widthMainIn, widthMainOut, heightMainIn, heightMainOut, diameterMainIn, diameterMainOut,
                ductTypeBranch, branchType, airFlowBranch, widthBranch, heightBranch, diameterBranch, roundingBranch);
            _main_in = new JunctionMain(this, JunctionConnectionSide.Inlet);
            _main_out = new JunctionMain(this, JunctionConnectionSide.Outlet);
            this.Branch.Elements._parent = this;
        }

        /// <summary>Trójnik.</summary>
        public Junction()
        {
            _type = ElementType.Junction;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            base.AirFlow = 2400;
            this.IsIncluded = true;

            _local = new JunctionBranch(DuctType.Rectangular, base.AirFlow, 400, 200, 400, 200, 450, 250, DuctType.Rectangular,
                BranchType.Straight, 400, 160, 160, 200, 0);
            _main_in = new JunctionMain(this, JunctionConnectionSide.Inlet);
            _main_out = new JunctionMain(this, JunctionConnectionSide.Outlet);
            this.Branch.Elements._parent = this;
        }

        public JunctionBranch Branch
        {
            get
            {
                return _local;
            }
        }

        public JunctionMain Inlet
        {
            get
            {
                return _main_in;
            }
        }

        public JunctionMain Outlet
        {
            get
            {
                return _main_out;
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
                _local.In.AirFlow = value;

                if (_local.AirFlow >= value)
                {
                    _local.AirFlow = value;
                }

                _local.Out.AirFlow = value - _local.AirFlow;
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_main_in.DuctType == DuctType.Rectangular && _local.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRectangular(HVACAcoustic.Branch.Main, _local.BranchType, _main_in.Width / 1000.0,_main_in.Height / 1000.0,
                        _main_out.Width / 1000.0, _main_out.Height / 1000.0, _local.Width / 1000.0, _local.Height / 1000.0);
            }
            else if (_main_in.DuctType == DuctType.Rectangular && _local.DuctType == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRectangularBranchRound(HVACAcoustic.Branch.Main, _local.BranchType, _main_in.Width / 1000.0, _main_in.Height / 1000.0,
                        _main_out.Width / 1000.0, _main_out.Height / 1000.0, _local.Diameter / 1000.0);

            }
            else if (_main_in.DuctType == DuctType.Round && _local.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRectangular(HVACAcoustic.Branch.Main, _local.BranchType, _main_in.Diameter / 1000.0,
                        _main_out.Diameter / 1000.0, _local.Width / 1000.0, _local.Height / 1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.JunctionMainRoundBranchRound(HVACAcoustic.Branch.Main, _local.BranchType, _main_in.Diameter / 1000.0,
                        _main_out.Diameter / 1000.0, _local.Diameter / 1000.0);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_main_in.DuctType == DuctType.Rectangular && _local.DuctType == DuctType.Rectangular)
            {
                if (_local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
                                   _main_in.Width / 1000.0 * _main_in.Height / 1000.0, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
               _main_in.Width / 1000.0 * _main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Rectangular && _local.DuctType == DuctType.Round)
            {
                if (_local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   _main_in.Width / 1000.0 * _main_in.Height / 1000.0, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   _main_in.Width / 1000.0 * _main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Round && _local.DuctType == DuctType.Rectangular)
            {
                if (_local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (_local.BranchType == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter/ 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Junction(HVACAcoustic.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }

    [Serializable]
    public class Plenum : ElementsBase, IChangeableDimensions<DuctConnection>
    {
        private static int _counter = 1;
        private static string _name = "pln_";
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
            _type = ElementType.Plenum;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _plenum_type = plenumType;
            _width = plenumWidth;
            _height = plenumHeight;
            _lenght = plenumLenght;
            _dl = inLocationLenght;
            _liner_check = linerCheck;
            _liner_thickness = linerThickness;
            _in = new DuctConnection(plenumIn, base.AirFlow, widthIn, heightIn, diameterIn);
            _out = new DuctConnection(plenumOut, base.AirFlow, widthOut, heightOut, diameterOut);
            _in.DimensionsChanged += _DimensionsChanged;
            _out.DimensionsChanged += _DimensionsChanged;
            _counter = 1;
        }

        private void _DimensionsChanged(object sender, EventArgs e)
        {
            UpdateLenght();
            UpdateWidth();
            UpdateHeight();
            UpdateDistance();
        }

        /// <summary>Skrzynka tłumiąca.</summary>
        public Plenum()
        {
            _type = ElementType.Plenum;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            this.AirFlow = 800;
            this.IsIncluded = true;
            _plenum_type = PlenumType.VerticalConnection;
            _width = 400;
            _height = 250;
            _lenght = 500;
            _dl = 300;
            _liner_check = false;
            _liner_thickness = 25;
            _in = new DuctConnection(DuctType.Round, base.AirFlow, 200, 160, 160);
            _out = new DuctConnection(DuctType.Rectangular, base.AirFlow, 400, 250, 250);
            _in.DimensionsChanged += _DimensionsChanged;
            _out.DimensionsChanged += _DimensionsChanged;
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
                if (_in.DuctType == DuctType.Rectangular) { temp_in = Math.Min(_in.Height, _in.Width); }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = Math.Min(_out.Height, _out.Width); }
                else { temp_out = _out.Diameter; }

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
                UpdateLenght();
                UpdateHeight();
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
                UpdateLenght();
                UpdateWidth();
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
                    if (_in.DuctType == DuctType.Rectangular) { temp_in = Math.Min(_in.Height,_in.Width); }
                    else { temp_in = _in.Diameter; }

                    if (_out.DuctType == DuctType.Rectangular) { temp_out = Math.Min(_out.Height,_out.Width); }
                    else { temp_out = _out.Diameter; }

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
                UpdateLenght();
                UpdateHeight();
                UpdateWidth();
            }
        }
    }

    [Serializable]
    public class Damper : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "dmp_";
        private int _diameter;
        private int _width;
        private int _height;
        private byte _blade_number;
        private byte _blade_angle;
        private DuctType _duct_type;
        private DamperType _damper_type;

        /// <summary>Przepustnica.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductType">Typ króćca podłączeniowego.</param>
        /// <param name="damperType">Typ przepustnicy.</param>
        /// <param name="airFlow">Przepływ powietrza przez przepustnicę [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="bladeAngle">Kąt nachylenia łopatek przepustnicy.</param>
        /// <param name="bladeNumber">Liczba łopatek.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Damper(string name, string comments, DamperType damperType, DuctType ductType, int airFlow, int width, int height,
             int diameter, byte bladeNumber, byte bladeAngle, bool include)
        {
            _type = ElementType.Damper;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _damper_type = damperType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _duct_type = ductType;
            _blade_number = bladeNumber;
            _blade_angle = bladeAngle;
            _counter = 1;
        }

        /// <summary>Przepustnica.</summary>
        public Damper()
        {
            _type = ElementType.Damper;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            this.AirFlow = 800;
            this.IsIncluded = true;
            _damper_type = DamperType.SingleBlade;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _duct_type = DuctType.Rectangular;
            _blade_number = 1;
            _blade_angle = 0;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = { 0, 0, 0, 0, 0, 0, 0, 0 };
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
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
                    return (this.AirFlow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
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

        public byte BladeNumber
        {
            get
            {
                return _blade_number;
            }
            set
            {
                if (_damper_type == DamperType.SingleBlade || value <= 1)
                {
                    _blade_number = 1;
                    _damper_type = DamperType.SingleBlade;
                    this.BladeAngle = _blade_angle;
                }
                else if (value < (byte)Math.Ceiling(_width / 20.0))
                {
                    _blade_number = value;
                }
                else
                {
                    _blade_number = (byte)Math.Ceiling(_width / 20.0);
                }
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
                byte temp;
                if (_damper_type == DamperType.SingleBlade) { temp = 70; }
                else { temp = 80; }

                if (value < 0)
                {
                    _blade_angle = 0;
                }
                else if (value < temp)
                {
                    _blade_angle = temp;
                }
                else
                {
                    _blade_angle = temp;
                }
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

                if (_damper_type == DamperType.SingleBlade) { _blade_number = 1; }
                this.BladeAngle = _blade_angle;
            }
        }
    }

    [Serializable]
    public class Grill: ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "grill_";
        private int _diameter;
        private int _width;
        private int _height;
        private int _eff_area;
        private GrillType _grill_type;
        private GrillLocation _grill_location;
        private GrillOrifice _local = null;

        /// <summary>Kratka wentylacyjna nawiewna/wyciągowa.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="grillType">Typ kratki wentylacyjnej.</param>
        /// <param name="grillLocation">Lokalizacja kratki wentylacyjnej.</param>
        /// <param name="airFlow">Przepływ powietrza przez kratkę [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="orificeDepth">Głębokość otworu żaluzjowego [mm].</param>
        /// <param name="orificeHeight">Wysokość otworu żaluzjowego [mm].</param>
        /// <param name="percEffectiveArea">Procentowy udział efektywnej powierzchni netto w stosunku do całkowitej powierzchni przekroju poprzecznego kratki [%]</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Grill(string name, string comments, GrillType grillType, GrillLocation grillLocation, int airFlow, int width, int height,
             int diameter, int orificeDepth, int orificeHeight, int percEffectiveArea, bool include)
        {
            _type = ElementType.Grill;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _grill_type = grillType;
            _grill_location = grillLocation;
            _width = width;
            _height = height;
            _diameter = diameter;
            _eff_area = percEffectiveArea;
            _local = new GrillOrifice( orificeHeight, orificeDepth);
            _counter = 1;
        }

        /// <summary>Kratka wentylacyjna nawiewna/wyciągowa.</summary>
        public Grill()
        {
            _type = ElementType.Grill;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 400;
            this.IsIncluded = true;
            _grill_type = GrillType.RectangularSupplyWire;
            _grill_location = GrillLocation.FlushWall;
            _width = 250;
            _height = 150;
            _diameter = 200;
            _eff_area = 70;
            _local = new GrillOrifice(20, 20);
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];
            DuctType duct_type;

            if ((Convert.ToInt16(_grill_type) <= 3) || ((Convert.ToInt16(_grill_type) >= 8) && (Convert.ToInt16(_grill_type) <= 11)))
            {
                duct_type = DuctType.Round;
            }
            else
            {
                duct_type = DuctType.Rectangular;
            }

            if (duct_type==DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.Grill(_grill_location, _width/1000.0 * _height/1000.0);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.Grill(_grill_location, Math.PI * 0.25 * Math.Pow(_diameter/1000.0, 2));
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];
            DuctType duct_type;

            if ((Convert.ToInt16(_grill_type) <= 3) || ((Convert.ToInt16(_grill_type) >= 8) && (Convert.ToInt16(_grill_type) <= 11)))
            {
                duct_type = DuctType.Round;
            }
            else
            {
                duct_type = DuctType.Rectangular;
            }

            if (duct_type==DuctType.Rectangular)
            {
                lw = HVACAcoustic.Noise.Grill(_grill_type, base.AirFlow, _width / 1000.0 * _height / 1000.0, _local.Depth / 10.0,
                    _width / 1000.0, _local.Height / 10.0, _eff_area);
            }
            else
            {
                lw = HVACAcoustic.Noise.Grill(_grill_type, base.AirFlow, Math.PI * 0.25 * Math.Pow(_diameter/1000.0, 2), _local.Depth / 10.0,
                    _width / 1000.0, _local.Height / 10.0, _eff_area);
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
                DuctType duct_type;

                if ((Convert.ToInt16(_grill_type) <= 3) || ((Convert.ToInt16(_grill_type) >= 8) && (Convert.ToInt16(_grill_type) <= 11)))
                {
                    duct_type = DuctType.Round;
                }
                else
                {
                    duct_type = DuctType.Rectangular;
                }

                if (duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
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
                return _eff_area;
            }
            set
            {
                if (value < 10)
                {
                    _eff_area = 10;
                }
                else if (value < 100)
                {
                    _eff_area = value;
                }
                else
                {
                    _eff_area = 100;
                }
            }
        }

        public GrillType GrillType
        {
            get
            {
                return _grill_type;
            }
            set
            {
                _grill_type = value;
            }
        }

        public GrillLocation GrillLocation
        {
            get
            {
                return _grill_location;
            }
            set
            {
                _grill_location = value;
            }
        }

        public GrillOrifice Orifice
        {
            get
            {
                return _local;
            }
        }
    }

    [Serializable]
    public class Fan: ElementsBase
    {
        private static int _counter = 1;
        private static string _name = "fan_";
        private FanType _fanType;
        public NoiseEmission NoiseEmission { get; set; }
        public WorkArea WorkArea { get; set; }
        private int _pressure_drop;
        private byte _efficient;
        private byte _blade_number;
        private int _rpm;

        /// <summary>Wentylator.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="fanType">Typ wentylatora.</param>
        /// <param name="airFlow">Wydajność wentylatora w analizowanym punkcie pracy [m3/h].</param>
        /// <param name="pressureDrop">Spręż całkowity wentylatora w analizowanym punkcie pracy [Pa].</param>
        /// <param name="rpm">Prędkość obrotowa wirnika dla założonej wydajności i sprężu [rpm].</param>
        /// <param name="bladeNumber">Liczba łopatek.</param>
        /// <param name="efficientDeviation">Względne odchylenie od punktu sprawności szczytowej [%].</param>
        /// <param name="workArea">Obszar pracy.</param>
        /// <param name="noiseEmissionDirection">Kierunek emisji hałasu.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Fan(string name, string comments, FanType fanType, int airFlow, int pressureDrop, int rpm, byte bladeNumber, byte efficientDeviation,
             WorkArea workArea, NoiseEmission noiseEmissionDirection, bool include)
        {
            _type = ElementType.Fan;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _fanType = fanType;
            _pressure_drop = pressureDrop;
            _rpm = rpm;
            _blade_number = bladeNumber;
            _efficient = efficientDeviation;
            this.NoiseEmission = noiseEmissionDirection;
            this.WorkArea = workArea;
            _counter = 1;
        }

        /// <summary>Wentylator.</summary>
        public Fan()
        {
            _type = ElementType.Fan;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 5000;
            this.IsIncluded = true;
            _fanType = FanType.CentrifugalBackwardCurved;
            _pressure_drop = 250;
            _rpm = 1500;
            _blade_number = 12;
            _efficient = 0;
            this.NoiseEmission = NoiseEmission.OneDirection;
            this.WorkArea = WorkArea.MaximumEfficiencyArea;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = { 0, 0, 0, 0, 0, 0, 0, 0 };
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
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

            return HVACAcoustic.Noise.Fan(_fanType, base.AirFlow, _pressure_drop, _rpm, _blade_number, _efficient, loc);
        }

        public FanType FanType
        {
            get
            {
                return _fanType;
            }
            set
            {
                _fanType = value;
                this.BladeNumber = _blade_number;
            }
        }

        public int PressureDrop
        {
            get
            {
                return _pressure_drop;
            }
            set
            {
                if (value < 10)
                {
                    _pressure_drop = 10;
                }
                else if (value < 9999)
                {
                    _pressure_drop = value;
                }
                else
                {
                    _pressure_drop = 9999;
                }
            }
        }

        public byte Efficient
        {
            get
            {
                return _efficient;
            }
            set
            {
                if (value < 0)
                {
                    _efficient = 0;
                }
                else if (value < 50)
                {
                    _efficient = value;
                }
                else
                {
                    _efficient = 50;
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
                byte max_temp, min_temp;

                switch (_fanType)
                {
                    case FanType.CentrifugalBackwardCurved:
                        min_temp = 10;
                        max_temp = 16;
                        break;
                    case FanType.CentrifugalRadial:
                        min_temp = 6;
                        max_temp = 10;
                        break;
                    case FanType.CentrifugalForwardCurved:
                        min_temp = 24;
                        max_temp = 64;
                        break;
                    case FanType.VaneAxial:
                        min_temp = 3;
                        max_temp = 16;
                        break;
                    case FanType.TubeAxial:
                        min_temp = 4;
                        max_temp = 8;
                        break;
                    case FanType.Propeller:
                        min_temp = 2;
                        max_temp = 8;
                        break;
                    default:
                        min_temp = 10;
                        max_temp = 16;
                        break;
                }

                if (value < min_temp)
                {
                    _blade_number = min_temp;
                }
                else if (value < max_temp)
                {
                    _blade_number = value;
                }
                else
                {
                    _blade_number = max_temp;
                }
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
                if (value < 150)
                {
                    _rpm = value;
                }
                else if (value < 3000)
                {
                    _rpm = value;
                }
                else
                {
                    _rpm = 3000;
                }
            }
        }
    }

    [Serializable]
    public class DoubleJunction : ElementsBase, IChangeableDimensions<DoubleJunctionMain>, IDoubleBranchingElement<DoubleJunctionBranch>
    {
        private static int _counter = 1;
        private static string _name = "djnt_";
        private readonly DoubleJunctionBranch _local_right = null;
        private readonly DoubleJunctionBranch _local_left = null;
        private DoubleJunctionContaier _container = null;
        private readonly DoubleJunctionMain _main_in = null;
        private readonly DoubleJunctionMain _main_out = null;

        /// <summary>Czwórnik.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductTypeMainIn">Typ głównego króćca podłączeniowego od strony wlotowej.</param>
        /// <param name="ductTypeBranch">Typ króćca odgałęźnego.</param>
        /// <param name="airFlowMainIn">Przepływ powietrza na wlocie do elementu [m3/h].</param>
        /// <param name="airFlowBranchRight">Przepływ powietrza przez odgałęzienie prawe [m3/h].</param>
        /// <param name="airFlowBranchLeft">Przepływ powietrza przez odgałęzienie lewe [m3/h].</param>
        /// <param name="branchTypeRight">Typ odgałęzienia prawego.</param>
        /// <param name="branchTypeLeft">Typ odgałęzienia lewego.</param>
        /// <param name="widthMainIn">Szerokość głównego króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="heightMainIn">Wysokość głównego króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="diameterMainIn">Średnica głównego króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="widthMainOut">Szerokość głównego króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="heightMainOut">Wysokość głównego króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="diameterMainOut">Średnica głównego króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="widthBranchRight">Szerokość króćca odgałęźnego prawego [mm].</param>
        /// <param name="heightBranchRight">Wysokość króćca odgałęźnego prawego [mm].</param>
        /// <param name="diameterBranchRight">Średnica króćca odgałęźnego prawego [mm].</param>
        /// <param name="roundingRight">Promień zaokrąglenia odgałęzienia prawego [mm].</param>
        /// <param name="widthBranchLeft">Szerokość króćca odgałęźnego lewego [mm].</param>
        /// <param name="heightBranchLeft">Wysokość króćca odgałęźnego lewego [mm].</param>
        /// <param name="diameterBranchLeft">Średnica króćca odgałęźnego lewego [mm].</param>
        /// <param name="roundingLeft">Promień zaokrąglenia odgałęzienia lewego [mm].</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public DoubleJunction(string name, string comments, DuctType ductTypeMainIn, int airFlowMainIn, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft, bool include)
        {
            _type = ElementType.DoubleJunction;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowMainIn;
            this.IsIncluded = include;
            _name = this.Name;
            _counter = 1;

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

            _container = new DoubleJunctionContaier(ductTypeMainIn, airFlowMainIn, widthMainIn, widthMainOut, heightMainIn, heightMainOut,
                diameterMainIn, diameterMainOut, ductTypeBranch, branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight,
                diameterBranchRight, roundingRight, branchTypeLeft, airFlowBranchLeft, widthBranchLeft, heightBranchLeft, diameterBranchLeft,
                roundingLeft);
            _local_right = new DoubleJunctionBranch(_container, Branch.BranchRight);
            _local_left = new DoubleJunctionBranch(_container, Branch.BranchLeft);
            _main_in = new DoubleJunctionMain(this, JunctionConnectionSide.Inlet);
            _main_out = new DoubleJunctionMain(this, JunctionConnectionSide.Outlet);
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }

        /// <summary>Czwórnik.</summary>
        public DoubleJunction()
        {
            _type = ElementType.DoubleJunction;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            base.AirFlow = 2600;
            this.IsIncluded = true;

            _container = new DoubleJunctionContaier(DuctType.Rectangular, base.AirFlow, 400, 200, 400, 200, 450, 250,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0, BranchType.Straight, 400, 160, 160, 200, 0);
            _local_right = new DoubleJunctionBranch(_container, Branch.BranchRight);
            _local_left = new DoubleJunctionBranch(_container, Branch.BranchLeft);
            _main_in = new DoubleJunctionMain(this, JunctionConnectionSide.Inlet);
            _main_out = new DoubleJunctionMain(this, JunctionConnectionSide.Outlet);
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }

        public DoubleJunctionBranch BranchRight
        {
            get
            {
                return _local_right;
            }
        }

        public DoubleJunctionBranch BranchLeft
        {
            get
            {
                return _local_left;
            }
        }

        public DoubleJunctionMain Inlet
        {
            get
            {
                return _main_in;
            }
        }

        public DoubleJunctionMain Outlet
        {
            get
            {
                return _main_out;
            }
        }

        internal DoubleJunctionContaier Container
        {
            get
            {
                return _container;
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
                _container.In.AirFlow = value;

                if ((_container.AirFlowBranchRight + _container.AirFlowBranchLeft) >= value)
                {
                    double temp = Convert.ToDouble(_container.AirFlowBranchRight) / Convert.ToDouble(_container.AirFlowBranchRight + _container.AirFlowBranchLeft);
                    _container.AirFlowBranchRight = (int)Math.Round(temp * value);
                    _container.AirFlowBranchLeft = (int)Math.Round((1 - temp) * value);
                }

                _container.Out.AirFlow = value - _container.AirFlowBranchRight - _container.AirFlowBranchLeft;
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.Main, _container.BranchTypeRight,
                    _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height, _container.WidthBranchRight, _container.HeightBranchRight,
                    _container.WidthBranchLeft, _container.HeightBranchLeft);
            }
            else if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Round)
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.Main, _container.BranchTypeRight,
                    _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height,
                    _container.DiameterBranchRight, _container.DiameterBranchLeft);
            }
            else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.Main, _container.BranchTypeRight,
                    _container.In.Diameter, _container.Out.Diameter, _container.WidthBranchRight, _container.HeightBranchRight,
                    _container.WidthBranchLeft, _container.HeightBranchLeft);
            }
            else
            {
                attn = HVACAcoustic.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.Main, _container.BranchTypeRight,
                    _container.In.Diameter, _container.Out.Diameter, _container.DiameterBranchRight, _container.DiameterBranchLeft);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Rectangular)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Round)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }

    [Serializable]
    public class TJunction : ElementsBase, IDoubleBranchingElement<TJunctionBranch>
    {
        private static int _counter = 1;
        private static string _name = "tjnt_";
        private readonly TJunctionBranch _local_right = null;
        private readonly TJunctionBranch _local_left = null;
        private TJunctionContaier _container = null;

        /// <summary>Trójnik typu T.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductTypeMainIn">Typ głównego króćca podłączeniowego.</param>
        /// <param name="ductTypeBranch">Typ króćca odgałęźnego.</param>
        /// <param name="airFlowBranchRight">Przepływ powietrza przez odgałęzienie prawe [m3/h].</param>
        /// <param name="airFlowBranchLeft">Przepływ powietrza przez odgałęzienie lewe [m3/h].</param>
        /// <param name="branchTypeRight">Typ odgałęzienia prawego.</param>
        /// <param name="branchTypeLeft">Typ odgałęzienia lewego.</param>
        /// <param name="widthMainIn">Szerokość głównego króćca podłączeniowego [mm].</param>
        /// <param name="heightMainIn">Wysokość głównego króćca podłączeniowego [mm].</param>
        /// <param name="diameterMainIn">Średnica głównego króćca podłączeniowego [mm].</param>
        /// <param name="widthBranchRight">Szerokość króćca odgałęźnego prawego [mm].</param>
        /// <param name="heightBranchRight">Wysokość króćca odgałęźnego prawego [mm].</param>
        /// <param name="diameterBranchRight">Średnica króćca odgałęźnego prawego [mm].</param>
        /// <param name="roundingRight">Promień zaokrąglenia odgałęzienia prawego [mm].</param>
        /// <param name="widthBranchLeft">Szerokość króćca odgałęźnego lewego [mm].</param>
        /// <param name="heightBranchLeft">Wysokość króćca odgałęźnego lewego [mm].</param>
        /// <param name="diameterBranchLeft">Średnica króćca odgałęźnego lewego [mm].</param>
        /// <param name="roundingLeft">Promień zaokrąglenia odgałęzienia lewego [mm].</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public TJunction(string name, string comments, DuctType ductTypeMainIn, int widthMainIn, int heightMainIn,
            int diameterMainIn, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft, bool include)
        {
            _type = ElementType.TJunction;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowBranchLeft + airFlowBranchRight;
            this.IsIncluded = include;
            _name = this.Name;
            _counter = 1;
           
            _container = new TJunctionContaier(this, ductTypeMainIn, widthMainIn, heightMainIn, diameterMainIn, ductTypeBranch, 
                branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight, diameterBranchRight, 
                roundingRight, branchTypeLeft, airFlowBranchLeft, widthBranchLeft, heightBranchLeft, diameterBranchLeft,
                roundingLeft);
            _local_right = new TJunctionBranch(_container, Branch.BranchRight);
            _local_left = new TJunctionBranch(_container, Branch.BranchLeft);
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }


        /// <summary>Trójnik typu T.</summary>
        public TJunction()
        {
            _type = ElementType.TJunction;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.IsIncluded = true;

            _container = new TJunctionContaier(this, DuctType.Rectangular, 400, 200, 450, DuctType.Rectangular,
                BranchType.Straight, 600, 160, 160, 200, 0, BranchType.Straight, 800, 160, 160, 200, 0);
            _local_right = new TJunctionBranch(_container, Branch.BranchRight);
            _local_left = new TJunctionBranch(_container, Branch.BranchLeft);
            base.AirFlow = 1400;
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }

        public TJunctionBranch BranchRight
        {
            get
            {
                return _local_right;
            }
        }

        public TJunctionBranch BranchLeft
        {
            get
            {
                return _local_left;
            }
        }

        internal TJunctionContaier Container
        {
            get
            {
                return _container;
            }
        }

        public int Width
        {
            get
            {
                return _container.WidthMain;
            }
            set
            {
                if (value < 100)
                {
                    _container.WidthMain = 100;
                }
                else if (value < 2000)
                {
                    _container.WidthMain = value;
                }
                else
                {
                    _container.WidthMain = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return _container.HeightMain;
            }
            set
            {
                if (value < 100)
                {
                    _container.HeightMain = 100;
                }
                else if (value < 2000)
                {
                    _container.HeightMain = value;
                }
                else
                {
                    _container.HeightMain = 2000;
                }
            }
        }

        public int Diameter
        {
            get
            {
                return _container.DiameterMain;
            }
            set
            {
                if (value < 80)
                {
                    _container.DiameterMain = 80;
                }
                else if (value < 1600)
                {
                    _container.DiameterMain = value;
                }
                else
                {
                    _container.DiameterMain = 1600;
                }
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
                double temp = Convert.ToDouble(_container.AirFlowBranchRight) / Convert.ToDouble(_container.AirFlowBranchRight + _container.AirFlowBranchLeft);
                _container.AirFlowBranchRight = (int)Math.Round(temp * value);
                _container.AirFlowBranchLeft = (int)Math.Round((1 - temp) * value);
            }
        }

        public double Velocity
        {
            get
            {
                if (_container.DuctTypeMain == DuctType.Rectangular)
                {
                    return (this.AirFlow / 3600.0) / ((_container.WidthMain / 1000.0) * (_container.HeightMain / 1000.0));
                }
                else
                {
                    return (this.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_container.DiameterMain / 1000.0, 2));
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _container.DuctTypeMain;
            }
            set
            {
                _container.DuctTypeMain = value;
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = { 0, 0, 0, 0, 0, 0, 0, 0 };
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Rectangular)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,  _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0,
                        _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0, _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0, _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0,
                        _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0, _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Round)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = HVACAcoustic.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }

    [Serializable]
    public class Silencer : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "sln_";
        private SoundAttenuation _local = null;
        private int _width;
        private int _height;
        private int _diameter;
        private int _eff_area;
        private DuctType _duct_type;
        private SilencerType _silencer_type;
        private double _lenght;

        /// <summary>Tłumik akstyczny.</summary>
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
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Silencer(string name, string comments, SilencerType silencerType, DuctType ductType, int airFlow, int width, int height,
             int diameter, double lenght, int octaveBand63Hz, int octaveBand125Hz, int octaveBand250Hz, int octaveBand500Hz,
             int octaveBand1000Hz, int octaveBand2000Hz, int octaveBand4000Hz, int octaveBand8000Hz, int percEffectiveArea,
             bool include)
        {
            _type = ElementType.Silencer;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _silencer_type = silencerType;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _lenght = lenght;
            _eff_area = percEffectiveArea;
            _local = new SoundAttenuation(octaveBand63Hz, octaveBand125Hz, octaveBand250Hz, octaveBand500Hz,
             octaveBand1000Hz, octaveBand2000Hz, octaveBand4000Hz, octaveBand8000Hz);
            _counter = 1;
        }

        /// <summary>Tłumik akstyczny.</summary>
        public Silencer()
        {
            _type = ElementType.Silencer;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 500;
            this.IsIncluded = true;
            _silencer_type = SilencerType.Absorptive;
            _duct_type = DuctType.Round;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _lenght = 0.6;
            _eff_area = 100;
            _local = new SoundAttenuation(1, 2, 5, 9, 16, 13, 5, 6);
            _name = this.Name;
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
                    return (this.AirFlow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0) * _eff_area/100.0);
                }
                else
                {
                    return (this.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2) * _eff_area / 100.0);
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
                return _eff_area;
            }
            set
            {
                if (value < 10)
                {
                    _eff_area = 10;
                }
                else if (value < 100)
                {
                    _eff_area = value;
                }
                else
                {
                    _eff_area = 100;
                }
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

        public SilencerType SilencerType
        {
            get
            {
                return _silencer_type;
            }
            set
            {
                _silencer_type = value;
            }
        }

        public SoundAttenuation OctaveBandAttenuation
        {
            get
            {
                return _local;
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = { _local.OctaveBand63Hz, _local.OctaveBand125Hz, _local.OctaveBand250Hz, _local.OctaveBand500Hz, _local.OctaveBand1000Hz,
                _local.OctaveBand2000Hz, _local.OctaveBand4000Hz, _local.OctaveBand8000Hz };
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                lw = HVACAcoustic.Noise.Silencer(base.AirFlow, _height / 1000.0 * _width / 1000.0, _eff_area);
            }
            else
            {
                if (_silencer_type == SilencerType.ParallelBaffles)
                {
                    lw = HVACAcoustic.Noise.Silencer(base.AirFlow, 0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2), _eff_area);
                }
                else
                {
                    lw = HVACAcoustic.Noise.Silencer(base.AirFlow, 0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2), 100);
                }
            }
            return lw;
        }
    }

    [Serializable]
    public class Room : ElementsBase
    {
        private static int _counter = 1;
        private static string _name = "room_";
        private RoomConstant _local = null;
        private double _width;
        private double _height;
        private double _lenght;
        private NoiseLocation _noiseLocation;
        private double _distance;
        private int _temperature;
        private byte _rh;
        private byte _directionFactor;

        /// <summary>Pomieszczenie.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="temperature">Temperatura powietrza w pomieszczeniu [deg C].</param>
        /// <param name="relativeHumidity">Wilgotność względna powietrza w pomieszczeniu [%].</param>
        /// <param name="noiseLocation">Lokalizacja źródła hałasu</param>
        /// <param name="distance">Odległość między źródłem dźwięku a słuchaczem [m]</param>
        /// <param name="airFlow">Ilość powietrza nawiewana o pomieszczenia [m3/h].</param>
        /// <param name="width">Szerokość pomieszczenia [m].</param>
        /// <param name="height">Wysokość pomieszczenia [m].</param>
        /// <param name="lenght">Długość pomieszczenia [m].</param>
        /// <param name="octaveBand63Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 63Hz.</param>
        /// <param name="octaveBand125Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 125Hz.</param>
        /// <param name="octaveBand250Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 250Hz.</param>
        /// <param name="octaveBand500Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 500Hz.</param>
        /// <param name="octaveBand1000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 1000Hz.</param>
        /// <param name="octaveBand2000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 2000Hz.</param>
        /// <param name="octaveBand4000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 4000Hz.</param>
        /// <param name="octaveBand8000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 8000Hz.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Room(string name, string comments, int temperature, byte relativeHumidity, double distance, NoiseLocation noiseLocation, double width,
            double height, double lenght, double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz, bool include)
        {
            _type = ElementType.Room;
            this.Comments = comments;
            this.Name = name;
            base.IsIncluded = include;
            _temperature = temperature;
            _rh = relativeHumidity;
            _distance = distance;
            _noiseLocation = noiseLocation;
            _width = width;
            _height = height;
            _lenght = lenght;
            _local = new RoomConstant(this, octaveBand63Hz, octaveBand125Hz, octaveBand250Hz, octaveBand500Hz,
                octaveBand1000Hz, octaveBand2000Hz, octaveBand4000Hz, octaveBand8000Hz);
            _counter = 1;
        }

        /// <summary>Pomieszczenie.</summary>
        public Room()
        {
            _type = ElementType.Room;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            base.IsIncluded = true;
            _temperature = 20;
            _rh = 40;
            _distance = 3;
            _noiseLocation = NoiseLocation.SurfaceCenter;
            _width = 10;
            _height = 3;
            _lenght = 12;
            _local = new RoomConstant(this, Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[0], Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[1],
                Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[2], Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[3],
                Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[4], Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[5],
                Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[6],Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[7]);
        }

        private void UpdateOctaveBandAbsorption()
        {
            this.OctaveBandAbsorption.OctaveBand63Hz = _local.OctaveBand63Hz;
            this.OctaveBandAbsorption.OctaveBand125Hz = _local.OctaveBand125Hz;
            this.OctaveBandAbsorption.OctaveBand250Hz = _local.OctaveBand250Hz;
            this.OctaveBandAbsorption.OctaveBand500Hz = _local.OctaveBand500Hz;
            this.OctaveBandAbsorption.OctaveBand1000Hz = _local.OctaveBand1000Hz;
            this.OctaveBandAbsorption.OctaveBand2000Hz = _local.OctaveBand2000Hz;
            this.OctaveBandAbsorption.OctaveBand4000Hz = _local.OctaveBand4000Hz;
            this.OctaveBandAbsorption.OctaveBand8000Hz = _local.OctaveBand8000Hz;
        }

        public new bool IsIncluded
        {
            get
            {
                return base.IsIncluded;
            }
            set
            {
                base.IsIncluded = value;
            }
        }
         
        public int Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                if (value < -20)
                {
                    _temperature = -20;
                }
                else if (value < 50)
                {
                    _temperature = value;
                }
                else
                {
                    _temperature = 50;
                }
            }
        }

        public byte RelativeHumidity
        {
            get
            {
                return _rh;
            }
            set
            {
                if (value < 0)
                {
                    _rh = 0;
                }
                else if (value < 100)
                {
                    _rh = value;
                }
                else
                {
                    _rh = 100;
                }
            }
        }

        public new int AirFlow
        {
            get
            {
                return base.AirFlow;
            }
            internal set
            {
                base.AirFlow = value;
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 1)
                {
                    _width = 1;
                }
                else if (value < 100)
                {
                    _width = Math.Round(value, 2);
                }
                else
                {
                    _width = 100;
                }
                this.Distance = _distance;
                UpdateOctaveBandAbsorption();
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 1)
                {
                    _height = 1;
                }
                else if (value < 100)
                {
                    _height = Math.Round(value, 2);
                }
                else
                {
                    _height = 100;
                }
                this.Distance = _distance;
                UpdateOctaveBandAbsorption();
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
                if (value < 1)
                {
                    _lenght = 1;
                }
                else if (value < 100)
                {
                    _lenght = Math.Round(value, 2);
                }
                else
                {
                    _lenght = 100;
                }
                this.Distance = _distance;
                UpdateOctaveBandAbsorption();
            }
        }

        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                double[] temp = new double[] { _width, _height, _lenght };

                if (value < 0.1) { _distance = 0.1; }
                else if (value < temp.Min()) { _distance = value; }
                else { _distance = Math.Round(temp.Min(), 2); }
            }
        }

        public NoiseLocation NoiseLocation
        {
            get
            {
                return _noiseLocation;
            }
            set
            {
                _noiseLocation = value;

                switch (_noiseLocation)
                {
                    case NoiseLocation.RoomCenter:
                        _directionFactor = 1;
                        break;
                    case NoiseLocation.SurfaceCenter:
                        _directionFactor = 2;
                        break;
                    case NoiseLocation.SurfaceCorner:
                        _directionFactor = 8;
                        break;
                    case NoiseLocation.SurfaceEdge:
                        _directionFactor = 4;
                        break;
                }
            }
        }

        public RoomConstant OctaveBandAbsorption
        {
            get
            {
                return _local;
            }
        }

        /// <summary>Oblicz tłumienność pomieszczenia.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (this.IsIncluded == true)
            {
                attn = Transmission.PointCorrection(_temperature, _rh, _directionFactor, _distance, new double[] { _local.OctaveBand63Hz, _local.OctaveBand125Hz, _local.OctaveBand250Hz,
            _local.OctaveBand500Hz, _local.OctaveBand1000Hz, _local.OctaveBand2000Hz, _local.OctaveBand4000Hz, _local.OctaveBand8000Hz}, _width, _lenght, _height);
            }
            else
            {
                for (int i = 0; i < attn.Length; i++)
                {
                    attn[i] = -1000;
                }
            }

            return attn;
        }

        /// <summary>Oblicz szum generowany przez pomieszczenie.</summary>
        public override double[] Noise()
        {
            double[] lw = { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            return lw;
        }
    }

    [Serializable]
    public class ElementsCollection: IEnumerable<ElementsBase>
    {
        private List<ElementsBase> _children;
        internal ElementsBase _parent;

        /// <summary>Zwróć element o podanym indeksie.</summary>
        /// <param name="index">Numer elementu.</param>
        public ElementsBase this[int index]
        {
            get
            {
                return _children[index];
            }
        }

        public ElementsCollection()
        {
            _children = new List<ElementsBase>();
        }

        public ElementsCollection(ElementsBase element)
        {
            ElementsBase loc = (ElementsBase)element.Clone();
            loc.Parent = _parent;
            _children = new List<ElementsBase>() { loc };
        }

        public ElementsCollection(params ElementsBase[] element)
        {
            _children = new List<ElementsBase>();
            ElementsBase loc = null;

            foreach (ElementsBase eb in element)
            {
                loc = (ElementsBase)eb.Clone();
                loc.Parent = _parent;
                _children.Add(loc);
            }
        }

        /// <summary>Dodaj nowy element.</summary>
        /// <param name="element">Element do dodania.</param>
        public void Add(ElementsBase element)
        {
            ElementsBase loc = (ElementsBase)element.Clone();
            loc.Parent = _parent;
            _children.Add(loc);
        }

        /// <summary>Dodaj nową grupę elementów.</summary>
        /// <param name="element">Tablica elementów do dodania.</param>
        public void AddRange(params ElementsBase[] element)
        {
            ElementsBase loc = null;

            foreach (ElementsBase eb in element)
            {
                loc = (ElementsBase)eb.Clone();
                loc.Parent = _parent;
                _children.Add(loc);
            }
        }

        /// <summary>Usuń element.</summary>
        /// <param name="elementName">Nazwa elementu.</param>
        public void RemoveByName(string elementName)
        {
            ElementsBase element = Find(elementName);
            _children.Remove(element);
        }

        /// <summary>Usuń element o podanym indeksie.</summary>
        /// <param name="index">Numer elementu.</param>
        public void RemoveAt(int index)
        {
            _children.RemoveAt(index);
        }

        /// <summary>Oblicz poziom ciśnienia akustycznego.</summary>
        public Dictionary<Room, double[]> ComputeSoundPressureLevel()
        {
            double[] loc_result = new double[8] { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            Dictionary<Room, double[]> overall_result = new Dictionary<Room, double[]>();
            List<List<ElementsBase>> local = ElementsLists();

            foreach (List<ElementsBase> list in local)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    if (list[i-1].Parent == list[i])
                    {
                        if (list[i] is Junction)
                        {
                            loc_result = MathOperations.OctaveSubstract(loc_result, ((Junction)list[i]).Branch.Attenuation());
                            loc_result = MathOperations.OctaveDecibelAdd(loc_result, ((Junction)list[i]).Branch.Noise());
                        }
                        else if (list[i] is DoubleJunction)
                        {
                            if (((DoubleJunction)list[i]).BranchRight.Elements.Contains(list[i - 1]))
                            {
                                loc_result = MathOperations.OctaveSubstract(loc_result, ((DoubleJunction)list[i]).BranchRight.Attenuation());
                                loc_result = MathOperations.OctaveDecibelAdd(loc_result, ((DoubleJunction)list[i]).BranchRight.Noise());
                            }
                            else
                            {
                                loc_result = MathOperations.OctaveSubstract(loc_result, ((DoubleJunction)list[i]).BranchLeft.Attenuation());
                                loc_result = MathOperations.OctaveDecibelAdd(loc_result, ((DoubleJunction)list[i]).BranchLeft.Noise());
                            }
                        }
                        else if (list[i] is TJunction)
                        {
                            if (((TJunction)list[i]).BranchRight.Elements.Contains(list[i - 1]))
                            {
                                loc_result = MathOperations.OctaveSubstract(loc_result, ((TJunction)list[i]).BranchRight.Attenuation());
                                loc_result = MathOperations.OctaveDecibelAdd(loc_result, ((TJunction)list[i]).BranchRight.Noise());
                            }
                            else
                            {
                                loc_result = MathOperations.OctaveSubstract(loc_result, ((TJunction)list[i]).BranchLeft.Attenuation());
                                loc_result = MathOperations.OctaveDecibelAdd(loc_result, ((TJunction)list[i]).BranchLeft.Noise());
                            }
                        }
                    }
                    else
                    {
                        loc_result = MathOperations.OctaveSubstract(loc_result, list[i].Attenuation());
                        loc_result = MathOperations.OctaveDecibelAdd(loc_result, list[i].Noise());
                    }
                }

                loc_result = MathOperations.OctaveSubstract(loc_result, list[0].Attenuation());
                loc_result = MathOperations.OctaveDecibelAdd(loc_result, list[0].Noise());
                overall_result.Add((Room)list[0], loc_result);
            }
            return overall_result;
        }

        /// <summary>Znajdź element o podanej nazwie.</summary>
        /// <param name="elementName">Nazwa elementu.</param>
        public ElementsBase Find(string elementName)
        {
            ElementsBase result = null;
            ElementsBase loc = null;

            foreach (ElementsBase element in _children)
            {
                if (element.Name == elementName)
                {
                    result = element;
                    break;
                }
                else if (element.Type == ElementType.Junction)
                {
                    loc = ((Junction)element).Branch.Elements.Find(elementName);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                }
                else if (element.Type == ElementType.DoubleJunction)
                {
                    loc = ((DoubleJunction)element).BranchRight.Elements.Find(elementName);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((DoubleJunction)element).BranchLeft.Elements.Find(elementName);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
                else if (element.Type == ElementType.TJunction)
                {
                    loc = ((TJunction)element).BranchRight.Elements.Find(elementName);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((TJunction)element).BranchLeft.Elements.Find(elementName);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>Zwróć ciąg elementów do obiektu o podanej nazwie.</summary>
        /// <param name="elementName">Nazwa elementu.</param>
        public List<ElementsBase> ElementsRow(string elementName)
        {
            ElementsBase _temp = Find(elementName);
            List<ElementsBase> _loc = new List<ElementsBase>();
            ElementsBase _dtee = _temp;
            int _index = 0;

            if (_temp.Parent != null)
            {
                do
                {
                    #region ElementType
                    if (_temp.Parent is Junction)
                    {
                        Check(((Junction)_temp.Parent).Branch.Elements);
                        if (_dtee != null)
                        {
                            for (int j = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; j >= 0; j--)
                            {
                                if (_dtee == ((Junction)_temp.Parent).Branch.Elements[j]) { _index = j; break; }
                            }
                        }
                        else
                        {
                            if (((Junction)_temp.Parent).Branch.Elements.Last().Type == ElementType.Room) { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 2; }
                            else { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; }
                        }

                        for (int j = _index; j >= 0; j--)
                        {
                            _loc.Add(((Junction)_temp.Parent).Branch.Elements[j]);
                        }

                        _dtee = null;
                    }
                    else if (_temp.Parent is DoubleJunction)
                    {
                        if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                        {
                            Check(((DoubleJunction)_temp.Parent).BranchRight.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((DoubleJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                else { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((DoubleJunction)_temp.Parent).BranchRight.Elements[j]);
                            }
                        }
                        else
                        {
                            Check(((DoubleJunction)_temp.Parent).BranchLeft.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((DoubleJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                else { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]);
                            }
                        }

                        _dtee = _temp.Parent;
                    }
                    else if (_temp.Parent is TJunction)
                    {
                        if (((TJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                        {
                            Check(((TJunction)_temp.Parent).BranchRight.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((TJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((TJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                else { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((TJunction)_temp.Parent).BranchRight.Elements[j]);
                            }
                        }
                        else
                        {
                            Check(((TJunction)_temp.Parent).BranchLeft.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((TJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((TJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                else { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((TJunction)_temp.Parent).BranchLeft.Elements[j]);
                            }
                        }

                        _dtee = null;
                    }
                    #endregion

                    _temp = _temp.Parent;
                } while (_temp.Parent != null);

                Check(this);
                for (int j = _children.Count - 1; j >= 0; j--)
                {
                    if (_temp == _children[j]) { _index = j; break; }
                }

                for (int j = _index; j >= 0; j--)
                {
                    _loc.Add(_children[j]);
                }
            }
            else
            {
                Check(this);
                for (int j = _children.Count - 1; j >= 0; j--)
                {
                    if (_temp == _children[j]) { _index = j; break; }
                }

                for (int j = _index; j >= 0; j--)
                {
                    _loc.Add(_children[j]);
                }
            }

            _loc.Reverse();
            return _loc;
        }

        private ElementsBase FindElementType(List<ElementsBase> Exist)
        {
            ElementsBase result = null;
            ElementsBase loc = null;

            foreach (ElementsBase element in _children)
            {
                if (element.Type == ElementType.Room && Exist.Contains(element) == false)
                {
                    result = element;
                    break;
                }
                else if (element.Type == ElementType.Junction)
                {
                    loc = ((Junction)element).Branch.Elements.FindElementType(Exist);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                }
                else if (element.Type == ElementType.DoubleJunction)
                {
                    loc = ((DoubleJunction)element).BranchRight.Elements.FindElementType(Exist);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((DoubleJunction)element).BranchLeft.Elements.FindElementType(Exist);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
                else if (element.Type == ElementType.TJunction)
                {
                    loc = ((TJunction)element).BranchRight.Elements.FindElementType(Exist);

                    if (loc != null)
                    {
                        result = loc;
                        break;
                    }
                    else
                    {
                        loc = ((TJunction)element).BranchLeft.Elements.FindElementType(Exist);

                        if (loc != null)
                        {
                            result = loc;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private List<List<ElementsBase>> ElementsLists()
        {
            List<ElementsBase> _count = new List<ElementsBase>();

            do
            {
                _count.Add(FindElementType(_count));
            } while (FindElementType(_count) != null);
            
            List<List<ElementsBase>> _result = new List<List<ElementsBase>>();

            for (int i = 0; i < _count.Count; i++)
            {
                ElementsBase _temp = _count[i];
                List<ElementsBase> _loc = new List<ElementsBase>() { _count[i] };
                ElementsBase _dtee = null;

                if (_temp.Parent != null)
                {
                    int _index = 0;

                    do
                    {
                        #region ElementType
                        if (_temp.Parent is Junction)
                        {
                            Check(((Junction)_temp.Parent).Branch.Elements);
                            if (_dtee != null)
                            {
                                for (int j = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; j >= 0; j--)
                                {
                                    if (_dtee == ((Junction)_temp.Parent).Branch.Elements[j]) { _index = j; break; }
                                }
                            }
                            else
                            {
                                if (((Junction)_temp.Parent).Branch.Elements.Last().Type == ElementType.Room) { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 2; }
                                else { _index = ((Junction)_temp.Parent).Branch.Elements.Count() - 1; }
                            }

                            for (int j = _index; j >= 0; j--)
                            {
                                _loc.Add(((Junction)_temp.Parent).Branch.Elements[j]);
                            }

                            _dtee = null;
                        }
                        else if (_temp.Parent is DoubleJunction)
                        {
                            if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                            {
                                Check(((DoubleJunction)_temp.Parent).BranchRight.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((DoubleJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((DoubleJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                    else { _index = ((DoubleJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((DoubleJunction)_temp.Parent).BranchRight.Elements[j]);
                                }
                            }
                            else
                            {
                                Check(((DoubleJunction)_temp.Parent).BranchLeft.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((DoubleJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                    else { _index = ((DoubleJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((DoubleJunction)_temp.Parent).BranchLeft.Elements[j]);
                                }
                            }

                            _dtee = _temp.Parent;
                        }
                        else if (_temp.Parent is TJunction)
                        {
                            if (((TJunction)_temp.Parent).BranchRight.Elements.Contains(_temp) == true)
                            {
                                Check(((TJunction)_temp.Parent).BranchRight.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((TJunction)_temp.Parent).BranchRight.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((TJunction)_temp.Parent).BranchRight.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 2; }
                                    else { _index = ((TJunction)_temp.Parent).BranchRight.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((TJunction)_temp.Parent).BranchRight.Elements[j]);
                                }
                            }
                            else
                            {
                                Check(((TJunction)_temp.Parent).BranchLeft.Elements);
                                if (_dtee != null)
                                {
                                    for (int j = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; j >= 0; j--)
                                    {
                                        if (_dtee == ((TJunction)_temp.Parent).BranchLeft.Elements[j]) { _index = j; break; }
                                    }
                                }
                                else
                                {
                                    if (((TJunction)_temp.Parent).BranchLeft.Elements.Last().Type == ElementType.Room) { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 2; }
                                    else { _index = ((TJunction)_temp.Parent).BranchLeft.Elements.Count() - 1; }
                                }

                                for (int j = _index; j >= 0; j--)
                                {
                                    _loc.Add(((TJunction)_temp.Parent).BranchLeft.Elements[j]);
                                }
                            }

                            _dtee = null;
                        }
                        #endregion

                        _temp = _temp.Parent;
                    } while (_temp.Parent != null);

                    Check(this);
                    for (int j = _children.Count - 1; j >= 0; j--)
                    {
                        if (_temp == _children[j]) { _index = j; break; }
                    }

                    for (int j = _index; j >= 0; j--)
                    {
                        _loc.Add(_children[j]);
                    }
                }
                else
                {
                    Check(this);
                    for (int j = _children.Count - 2; j >= 0; j-- )
                    {
                        _loc.Add(_children[j]);
                    }
                }

                _result.Add(_loc);
            }
            return _result;
        }

        private void Check(ElementsCollection elementsCollection)
        {
            if (elementsCollection == null) { throw new ArgumentNullException(); }
            else if (elementsCollection.Count() == 0) { throw new Exception("Brak wystarczającej ilości elementów do przeprowadzenia obliczeń."); }
            else if ((from element in elementsCollection where element.Type == ElementType.Room select element).ToList().Count > 1)
            { throw new Exception("Zbyt duża liczba elementów typu Room w sekwencji."); }
            else if ((from element in elementsCollection where element.Type == ElementType.TJunction select element).ToList().Count > 1)
            { throw new Exception("Zbyt duża liczba elementów typu T-trónik w sekwencji."); }
            else if (elementsCollection.Last().Type != ElementType.Room && elementsCollection.Last().Type != ElementType.TJunction)
            { throw new Exception("Nieprawidłowa kolejność elementów w sekwencji."); }
            else if ((from element in elementsCollection where element.Type == ElementType.TJunction select element).ToList().Count == 1 &&
                (from element in elementsCollection where element.Type == ElementType.Room select element).ToList().Count == 1)
            { throw new Exception("Elementy typu T-trónik i Room nie mogą występować w tym samym ciągu."); }
        }

        public IEnumerator<ElementsBase> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
