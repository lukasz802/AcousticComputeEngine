using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compute_Engine.Enums;

namespace Compute_Engine
{
    public class Interfaces
    {
        public interface IBranch : IRectangular, IRound, IVelocity, IElementsContainer
        {
            double[] Attenuation();
            double[] Noise();
            int AirFlow { get; set; }
            int Rounding { get; set; }
            BranchType BranchType { get; set; }
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

        public interface IVelocity
        {
            double Velocity { get; }
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

        public interface IOctaveBandAttenuation
        {
            int OctaveBand63Hz { get; set; }
            int OctaveBand125Hz { get; set; }
            int OctaveBand250Hz { get; set; }
            int OctaveBand500Hz { get; set; }
            int OctaveBand1000Hz { get; set; }
            int OctaveBand2000Hz { get; set; }
            int OctaveBand4000Hz { get; set; }
            int OctaveBand8000Hz { get; set; }
        }

        public interface IOctaveBandAbsorption
        {
            double OctaveBand63Hz { get; set; }
            double OctaveBand125Hz { get; set; }
            double OctaveBand250Hz { get; set; }
            double OctaveBand500Hz { get; set; }
            double OctaveBand1000Hz { get; set; }
            double OctaveBand2000Hz { get; set; }
            double OctaveBand4000Hz { get; set; }
            double OctaveBand8000Hz { get; set; }
        }

        public interface IElementsContainer
        {
            ElementsCollection Elements { get; }
        }
    }
}
