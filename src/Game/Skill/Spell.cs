using System.Collections.Generic;
using SFML.Graphics;

namespace BlazeraLib
{
    #region Enums

    public enum SpellType
    {
        Aggressive,
        Heal,
        Bonus,
        Malus
    }

    #endregion

    public class Spell : BaseObject
    {
        #region Constants

        const int EFFECT_DEFAULT_FRAMERATE = 15;

        #endregion

        #region Members

        public AnimationMapEffect Effect { get; private set; }

        public CellArea RangeArea { get; private set; }
        public CellArea EffectArea { get; private set; }

        SpellType Type;

        uint BaseValue;
        uint MargeValue;

        uint BaseSPCost;

        SpellLimitation Limitation;

        uint TurnDelay;

        ElementalCaracteristic Element;

        List<StatusEffect> Effects;

        #endregion

        public Spell() :
            base()
        {
            Effect = new AnimationMapEffect();
        }

        public Spell(Spell copy) :
            base(copy)
        {
            Effect = new AnimationMapEffect(copy.Effect);

            RangeArea = new CellArea(copy.RangeArea);
            EffectArea = new CellArea(copy.EffectArea);
        }

        public void InitEffect(
            string animationType, string soundName,
            CellAreaType rangeAreaType, int rangeAreaMinRange, int rangeAreaMaxRange,
            CellAreaType effectAreaType, int effectAreaMinRange, int effectAreaMaxRange)
        {
            Effect.Init(animationType, soundName);

            RangeArea = new CellArea(rangeAreaType, rangeAreaMinRange, rangeAreaMaxRange);
            EffectArea = new CellArea(effectAreaType, effectAreaMinRange, effectAreaMaxRange);
        }
    }

    public class SpellLimitation
    {
        #region Constants

        const bool DEFAULT_IS_TARGET_LIMITED = false;
        const bool DEFAULT_IS_TURN_LIMITED = false;

        const uint DEFAULT_TARGET_LIMITATION = 0;
        const uint DEFAULT_TURN_LIMITATION = 0;

        #endregion

        #region Members

        public bool IsTargetLimited { get; private set; }
        public bool IsTurnLimited { get; private set; }

        public uint TargetLimitation { get; private set; }
        public uint TurnLimitation { get; private set; }

        #endregion

        public SpellLimitation()
        {
            IsTargetLimited = DEFAULT_IS_TARGET_LIMITED;
            IsTurnLimited = DEFAULT_IS_TURN_LIMITED;

            TargetLimitation = DEFAULT_TARGET_LIMITATION;
            TurnLimitation = DEFAULT_TURN_LIMITATION;
        }

        public void SetTargetLimitation(uint targetLimitation)
        {
            IsTargetLimited = true;

            TargetLimitation = targetLimitation;
        }

        public void SetTurnLimitation(uint turnLimitation)
        {
            IsTurnLimited = true;

            TurnLimitation = turnLimitation;
        }
    }
}
