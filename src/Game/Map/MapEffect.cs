using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    #region EventHandlers

    public class MapEffectEventArgs : System.EventArgs
    {
        public MapEffectEventArgs() { }
    }

    public delegate void MapEffectEventHandler(MapEffect sender, MapEffectEventArgs e);

    #endregion

    public abstract class MapEffect : BaseDrawable, IUpdateable
    {
        #region Members

        public DrawOrder DrawOrder { get; protected set; }

        /// <summary>
        /// Position of the effect when it starts.
        /// </summary>
        protected Vector2f BasePosition;

        #endregion

        #region Events

        public event MapEffectEventHandler OnStarting;
        protected bool CallOnStarting() { if (OnStarting == null) return false; OnStarting(this, new MapEffectEventArgs()); return true; }
        public event MapEffectEventHandler OnStopping;
        protected bool CallOnStopping() { if (OnStopping == null) return false; OnStopping(this, new MapEffectEventArgs()); return true; }

        #endregion

        public MapEffect() :
            base()
        {
            
        }

        public MapEffect(MapEffect copy) :
            base(copy)
        {
            DrawOrder = copy.DrawOrder;
        }

        public virtual void Start()
        {
            CallOnStarting();
        }

        public abstract void Update(Time dt);

        public virtual void SetBasePosition(Vector2f basePosition)
        {
            BasePosition = basePosition;
        }
    }
}
