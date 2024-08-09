namespace CubeRobot.Robot;

internal enum Direction
{
    Front = 0,
    Right = 1,
    Left = 2,
    Back = 3
}

internal static class DirectionExtensions
{
    public static Direction Right(this Direction direction)
    {
        return direction switch
        {
            Direction.Front => Direction.Right,
            Direction.Right => Direction.Back,
            Direction.Back => Direction.Left,
            Direction.Left => Direction.Front,
            _ => throw new ArgumentException("Illegal value"),
        };
    }

    public static Direction Left(this Direction direction)
    {
        return direction switch
        {
            Direction.Front => Direction.Left,
            Direction.Left => Direction.Back,
            Direction.Back => Direction.Right,
            Direction.Right => Direction.Front,
            _ => throw new ArgumentException("Illegal value"),
        };
    }

    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.Front => Direction.Back,
            Direction.Right => Direction.Left,
            Direction.Left => Direction.Right,
            Direction.Back => Direction.Front,
            _ => throw new ArgumentException("Illegal value"),
        };
    }

    public static RobotMove[] FixArmCommand(this Direction direction)
    {
        return direction switch
        {
            Direction.Front => HighLevelRobotMove.FixFrontArm,
            Direction.Right => HighLevelRobotMove.FixRightArm,
            Direction.Left => HighLevelRobotMove.FixLeftArm,
            Direction.Back => HighLevelRobotMove.FixBackArm,
            _ => throw new ArgumentException("Illegal value")
        };
    }

    public static RobotMove MoveArmCloseCommand(this Direction direction)
    {
        return direction switch
        {
            Direction.Front => RobotMove.MoveFrontForward,
            Direction.Right => RobotMove.MoveRightForward,
            Direction.Left => RobotMove.MoveLeftForward,
            Direction.Back => RobotMove.MoveBackForward,
            _ => throw new ArgumentException("Illegal value")
        };
    }
}