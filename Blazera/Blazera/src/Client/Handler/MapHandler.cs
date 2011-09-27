using BlazeraLib;

namespace Blazera
{
    public class MapHandler : PacketHandler
    {
        CMap Map;

        public MapHandler(CMap map) :
            base()
        {
            Map = map;
        }
    }
}
