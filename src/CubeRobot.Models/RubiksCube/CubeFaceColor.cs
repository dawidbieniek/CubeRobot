namespace CubeRobot.Models.RubiksCube;

public enum CubeFaceColor
{
    None = -1,
    White = 0,
    Orange = 1,
    Green = 2,
    Yellow = 3,
    Red = 4,
    Blue = 5
}

public static class CubeFaceColorExtensions
{
    public static char ToFaceChar(this CubeFaceColor color) => color switch
    {
        CubeFaceColor.None => 'X',
        CubeFaceColor.White => 'F',
        CubeFaceColor.Orange => 'R',
        CubeFaceColor.Green => 'U',
        CubeFaceColor.Yellow => 'B',
        CubeFaceColor.Red => 'L',
        CubeFaceColor.Blue => 'D',
        _ => throw new ArgumentOutOfRangeException(nameof(color), $"Invalid CubeFaceColor - {color}"),
    };

    public static CubeFaceColor FromFaceChar(char faceChar) => faceChar switch
    {
        'X' => CubeFaceColor.None,
        'F' => CubeFaceColor.White,
        'R' => CubeFaceColor.Orange,
        'U' => CubeFaceColor.Green,
        'B' => CubeFaceColor.Yellow,
        'L' => CubeFaceColor.Red,
        'D' => CubeFaceColor.Blue,
        _ => throw new ArgumentException($"Invalid face character '{faceChar}'"),
    };
}