using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class TJunctionBranch : BranchElementTemplate, IBranch
    {
        private IDuctConnection _connection;
        private IBranch _branch_in_pair;
        private BranchDirection _branch_direction;

        public TJunctionBranch(IDuctConnection connection, IBranch branchInPair, BranchDirection branchDirection,
            DuctType ductTypeBranch, BranchType branchType, int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int rounding)
            : base(ductTypeBranch, branchType, airFlowBranch, widthBranch, heightBranch, diameterBranch, rounding)
        {
            _connection = connection;
            _branch_in_pair = branchInPair;
            _branch_direction = branchDirection;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_branch_direction == BranchDirection.BranchRight)
            {
                if (_connection.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(BranchDirection.BranchRight, BranchType, _connection.Width,
                         _connection.Height, Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5),
                         Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5), Width, Height);
                }
                else if (_connection.DuctType == DuctType.Rectangular && DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.JunctionMainRectangularBranchRound(BranchDirection.BranchRight, BranchType, _connection.Width,
                         _connection.Height, Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5),
                        Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5), Diameter);
                }
                else if (_connection.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRectangular(BranchDirection.BranchRight, BranchType, _connection.Diameter,
                        Math.Pow((4 * _branch_in_pair.Width * _branch_in_pair.Height) / Math.PI, 0.5), Width, Height);
                }
                else
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRound(BranchDirection.BranchRight, BranchType, _connection.Diameter,
                        _branch_in_pair.Diameter, Diameter);
                }
            }
            else
            {
                if (_connection.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(BranchDirection.BranchLeft, BranchType, _connection.Width,
                         _connection.Height, Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5),
                         Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5), Width, Height);
                }
                else if (_connection.DuctType == DuctType.Rectangular && DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.JunctionMainRectangularBranchRound(BranchDirection.BranchLeft, BranchType, _connection.Width,
                        _connection.Height, Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5),
                        Math.Pow(0.25 * Math.PI * Math.Pow(_branch_in_pair.Diameter, 2), 0.5), Diameter);
                }
                else if (_connection.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRectangular(BranchDirection.BranchLeft, BranchType, _connection.Diameter,
                        Math.Pow((4 * _branch_in_pair.Width * _branch_in_pair.Height) / Math.PI, 0.5), Width, Height);
                }
                else
                {
                    attn = Function.Attenuation.JunctionMainRoundBranchRound(BranchDirection.BranchLeft, BranchType, _connection.Diameter,
                        _branch_in_pair.Diameter, Diameter);
                }
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public double[] Noise()
        {
            double[] lw = new double[8];

            if (_branch_direction == BranchDirection.BranchRight)
            {
                if (_connection.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            _connection.Width / 1000.0 * _connection.Height / 1000.0, Rounding / 1000.0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            _connection.Width / 1000.0 * _connection.Height / 1000.0, 0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                }
                else if (_connection.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0, 
                            Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2), Rounding, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2), 0, _branch_in_pair.Diameter / 1000.0, Turbulence.No);
                    }
                }
                else if (_connection.DuctType == DuctType.Round && DuctType == DuctType.Round)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2),
                             Rounding / 1000.0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2),
                             0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                }
                else
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), _connection.Width / 1000.0 * _connection.Height / 1000.0,
                             Rounding / 1000.0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), _connection.Width / 1000.0 * _connection.Height / 1000.0,
                             0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                }
            }
            else
            {
                if (_connection.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    if (_branch_in_pair.BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, _connection.Width / 1000.0 * _connection.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, _connection.Width / 1000.0 * _connection.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_connection.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    if (_branch_in_pair.BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_connection.DuctType == DuctType.Round && DuctType == DuctType.Round)
                {
                    if (_branch_in_pair.BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_connection.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
                else
                {
                    if (_branch_in_pair.BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), _connection.Width / 1000.0 * _connection.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.TJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), _connection.Width / 1000.0 * _connection.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
            }
            return lw;
        }
    }
}
