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
    public class Duct : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "dct_";
        private int _width;
        private int _height;
        private int _diameter;
        private double _lenght;
        private readonly int _liner_thickness;
        private bool _liner_check;
        private DuctType _duct_type;

        /// <summary>Kanał prosty.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductType">Typ króćca podłączeniowego.</param>
        /// <param name="airFlow">Przepływ powietrza przez kanał [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="lenght">Długość kanału [m].</param>
        /// <param name="linerThickness">Grubość izoloacji akustycznej kanału [mm].</param>
        /// <param name="linerCheck">Czy kanał jest zaizolowany akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Duct(string name, string comments, DuctType ductType, int airFlow, int width, int height, int diameter, double lenght, int linerThickness, bool linerCheck, bool include)
        {
            _type = ElementType.Duct;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _lenght = lenght;
            _liner_thickness = linerThickness;
            _liner_check = linerCheck;
            _counter = 1;
        }

        /// <summary>Kanał prosty.</summary>
        public Duct()
        {
            _type = ElementType.Duct;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 500;
            this.IsIncluded = true;
            _duct_type = DuctType.Rectangular;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _lenght = 1;
            _liner_thickness = 25;
            _liner_check = false;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                if (_liner_check == true)
                {
                    attn = Function.Attenuation.DuctRectanguar(_liner_thickness / 10.0, _width / 1000.0, _height / 1000.0, _lenght);
                }
                else
                {
                    attn = Function.Attenuation.DuctRectanguar(0, _width / 1000.0, _height / 1000.0, _lenght);
                }
            }
            else
            {
                if (_liner_check == true)
                {
                    attn = Function.Attenuation.DuctRound(_liner_thickness / 10, _diameter / 1000, _lenght);
                }
                else
                {
                    attn = Function.Attenuation.DuctRound(0, _diameter / 1000, _lenght);
                }
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                lw = Function.Noise.Duct(this.AirFlow, _width / 1000.0 * _height / 1000.0);
            }
            else
            {
                lw = Function.Noise.Duct(this.AirFlow, Math.Pow(_diameter / 1000.0, 2) * 0.25 * Math.PI);
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

        public double Lenght
        {
            get
            {
                return _lenght;
            }
            set
            {
                if (value < 0.1)
                {
                    _lenght = 0.1;
                }
                else
                {
                    _lenght = Math.Round(value, 2);
                }
            }
        }

        public int LinerDepth
        {
            get
            {
                return _liner_thickness;
            }
            set
            {
                if (this.DuctType == DuctType.Round)
                {
                    if (value < 25)
                    {
                        _width = 25;
                    }
                    else if (value < 75)
                    {
                        _width = value;
                    }
                    else
                    {
                        _width = 75;
                    }
                }
                else
                {
                    if (value < 25)
                    {
                        _width = 25;
                    }
                    else if (value < 50)
                    {
                        _width = value;
                    }
                    else
                    {
                        _width = 50;
                    }
                }
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

        public DuctType DuctType
        {
            get
            {
                return _duct_type;
            }
            set
            {
                if (value == DuctType.Rectangular && this.LinerDepth > 50)
                {
                    this.LinerDepth = 50;
                }

                _duct_type = value;
            }
        }
    }
}
