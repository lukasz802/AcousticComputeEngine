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
    public class Silencer : ElementsBase, IRectangular, IRound, IVelocity
    {
        private static int _counter = 1;
        private static string _name = "sln_";
        private SoundAttenuation _local = null;
        private int _width;
        private int _height;
        private int _diameter;
        private int _eff_area;
        private DuctType _duct_type;
        private SilencerType _silencer_type;
        private double _lenght;

        /// <summary>Tłumik akstyczny.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="silencerType">Typ tłumika.</param>
        /// <param name="ductType">Typ króćca podłączeniowego.</param>
        /// <param name="airFlow">Przepływ powietrza przez tłumik [m3/h].</param>
        /// <param name="width">Szerokość króćca przyłączeniowego [mm].</param>
        /// <param name="height">Wysokość króćca przyłączeniowego [mm].</param>
        /// <param name="diameter">Średnica króćca przyłączeniowego [mm].</param>
        /// <param name="lenght">Długość tłumika [m].</param>
        /// <param name="octaveBand63Hz">Tłumienie akustyczne w paśmie 63Hz [dB].</param>
        /// <param name="octaveBand125Hz">Tłumienie akustyczne w paśmie 125Hz [dB].</param>
        /// <param name="octaveBand250Hz">Tłumienie akustyczne w paśmie 250Hz [dB].</param>
        /// <param name="octaveBand500Hz">Tłumienie akustyczne w paśmie 500Hz [dB].</param>
        /// <param name="octaveBand1000Hz">Tłumienie akustyczne w paśmie 1000Hz [dB].</param>
        /// <param name="octaveBand2000Hz">Tłumienie akustyczne w paśmie 2000Hz [dB].</param>
        /// <param name="octaveBand4000Hz">Tłumienie akustyczne w paśmie 4000Hz [dB].</param>
        /// <param name="octaveBand8000Hz">Tłumienie akustyczne w paśmie 8000Hz [dB].</param>
        /// <param name="percEffectiveArea">Procentowy udział efektywnej powierzchnia netto króćca przyłączeniowego [%].</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Silencer(string name, string comments, SilencerType silencerType, DuctType ductType, int airFlow, int width, int height,
             int diameter, double lenght, int octaveBand63Hz, int octaveBand125Hz, int octaveBand250Hz, int octaveBand500Hz,
             int octaveBand1000Hz, int octaveBand2000Hz, int octaveBand4000Hz, int octaveBand8000Hz, int percEffectiveArea,
             bool include)
        {
            _type = ElementType.Silencer;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _silencer_type = silencerType;
            _duct_type = ductType;
            _width = width;
            _height = height;
            _diameter = diameter;
            _lenght = lenght;
            _eff_area = percEffectiveArea;
            _local = new SoundAttenuation(octaveBand63Hz, octaveBand125Hz, octaveBand250Hz, octaveBand500Hz,
             octaveBand1000Hz, octaveBand2000Hz, octaveBand4000Hz, octaveBand8000Hz);
            _counter = 1;
        }

        /// <summary>Tłumik akstyczny.</summary>
        public Silencer()
        {
            _type = ElementType.Silencer;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 500;
            this.IsIncluded = true;
            _silencer_type = SilencerType.Absorptive;
            _duct_type = DuctType.Round;
            _width = 200;
            _height = 200;
            _diameter = 250;
            _lenght = 0.6;
            _eff_area = 100;
            _local = new SoundAttenuation(1, 2, 5, 9, 16, 13, 5, 6);
            _name = this.Name;
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
                    return (this.AirFlow / 3600.0) / ((_width / 1000.0) * (_height / 1000.0) * _eff_area / 100.0);
                }
                else
                {
                    return (this.AirFlow / 3600.0) / (0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2) * _eff_area / 100.0);
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
                else if (value < 3.0)
                {
                    _lenght = value;
                }
                else
                {
                    _lenght = 3.0;
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

        public SilencerType SilencerType
        {
            get
            {
                return _silencer_type;
            }
            set
            {
                _silencer_type = value;
            }
        }

        public SoundAttenuation OctaveBandAttenuation
        {
            get
            {
                return _local;
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = { _local.OctaveBand63Hz, _local.OctaveBand125Hz, _local.OctaveBand250Hz, _local.OctaveBand500Hz, _local.OctaveBand1000Hz,
                _local.OctaveBand2000Hz, _local.OctaveBand4000Hz, _local.OctaveBand8000Hz };
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = new double[8];

            if (_duct_type == DuctType.Rectangular)
            {
                lw = Function.Noise.Silencer(base.AirFlow, _height / 1000.0 * _width / 1000.0, _eff_area);
            }
            else
            {
                if (_silencer_type == SilencerType.ParallelBaffles)
                {
                    lw = Function.Noise.Silencer(base.AirFlow, 0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2), _eff_area);
                }
                else
                {
                    lw = Function.Noise.Silencer(base.AirFlow, 0.25 * Math.PI * Math.Pow(_diameter / 1000.0, 2), 100);
                }
            }
            return lw;
        }
    }
}
