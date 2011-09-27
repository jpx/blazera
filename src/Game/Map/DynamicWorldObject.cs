namespace BlazeraLib
{
    public class DynamicWorldObject : WorldObject
    {
        public int Guid { get; set; }

        public DynamicWorldObject() :
            base()
        {

        }

        public DynamicWorldObject(DynamicWorldObject copy) :
            base(copy)
        {
            Guid = copy.Guid;
        }
    }
}
