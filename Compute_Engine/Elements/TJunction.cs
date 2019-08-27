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
    public class TJunction : ElementsBase, IDoubleBranchingElement<TJunctionBranch>
    {
        private static int _counter = 1;
        private static string _name = "tjnt_";
        private readonly TJunctionBranch _local_right = null;
        private readonly TJunctionBranch _local_left = null;
        private TJunctionContaier _container = null;

        /// <summary>Trójnik typu T.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductTypeMainIn">Typ głównego króćca podłączeniowego.</param>
        /// <param name="ductTypeBranch">Typ króćca odgałęźnego.</param>
        /// <param name="airFlowBranchRight">Przepływ powietrza przez odgałęzienie prawe [m3/h].</param>
        /// <param name="airFlowBranchLeft">Przepływ powietrza przez odgałęzienie lewe [m3/h].</param>
        /// <param name="branchTypeRight">Typ odgałęzienia prawego.</param>
        /// <param name="branchTypeLeft">Typ odgałęzienia lewego.</param>
        /// <param name="widthMainIn">Szerokość głównego króćca podłączeniowego [mm].</param>
        /// <param name="heightMainIn">Wysokość głównego króćca podłączeniowego [mm].</param>
        /// <param name="diameterMainIn">Średnica głównego króćca podłączeniowego [mm].</param>
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
        public TJunction(string name, string comments, DuctType ductTypeMainIn, int widthMainIn, int heightMainIn,
            int diameterMainIn, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft, bool include)
        {
            _type = ElementType.TJunction;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowBranchLeft + airFlowBranchRight;
            this.IsIncluded = include;
            _name = this.Name;
            _counter = 1;

            _container = new TJunctionContaier(this, ductTypeMainIn, widthMainIn, heightMainIn, diameterMainIn, ductTypeBranch,
                branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight, diameterBranchRight,
                roundingRight, branchTypeLeft, airFlowBranchLeft, widthBranchLeft, heightBranchLeft, diameterBranchLeft,
                roundingLeft);
            _local_right = new TJunctionBranch(_container, Branch.BranchRight);
            _local_left = new TJunctionBranch(_container, Branch.BranchLeft);
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }


        /// <summary>Trójnik typu T.</summary>
        public TJunction()
        {
            _type = ElementType.TJunction;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.IsIncluded = true;

            _container = new TJunctionContaier(this, DuctType.Rectangular, 400, 200, 450, DuctType.Rectangular,
                BranchType.Straight, 600, 160, 160, 200, 0, BranchType.Straight, 800, 160, 160, 200, 0);
            _local_right = new TJunctionBranch(_container, Branch.BranchRight);
            _local_left = new TJunctionBranch(_container, Branch.BranchLeft);
            base.AirFlow = 1400;
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }

        public TJunctionBranch BranchRight
        {
            get
            {
                return _local_right;
            }
        }

        public TJunctionBranch BranchLeft
        {
            get
            {
                return _local_left;
            }
        }

        internal TJunctionContaier Container
        {
            get
            {
                return _container;
            }
        }

        public int Width
        {
            get
            {
                return _container.WidthMain;
            }
            set
            {
                if (value < 100)
                {
                    _container.WidthMain = 100;
                }
                else if (value < 2000)
                {
                    _container.WidthMain = value;
                }
                else
                {
                    _container.WidthMain = 2000;
                }
            }
        }

        public int Height
        {
            get
            {
                return _container.HeightMain;
            }
            set
            {
                if (value < 100)
                {
                    _container.HeightMain = 100;
                }
                else if (value < 2000)
                {
                    _container.HeightMain = value;
                }
                else
                {
                    _container.HeightMain = 2000;
                }
            }
        }

        public int Diameter
        {
            get
            {
                return _container.DiameterMain;
            }
            set
            {
                if (value < 80)
                {
                    _container.DiameterMain = 80;
                }
                else if (value < 1600)
                {
                    _container.DiameterMain = value;
                }
                else
                {
                    _container.DiameterMain = 1600;
                }
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
                double temp = Convert.ToDouble(_container.AirFlowBranchRight) / Convert.ToDouble(_container.AirFlowBranchRight + _container.AirFlowBranchLeft);
                _container.AirFlowBranchRight = (int)Math.Round(temp * value);
                _container.AirFlowBranchLeft = (int)Math.Round((1 - temp) * value);
            }
        }

        public double Velocity
        {
            get
            {
                if (_container.DuctTypeMain == DuctType.Rectangular)
                {
                    return (this.AirFlow / 3600.0) / ((_container.WidthMain / 1000.0) * (_container.HeightMain / 1000.0));
                }
                else
                {
                    return (this.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_container.DiameterMain / 1000.0, 2));
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _container.DuctTypeMain;
            }
            set
            {
                _container.DuctTypeMain = value;
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = { 0, 0, 0, 0, 0, 0, 0, 0 };
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_container.DuctTypeMain == DuctType.Rectangular && _container.DuctTypeBranch == DuctType.Rectangular)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0, _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0,
                        _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0, _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0, _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0,
                        _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0, _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Rectangular)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                        _container.WidthBranchRight / 1000.0 * _container.HeightBranchRight / 1000.0,
                        _container.WidthBranchLeft / 1000.0 * _container.HeightBranchLeft / 1000.0, Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else if (_container.DuctTypeMain == DuctType.Round && _container.DuctTypeBranch == DuctType.Round)
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), Math.PI * 0.25 * Math.Pow(_container.DiameterMain / 1000.0, 2),
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            else
            {
                if (_container.BranchTypeRight == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                        _container.RoundingBranchRight / 1000.0, _container.RoundingBranchLeft / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(Branch.Main, _container.AirFlowBranchRight, _container.AirFlowBranchLeft,
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchRight / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(_container.DiameterBranchLeft / 1000.0, 2), _container.WidthMain / 1000.0 * _container.HeightMain / 1000.0,
                        _container.RoundingBranchRight / 1000.0, 0, Turbulence.No);
                }
            }
            return lw;
        }
    }
}
