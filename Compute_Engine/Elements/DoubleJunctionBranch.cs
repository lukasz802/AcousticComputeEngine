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
                        return (_container.AirFlowBranchLeft / 3600.0) / (0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2));
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
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height,
                        _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Diameter, _container.Out.Diameter, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.BranchRight, _container.BranchTypeRight,
                        _container.In.Diameter, _container.Out.Diameter, _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
            }
            else
            {
                if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.BranchLeft, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.BranchLeft, _container.BranchTypeRight,
                        _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height,
                        _container.DiameterBranchRight, _container.DiameterBranchLeft);
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.BranchLeft, _container.BranchTypeRight,
                        _container.In.Diameter, _container.Out.Diameter, _container.WidthBranchRight, _container.HeightBranchRight,
                        _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.BranchLeft, _container.BranchTypeRight,
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
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
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
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
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
}
