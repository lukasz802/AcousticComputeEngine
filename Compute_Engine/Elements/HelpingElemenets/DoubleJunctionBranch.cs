using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class DoubleJunctionBranch : BranchElementTemplate, IBranch
    {
        private IDuctConnection _in;
        private IDuctConnection _out;
        private IBranch _branch_in_pair;
        private readonly BranchDirection _branch_direction;

        public DoubleJunctionBranch(IDuctConnection connectionIn, IDuctConnection connectionOut, IBranch branchInPair, BranchDirection branchDirection,
            DuctType ductTypeBranch, BranchType branchType, int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int rounding)
            : base(ductTypeBranch, branchType, airFlowBranch, widthBranch, heightBranch, diameterBranch, rounding)
        {
            _in = connectionIn;
            _out = connectionOut;
            _branch_in_pair = branchInPair;
            _branch_direction = branchDirection;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_branch_direction == BranchDirection.BranchRight)
            {
                if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRectangular(BranchDirection.BranchRight, BranchType,
                        _in.Width, _in.Height, _out.Width, _out.Height, Width, Height, _branch_in_pair.Width, _branch_in_pair.Height);
                }
                else if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRound(BranchDirection.BranchRight, BranchType,
                        _in.Width, _in.Height, _out.Width, _out.Height, Diameter, _branch_in_pair.Diameter);
                }
                else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRectangular(BranchDirection.BranchRight, BranchType,
                        _in.Diameter, _out.Diameter, Width, Height, _branch_in_pair.Width, _branch_in_pair.Height);
                }
                else
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRound(BranchDirection.BranchRight, BranchType,
                        _in.Diameter, _out.Diameter, Diameter, _branch_in_pair.Diameter);
                }
            }
            else
            {
                if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRectangular(BranchDirection.BranchLeft, _branch_in_pair.BranchType,
                        _in.Width, _in.Height, _out.Width, _out.Height, _branch_in_pair.Width, _branch_in_pair.Height, Width, Height);
                }
                else if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRound(BranchDirection.BranchLeft, _branch_in_pair.BranchType,
                        _in.Width, _in.Height, _out.Width, _out.Height, _branch_in_pair.Diameter, Diameter);
                }
                else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRectangular(BranchDirection.BranchLeft, _branch_in_pair.BranchType,
                        _in.Diameter, _out.Diameter, _branch_in_pair.Width, _branch_in_pair.Height, Width, Height);
                }
                else
                {
                    attn = Function.Attenuation.DoubleJunctionMainRoundBranchRound(BranchDirection.BranchLeft, _branch_in_pair.BranchType,
                        _in.Diameter, _out.Diameter, _branch_in_pair.Diameter, Diameter);
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
                if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            _in.Width / 1000.0 * _in.Height / 1000.0, Rounding / 1000.0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            _in.Width / 1000.0 * _in.Height / 1000.0, 0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                }
                else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2), Rounding / 1000.0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Width / 1000.0 * Height / 1000.0, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2), 0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                }
                else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Round)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2),
                            Rounding / 1000.0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2),
                            0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                }
                else
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), _in.Width / 1000.0 * _in.Height / 1000.0,
                            Rounding / 1000.0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchRight, AirFlow, _branch_in_pair.AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2), _in.Width / 1000.0 * _in.Height / 1000.0,
                            0, _branch_in_pair.Rounding / 1000.0, Turbulence.No);
                    }
                }
            }
            else
            {
                if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, _in.Width / 1000.0 * _in.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, _in.Width / 1000.0 * _in.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, _branch_in_pair.Width / 1000.0 * _branch_in_pair.Height / 1000.0,
                            Width / 1000.0 * Height / 1000.0, Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
                else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Round)
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_in.Diameter / 1000.0, 2),
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
                else
                {
                    if (BranchType == BranchType.Rounded)
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), _in.Width / 1000.0 * _in.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, Rounding / 1000.0, Turbulence.No);
                    }
                    else
                    {
                        lw = Function.Noise.DoubleJunction(BranchDirection.BranchLeft, _branch_in_pair.AirFlow, AirFlow,
                            _in.AirFlow, Math.PI * 0.25 * Math.Pow(_branch_in_pair.Diameter / 1000.0, 2),
                             Math.PI * 0.25 * Math.Pow(Diameter / 1000.0, 2), _in.Width / 1000.0 * _in.Height / 1000.0,
                            _branch_in_pair.Rounding / 1000.0, 0, Turbulence.No);
                    }
                }
            }
            return lw;
        }
    }
}
