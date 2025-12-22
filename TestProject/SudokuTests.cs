namespace TestProject
{
    public class SudokuTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase("", 162)]
        
        [TestCase("9 6 1 2 8 7 3 4 5 " +
                  "5 3 7 4 1 9 2 8 6 " +
                  "2 8 4 6 3 5 1 7 9 " +
                  "8 5 9 4 2 6 7 1 3 " +
                  "7 6 1 8 5 3 9 2 4 " +
                  "4 2 3 7 9 1 8 5 6 " +
                  "5 3 4 6 7 2 1 9 8 " +
                  "6 7 8 1 9 5 3 4 2 " +
                  "9 1 2 3 4 8 5 6 7", 0)]

        [TestCase("9 6 1 2 8 7 3 4 5 " +
                  "5 3 9 4 1 9 2 8 6 " +
                  "2 8 4 6 3 5 1 7 9 " +
                  "8 5 9 4 2 6 7 1 3 " +
                  "7 6 1 8 5 3 9 2 4 " +
                  "4 2 3 7 9 1 9 5 6 " +
                  "5 3 4 6 7 2 1 9 8 " +
                  "6 7 9 1 9 5 3 4 2 " +
                  "9 1 2 3 9 8 5 6 9", 10)]

        public void TestSudokuEvaluate(string sudokuString, int expected)
        {
            SudokuBuilder builder = new SudokuBuilder();
            Sudoku sudoku;
            if(sudokuString == "")
            {
                sudoku = builder.CreateEmpty();
            }
            else
            {
                sudoku = builder.BuildSudokuFromText(sudokuString);
            }

            Assert.IsTrue(sudoku.Evaluate().TotalScore() == expected);
        }

        [Test]
        public void TestSudokuFillRandom()
        {
            SudokuBuilder builder = new SudokuBuilder();
            Sudoku sudoku = builder.CreateEmpty();
            sudoku.FillRandom();

            Dictionary<int, int> numberCounts = new Dictionary<int, int>();

            for (int i = 0; i < 10; i++)
            {
                numberCounts[i] = 0;
            }

            // Count occurrences of each number in the Sudoku
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            numberCounts[sudoku.Boxes[i, j].Squares[k, l].value]++;
                        }
                    }
                }
            }

            //CHeck if each number occurs 9 times and zero 0 times
            bool allNumbersCorrect = numberCounts[0] == 0;

            for (int i = 1; i < 10; i++) {
                allNumbersCorrect = numberCounts[i] == 9 && allNumbersCorrect;
                TestContext.WriteLine($"Number {i} occurs {numberCounts[i]} times.");
            }

            Assert.IsTrue(allNumbersCorrect);
        }
    }
}