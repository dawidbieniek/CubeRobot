using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Robot;

internal class CubePresenter : IDisposable
{
    private readonly RobotCommunicator _communicator;
    private readonly RobotStateManager _stateManager;

    private int _currentFaceIndex = -1;

    public CubePresenter(RobotCommunicator communicator, RobotStateManager stateManager)
    {
        _communicator = communicator;
        _stateManager = stateManager;

        _communicator.CommandQueueChanged += OnCommunicatorCommandQueueChanged;
    }

    private void OnCommunicatorCommandQueueChanged(object sender, Events.CommandQueueEventArgs e)
    {
        if (e.RemainingCommands.Any())
            return;

        _stateManager.CurrentState = (RobotState)((int)_stateManager.CurrentState + 1);

        if (_stateManager.CurrentState == RobotState.ReadyForSolve)
            StopPresentation();
    }

    private CubeFace? CurrentFace => _currentFaceIndex > 0 ? Cube.CubeFaceStringConvertOrder[_currentFaceIndex] : null;

    public void Dispose()
    {
        // TODO: Dipose it
    }

    public void NextFace()
    {

    }

    public void StopPresentation()
    {
        _communicator.CommandQueueChanged -= OnCommunicatorCommandQueueChanged;
        // TODO: Fix cube orientation
        _stateManager.CurrentState = RobotState.ReadyForSolve;
    }
}