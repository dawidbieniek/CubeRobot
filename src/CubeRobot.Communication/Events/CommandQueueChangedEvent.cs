namespace CubeRobot.Robot.Events;

public delegate void CommandQueueChangedEventHandler(object sender, CommandQueueEventArgs e);

public class CommandQueueEventArgs(RobotMove[] finished, RobotMove[] remaining) : EventArgs
{
    public RobotMove[] FinishedCommands { get; private init; } = finished;
    public RobotMove[] RemainingCommands { get; private init; } = remaining;
}