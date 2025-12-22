public class SudokuBuilder
{
    //We expect a string of text with a single line of 81 digits (0-9), where 0 represents an empty square. Left to right, top to bottom for each Box. 
    //For example if a string starts with "1 2 3 4 5 6 7 8 9 9 8 7 6 5 4 3 2 1" then the sudoku will look like this:
    // 1 2 3 | 9 8 7 | . . .
    // 4 5 6 | 6 5 4 | . . .
    // 7 8 9 | 3 2 1 | . . .
    //----------------------
    // . . . | . . . | . . .
    // . . . | . . . | . . .
    // . . . | . . . | . . .
    // ---------------------
    // . . . | . . . | . . .
    // . . . | . . . | . . .
    // . . . | . . . | . . .


    //Builds a sudoku from a text representation
    public Sudoku BuildSudokuFromText(string text)
    {
        int[] numbers = text.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
        
        if(numbers.Length != 81)
        {
            throw new Exception("Not a valid sudoku text file");
        }
        
        int[][] numbersPerBox = new int[9][];
        for(int i = 0; i < 9; i++)
        {
            int[] temp = new int[9];
            for (int j = 0; j < 9; j++)
            {
                temp[j] = numbers[i * 9 + j];
            }
            numbersPerBox[i] = temp;
        }

        return new Sudoku(numbersPerBox);
    }
    
    //We expect a string of text with a single line of 81 digits (0-9), where 0 represents an empty square. 
    //For example if a string starts with "1 2 3 4 5 6 7 8 9 9 8 7 6 5 4 3 2 1" then the sudoku will look like this:
    // 1 2 3 | 4 5 6 | 7 8 9
    // 9 8 7 | 6 5 4 | 3 2 1
    // . . . | . . . | . . .
    //----------------------
    // . . . | . . . | . . .
    // . . . | . . . | . . .
    // . . . | . . . | . . .
    // ---------------------
    // . . . | . . . | . . .
    // . . . | . . . | . . .
    // . . . | . . . | . . .
    public Sudoku BuildSudokuFromCursusText(string text)
    {
        int[] numbers = text.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();

        if (numbers.Length != 81)
        {
            throw new Exception("Not a valid sudoku text file");
        }
        
        int[][] numbersPerBox = new int[9][];
        for (int boxIndex = 0; boxIndex < 9; boxIndex++)
        {
            numbersPerBox[boxIndex] = new int[9];

            int row = boxIndex / 3;
            int col = boxIndex % 3;

            int internalIndex = 0;
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int fullIndex = 
                        row * 27 + //skip 3 blocks if it is past the first blocks
                        i * 9    + //skip row inside of block rows
                        col * 3  + //skip 3 forward in every row for every column to the right
                        j;
                    
                    numbersPerBox[boxIndex][internalIndex++] = numbers[fullIndex];
                }
            }
        }
        
        return new Sudoku(numbersPerBox);
    }

    //Build an empty sudoku
    public Sudoku CreateEmpty()
    {
        return new Sudoku();
    }
}


// Can be used to read a text file from disk so you can easily load sudoku strings
public static class FileHandler
{
    public static string ReadTextFile(string pad)
    {
        try
        {
            return File.ReadAllText(pad);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij lezen: {ex.Message}");
            return string.Empty;
        }
    }
}