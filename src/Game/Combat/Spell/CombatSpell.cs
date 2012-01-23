namespace BlazeraLib
{
    #region Event handlers

    public class CombatSpellEventArgs : System.EventArgs { }
    public delegate void CombatSpellEventHandler(CombatSpell sender, CombatSpellEventArgs e);

    #endregion

    public class CombatSpell
    {
        #region Members

        protected Spell Spell { get; private set; }
        protected Vector2I LaunchingCellPosition;
        protected BaseCombatant Holder { get; private set; }

        #endregion

        #region Events

        public event CombatSpellEventHandler OnStarting;
        public event CombatSpellEventHandler OnStopping;

        protected bool CallOnStarting() { if (OnStarting == null) return false; OnStarting(this, new CombatSpellEventArgs()); return true; }
        protected bool CallOnStopping() { if (OnStopping == null) return false; OnStopping(this, new CombatSpellEventArgs()); return true; }

        #endregion

        public CombatSpell(Spell spell, BaseCombatant holder)
        {
            Spell = spell;

            Holder = holder;
        }

        public virtual void Launch(Vector2I cellPosition)
        {
            LaunchingCellPosition = cellPosition;

            AnimationMapEffect effect = new AnimationMapEffect(Spell.Effect);
            effect.OnStopping += new MapEffectEventHandler(Effect_OnStopping);

            MapEffectManager.Instance.AddEffect(effect, CombatCell.GetCenterFromCellPosition(LaunchingCellPosition));
            
            foreach (CombatCell cell in GetCombat().GetCellFromArea(Spell.EffectArea, LaunchingCellPosition))
                foreach (BaseCombatant combatant in cell.Combatants)
                    combatant.TakeDamage();
        }

        void Effect_OnStopping(MapEffect sender, MapEffectEventArgs e)
        {
            Spell.Effect.OnStopping -= new MapEffectEventHandler(Effect_OnStopping);
            End();
        }

        public virtual void Perform()
        {

        }

        public virtual void End()
        {
            CallOnStopping();
        }

        protected Combat GetCombat()
        {
            return Holder.Map.Combat;
        }

        public string GetName()
        {
            return Spell.Name;
        }
    }
}
