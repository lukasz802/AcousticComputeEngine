using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class JunctionMain : IRectangular, IRound, IVelocity
    {
        private Junction _local_junction = null;
        private JunctionConnectionSide? _junction_connection_side = null;

        internal JunctionMain(Junction baseJunction, JunctionConnectionSide junctionConnectionSide)
        {
            _junction_connection_side = junctionConnectionSide;
            _local_junction = baseJunction;
        }

        public int Width
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.Width;
                }
                else
                {
                    return _local_junction.Branch.Out.Width;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.In.Width = value;
                }
                else
                {
                    _local_junction.Branch.Out.Width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.Height;
                }
                else
                {
                    return _local_junction.Branch.Out.Height;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.In.Height = value;
                }
                else
                {
                    _local_junction.Branch.Out.Height = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.Diameter;
                }
                else
                {
                    return _local_junction.Branch.Out.Diameter;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.In.Diameter = value;
                }
                else
                {
                    _local_junction.Branch.Out.Diameter = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    if (_local_junction.Branch.In.DuctType == DuctType.Rectangular)
                    {
                        return (_local_junction.Branch.In.AirFlow / 3600.0) / ((_local_junction.Branch.In.Width / 1000.0) * (_local_junction.Branch.In.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_junction.Branch.In.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_local_junction.Branch.In.Diameter / 1000.0, 2));
                    }
                }
                else
                {
                    if (_local_junction.Branch.Out.DuctType == DuctType.Rectangular)
                    {
                        return (_local_junction.Branch.Out.AirFlow / 3600.0) / ((_local_junction.Branch.Out.Width / 1000.0) * (_local_junction.Branch.Out.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_junction.Branch.Out.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_local_junction.Branch.Out.Diameter / 1000.0, 2));
                    }
                }
            }
        }

        public DuctType DuctType
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.DuctType;
                }
                else
                {
                    return _local_junction.Branch.Out.DuctType;
                }
            }
            set
            {
                _local_junction.Branch.In.DuctType = value;
                _local_junction.Branch.Out.DuctType = value;
            }
        }

        public int AirFlow
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_junction.Branch.In.AirFlow;
                }
                else
                {
                    return _local_junction.Branch.Out.AirFlow;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_junction.Branch.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    _local_junction.AirFlow = value;
                }
                else
                {
                    _local_junction.Branch.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    _local_junction.AirFlow = value + _local_junction.Branch.AirFlow;
                }
            }
        }
    }
}
