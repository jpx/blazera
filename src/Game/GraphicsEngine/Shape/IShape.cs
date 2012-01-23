using SFML.Window;

namespace BlazeraLib
{
    public interface IShape : IDrawable
    {
        Vector2f GetBasePosition();
    }
}
