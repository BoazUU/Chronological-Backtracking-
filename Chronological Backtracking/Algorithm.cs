public abstract class SudokuAlgorithm
{
    public abstract (Sudoku, int) Apply(Sudoku sudoku);
    
}

public class ChronologicBacktracking : SudokuAlgorithm
{
    public override (Sudoku, int) Apply(Sudoku sudoku)
    {
        //Create hardcopy so orgiginal wont be changed
        Sudoku sudokuCopy = new Sudoku(sudoku);

        // Create a list of empty squares
        List<Square> emptySquares = sudokuCopy.FindAllEmptySquare();

        // Apply the recursive function
        ApplyRecursive(sudokuCopy, emptySquares, 0);

        //if no solution is found return (Null, iteration)
        sudokuCopy.Evaluate();
        if(sudokuCopy.FindAllEmptySquare().Count > 0)
        {
            return (null, 0);
        }

        return (sudokuCopy, 0);
    }

    public bool ApplyRecursive(Sudoku sudoku, List<Square> emptySquares, int depth)
    {
        // Find an empty square (search sudoku, from left to right, top to bottem) 
        // If there are none, return the sudoku as it is solved
        if (depth >= emptySquares.Count)
            return true;

        // Loop from 1 to 9
        //      fill in the value
        //          Check if the sudoku is still valid
        //      Apply this function again
        // If no value worked, reset the square and return to previous 

        Square square = emptySquares[depth];
        for(int i = 1; i < 10; i++)
        {
            sudoku.Squares[square.position.x, square.position.y].SetValue(i);
            sudoku.Evaluate(new int[] { square.position.x }, new int[] { square.position.y });
            
            if(sudoku.Evaluation.TotalScore() == 0)
            {
                bool solved = ApplyRecursive(sudoku, emptySquares, depth + 1);
                if (solved)
                {
                    return true;
                }
            }
        }
    
        sudoku.Squares[square.position.x, square.position.y].SetValue(0);
        sudoku.Evaluate(new int[] { square.position.x }, new int[] { square.position.y });
        return false;
    }
}

public class ForwardChecking : SudokuAlgorithm
{
    public override (Sudoku, int) Apply(Sudoku sudoku)
    {
        //Create hardcopy so orgiginal wont be changed
        Sudoku sudokuCopy = new Sudoku(sudoku);

        // Create a queue of empty squares
        List<Square> emptySquares = sudokuCopy.FindAllEmptySquare();

        // Setup domains for all empty squares
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++) { 
                sudokuCopy.UpdateDomains(sudokuCopy.Squares[i,j]);
            }
        }

        // Apply the recursive function
        (bool solved, Sudoku result) = ApplyRecursive(sudokuCopy, emptySquares, 0);

        if (solved)
        {
            return (result, 0);
        }

        return (null, 0);
    }

    public (bool solved, Sudoku result) ApplyRecursive(Sudoku sudoku, List<Square> emptySquares, int depth)
    {
        // Find an empty square (search sudoku, from left to right, top to bottem) 
        // If there are none, return the sudoku as it is solved
        if (depth >= emptySquares.Count)
            return (true, sudoku);


        //Go through all possible values in the squares domain. If a value makes it so another emtpy square has a empty domain, continue with searching.
        // If no values work go back to previous square, aka return false. 
        // Update domains
        // Check if any other empty square has an empty domain
        
        Square square = sudoku.Squares[emptySquares[depth].position.x, emptySquares[depth].position.y];
        SortedSet<int> domain = new(square.domain);
        
        foreach (int value in domain)
        {
            Sudoku temp = new Sudoku(sudoku);

            temp.Squares[square.position.x, square.position.y].SetValue(value);
            temp.UpdateDomains(temp.Squares[square.position.x, square.position.y]);
            if (temp.AnyEmptyDomains(temp.Squares[square.position.x, square.position.y])) continue;

            // If we are here, no empty domains where found, continue the search
            (bool solved, Sudoku result) tuple = ApplyRecursive(temp, emptySquares, depth + 1);
            if (tuple.solved)
            {
                return tuple;
            }
        }

        // No values worked, return false
        return (false, null);
    }
}

// An implementation of the Iterated Local Search algorithm for solving Sudoku puzzles
public class IteratedLocalSearch : SudokuAlgorithm
{
    private readonly static Random _random = new Random();
    public override (Sudoku, int) Apply(Sudoku startSudoku)
    {
        // We need to keep track of the old and new value to detect stagnation, by checking difference in score.
        // If we detect stagnation, we perform S random swaps to escape local minima.
        // Max iterations is also set to avoid infinite loops.
        // Fill the sudoku randomly at the start

        startSudoku.FillRandom();
        Sudoku newSudoku = startSudoku;
        Sudoku oldSudoku = startSudoku;

        // The amount of times a new iteration has led to the same score, if this has reached the max duplicates,
        // then the algorithm has stagnated.
        int duplicateScores = 0;
        int iteration = 0;
        const int S = 15;
        const int MAX_DUPLICATES = 50;
        const int MAX_ITERATION = 200_000;

        //Check if the sudoku is solved or max iterations is reached, if not, continue the process
        while (newSudoku.Evaluation.TotalScore() > 0 && iteration < MAX_ITERATION)
        {
            //check for duplicate scores
            if (oldSudoku.Evaluation.TotalScore() == newSudoku.Evaluation.TotalScore())
            {
                duplicateScores++;
            }
            else
            {
                duplicateScores = 0;
            }

            //update old sudoku
            oldSudoku = new Sudoku(newSudoku);

            //If we have too many duplicate scores, perform S random swaps to escape local minima
            if (duplicateScores > MAX_DUPLICATES)
            {
                for (int i = 0; i < S; i++)
                {
                    newSudoku = Step(newSudoku, true);
                }
                duplicateScores = 0;
            }

            //Perform a normal step
            else
            {
                newSudoku = Step(newSudoku, false);
            }

            iteration++;

            //For testing purposes only
            //if (iteration % 1000 == 0)
            //{
            //    Console.WriteLine($"{iteration} iterations : Score is {newSudoku.Evaluation.TotalScore()}");
            //    newSudoku.Print();
            //}
        }

        return (newSudoku, iteration);
    }

    public Sudoku Step(Sudoku sudoku, bool randomSwap)
    {
        int boxIndex = _random.Next(0, 9);

        // Create all possible swaps within a box and choose the best one if it better than the score
        List<Sudoku> allSwitches = SingleSwitchedSudoku(sudoku, boxIndex);

        // There should be a minimum of one possible switch otherwise just return the sudoku
        if (allSwitches.Count == 0)
            return sudoku;

        if (randomSwap)
            return allSwitches[_random.Next(allSwitches.Count)];

        Sudoku newSudoku = allSwitches.MinBy(s => s.Evaluation.TotalScore());
        if (newSudoku.Evaluation.TotalScore() <= sudoku.Evaluation.TotalScore())
            return newSudoku;

        return sudoku;
    }

    private List<Sudoku> SingleSwitchedSudoku(Sudoku sudoku, int boxIndex)
    {
        List<Coord> coords = new List<Coord>();
        Square[,] box = sudoku.GetBox(boxIndex);
        // Loop through all squares in the box to find non-fixed squares

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (box[i, j].isFixed)
                    continue;

                coords.Add(box[i, j].position);
            }
        }

        // Create all possible couples of coordinates to switch
        List<(Coord one, Coord two)> couples = new List<(Coord, Coord)>();
        for (int i = 0; i < coords.Count; i++)
        {
            for (int j = i + 1; j < coords.Count; j++)
            {
                couples.Add((coords[i], coords[j]));
            }
        }

        // Create new sudokus (hard copies) with each possible switch
        List<Sudoku> switchedSudokus = new List<Sudoku>();
        foreach (var currCouple in couples)
        {
            Sudoku copySudoku = new Sudoku(sudoku);

            copySudoku.Switch(currCouple.one, currCouple.two);
            copySudoku.Evaluate();

            switchedSudokus.Add(copySudoku);
        }

        return switchedSudokus;
    }
}
    
