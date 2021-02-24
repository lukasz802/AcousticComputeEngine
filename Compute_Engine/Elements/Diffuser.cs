using Compute_Engine.Factories;
using System;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;
using Function = Compute_Engine;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class Diffuser : ElementsBase, IChangeableDimensions<IDuctConnection>
    {
        private static int _counter = 1;
        private static string _name = "dfs_";
        private IDuctConnection _in;
        private IDuctConnection _out;
        private double _lenght;
        private DiffuserType _diffuser_type;

        /// <summary>Dyfuzor/konfuzor lub nagłe zwężenie/rozszerzenie.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="diffuserType">Typ redukcji.</param>
        /// <param name="diffuserIn">Typ wlotowego króćca podłączeniowego.</param>
        /// <param name="diffuserOut">Typ wylotowego króćca podłączeniowego.</param>
        /// <param name="airFlow">Przepływ powietrza przez element [m3/h].</param>
        /// <param name="widthIn">Szerokość wlotwego króćca przyłączeniowego [mm].</param>
        /// <param name="heightIn">Wysokość wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="diameterIn">Średnica wlotowego króćca przyłączeniowego [mm].</param>
        /// <param name="widthOut">Szerokość wylotowego króćca przyłączeniowego [mm].</param>
        /// <param name="heightOut">Wysokość wylotowego króćca przyłączeniowego [mm].</param>
        /// <param name="diameterOut">Średnica wylotowego króćca przyłączeniowego [mm].</param>
        /// <param name="lenght">Długość kształtki [m].</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Diffuser(string name, string comments, DiffuserType diffuserType, DuctType diffuserIn, DuctType diffuserOut, int airFlow, int widthIn, int heightIn,
            int widthOut, int heightOut, int diameterIn, int diameterOut, double lenght, bool include)
        {
            _type = ElementType.Diffuser;
            this.Comments = comments;
            this.Name = name;
            base.AirFlow = airFlow;
            this.IsIncluded = include;
            _diffuser_type = diffuserType;
            _lenght = lenght;
            _in = ConnectionElementsFactory.GetConnectionElement(diffuserIn, base.AirFlow, widthIn, heightIn, diameterIn);
            _out = ConnectionElementsFactory.GetConnectionElement(diffuserOut, base.AirFlow, widthOut, heightOut, diameterOut);
            _counter = 1;
        }
        /// <summary>Dyfuzor/konfuzor lub nagłe zwężenie/rozszerzenie.</summary>
        public Diffuser()
        {
            _type = ElementType.Diffuser;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            _counter++;
            base.AirFlow = 500;
            this.IsIncluded = true;
            _diffuser_type = DiffuserType.Sudden;
            _lenght = 0;
            _in = ConnectionElementsFactory.GetConnectionElement(DuctType.Rectangular, 500, 200, 200, 250);
            _out = ConnectionElementsFactory.GetConnectionElement(DuctType.Rectangular, 500, 200, 200, 250);
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];

            if (_diffuser_type == DiffuserType.Sudden)
            {
                if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.Diffuser(_in.Width / 1000.0 * _in.Height / 1000.0, _out.Width / 1000.0 * _out.Height / 1000.0, 0);
                }
                else if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.Diffuser(_in.Width / 1000.0 * _in.Height / 1000.0, 0.25 * Math.PI * Math.Pow(_out.Diameter / 1000.0, 2), 0);
                }
                else if (_in.DuctType == DuctType.Round && _out.DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter / 1000.0, 2), _out.Width / 1000.0 * _out.Height / 1000.0, 0);
                }
                else
                {
                    attn = Function.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter / 1000.0, 2), 0.25 * Math.PI * Math.Pow(_out.Diameter / 1000.0, 2), 0);
                }
            }
            else
            {
                if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.Diffuser(_in.Width / 1000.0 * _in.Height / 1000.0, _out.Width / 1000.0 * _out.Height / 1000.0, _lenght);
                }
                else if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Round)
                {
                    attn = Function.Attenuation.Diffuser(_in.Width / 1000.0 * _in.Height / 1000.0, 0.25 * Math.PI * Math.Pow(_out.Diameter / 1000.0, 2), _lenght);
                }
                else if (_in.DuctType == DuctType.Round && _out.DuctType == DuctType.Rectangular)
                {
                    attn = Function.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter / 1000.0, 2), _out.Width / 1000.0 * _out.Height / 1000.0, _lenght);
                }
                else
                {
                    attn = Function.Attenuation.Diffuser(0.25 * Math.PI * Math.Pow(_in.Diameter / 1000.0, 2), 0.25 * Math.PI * Math.Pow(_out.Diameter / 1000.0, 2), _lenght);
                }
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            return lw;
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

        public DiffuserType DiffuserType
        {
            get
            {
                return _diffuser_type;
            }
            set
            {
                _diffuser_type = value;
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
                _in.AirFlow = value;
                _out.AirFlow = value;
            }
        }

        public IDuctConnection Inlet
        {
            get { return _in; }
        }

        public IDuctConnection Outlet
        {
            get { return _out; }
        }
    }
}
