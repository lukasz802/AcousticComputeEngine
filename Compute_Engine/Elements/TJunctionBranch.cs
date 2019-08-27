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
                        _container.AirFlowBranchLeft = 0;
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
                    attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(Branch.BranchRight, _container.BranchTypeRight, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5),
                         Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5), _container.WidthBranchRight, _container.HeightBranchRight);
                }
                else if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Round)
                {
                    attn = Function.Attenuation.JunctionMainRectangularBranchRound(Branch.BranchRight, _container.BranchTypeRight, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5),
                        Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchLeft, 2), 0.5), _container.DiameterBranchRight);
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRectangular(Branch.BranchRight, _container.BranchTypeRight, _container.DiameterMain,
                        Math.Pow((4 * _container.WidthBranchLeft * _container.HeightBranchLeft) / Math.PI, 0.5), _container.WidthBranchRight, _container.HeightBranchRight);
                }
                else
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRound(Branch.BranchRight, _container.BranchTypeRight, _container.DiameterMain,
                        _container.DiameterBranchLeft, _container.DiameterBranchRight);
                }
            }
            else
            {
                if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(Branch.BranchLeft, _container.BranchTypeLeft, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5),
                         Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5), _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Round)
                {
                    attn = Function.Attenuation.JunctionMainRectangularBranchRound(Branch.BranchLeft, _container.BranchTypeLeft, _container.WidthMain,
                        _container.WidthMain, Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5),
                        Math.Pow(0.25 * Math.PI * Math.Pow(_container.DiameterBranchRight, 2), 0.5), _container.DiameterBranchLeft);
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRectangular(Branch.BranchLeft, _container.BranchTypeLeft, _container.DiameterMain,
                        Math.Pow((4 * _container.WidthBranchRight * _container.HeightBranchRight) / Math.PI, 0.5), _container.WidthBranchLeft, _container.HeightBranchLeft);
                }
                else
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRound(Branch.BranchLeft, _container.BranchTypeLeft, _container.DiameterMain,
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
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                             0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchRight, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
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
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                            _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                            _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Round)
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
                else
                {
                    if (_container.BranchTypeRight == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(Branch.BranchLeft, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                            _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                    }
                }
            }
            return lw;
        }
    }
}
