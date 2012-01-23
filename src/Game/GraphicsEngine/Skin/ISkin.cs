namespace BlazeraLib
{
    public interface ISkin : IUpdateable, IDrawable, IScriptable
    {
        Texture GetTexture();

        void Start();

        void Stop();
    }
}
