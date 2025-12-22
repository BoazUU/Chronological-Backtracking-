using System.ComponentModel;

public class SudokuBuilder
{
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

    public Sudoku BuildSudokuFromText(string text)
    {
        int[] numbers = text.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
        
        if(numbers.Length != 81)
        {
            throw new Exception("Not a valid sudoku text file");
        }
        int[,] numbers2d = new int[9,9];
        
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                numbers2d[i, j] = numbers[i * 9 + j];
            }
        }

        return new Sudoku(numbers2d);
    }
    
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