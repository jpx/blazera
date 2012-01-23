namespace BlazeraLib
{
    public abstract class Skin : DrawableBaseObject, ISkin
    {
        public class EventArgs : System.EventArgs { }
        public delegate void EventHandler(Skin sender, EventArgs e);

        public event EventHandler OnStarting;
        bool CallOnStarting() { if (OnStarting == null) return false; OnStarting(this, new EventArgs()); return true; }
        public event EventHandler OnStopping;
        bool CallOnStopping() { if (OnStopping == null) return false; OnStopping(this, new EventArgs()); return true; }

        public Skin()
            : base()
        { }

        public Skin(Skin copy)
            : base(copy)
        { }

        public virtual void Update(Time dt) { }

        public abstract Texture GetTexture();

        public virtual void Start() { CallOnStarting(); }
        public virtual void Stop() { CallOnStopping(); }
    }
}
