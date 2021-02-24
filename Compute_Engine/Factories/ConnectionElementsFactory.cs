using Compute_Engine.Elements;
using static Compute_Engine.Enums;
using static Compute_Engine.Interfaces;

namespace Compute_Engine.Factories
{
    public class ConnectionElementsFactory
    {
        static public IDuctConnection GetConnectionElement(DuctType ductType, int airFlow, int w, int h, int d)
        {
            return new DuctConnection(ductType, airFlow, w, h, d);
        }
    }
}
