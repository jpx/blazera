namespace BlazeraLib
{
    public interface IVisitable<T>
    {
        void Accept(IVisitor<T> visitor);
    }
}
