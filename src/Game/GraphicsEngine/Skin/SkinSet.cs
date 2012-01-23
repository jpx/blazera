using System.Collections.Generic;

using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    public class SkinSet : Skin
    {
        #region Constants

        public static readonly string DEFAULT_DEFAULT_STATE = "Inactive";

        #endregion Constants

        #region Members

        Dictionary<State<string>, Skin> Skins;
        string CurrentState;
        string DefaultState;

        Skin CurrentSkin;

        #endregion Members

        public SkinSet()
            : base()
        {
            Skins = new Dictionary<State<string>, Skin>();
            CurrentState = null;
            DefaultState = null;
            CurrentSkin = null;
        }

        public SkinSet(SkinSet copy)
            : base(copy)
        {
            Skins = new Dictionary<State<string>, Skin>();
            foreach (KeyValuePair<State<string>, Skin> skin in copy.Skins)
                AddSkin(skin.Key.Value, (Skin)skin.Value.Clone());

            SetState(copy.CurrentState);
            DefaultState = copy.DefaultState;
        }

        public override object Clone()
        {
            return new SkinSet(this);
        }

        Skin GetCurrentSkin()
        {
            if (CurrentState == null || !Skins.ContainsKey(CurrentState))
                return null;

            return Skins[CurrentState];
        }

        public override Texture GetTexture()
        {
            if (CurrentSkin == null)
                return null;

            return CurrentSkin.GetTexture();
        }

        public override void Update(Time dt)
        {
            if (CurrentSkin != null)
                CurrentSkin.Update(dt);
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            if (CurrentSkin != null)
                CurrentSkin.Draw(window);
        }

        public override void Start()
        {
            if (CurrentSkin != null)
                CurrentSkin.Start();
        }

        public override void Stop()
        {
            if (CurrentSkin != null)
                CurrentSkin.Stop();
        }

        public bool ContainsState(string state)
        {
            return Skins.ContainsKey(state);
        }

        public void AddSkin(string state, Skin skin, bool currentState = false)
        {
            if (Skins.ContainsKey(state))
                return;

            Skins.Add(state, skin);

            if (CurrentState == null || currentState)
                SetCurrentState(state);
        }

        public void AddSkin(Skin skin)
        {
            AddSkin(DEFAULT_DEFAULT_STATE, skin, true);
        }

        public bool RemoveSkin(string state)
        {
            return Skins.Remove(state);
        }

        public void SetCurrentState(string state)
        {
            SetState(state);

            if (DefaultState == null)
                DefaultState = CurrentState;
        }

        void SetState(string state)
        {
            CurrentState = state;

            CurrentSkin = GetCurrentSkin();
        }

        public Skin GetSkin(string state)
        {
            return Skins[state];
        }

        public IEnumerator<KeyValuePair<State<string>, Skin>> GetEnumerator()
        {
            return Skins.GetEnumerator();
        }

        public void Reset()
        {
            SetState(DefaultState);
        }

        public void SetDefaultState(string state)
        {
            DefaultState = state;
        }

        public int GetCount()
        {
            return Skins.Count;
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                if (Skins == null)
                    return;

                foreach (Skin skin in Skins.Values)
                    skin.Position = Position;
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (CurrentSkin == null)
                    return base.Dimension;
                return CurrentSkin.Dimension;
            }
            set
            {
                base.Dimension = value;

                if (Skins == null)
                    return;

                Vector2f factor = new Vector2f(
                    value.X / CurrentSkin.Dimension.X,
                    value.Y / CurrentSkin.Dimension.Y);

                foreach (Skin skin in Skins.Values)
                    skin.Dimension = new Vector2f(
                        skin.Dimension.X * factor.X,
                        skin.Dimension.Y * factor.Y);
            }
        }

        public override Vector2f BasePoint
        {
            get
            {
                if (CurrentSkin == null)
                    return base.BasePoint;
                return CurrentSkin.BasePoint;
            }
            set
            {
                base.BasePoint = value;

                if (Skins == null)
                    return;

                foreach (Skin skin in Skins.Values)
                    skin.BasePoint = BasePoint;
            }
        }

        public override Color Color
        {
            set
            {
                base.Color = value;

                if (Skins == null)
                    return;

                foreach (Skin skin in Skins.Values)
                    skin.Color = Color;
            }
        }
    }
}
