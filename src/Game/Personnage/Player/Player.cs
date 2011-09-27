using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class Player : Personnage
    {
        public Inventory Inventory { get; private set; }

        public Player() :
            base()
        {
            Inventory = new Inventory();
        }

        public Player(Player copy) :
            base(copy)
        {
            Inventory = new Inventory(copy.Inventory);
        }

        public override void ToScript()
        {
            this.Sw = new ScriptWriter(this);
            this.Sw.InitObject();
            base.ToScript();
            this.Sw.EndObject();
        }
    }
}
