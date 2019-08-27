using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compute_Engine
{
    public class Enums
    {
        public enum DuctType
        {
            Round = 0,
            Rectangular = 1
        }

        public enum DiffuserType
        {
            Sudden = 0,
            Standard = 1
        }

        public enum ElbowType
        {
            Straight = 0,
            Rounded = 1
        }

        public enum PlenumType
        {
            HorizontalConnection = 0,
            VerticalConnection = 1
        }

        public enum DamperType
        {
            SingleBlade = 0,
            MultiBlade = 1
        }

        public enum SilencerType
        {
            ParallelBaffles = 0,
            Absorptive = 1
        }

        public enum NoiseEmission
        {
            OneDirection = 0,
            TwoDirection = 1,
        }

        public enum WorkArea
        {
            MaximumEfficiencyArea = 0,
            OutOffMaximumEfficiencyArea = 1
        }

        public enum NoiseLocation
        {
            RoomCenter = 1,
            SurfaceCenter = 2,
            SurfaceCorner = 3,
            SurfaceEdge = 4,
        }

        public enum ElementType
        {
            Duct = 0,
            Diffuser = 1,
            Bow = 2,
            Elbow = 3,
            Junction = 4,
            DoubleJunction = 5,
            TJunction = 6,
            Plenum = 7,
            Damper = 8,
            Grill = 9,
            Fan = 10,
            Silencer = 11,
            Room = 12
        }

        internal enum JunctionConnectionSide
        {
            Inlet = 0,
            Outlet = 1
        }

        public enum FanType
        {
            CentrifugalBackwardCurved = 1,
            CentrifugalRadial = 2,
            CentrifugalForwardCurved = 3,
            VaneAxial = 4,
            TubeAxial = 5,
            Propeller = 6,
            TubularCentrifugal = 7
        }

        public enum TurnigVanes
        {
            No = 0,
            Yes = 1
        }

        public enum Turbulence
        {
            No = 0,
            Yes = 1
        }

        public enum Branch
        {
            BranchRight = 1,
            BranchLeft = 2,
            Main = 0
        }

        public enum BranchType
        {
            Straight = 0,
            Rounded = 1
        }

        public enum GrillType
        {
            CircularSupplyWire = 0,
            CircularSupplyPlate = 1,
            CircularSupplySingleLouver = 2,
            CircularSupplyDoubleLouver = 3,
            RectangularSupplyWire = 4,
            RectangularSupplyPlate = 5,
            RectangularSupplySingleLouver = 6,
            RectangularSupplyDoubleLouver = 7,
            CircularExtractWire = 8,
            CircularExtractPlate = 9,
            CircularExtractSingleLouver = 10,
            CircularExtractDoubleLouver = 11,
            RectangularExtractWire = 12,
            RectangularExtractPlate = 13,
            RectangularExtractSingleLouver = 14,
            RectangularExtractDoubleLouver = 15,
        }

        public enum GrillLocation
        {
            FreeSpace = 0,
            FlushWall = 1,
        }

        public enum Lining
        {
            No = 0,
            Yes = 1,
        }

        public enum LiningType
        {
            Concrete = 0,
            Steel = 1,
            Fiberglass = 2,
        }

        public enum CeiligType
        {
            Gypboard_10mm = 0,
            Gypboard_12mm = 1,
            Gypboard_15mm = 2,
            Gypboard_25mm = 3,
            AcousticalCeilingTileExposedTBarGridSuspendedLight_60x120x10mm = 4,
            AcousticalCeilingTileExposedTBarGridSuspendedHeavy_60x120x10mm = 5,
            AcousticalCeilingTileExposedTBarGridSuspended_60x120x15mm = 6,
            AcousticalCeilingTileExposedTBarGridSuspended_60x60x10mm = 7,
            AcousticalCeilingTileConcealedSplineSuspended = 8,
        }

        public enum CeilingConfiguration
        {
            IntegratedLightingAndDiffuserSystem = 0,
            NoIntegratedLightingAndDiffuserSystem = 1,
        }

        public enum RoomType
        {
            Dead = 0,
            MediumDead = 1,
            Average = 2,
            MediumLive = 3,
            Live = 4,
        }
    }
}
