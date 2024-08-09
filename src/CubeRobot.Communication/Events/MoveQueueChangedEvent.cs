using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Robot.Events;

public delegate void MoveQueueChangedEventHandler(object sender, MoveQueueEventArgs e);

public class MoveQueueEventArgs(CubeMove? finished, CubeMove[] remaining) : EventArgs
{
    public CubeMove? FinishedMove { get; private init; } = finished;
    public CubeMove[] RemainingMoves { get; private init; } = remaining;
}