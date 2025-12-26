using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;

public static class Program 
{    
    public static void Main()
    {        
        Test();
    }

    public static void Test()
    {
        string sudokuString =
            "2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3";

        SudokuBuilder builder = new();
        Sudoku sudoku = builder.BuildSudokuFromText(sudokuString);
         sudoku.Print();
        SudokuAlgorithm algorithm = new ForwardChecking();
        (Sudoku result, int iterations) = algorithm.Apply(sudoku);
        Console.WriteLine("Result");
        if(result != null) result.Print();
        else Console.WriteLine("No solution found");
    }

    public static void ApplyIteratedLocalSearch()
    {
        //Example sudoku string
        string sudokuString =
            "0 0 3 0 2 0 6 0 0 " +
            "9 0 0 3 0 5 0 0 1 " +
            "0 0 1 8 0 6 4 0 0 " +
            "0 0 8 1 0 2 9 0 0 " +
            "7 0 0 0 0 0 0 0 8 " +
            "0 0 6 7 0 8 2 0 0 " +
            "0 0 2 6 0 9 5 0 0 " +
            "8 0 0 2 0 3 0 0 9 " +
            "0 0 5 0 1 0 3 0 0";

        // consoleProgram();

        bool running = true;
        bool paused = false;

        while (running)
        {
            Console.Clear();

            //consoleAverage() for the average amount of iteration of 50 runs
            //consoleProgram() for the amount of iterations of a single run with more details
            consoleAverage();   // Your main program logic here

            while (Console.KeyAvailable) // clear leftover keys
                Console.ReadKey(true);

            // Console.WriteLine();
            paused = true;
            Console.WriteLine("Press ENTER to solve a new Sudoku, or ESC/Q to exit...");
            while (paused)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape || key == ConsoleKey.Q)
                {
                    paused = false;
                    running = false;
                }
                else if (key == ConsoleKey.Enter)
                {
                    paused = false;
                }
            }
        }

        Console.WriteLine("Sudoku Solver exited. Goodbye!");
    }

    public static void consoleProgram()
    {
        SudokuBuilder builder = new();

        string sudokuString = "";
        while (sudokuString.Length == 0)
        {
            Console.Clear();
            Console.WriteLine("Give a Sudoku");
            sudokuString = Console.ReadLine().Trim();
        }

        Sudoku sudoku;
        try
        {
           sudoku = builder.BuildSudokuFromText(sudokuString);
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Something went wrong");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(new string('_', Console.WindowWidth));
        
        Console.WriteLine("Input:");
        sudoku.Print();

        Stopwatch timer =  new Stopwatch();
        timer.Start();
        SudokuAlgorithm algorithm = new IteratedLocalSearch();
        (Sudoku solved, int iterations) = algorithm.Apply(sudoku);
        timer.Stop();
        
        TimeSpan timeTaken = timer.Elapsed;
        Console.WriteLine("\nSolved Output:");
        solved.Print();
        Console.WriteLine($"Solved in {iterations} iterations. With a time of: {timeTaken.ToString(@"mm\:ss\.ff")}");
    }

    public static void consoleAverage()
    {
        const int MAX_RUNS = 50;
        SudokuBuilder builder = new();

        string sudokuString = "";
        while (sudokuString.Length == 0)
        {
            Console.Clear();
            Console.WriteLine("Give a Sudoku");
            sudokuString = Console.ReadLine().Trim();
        }

        Sudoku sudoku;
        try
        {
           sudoku = builder.BuildSudokuFromText(sudokuString);
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Something went wrong");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(new string('_', Console.WindowWidth));
        
        Console.WriteLine("Input:");
        sudoku.Print();

        Stopwatch timer =  new Stopwatch();
        timer.Start();
        SudokuAlgorithm algorithm = new IteratedLocalSearch();

        List<int> iterations = new List<int>();

        for (int i = 0; i < MAX_RUNS; i++)
        {
            (Sudoku solved, int iter) = algorithm.Apply(sudoku);
            iterations.Add(iter);
            if (i%10 == 0)
            {
                Console.WriteLine($"Amount solved: {i}");
            }
        }
        double average = iterations.Average();
        timer.Stop();
        
        TimeSpan timeTaken = timer.Elapsed;
        Console.WriteLine($"\nAverage Iterations: {average} iterations");
        Console.WriteLine($"With a time of: {timeTaken.ToString(@"mm\:ss\.ff")}");
    }

}
