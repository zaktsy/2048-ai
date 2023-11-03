namespace _2048_ai.GameEngine;

public class DisplayHelper
{
    public void DisplayGameGrid(GameGrid gameGrid)
    {
        Console.Clear();
        Console.WriteLine();
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                using (new ConsoleColors(GetNumberColor(gameGrid.Tiles[i, j])))
                {
                    var symbol = gameGrid.Tiles[i, j] != 0 ? gameGrid.Tiles[i, j].ToString() : ".";
                    Console.Write($"{symbol,6}");
                }
            }

            Console.WriteLine();
        }
    }

    public void DisplayGameWin()
    {
        const string message = "You won!";

        PointCursorInMiddleOfScreen(message);
        
        using (new ConsoleColors(ConsoleColor.Green))
        {
            Console.WriteLine(message);
        }
    }
    
    public void DisplayGameLose()
    {
        const string message = "You Lost!";

        PointCursorInMiddleOfScreen(message);

        using (new ConsoleColors(ConsoleColor.Red))
        {
            Console.WriteLine(message);
        }
    }

    private static void PointCursorInMiddleOfScreen(string s)
    {
        Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.WindowHeight / 2);
    }

    private static ConsoleColor GetNumberColor(ulong number)
    {
        return number switch
        {
            2 => ConsoleColor.Cyan,
            4 => ConsoleColor.Magenta,
            8 => ConsoleColor.Red,
            16 => ConsoleColor.Green,
            32 => ConsoleColor.Yellow,
            64 => ConsoleColor.Yellow,
            128 => ConsoleColor.DarkCyan,
            256 => ConsoleColor.Cyan,
            512 => ConsoleColor.DarkMagenta,
            1024 => ConsoleColor.Magenta,
            _ => ConsoleColor.Red
        };
    }
}