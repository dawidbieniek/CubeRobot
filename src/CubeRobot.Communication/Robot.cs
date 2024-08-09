using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;

namespace CubeRobot.Robot;

public class Robot : IRobot
{
    private readonly RobotStateManager _stateManager = new();
    private readonly CubeMoveProcessor _moveProcessor = new();
    private RobotCommunicator? _communicator;

    public event RobotStateChangedEventHandler RobotStateChanged
    {
        add => _stateManager.RobotStateChanged += value;
        remove => _stateManager.RobotStateChanged -= value;
    }
    public event CommandQueueChangedEventHandler CommandQueueChanged
    {
        add => (_communicator ?? throw new InvalidOperationException("Communication channel must be first configured")).CommandQueueChanged += value;
        remove => (_communicator ?? throw new InvalidOperationException("Communication channel must be first configured")).CommandQueueChanged -= value;
    }
    public event MoveQueueChangedEventHandler MoveQueueChanged
    {
        add => (_communicator ?? throw new InvalidOperationException("Communication channel must be first configured")).MoveQueueChanged += value;
        remove => (_communicator ?? throw new InvalidOperationException("Communication channel must be first configured")).MoveQueueChanged -= value;
    }

    public RobotState CurrentState
    {
        get => _stateManager.CurrentState;
        private set => _stateManager.CurrentState = value;
    }

    public void ConfigureCommunicationChannel(CommunicationChannelBase channel)
    {
        _communicator?.Dispose();
        _communicator = new(channel);

        CurrentState = RobotState.NoCube;
    }

    public void GrabCube()
    {
        if (_communicator is null)
            throw new InvalidOperationException("Communication channel must be first configured");

        _communicator.SendMovesToRobot([.. HighLevelRobotMove.Grab, RobotMove.Separator]);

        _moveProcessor.SetMovers(MoverState.Close, MoverState.Close, MoverState.Close, MoverState.Close);

        CurrentState = RobotState.ReadyForPhotos;
    }

    public void PresentCube(CubeFace face)
    {
        if (_communicator is null)
            throw new InvalidOperationException("Communication channel must be first configured");

        throw new NotImplementedException();
        //switch (face)
        //{
        //	case CubeFace.Up:
        //		CurrentState = RobotState.Presenting1;
        //		break;

        // case CubeFace.Front: CurrentState = RobotState.Presenting2; break;

        // case CubeFace.Down: CurrentState = RobotState.Presenting3; break;

        // case CubeFace.Back: CurrentState = RobotState.Presenting4; break;

        // case CubeFace.Left: CurrentState = RobotState.Presenting5; break;

        //	case CubeFace.Right:
        //		CurrentState = RobotState.Presenting6;
        //		break;
        //}
    }

    public void StopPresenting()
    {
        throw new NotImplementedException();
        // TODO: Move cube to initial position
    }

    public void SolveCube(params CubeMove[] moves)
    {
        CurrentState = RobotState.Solving;
        PerformCubeMoves(moves);
    }

    public void ReleaseCube()
    {
        if (_communicator is null)
            throw new InvalidOperationException("Communication channel must be first configured");

        _communicator.SendMovesToRobot([.. HighLevelRobotMove.Release, RobotMove.Separator]);

        _moveProcessor.SetMovers(MoverState.Far, MoverState.Far, MoverState.Far, MoverState.Far);

        CurrentState = RobotState.NoCube;
    }

    public void Dispose()
    {
        _communicator?.Dispose();

        GC.SuppressFinalize(this);
    }

    private void PerformCubeMoves(params CubeMove[] moves)
    {
        if (_communicator is null)
            throw new InvalidOperationException("Communication channel must be first configured");

        _communicator.SendMovesToRobot([.. _moveProcessor.ProcessMoves(moves, out List<(CubeMove, int)> movesLeft)]);
        _communicator.MovesLeftQueue = movesLeft;
    }
}