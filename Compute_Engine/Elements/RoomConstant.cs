using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class RoomConstant : IOctaveBandAbsorption
    {
        private double _octaveBand63Hz;
        private double _octaveBand125Hz;
        private double _octaveBand250Hz;
        private double _octaveBand500Hz;
        private double _octaveBand1000Hz;
        private double _octaveBand2000Hz;
        private double _octaveBand4000Hz;
        private double _octaveBand8000Hz;
        private readonly Room _room = null;

        internal RoomConstant(Room room, double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz)
        {
            _room = room;
            _octaveBand63Hz = octaveBand63Hz;
            _octaveBand125Hz = octaveBand125Hz;
            _octaveBand250Hz = octaveBand250Hz;
            _octaveBand500Hz = octaveBand500Hz;
            _octaveBand1000Hz = octaveBand1000Hz;
            _octaveBand2000Hz = octaveBand2000Hz;
            _octaveBand4000Hz = octaveBand4000Hz;
            _octaveBand8000Hz = octaveBand8000Hz;
        }

        internal double[] MaxAbsorption(Room room)
        {
            double s, mfp;
            double[] m = new double[8];

            s = 2 * (UnitConvertion.MToFt(room.Width) * UnitConvertion.MToFt(room.Lenght)) + 2 * (UnitConvertion.MToFt(room.Lenght) * UnitConvertion.MToFt(room.Height))
                + 2 * (UnitConvertion.MToFt(room.Width) * UnitConvertion.MToFt(room.Height));
            mfp = 4 * (UnitConvertion.MToFt(room.Width) * UnitConvertion.MToFt(room.Height) * UnitConvertion.MToFt(room.Lenght)) / s;

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = Transmission.M_coeff(room.Temperature, room.RelativeHumidity)[i] / UnitConvertion.MToFt(1) * mfp;
            }

            return m;
        }

        public double OctaveBand63Hz
        {
            get { return _octaveBand63Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand63Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[0], 2)))
                {
                    _octaveBand63Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand63Hz = 0.99 - Math.Round(MaxAbsorption(_room)[0], 2);
                }
            }
        }

        public double OctaveBand125Hz
        {
            get { return _octaveBand125Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand125Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[1], 2)))
                {
                    _octaveBand125Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand125Hz = 0.99 - Math.Round(MaxAbsorption(_room)[1], 2);
                }
            }
        }

        public double OctaveBand250Hz
        {
            get { return _octaveBand250Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand250Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[2], 2)))
                {
                    _octaveBand250Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand250Hz = 0.99 - Math.Round(MaxAbsorption(_room)[2], 2);
                }
            }
        }

        public double OctaveBand500Hz
        {
            get { return _octaveBand500Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand500Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[3], 2)))
                {
                    _octaveBand500Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand500Hz = 0.99 - Math.Round(MaxAbsorption(_room)[3], 2);
                }
            }
        }

        public double OctaveBand1000Hz
        {
            get { return _octaveBand1000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand1000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[4], 2)))
                {
                    _octaveBand1000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand1000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[4], 2);
                }
            }
        }

        public double OctaveBand2000Hz
        {
            get { return _octaveBand2000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand2000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[5], 2)))
                {
                    _octaveBand2000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand2000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[5], 2);
                }
            }
        }

        public double OctaveBand4000Hz
        {
            get { return _octaveBand4000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand4000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[6], 2)))
                {
                    _octaveBand4000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand4000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[6], 2);
                }
            }
        }

        public double OctaveBand8000Hz
        {
            get { return _octaveBand8000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand8000Hz = 0;
                }
                else if (value < (0.99 - Math.Round(MaxAbsorption(_room)[7], 2)))
                {
                    _octaveBand8000Hz = Math.Round(value, 2);
                }
                else
                {
                    _octaveBand8000Hz = 0.99 - Math.Round(MaxAbsorption(_room)[7], 2);
                }
            }
        }
    }
}
