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
    internal class DoubleJunctionContaier
    {
        private int airflow_branch_right;
        private int width_branch_right;
        private int height_branch_right;
        private int diameter_branch_right;
        private int rnd_branch_right;
        private DuctType duct_type_branch;
        private BranchType branch_type_right;
        private int airflow_branch_left;
        private int width_branch_left;
        private int height_branch_left;
        private int diameter_branch_left;
        private int rnd_branch_left;
        private BranchType branch_type_left;
        private readonly DuctConnection _in = null;
        private readonly DuctConnection _out = null;

        internal DoubleJunctionContaier(DuctType ductTypeMain, int airFlowMain, int widthMainIn, int widthMainOut, int heightMainIn, int heightMainOut,
            int diameterMainIn, int diameterMainOut, DuctType ductTypeBranch, BranchType branchTypeRight, int airFlowBranchRight,
            int widthBranchRight, int heightBranchRight, int diameterBranchRight, int roundingRight, BranchType branchTypeLeft,
            int airFlowBranchLeft, int widthBranchLeft, int heightBranchLeft, int diameterBranchLeft, int roundingLeft)
        {
            duct_type_branch = ductTypeBranch;
            airflow_branch_right = airFlowBranchRight;
            width_branch_right = widthBranchRight;
            height_branch_right = heightBranchRight;
            diameter_branch_right = diameterBranchRight;
            rnd_branch_right = roundingRight;
            branch_type_right = branchTypeRight;
            airflow_branch_left = airFlowBranchLeft;
            width_branch_left = widthBranchLeft;
            height_branch_left = heightBranchLeft;
            diameter_branch_left = diameterBranchLeft;
            rnd_branch_left = roundingLeft;
            branch_type_left = branchTypeLeft;
            _in = new DuctConnection(ductTypeMain, airFlowMain, widthMainIn, heightMainIn, diameterMainIn);
            _out = new DuctConnection(ductTypeMain, airFlowMain - airFlowBranchRight - airFlowBranchLeft, widthMainOut, heightMainOut, diameterMainOut);
        }

        internal int AirFlowBranchRight
        {
            get
            {
                return airflow_branch_right;
            }
            set
            {
                airflow_branch_right = value;
            }
        }

        internal int WidthBranchRight
        {
            get
            {
                return width_branch_right;
            }
            set
            {
                if (value < 100)
                {
                    width_branch_right = 100;
                }
                else if (value < 2000)
                {
                    width_branch_right = value;
                }
                else
                {
                    width_branch_right = 2000;
                }
            }
        }

        internal int HeightBranchRight
        {
            get
            {
                return height_branch_right;
            }
            set
            {
                if (value < 100)
                {
                    height_branch_right = 100;
                }
                else if (value < 2000)
                {
                    height_branch_right = value;
                }
                else
                {
                    height_branch_right = 2000;
                }
            }
        }

        internal int DiameterBranchRight
        {
            get
            {
                return diameter_branch_right;
            }
            set
            {
                if (value < 80)
                {
                    diameter_branch_right = 80;
                }
                else if (value < 1600)
                {
                    diameter_branch_right = value;
                }
                else
                {
                    diameter_branch_right = 1600;
                }
            }
        }

        internal int RoundingBranchRight
        {
            get
            {
                return rnd_branch_right;
            }
            set
            {
                if (value < 0)
                {
                    rnd_branch_right = 0;
                }
                else if (value < Math.Ceiling(0.6 * width_branch_right))
                {
                    rnd_branch_right = value;
                }
                else
                {
                    rnd_branch_right = (int)Math.Ceiling(0.6 * width_branch_right);
                }
            }
        }

        internal BranchType BranchTypeRight
        {
            get
            {
                return branch_type_right;
            }
            set
            {
                branch_type_right = value;
            }
        }

        internal int AirFlowBranchLeft
        {
            get
            {
                return airflow_branch_left;
            }
            set
            {
                airflow_branch_left = value;
            }
        }

        internal int WidthBranchLeft
        {
            get
            {
                return width_branch_left;
            }
            set
            {
                if (value < 100)
                {
                    width_branch_left = 100;
                }
                else if (value < 2000)
                {
                    width_branch_left = value;
                }
                else
                {
                    width_branch_left = 2000;
                }
            }
        }

        internal int HeightBranchLeft
        {
            get
            {
                return height_branch_left;
            }
            set
            {
                if (value < 100)
                {
                    height_branch_left = 100;
                }
                else if (value < 2000)
                {
                    height_branch_left = value;
                }
                else
                {
                    height_branch_left = 2000;
                }
            }
        }

        internal int DiameterBranchLeft
        {
            get
            {
                return diameter_branch_left;
            }
            set
            {
                if (value < 80)
                {
                    diameter_branch_left = 80;
                }
                else if (value < 1600)
                {
                    diameter_branch_left = value;
                }
                else
                {
                    diameter_branch_left = 1600;
                }
            }
        }

        internal int RoundingBranchLeft
        {
            get
            {
                return rnd_branch_left;
            }
            set
            {
                if (value < 0)
                {
                    rnd_branch_left = 0;
                }
                else if (value < Math.Ceiling(0.6 * width_branch_left))
                {
                    rnd_branch_left = value;
                }
                else
                {
                    rnd_branch_left = (int)Math.Ceiling(0.6 * width_branch_left);
                }
            }
        }

        internal DuctConnection In
        {
            get
            {
                return _in;
            }
        }

        internal DuctConnection Out
        {
            get
            {
                return _out;
            }
        }

        internal BranchType BranchTypeLeft
        {
            get
            {
                return branch_type_left;
            }
            set
            {
                branch_type_left = value;
            }
        }

        internal DuctType DuctType
        {
            get
            {
                return duct_type_branch;
            }
            set
            {
                duct_type_branch = value;
            }
        }
    }
}
