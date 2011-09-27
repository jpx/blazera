using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class DynamicObjectDictionary
    {
        Dictionary<int, DynamicWorldObject> Objects;

        public DynamicObjectDictionary()
        {
            Objects = new Dictionary<int, DynamicWorldObject>();
        }

        public void Add(DynamicWorldObject dObj)
        {
            if (Objects.ContainsKey(dObj.Guid))
                Objects[dObj.Guid] = dObj;
            else
                Objects.Add(dObj.Guid, dObj);
        }

        public bool Remove(int guid)
        {
            return Objects.Remove(guid);
        }

        public bool Remove(DynamicWorldObject dObj)
        {
            return Remove(dObj.Guid);
        }

        public DynamicWorldObject this[int guid]
        {
            get { return Objects[guid]; }
        }

        public int GetCount()
        {
            return Objects.Count;
        }

        public Dictionary<int, DynamicWorldObject>.ValueCollection.Enumerator GetEnumerator()
        {
            return Objects.Values.GetEnumerator();
        }
    }
}
