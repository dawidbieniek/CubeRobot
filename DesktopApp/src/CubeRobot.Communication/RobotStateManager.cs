using CubeRobot.Robot.Events;

namespace CubeRobot.Robot;

internal class RobotStateManager
{
    private RobotState _currentState = RobotState.Disconnected;

    public event RobotStateChangedEventHandler RobotStateChanged = delegate { };

    public RobotState CurrentState
    {
        get => _currentState;
        internal set
        {
            if (_currentState != value)
            {
                RobotState prev = _currentState;

                _currentState = value;

                RobotStateChanged.Invoke(this, new(prev, _currentState));
            }
        }
    }
}