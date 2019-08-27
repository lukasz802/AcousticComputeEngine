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
    public class Grill : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "grill_";
        private int _diameter;
        private int _width;
        private int _height;
        private int _eff_area;
        private GrillType _grill_type;
        private GrillLocation _grill_location;
        private GrillOrifice _local = null;

        /// <summary>Kratka wentylacyjna nawiewna/wyciągowa.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="grillType">Typ kratki wentylacyjnej.</param>
        /// <param name="grillLocation">Lokalizacja kratki wentylacyjnej.</param>
        /// <param name="airFlow">Przepływ powietrza przez kratkę [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="orificeDepth">Głębokość otworu żaluzjowego [mm].</param>
        /// <param name="orificeHeight">Wysokość otworu żaluzjowego [mm].</param>
        /// <param name="percEffectiveArea">Procentowy udział efektywnej powierzchni netto w stosunku do całkowitej powierzchni przekroju poprzecznego kratki [%]</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Grill(string name, string comments, GrillType grillType, GrillLocation grillLocation, int airFlow, int width, int height,
             int diameter, int orificeDepth, int orificeHeight, int percEffectiveArea, bool include)
        {
            _type = ElementType.Grill;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _grill_type = grillType;
            _grill_location = grillLocation;
            _width = width;
            _height = height;
            _diameter = diameter;
            _eff_area = percEffectiveArea;
            _local = new GrillOrifice(orificeHeight, orificeDepth);
            _counter = 1;
        }

        /// <summary>Kratka wentylacyjna nawiewna/wyciągowa.</summary>
        public Grill()
        {
            _type = ElementType.Grill;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 400;
            this.IsIncluded = true;
            _grill_type = GrillType.RectangularSupplyWire;
            _grill_location = GrillLocation.FlushWall;
            _width = 250;
            _height = 150;
            _diameter = 200;
            _eff_area = 70;
            _local = new GrillOrifice(20, 20);
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];
            DuctType duct_type;

            if ((Convert.ToInt16(_grill_type) <= 3) || ((Convert.ToInt16(_grill_type) >= 8) && (Convert.ToInt16(_grill_type) <= 11)))
            {
                duct_type = DuctType.Round;
            }
            else
            {
                duct_type = DuctType.Rectangular;
            }

            if (duct_type == DuctType.Rectangular)
            {
                attn = Function.Attenuation.Grill(_grill_location, _width / 1000.0 * _height / 1000.0);
            }
            else
            {
                attn = Function.Attenuation.Grill(_grill_location, Math.PI * 0.25 * Math.Pow(_diameter / 1000.0, 2));
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];
            DuctType duct_type;

            if ((Convert.ToInt16(_grill_type) <= 3) || ((Convert.ToInt16(_grill_type) >= 8) && (Convert.ToInt16(_grill_type) <= 11)))
            {
                duct_type = DuctType.Round;
            }
            else
            {
                duct_type = DuctType.Rectangular;
            }

            if (duct_type == DuctType.Rectangular)
            {
                lw = Function.Noise.Grill(_grill_type, base.AirFlow, _width / 1000.0 * _height / 1000.0, _local.Depth / 10.0,
                    _width / 1000.0, _local.Height / 10.0, _eff_area);
            }
            else
            {
                lw = Function.Noise.Grill(_grill_type, base.AirFlow, Math.PI * 0.25 * Math.Pow(_diameter / 1000.0, 2), _local.Depth / 10.0,
                    _width / 1000.0, _local.Height / 10.0, _eff_area);
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
                DuctType duct_type;

                if ((Convert.ToInt16(_grill_type) <= 3) || ((Convert.ToInt16(_grill_type) >= 8) && (Convert.ToInt16(_grill_type) <= 11)))
                {
                    duct_type = DuctType.Round;
                }
                else
                {
                    duct_type = DuctType.Rectangular;
                }

                if (duct_type == DuctType.Rectangular)
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

        public int EffectiveArea
        {
            get
            {
                return _eff_area;
            }
            set
            {
                if (value < 10)
                {
                    _eff_area = 10;
                }
                else if (value < 100)
                {
                    _eff_area = value;
                }
                else
                {
                    _eff_area = 100;
                }
            }
        }

        public GrillType GrillType
        {
            get
            {
                return _grill_type;
            }
            set
            {
                _grill_type = value;
            }
        }

        public GrillLocation GrillLocation
        {
            get
            {
                return _grill_location;
            }
            set
            {
                _grill_location = value;
            }
        }

        public GrillOrifice Orifice
        {
            get
            {
                return _local;
            }
        }
    }
}
