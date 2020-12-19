using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Benchmarks
{
    class Pathfinding
    {
        public static void Benchmark(int threadCount)
        {
            BigInteger totalTime = 0;
            List<BigInteger> firstMap = new List<BigInteger>(), secondMap = new List<BigInteger>(), thirdMap = new List<BigInteger>();
            List<Thread> threads = new List<Thread>();
            Console.WriteLine("How many iterations? (int)");
            BigInteger iterations = BigInteger.Parse(Console.ReadLine());
            Console.WriteLine("Press enter to start");
            Console.ReadLine();
            if (iterations < threadCount && iterations > 0)
                threadCount = (int)iterations;
            BigInteger iterationsPerThread = iterations / threadCount;
            for (int tCount = 0; tCount < threadCount; tCount++)
            {
                Thread newThread = new Thread(() =>
                {
                    NodeManager nM = new NodeManager();
                    nM.CreatMaps();
                    BigInteger totalThreadTime = 0;
                    BigInteger i = 0;
                    while (i < iterationsPerThread)
                    {
                        i++;
                        BigInteger iterationTime = 0;
                        Stopwatch s = new Stopwatch();
                        Stopwatch s2 = new Stopwatch();
                        s.Start();
                        s2.Start();
                        Vector2 startPos = new Vector2(20, 140);
                        Vector2 endPos = new Vector2(180, 15);
                        Stack<Node> path = AStar.Pathfinding(startPos, endPos, nM.Maps[0]);
                        s2.Stop();
                        firstMap.Add(s2.ElapsedMilliseconds);
                        Console.WriteLine("First map done\n");
                        s2.Restart();
                        startPos = new Vector2(20, 140);
                        endPos = new Vector2(180, 140);
                        path = AStar.Pathfinding(startPos, endPos, nM.Maps[1]);
                        s2.Stop();
                        secondMap.Add(s2.ElapsedMilliseconds);
                        Console.WriteLine("Second map done\n");
                        s2.Restart();
                        startPos = new Vector2(10, 145);
                        endPos = new Vector2(115, 80);
                        path = AStar.Pathfinding(startPos, endPos, nM.Maps[2]);
                        s.Stop();
                        s2.Stop();
                        thirdMap.Add(s2.ElapsedMilliseconds);
                        Console.WriteLine("Third map done\n");
                        iterationTime = s.ElapsedMilliseconds;
                        totalThreadTime += iterationTime;
                        Console.WriteLine("Total iteration time: " + s.ElapsedMilliseconds);
                    }
                    totalTime += totalThreadTime;
                    Console.WriteLine("Total thread time: " + totalThreadTime + "\n");
                });
                threads.Add(newThread);
                newThread.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            BigInteger totalmap = 0, count = 0;
            foreach (BigInteger value in firstMap)
            {
                count++;
                totalmap += value;
            }
            Console.WriteLine("Avarage time on firt map: " + totalmap);
            totalmap = 0; count = 0;
            foreach (BigInteger value in secondMap)
            {
                count++;
                totalmap += value;
            }
            Console.WriteLine("Avarage time on second map: " + totalmap);
            totalmap = 0; count = 0;
            foreach (BigInteger value in thirdMap)
            {
                count++;
                totalmap += value;
            }
            Console.WriteLine("Avarage time on third map: " + totalmap);
            Console.WriteLine("Total benchmark time: " + totalTime);
        }
    }

    class AStar
    {
        public static Stack<Node> Pathfinding(Vector2 start, Vector2 end, Dictionary<Vector2Int, Node> dict = null)
        {
            Stack<Node> newPath = PathfindingCode(start, end, dict ?? null);
            return newPath;
        }

        private static Stack<Node> PathfindingCode(Vector2 start, Vector2 end, Dictionary<Vector2Int, Node> dict = null)
        {
            NodeManager.ResetAllNodesInt(dict ?? null);
            Stack<Node> path = new Stack<Node>();
            Node startNode = NodeManager.FindClosestNodeInt(start, dict ?? null);
            Node endNode = NodeManager.FindClosestNodeInt(end, dict ?? null);
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
                connectedNodes = NodeManager.GetConnectedNodesInt(current, dict ?? null);
                foreach (Node node in connectedNodes)
                {
                    if (node == null)
                        continue;
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
            return path;
        }
    }

    class NodeManager
    {
        public static Dictionary<Vector2Int, Node> DictNodes = new Dictionary<Vector2Int, Node>();
        public List<Dictionary<Vector2Int, Node>> Maps = new List<Dictionary<Vector2Int, Node>>();
        private static readonly Vector2Int[] directions = { new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1) };

        public static void CreatNodes()
        {
            int gridMinY = 0, gridMaxX = 200;
            int gridMinX = 0, gridMaxY = 150;
            Vector2Int currentPos = new Vector2Int(gridMinX, gridMinY);
            while (currentPos.y <= gridMaxY)
            {
                currentPos.x = gridMinX;
                while (currentPos.x <= gridMaxX)
                {
                    Node instantiatedNode = new Node(new Vector2Int(currentPos.x, currentPos.y));
                    DictNodes.Add(new Vector2Int(currentPos.x, currentPos.y), instantiatedNode);
                    currentPos.x++;
                }
                currentPos.y++;
            }
            CreateXAndYWalls(50, 110, 0, 120);
            CreateXAndYWalls(100, 200, 40, 90);
            CreateXAndYWalls(150, 175, 0, 60);
            return;
        }

        public static void CreateXAndYWalls(int startingNodeX, int endingNodeX, int startingNodeY, int endingNodeY)
        {
            int currentMod;
            currentMod = startingNodeY;
            while (currentMod <= endingNodeY)
            {
                if (DictNodes.TryGetValue(new Vector2Int(startingNodeX, currentMod), out Node retriNode))
                    retriNode.isWalkable = false;
                currentMod++;
            }
            currentMod = startingNodeX;
            while (currentMod <= endingNodeX)
            {
                if (DictNodes.TryGetValue(new Vector2Int(currentMod, endingNodeY), out Node retriNode))
                    retriNode.isWalkable = false;
                currentMod++;
            }
        }

        public static void CreateWalls(int startingNodeX, int endingNodeX, int startingNodeY, int endingNodeY, Dictionary<Vector2Int, Node> dict)
        {
            int currentMod;
            currentMod = startingNodeY;
            while (currentMod <= endingNodeY)
            {
                if (dict.TryGetValue(new Vector2Int(startingNodeX, currentMod), out Node retriNode))
                    retriNode.isWalkable = false;
                currentMod++;
            }
            currentMod = startingNodeX;
            while (currentMod <= endingNodeX)
            {
                if (dict.TryGetValue(new Vector2Int(currentMod, endingNodeY), out Node retriNode))
                    retriNode.isWalkable = false;
                currentMod++;
            }
        }

        public static void CreateWallX(int startingNodeX, int endingNodeX, int Y, Dictionary<Vector2Int, Node> dict)
        {
            int currentMod = startingNodeX;
            while (currentMod <= endingNodeX)
            {
                if (dict.TryGetValue(new Vector2Int(currentMod, Y), out Node retriNode))
                    retriNode.isWalkable = false;
                currentMod++;
            }
        }

        public static void CreateWallY(int startingNodeY, int endingNodeY, int X, Dictionary<Vector2Int, Node> dict)
        {
            int currentMod;
            currentMod = startingNodeY;
            while (currentMod <= endingNodeY)
            {
                if (dict.TryGetValue(new Vector2Int(X, currentMod), out Node retriNode))
                    retriNode.isWalkable = false;
                currentMod++;
            }
        }

        public void CreatMaps()
        {
            Dictionary<Vector2Int, Node> newDict = NewMapStruct();
            CreateWalls(50, 110, 0, 120, newDict);
            CreateWalls(100, 200, 40, 90, newDict);
            CreateWalls(150, 175, 0, 60, newDict);
            Maps.Add(newDict);
            Dictionary<Vector2Int, Node> newDict2 = NewMapStruct();
            CreateWallY(75, 150, 100, newDict2);
            CreateWallX(100, 130, 75, newDict2);
            Maps.Add(newDict2);
            Dictionary<Vector2Int, Node> newDict3 = NewMapStruct();
            CreateWallY(20, 150, 20, newDict3);
            CreateWallX(20, 180, 20, newDict3);
            CreateWallY(20, 130, 180, newDict3);
            CreateWallX(50, 180, 130, newDict3);
            CreateWallY(50, 130, 50, newDict3);
            CreateWallX(50, 150, 50, newDict3);
            CreateWallY(50, 100, 150, newDict3);
            CreateWallX(100, 150, 100, newDict3);
            CreateWallY(75, 100, 100, newDict3);
            CreateWallX(100, 125, 75, newDict3);
            CreateWallY(75, 85, 125, newDict3);
            Maps.Add(newDict3);
        }

        public static Dictionary<Vector2Int, Node> NewMapStruct()
        {
            Vector2Int startingPos = new Vector2Int(0, 0);
            Vector2Int endingPos = new Vector2Int(200, 150);
            Vector2Int currentPos = new Vector2Int(startingPos.x, startingPos.y);
            Dictionary<Vector2Int, Node> newDict = new Dictionary<Vector2Int, Node>();
            while (currentPos.x <= endingPos.x)
            {
                currentPos.y = startingPos.y;
                while (currentPos.y <= endingPos.y)
                {
                    Node newNode = new Node(new Vector2Int(currentPos.x, currentPos.y));
                    newDict.Add(new Vector2Int(currentPos.x, currentPos.y), newNode);
                    currentPos.y++;
                }
                currentPos.x++;
            }
            return newDict;
        }

        /* Start of node manipulation using Dictionary and Vector2Int */
        public static void ResetAllNodesInt(Dictionary<Vector2Int, Node> dict = null)
        {
            if (dict == null)
                foreach (Node node in DictNodes.Values)
                {
                    node.ResetNode();
                }
            else
                foreach (Node node in dict.Values)
                {
                    node.ResetNode();
                }
        }

        //won't work outside the grid
        public static Node FindClosestNodeInt(Vector2 position, Dictionary<Vector2Int, Node> dict = null)
        {
            Vector2Int convertedPos = new Vector2Int((int)position.x, (int)position.y);
            Node returnedNode;
            if (dict == null)
            {
                if (DictNodes.ContainsKey(convertedPos))
                {
                    DictNodes.TryGetValue(convertedPos, out returnedNode);
                    return returnedNode;
                }
            }
            else
            {
                if (dict.ContainsKey(convertedPos))
                {
                    dict.TryGetValue(convertedPos, out returnedNode);
                    return returnedNode;
                }
            }
            return null;
        }

        public static Node FindClosestNodeOutsideGridInt(Vector2 position, Dictionary<Vector2Int, Node> dict = null)
        {
            Node bestMatch = null;
            float distance;
            float previousDistance = float.PositiveInfinity;
            if (dict == null)
                foreach (Node node in DictNodes.Values)
                {
                    distance = Vector2.Distance(position, new Vector2(node.position.x, node.position.y));
                    if (distance < previousDistance)
                        bestMatch = node;
                    previousDistance = distance;
                }
            else
                foreach (Node node in dict.Values)
                {
                    distance = Vector2.Distance(position, new Vector2(node.position.x, node.position.y));
                    if (distance < previousDistance)
                        bestMatch = node;
                    previousDistance = distance;
                }
            return bestMatch;
        }

        public static Node FindExactNodeInt(Vector2Int position, Dictionary<Vector2Int, Node> dict = null)
        {
            if (dict == null)
            {
                if (DictNodes.TryGetValue(position, out Node returningNode))
                    return returningNode;
            }
            else
            {
                if (dict.TryGetValue(position, out Node returningNode))
                    return returningNode;
            }
            return null;
        }

        public static List<Node> GetConnectedNodesInt(Node node, Dictionary<Vector2Int, Node> dict = null)
        {
            List<Node> connectedNodes = new List<Node>();
            int posX = (int)node.position.x;
            int posY = (int)node.position.y;
            Vector2Int v2Int = new Vector2Int(posX, posY);
            foreach (Vector2Int dir in directions)
            {
                if (dict == null)
                    connectedNodes.Add(FindExactNodeInt(new Vector2Int(v2Int.x + dir.x, v2Int.y + dir.y)));
                else
                    connectedNodes.Add(FindExactNodeInt(new Vector2Int(v2Int.x + dir.x, v2Int.y + dir.y), dict));
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
