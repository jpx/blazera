namespace BlazeraLib
{
    public interface IVisitor<T>
    {
        void Visit(T visitedObject);
    }
}
