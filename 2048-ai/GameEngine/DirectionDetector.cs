namespace _2048_ai.GameEngine;

public class DirectionDetector
{
    public Direction DetectDirection()
    {
        var directionDetection = false;

        Direction direction = Direction.None;

        while (!directionDetection)
        {
            var input = Console.ReadKey(true);

            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    direction = Direction.Up;
                    directionDetection = true;
                    break;

                case ConsoleKey.DownArrow:
                    direction = Direction.Down;
                    directionDetection = true;
                    break;

                case ConsoleKey.LeftArrow:
                    direction = Direction.Left;
                    directionDetection = true;
                    break;

                case ConsoleKey.RightArrow:
                    direction = Direction.Right;
                    directionDetection = true;
                    break;
            }
        }

        return direction;
    }
}