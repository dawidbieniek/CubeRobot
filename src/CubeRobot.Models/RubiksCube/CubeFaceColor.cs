namespace CubeRobot.Models.RubiksCube;

public enum CubeFaceColor
{
    None = -1,
    White = 0,
    Red = 1,
    Green = 2,
    Yellow = 3,
    Orange = 4,
    Blue = 5
}

public static class CubeFaceColorExtensions
{
    public static char ToFaceChar(this CubeFaceColor color) => color switch
    {
        CubeFaceColor.None => 'X',
        CubeFaceColor.White => 'F',
        CubeFaceColor.Red => 'L',
        CubeFaceColor.Green => 'U',
        CubeFaceColor.Yellow => 'B',
        CubeFaceColor.Orange => 'R',
        CubeFaceColor.Blue => 'D',
        _ => throw new ArgumentOutOfRangeException(nameof(color), $"Invalid {nameof(CubeFaceColor)}"),
    };
}