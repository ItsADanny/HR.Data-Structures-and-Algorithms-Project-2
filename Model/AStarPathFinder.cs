
namespace Model
{
    public class AStarPathFinder : IPathFinder
    {
        PathFinderType _algType = PathFinderType.Astar;
        public PathFinderType algType { get => _algType; set {} }

        // Manhattan distance heuristic
        private int Heuristic(int row, int col, int goalRow, int goalCol)
        {
            return Math.Abs(row - goalRow) + Math.Abs(col - goalCol);
        }

        public void FindPath(Maze maze, int[] pos, Queue<int[]> visitedPositions)
        {
            // Validate maze
            if (maze.MazeArray == null || maze.MazeArray.Length == 0 || pos == null || pos.Length != 2)
            {
                return;
            }

            int rows = maze.MazeArray.Length;
            int cols = maze.MazeArray[0].Length;
            int goalRow = maze.End[0];
            int goalCol = maze.End[1];

            var gScore = new Dictionary<string, int>();
            var parent = new Dictionary<string, int[]>();
            var openSet = new PriorityQueue<int[], int>();
            var closedSet = new HashSet<string>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    string key = $"{i},{j}";
                    gScore[key] = int.MaxValue;
                }
            }

            string startKey = $"{pos[0]},{pos[1]}";
            gScore[startKey] = 0;
            int startHeuristic = Heuristic(pos[0], pos[1], goalRow, goalCol);
            openSet.Enqueue(pos, startHeuristic);
            parent[startKey] = null;

            while (openSet.Count > 0)
            {
                int[] current = openSet.Dequeue();
                string currentKey = $"{current[0]},{current[1]}";

                if (closedSet.Contains(currentKey))
                {
                    continue;
                }

                closedSet.Add(currentKey);

                if (current[0] == goalRow && current[1] == goalCol)
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

                int currentG = gScore[currentKey];
                foreach (var move in maze.moves)
                {
                    int nextRow = current[0] + move[0];
                    int nextCol = current[1] + move[1];
                    string nextKey = $"{nextRow},{nextCol}";

                    if (maze.IsValidMove(nextRow, nextCol) && !closedSet.Contains(nextKey))
                    {
                        int newG = currentG + 1;

                        if (newG < gScore[nextKey])
                        {
                            gScore[nextKey] = newG;
                            parent[nextKey] = current;
                            
                            // Calculate f-score = g + h
                            int heuristic = Heuristic(nextRow, nextCol, goalRow, goalCol);
                            int fScore = newG + heuristic;
                            
                            openSet.Enqueue(new int[] { nextRow, nextCol }, fScore);
                        }
                    }
                }
            }

            // If no path found, just add start position
            visitedPositions.Enqueue(pos);
        }

    }
}

            

