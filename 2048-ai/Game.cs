using _2048_ai.Ai;
using _2048_ai.GameEngine;

namespace _2048_ai;

public class Game
{
    private readonly ManualResetEvent _exitEvent = new(false);
    private readonly GameGrid _gameGrid = new();
    private readonly DisplayHelper _displayHelper = new();
    private readonly DirectionDetector _directionDetector = new();
    
    private DirectionPredictor _directionPredictor;

    public Game()
    {
        _directionPredictor = new DirectionPredictor(_gameGrid);

        Console.CancelKeyPress += delegate(object? _, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _exitEvent.Set();
        };
    }

    public void Run()
    {
        var isAnythingMoved = false;

        Task.Factory.StartNew(() =>
        {
            _gameGrid.AddNewValues();
            _displayHelper.DisplayGameGrid(_gameGrid);

            do
            {
                if (!_gameGrid.CanMakeMove())
                {
                    FinishGame();
                    break;
                }

                //var direction = _directionDetector.DetectDirection();

                var direction = _directionPredictor.PredictDirection();

                if (direction != Direction.None)
                {
                    isAnythingMoved = _gameGrid.MakeMove(direction);
                }

                _displayHelper.DisplayGameGrid(_gameGrid);

                if (!_gameGrid.CanMakeMove())
                {
                    FinishGame();
                    break;
                }

                if (isAnythingMoved)
                {
                    _gameGrid.AddNewValues();
                    isAnythingMoved = false;
                }

                _displayHelper.DisplayGameGrid(_gameGrid);
            } while (true);
        });

        _exitEvent.WaitOne();

        Console.WriteLine("Exit...");
        Console.ReadKey(true);
    }

    private void FinishGame()
    {
        if (_gameGrid.Is2048Reached)
            _displayHelper.DisplayGameWin();
        else
            _displayHelper.DisplayGameLose();

        _exitEvent.Set();
    }
}