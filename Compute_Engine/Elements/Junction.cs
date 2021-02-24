using Compute_Engine.Factories;
using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class Junction : ElementsBase, IChangeableDimensions<IDuctConnection>, ISingleBranchingElement<IBranch>
    {
        private static int _counter = 1;
        private static string _name = "jnt_";
        private IBranch _local;
        private IDuctConnection _main_in;
        private IDuctConnection _main_out;

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

            if (airFlowBranch < 0)
            {
                airFlowBranch = 0;
            }

            if (airFlowBranch >= airFlowMainIn)
            {
                airFlowBranch = airFlowMainIn;
            }

            _main_in = ConnectionElementsFactory.GetConnectionElement(ductTypeMainIn, base.AirFlow, widthMainIn, heightMainIn, diameterMainIn);
            _main_out = ConnectionElementsFactory.GetConnectionElement(ductTypeMainIn, base.AirFlow - airFlowBranch, widthMainOut, heightMainOut, diameterMainOut);
            _local = BranchingElementsFactory.GetJunctionBranch(_main_in, _main_out, ductTypeBranch, branchType, airFlowBranch, widthBranch,
                heightBranch, diameterBranch, roundingBranch);
            _main_in.AirFlowChanged += UpdateInletAirFlow;
            _main_in.DuctTypeChanged += UpdateInletDuctType;
            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _main_out.DuctTypeChanged += UpdateOutletDuctType;
            _local.AirFlowChanged += UpdateBranchAirFlow;
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

            _main_in = ConnectionElementsFactory.GetConnectionElement(DuctType.Rectangular, base.AirFlow, 400, 200, 450);
            _main_out = ConnectionElementsFactory.GetConnectionElement(DuctType.Rectangular, base.AirFlow - 400, 400, 200, 450);
            _local = BranchingElementsFactory.GetJunctionBranch(_main_in, _main_out, DuctType.Rectangular, BranchType.Straight, 400, 160, 160, 200, 0);
            _main_in.AirFlowChanged += UpdateInletAirFlow;
            _main_in.DuctTypeChanged += UpdateInletDuctType;
            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _main_out.DuctTypeChanged += UpdateOutletDuctType;
            _local.AirFlowChanged += UpdateBranchAirFlow;
            this.Branch.Elements._parent = this;
        }

        public IBranch Branch
        {
            get
            {
                return _local;
            }
        }

        public IDuctConnection Inlet
        {
            get
            {
                return _main_in;
            }
        }

        public IDuctConnection Outlet
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
                base.AirFlow = Inlet.AirFlow = value;
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_main_in.DuctType == DuctType.Rectangular && Branch.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.JunctionMainRectangularBranchRectangular(Enums.BranchDirection.Main, Branch.BranchType, _main_in.Width / 1000.0, _main_in.Height / 1000.0,
                        Outlet.Width / 1000.0, Outlet.Height / 1000.0, Branch.Width / 1000.0, Branch.Height / 1000.0);
            }
            else if (_main_in.DuctType == DuctType.Rectangular && Branch.DuctType == DuctType.Round)
            {
                attn = Function.Attenuation.JunctionMainRectangularBranchRound(Enums.BranchDirection.Main, Branch.BranchType, _main_in.Width / 1000.0, _main_in.Height / 1000.0,
                        Outlet.Width / 1000.0, Outlet.Height / 1000.0, Branch.Diameter / 1000.0);

            }
            else if (_main_in.DuctType == DuctType.Round && Branch.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRectangular(Enums.BranchDirection.Main, Branch.BranchType, _main_in.Diameter / 1000.0,
                        Outlet.Diameter / 1000.0, Branch.Width / 1000.0, Branch.Height / 1000.0);
            }
            else
            {
                attn = Function.Attenuation.JunctionMainRoundBranchRound(Enums.BranchDirection.Main, Branch.BranchType, _main_in.Diameter / 1000.0,
                        Outlet.Diameter / 1000.0, Branch.Diameter / 1000.0);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_main_in.DuctType == DuctType.Rectangular && Branch.DuctType == DuctType.Rectangular)
            {
                if (Branch.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Branch.Width / 1000.0 * Branch.Height / 1000.0,
                         _main_in.Width / 1000.0 * _main_in.Height / 1000.0, Branch.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Branch.Width / 1000.0 * Branch.Height / 1000.0,
                        _main_in.Width / 1000.0 * _main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Rectangular && Branch.DuctType == DuctType.Round)
            {
                if (Branch.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Math.Pow(Branch.Diameter / 1000.0, 2) * Math.PI * 0.25,
                        _main_in.Width / 1000.0 * _main_in.Height / 1000.0, Branch.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Math.Pow(Branch.Diameter / 1000.0, 2) * Math.PI * 0.25,
                        _main_in.Width / 1000.0 * _main_in.Height / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Round && Branch.DuctType == DuctType.Rectangular)
            {
                if (Branch.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Branch.Width / 1000.0 * Branch.Height / 1000.0,
                        Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, Branch.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Branch.Width / 1000.0 * Branch.Height / 1000.0,
                        Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            else
            {
                if (Branch.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Math.Pow(Branch.Diameter / 1000.0, 2) * Math.PI * 0.25,
                        Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, Branch.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.Junction(Enums.BranchDirection.Main, Branch.AirFlow, base.AirFlow, Math.Pow(Branch.Diameter / 1000.0, 2) * Math.PI * 0.25,
                        Math.Pow(_main_in.Diameter / 1000.0, 2) * Math.PI * 0.25, 0, Turbulence.No);
                }
            }
            return lw;
        }

        private void UpdateInletDuctType(object sender, EventArgs e)
        {
            _main_out.DuctTypeChanged -= UpdateOutletDuctType;
            Outlet.DuctType = Inlet.DuctType;
            _main_out.DuctTypeChanged += UpdateOutletDuctType;
        }

        private void UpdateOutletDuctType(object sender, EventArgs e)
        {
            _main_in.DuctTypeChanged -= UpdateInletDuctType;
            Inlet.DuctType = Outlet.DuctType;
            _main_in.DuctTypeChanged += UpdateInletDuctType;
        }

        private void UpdateInletAirFlow(object sender, EventArgs e)
        {
            _main_out.AirFlowChanged -= UpdateOutletAirFlow;
            _local.AirFlowChanged -= UpdateBranchAirFlow;

            base.AirFlow = Inlet.AirFlow;
            if (base.AirFlow >= Branch.AirFlow)
            {
                Outlet.AirFlow = base.AirFlow - Branch.AirFlow;
            }
            else
            {
                Branch.AirFlow = base.AirFlow;
                Outlet.AirFlow = 0;
            }

            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _local.AirFlowChanged += UpdateBranchAirFlow;
        }

        private void UpdateOutletAirFlow(object sender, EventArgs e)
        {
            _main_in.AirFlowChanged -= UpdateInletAirFlow;
            Inlet.AirFlow = Outlet.AirFlow + Branch.AirFlow;
            base.AirFlow = Inlet.AirFlow;
            _main_in.AirFlowChanged += UpdateInletAirFlow;
        }

        private void UpdateBranchAirFlow(object sender, EventArgs e)
        {
            _main_out.AirFlowChanged -= UpdateOutletAirFlow;
            _main_in.AirFlowChanged -= UpdateInletAirFlow;

            if (Branch.AirFlow <= Inlet.AirFlow)
            {
                Outlet.AirFlow = Inlet.AirFlow - Branch.AirFlow;
            }
            else
            {
                Branch.AirFlow = Inlet.AirFlow;
                Outlet.AirFlow = 0;
            }

            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _main_in.AirFlowChanged += UpdateInletAirFlow;
        }
    }
}
