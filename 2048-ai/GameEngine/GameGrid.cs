namespace _2048_ai.GameEngine;

public class GameGrid
{
    private const byte FieldSize = 4;
    private readonly Random _random = new();

    public ulong[,] Tiles { get; init; } = new ulong[FieldSize, FieldSize];

    public bool Is2048Reached { get; private set; } = false;

    public ulong Max
    {
        get
        {
            ulong max = 0;

            for (var x = 0; x < FieldSize; x++)
            {
                for (var y = 0; y < FieldSize; y++)
                {
                    if (Tiles[x, y] > max)
                        max = Tiles[x, y];
                }
            }

            return max;
        }
    }

    public bool CanMakeMove()
    {
        foreach (var direction in new[] { Direction.Down, Direction.Up, Direction.Left, Direction.Right })
        {
            var tilesClone = (ulong[,])Tiles.Clone();

            if (MakeMove(direction, tilesClone))
                return true;
        }

        return false;
    }

    public void AddNewValues()
    {
        AddNewValues(Tiles);
    }

    public void AddNewValues(ulong[,] tiles)
    {
        var count = _random.Next(1, 2);

        for (var i = 0; i < count; i++)
        {
            var nRows = tiles.GetLength(0);
            var nCols = tiles.GetLength(1);

            var emptySlots = new List<Tuple<int, int>>();
            for (var iRow = 0; iRow < nRows; iRow++)
            {
                for (var iCol = 0; iCol < nCols; iCol++)
                {
                    if (tiles[iRow, iCol] == 0)
                        emptySlots.Add(new Tuple<int, int>(iRow, iCol));
                }
            }

            var iSlot = _random.Next(0, emptySlots.Count);
            var value = _random.Next(0, 100) < 90 ? 2 : 4;

            if (emptySlots.Any())
                tiles[emptySlots[iSlot].Item1, emptySlots[iSlot].Item2] = (ulong)value;
        }
    }

    public bool MakeMove(Direction direction)
    {
        return MakeMove(direction, Tiles);
    }

    public bool MakeMove(Direction direction, ulong[,] tiles)
    {
        return direction switch
        {
            Direction.Up => ShiftByColumns(direction, tiles),
            Direction.Down => ShiftByColumns(direction, tiles),
            Direction.Right => ShiftByRows(direction, tiles),
            Direction.Left => ShiftByRows(direction, tiles),
            Direction.None => false,
            _ => false
        };
    }

    private bool ShiftByRows(Direction direction, ulong[,] tiles)
    {
        var isAnythingMoved = false;

        for (var i = 0; i < FieldSize; i++)
        {
            var row = GetRow(i, tiles);

            if (direction == Direction.Right)
                Array.Reverse(row);

            var shiftedRow = ShiftArray(row);

            var rowChanged = !row.SequenceEqual(shiftedRow);

            if (!isAnythingMoved)
                isAnythingMoved = rowChanged;

            if (direction == Direction.Right)
                Array.Reverse(shiftedRow);

            SetRow(i, shiftedRow, tiles);
        }

        return isAnythingMoved;
    }

    private ulong[] GetRow(int columnIndex, ulong[,] tiles)
    {
        var row = new ulong[FieldSize];

        for (var i = 0; i < FieldSize; i++)
            row[i] = tiles[columnIndex, i];

        return row;
    }

    private void SetRow(int index, ulong[] row, ulong[,] tiles)
    {
        for (var i = 0; i < FieldSize; i++)
            tiles[index, i] = row[i];
    }

    private bool ShiftByColumns(Direction direction, ulong[,] tiles)
    {
        var isAnythingMoved = false;

        for (var i = 0; i < FieldSize; i++)
        {
            var column = GetColumn(i, tiles);

            if (direction == Direction.Down)
                Array.Reverse(column);

            var shiftedColumn = ShiftArray(column);

            var columnChanged = !column.SequenceEqual(shiftedColumn);

            if (!isAnythingMoved)
                isAnythingMoved = columnChanged;

            if (direction == Direction.Down)
                Array.Reverse(shiftedColumn);

            SetColumn(i, shiftedColumn, tiles);
        }

        return isAnythingMoved;
    }

    private ulong[] GetColumn(int rowIndex, ulong[,] tiles)
    {
        var column = new ulong[FieldSize];

        for (var i = 0; i < FieldSize; i++)
            column[i] = tiles[i, rowIndex];

        return column;
    }

    private void SetColumn(int index, ulong[] column, ulong[,] tiles)
    {
        for (var i = 0; i < FieldSize; i++)
            tiles[i, index] = column[i];
    }

    private ulong[] ShiftArray(ulong[] array)
    {
        var resultArray = new ulong[FieldSize];
        var arrayWithoutZeroes = new ulong[FieldSize];

        var q = 0;

        foreach (var arrayElement in array)
        {
            if (arrayElement != 0)
                arrayWithoutZeroes[q++] = arrayElement;
        }

        var i = 0;
        q = 0;

        while (i < arrayWithoutZeroes.Length)
        {
            var canMergeTiles = i + 1 < arrayWithoutZeroes.Length
                                && arrayWithoutZeroes[i] == arrayWithoutZeroes[i + 1]
                                && arrayWithoutZeroes[i] != 0;

            if (canMergeTiles)
                resultArray[q] = arrayWithoutZeroes[i++] * 2;
            else
                resultArray[q] = arrayWithoutZeroes[i];

            i++;
            q++;
        }

        return resultArray;
    }
}