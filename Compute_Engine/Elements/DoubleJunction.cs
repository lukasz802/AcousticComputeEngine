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

        public override int AirFlow
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
                attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRectangular(Branch.Main, _container.BranchTypeRight,
                    _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height, _container.WidthBranchRight, _container.HeightBranchRight,
                    _container.WidthBranchLeft, _container.HeightBranchLeft);
            }
            else if (_container.In.DuctType == DuctType.Rectangular && _container.DuctType == DuctType.Round)
            {
                attn = Function.Attenuation.DoubleJunctionMainRectangularBranchRound(Branch.Main, _container.BranchTypeRight,
                    _container.In.Width, _container.In.Height, _container.Out.Width, _container.Out.Height,
                    _container.DiameterBranchRight, _container.DiameterBranchLeft);
            }
            else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.DoubleJunctionMainRoundBranchRectangular(Branch.Main, _container.BranchTypeRight,
                    _container.In.Diameter, _container.Out.Diameter, _container.WidthBranchRight, _container.HeightBranchRight,
                    _container.WidthBranchLeft, _container.HeightBranchLeft);
            }
            else
            {
                attn = Function.Attenuation.DoubleJunctionMainRoundBranchRound(Branch.Main, _container.BranchTypeRight,
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
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Rectangular)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.In.DuctType == DuctType.Round && _container.DuctType == DuctType.Round)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.In.Diameter / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.DoubleJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.In.AirFlow, Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.In.Width / 1000.0 * _container.In.Height / 1000.0,
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }
}
