
namespace Model
{
    public class RecursivePathFinder : IPathFinder
    {
        PathFinderType _algType = PathFinderType.Recursive;
        public PathFinderType algType { get => _algType; set {} }
        private HashSet<string> _visitedSet = new HashSet<string>();
        private bool _foundEnd = false;

        public void FindPath(Maze maze, int[] pos, Queue<int[]> visitedPositions)
        {
            // Reset on new search
            _visitedSet.Clear();
            _foundEnd = false;

            // Start recursive DFS
            if (maze.MazeArray != null && maze.MazeArray.Length > 0 && pos != null && pos.Length == 2)
            {
                FindPathRecursive(maze, pos, visitedPositions);
            }
        }

        private void FindPathRecursive(Maze maze, int[] pos, Queue<int[]> visitedPositions)
        {
            // Early exit if we already found the end
            if (_foundEnd)
            {
                return;
            }

            // Check if position is valid and unvisited
            if (!maze.IsValidMove(pos[0], pos[1]))
            {
                return;
            }

            // Create a key for this position
            string posKey = $"{pos[0]},{pos[1]}";

            // Check if already visited
            if (_visitedSet.Contains(posKey))
            {
                return;
            }

            // Mark as visited
            _visitedSet.Add(posKey);
            visitedPositions.Enqueue(pos);

            // Check if we reached the end
            if (pos[0] == maze.End[0] && pos[1] == maze.End[1])
            {
                _foundEnd = true;
                return;
            }

            // Recursively explore all four directions: down, up, left, right
            foreach (var move in maze.moves)
            {
                int nextRow = pos[0] + move[0];
                int nextCol = pos[1] + move[1];
                FindPathRecursive(maze, new int[] { nextRow, nextCol }, visitedPositions);

                // Stop if we found the end
                if (_foundEnd)
                {
                    return;
                }
            }
        }
    }
}
