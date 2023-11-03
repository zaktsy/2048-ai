using _2048_ai.GameEngine;

namespace _2048_ai.Ai;

public class DirectionPredictor
{
    private readonly int[,] _model1 = { { 16, 15, 14, 13 }, { 9, 10, 11, 12 }, { 8, 7, 6, 5 }, { 1, 2, 3, 4 }, };
    private readonly int[,] _model2 = { { 16, 15, 12, 4 }, { 14, 13, 11, 3 }, { 10, 9, 8, 2 }, { 7, 6, 5, 1 }, };
    private readonly int[,] _model3 = { { 16, 15, 14, 4 }, { 13, 12, 11, 3 }, { 10, 9, 8, 2 }, { 7, 6, 5, 1 }, };

    private readonly Dictionary<int, float> _expectationRate = new() { { 2, 0.9f }, { 4, 0.1f } };

    private readonly GameGrid _grid;

    private readonly List<Direction> _directions = new()
        { Direction.Up, Direction.Left, Direction.Down, Direction.Right };

    private bool _active;


    public DirectionPredictor(GameGrid grid)
    {
        _grid = grid;
    }

    public Direction PredictDirection()
    {
        var bestDirection = Direction.None;
        var bestScore = -1f;

        var searchDepth = GetDepth();
        foreach (var direction in _directions)
        {
            var tilesClone = (ulong[,])_grid.Tiles.Clone();

            if (!_grid.MakeMove(direction, tilesClone))
                continue;

            _active = false;
            var newScore = ExpectSearch(searchDepth, tilesClone);

            if (!(newScore > bestScore))
                continue;

            bestDirection = direction;
            bestScore = newScore;
        }

        return bestDirection;
    }

    private int GetDepth()
    {
        var max = _grid.Max;

        return max switch
        {
            >= 2048 => 6,
            >= 1024 => 5,
            _ => 4
        };
    }

    private float ExpectSearch(int searchDepth, ulong[,] tilesClone)
    {
        if (searchDepth == 0)
            return Score(tilesClone);

        var score = 0f;

        if (_active)
        {
            foreach (var direction in _directions)
            {
                var newTilesClone = (ulong[,])tilesClone.Clone();

                if (!_grid.MakeMove(direction, newTilesClone))
                    continue;

                _active = false;
                var newScore = ExpectSearch(searchDepth - 1, newTilesClone);
                if (newScore > score)
                    score = newScore;
            }
        }
        else
        {
            var zeroPoints = GetZeroPoints(tilesClone).ToArray();

            foreach (var kv in _expectationRate)
            {
                foreach (var point in zeroPoints)
                {
                    var newTilesClone = (ulong[,])tilesClone.Clone();
                    newTilesClone[point.Item1, point.Item2] = (ulong)kv.Key;

                    _active = true;
                    var newScore = ExpectSearch(searchDepth - 1, newTilesClone);
                    score += newScore * kv.Value;
                }
            }

            score /= zeroPoints.Length;
        }

        return score;
    }

    private IEnumerable<Tuple<int, int>> GetZeroPoints(ulong[,] tilesClone)
    {
        for (var x = 0; x < 4; x++)
        {
            for (var y = 0; y < 4; y++)
            {
                var value = tilesClone[x, y];
                if (value == 0)
                    yield return new Tuple<int, int>(x, y);
            }
        }
    }

    private float Score(ulong[,] tilesClone)
    {
        var result = new ulong[24];

        for (var x = 0; x < 4; x++)
        {
            for (var y = 0; y < 4; y++)
            {
                var value = tilesClone[x, y];
                if (value != 0)
                {
                    ModelScore(0, x, y, value, _model1, result);
                    ModelScore(0, x, y, value, _model2, result);
                    ModelScore(0, x, y, value, _model3, result);
                }
            }
        }

        return result.Max();
    }

    private void ModelScore(int index, int x, int y, ulong value, int[,] model, ulong[] result)
    {
        var start = index * 8;
        result[start] += value * (ulong)model[x, y];
        result[start + 1] += value * (ulong)model[x, 3 - y];

        result[start + 2] += value * (ulong)model[y, x];
        result[start + 3] += value * (ulong)model[3 - y, x];

        result[start + 4] += value * (ulong)model[3 - x, 3 - y];
        result[start + 5] += value * (ulong)model[3 - x, y];

        result[start + 6] += value * (ulong)model[y, 3 - x];
        result[start + 7] += value * (ulong)model[3 - y, 3 - x];
    }
}