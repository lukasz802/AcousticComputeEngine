using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class JunctionBranch : BranchElementTemplate, IBranch
    {
        private IDuctConnection _in;
        private IDuctConnection _out;

        public JunctionBranch(IDuctConnection connectionIn, IDuctConnection connectionOut, DuctType ductTypeBranch, BranchType branchType,
            int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int rounding)
            : base(ductTypeBranch, branchType, airFlowBranch, widthBranch, heightBranch, diameterBranch, rounding)
        {
            _in = connectionIn;
            _out = connectionOut;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(BranchDirection.BranchRight, BranchType, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, Width / 1000.0, Height / 1000.0);
            }
            else if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Round)
            {
                attn = Function.Attenuation.JunctionMainRectangularBranchRound(BranchDirection.BranchRight, BranchType, _in.Width / 1000.0, _in.Height / 1000.0,
                        _out.Width / 1000.0, _out.Height / 1000.0, Diameter / 1000.0);

            }
            else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRectangular(BranchDirection.BranchRight, BranchType, _in.Diameter / 1000.0,
                        _out.Diameter / 1000.0, Width / 1000.0, Height / 1000.0);
            }
            else
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRound(BranchDirection.BranchRight, BranchType, _in.Diameter / 1000.0,
                        _out.Diameter / 1000.0, Diameter / 1000.0);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public double[] Noise()
        {
            double[] lw = new double[8];

            if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Rectangular)
            {
                if (BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Width / 1000.0 * Height / 1000.0,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Width / 1000.0 * Height / 1000.0,
               _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Rectangular && DuctType == DuctType.Round)
            {
                if (BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Math.Pow(Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Math.Pow(Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   _in.Width / 1000.0 * _in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_in.DuctType == DuctType.Round && DuctType == DuctType.Rectangular)
            {
                if (BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Width / 1000.0 * Height / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Width / 1000.0 * Height / 1000.0,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Math.Pow(Diameter / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(BranchDirection.BranchRight, AirFlow, _in.AirFlow, Math.Pow(Diameter / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }
}
