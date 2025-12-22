public abstract class SudokuAlgorithm
{
    public abstract (Sudoku, int) Apply(Sudoku sudoku);
    
}

public class ChronologicBacktracking : SudokuAlgorithm
{
    public override (Sudoku, int) Apply(Sudoku sudoku)
    {
        // Find an empty square (search sudoku, from left to right, top to bottem) 
        // If there are none, evaluate and return

        // Loop from 1 to 9
            // fill in the value
            // Apply this function again
            // If sudoku that is made has a score of 0 return this, else go through with the loop

        //if no solution is found return (Null, iteration)
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
                for(int i = 0; i < S; i++)
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
            // if (iteration % 1000 == 0)
            // {
            //     Console.WriteLine($"{iteration} iterations : Score is {newSudoku.Evaluation.TotalScore()}");
            //     //newSudoku.Print();
            // }
        }
        
        // Console.WriteLine($"{iteration} iterations");
        return (newSudoku, iteration);
    }

    public Sudoku Step(Sudoku sudoku, bool randomSwap)
    {
        int rowIndex = _random.Next(0, 3);
        int columnIndex = _random.Next(0, 3);

        // Create all possible swaps within a box and choose the best one if it better than the score
        List<Sudoku> allSwitches = SingleSwitchedSudoku(sudoku, rowIndex, columnIndex);

        // There should be a minimum of one possible switch otherwise just return the sudoku
        if (allSwitches.Count == 0)
            return sudoku;

        if (randomSwap)
            return allSwitches[_random.Next(allSwitches.Count)];

        Sudoku newSudoku = allSwitches.MinBy(s => s.Evaluation.TotalScore());
        int[] evaluations = allSwitches.Select(s => s.Evaluation.TotalScore()).ToArray();
        if (newSudoku.Evaluation.TotalScore() <= sudoku.Evaluation.TotalScore())
            return newSudoku;
        
        return sudoku;
    }

    private List<Sudoku> SingleSwitchedSudoku(Sudoku sudoku, int X, int Y)
    {
        List<Coord> coords = new List<Coord>();

        // Loop through all squares in the box to find non-fixed squares
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (sudoku.Boxes[X, Y].Squares[i, j].isFixed)  
                    continue;
                
                coords.Add(new(i, j));
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

            copySudoku.Boxes[X, Y].Switch(currCouple.one, currCouple.two);
            copySudoku.Evaluate();
            
            bool test = copySudoku == sudoku;

            //do not add if the score is smaller than the original score.
            switchedSudokus.Add(copySudoku);
        }
        
        return switchedSudokus;
    }
}
    
