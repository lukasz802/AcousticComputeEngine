using Compute_Engine.Factories;
using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class TJunction : ElementsBase, IDuctConnection, IDoubleBranchingElement<IBranch>
    {
        private static int _counter = 1;
        private static string _name = "tjnt_";
        private int _width;
        private int _height;
        private int _diameter;
        private DuctType _ductType;
        private readonly IBranch _local_right;
        private readonly IBranch _local_left;

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
        public TJunction(string name, string comments, DuctType ductTypeMainIn, int widthMainIn, int heightMainIn, int diameterMainIn,
            DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight, int widthBranchRight, int heightBranchRight,
            int diameterBranchRight, int roundingRight, BranchType branchTypeLeft, int airFlowBranchLeft, int widthBranchLeft,
            int heightBranchLeft, int diameterBranchLeft, int roundingLeft, bool include)
        {

            if (airFlowBranchRight < 0)
            {
                airFlowBranchRight = 0;
            }

            if (airFlowBranchLeft < 0)
            {
                airFlowBranchLeft = 0;
            }

            _type = ElementType.TJunction;
            this.DuctType = ductTypeMainIn;
            this.Width = widthMainIn;
            this.Height = heightMainIn;
            this.Diameter = diameterMainIn;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlowBranchRight + airFlowBranchLeft;
            this.IsIncluded = include;
            _name = this.Name;
            _counter = 1;

            _local_right = BranchingElementsFactory.GetTJunctionBranch(this, _local_left, BranchDirection.BranchRight,
                ductTypeBranch, branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight, diameterBranchRight, roundingRight);
            _local_left = BranchingElementsFactory.GetTJunctionBranch(this, _local_right, BranchDirection.BranchLeft,
                ductTypeBranch, branchTypeLeft, airFlowBranchLeft, widthBranchLeft, heightBranchLeft, diameterBranchLeft, roundingLeft);
            _local_right = BranchingElementsFactory.GetTJunctionBranch(this, _local_left, BranchDirection.BranchRight,
                ductTypeBranch, branchTypeRight, airFlowBranchRight, widthBranchRight, heightBranchRight, diameterBranchRight, roundingRight);
            this.AirFlowChanged += UpdateInletAirFlow;
            _local_right.AirFlowChanged += UpdateRightBranchAirFlow;
            _local_left.AirFlowChanged += UpdateLeftBranchAirFlow;
            _local_right.DuctTypeChanged += UpdateBranchRightDuctType;
            _local_left.DuctTypeChanged += UpdateBranchLeftDuctType;
            this.BranchRight.Elements._parent = this;
            this.BranchLeft.Elements._parent = this;
        }


        /// <summary>Trójnik typu T.</summary>
        public TJunction()
        {
            _type = ElementType.TJunction;
            this.DuctType = DuctType.Rectangular;
            this.Width = 400;
            this.Height = 200;
            this.Diameter = 315;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            base.AirFlow = 1200;
            this.IsIncluded = true;

            _local_right = BranchingElementsFactory.GetTJunctionBranch(this, _local_left, BranchDirection.BranchLeft,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0);
            _local_left = BranchingElementsFactory.GetTJunctionBranch(this, _local_right, BranchDirection.BranchRight,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0);
            _local_right = BranchingElementsFactory.GetTJunctionBranch(this, _local_left, BranchDirection.BranchLeft,
                DuctType.Rectangular, BranchType.Straight, 600, 160, 160, 200, 0);
            this.AirFlowChanged += UpdateInletAirFlow;
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

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 100)
                {
                    _width = 100;
                }
                else if (value < 2000)
                {
                    _width = value;
                }
                else
                {
                    _width = 2000;
                }
                OnDimensionsChanged();
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 100)
                {
                    _height = 100;
                }
                else if (value < 2000)
                {
                    _height = value;
                }
                else
                {
                    _height = 2000;
                }
                OnDimensionsChanged();
            }
        }

        public int Diameter
        {
            get
            {
                return _diameter;
            }
            set
            {
                if (value < 80)
                {
                    _diameter = 80;
                }
                else if (value < 1600)
                {
                    _diameter = value;
                }
                else
                {
                    _diameter = 1600;
                }
                OnDimensionsChanged();
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
                if (value < 1)
                {
                    base.AirFlow = 1;
                }
                else
                {
                    base.AirFlow = value;
                }
                OnAirFlowChanged();
            }
        }

        public double Velocity
        {
            get
            {
                if (DuctType == DuctType.Rectangular)
                {
                    return (AirFlow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _ductType;
            }
            set
            {
                _ductType = value;
                OnDuctTypeChanged();
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

            if (this.DuctType == DuctType.Rectangular && BranchRight.DuctType == DuctType.Rectangular)
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0, BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0,
                        this.Width / 1000.0 * this.Height / 1000.0, BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0, BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0,
                        this.Width / 1000.0 * this.Height / 1000.0, BranchRight.Rounding / 1000.0, 0, Turbulence.No);
                }
            }
            else if (this.DuctType == DuctType.Round && BranchRight.DuctType == DuctType.Rectangular)
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0,
                        BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0, Math.PI * 0.25 * Math.Pow(this.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                        BranchRight.Width / 1000.0 * BranchRight.Height / 1000.0,
                        BranchLeft.Width / 1000.0 * BranchLeft.Height / 1000.0, Math.PI * 0.25 * Math.Pow(this.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, 0, Turbulence.No);
                }
            }
            else if (this.DuctType == DuctType.Round && BranchRight.DuctType == DuctType.Round)
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                         Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter / 1000.0, 2), Math.PI * 0.25 * Math.Pow(this.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                         Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter  / 1000.0, 2), Math.PI * 0.25 * Math.Pow(this.Diameter / 1000.0, 2),
                        BranchRight.Rounding / 1000.0, 0, Turbulence.No);
                }
            }
            else
            {
                if (BranchRight.BranchType == BranchType.Rounded)
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                         Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter / 1000.0, 2), this.Width / 1000.0 *this.Height / 1000.0,
                        BranchRight.Rounding / 1000.0, BranchLeft.Rounding / 1000.0, Turbulence.No);
                }
                else
                {
                    lw = Function.Noise.TJunction(BranchDirection.Main, BranchRight.AirFlow, BranchLeft.AirFlow,
                         Math.PI * 0.25 * Math.Pow(BranchRight.Diameter / 1000.0, 2),
                         Math.PI * 0.25 * Math.Pow(BranchLeft.Diameter / 1000.0, 2), this.Width / 1000.0 *this.Height / 1000.0,
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

        private void UpdateInletAirFlow(object sender, EventArgs e)
        {
            _local_left.AirFlowChanged -= UpdateLeftBranchAirFlow;
            _local_right.AirFlowChanged -= UpdateRightBranchAirFlow;

            var n = (double)BranchRight.AirFlow / (BranchRight.AirFlow + BranchLeft.AirFlow);

            BranchRight.AirFlow = (int)Math.Ceiling(base.AirFlow * n);
            BranchLeft.AirFlow = base.AirFlow - BranchRight.AirFlow;

            _local_left.AirFlowChanged += UpdateLeftBranchAirFlow;
            _local_right.AirFlowChanged += UpdateRightBranchAirFlow;
        }

        private void UpdateRightBranchAirFlow(object sender, EventArgs e)
        {
            _local_left.AirFlowChanged -= UpdateLeftBranchAirFlow;

            if (BranchRight.AirFlow <= base.AirFlow)
            {
                BranchLeft.AirFlow = base.AirFlow - BranchRight.AirFlow;
            }
            else
            {
                BranchRight.AirFlow = base.AirFlow;
                BranchLeft.AirFlow = 0;
            }

            _local_left.AirFlowChanged += UpdateLeftBranchAirFlow;
        }

        private void UpdateLeftBranchAirFlow(object sender, EventArgs e)
        {
            _local_right.AirFlowChanged -= UpdateRightBranchAirFlow;

            if (BranchLeft.AirFlow <= base.AirFlow)
            {
                BranchRight.AirFlow = base.AirFlow - BranchLeft.AirFlow;
            }
            else
            {
                BranchLeft.AirFlow = base.AirFlow;
                BranchRight.AirFlow = 0;
            }

            _local_right.AirFlowChanged += UpdateRightBranchAirFlow;
        }

        public event EventHandler DimensionsChanged;

        public event EventHandler AirFlowChanged;

        public event EventHandler DuctTypeChanged;

        private void OnDimensionsChanged()
        {
            DimensionsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnAirFlowChanged()
        {
            AirFlowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnDuctTypeChanged()
        {
            DuctTypeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
