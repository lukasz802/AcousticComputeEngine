using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Enums;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class Fan : ElementsBase
    {
        private static int _counter = 1;
        private static string _name = "fan_";
        private FanType _fanType;
        public NoiseEmission NoiseEmission { get; set; }
        public WorkArea WorkArea { get; set; }
        private int _pressure_drop;
        private byte _efficient;
        private byte _blade_number;
        private int _rpm;

        /// <summary>Wentylator.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="fanType">Typ wentylatora.</param>
        /// <param name="airFlow">Wydajność wentylatora w analizowanym punkcie pracy [m3/h].</param>
        /// <param name="pressureDrop">Spręż całkowity wentylatora w analizowanym punkcie pracy [Pa].</param>
        /// <param name="rpm">Prędkość obrotowa wirnika dla założonej wydajności i sprężu [rpm].</param>
        /// <param name="bladeNumber">Liczba łopatek.</param>
        /// <param name="efficientDeviation">Względne odchylenie od punktu sprawności szczytowej [%].</param>
        /// <param name="workArea">Obszar pracy.</param>
        /// <param name="noiseEmissionDirection">Kierunek emisji hałasu.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Fan(string name, string comments, FanType fanType, int airFlow, int pressureDrop, int rpm, byte bladeNumber, byte efficientDeviation,
             WorkArea workArea, NoiseEmission noiseEmissionDirection, bool include)
        {
            _type = ElementType.Fan;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _fanType = fanType;
            _pressure_drop = pressureDrop;
            _rpm = rpm;
            _blade_number = bladeNumber;
            _efficient = efficientDeviation;
            this.NoiseEmission = noiseEmissionDirection;
            this.WorkArea = workArea;
            _counter = 1;
        }

        /// <summary>Wentylator.</summary>
        public Fan()
        {
            _type = ElementType.Fan;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            this.AirFlow = 5000;
            this.IsIncluded = true;
            _fanType = FanType.CentrifugalBackwardCurved;
            _pressure_drop = 250;
            _rpm = 1500;
            _blade_number = 12;
            _efficient = 0;
            this.NoiseEmission = NoiseEmission.OneDirection;
            this.WorkArea = WorkArea.MaximumEfficiencyArea;
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
            byte loc;

            if (this.NoiseEmission == NoiseEmission.OneDirection)
            {
                loc = 0;
            }
            else
            {
                loc = 1;
            }

            return Function.Noise.Fan(_fanType, base.AirFlow, _pressure_drop, _rpm, _blade_number, _efficient, loc);
        }

        public FanType FanType
        {
            get
            {
                return _fanType;
            }
            set
            {
                _fanType = value;
                this.BladeNumber = _blade_number;
            }
        }

        public int PressureDrop
        {
            get
            {
                return _pressure_drop;
            }
            set
            {
                if (value < 10)
                {
                    _pressure_drop = 10;
                }
                else if (value < 9999)
                {
                    _pressure_drop = value;
                }
                else
                {
                    _pressure_drop = 9999;
                }
            }
        }

        public byte Efficient
        {
            get
            {
                return _efficient;
            }
            set
            {
                if (value < 0)
                {
                    _efficient = 0;
                }
                else if (value < 50)
                {
                    _efficient = value;
                }
                else
                {
                    _efficient = 50;
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
                byte max_temp, min_temp;

                switch (_fanType)
                {
                    case FanType.CentrifugalBackwardCurved:
                        min_temp = 10;
                        max_temp = 16;
                        break;
                    case FanType.CentrifugalRadial:
                        min_temp = 6;
                        max_temp = 10;
                        break;
                    case FanType.CentrifugalForwardCurved:
                        min_temp = 24;
                        max_temp = 64;
                        break;
                    case FanType.VaneAxial:
                        min_temp = 3;
                        max_temp = 16;
                        break;
                    case FanType.TubeAxial:
                        min_temp = 4;
                        max_temp = 8;
                        break;
                    case FanType.Propeller:
                        min_temp = 2;
                        max_temp = 8;
                        break;
                    default:
                        min_temp = 10;
                        max_temp = 16;
                        break;
                }

                if (value < min_temp)
                {
                    _blade_number = min_temp;
                }
                else if (value < max_temp)
                {
                    _blade_number = value;
                }
                else
                {
                    _blade_number = max_temp;
                }
            }
        }

        public int RPM
        {
            get
            {
                return _rpm;
            }
            set
            {
                if (value < 150)
                {
                    _rpm = value;
                }
                else if (value < 3000)
                {
                    _rpm = value;
                }
                else
                {
                    _rpm = 3000;
                }
            }
        }
    }
}
