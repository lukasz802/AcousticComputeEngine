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

        public override int AirFlow
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
                attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(Enums.Branch.Main, _local.BranchType, _main_in.Width / 1000.0, _main_in.Height / 1000.0,
                        _main_out.Width / 1000.0, _main_out.Height / 1000.0, _local.Width / 1000.0, _local.Height / 1000.0);
            }
            else if (_main_in.DuctType == DuctType.Rectangular && _local.DuctType == DuctType.Round)
            {
                attn = Function.Attenuation.JunctionMainRectangularBranchRound(Enums.Branch.Main, _local.BranchType, _main_in.Width / 1000.0, _main_in.Height / 1000.0,
                        _main_out.Width / 1000.0, _main_out.Height / 1000.0, _local.Diameter / 1000.0);

            }
            else if (_main_in.DuctType == DuctType.Round && _local.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRectangular(Enums.Branch.Main, _local.BranchType, _main_in.Diameter / 1000.0,
                        _main_out.Diameter / 1000.0, _local.Width / 1000.0, _local.Height / 1000.0);
            }
            else
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRound(Enums.Branch.Main, _local.BranchType, _main_in.Diameter / 1000.0,
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
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
                                   _main_in.Width / 1000.0 * _main_in.Height / 1000.0, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
               _main_in.Width / 1000.0 * _main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Rectangular && _local.DuctType == DuctType.Round)
            {
                if (_local.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   _main_in.Width / 1000.0 * _main_in.Height / 1000.0, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                                   _main_in.Width / 1000.0 * _main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Round && _local.DuctType == DuctType.Rectangular)
            {
                if (_local.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, _local.Width / 1000.0 * _local.Height / 1000.0,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (_local.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, _local.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.Branch.Main, _local.AirFlow, _main_in.AirFlow, Math.Pow(_local.Diameter / 1000.0, 2) * Math.PI * 0.25,
                   Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }
}
