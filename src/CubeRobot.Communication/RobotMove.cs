namespace CubeRobot.Robot;

public enum RobotMove
{
    Separator = ';',
    Stop = 's',
    RotateFrontClockwise = 'f',
    RotateFrontCounterClockwise = 'F',
    RotateRightClockwise = 'r',
    RotateRightCounterClockwise = 'R',
    RotateLeftClockwise = 'l',
    RotateLeftCounterClockwise = 'L',
    RotateBackClockwise = 'b',
    RotateBackCounterClockwise = 'B',
    MoveFrontForward = 'j',
    MoveFrontBackward = 'J',
    MoveRightForward = 'k',
    MoveRightBackward = 'K',
    MoveLeftForward = 'h',
    MoveLeftBackward = 'H',
    MoveBackForward = 'u',
    MoveBackBackward = 'U'
}

internal static class HighLevelRobotMove
{
    public static readonly RobotMove[] Grab = [RobotMove.MoveFrontForward, RobotMove.MoveRightForward, RobotMove.MoveLeftForward, RobotMove.MoveBackForward];
    public static readonly RobotMove[] Release = [RobotMove.MoveFrontBackward, RobotMove.MoveRightBackward, RobotMove.MoveLeftBackward, RobotMove.MoveBackBackward];

    public static readonly RobotMove[] RotateCubeX = [RobotMove.RotateRightClockwise, RobotMove.RotateLeftCounterClockwise];
    public static readonly RobotMove[] RotateCubeXPrime = [RobotMove.RotateRightCounterClockwise, RobotMove.RotateLeftClockwise];
    public static readonly RobotMove[] RotateCubeZ = [RobotMove.RotateFrontClockwise, RobotMove.RotateBackCounterClockwise];
    public static readonly RobotMove[] RotateCubeZPrime = [RobotMove.RotateFrontCounterClockwise, RobotMove.RotateBackClockwise];

    public static readonly RobotMove[] RotateFrontDouble = [RobotMove.RotateFrontClockwise, RobotMove.Separator, RobotMove.RotateFrontClockwise];
    public static readonly RobotMove[] RotateRightDouble = [RobotMove.RotateRightClockwise, RobotMove.Separator, RobotMove.RotateRightClockwise];
    public static readonly RobotMove[] RotateLeftDouble = [RobotMove.RotateLeftClockwise, RobotMove.Separator, RobotMove.RotateLeftClockwise];
    public static readonly RobotMove[] RotateBackDouble = [RobotMove.RotateBackClockwise, RobotMove.Separator, RobotMove.RotateBackClockwise];

    public static readonly RobotMove[] FixFrontArm = [RobotMove.MoveFrontBackward, RobotMove.Separator, RobotMove.RotateFrontClockwise, RobotMove.Separator, RobotMove.MoveFrontForward];
    public static readonly RobotMove[] FixRightArm = [RobotMove.MoveRightBackward, RobotMove.Separator, RobotMove.RotateRightClockwise, RobotMove.Separator, RobotMove.MoveRightForward];
    public static readonly RobotMove[] FixLeftArm = [RobotMove.MoveLeftBackward, RobotMove.Separator, RobotMove.RotateLeftClockwise, RobotMove.Separator, RobotMove.MoveLeftForward];
    public static readonly RobotMove[] FixBackArm = [RobotMove.MoveBackBackward, RobotMove.Separator, RobotMove.RotateBackClockwise, RobotMove.Separator, RobotMove.MoveBackForward];
}

public static class RobotMoveExtensions
{
    public static string ToProtocolString(this RobotMove move) => $"{(char)move}";

    public static string ToProtocolString(this IEnumerable<RobotMove> moves) => string.Concat(moves.Select(m => m.ToProtocolString()));
}