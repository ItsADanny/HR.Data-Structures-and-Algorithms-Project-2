
namespace Model
{
    public class StackPathFinder : IPathFinder
    {
        PathFinderType _algType = PathFinderType.Stack;
        public PathFinderType algType { get => _algType; set {} }

        public void FindPath(Maze maze, int[] pos, Queue<int[]> visitedPositions)
        {
            // Validate maze
            if (maze.MazeArray == null || maze.MazeArray.Length == 0 || pos == null || pos.Length != 2)
            {
                return;
            }

            // Use a HashSet for O(1) visited lookups
            var visited = new HashSet<string>();
            
            // Use a Stack for DFS (LIFO - Last In First Out)
            var stack = new Stack<int[]>();
            stack.Push(pos);

            // DFS using stack
            while (stack.Count > 0)
            {
                int[] current = stack.Pop();

                // Check if position is valid and unvisited
                if (!maze.IsValidMove(current[0], current[1]))
                {
                    continue;
                }

                string posKey = $"{current[0]},{current[1]}";
                if (visited.Contains(posKey))
                {
                    continue;
                }

                // Mark as visited
                visited.Add(posKey);
                visitedPositions.Enqueue(current);

                // Check if we reached the end
                if (current[0] == maze.End[0] && current[1] == maze.End[1])
                {
                    return;
                }

                // Push all valid neighbors onto the stack
                foreach (var move in maze.moves)
                {
                    int nextRow = current[0] + move[0];
                    int nextCol = current[1] + move[1];
                    
                    string nextKey = $"{nextRow},{nextCol}";
                    if (maze.IsValidMove(nextRow, nextCol) && !visited.Contains(nextKey))
                    {
                        stack.Push(new int[] { nextRow, nextCol });
                    }
                }
            }
        }       
    }
}