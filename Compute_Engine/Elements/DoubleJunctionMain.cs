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
    public class DoubleJunctionMain : IRectangular, IRound, IVelocity
    {
        private DoubleJunction _local_djunction = null;
        private JunctionConnectionSide? _junction_connection_side = null;

        internal DoubleJunctionMain(DoubleJunction baseDoubleJunction, JunctionConnectionSide junctionConnectionSide)
        {
            _junction_connection_side = junctionConnectionSide;
            _local_djunction = baseDoubleJunction;
        }

        public int Width
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.Width;
                }
                else
                {
                    return _local_djunction.Container.Out.Width;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.Container.In.Width = value;
                }
                else
                {
                    _local_djunction.Container.Out.Width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.Height;
                }
                else
                {
                    return _local_djunction.Container.Out.Height;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.Container.In.Height = value;
                }
                else
                {
                    _local_djunction.Container.Out.Height = value;
                }
            }
        }

        public int Diameter
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.Diameter;
                }
                else
                {
                    return _local_djunction.Container.Out.Diameter;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.Container.In.Diameter = value;
                }
                else
                {
                    _local_djunction.Container.Out.Diameter = value;
                }
            }
        }

        public double Velocity
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    if (_local_djunction.Container.In.DuctType == DuctType.Rectangular)
                    {
                        return (_local_djunction.Container.In.AirFlow / 3600.0) / ((_local_djunction.Container.In.Width / 1000.0)
                            * (_local_djunction.Container.In.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_djunction.Container.In.AirFlow / 3600.0) /
                            (0.25 * Math.PI * Math.Pow(_local_djunction.Container.In.Diameter / 1000.0, 2));
                    }
                }
                else
                {
                    if (_local_djunction.Container.Out.DuctType == DuctType.Rectangular)
                    {
                        return (_local_djunction.Container.Out.AirFlow / 3600.0) /
                            ((_local_djunction.Container.Out.Width / 1000.0) * (_local_djunction.Container.Out.Height / 1000.0));
                    }
                    else
                    {
                        return (_local_djunction.Container.Out.AirFlow / 3600.0) /
                            (0.25 * Math.PI * Math.Pow(_local_djunction.Container.Out.Diameter / 1000.0, 2));
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
                    return _local_djunction.Container.In.DuctType;
                }
                else
                {
                    return _local_djunction.Container.Out.DuctType;
                }
            }
            set
            {
                _local_djunction.Container.In.DuctType = value;
                _local_djunction.Container.Out.DuctType = value;
            }
        }

        public int AirFlow
        {
            get
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    return _local_djunction.Container.In.AirFlow;
                }
                else
                {
                    return _local_djunction.Container.Out.AirFlow;
                }
            }
            set
            {
                if (_junction_connection_side == JunctionConnectionSide.Inlet)
                {
                    _local_djunction.BranchRight.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    _local_djunction.BranchLeft.JunctionConnectionSide = JunctionConnectionSide.Inlet;
                    _local_djunction.AirFlow = value;
                }
                else
                {
                    _local_djunction.BranchRight.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    _local_djunction.BranchLeft.JunctionConnectionSide = JunctionConnectionSide.Outlet;
                    _local_djunction.AirFlow = value + _local_djunction.BranchRight.AirFlow + _local_djunction.BranchLeft.AirFlow;
                }
            }
        }
    }
}
