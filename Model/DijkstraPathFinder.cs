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

            // Dictionary to store distances from start
            var distances = new Dictionary<string, int>();
            
            // Dictionary to store parent nodes for path reconstruction
            var parent = new Dictionary<string, int[]>();
            
            // Priority queue: (distance, position)
            var priorityQueue = new PriorityQueue<int[], int>();
            
            // HashSet for visited nodes
            var visited = new HashSet<string>();

            // Initialize distances to infinity
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    string key = $"{i},{j}";
                    distances[key] = int.MaxValue;
                }
            }

            // Start position has distance 0
            string startKey = $"{pos[0]},{pos[1]}";
            distances[startKey] = 0;
            priorityQueue.Enqueue(pos, 0);
            parent[startKey] = null;

            // Dijkstra's algorithm
            while (priorityQueue.Count > 0)
            {
                int[] current = priorityQueue.Dequeue();
                string currentKey = $"{current[0]},{current[1]}";

                // Skip if already visited
                if (visited.Contains(currentKey))
                {
                    continue;
                }

                // Mark as visited
                visited.Add(currentKey);

                // Check if we reached the end
                if (current[0] == maze.End[0] && current[1] == maze.End[1])
                {
                    // Reconstruct path from end to start
                    var path = new Stack<int[]>();
                    int[] node = current;
                    
                    while (node != null)
                    {
                        path.Push(node);
                        string nodeKey = $"{node[0]},{node[1]}";
                        node = parent[nodeKey];
                    }

                    // Enqueue path in order (start to end)
                    while (path.Count > 0)
                    {
                        visitedPositions.Enqueue(path.Pop());
                    }
                    return;
                }

                // Explore neighbors
                int currentDistance = distances[currentKey];
                foreach (var move in maze.moves)
                {
                    int nextRow = current[0] + move[0];
                    int nextCol = current[1] + move[1];
                    string nextKey = $"{nextRow},{nextCol}";

                    // Check if valid move and not visited
                    if (maze.IsValidMove(nextRow, nextCol) && !visited.Contains(nextKey))
                    {
                        int newDistance = currentDistance + 1;

                        // If we found a shorter path, update it
                        if (newDistance < distances[nextKey])
                        {
                            distances[nextKey] = newDistance;
                            parent[nextKey] = current;
                            priorityQueue.Enqueue(new int[] { nextRow, nextCol }, newDistance);
                        }
                    }
                }
            }

            // If no path found, just add start position
            visitedPositions.Enqueue(pos);
        }
   }
}

