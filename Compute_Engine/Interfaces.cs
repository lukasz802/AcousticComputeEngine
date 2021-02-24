using System;
using static Compute_Engine.Enums;

namespace Compute_Engine
{
    public class Interfaces
    {
        public interface IBranch : IComputeSoundLevel, IDuctConnection, IElementsContainer
        {
            int Rounding { get; set; }
            BranchType BranchType { get; set; }
        }

        public interface IComputeSoundLevel
        {
            double[] Attenuation();
            double[] Noise();
        }

        public interface IRectangular
        {
            int Width { get; set; }
            int Height { get; set; }
        }

        public interface IRound
        {
            int Diameter { get; set; }
        }

        public interface IDuctElement
        {
            DuctType DuctType { get; set; }
        }

        public interface IVelocity
        {
            double Velocity { get; }
            int AirFlow { get; set; }
        }

        public interface IGrillOrifice
        {
            int Height { get; set; }
            int Depth { get; set; }
        }

        public interface IDuctConnection : IRectangular, IRound, IVelocity, IDuctElement
        {
            event EventHandler DimensionsChanged;
            event EventHandler AirFlowChanged;
            event EventHandler DuctTypeChanged;
        }

        public interface IChangeableDimensions<T> where T : IRectangular, IRound, IVelocity
        {
            T Inlet { get; }
            T Outlet { get; }
        }

        public interface IDoubleBranchingElement<T> where T : IBranch
        {
            T BranchRight { get; }
            T BranchLeft { get; }
        }

        public interface ISingleBranchingElement<T> where T : IBranch
        {
            T Branch { get; }
        }

        public interface ISoundAttenuator
        {
            double TotalAttenution();
            double OctaveBand63 { get; set; }
            double OctaveBand125 { get; set; }
            double OctaveBand250 { get; set; }
            double OctaveBand500 { get; set; }
            double OctaveBand1k { get; set; }
            double OctaveBand2k { get; set; }
            double OctaveBand4k { get; set; }
            double OctaveBand8k { get; set; }
        }

        public interface IRoom : IComputeSoundLevel
        {
            NoiseLocation NoiseLocation { get; set; }
            double Width { get; set; }
            double Height { get; set; }
            double Length { get; set; }
            double Distance { get; set; }
        }

        public interface IElementsContainer
        {
            ElementsCollection Elements { get; }
        }
    }
}
