using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{
    public abstract class BranchElementTemplate : IDuctConnection
    {
        private int _airflow_branch;
        private int _width_branch;
        private int _height_branch;
        private int _diameter_branch;
        private int _rnd_branch;
        private DuctType _duct_type_branch;
        private BranchType _branch_type;

        public event EventHandler DimensionsChanged;
        public event EventHandler AirFlowChanged;
        public event EventHandler DuctTypeChanged;

        public BranchElementTemplate(DuctType ductTypeBranch, BranchType branchType, int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int rounding)
        {
            _duct_type_branch = ductTypeBranch;
            _airflow_branch = airFlowBranch;
            _width_branch = widthBranch;
            _height_branch = heightBranch;
            _diameter_branch = diameterBranch;
            _rnd_branch = rounding;
            _branch_type = branchType;
            Elements = new ElementsCollection();
        }

        public int AirFlow
        {
            get
            {
                return _airflow_branch;
            }
            set
            {
                _airflow_branch = value;
                OnAirFlowChanged();
            }
        }

        public int Width
        {
            get
            {
                return _width_branch;
            }
            set
            {
                if (value < 100)
                {
                    _width_branch = 100;
                }
                else if (value < 2000)
                {
                    _width_branch = value;
                }
                else
                {
                    _width_branch = 2000;
                }
                OnDimensionsChanged();
            }
        }

        public int Height
        {
            get
            {
                return _height_branch;
            }
            set
            {
                if (value < 100)
                {
                    _height_branch = 100;
                }
                else if (value < 2000)
                {
                    _height_branch = value;
                }
                else
                {
                    _height_branch = 2000;
                }
                OnDimensionsChanged();
            }
        }

        public int Diameter
        {
            get
            {
                return _diameter_branch;
            }
            set
            {
                if (value < 80)
                {
                    _diameter_branch = 80;
                }
                else if (value < 1600)
                {
                    _diameter_branch = value;
                }
                else
                {
                    _diameter_branch = 1600;
                }
                OnDimensionsChanged();
            }
        }

        public int Rounding
        {
            get
            {
                return _rnd_branch;
            }
            set
            {
                if (value < 0)
                {
                    _rnd_branch = 0;
                }
                else if (value < Math.Ceiling(0.6 * _width_branch))
                {
                    _rnd_branch = value;
                }
                else
                {
                    _rnd_branch = (int)Math.Ceiling(0.6 * _width_branch);
                }
            }
        }

        public BranchType BranchType
        {
            get
            {
                return _branch_type;
            }
            set
            {
                _branch_type = value;
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _duct_type_branch;
            }
            set
            {
                _duct_type_branch = value;
                OnDuctTypeChanged();
            }
        }

        public double Velocity
        {
            get
            {
                if (_duct_type_branch == DuctType.Rectangular)
                {
                    return (_airflow_branch / 3600.0) / ((_width_branch / 1000.0) * (_height_branch / 1000.0));
                }
                else
                {
                    return (_airflow_branch / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter_branch / 1000.0, 2));
                }
            }
        }

        public ElementsCollection Elements { get; }

        private void OnAirFlowChanged()
        {
            AirFlowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnDimensionsChanged()
        {
            DimensionsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnDuctTypeChanged()
        {
            DuctTypeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
