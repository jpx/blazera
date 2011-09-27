using System.Collections.Generic;

namespace BlazeraLib.Game.Pathfinding
{
    public class Node
    {
        #region Classes

        #region Comparer

        public class Comparer : IComparer<Node>
        {
            public int Compare(Node n1, Node n2)
            {
                return n1.Cost.Value - n2.Cost.Value;
            }
        }

        #endregion

        #endregion

        #region Constants

        bool DEFAULT_WALKABLE_STATE = true;

        #endregion

        #region Members

        public bool IsWalkable { get; private set; }
        public Cost Cost { get; private set; }

        public bool IsVisited { get; private set; }

        public Vector2I Position { get; private set; }
        public Node Parent { get; private set; }

        #endregion

        public Node(Vector2I position)
        {
            IsWalkable = DEFAULT_WALKABLE_STATE;
            Cost = new Cost();

            IsVisited = false;

            Position = position;
        }

        public void Visit()
        {
            IsVisited = true;
        }

        public void SetWalkable(bool isWalkable = true)
        {
            IsWalkable = isWalkable;
        }

        public void SetParent(Node parent)
        {
            Parent = parent;
        }

        public void Reset()
        {
            IsVisited = false;
            Parent = null;
        }
    }

    public class Path
    {

    }

    public class Cost
    {
        #region Constants

        const int DEFAULT_COST_VALUE = 1;

        #endregion

        #region Members

        public bool IsUndefined { get; private set; }
        public int Value { get; private set; }

        #endregion

        public Cost(int value = DEFAULT_COST_VALUE)
        {
            SetValue(value);
        }

        public void SetValue(int value)
        {
            IsUndefined = false;

            Value = value;
        }
    }
}
