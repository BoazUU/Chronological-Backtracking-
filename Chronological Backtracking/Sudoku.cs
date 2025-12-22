using System.Text;
using System.Linq;

//An entire sudoku with 9 boxes
//Note that everything is zero indexed
public class Sudoku
{
	public Square[,] Squares { get; set; } = new Square[9, 9];
	public Evaluation Evaluation { get; set; } = new Evaluation();

	public Sudoku()
	{
		FillEmpty();
		Evaluate();
	}

	public Sudoku(int[,] sudokuInNumbers)
	{
		FillEmpty();
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (sudokuInNumbers[i, j] == 0)
					continue;
				
				Squares[i, j] = new Square(sudokuInNumbers[i, j], new Coord(i, j));
			}
		}

		Evaluate();
	}

	public Square[] GetRow(int row)
	{
		return new Square[] {
			Squares[row,0], Squares[row,1], Squares[row,2],
			Squares[row,3], Squares[row,4], Squares[row,5],
			Squares[row,6], Squares[row,7], Squares[row,8]
		};
	}

	public Square[] GetColumn(int column)
	{
		return new Square[] {
			Squares[0,column], Squares[1,column], Squares[2,column],
			Squares[3,column], Squares[4,column], Squares[5,column],
			Squares[6,column], Squares[7,column], Squares[8,column]
		};
	}
	
	// Left to right, top to bottem
	public Square[,] GetBox(int boxIndex){
		return new Square[,] {
			{ Squares[(boxIndex/3)*3 + 0, (boxIndex%3)*3 + 0], Squares[(boxIndex/3)*3 + 0, (boxIndex%3)*3 + 1], Squares[(boxIndex/3)*3 + 0, (boxIndex%3)*3 + 2] },
			{ Squares[(boxIndex/3)*3 + 1, (boxIndex%3)*3 + 0], Squares[(boxIndex/3)*3 + 1, (boxIndex%3)*3 + 1], Squares[(boxIndex/3)*3 + 1, (boxIndex%3)*3 + 2] },
			{ Squares[(boxIndex/3)*3 + 2, (boxIndex%3)*3 + 0], Squares[(boxIndex/3)*3 + 2, (boxIndex%3)*3 + 1], Squares[(boxIndex/3)*3 + 2, (boxIndex%3)*3 + 2] }
		};
    }

	public void Print()
	{
		ConsoleColor originalBg = Console.BackgroundColor;
		ConsoleColor originalFg = Console.ForegroundColor;

		StringBuilder sudokuBox = new();
		for (int row = 0; row < 9; row++)
		{
			if (row % 3 == 0)
			{
				sudokuBox.AppendLine("+─────────+─────────+─────────+");
			}

			Square[] rowArray = GetRow(row);

			for (int i = 0; i < 9; i++)
			{
				sudokuBox.Append(" " + (rowArray[i].value == 0 ? "•" : rowArray[i].value.ToString()) + " ");
				if (i % 3 == 2)
				{
					sudokuBox.Append("│");
				}
			}

			sudokuBox.AppendLine();
		}

		sudokuBox.AppendLine("+─────────+─────────+─────────+");
		Console.WriteLine(sudokuBox.ToString());
		Console.ResetColor();
	}

	//Copy constructor
	public Sudoku(Sudoku sudoku)
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				Squares[i, j] = sudoku.Squares[i, j];
			}
		}

		Evaluation = new Evaluation(sudoku.Evaluation);
	}

	//Evaluates all rows and columns
	public void Evaluate()
	{
		int[] range = Enumerable.Range(0, 9).ToArray();
		Evaluate(range, range);
	}

	//Evaluates only the given rows and columns
	public void Evaluate(int[] rows, int[] columns)
	{
		foreach (int row in rows)
		{
			Evaluation.RowScores[row] = EvaluateRow(row);
		}

		foreach (int column in columns)
		{
			Evaluation.ColumnScores[column] = EvaluateColumn(column);
		}

	}

	//Fills all empty squares with random numbers that are not already used in the box
	public void FillRandom()
	{
		Random random = new();
		for (int i = 0; i < 9; i++)
		{
			Square[] box = GetBox(i).Cast<Square>().ToArray();
			Queue<int> unusedNumbers = new Queue<int>(Enumerable.Range(1, 9)
									    .Where(n => !box.Select(s => s.value).Contains(n))
									    .OrderBy(x => random.Next())
                                        .ToList());
            
			foreach (Square square in box)
			{
				if (square.value != 0) continue;
				Squares[square.position.x, square.position.y].SetValue(unusedNumbers.Dequeue());
			}
		}
	}

	//Fills all empty squares with empty Square structs
	public void FillEmpty()
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				Squares[i, j] = new Square(new Coord(i, j));
			}
		}
	}

	// Evaluates a row and returns the number of duplicates or empty squares
	public int EvaluateRow(int row)
	{
		return EvaluateUnit(GetRow(row));
	}

	// Evaluates a column and returns the number of duplicates or empty squares
	public int EvaluateColumn(int column)
	{
		return EvaluateUnit(GetColumn(column));
	}

	// This evaluates an array of 9 squares and returns the number of duplicates or empty squares
	private int EvaluateUnit(Square[] squares)
	{
		int score = 0;

		List<int> doubles = new();
		foreach (Square square in squares)
		{
			int value = square.value;
			if (value != 0 && !doubles.Contains(value))
			{
				doubles.Add(value);
			} else
			{
				score++;
			}
		}

		return score;
	}

	public void Switch(Coord posA, Coord posB)
	{
		Square temp = Squares[posA.x, posA.y];
		Squares[posA.x, posA.y] = Squares[posB.x, posB.y];
		Squares[posB.x, posB.y] = temp;
    }

    public Queue<Square> FindAllEmptySquare()
	{
		Queue<Square> emptySquares = new Queue<Square>();
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (Squares[i, j].value == 0)
				{
					emptySquares.Enqueue(Squares[i, j]);
				}
			}
		}

		return emptySquares;
	}
}

//A single square in a box
public struct Square
{
	public int value { get; private set; }
	public bool isFixed;
	public Coord position;

	//Zero is used for an empty value
	public Square(Coord position)
	{
		value = 0;
		isFixed = false;
		this.position = position;
    }

	public Square(int value, bool isFixed, Coord position)
	{
		this.value = value;
		this.isFixed = isFixed;
		this.position = position;
    }
	public Square(int value, Coord position) : this(value, true, position)
	{

	}

	public void SetValue(int value)
	{
		if (!isFixed) this.value = value;
		else throw new Exception("A fixed value should not be changed");
	}
}

//A simple coordinate struct
public struct Coord
{
	public int x, y;
	public Coord(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

}

//Holds the evaluation scores for rows and columns. This is so you can easily update only the changed rows and columns
public struct Evaluation
{
	public Dictionary<int, int> RowScores { get; }
	public Dictionary<int, int> ColumnScores { get; }

	public Evaluation()
	{
		RowScores = new Dictionary<int, int>();
		ColumnScores = new Dictionary<int, int>();

		for (int i = 0; i < 9; i++)
		{
			RowScores[i] = 0;
			ColumnScores[i] = 0;
		}
	}

	public Evaluation(Evaluation evaluation)
	{
		RowScores = new Dictionary<int, int>();
		ColumnScores = new Dictionary<int, int>();

		foreach (int key in evaluation.RowScores.Keys)
		{
			RowScores[key] = evaluation.RowScores[key];
		}

		foreach (int key in evaluation.ColumnScores.Keys)
		{
			ColumnScores[key] = evaluation.ColumnScores[key];
		}
	}

	public int TotalScore()
	{
		return RowScores.Values.Sum() + ColumnScores.Values.Sum();
	}
}
