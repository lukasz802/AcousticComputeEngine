using System;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class RoomConstant : ISoundAttenuator
    {
        private double _octaveBand63Hz;
        private double _octaveBand125Hz;
        private double _octaveBand250Hz;
        private double _octaveBand500Hz;
        private double _octaveBand1000Hz;
        private double _octaveBand2000Hz;
        private double _octaveBand4000Hz;
        private double _octaveBand8000Hz;
        private readonly Room _room;

        public RoomConstant(Room room, double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
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

        public double TotalAttenution()
        {
            throw new NotImplementedException();
        }

        public double OctaveBand63
        {
            get { return _octaveBand63Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand63Hz, value, MaxAbsorption(_room)[0]);
            }
        }

        public double OctaveBand125
        {
            get { return _octaveBand125Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand125Hz, value, MaxAbsorption(_room)[1]);
            }
        }

        public double OctaveBand250
        {
            get { return _octaveBand250Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand250Hz, value, MaxAbsorption(_room)[2]);
            }
        }

        public double OctaveBand500
        {
            get { return _octaveBand500Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand500Hz, value, MaxAbsorption(_room)[3]);
            }
        }

        public double OctaveBand1k
        {
            get { return _octaveBand1000Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand1000Hz, value, MaxAbsorption(_room)[4]);
            }
        }

        public double OctaveBand2k
        {
            get { return _octaveBand2000Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand2000Hz, value, MaxAbsorption(_room)[5]);
            }
        }

        public double OctaveBand4k
        {
            get { return _octaveBand4000Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand4000Hz, value, MaxAbsorption(_room)[6]);
            }
        }

        public double OctaveBand8k
        {
            get { return _octaveBand8000Hz; }
            set
            {
                SetOctaveBandValue(_octaveBand8000Hz, value, MaxAbsorption(_room)[7]);
            }
        }

        private void SetOctaveBandValue(double octaveToSet, double value, double maxAbsortion)
        {
            if (value < 0)
            {
                octaveToSet = 0;
            }
            else if (value < (0.99 - Math.Round(maxAbsortion, 2)))
            {
                octaveToSet = Math.Round(value, 2);
            }
            else
            {
                octaveToSet = 0.99 - Math.Round(maxAbsortion, 2);
            }
        }

        private double[] MaxAbsorption(Room room)
        {
            double s, mfp;
            double[] m = new double[8];

            s = 2 * (UnitConvertion.MToFt(room.Width) * UnitConvertion.MToFt(room.Length)) + 2 * (UnitConvertion.MToFt(room.Length) * UnitConvertion.MToFt(room.Height))
                + 2 * (UnitConvertion.MToFt(room.Width) * UnitConvertion.MToFt(room.Height));
            mfp = 4 * (UnitConvertion.MToFt(room.Width) * UnitConvertion.MToFt(room.Height) * UnitConvertion.MToFt(room.Length)) / s;

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = Transmission.M_coeff(room.Temperature, room.RelativeHumidity)[i] / UnitConvertion.MToFt(1) * mfp;
            }

            return m;
        }
    }
}
