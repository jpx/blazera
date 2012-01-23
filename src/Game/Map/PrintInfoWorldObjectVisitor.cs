namespace BlazeraLib
{
    public class PrintInfoWorldObjectVisitor : IVisitor<WorldObject>
    {
        public void Visit(WorldObject wObj)
        {
            Log.Cl(wObj.Id, "Id");
            Log.Cl(wObj.Position.X, "X");
            Log.Cl(wObj.Position.Y, "Y");
        }
    }
}
