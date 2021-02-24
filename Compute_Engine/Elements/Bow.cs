using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class Bow : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "bow_";
        private int _width;
        private int _height;
        private int _diameter;
        private double _rd;
        private double _rw;
        private int _liner_thickness;
        private bool _liner_check;
        private DuctType _duct_type;

        /// <summary>Łuk.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="ductType">Typ łuku.</param>
        /// <param name="airFlow">Przepływ powietrza przez element [m3/h].</param>
        /// <param name="width">Szerokość wlotwego króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="rw">Względny promień gięcia łuku (w odniesieniu do łuku o przekroju prostokątnym).</param>
        /// <param name="rd">Względny promień gięcia łuku (w odniesieniu do łuku o przekroju okrągłym).</param>
        /// <param name="linerThickness">Grubość izoloacji akustycznej łuku [mm].</param>
        /// <param name="linerCheck">Czy łuk jest zaizolowany akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Bow(string name, string comments, DuctType ductType, int airFlow, int width, int height, int diameter, double rw, double rd, int linerThckness, bool linerCheck, bool include)
        {
            _type = ElementType.Bow;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _rw = rw;
            _rd = rd;
            _liner_thickness = linerThckness;
            _liner_check = linerCheck;
            _counter = 1;
        }

        /// <summary>Łuk.</summary>
        public Bow()
        {
            _type = ElementType.Bow;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 500;
            this.IsIncluded = true;
            _duct_type = DuctType.Rectangular;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _rw = 1.5;
            _rd = 1.5;
            _liner_thickness = 25;
            _liner_check = false;
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                attn = Function.Attenuation.BowRectangular(_width / 1000.0);
            }
            else
            {
                if (_liner_check == true)
                {
                    attn = Function.Attenuation.BowRound(_liner_thickness / 10.0, _diameter / 1000.0);
                }
                else
                {
                    attn = Function.Attenuation.BowRound(0, _diameter / 1000.0);
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
                lw = Function.Noise.BowRectangular(this.AirFlow, _width, _height, (_rw * _width - _width / 2.0) / 1000.0);
            }
            else
            {
                lw = Function.Noise.BowRound(this.AirFlow, _diameter, (_rd * _diameter - _diameter / 2.0) / 1000.0);
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

        public double RD
        {
            get
            {
                return _rd;
            }
            set
            {
                if (_rd < 0.5)
                {
                    _rd = 0.5;
                }
                else if (_rd < 5.0)
                {
                    _rd = Math.Round(value, 2);
                }
                else
                {
                    _rd = 5.0;
                }
            }
        }

        public double RW
        {
            get
            {
                return _rw;
            }
            set
            {
                if (_rw < 0.5)
                {
                    _rw = 0.5;
                }
                else if (_rw < 5.0)
                {
                    _rw = Math.Round(value, 2);
                }
                else
                {
                    _rw = 5.0;
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

        public int LinerDepth
        {
            get
            {
                return _liner_thickness;
            }
            set
            {
                if (value < 25)
                {
                    _liner_thickness = 25;
                }
                else if (value < 75)
                {
                    _liner_thickness = value;
                }
                else
                {
                    _liner_thickness = 75;
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
                _duct_type = value;
            }
        }

    }
}
