namespace BlazeraLib
{
    public class RandomEffectAreaSpell : CombatSpell
    {
        #region Constants

        const double DEFAULT_EFFECT_DELAY = .2D;
        const uint DEFAULT_EFFECT_COUNT = 10;

        #endregion

        #region Members

        Timer Timer;
        uint CurrentEffectCount;
        bool IsEnding;

        #endregion

        public RandomEffectAreaSpell(Spell spell, BaseCombatant holder) :
            base(spell, holder)
        {
            Timer = new Timer();
            CurrentEffectCount = 0;
            IsEnding = false;
        }

        public override void Launch(Vector2I cellPosition)
        {
            Timer.Start();
            LaunchingCellPosition = cellPosition;
        }

        public override void Perform()
        {
            if (IsEnding)
                return;

            if (!Timer.IsDelayCompleted(DEFAULT_EFFECT_DELAY))
                return;

            AnimationMapEffect effect = new AnimationMapEffect(Spell.Effect);
            Vector2I launchingPosition = Spell.RangeArea.GetRandomCellPosition(LaunchingCellPosition);

            MapEffectManager.Instance.AddEffect(effect, CombatCell.GetCenterFromCellPosition(launchingPosition));

            foreach (CombatCell cell in GetCombat().GetCellFromArea(Spell.EffectArea, launchingPosition))
                foreach (BaseCombatant combatant in cell.Combatants)
                    combatant.TakeDamage();

            ++CurrentEffectCount;

            if (CurrentEffectCount >= DEFAULT_EFFECT_COUNT)
            {
                IsEnding = true;
                effect.OnStopping += new MapEffectEventHandler(effect_OnStopping);
                return;
            }
        }

        void effect_OnStopping(MapEffect sender, MapEffectEventArgs e)
        {
            End();
        }
    }
}
