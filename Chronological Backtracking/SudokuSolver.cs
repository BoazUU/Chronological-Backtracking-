using System;


// A simple solver class that connects a Sudoku with a solving algorithm
public class SudokuSolver
{
	Sudoku Sudoku { get; set; }
    SudokuAlgorithm Algorithm { get; set; }

    public SudokuSolver(Sudoku sudoku, SudokuAlgorithm algorithm)
    {
		Sudoku = sudoku;
		Algorithm = algorithm;
	}

	public void Solve()
	{
		Algorithm.Apply(Sudoku);
    }
}
