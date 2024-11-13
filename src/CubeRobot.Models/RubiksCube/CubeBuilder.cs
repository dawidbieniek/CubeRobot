namespace CubeRobot.Models.RubiksCube;

public class CubeBuilder(int size)
{
    private readonly int _size = size;
    private readonly CubeFaceColor[][,] _blocks = Enumerable.Range(0, Cube.NumberOfFaces).Select(_ => new CubeFaceColor[size, size]).ToArray();

    public CubeBuilder SetBlock(CubeFace face, int y, int x, CubeFaceColor color)
    {
        _blocks[(int)face][y, x] = color;
        return this;
    }

    public CubeBuilder SetFace(CubeFace face, CubeFaceColor[,]? colors)
    {
        if (colors is null)
        {
            SetFaceToBlanks(face);
            return this;
        }

        for (int y = 0; y < colors.GetLength(0); y++)
            for (int x = 0; x < colors.GetLength(1); x++)
            {
                _blocks[(int)face][y, x] = colors[y, x];
            }

        return this;
    }

    public Cube Build()
    {
        return new(_blocks, _size);
    }

    private void SetFaceToBlanks(CubeFace face)
    {
        for (int y = 0; y < _size; y++)
            for (int x = 0; x < _size; x++)
            {
                _blocks[(int)face][y, x] = CubeFaceColor.None;
            }
    }
}