namespace CubeRobot.Robot.Events;

public delegate void CommandQueueChangedEventHandler(object sender, CommandQueueEventArgs e);

public class CommandQueueEventArgs(IEnumerable<RobotMove> finished, IEnumerable<RobotMove> remaining) : EventArgs
{
    public IEnumerable<RobotMove> FinishedCommands { get; private init; } = finished;
    public IEnumerable<RobotMove> RemainingCommands { get; private init; } = remaining;
}