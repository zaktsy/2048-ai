namespace _2048_ai.GameEngine;

public class ConsoleColors : IDisposable
{
    public ConsoleColors(ConsoleColor fg, ConsoleColor bg = ConsoleColor.Black)
    {
        Console.ForegroundColor = fg;
        Console.BackgroundColor = bg;
    }

    public void Dispose()
    {
        Console.ResetColor();
    }
}