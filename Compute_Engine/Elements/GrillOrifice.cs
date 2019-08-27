using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compute_Engine.Elements
{
    [Serializable]
    public class GrillOrifice
    {
        private int _height;
        private int _depth;

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 5)
                {
                    _height = 5;
                }
                else if (value < 30)
                {
                    _height = value;
                }
                else
                {
                    _height = 30;
                }
            }
        }

        public int Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                if (value < 10)
                {
                    _depth = 10;
                }
                else if (value < 99)
                {
                    _depth = value;
                }
                else
                {
                    _depth = 99;
                }
            }
        }

        internal GrillOrifice(int height, int depth)
        {
            this.Height = height;
            this.Depth = depth;
        }
    }
}
