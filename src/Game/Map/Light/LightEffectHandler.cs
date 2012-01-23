using SFML.Window;

namespace BlazeraLib
{
    public class LightEffectHandler
    {
        #region Constants

        static readonly Vector2f DEFAULT_LIGHT_BASE_POSITION = new Vector2f();
        const int DEFAULT_LIGHT_BASE_Z = 0;

        #endregion

        #region Members

        WorldElement Parent;
        public LightEffect Effect { get; private set; }
        Vector2f BasePosition;
        int BaseZ;

        #endregion

        public LightEffectHandler(WorldElement parent, LightEffect effect)
            : this(parent, effect, DEFAULT_LIGHT_BASE_POSITION, DEFAULT_LIGHT_BASE_Z)
        {
        }

        public LightEffectHandler(WorldElement parent, LightEffect effect, Vector2f basePosition)
            : this(parent, effect, basePosition, DEFAULT_LIGHT_BASE_Z)
        {
        }

        public LightEffectHandler(WorldElement parent, LightEffect effect, int baseZ)
            : this(parent, effect, DEFAULT_LIGHT_BASE_POSITION, baseZ)
        {
        }

        public LightEffectHandler(WorldElement parent, LightEffect effect, Vector2f basePosition, int baseZ)
        {
            Parent = parent;
            Effect = effect;
            BasePosition = basePosition;
            BaseZ = baseZ;

            Parent.OnMove += new MoveEventHandler(Parent_OnMove);

            Update();
        }

        void Parent_OnMove(IDrawable sender, MoveEventArgs e)
        {
            Update();
        }

        public LightEffectHandler(WorldElement parent, LightEffectHandler copy)
        {
            Parent = parent;

            Effect = (LightEffect)copy.Effect.Clone();
            BasePosition = copy.BasePosition;

            Update();
        }

        void Update()
        {
            Effect.Position = Parent.DrawingCenter + BasePosition;
            Effect.Z = Parent.Z + BaseZ;
        }
    }
}
