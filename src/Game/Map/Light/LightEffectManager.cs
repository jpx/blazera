using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class LightEffectManager
    {
        #region Constants

        const float SMOOTHNESS = .5F;

        static readonly Color DEFAUT_GLOBAL_COLOR = Color.White;

        #endregion Constants

        #region Members

        Map Parent;

        List<LightEffect> DynamicEffects;
        List<LightEffect> StaticEffects;

        RenderTexture RenderTexture;
        Shader BlurEffect;

        public Color GlobalColor { get; set; }

        bool IsRefreshed;

        Dictionary<int, List<OpacityWall>> Walls;
        Dictionary<int, List<OpacityBox>> OpacityBoxes;

        public bool IsActive { get; set; }

        #endregion Members

        public LightEffectManager(Map parent)
        {
            Parent = parent;

            DynamicEffects = new List<LightEffect>();
            StaticEffects = new List<LightEffect>();

            GlobalColor = DEFAUT_GLOBAL_COLOR;

            IsRefreshed = false;

            RenderTexture = new RenderTexture(GameData.WINDOW_WIDTH, GameData.WINDOW_HEIGHT);
            BlurEffect = new Shader(GameData.DATAS_DEFAULT_PATH + "gfx/blur.sfx");
            BlurEffect.SetCurrentTexture("texture");
            BlurEffect.SetParameter("offset", 0.005F * SMOOTHNESS);

            Walls = null;
            OpacityBoxes = null;
        }

        public LightEffectManager(Map parent, LightEffectManager copy)
            : this(parent)
        {
        }

        public void Init()
        {
            if (!IsActive)
                return;

            Generate(true);
        }

        public void Update(Time dt)
        {
            if (!IsActive)
                return;

            Generate();
        }

        void Generate(bool generateStatic = false)
        {
            foreach (LightEffect effect in DynamicEffects)
                GenerateEffect(effect);

            if (!generateStatic)
                return;

            foreach (LightEffect effect in StaticEffects)
                GenerateEffect(effect);
        }

        public void GenerateEffect(LightEffect effect)
        {
            effect.Generate(GetWalls(effect.Z));
        }

        public void Draw(RenderTarget window)
        {
            if (!IsActive)
                return;

            RenderTexture.SetView(window.GetView());
            RenderTexture.Clear(GlobalColor);

            foreach (LightEffect effect in DynamicEffects)
                if (WindowContainsLight(window, effect))
                    effect.Draw(RenderTexture);

            foreach (LightEffect effect in StaticEffects)
                if (WindowContainsLight(window, effect))
                    effect.Draw(RenderTexture);

            RenderTexture.Display();

            Sprite sprite = new Sprite(RenderTexture.Texture);
            sprite.BlendMode = BlendMode.Multiply;
            sprite.Position = window.GetView().Center - window.GetView().Size / 2F;
            window.Draw(sprite, BlurEffect);
        }

        public void AddDynamicEffect(LightEffect effect)
        {
            DynamicEffects.Add(effect);

            GenerateEffect(effect);
        }

        public void RemoveDynamicEffect(LightEffect effect)
        {
            DynamicEffects.Remove(effect);
        }

        public void AddStaticEffect(LightEffect effect)
        {
            StaticEffects.Add(effect);

            GenerateEffect(effect);
        }

        public void RemoveStaticEffect(LightEffect effect)
        {
            StaticEffects.Remove(effect);
        }

        List<OpacityWall> GetWalls(int z)
        {
            if (Walls == null)
                Walls = new Dictionary<int, List<OpacityWall>>();

            if (Walls.ContainsKey(z))
                return Walls[z];

            Walls.Add(z, new List<OpacityWall>());
            foreach (OpacityBox opacityBox in GetOpacityBoxes(z))
                Walls[z].AddRange(opacityBox.GetActiveWalls());

            return Walls[z];
        }

        List<OpacityBox> GetOpacityBoxes(int z)
        {
            if (OpacityBoxes == null)
                OpacityBoxes = new Dictionary<int, List<OpacityBox>>();

            if (OpacityBoxes.ContainsKey(z))
                return OpacityBoxes[z];

            OpacityBoxes.Add(z, new List<OpacityBox>());
            foreach (WorldElement wElem in Parent.GetElements())
            {
                IEnumerator<OpacityBox> opacityBoxEnum = wElem.GetOpacityBoxesEnumerator();
                while (opacityBoxEnum.MoveNext())
                    if (opacityBoxEnum.Current.GetZ() >= z)
                        OpacityBoxes[z].Add(opacityBoxEnum.Current);
            }

            return OpacityBoxes[z];
        }

        bool WindowContainsLight(RenderTarget window, LightEffect effect)
        {
            const float DRAW_MARGIN = 20F;
            Vector2f viewTopLeft = window.GetView().Center - window.GetView().Size / 2F;
            Vector2f viewBottomRight = viewTopLeft + window.GetView().Size;

            FloatRect viewRect = new FloatRect(
                viewTopLeft.X - DRAW_MARGIN,
                viewTopLeft.Y - DRAW_MARGIN,
                viewBottomRight.X + DRAW_MARGIN,
                viewBottomRight.Y + DRAW_MARGIN);

            return !(
                effect.Right < viewRect.Left ||
                effect.Bottom < viewRect.Top ||
                effect.Left >= viewRect.Right ||
                effect.Top >= viewRect.Bottom);
        }

        public void Clear()
        {
            DynamicEffects.Clear();
            StaticEffects.Clear();

            Walls.Clear();
            Walls = null;
            OpacityBoxes.Clear();
            OpacityBoxes = null;
        }
    }
}
