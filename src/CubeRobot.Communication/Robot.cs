using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;
using CubeRobot.Robot.Helpers;

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
    public event RobotEffectorsStateChangedEventHandler RobotEffectorsStateChanged
    {
        add => _moveProcessor.RobotEffectorsStateChanged += value;
        remove => _moveProcessor.RobotEffectorsStateChanged -= value;
    }
    public event CommandQueueChangedEventHandler CommandQueueChanged = delegate { };
    public event MoveQueueChangedEventHandler MoveQueueChanged = delegate { };
    public event EventHandler ConnectionEstablished = delegate {};
    public event EventHandler ConnectionEstablishmentFailed = delegate {};
    public RobotState CurrentState
    {
        get => _stateManager.CurrentState;
        private set => _stateManager.CurrentState = value;
    }

    public void ConfigureCommunicationChannel(CommunicationChannelBase channel)
    {
        _communicator?.Dispose();
        _communicator = new(channel);
        _communicator.MoveQueueChanged += MoveQueueChanged;
        _communicator.CommandQueueChanged += CommandQueueChanged;
        _communicator.CommunicationEstablished += (s, e) => { CurrentState = RobotState.NoCube; ConnectionEstablished?.Invoke(this, EventArgs.Empty); };
        _communicator.CommunicationEstablishmentFailed += (s, e) => { CurrentState = RobotState.Disconnected; ConnectionEstablishmentFailed?.Invoke(this, EventArgs.Empty); };
    }

    public void GrabCube()
    {
        CheckIfCommunicatorInitialized();

        _communicator!.SendMovesToRobot([.. HighLevelRobotMove.Grab, RobotMove.Separator]);
        _moveProcessor.SetMovers(MoverState.Close, MoverState.Close, MoverState.Close, MoverState.Close);

        CurrentState = RobotState.ReadyForPhotos;
    }

    public void PresentCube(CubeFace face)
    {
        CheckIfCommunicatorInitialized();

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

    public void SolveCube(IEnumerable<CubeMove> moves)
    {
        CheckIfCommunicatorInitialized();

        CurrentState = RobotState.Solving;
        PerformCubeMoves(moves);
    }

    public void ReleaseCube()
    {
        CheckIfCommunicatorInitialized();

        _communicator!.SendMovesToRobot([.. HighLevelRobotMove.Release, RobotMove.Separator]);

        _moveProcessor.SetMovers(MoverState.Far, MoverState.Far, MoverState.Far, MoverState.Far);

        CurrentState = RobotState.NoCube;
    }

    public void Dispose()
    {
        _communicator?.Dispose();
    }

    private void CheckIfCommunicatorInitialized()
    {
        if (_communicator is null)
            throw new InvalidOperationException("Communication channel must be first configured");
    }

    private void PerformCubeMoves(IEnumerable<CubeMove> moves)
    {
        IEnumerable<RobotMove> robotMoves = _moveProcessor.ProcessMoves(moves, out Queue<MutablePair<CubeMove, int>> cubeMovesLeft);
        _communicator!.SendMovesToRobot(robotMoves, cubeMovesLeft);
    }
}