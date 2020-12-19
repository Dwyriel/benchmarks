using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Benchmarks
{
    class Pathfinding
    {
        public static void Benchmark(int threads)
        {
            Stopwatch s = new Stopwatch();
            Random rng = new Random();
            Console.WriteLine("How many iterations? (int)");
            BigInteger iterations = BigInteger.Parse(Console.ReadLine());
            Console.WriteLine("How many nodes on X axis? (int)");
            int x = int.Parse(Console.ReadLine());
            Console.WriteLine("How many nodes on Y axis? (int)");
            int y = int.Parse(Console.ReadLine());
            Console.WriteLine("Press enter to start");
            Console.ReadLine();
            NodeManager.CreatNodes(x, y);
            BigInteger i = 0;
            while(i < iterations)
            {
                i++;
                Vector2 startPos = new Vector2(rng.Next(0, x + 1), rng.Next(0, y + 1));
                Vector2 endPos = new Vector2(rng.Next(0, x + 1), rng.Next(0, y + 1));
                Console.WriteLine("\nStarting on position: " + startPos + " Ending on position: " + endPos);
                Stack<Node> path = AStar.Pathfinding(startPos, endPos);
            }
        }
    }

    class AStar
    {
        public static Stack<Node> Pathfinding(Vector2 start, Vector2 end)
        {
            Stack<Node> newPath = PathfindingCode(start, end);
            return newPath;
        }

        private static Stack<Node> PathfindingCode(Vector2 start, Vector2 end)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            NodeManager.ResetAllNodesInt();
            Stack<Node> path = new Stack<Node>();
            Node startNode = NodeManager.FindClosestNodeInt(start);
            Node endNode = NodeManager.FindClosestNodeInt(end);
            List<Node> closedList = new List<Node>();
            List<Node> openList = new List<Node>();
            List<Node> connectedNodes = new List<Node>();
            Node current = startNode;
            openList.Add(startNode);
            while (openList.Count > 0 && !closedList.Exists(nNode => nNode.position == endNode.position))
            {
                current = openList[0];
                openList.Remove(current);
                closedList.Add(current);
                connectedNodes = NodeManager.GetConnectedNodesInt(current);
                foreach (Node node in connectedNodes)
                {
                    if (!closedList.Contains(node) && node.isWalkable)
                    {
                        if (!openList.Contains(node))
                        {
                            node.parent = current;
                            node.distanceToTarget = Math.Abs(node.position.x - endNode.position.x) + Math.Abs(node.position.y - endNode.position.y);
                            node.cost = node.weight + node.parent.cost;
                            openList.Add(node);
                            openList = openList.OrderBy(n => n.F).ToList<Node>();
                        }
                    }
                }
            }
            if (!closedList.Exists(n => n.position == endNode.position))
                return null;
            Node temp = closedList[closedList.IndexOf(current)];
            if (temp == null)
                return null;
            do
            {
                path.Push(temp);
                temp = temp.parent;
            } while (temp != startNode && temp != null);
            sw.Stop();
            Console.WriteLine("Took " + sw.Elapsed);
            return path;
        }
    }

    class NodeManager
    {
        public static Dictionary<Vector2Int, Node> DictNodes = new Dictionary<Vector2Int, Node>();
        private static readonly Vector2Int[] directions = { new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1) };

        public static void CreatNodes(int gridMaxX, int gridMaxY)
        {
            int gridMinY = 0;
            int gridMinX = 0;
            Vector2Int currentPos = new Vector2Int(gridMinX, gridMaxY);
            while (currentPos.y >= gridMinY)
            {
                currentPos.x = gridMinX;
                while (currentPos.x <= gridMaxX)
                {
                    Node instantiatedNode = new Node(new Vector2Int(currentPos.x, currentPos.y));
                    DictNodes.Add(new Vector2Int(currentPos.x, currentPos.y), instantiatedNode);
                    currentPos.x++;
                }
                currentPos.y--;
            }
            return;
        }

        /* Start of node manipulation using Dictionary and Vector2Int */
        public static void ResetAllNodesInt()
        {
            foreach (Node node in DictNodes.Values)
            {
                node.ResetNode();
            }
        }

        //won't work outside the grid
        public static Node FindClosestNodeInt(Vector2 position)
        {
            Vector2Int convertedPos = new Vector2Int((int)position.x, (int)position.y);
            Node returnedNode;
            if (DictNodes.ContainsKey(convertedPos))
            {
                DictNodes.TryGetValue(convertedPos, out returnedNode);
                return returnedNode;
            }
            return null;
        }

        public static Node FindClosestNodeOutsideGridInt(Vector2 position)
        {
            Node bestMatch = null;
            float distance;
            float previousDistance = float.PositiveInfinity;
            foreach (Node node in DictNodes.Values)
            {
                distance = Vector2.Distance(position, new Vector2(node.position.x, node.position.y));
                if (distance < previousDistance)
                    bestMatch = node;
                previousDistance = distance;
            }
            return bestMatch;
        }

        public static Node FindExactNodeInt(Vector2Int position)
        {
            if (DictNodes.TryGetValue(position, out Node returningNode))
                return returningNode;
            return null;
        }

        public static List<Node> GetConnectedNodesInt(Node node)
        {
            List<Node> connectedNodes = new List<Node>();
            int posX = (int)node.position.x;
            int posY = (int)node.position.y;
            Vector2Int v2Int = new Vector2Int(posX, posY);
            foreach (Vector2Int dir in directions)
            {
                connectedNodes.Add(FindExactNodeInt(new Vector2Int(v2Int.x + dir.x, v2Int.y + dir.y)));
            }
            return connectedNodes;
        }
    }

    class Node
    {
        public bool isWalkable = true;
        public Node parent;
        public Vector2Int position;
        public Vector2 center { get { return new Vector2(position.x + .5f, position.y + .5f); } }
        public float? distanceToTarget, cost, weight;
        public float? F { get { return (distanceToTarget != null && cost != null) ? (distanceToTarget + cost) : null; } }

        public Node(Vector2Int pos, float weight = 1)
        {
            this.parent = null;
            this.position = pos;
            distanceToTarget = null;
            cost = 1;
            this.weight = weight;
        }

        public void ResetNode()
        {
            this.parent = null;
            this.distanceToTarget = null;
            this.cost = 1;
        }
    }
}
