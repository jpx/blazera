using System.Collections;

namespace BlazeraLib
{
    public class StateTable
    {
        #region Members

        static int CurrentCount = 0;

        Hashtable Table;

        #endregion

        public StateTable()
        {
            Table = new Hashtable();
        }

        public void Add(object obj)
        {
            if (Table.ContainsKey(obj))
                return;

            Table.Add(obj, CurrentCount++);
        }

        public void Remove(object obj)
        {
            Table.Remove(obj);
        }

        public int GetId(object obj)
        {
            return (int)Table[obj];
        }
    }
}
