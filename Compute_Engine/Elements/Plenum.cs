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
    public class Plenum : ElementsBase, IChangeableDimensions<DuctConnection>
    {
        private static int _counter = 1;
        private static string _name = "pln_";
        private int _width;
        private int _height;
        private int _lenght;
        private int _dl;
        private int _liner_thickness;
        private bool _liner_check;
        private PlenumType _plenum_type;
        private DuctConnection _in = null;
        private DuctConnection _out = null;

        /// <summary>Skrzynka tłumiąca.</summary>
        /// <param name="name">Nazwa elementu.</param>
        /// <param name="comments">Informacje dodatkowe.</param>
        /// <param name="plenumIn">Typ króćca podłączeniowego od strony wlotowej.</param>
        /// <param name="plenumOut">Typ króćca podłączeniowego od strony wylotowej.</param>
        /// <param name="airFlow">Przepływ powietrza na wlocie do elementu [m3/h].</param>
        /// <param name="widthIn">Szerokość króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="heightIn">Wysokość króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="diameterIn">Średnica króćca podłączeniowego od strony wlotowej [mm].</param>
        /// <param name="widthOut">Szerokość króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="heightOut">Wysokość króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="diameterOut">Średnica króćca podłączeniowego od strony wylotowej [mm].</param>
        /// <param name="plenumWidth">Szerokość skrzynki tłumiącej [mm].</param>
        /// <param name="plenumHeight">Wysokość skrzynki tłumiącej [mm].</param>
        /// <param name="plenumLenght">Długość skrzynki tłumiącej [mm].</param>
        /// <param name="linerThickness">Grubość izoloacji akustycznej skrzynki [mm].</param>
        /// <param name="linerCheck">Czy skrzynka jest zaizolowana akustycznie.</param>
        /// <param name="include">Czy uwzględnić element podczas obliczeń.</param>
        /// <returns></returns>
        public Plenum(string name, string comments, PlenumType plenumType, DuctType plenumIn, DuctType plenumOut, int airFlow, int widthIn, int heightIn, int widthOut, int heightOut, int diameterIn,
             int diameterOut, int plenumWidth, int plenumHeight, int plenumLenght, int inLocationLenght, int linerThickness, bool linerCheck, bool include)
        {
            _type = ElementType.Plenum;
            this.Comments = comments;
            this.Name = name;
            this.AirFlow = airFlow;
            this.IsIncluded = include;
            _plenum_type = plenumType;
            _width = plenumWidth;
            _height = plenumHeight;
            _lenght = plenumLenght;
            _dl = inLocationLenght;
            _liner_check = linerCheck;
            _liner_thickness = linerThickness;
            _in = new DuctConnection(plenumIn, base.AirFlow, widthIn, heightIn, diameterIn);
            _out = new DuctConnection(plenumOut, base.AirFlow, widthOut, heightOut, diameterOut);
            _in.DimensionsChanged += _DimensionsChanged;
            _out.DimensionsChanged += _DimensionsChanged;
            _counter = 1;
        }

        private void _DimensionsChanged(object sender, EventArgs e)
        {
            UpdateLenght();
            UpdateWidth();
            UpdateHeight();
            UpdateDistance();
        }

        /// <summary>Skrzynka tłumiąca.</summary>
        public Plenum()
        {
            _type = ElementType.Plenum;
            this.Comments = "";
            this.Name = (_name + _counter).ToString();
            this.AirFlow = 800;
            this.IsIncluded = true;
            _plenum_type = PlenumType.VerticalConnection;
            _width = 400;
            _height = 250;
            _lenght = 500;
            _dl = 300;
            _liner_check = false;
            _liner_thickness = 25;
            _in = new DuctConnection(DuctType.Round, base.AirFlow, 200, 160, 160);
            _out = new DuctConnection(DuctType.Rectangular, base.AirFlow, 400, 250, 250);
            _in.DimensionsChanged += _DimensionsChanged;
            _out.DimensionsChanged += _DimensionsChanged;
        }

        private void UpdateWidth()
        {
            int temp_in, temp_out;

            if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Width; }
            else { temp_in = _in.Diameter; }

            if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Width; }
            else { temp_out = _out.Diameter; }

            if (_plenum_type == PlenumType.HorizontalConnection)
            {
                if (Math.Max(temp_in, temp_out) > _width)
                {
                    _width = Math.Max(temp_in, temp_out);
                }
            }
            else
            {
                if (temp_in > _width)
                {
                    _width = temp_in;
                }
            }
        }

        private void UpdateHeight()
        {
            int temp_in, temp_out;

            if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
            else { temp_in = _in.Diameter; }

            if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
            else { temp_out = _out.Diameter; }

            if (_plenum_type == PlenumType.HorizontalConnection)
            {
                if (Math.Max(temp_in, temp_out) > _height)
                {
                    _height = Math.Max(temp_in, temp_out);
                }
            }
            else
            {
                if (temp_in > _height)
                {
                    _height = temp_in;
                }
            }
        }

        private void UpdateLenght()
        {
            int temp_in, temp_out;

            if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
            else { temp_in = _in.Diameter; }

            if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
            else { temp_out = _out.Diameter; }

            if (_plenum_type == PlenumType.VerticalConnection)
            {
                if (temp_in > _lenght)
                {
                    _lenght = temp_in;
                }
            }
            else
            {
                if (_in.DuctType == DuctType.Rectangular) { temp_in = Math.Min(_in.Height, _in.Width); }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = Math.Min(_out.Height, _out.Width); }
                else { temp_out = _out.Diameter; }

                if (Math.Min(temp_in, temp_out) > _lenght)
                {
                    _lenght = Math.Min(temp_in, temp_out);
                }
            }
        }

        private void UpdateDistance()
        {
            int temp;

            if (_in.DuctType == DuctType.Rectangular) { temp = _in.Height; }
            else { temp = _in.Diameter; }

            if (Math.Floor(0.5 * temp) > _dl)
            {
                _dl = (int)Math.Floor(0.5 * temp);
            }
            else if (!(Math.Ceiling(_lenght - 0.5 * temp) > _dl))
            {
                _dl = (int)Math.Ceiling(_lenght - 0.5 * temp);
            }
        }

        /// <summary>Oblicz tłumienie akustyczne elementu.</summary>
        public override double[] Attenuation()
        {
            double[] attn = new double[8];
            double dq;

            if (_plenum_type == PlenumType.VerticalConnection)
            {
                if ((4 - 4 * ((_lenght - _dl) / _lenght)) >= 2.0 && (4 - 4 * ((_lenght - _dl) / _lenght)) <= 4.0)
                {
                    dq = 4 - 4 * ((_lenght - _dl) / _lenght);
                }
                else if ((4 - 4 * ((_lenght - _dl) / _lenght)) > 4.0)
                {
                    dq = 4.0;
                }
                else
                {
                    dq = 2.0;
                }
            }
            else
            {
                dq = 2.0;
            }

            if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.PlenumInletRectangular(_liner_thickness / 10.0, dq, _in.Width / 1000.0 * _in.Height / 1000.0, _out.Width / 1000.0 * _out.Height / 1000.0,
                    Math.Max(_in.Width, _in.Height), _dl / 1000.0, _lenght / 1000.0, _width / 1000.0, _height / 1000.0);
            }
            else if (_in.DuctType == DuctType.Rectangular && _out.DuctType == DuctType.Round)
            {
                attn = Function.Attenuation.PlenumInletRectangular(_liner_thickness / 10.0, dq, _in.Width / 1000.0 * _in.Height / 1000.0, Math.Pow(_out.Diameter / 1000.0, 2) * 0.25 * Math.PI,
                    Math.Max(_in.Width, _in.Height), _dl / 1000.0, _lenght / 1000.0, _width / 1000.0, _height / 1000.0);
            }
            else if (_in.DuctType == DuctType.Round && _out.DuctType == DuctType.Rectangular)
            {
                attn = Function.Attenuation.PlenumInletRound(_liner_thickness / 10.0, dq, _out.Width / 1000.0 * _out.Height / 1000.0, _in.Diameter / 1000.0, _dl / 1000.0, _lenght / 1000.0,
                    _width / 1000.0, _height / 1000.0);
            }
            else
            {
                attn = Function.Attenuation.PlenumInletRound(_liner_thickness / 10.0, dq, Math.Pow(_out.Width / 1000.0, 2) * 0.25 * Math.PI, _in.Diameter / 1000.0, _dl / 1000.0, _lenght / 1000.0,
                    _width / 1000.0, _height / 1000.0);
            }
            return attn;
        }

        /// <summary>Oblicz szum generowany przez element.</summary>
        public override double[] Noise()
        {
            double[] lw = { -10000, -10000, -10000, -10000, -10000, -10000, -10000, -10000 };
            return lw;
        }

        public DuctConnection Inlet
        {
            get
            {
                return _in;
            }
        }

        public DuctConnection Outlet
        {
            get
            {
                return _out;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                int temp_in, temp_out;

                if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Width; }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Width; }
                else { temp_out = _out.Diameter; }

                if (_plenum_type == PlenumType.HorizontalConnection)
                {
                    if (Math.Max(temp_in, temp_out) > value)
                    {
                        _width = Math.Max(temp_in, temp_out);
                    }
                    else
                    {
                        _width = value;
                    }
                }
                else
                {
                    if (temp_in > value)
                    {
                        _width = temp_in;
                    }
                    else
                    {
                        _width = value;
                    }
                }
                UpdateLenght();
                UpdateHeight();
                UpdateDistance();
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
                int temp_in, temp_out;

                if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
                else { temp_out = _out.Diameter; }

                if (_plenum_type == PlenumType.HorizontalConnection)
                {
                    if (Math.Max(temp_in, temp_out) > value)
                    {
                        _height = Math.Max(temp_in, temp_out);
                    }
                    else
                    {
                        _height = value;
                    }
                }
                else
                {
                    if (temp_in > value)
                    {
                        _height = temp_in;
                    }
                    else
                    {
                        _height = value;
                    }
                }
                UpdateLenght();
                UpdateWidth();
                UpdateDistance();
            }
        }

        public int Lenght
        {
            get
            {
                return _lenght;
            }
            set
            {
                int temp_in, temp_out;

                if (_in.DuctType == DuctType.Rectangular) { temp_in = _in.Height; }
                else { temp_in = _in.Diameter; }

                if (_out.DuctType == DuctType.Rectangular) { temp_out = _out.Height; }
                else { temp_out = _out.Diameter; }

                if (_plenum_type == PlenumType.VerticalConnection)
                {
                    if (temp_in > value)
                    {
                        _lenght = temp_in;
                    }
                    else
                    {
                        _lenght = value;
                    }
                }
                else
                {
                    if (_in.DuctType == DuctType.Rectangular) { temp_in = Math.Min(_in.Height, _in.Width); }
                    else { temp_in = _in.Diameter; }

                    if (_out.DuctType == DuctType.Rectangular) { temp_out = Math.Min(_out.Height, _out.Width); }
                    else { temp_out = _out.Diameter; }

                    if (Math.Min(temp_in, temp_out) > value)
                    {
                        _lenght = Math.Min(temp_in, temp_out);
                    }
                    else
                    {
                        _lenght = value;
                    }
                }
                UpdateHeight();
                UpdateWidth();
                UpdateDistance();
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
                else if (value < 100)
                {
                    _liner_thickness = value;
                }
                else
                {
                    _liner_thickness = 100;
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

        public PlenumType PlenumType
        {
            get
            {
                return _plenum_type;
            }
            set
            {
                _plenum_type = value;
                UpdateLenght();
                UpdateWidth();
                UpdateHeight();
                UpdateDistance();
            }
        }

        public int InletDistance
        {
            get
            {
                return _dl;
            }
            set
            {
                int temp;

                if (_in.DuctType == DuctType.Rectangular) { temp = _in.Height; }
                else { temp = _in.Diameter; }

                if (Math.Floor(0.5 * temp) > value)
                {
                    _dl = (int)Math.Floor(0.5 * temp);
                }
                else if (Math.Ceiling(_lenght - 0.5 * temp) > value)
                {
                    _dl = value;
                }
                else
                {
                    _dl = (int)Math.Ceiling(_lenght - 0.5 * temp);
                }
                UpdateLenght();
                UpdateHeight();
                UpdateWidth();
            }
        }
    }
}
