using System.Text;

namespace CubeRobot.Models.RubiksCube;

public class Cube
{
    public const int NumberOfFaces = 6;
    public static readonly CubeFace[] CubeFaceStringConvertOrder = [CubeFace.Up, CubeFace.Right, CubeFace.Front, CubeFace.Down, CubeFace.Left, CubeFace.Back];

    private readonly CubeRotationHelper _rotationHelper;

    private readonly CubeFaceColor[][,] _blocks;

    public Cube(CubeFaceColor[][,] blocks, int size = 3)
    {
        Size = size;
        _blocks = blocks;

        _rotationHelper = new(size, _blocks);
    }

    public Cube(int size = 3, bool initializeColors = false) : this(Enumerable.Range(0, NumberOfFaces).Select(_ => new CubeFaceColor[size, size]).ToArray(), size)
    {
        if (initializeColors)
            InitializeColors();
        else
            InitializeGrey();
    }

    public int Size { get; }

    /// <summary>
    /// Returns true if cube doesn't contain any <see cref="CubeFaceColor.None"/>
    /// </summary>
    public bool IsFullyColored
    {
        get
        {
            foreach (CubeFaceColor[,] face in _blocks)
            {
                foreach (CubeFaceColor block in face)
                {
                    if (block == CubeFaceColor.None)
                        return false;
                }
            }
            return true;
        }
    }

    public CubeFaceColor this[CubeFace face, int y, int x] => _blocks[(int)face][y, x];

    public CubeFaceColor[,] this[CubeFace face] => _blocks[(int)face];

    public static Cube FromConfigurationString(string configuration, int cubeSize = 3)
    {
        if (configuration.Length != NumberOfFaces * cubeSize * cubeSize)
            throw new ArgumentException($"Configuration have incorrect length: {configuration.Length} (should be {NumberOfFaces * cubeSize * cubeSize})");

        Cube cube = new(cubeSize);

        int i = 0;
        foreach (CubeFace face in CubeFaceStringConvertOrder)
        {
            for (int y = 0; y < cubeSize; y++)
            {
                for (int x = 0; x < cubeSize; x++)
                {
                    CubeFaceColor color = CubeFaceColorExtensions.FromFaceChar(configuration[i++]);
                    cube._blocks[(int)face][y, x] = color;
                }
            }
        }

        return cube;
    }

    public void PerformMove(CubeMove move)
    {
        switch (move.MoveType())
        {
            case CubeMoveType.Face:
                switch (move.MoveDirection())
                {
                    case CubeMoveDirection.Clockwise:
                        _rotationHelper.RotateFaceClockwise(move.CubeFace());
                        break;

                    case CubeMoveDirection.CounterClockwise:
                        _rotationHelper.RotateFaceCounterclockwise(move.CubeFace());
                        break;

                    case CubeMoveDirection.Double:
                        _rotationHelper.RotateFaceClockwise(move.CubeFace());
                        _rotationHelper.RotateFaceClockwise(move.CubeFace());
                        break;
                }
                break;

            case CubeMoveType.Slice:
                throw new NotImplementedException();

            case CubeMoveType.Rotation:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns string representation of cube based on <see href="https://github.com/Megalomatt/Kociemba?tab=readme-ov-file#preparing-the-search-string"/>
    /// </summary>
    /// <returns> </returns>
    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (CubeFace face in CubeFaceStringConvertOrder)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    sb.Append(this[face, y, x].ToFaceChar());
                }
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Sets colors of cube to standard color scheme
    /// </summary>
    private void InitializeColors()
    {
        // Initialize each face with the correct color
        for (int face = 0; face < NumberOfFaces; face++)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _blocks[face][i, j] = (CubeFaceColor)face;
                }
            }
        }
    }

    /// <summary>
    /// Sets colors of cube to <see cref="CubeFaceColor.None"/>
    /// </summary>
    private void InitializeGrey()
    {
        for (int face = 0; face < NumberOfFaces; face++)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _blocks[face][i, j] = CubeFaceColor.None;
                }
            }
        }
    }
}