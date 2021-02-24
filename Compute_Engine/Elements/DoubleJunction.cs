using Compute_Engine.Factories;
using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class DoubleJunction : ElementsBase, IChangeableDimensions<IDuctConnection>, IDoubleBranchingElement<IBranch>
    {
        private static int _counter = 1;
        private static string _name = "djnt_";
        private IBranch _local_right;
        private IBranch _local_left;
        private IDuctConnection _main_in;
        private IDuctConnection _main_out;

        /// <summary>Czwórnik.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductTypeMainIn">Typ głównego króćca podłączeniowego od strony wlotowej.</param>
        /// <param name="ductTypeBranch">Typ króćca odgałęźnego.</param>
        /// <param name="airFlowMainOut">Przepływ powietrza na wylocie z elementu [m3/h].</param>
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
        public DoubleJunction(string name, string comments, DuctType ductTypeMainIn, int airFlowMainOut, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft, bool include)
        {
            if (airFlowBranchRight < 0)
            {
                airFlowBranchRight = 0;
            }

            if (airFlowBranchLeft < 0)
            {
                airFlowBranchLeft = 0;
            }

            _type = ElementType.DoubleJunction;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowMainOut + airFlowBranchRight + airFlowBranchLeft;
            this.IsIncluded = include;
            _name = this.Name;
            _counter = 1;

            _main_in = ConnectionElementsFactory.GetConnectionElement(ductTypeMainIn, base.AirFlow, widthMainIn, heightMainIn, diameterMainIn);
            _main_out = ConnectionElementsFactory.GetConnectionElement(ductTypeMainIn, airFlowMainOut,
                widthMainOut, heightMainOut, diameterMainOut);
            _local_right = BranchingElementsFactory.GetDoubleJunctionBranch(_main_in, _main_out, _local_left, BranchDirection.BranchRight, 
                ductTypeBranch, branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight, diameterBranchRight, roundingRight);
            _local_left = BranchingElementsFactory.GetDoubleJunctionBranch(_main_in, _main_out, _local_right, BranchDirection.BranchLeft,
                ductTypeBranch, branchTypeLeft, airFlowBranchLeft, widthBranchLeft, heightBranchLeft, diameterBranchLeft, roundingLeft);
            _local_right = BranchingElementsFactory.GetDoubleJunctionBranch(_main_in, _main_out, _local_left, BranchDirection.BranchRight,
                ductTypeBranch, branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight, diameterBranchRight, roundingRight);
            _main_in.AirFlowChanged += UpdateInletAirFlow;
            _main_in.DuctTypeChanged += UpdateInletDuctType;
            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _main_out.DuctTypeChanged += UpdateOutletDuctType;
            _local_right.AirFlowChanged += UpdateRightBranchAirFlow;
            _local_left.AirFlowChanged += UpdateLeftBranchAirFlow;
            _local_right.DuctTypeChanged += UpdateBranchRightDuctType;
            _local_left.DuctTypeChanged += UpdateBranchLeftDuctType;
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

            _main_in = ConnectionElementsFactory.GetConnectionElement(DuctType.Rectangular, base.AirFlow, 400, 400, 450);
            _main_out = ConnectionElementsFactory.GetConnectionElement(DuctType.Rectangular, base.AirFlow - 1200, 200, 200, 250);
            _local_right = BranchingElementsFactory.GetDoubleJunctionBranch(_main_in, _main_out, _local_left, BranchDirection.BranchLeft,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0);
            _local_left = BranchingElementsFactory.GetDoubleJunctionBranch(_main_in, _main_out, _local_right, BranchDirection.BranchRight,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0);
            _local_right = BranchingElementsFactory.GetDoubleJunctionBranch(_main_in, _main_out, _local_left, BranchDirection.BranchLeft,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0);
            _main_in.AirFlowChanged += UpdateInletAirFlow;
            _main_in.DuctTypeChanged += UpdateInletDuctType;
            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _main_out.DuctTypeChanged += UpdateOutletDuctType;
            _local_right.AirFlowChanged += UpdateRightBranchAirFlow;
            _local_left.AirFlowChanged += UpdateLeftBranchAirFlow;
            _local_right.DuctTypeChanged += UpdateBranchRightDuctType;
            _local_left.DuctTypeChanged += UpdateBranchLeftDuctType;
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }

        public IBranch BranchRight
        {
            get
            {
                return _local_right;
            }
        }

        public IBranch BranchLeft
        {
            get
            {
                return _local_left;
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

            if (_main_in.DuctType == DuctType.Rectangular && BranchRight.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRectangular(BranchDirection.Main, BranchRight.BranchType,
                    _main_in.Width, _main_in.Height, _main_out.Width, _main_out.Height, BranchRight.Width, BranchRight.Height,
                    BranchLeft.Width, BranchLeft.Height);
            }
            else if (_main_in.DuctType == DuctType.Rectangular && BranchRight.DuctType == DuctType.Round)
            {
                attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRound(BranchDirection.Main, BranchRight.BranchType,
                    _main_in.Width, _main_in.Height, _main_out.Width, _main_out.Height,
                    BranchRight.Diameter, BranchLeft.Diameter);
            }
            else if (_main_in.DuctType == DuctType.Round && BranchRight.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.DoubleJunctionMainRoundBranchRectangular(BranchDirection.Main, BranchRight.BranchType,
                    _main_in.Diameter, _main_out.Diameter, BranchRight.Width, BranchRight.Height,
                    BranchLeft.Width, BranchLeft.Height);
            }
            else
            {
                attn = Function.Attenuation.DoubleJunctionMainRoundBranchRound(BranchDirection.Main, BranchRight.BranchType,
                    _main_in.Diameter, _main_out.Diameter, BranchRight.Diameter, BranchLeft.Diameter);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_main_in.DuctType == DuctType.Rectangular && BranchRight.DuctType == DuctType.Rectangular)
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0,
                        BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0, _main_in.Width / 1000.0 * _main_in.Height / 1000.0,
                        BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0,
                        BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0, _main_in.Width / 1000.0 * _main_in.Height / 1000.0,
                        BranchRight.Rounding / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Round && BranchRight.DuctType == DuctType.Rectangular)
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0,
                        BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0, Math.PI * 0.25 * Math.Pow(_main_in.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0,
                        BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0, Math.PI * 0.25 * Math.Pow(_main_in.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_main_in.DuctType == DuctType.Round && BranchRight.DuctType == DuctType.Round)
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_main_in.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_main_in.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, 0, Turbulence.No);
                }
            }
            else
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter / 1000.0, 2), _main_in.Width / 1000.0 * _main_in.Height / 1000.0,
                        BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        _main_in.AirFlow, Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter / 1000.0, 2), _main_in.Width / 1000.0 * _main_in.Height / 1000.0,
                        BranchRight.Rounding / 1000.0, 0, Turbulence.No);
                }
            }
            return lw;
        }

        private void UpdateBranchRightDuctType(object sender, EventArgs e)
        {
            BranchLeft.DuctTypeChanged -= UpdateBranchLeftDuctType;
            BranchLeft.DuctType = BranchRight.DuctType;
            BranchLeft.DuctTypeChanged += UpdateBranchLeftDuctType;
        }

        private void UpdateBranchLeftDuctType(object sender, EventArgs e)
        {
            BranchRight.DuctTypeChanged -= UpdateBranchRightDuctType;
            BranchRight.DuctType = BranchLeft.DuctType;
            BranchRight.DuctTypeChanged += UpdateBranchRightDuctType;
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
            _local_left.AirFlowChanged -= UpdateLeftBranchAirFlow;
            _local_right.AirFlowChanged -= UpdateRightBranchAirFlow;

            base.AirFlow = Inlet.AirFlow;
            if (base.AirFlow >= (BranchRight.AirFlow + BranchLeft.AirFlow))
            {
                Outlet.AirFlow = base.AirFlow - BranchRight.AirFlow - BranchLeft.AirFlow;
            }
            else
            {
                var n = (double)BranchRight.AirFlow / (BranchRight.AirFlow + BranchLeft.AirFlow);

                BranchRight.AirFlow = (int)Math.Ceiling(base.AirFlow * n);
                BranchLeft.AirFlow = base.AirFlow - BranchRight.AirFlow;
                Outlet.AirFlow = 0;
            }

            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _local_left.AirFlowChanged += UpdateLeftBranchAirFlow;
            _local_right.AirFlowChanged += UpdateRightBranchAirFlow;
        }

        private void UpdateOutletAirFlow(object sender, EventArgs e)
        {
            _main_in.AirFlowChanged -= UpdateInletAirFlow;

            Inlet.AirFlow = Outlet.AirFlow + BranchRight.AirFlow + BranchLeft.AirFlow;
            base.AirFlow = Inlet.AirFlow;

            _main_in.AirFlowChanged += UpdateInletAirFlow;
        }

        private void UpdateRightBranchAirFlow(object sender, EventArgs e)
        {
            _main_out.AirFlowChanged -= UpdateOutletAirFlow;
            _main_in.AirFlowChanged -= UpdateInletAirFlow;
            _local_left.AirFlowChanged -= UpdateLeftBranchAirFlow;

            if (BranchRight.AirFlow <= (Inlet.AirFlow - BranchLeft.AirFlow))
            {
                Outlet.AirFlow = Inlet.AirFlow - BranchRight.AirFlow - BranchLeft.AirFlow;
            }
            else if (BranchRight.AirFlow <= Inlet.AirFlow)
            {
                BranchLeft.AirFlow = Inlet.AirFlow - BranchRight.AirFlow;
                Outlet.AirFlow = 0;
            }
            else
            {
                BranchRight.AirFlow = Inlet.AirFlow;
                Outlet.AirFlow = BranchLeft.AirFlow = 0;
            }

            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _main_in.AirFlowChanged += UpdateInletAirFlow;
            _local_left.AirFlowChanged += UpdateLeftBranchAirFlow;
        }

        private void UpdateLeftBranchAirFlow(object sender, EventArgs e)
        {
            _main_out.AirFlowChanged -= UpdateOutletAirFlow;
            _main_in.AirFlowChanged -= UpdateInletAirFlow;
            _local_right.AirFlowChanged -= UpdateRightBranchAirFlow;

            if (BranchLeft.AirFlow <= (Inlet.AirFlow - BranchRight.AirFlow))
            {
                Outlet.AirFlow = Inlet.AirFlow - BranchRight.AirFlow - BranchLeft.AirFlow;
            }
            else if (BranchRight.AirFlow <= Inlet.AirFlow)
            {
                BranchRight.AirFlow = Inlet.AirFlow - BranchLeft.AirFlow;
                Outlet.AirFlow = 0;
            }
            else
            {
                BranchLeft.AirFlow = Inlet.AirFlow;
                Outlet.AirFlow = BranchRight.AirFlow = 0;
            }

            _main_out.AirFlowChanged += UpdateOutletAirFlow;
            _main_in.AirFlowChanged += UpdateInletAirFlow;
            _local_right.AirFlowChanged += UpdateRightBranchAirFlow;
        }
    }
}
