using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class DuctConnection : IDuctConnection
    {
        private int _width;
        private int _height;
        private int _diameter;
        private int _airflow;
        private DuctType _ductType;

        public DuctConnection(DuctType ductType, int airFlow, int w, int h, int d)
        {
            DuctType = ductType;
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
                if (DuctType == DuctType.Rectangular)
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
                return _ductType;
            }
            set
            {
                _ductType = value;
                OnDuctTypeChanged();
            }
        }

        public int AirFlow
        {
            get
            {
                return _airflow;
            }
            set
            {
                if (value < 0)
                {
                    _airflow = 0;
                }
                else
                {
                    _airflow = value;
                }
                OnAirFlowChanged();
            }
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
