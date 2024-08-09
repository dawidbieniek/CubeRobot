namespace CubeRobot.Models.RubiksCube;

// Move coding 0x<faceNumber><moveType><moveDirection>
// moveType: face (0), slice (1), rotation (2)
// moveDirection: clockwise (0), counterclockwise (1), double (2)
/// <summary>
/// Represents a Rubik's Cube move. Each move is encoded as a combination of face number, move type,
/// and move direction.
/// </summary>
public enum CubeMove
{
    // Face moves
    F = 0x000,
    FPrime = 0x001,
    F2 = 0x002,
    R = 0x100,
    RPrime = 0x101,
    R2 = 0x102,
    U = 0x200,
    UPrime = 0x201,
    U2 = 0x202,
    B = 0x300,
    BPrime = 0x301,
    B2 = 0x302,
    L = 0x400,
    LPrime = 0x401,
    L2 = 0x402,
    D = 0x500,
    DPrime = 0x501,
    D2 = 0x502,

    // Slice moves
    M = 0x010,
    MPrime = 0x011,
    M2 = 0x012,
    E = 0x110,
    EPrime = 0x111,
    E2 = 0x112,
    S = 0x210,
    SPrime = 0x211,
    S2 = 0x212,

    // Cube rotations
    X = 0x020,
    XPrime = 0x021,
    X2 = 0x022,
    Y = 0x120,
    YPrime = 0x121,
    Y2 = 0x122,
    Z = 0x220,
    ZPrime = 0x221,
    Z2 = 0x222
}

public enum CubeMoveType
{
    /// <summary>
    /// Move outer layer
    /// </summary>
    Face,
    /// <summary>
    /// Move middle layer (M, E, S)
    /// </summary>
    Slice,
    /// <summary>
    /// Rotate whole cube
    /// </summary>
    Rotation
}

public enum CubeMoveDirection
{
    Clockwise,
    CounterClockwise,
    Double
}

public static class CubeMoveExtensions
{
    public const string PrimeMoveModifierString = "Prime";
    public const string DoubleMoveModifierString = "2";

    private const int MoveTypeMask = 0x0f0;
    private const int MoveDirectionMask = 0x00f;
    private const int CubeFaceMask = 0xf00;

    public static CubeMoveType MoveType(this CubeMove move)
    {
        return (CubeMoveType)(((int)move & MoveTypeMask) >> 4);
    }

    public static CubeMoveDirection MoveDirection(this CubeMove move)
    {
        return (CubeMoveDirection)((int)move & MoveDirectionMask);
    }

    public static CubeFace CubeFace(this CubeMove move)
    {
        return (CubeFace)(((int)move & CubeFaceMask) >> 8);
    }
}