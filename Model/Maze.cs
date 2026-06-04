
using System.Data;

namespace Model
{
    public class Maze
    {
        public int[][]? MazeArray { get; private set; }
        public int[,]? MazeMDArray { get; private set; }
        public int[]? Begin { get; private set; }
        public int[]? End { get; private set; }

        public readonly int[][] moves = {           
            new int[] {  1,  0 },  //down
            new int[] { -1,  0 },  //up
            new int[] {  0, -1 },  //left
            new int[] {  0,  1 },  //right
            };
        
        public Maze() => GenerateMaze();
        public Maze(bool automatic = true) {if(automatic) GenerateMaze(); else GenerateFromText(MazeGrids.mazeText);}
        public Maze(int rows, int cols) {if(rows <= 0 && cols <= 0) GenerateFromText(MazeGrids.mazeText); else GenerateMaze(rows, cols);}
        public Maze(string lines) => GenerateFromText(lines);

        void GenerateFromText(string lines){
            MazeArray = ToMazeArray(lines);
            MazeMDArray = ToMazeMDArray(lines);
        }

        void GenerateMaze(int rows = 20, int cols = 40)
        {
            if(rows < 4 || cols < 4) {rows = 20; cols = 40;}
            if(rows % 2 != 0) {rows++;}
            if(cols % 2 != 0) {cols++;}

            // STEP 1: Initialize jagged array - fill with walls (-1)
            MazeArray = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                MazeArray[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    MazeArray[i][j] = -1; // All cells start as walls
                }
            }

            // STEP 2: Generate maze using one of the carving algorithms:
            // CarvePassagesDFS(1, 1);         // Uncomment to use Depth-First Search
            // CarvePassagesBinaryTree();      // Uncomment to use Binary Tree algorithm
            CarvePassagesWilsons();          // Uncomment to use Wilson's Algorithm (default)

            // STEP 3: Set start and end positions
            Begin = [1, 1];
            MazeArray[Begin[0]][Begin[1]] = 1;   // mark start
            
            // Set end at a random valid position (not a wall, not the begin position)
            Random random = new Random();
            int endRow, endCol;
            do
            {
                endRow = random.Next(1, rows - 1);
                endCol = random.Next(1, cols - 1);
            } while (MazeArray[endRow][endCol] == -1 || (endRow == Begin[0] && endCol == Begin[1]));
            
            End = [endRow, endCol];
            MazeArray[End[0]][End[1]] = 2;       // mark end

            // STEP 3B: Convert jagged array to multidimensional array
            MazeMDArray = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    MazeMDArray[i, j] = MazeArray[i][j];
                }
            }
        }

        void CarvePassagesDFS(int row, int col)
        {
            if (MazeArray == null) return;

            // Mark current cell as passage (0 = carved/open)
            MazeArray[row][col] = 0;

            // Shuffle the directions to create random maze patterns
            var directions = new int[][] { moves[0], moves[1], moves[2], moves[3] };
            ShuffleArray(directions);

            // Try each direction
            foreach (var direction in directions)
            {
                int nextRow = row + direction[0] * 2;  // Move 2 cells to maintain walls
                int nextCol = col + direction[1] * 2;

                // Check if next position is valid and unvisited (wall)
                if (IsValidPos(MazeArray, nextRow, nextCol) && MazeArray[nextRow][nextCol] == -1)
                {
                    // Carve wall between current and next cell
                    int wallRow = row + direction[0];
                    int wallCol = col + direction[1];
                    MazeArray[wallRow][wallCol] = 0;  // carve passage through wall

                    // Recursively carve from next cell
                    CarvePassagesDFS(nextRow, nextCol);
                }
            }
        }

        void CarvePassagesBinaryTree()
        {
            if (MazeArray == null) return;

            int rows = MazeArray.Length;
            int cols = MazeArray[0]?.Length ?? 0;
            Random random = new Random();

            // Iterate through each cell in the maze (using odd indices to skip walls)
            for (int row = 1; row < rows; row += 2)
            {
                for (int col = 1; col < cols; col += 2)
                {
                    // Mark current cell as passage (0 = carved/open)
                    MazeArray[row][col] = 0;

                    // Determine available directions (up and left)
                    var availableDirections = new List<int[]>();
                    
                    // Check if we can carve UP (not at top edge)
                    if (row > 1)
                        availableDirections.Add(moves[1]); // up
                    
                    // Check if we can carve LEFT (not at left edge)
                    if (col > 1)
                        availableDirections.Add(moves[2]); // left

                    // If we have available directions, randomly choose one
                    if (availableDirections.Count > 0)
                    {
                        int randomIndex = random.Next(availableDirections.Count);
                        int[] chosenDirection = availableDirections[randomIndex];

                        // Carve the wall between current cell and chosen neighbor
                        int wallRow = row + chosenDirection[0];
                        int wallCol = col + chosenDirection[1];
                        MazeArray[wallRow][wallCol] = 0;
                    }
                }
            }
        }

        void ShuffleArray(int[][] array)
        {
            Random random = new Random();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int randomIndex = random.Next(i + 1);
                // Swap elements
                var temp = array[i];
                array[i] = array[randomIndex];
                array[randomIndex] = temp;
            }
        }

        void CarvePassagesWilsons()
        {
            if (MazeArray == null) return;

            int rows = MazeArray.Length;
            int cols = MazeArray[0]?.Length ?? 0;
            Random random = new Random();

            // STEP 1: Mark all cells as unvisited (part of the maze algorithm)
            // We'll use -1 for walls (unvisited by algorithm) and 0 for passages
            bool[,] inMaze = new bool[rows, cols];

            // STEP 2: Pick a random starting cell and mark it as part of the maze
            int startRow = 1 + random.Next((rows - 1) / 2) * 2;  // Odd row
            int startCol = 1 + random.Next((cols - 1) / 2) * 2;  // Odd column
            inMaze[startRow, startCol] = true;
            MazeArray[startRow][startCol] = 0;

            // STEP 3: Pick random cells and carve paths to the maze using random walks
            int cellsInMaze = 1;
            int totalCells = ((rows - 1) / 2) * ((cols - 1) / 2);

            while (cellsInMaze < totalCells)
            {
                // Pick a random cell not yet in the maze
                int row, col;
                do
                {
                    row = 1 + random.Next((rows - 1) / 2) * 2;
                    col = 1 + random.Next((cols - 1) / 2) * 2;
                } while (inMaze[row, col]);

                // Perform random walk from this cell until we hit a cell in the maze
                List<(int, int)> path = new List<(int, int)> { (row, col) };
                (int currentRow, int currentCol) = (row, col);
                Dictionary<(int, int), int> visited = new Dictionary<(int, int), int> { { (currentRow, currentCol), 0 } };

                // Random walk until we hit the maze
                while (!inMaze[currentRow, currentCol])
                {
                    // Choose random direction
                    int[] direction = moves[random.Next(4)];
                    int nextRow = currentRow + direction[0] * 2;
                    int nextCol = currentCol + direction[1] * 2;

                    // If out of bounds, retry from current position
                    if (nextRow < 1 || nextRow >= rows - 1 || nextCol < 1 || nextCol >= cols - 1)
                        continue;

                    // Check if we've been to this cell in this walk
                    if (visited.ContainsKey((nextRow, nextCol)))
                    {
                        // We've found a loop - remove the loop by truncating the path
                        int loopStart = visited[(nextRow, nextCol)];
                        while (path.Count > loopStart + 1)
                            path.RemoveAt(path.Count - 1);
                    }
                    else
                    {
                        // Add to path and visited set
                        visited[(nextRow, nextCol)] = path.Count;
                        path.Add((nextRow, nextCol));
                    }

                    currentRow = nextRow;
                    currentCol = nextCol;
                }

                // We've hit the maze! Add all cells in the path to the maze
                for (int i = 0; i < path.Count; i++)
                {
                    (int row2, int col2) = path[i];

                    if (!inMaze[row2, col2])
                    {
                        inMaze[row2, col2] = true;
                        MazeArray[row2][col2] = 0;
                        cellsInMaze++;

                        // Carve passage between this cell and the next in the path
                        if (i < path.Count - 1)
                        {
                            (int nextRow, int nextCol) = path[i + 1];
                            int wallRow = row2 + (nextRow - row2) / 2;
                            int wallCol = col2 + (nextCol - col2) / 2;
                            MazeArray[wallRow][wallCol] = 0;
                        }
                    }
                }
            }
        }

        int[][] ToMazeArray(string maze)
        {
            // substrings from the maze string
            var arrayLines = maze.Split(new char[] { '.', '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            int[][] outArray = new int[arrayLines.Length][];

            for (var rowIdx = 0; rowIdx < arrayLines.Length; rowIdx++)
            {
                var line = arrayLines[rowIdx];
                // row array:
                var row = new int[line.Length];
                for (int colIdx = 0; colIdx < line.Length; colIdx++)
                {
                    //from chars to integers
                    switch (line[colIdx])
                    {
                        case 'x':
                            row[colIdx] = -1;  //walls
                            break;
                        case '1':
                            row[colIdx] = 1;   //begin
                            Begin = [rowIdx, colIdx];
                            break;
                        case '2':
                            row[colIdx] = 2;   //end 
                            End = [rowIdx, colIdx];
                            break;
                        default:
                            row[colIdx] = 0;   //not visited
                            break;
                    }
                }
                // row in the output jagged array.
                outArray[rowIdx] = row;
            }

            return outArray;
            
        }

        int[,] ToMazeMDArray(string maze)
        {
            // substrings from the maze string
            var arrayLines = maze.Split(new char[] { '.', '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            var lineLength = 0;
            if (arrayLines != null && arrayLines.Length > 0)
                lineLength = arrayLines[0].Length;
            else
            throw new Exception($"Maze incorrect");
            
            for (var rowIdx = 0; arrayLines != null && rowIdx < arrayLines.Length; rowIdx++)
            {
                var line = arrayLines[rowIdx];
                if (arrayLines[rowIdx] == null || line.Length != lineLength)
                    throw new Exception($"Not same line length for rows in maze:\n at row 0: {lineLength}, at row {rowIdx}: {line.Length}");
            }
            
            int[,] outArray = new int[arrayLines.Length, lineLength];

            for (var rowIdx = 0; rowIdx < arrayLines.Length; rowIdx++)
            {
                var line = arrayLines[rowIdx];

                for (int colIdx = 0; colIdx < line.Length; colIdx++)
                {
                    //from chars to integers
                    switch (line[colIdx])
                    {
                        case 'x':
                            outArray[rowIdx, colIdx] = -1;  //walls
                            break;
                        case '1':
                            outArray[rowIdx, colIdx] = 1;   //begin
                            Begin = [rowIdx, colIdx];
                            break;
                        case '2':
                            outArray[rowIdx, colIdx] = 2;   //end 
                            End = [rowIdx, colIdx];
                            break;
                        default:
                            outArray[rowIdx, colIdx] = 0;   //not visited
                            break;
                    }
                }
            }
            return outArray;
        }

        static int CountNotVisited(int[][] maze)
        {
            int cnt = 0;
            if (maze != null && maze.Length > 0)
            {
                for (int rowIdx = 0; rowIdx < maze.Length; rowIdx++)
                {
                    for (int colIdx = 0; maze[rowIdx] != null && colIdx < maze[rowIdx].Length; colIdx++)
                    {
                        cnt = maze[rowIdx][colIdx] == 0 ? cnt + 1 : cnt;
                    }
                }
            }
            return cnt;
        }

        public int CountNotVisited() => CountNotVisited(MazeArray);

        static bool IsValidPos(int[][] array, int newRow, int newColumn)
        {
            // ... Ensure position is within the array bounds.
            /*
            if (newRow < 0) return false;
            if (newColumn < 0) return false;
            if (newRow >= array.Length) return false;
            if (newColumn >= array[newRow].Length) return false;
            return true;
            */
            return !(newRow < 0)
                    && !(newColumn < 0)
                    && !(newRow >= array.Length)
                    && !(newColumn >= array[newRow].Length);
        }
        
        // Make sure the position is within the maze array bounds.
        // no walls
        public bool IsValidMove(int newRow, int newColumn) => 
            IsValidPos(MazeArray, newRow, newColumn) &&
            !(MazeArray[newRow][newColumn] == -1); //no walls 

        //Marking strategy
        public bool IsValidMove(int newRow, int newColumn, bool notVisited = true)
        {
            // Make sure the position is within the maze array bounds.
            // no walls, not yet visited ? (flag notVisited: false)
            return notVisited ?
                    IsValidPos(MazeArray, newRow, newColumn) &&
                    !(MazeArray[newRow][newColumn] == -1)  //no walls, but already visited -> ok
                    :
                    IsValidPos(MazeArray, newRow, newColumn) &&
                    !(MazeArray[newRow][newColumn] == -1 || MazeArray[newRow][newColumn] == 4); //no walls, not yet visited 
        }
        
    }

    public static class MazeGrids
    {
      public static string mazeText = @"
xxxxxx1xxxxxxxxxxxxxxxxxxxxxxx.
 x   x   x                    .
xx2x xxx   x xxxxxxxx    x xx .
x  x xxxxxxx xxxxxxxxxxxxx xxx.
 x x xx      x                .
x  x xx xxxxx  x xxxx xxxxx  x.
xx    x xxx   xx xxx  xxx   xx.
xxx   xxx   x xxxx   xx   x xx.
xx     xx   x xxxx   xx   x xx.
xxxx    xxxxx xx xxxx xxxxx xx.
xx            xx            xx.";
    }
}