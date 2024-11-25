using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;
using CubeRobot.Robot.Helpers;

namespace CubeRobot.Robot;

public class Robot : IRobot
{
    private readonly RobotStateManager _stateManager = new();
    private readonly CubeMoveProcessor _moveProcessor = new();
    private CubePresenter? _cubePresenter;
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

        CommandQueueChangedEventHandler onCommandQueueChanged = delegate { };
        
        onCommandQueueChanged = (s, e) => 
        {
            if (e.RemainingCommands.Any())
                return;

            _communicator.CommandQueueChanged -= onCommandQueueChanged;
            _moveProcessor.SetMovers(MoverState.Close, MoverState.Close, MoverState.Close, MoverState.Close);
            CurrentState = RobotState.ReadyForPhotos;
        };

        _communicator.CommandQueueChanged += onCommandQueueChanged;
    }

    public void StartPresenting()
    {
        CheckIfCommunicatorInitialized();
        _cubePresenter = new(_communicator!, _stateManager);

        _cubePresenter.NextFace();
    }

    public void NextPresentationStep()
    {
        CheckIfCommunicatorInitialized();
        _cubePresenter?.NextFace();
    }

    public void SkipState()
    {
        switch (CurrentState)
        {
            case RobotState.NoCube:
                CurrentState = RobotState.ReadyForPhotos;
                break;
            case RobotState.ReadyForPhotos:
            case RobotState.Presenting1:
            case RobotState.Presenting2:
            case RobotState.Presenting3:
            case RobotState.Presenting4:
            case RobotState.Presenting5:
            case RobotState.Presenting6:
                if (_cubePresenter is not null)
                    _cubePresenter.StopPresentation();
                else
                    CurrentState = RobotState.ReadyForSolve;
                break;
            case RobotState.ReadyForSolve:
                CurrentState = RobotState.ReadyForRelease;
                break;
            case RobotState.Solving:
                // TODO: Stop solving
                CurrentState = RobotState.ReadyForRelease;
                break;
            case RobotState.ReadyForRelease:
                CurrentState = RobotState.NoCube;
                break;
        }
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