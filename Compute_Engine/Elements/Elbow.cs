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
    public class Elbow : ElementsBase, IRectangular, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "elb_";
        private int _width;
        private int _height;
        private int _rnd;
        private byte _vanes_number;
        private bool _liner_check;
        private TurnigVanes _tuning_vanes;
        private ElbowType _elbow_type;

        /// <summary>Kolano.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="elbowType">Typ kolana.</param>
        /// <param name="airFlow">Przepływ powietrza przez element [m3/h].</param>
        /// <param name="width">Szerokość wlotwego króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="vanesNumber">Liczba kierownic powietrza.</param>
        /// <param name="turnigVanes">Czy kolano posiada kierownice powietrza.</param>
        /// <param name="rounding">Promień zaokrąglenia kolana [mm].</param>
        /// <param name="linerCheck">Czy kolano jest zaizolowany akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Elbow(string name, string comments, int airFlow, ElbowType elbowType, TurnigVanes turnigVanes, byte vanesNumber,
                 int width, int height, int rounding, bool linerCheck, bool include)
        {
            _type = ElementType.Elbow;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _elbow_type = elbowType;
            _tuning_vanes = TurnigVanes;
            _width = width;
            _height = height;
            _vanes_number = vanesNumber;
            _rnd = rounding;
            _liner_check = linerCheck;
            _counter = 1;
        }

        /// <summary>Kolano.</summary>
        public Elbow()
        {
            _type = ElementType.Elbow;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 800;
            this.IsIncluded = true;
            _elbow_type = ElbowType.Straight;
            _tuning_vanes = TurnigVanes.No;
            _width = 400;
            _height = 200;
            _vanes_number = 3;
            _rnd = 0;
            _liner_check = false;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_liner_check == true)
            {
                attn = Function.Attenuation.Elbow(_tuning_vanes, 10.0, _width / 1000.0);
            }
            else
            {
                attn = Function.Attenuation.Elbow(_tuning_vanes, 0, _width / 1000.0);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_elbow_type == ElbowType.Rounded)
            {
                lw = Function.Noise.Elbow(_tuning_vanes, _vanes_number, this.AirFlow, _width / 1000.0, _height / 1000.0, _rnd / 1000.0);
            }
            else
            {
                lw = Function.Noise.Elbow(_tuning_vanes, _vanes_number, this.AirFlow, _width / 1000.0, _height / 1000.0, 0);
            }
            return lw;
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
                this.Rouning = _rnd;
                this.VanesNumber = _vanes_number;
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
                this.Rouning = _rnd;
                this.VanesNumber = _vanes_number;
            }
        }

        public double Velocity
        {
            get
            {
                return (this.AirFlow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0));
            }
        }

        public byte VanesNumber
        {
            get
            {
                return _vanes_number;
            }
            set
            {
                if (!(_rnd == 0))
                {
                    if (value < 1)
                    {
                        _vanes_number = 1;
                    }
                    else if (Math.Round(2.13 * Math.Pow((_rnd / 1000.0 / _width / 1000.0), -1) - 1) >= value)
                    {
                        _vanes_number = value;
                    }
                    else
                    {
                        _vanes_number = (byte)Math.Round(2.13 * Math.Pow((_rnd / 1000.0 / _width / 1000.0), -1) - 1);
                    }
                }
                else
                {
                    double dh = 2 * _height / 1000.0 * _width / 1000.0 / (_height / 1000.0 + _width / 1000.0);

                    if (value < 1)
                    {
                        _vanes_number = 1;
                    }
                    else if (Math.Round(2.13 * Math.Pow((0.35 * dh / (_width / 1000.0 * Math.Pow(2, 0.5))), (-1)) - 1) >= value)
                    {
                        _vanes_number = value;
                    }
                    else
                    {
                        _vanes_number = (byte)Math.Round(2.13 * Math.Pow((0.35 * dh / (_width / 1000.0 * Math.Pow(2, 0.5))), (-1)) - 1);
                    }
                }

            }
        }

        public int Rouning
        {
            get
            {
                return _rnd;
            }
            set
            {
                if (value < 0)
                {
                    _rnd = 0;
                }
                else if (value < Math.Ceiling(0.6 * _width))
                {
                    _rnd = value;
                }
                else
                {
                    _rnd = (int)Math.Ceiling(0.6 * _width);
                }
                this.VanesNumber = _vanes_number;
            }
        }

        public bool LinerCheck
        {
            get
            {
                return _liner_check;
            }
            set
            {
                _liner_check = value;
            }
        }

        public TurnigVanes TurnigVanes
        {
            get
            {
                return _tuning_vanes;
            }
            set
            {
                _tuning_vanes = value;
            }
        }

        public ElbowType ElbowType
        {
            get
            {
                return _elbow_type;
            }
            set
            {
                _elbow_type = value;
            }
        }
    }
}
