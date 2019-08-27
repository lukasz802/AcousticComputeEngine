using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class JunctionBranch : IBranch
    {
        private JunctionConnectionSide connectionSide;
        private DuctConnection _in = null;
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
                attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(Branch.BranchRight, _branch_type, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, _width_branch / 1000.0, _height_branch / 1000.0);
            }
            else if (_in.DuctType == DuctType.Rectangular && _duct_type_branch == DuctType.Round)
            {
                attn = Function.Attenuation.JunctionMainRectangularBranchRound(Branch.BranchRight, _branch_type, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, _diameter_branch / 1000.0);

            }
            else if (_in.DuctType == DuctType.Round && _duct_type_branch == DuctType.Rectangular)
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRectangular(Branch.BranchRight, _branch_type, _in.Diameter / 1000.0,
                        _out.Diameter / 1000.0, _width_branch / 1000.0, _height_branch / 1000.0);
            }
            else
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRound(Branch.BranchRight, _branch_type, _in.Diameter / 1000.0,
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
                    lw = Compute_Engine.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
               _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Rectangular && _duct_type_branch == DuctType.Round)
            {
                if (_branch_type == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Round && _duct_type_branch == DuctType.Rectangular)
            {
                if (_branch_type == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, _width_branch / 1000.0 * _height_branch / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (_branch_type == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _rnd_branch / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Branch.BranchRight, _airflow_branch, _in.AirFlow, Math.Pow(_diameter_branch / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }
}
