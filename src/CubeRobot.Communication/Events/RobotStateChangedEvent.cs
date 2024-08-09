namespace CubeRobot.Robot.Events;

public delegate void RobotStateChangedEventHandler(object sender, RobotStateEventArgs e);

public class RobotStateEventArgs(RobotState previous, RobotState current) : EventArgs
{
    public RobotState PreviousState { get; private init; } = previous;
    public RobotState CurrentState { get; private init; } = current;
}