using System.Collections.Generic;

namespace BlazeraLib.Game.Pathfinding
{
    public class Pathfinding
    {
        #region Constants

        #endregion

        #region Members

        #region Static members

        const double STANDARDIZATION_FACTOR = 1D;
        static Cost Distance(Node node, Node goalNode)
        {
            return new Cost(
                (int)(System.Math.Abs(node.Position.X - goalNode.Position.X) * STANDARDIZATION_FACTOR) +
                (int)(System.Math.Abs(node.Position.Y - goalNode.Position.Y) * STANDARDIZATION_FACTOR));
        }

        #endregion

        Node[,] Nodes;
        Queue<Node> ProcessedNodes;
        List<Node> LProcessedNodes;
        Node StartNode;
        Node GoalNode;

        int Width;
        int Height;

        CostComputer CostComputer = Distance;

        #endregion

        public Pathfinding()
        {
            ProcessedNodes = new Queue<Node>();
            LProcessedNodes = new List<Node>();
        }

        public void InitNodeSet(CombatMap map)
        {
            Width = map.Width;
            Height = map.Height;

            Nodes = new Node[Height, Width];

            BuildNodeSet(map);
        }

        void BuildNodeSet(CombatMap map)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Node node = new Node(new Vector2I(x, y));
                    node.SetWalkable(map.Combat.GetCell(x, y).IsUsable());
                    SetNode(x, y, node);
                }
            }
        }

        // only if we use heuristics
        void PrecomputeCosts()
        {
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                    GetNode(x, y).Cost.SetValue(CostComputer(GetNode(x, y), GoalNode).Value);
        }

        void Reset()
        {
            ProcessedNodes.Clear();

            foreach (Node node in LProcessedNodes)
                node.Reset();
            LProcessedNodes.Clear();
        }

        public List<Vector2I> FindPath(Vector2I startNode, Vector2I goalNode)
        {
            Reset();

            StartNode = GetNode(startNode.X, startNode.Y);
            GoalNode = GetNode(goalNode.X, goalNode.Y);

            AddNode(null, StartNode.Position.X, StartNode.Position.Y);

            while (ProcessedNodes.Count > 0)
            {
                Node currentNode = ProcessedNodes.Dequeue();

                if (currentNode.Position == goalNode)
                    return GetPath();

                AddNode(currentNode, currentNode.Position.X + 1, currentNode.Position.Y);
                AddNode(currentNode, currentNode.Position.X - 1, currentNode.Position.Y);
                AddNode(currentNode, currentNode.Position.X, currentNode.Position.Y + 1);
                AddNode(currentNode, currentNode.Position.X, currentNode.Position.Y - 1);
            }

            return null;
        }

        Node GetNode(int x, int y)
        {
            if (x < 0 || y < 0 ||
                x >= Width || y >= Height)
                return null;

            return Nodes[y, x];
        }

        void SetNode(int x, int y, Node node)
        {
            Nodes[y, x] = node;
        }

        void AddNode(Node parent, int x, int y)
        {
            Node addedNode = GetNode(x, y);

            if (addedNode == null ||
                addedNode.IsVisited ||
                !addedNode.IsWalkable)
                return;

            addedNode.Visit();

            addedNode.SetParent(parent);

            ProcessedNodes.Enqueue(addedNode);
            LProcessedNodes.Add(addedNode);

            return;
        }

        List<Vector2I> GetPath()
        {
            List<Vector2I> path = new List<Vector2I>();
            Node parentNode = GoalNode;

            while (parentNode != null)
            {
                path.Insert(0, parentNode.Position);
                parentNode = parentNode.Parent;
            }

            return path;
        }
    }

    public delegate Cost CostComputer(Node node, Node goalNode);
}
