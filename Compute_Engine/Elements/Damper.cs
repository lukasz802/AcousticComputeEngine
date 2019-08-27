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
    public class Damper : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "dmp_";
        private int _diameter;
        private int _width;
        private int _height;
        private byte _blade_number;
        private byte _blade_angle;
        private DuctType _duct_type;
        private DamperType _damper_type;

        /// <summary>Przepustnica.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductType">Typ króćca podłączeniowego.</param>
        /// <param name="damperType">Typ przepustnicy.</param>
        /// <param name="airFlow">Przepływ powietrza przez przepustnicę [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="bladeAngle">Kąt nachylenia łopatek przepustnicy.</param>
        /// <param name="bladeNumber">Liczba łopatek.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Damper(string name, string comments, DamperType damperType, DuctType ductType, int airFlow, int width, int height,
             int diameter, byte bladeNumber, byte bladeAngle, bool include)
        {
            _type = ElementType.Damper;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _damper_type = damperType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _duct_type = ductType;
            _blade_number = bladeNumber;
            _blade_angle = bladeAngle;
            _counter = 1;
        }

        /// <summary>Przepustnica.</summary>
        public Damper()
        {
            _type = ElementType.Damper;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            this.AirFlow = 800;
            this.IsIncluded = true;
            _damper_type = DamperType.SingleBlade;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _duct_type = DuctType.Rectangular;
            _blade_number = 1;
            _blade_angle = 0;
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

            if (_duct_type == DuctType.Rectangular)
            {
                if (_damper_type == DamperType.SingleBlade)
                {
                    lw = Function.Noise.DamperRectangular(1, _blade_angle, base.AirFlow, _width / 1000.0, _height / 1000.0);
                }
                else
                {
                    lw = Function.Noise.DamperRectangular(_blade_number, _blade_angle, base.AirFlow, _width / 1000.0, _height / 1000.0);
                }
            }
            else
            {
                lw = Function.Noise.DamperRound(_blade_angle, base.AirFlow, _diameter / 1000.0);
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
            }
        }

        public double Velocity
        {
            get
            {
                if (_duct_type == DuctType.Rectangular)
                {
                    return (this.AirFlow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0));
                }
                else
                {
                    return (this.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2));
                }
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
            }
        }

        public byte BladeNumber
        {
            get
            {
                return _blade_number;
            }
            set
            {
                if (_damper_type == DamperType.SingleBlade || value <= 1)
                {
                    _blade_number = 1;
                    _damper_type = DamperType.SingleBlade;
                    this.BladeAngle = _blade_angle;
                }
                else if (value < (byte)Math.Ceiling(_width / 20.0))
                {
                    _blade_number = value;
                }
                else
                {
                    _blade_number = (byte)Math.Ceiling(_width / 20.0);
                }
            }
        }

        public byte BladeAngle
        {
            get
            {
                return _blade_angle;
            }
            set
            {
                byte temp;
                if (_damper_type == DamperType.SingleBlade) { temp = 70; }
                else { temp = 80; }

                if (value < 0)
                {
                    _blade_angle = 0;
                }
                else if (value < temp)
                {
                    _blade_angle = temp;
                }
                else
                {
                    _blade_angle = temp;
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

        public DamperType DamperType
        {
            get
            {
                return _damper_type;
            }
            set
            {
                _damper_type = value;

                if (_damper_type == DamperType.SingleBlade) { _blade_number = 1; }
                this.BladeAngle = _blade_angle;
            }
        }
    }
}
