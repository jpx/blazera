namespace BlazeraLib
{
    public class PlayerCombatant : BaseCombatant
    {
        public PlayerCombatant(Personnage basePersonnage) :
            base(basePersonnage)
        {

        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
