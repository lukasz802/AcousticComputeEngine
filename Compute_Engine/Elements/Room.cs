using Compute_Engine.Factories;
using System;
using System.Linq;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class Room : ElementsBase, IRoom
    {
        private static int _counter = 1;
        private static string _name = "room_";
        private ISoundAttenuator _local;
        private double _width;
        private double _height;
        private double _lenght;
        private NoiseLocation _noiseLocation;
        private double _distance;
        private int _temperature;
        private int _rh;
        private int _directionFactor;

        /// <summary>Pomieszczenie.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="temperature">Temperatura powietrza w pomieszczeniu [deg C].</param>
        /// <param name="relativeHumidity">Wilgotność względna powietrza w pomieszczeniu [%].</param>
        /// <param name="noiseLocation">Lokalizacja źródła hałasu</param>
        /// <param name="distance">Odległość między źródłem dźwięku a słuchaczem [m]</param>
        /// <param name="airFlow">Ilość powietrza nawiewana o pomieszczenia [m3/h].</param>
        /// <param name="width">Szerokość pomieszczenia [m].</param>
        /// <param name="height">Wysokość pomieszczenia [m].</param>
        /// <param name="lenght">Długość pomieszczenia [m].</param>
        /// <param name="octaveBand63Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 63Hz.</param>
        /// <param name="octaveBand125Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 125Hz.</param>
        /// <param name="octaveBand250Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 250Hz.</param>
        /// <param name="octaveBand500Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 500Hz.</param>
        /// <param name="octaveBand1000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 1000Hz.</param>
        /// <param name="octaveBand2000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 2000Hz.</param>
        /// <param name="octaveBand4000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 4000Hz.</param>
        /// <param name="octaveBand8000Hz">Współczynnik pochłaniania dźwięku przez pomieszczenie w paśmie 8000Hz.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Room(string name, string comments, int temperature, byte relativeHumidity, double distance, NoiseLocation noiseLocation, double width,
            double height, double lenght, double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz, bool include)
        {
            _type = ElementType.Room;
            this.Comments = comments;
            this.Name = name;
            base.IsIncluded = include;
            _temperature = temperature;
            _rh = relativeHumidity;
            _distance = distance;
            _noiseLocation = noiseLocation;
            _width = width;
            _height = height;
            _lenght = lenght;
            _local = EquipElementsFactory.GetRoomConstatnt(this, octaveBand63Hz, octaveBand125Hz, octaveBand250Hz, octaveBand500Hz,
                octaveBand1000Hz, octaveBand2000Hz, octaveBand4000Hz, octaveBand8000Hz);
            _counter = 1;
        }

        /// <summary>Pomieszczenie.</summary>
        public Room()
        {
            _type = ElementType.Room;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            base.IsIncluded = true;
            _temperature = 20;
            _rh = 40;
            _distance = 3;
            _noiseLocation = NoiseLocation.SurfaceCenter;
            _width = 10;
            _height = 3;
            _lenght = 12;
            _local = EquipElementsFactory.GetRoomConstatnt(this,
                Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[0], Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[1],
                Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[2], Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[3],
                Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[4], Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[5],
                Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[6], Transmission.RoomAbsorptionCoeffiecient(RoomType.Average)[7]);
        }

        private void UpdateOctaveBandAbsorption()
        {
            this.OctaveBandAbsorption.OctaveBand63 = _local.OctaveBand63;
            this.OctaveBandAbsorption.OctaveBand125 = _local.OctaveBand125;
            this.OctaveBandAbsorption.OctaveBand250 = _local.OctaveBand250;
            this.OctaveBandAbsorption.OctaveBand500 = _local.OctaveBand500;
            this.OctaveBandAbsorption.OctaveBand1k = _local.OctaveBand1k;
            this.OctaveBandAbsorption.OctaveBand2k = _local.OctaveBand2k;
            this.OctaveBandAbsorption.OctaveBand4k = _local.OctaveBand4k;
            this.OctaveBandAbsorption.OctaveBand8k = _local.OctaveBand8k;
        }

        public override bool IsIncluded
        {
            get
            {
                return base.IsIncluded;
            }
            set
            {
                base.IsIncluded = value;
            }
        }

        public int Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                if (value < -20)
                {
                    _temperature = -20;
                }
                else if (value < 50)
                {
                    _temperature = value;
                }
                else
                {
                    _temperature = 50;
                }
            }
        }

        public int RelativeHumidity
        {
            get
            {
                return _rh;
            }
            set
            {
                if (value < 0)
                {
                    _rh = 0;
                }
                else if (value < 100)
                {
                    _rh = value;
                }
                else
                {
                    _rh = 100;
                }
            }
        }

        public override int AirFlow
        {
            get
            {
                return base.AirFlow;
            }
            set
            {
                base.AirFlow = value;
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < 1)
                {
                    _width = 1;
                }
                else if (value < 100)
                {
                    _width = Math.Round(value, 2);
                }
                else
                {
                    _width = 100;
                }
                this.Distance = _distance;
                UpdateOctaveBandAbsorption();
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 1)
                {
                    _height = 1;
                }
                else if (value < 100)
                {
                    _height = Math.Round(value, 2);
                }
                else
                {
                    _height = 100;
                }
                this.Distance = _distance;
                UpdateOctaveBandAbsorption();
            }
        }

        public double Length
        {
            get
            {
                return _lenght;
            }
            set
            {
                if (value < 1)
                {
                    _lenght = 1;
                }
                else if (value < 100)
                {
                    _lenght = Math.Round(value, 2);
                }
                else
                {
                    _lenght = 100;
                }
                this.Distance = _distance;
                UpdateOctaveBandAbsorption();
            }
        }

        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                double[] temp = new double[] { _width, _height, _lenght };

                if (value < 0.1) { _distance = 0.1; }
                else if (value < temp.Min()) { _distance = value; }
                else { _distance = Math.Round(temp.Min(), 2); }
            }
        }

        public NoiseLocation NoiseLocation
        {
            get
            {
                return _noiseLocation;
            }
            set
            {
                _noiseLocation = value;

                switch (_noiseLocation)
                {
                    case NoiseLocation.RoomCenter:
                        _directionFactor = 1;
                        break;
                    case NoiseLocation.SurfaceCenter:
                        _directionFactor = 2;
                        break;
                    case NoiseLocation.SurfaceCorner:
                        _directionFactor = 8;
                        break;
                    case NoiseLocation.SurfaceEdge:
                        _directionFactor = 4;
                        break;
                }
            }
        }

        public ISoundAttenuator OctaveBandAbsorption
        {
            get
            {
                return _local;
            }
        }

        /// <summary>Oblicz tłumienność pomieszczenia.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (this.IsIncluded == true)
            {
                attn = Transmission.PointCorrection(_temperature, _rh, _directionFactor, _distance, new double[] { _local.OctaveBand63, _local.OctaveBand125, _local.OctaveBand250,
                    _local.OctaveBand500, _local.OctaveBand1k, _local.OctaveBand2k, _local.OctaveBand4k, _local.OctaveBand8k}, _width, _lenght, _height);
            }
            else
            {
                for (int i = 0; i < attn.Length; i++)
                {
                    attn[i] = -1000;
                }
            }

            return attn;
        }

        /// <summary>Oblicz szum generowany przez pomieszczenie.</summary>
        public override double[] Noise()
        {
            double[] lw = { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            return lw;
        }
    }
}
