using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Elements
{

    [Serializable]
    public class SoundAttenuation : IOctaveBandAttenuation
    {
        private int _octaveBand63Hz;
        private int _octaveBand125Hz;
        private int _octaveBand250Hz;
        private int _octaveBand500Hz;
        private int _octaveBand1000Hz;
        private int _octaveBand2000Hz;
        private int _octaveBand4000Hz;
        private int _octaveBand8000Hz;

        internal SoundAttenuation(int octaveBand63Hz, int octaveBand125Hz, int octaveBand250Hz, int octaveBand500Hz,
            int octaveBand1000Hz, int octaveBand2000Hz, int octaveBand4000Hz, int octaveBand8000Hz)
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
                _octaveBand4000Hz, OctaveBand8000Hz);

            return result;
        }

        public int OctaveBand63Hz
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

        public int OctaveBand125Hz
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

        public int OctaveBand250Hz
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

        public int OctaveBand500Hz
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

        public int OctaveBand1000Hz
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

        public int OctaveBand2000Hz
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

        public int OctaveBand4000Hz
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

        public int OctaveBand8000Hz
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
