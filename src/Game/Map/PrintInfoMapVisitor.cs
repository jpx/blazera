namespace BlazeraLib
{
    public class PrintInfoMapVisitor : IVisitor<Map>
    {
        public void Visit(Map map)
        {
            Log.Cl(map.Type, "Type");
            Log.Cl(map.Width, "Width");
            Log.Cl(map.Height, "Height");
            Log.Cl(map.GetObjectCount(), "Object count");

            map.Accept(new PrintInfoWorldObjectVisitor());
        }
    }
}
