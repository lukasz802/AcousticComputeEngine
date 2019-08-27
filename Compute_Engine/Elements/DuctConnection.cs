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
    public class DuctConnection : IRectangular, IRound, IVelocity
    {
        private DuctType _duct_type;
        private int _width;
        private int _height;
        private int _diameter;
        private int _airflow;

        internal DuctConnection(DuctType ductType, int airFlow, int w, int h, int d)
        {
            _duct_type = ductType;
            _airflow = airFlow;
            _width = w;
            _height = h;
            _diameter = d;
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

        public double Velocity
        {
            get
            {
                if (_duct_type == DuctType.Rectangular)
                {
                    return (_airflow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (_airflow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                return _duct_type;
            }
            set
            {
                _duct_type = value;
            }
        }

        internal int AirFlow
        {
            get
            {
                return _airflow;
            }
            set
            {
                if (value < 1)
                {
                    _airflow = 1;
                }
                else
                {
                    _airflow = value;
                }
            }
        }

        internal event EventHandler DimensionsChanged;

        protected void OnDimensionsChanged()
        {
            DimensionsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
