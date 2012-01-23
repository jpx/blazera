using System.Collections.Generic;

using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    public abstract class WorldElement : WorldObject
    {
        #region Members

        List<OpacityBox> OpacityBoxes;
        List<LightEffectHandler> LightEffects;

        #endregion Members

        public WorldElement() :
            base()
        {
            OpacityBoxes = new List<OpacityBox>();
            LightEffects = new List<LightEffectHandler>();
        }

        public WorldElement(WorldElement copy) :
            base(copy)
        {
            OpacityBoxes = new List<OpacityBox>(copy.OpacityBoxes.Count);
            foreach (OpacityBox opacityBox in copy.OpacityBoxes)
                AddOpacityBox(new OpacityBox(opacityBox));

            LightEffects = new List<LightEffectHandler>(copy.LightEffects.Count);
            foreach (LightEffectHandler lightEffect in copy.LightEffects)
                AddLightEffect(new LightEffectHandler(this, lightEffect));
        }

        public void AddOpacityBox(OpacityBox opacityBox)
        {
            opacityBox.SetParent(this);

            OpacityBoxes.Add(opacityBox);
        }

        public bool RemoveOpacityBox(OpacityBox opacityBox)
        {
            return OpacityBoxes.Remove(opacityBox);
        }

        public void ClearOpacityBoxes()
        {
            foreach (OpacityBox opacityBox in OpacityBoxes)
                RemoveOpacityBox(opacityBox);
        }

        public IEnumerator<OpacityBox> GetOpacityBoxesEnumerator()
        {
            return OpacityBoxes.GetEnumerator();
        }

        void AddLightEffect(LightEffectHandler lightEffect)
        {
            LightEffects.Add(lightEffect);

            if (Map != null)
                Map.AddStaticLightEffect(lightEffect.Effect);
        }

        public void AddLightEffect(LightEffect lightEffect)
        {
            AddLightEffect(new LightEffectHandler(this, lightEffect));
        }

        public void AddLightEffect(LightEffect lightEffect, Vector2f basePosition, int baseZ)
        {
            AddLightEffect(new LightEffectHandler(this, lightEffect, basePosition, baseZ));
        }

        public void AddLightEffect(LightEffect lightEffect, Vector2f basePosition)
        {
            AddLightEffect(new LightEffectHandler(this, lightEffect, basePosition));
        }

        public void AddLightEffect(LightEffect lightEffect, int baseZ)
        {
            AddLightEffect(new LightEffectHandler(this, lightEffect, baseZ));
        }

        public bool RemoveLightEffect(LightEffect lightEffect)
        {
            foreach (LightEffectHandler lightEffectHandler in LightEffects)
            {
                if (lightEffect != lightEffectHandler.Effect)
                    continue;

                if (Map != null)
                    Map.RemoveStaticLightEffect(lightEffect);

                return LightEffects.Remove(lightEffectHandler);
            }

            return false;
        }

        public void ClearLightEffects()
        {
            foreach (LightEffectHandler lightEffect in LightEffects)
                RemoveLightEffect(lightEffect.Effect);
        }

        public override void SetMap(Map map, float x, float y, int z = BaseDrawable.DEFAULT_Z)
        {
            base.SetMap(map, x, y, z);

            foreach (LightEffectHandler lightEffect in LightEffects)
                Map.AddStaticLightEffect(lightEffect.Effect);
        }

        public override void UnsetMap()
        {
            foreach (LightEffectHandler lightEffect in LightEffects)
                Map.RemoveStaticLightEffect(lightEffect.Effect);

            base.UnsetMap();
        }
    }
}
