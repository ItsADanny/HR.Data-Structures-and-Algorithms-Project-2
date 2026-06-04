namespace Model
{
    public class DijkstraPathFinder : IPathFinder
    {
        PathFinderType _algType = PathFinderType.Dijkstra;
        public PathFinderType algType { get => _algType; set {} }
        
        public void FindPath(Maze maze, int[] pos, Queue<int[]> visitedPositions)
        {
            // Validate maze
            if (maze.MazeArray == null || maze.MazeArray.Length == 0 || pos == null || pos.Length != 2)
            {
                return;
            }

            int rows = maze.MazeArray.Length;
            int cols = maze.MazeArray[0].Length;

            var distances = new Dictionary<string, int>();
            var parent = new Dictionary<string, int[]>();
            var priorityQueue = new PriorityQueue<int[], int>();
            var visited = new HashSet<string>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    string key = $"{i},{j}";
                    distances[key] = int.MaxValue;
                }
            }

            string startKey = $"{pos[0]},{pos[1]}";
            distances[startKey] = 0;
            priorityQueue.Enqueue(pos, 0);
            parent[startKey] = null;

            // Dijkstra's algorithm
            while (priorityQueue.Count > 0)
            {
                int[] current = priorityQueue.Dequeue();
                string currentKey = $"{current[0]},{current[1]}";

                if (visited.Contains(currentKey))
                {
                    continue;
                }

                visited.Add(currentKey);

                if (current[0] == maze.End[0] && current[1] == maze.End[1])
                {
                    var path = new Stack<int[]>();
                    int[] node = current;
                    
                    while (node != null)
                    {
                        path.Push(node);
                        string nodeKey = $"{node[0]},{node[1]}";
                        node = parent[nodeKey];
                    }

                    while (path.Count > 0)
                    {
                        visitedPositions.Enqueue(path.Pop());
                    }
                    return;
                }

                int currentDistance = distances[currentKey];
                foreach (var move in maze.moves)
                {
                    int nextRow = current[0] + move[0];
                    int nextCol = current[1] + move[1];
                    string nextKey = $"{nextRow},{nextCol}";

                    if (maze.IsValidMove(nextRow, nextCol) && !visited.Contains(nextKey))
                    {
                        int newDistance = currentDistance + 1;
                        if (newDistance < distances[nextKey])
                        {
                            distances[nextKey] = newDistance;
                            parent[nextKey] = current;
                            priorityQueue.Enqueue(new int[] { nextRow, nextCol }, newDistance);
                        }
                    }
                }
            }
            visitedPositions.Enqueue(pos);
        }
   }
}

