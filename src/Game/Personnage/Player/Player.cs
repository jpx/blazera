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

        public override object Clone()
        {
            return new Player(this);
        }

        public override void ToScript()
        {
            Sw = new ScriptWriter(this);
            Sw.InitObject();
            base.ToScript();
            Sw.EndObject();
        }
    }
}
