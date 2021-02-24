using System;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{

    [Serializable]
    public class SoundAttenuator : ISoundAttenuator
    {
        private double _octaveBand63Hz;
        private double _octaveBand125Hz;
        private double _octaveBand250Hz;
        private double _octaveBand500Hz;
        private double _octaveBand1000Hz;
        private double _octaveBand2000Hz;
        private double _octaveBand4000Hz;
        private double _octaveBand8000Hz;

        public SoundAttenuator(double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz)
        {
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
            double result;
            result = MathOperation.OctaveSum(_octaveBand63Hz, _octaveBand125Hz, _octaveBand250Hz, _octaveBand500Hz, _octaveBand1000Hz, _octaveBand2000Hz,
                _octaveBand4000Hz, OctaveBand8k);

            return result;
        }

        public double OctaveBand63
        {
            get { return _octaveBand63Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand63Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand63Hz = value;
                }
                else
                {
                    _octaveBand63Hz = 99;
                }
            }
        }

        public double OctaveBand125
        {
            get { return _octaveBand125Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand125Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand125Hz = value;
                }
                else
                {
                    _octaveBand125Hz = 99;
                }
            }
        }

        public double OctaveBand250
        {
            get { return _octaveBand250Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand250Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand250Hz = value;
                }
                else
                {
                    _octaveBand250Hz = 99;
                }
            }
        }

        public double OctaveBand500
        {
            get { return _octaveBand500Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand500Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand500Hz = value;
                }
                else
                {
                    _octaveBand500Hz = 99;
                }
            }
        }

        public double OctaveBand1k
        {
            get { return _octaveBand1000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand1000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand1000Hz = value;
                }
                else
                {
                    _octaveBand1000Hz = 99;
                }
            }
        }

        public double OctaveBand2k
        {
            get { return _octaveBand2000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand2000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand2000Hz = value;
                }
                else
                {
                    _octaveBand2000Hz = 99;
                }
            }
        }

        public double OctaveBand4k
        {
            get { return _octaveBand4000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand4000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand4000Hz = value;
                }
                else
                {
                    _octaveBand4000Hz = 99;
                }
            }
        }

        public double OctaveBand8k
        {
            get { return _octaveBand8000Hz; }
            set
            {
                if (value < 0)
                {
                    _octaveBand8000Hz = 0;
                }
                else if (value < 99)
                {
                    _octaveBand8000Hz = value;
                }
                else
                {
                    _octaveBand8000Hz = 99;
                }
            }
        }
    }
}
