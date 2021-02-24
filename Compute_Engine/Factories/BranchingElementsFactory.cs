using Compute_Engine.Elements;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Factories
{
    public class BranchingElementsFactory
    {
        static public IBranch GetJunctionBranch(IDuctConnection connectionIn, IDuctConnection connectionOut, DuctType ductTypeBranch,
            BranchType branchType, int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int rounding)
        {
            return new JunctionBranch(connectionIn, connectionOut, ductTypeBranch, branchType, airFlowBranch, widthBranch, heightBranch,
                diameterBranch, rounding);
        }

        static public IBranch GetDoubleJunctionBranch(IDuctConnection connectionIn, IDuctConnection connectionOut, IBranch branchInPair, BranchDirection branchDirection,
            DuctType ductTypeBranch, BranchType branchType, int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int rounding)
        {
            return new DoubleJunctionBranch(connectionIn, connectionOut, branchInPair, branchDirection, ductTypeBranch, branchType, airFlowBranch,
                widthBranch, heightBranch, diameterBranch, rounding);
        }

        static public IBranch GetTJunctionBranch(IDuctConnection connection, IBranch branchInPair, BranchDirection branchDirection,
            DuctType ductTypeBranch, BranchType branchType, int airFlowBranch, int widthBranch, int heightBranch, int diameterBranch, int rounding)
        {
            return new TJunctionBranch(connection, branchInPair, branchDirection, ductTypeBranch, branchType, airFlowBranch,
                widthBranch, heightBranch, diameterBranch, rounding);
        }
    }
}
