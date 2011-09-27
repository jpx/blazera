using System.Collections.Generic;

namespace BlazeraLib
{
    public class MapEffectManager
    {
        #region Singleton

        static MapEffectManager _instance;
        public static MapEffectManager Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new MapEffectManager();
                return _instance;
            }
            set { _instance = value; }
        }

        #endregion

        #region Members

        Queue<MapEffect> NewEffects;
        Queue<MapEffect> OldEffects;
        List<MapEffect> Effects;

        #endregion

        MapEffectManager()
        {
            NewEffects = new Queue<MapEffect>();
            OldEffects = new Queue<MapEffect>();
            Effects = new List<MapEffect>();
        }

        public void AddEffect(MapEffect effect, SFML.Graphics.Vector2 position)
        {
            effect.SetBasePosition(position);
            NewEffects.Enqueue(effect);
        }

        public bool IsEmpty()
        {
            return NewEffects.Count == 0;
        }

        public MapEffect GetEffect()
        {
            MapEffect effect = NewEffects.Dequeue();
            effect.OnStopping += new MapEffectEventHandler(effect_OnStopping);
            Effects.Add(effect);
            return effect;
        }

        void effect_OnStopping(MapEffect sender, MapEffectEventArgs e)
        {
            OldEffects.Enqueue(sender);
        }

        public void Update(Time dt)
        {
            while (OldEffects.Count > 0)
                Effects.Remove(OldEffects.Dequeue());

            foreach (MapEffect effect in Effects)
                effect.Update(dt);
        }
    }
}
