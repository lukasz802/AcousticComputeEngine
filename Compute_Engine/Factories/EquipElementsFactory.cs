using Compute_Engine.Elements;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Factories
{
    public class EquipElementsFactory
    {
        static public IGrillOrifice GetGrillOrifice(int height, int depth)
        {
            return new GrillOrifice(height, depth);
        }

        static public ISoundAttenuator GetSoundAttenuator(double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz)
        {
            return new SoundAttenuator(octaveBand63Hz, octaveBand125Hz, octaveBand250Hz, octaveBand500Hz,
                octaveBand1000Hz, octaveBand2000Hz, octaveBand4000Hz, octaveBand8000Hz);
        }

        static public ISoundAttenuator GetRoomConstatnt(Room room, double octaveBand63Hz, double octaveBand125Hz, double octaveBand250Hz, double octaveBand500Hz,
            double octaveBand1000Hz, double octaveBand2000Hz, double octaveBand4000Hz, double octaveBand8000Hz)
        {
            return new RoomConstant(room, octaveBand63Hz, octaveBand125Hz, octaveBand250Hz, octaveBand500Hz,
                octaveBand1000Hz, octaveBand2000Hz, octaveBand4000Hz, octaveBand8000Hz);
        }
    }
}
