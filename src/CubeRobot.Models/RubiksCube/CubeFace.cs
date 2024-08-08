namespace CubeRobot.Models.RubiksCube;

public enum CubeFace
{
    Front,
    Right,
    Up,
    Back,
    Left,
    Down
}

public static class CubeFaceExtensions
{
    // Face: Left, Up, Right, Down
    public static CubeFace[] AdjacentFaces(this CubeFace face) => face switch
    {
        CubeFace.Front => [CubeFace.Left, CubeFace.Up, CubeFace.Right, CubeFace.Down],
        CubeFace.Right => [CubeFace.Front, CubeFace.Up, CubeFace.Back, CubeFace.Down],
        CubeFace.Up => [CubeFace.Left, CubeFace.Back, CubeFace.Right, CubeFace.Front],
        CubeFace.Back => [CubeFace.Right, CubeFace.Up, CubeFace.Left, CubeFace.Down],
        CubeFace.Left => [CubeFace.Back, CubeFace.Up, CubeFace.Front, CubeFace.Down],
        CubeFace.Down => [CubeFace.Left, CubeFace.Front, CubeFace.Right, CubeFace.Back],
        _ => throw new ArgumentOutOfRangeException(nameof(face), face, null),
    };

    /// <summary>
    /// Returns 4 functions that will translate current index (first argument) using last index
    /// (second argument) to indices of blocks on faces adjecent to the <paramref name="face"/>
    /// </summary>
    /// <returns>
    /// First value -&gt; y index.
    /// <para/>
    /// Second value -&gt; x index
    /// </returns>
    public static Func<int, int, (int y, int x)>[] AdjacentIterators(this CubeFace face) => face switch
    {
        CubeFace.Front => [RightIterator, DownIterator, LeftIterator, UpIterator],
        CubeFace.Right => [RightIterator, RightIterator, LeftIterator, RightIterator],
        CubeFace.Up => [UpIterator, UpIterator, UpIterator, UpIterator],
        CubeFace.Back => [RightIterator, UpIterator, LeftIterator, DownIterator],
        CubeFace.Left => [RightIterator, LeftIterator, LeftIterator, LeftIterator],
        CubeFace.Down => [DownIterator, DownIterator, DownIterator, DownIterator],
        _ => throw new ArgumentOutOfRangeException(nameof(face), face, null),
    };

    private static (int y, int x) UpIterator(int i, int lastIndex)
    {
        return (0, lastIndex - i);
    }

    private static (int y, int x) RightIterator(int i, int lastIndex)
    {
        return (lastIndex - i, lastIndex);
    }

    private static (int y, int x) DownIterator(int i, int lastIndex)
    {
        return (lastIndex, i);
    }

    private static (int y, int x) LeftIterator(int i, int lastIndex)
    {
        return (i, 0);
    }
}