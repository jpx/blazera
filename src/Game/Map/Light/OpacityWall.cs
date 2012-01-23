using SFML.Window;

namespace BlazeraLib
{
    public class OpacityWall
    {
        public Vector2f FirstPoint { get; set; }
        public Vector2f SecondPoint { get; set; }

        public OpacityWall(Vector2f firstPoint, Vector2f secondPoint)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
        }

        public Vector2f Center
        {
            get { return (FirstPoint + SecondPoint) / 2F; }
        }
    }
}
