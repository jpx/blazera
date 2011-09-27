using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public enum EquipmentField
    {
        Tete,
        Dos,
        Jambes,
        Cou,
        Bras,
        Mains,
        Doigts,
        Corps,
        Taille
    };

    public class Equipment : Dictionary<EquipmentField, Item>
    {
        public Equipment() :
            base()
        {
        
        }
    }
}
