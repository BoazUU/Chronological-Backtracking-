namespace TestProject
{
    public class SudokuTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase("", 0)]
        [TestCase("1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1", 144)]

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

            TestContext.WriteLine(sudoku.Evaluation.TotalScore());

            Assert.That(sudoku.Evaluation.TotalScore() == expected);
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
            for (int k = 0; k < 9; k++)
            {
                for (int l = 0; l < 9; l++)
                {
                    numberCounts[sudoku.Squares[k, l].value]++;
                }
            }



            //Check if each number occurs 9 times and zero 0 times
            bool allNumbersCorrect = numberCounts[0] == 0;

            TestContext.WriteLine($"Number 0 occurs {numberCounts[0]} times.");

            for (int i = 1; i < 10; i++) {
                allNumbersCorrect = numberCounts[i] == 9 && allNumbersCorrect;
                TestContext.WriteLine($"Number {i} occurs {numberCounts[i]} times.");
            }

            Assert.That(allNumbersCorrect);
        }
    }
}