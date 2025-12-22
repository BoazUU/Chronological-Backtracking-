using System.Data.Common;
using System.IO.Compression;
using System.Linq;
using System.Text;

//An entire sudoku with 9 boxes
//Note that everything is zero indexed
public class Sudoku
{
	public Box[,] Boxes { get; set; }
	public Evaluation Evaluation { get; set; } = new Evaluation();

	public Sudoku()
	{
		FillEmpty();
		Evaluate();
	}

	public Sudoku(int[][] sudokuInNumbers)
	{
		FillEmpty();
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Boxes[i, j] = new Box(sudokuInNumbers[i * 3 + j]);
			}
		}

		Evaluate();
	}

	public void Print()
	{
		// Console.BackgroundColor = ConsoleColor.White;
		// Console.ForegroundColor = ConsoleColor.Black;

		ConsoleColor originalBg = Console.BackgroundColor;
		ConsoleColor originalFg = Console.ForegroundColor;

		StringBuilder sudokuBox = new();

		for (int boxRow = 0; boxRow < 3; boxRow++)
		{
			sudokuBox.AppendLine("+─────────+─────────+─────────+");

			for (int row = 0; row < 3; row++)
			{
				// StringBuilder line = new StringBuilder();
				sudokuBox.Append("│");

				for (int boxCol = 0; boxCol < 3; boxCol++)
				{
					for (int col = 0; col < 3; col++)
					{
						int value = Boxes[boxRow, boxCol].Squares[row, col].value;

						sudokuBox.Append(" " + (value == 0 ? "•" : value.ToString()) + " ");
					}

					sudokuBox.Append("│");
				}

				sudokuBox.AppendLine();
			}
		}

		sudokuBox.AppendLine("+─────────+─────────+─────────+");

		Console.WriteLine(sudokuBox.ToString());

		Console.ResetColor();

	}

	//Copy constructor
	public Sudoku(Sudoku sudoku)
	{
		Boxes = new Box[3, 3];
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Boxes[i, j] = new Box(sudoku.Boxes[i, j]);
			}
		}
		Evaluation copyEvaluation = new Evaluation(sudoku.Evaluation);
		Evaluation = copyEvaluation;
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
		Box[] allBoxes = Boxes.Cast<Box>().ToArray();
		foreach (Box box in allBoxes)
		{
			box.FillRandom();
		}
	}

	//Fills all empty squares with empty Square structs
	public void FillEmpty()
	{
		Boxes = new Box[3, 3];

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Boxes[i, j] = new Box();
				Boxes[i, j].FillEmpty();
			}
		}
	}

	// Evaluates a row and returns the number of duplicates or empty squares
	public int EvaluateRow(int row)
	{
		int boxRow = row / 3;
		Box[] boxes = new Box[] { Boxes[boxRow, 0], Boxes[boxRow, 1], Boxes[boxRow, 2] };
		Func<Box, int, Square[]> getSquaresInBox = (box, i) => box.GetRow(i);
		return EvaluateUnit(row, boxes, getSquaresInBox);
	}

	// Evaluates a column and returns the number of duplicates or empty squares
	public int EvaluateColumn(int column)
	{
		int boxColum = column / 3;
		Box[] boxes = new Box[] { Boxes[0, boxColum], Boxes[1, boxColum], Boxes[2, boxColum] };
		Func<Box, int, Square[]> getSquaresInBox = (box, i) => box.GetColomn(i);
		return EvaluateUnit(column, boxes, getSquaresInBox);
	}

	// Evaluates a unit (row or column) given the boxes that contain it and a function to get the squares in the box
	private int EvaluateUnit(int index, Box[] boxes, Func<Box, int, Square[]> getSquaresInBox)
	{
		int unitIndex = index % 3;
		Square[] squares = new Square[0];

		foreach (Box box in boxes)
		{
			Square[] squaresInBox = getSquaresInBox(box, unitIndex);
			squares = squares.Concat(squaresInBox).ToArray();
		}

		return EvaluateUnitSquares(squares);
	}

	// This evaluates an array of 9 squares and returns the number of duplicates or empty squares
	private int EvaluateUnitSquares(Square[] squares)
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

	public Queue<Square> findAllEmptySquare()
	{
		Queue<Square> emptySquaresQueue = new Queue<Square>();
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					Square[] emptySquares = Boxes[i, j].GetRow(k).Where(n => n.value == 0).ToArray();
					foreach (Square emptySquare in emptySquares)
					{
						emptySquaresQueue.Enqueue(emptySquare);
					}
				}
			}
		}
		return emptySquaresQueue;
	}
}

	//3x3 box that has to contain all 9 numbers
	public class Box
	{
		public Square[,] Squares { get; set; }

		public Box()
		{
			Squares = new Square[3, 3];
		}

		//Takes an array of 9 integers representing the box
		public Box(int[] values)
		{
			Squares = new Square[3, 3];

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int value = values[i * 3 + j];
					bool isFixed = value != 0;
					Squares[i, j] = new Square(value, isFixed);
				}
			}
		}

		//Copy constructor
		public Box(Box box)
		{
			Squares = new Square[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Squares[i, j] = box.Squares[i, j];
				}
			}
		}

		//Switches the values of two squares
		public void Switch(Coord posA, Coord posB)
		{
			Square temp = Squares[posA.x, posA.y];
			Squares[posA.x, posA.y] = Squares[posB.x, posB.y];
			Squares[posB.x, posB.y] = temp;
		}

		// Fills empty squares with random numbers that are not already used in the box
		public void FillRandom()
		{
			Random random = new();
			Square[] allSquares = GetRow(0).Concat(GetRow(1).Concat(GetRow(2))).ToArray();
			int[] usedNumbers = allSquares.Select(n => n.value).ToArray();
			Queue<int> availableNumbers = new(
										Enumerable.Range(1, 9)
										.Where(n => !usedNumbers.Contains(n))
										.OrderBy(x => random.Next()));

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (Squares[i, j].value == 0) Squares[i, j].SetValue(availableNumbers.Dequeue());
				}
			}


		}

		public void FillEmpty()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (Squares[i, j].value == 0)
					{
						Squares[i, j] = new Square();
					}
				}
			}
		}

		//Zero indexed
		public Square[] GetRow(int row)
		{
			return new Square[] { Squares[row, 0], Squares[row, 1], Squares[row, 2] };
		}

		//Zero indexed
		public Square[] GetColomn(int column)
		{
			return new Square[] { Squares[0, column], Squares[1, column], Squares[2, column] };
		}
	}

	//A single square in a box
	public struct Square
	{

		public int value { get; private set; }
		public bool isFixed;

		//Zero is used for an empty value
		public Square()
		{
			value = 0;
			isFixed = false;
		}

		public Square(int value, bool isFixed)
		{
			this.value = value;
			this.isFixed = isFixed;
		}
		public Square(int value) : this(value, true)
		{
			this.value = value;
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
}